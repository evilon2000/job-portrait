using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalCodeWiki.BoveMES
{
    // Adapted from Frm_Test: keep the workflow skeleton, not the WinForms details.
    public class TestWorkflowOrchestrator
    {
        private readonly IController _controller;
        private readonly ProtocolService _protocolService;
        private readonly QCService _qcService;

        public TestWorkflowOrchestrator(
            IController controller,
            ProtocolService protocolService,
            QCService qcService)
        {
            _controller = controller;
            _protocolService = protocolService;
            _qcService = qcService;
        }

        public void Run(InstructSet instructSet, Products product, string lotNo, int timeout)
        {
            var instructList = _protocolService
                .GetInstructDetail(instructSet.InstructSetId)
                .OrderBy(x => x.order)
                .ToList();

            var resultDic = new Dictionary<string, string>();
            var protocolId = instructList
                .Where(x => x.ProtocolId != null && x.ProtocolId != 0)
                .FirstOrDefault()
                ?.ProtocolId ?? 0;

            PrepareProtocolContext(protocolId, instructList);

            _controller.StartBatchModle(timeout);
            try
            {
                if (!AutoSendCommand(instructList, resultDic))
                {
                    return;
                }

                ValidateResult(resultDic, protocolId, lotNo);

                var meterId = resultDic.ContainsKey("ReadMeterId")
                    ? resultDic["ReadMeterId"]
                    : null;

                var deviceId = GetDeviceId(protocolId, resultDic);
                SaveTestRecord(resultDic, meterId, deviceId, instructSet.InstructSetId, product, lotNo);
            }
            finally
            {
                _controller.StopBatchModle();
            }
        }

        private static void PrepareProtocolContext(int protocolId, IEnumerable<InstructSetDTO> instructList)
        {
            if (protocolId != 2)
            {
                return;
            }

            MesCache.SigFoxMap = new Dictionary<string, string>();

            var sigfoxIdCmd = instructList
                .Where(x => x.InstructContent.Equals("SIGFOX-AT$I=10", StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            var rateCmd = instructList
                .Where(x => x.InstructContent.Equals("SIGFOX-AT$DR?", StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (sigfoxIdCmd is null || rateCmd is null)
            {
                throw new InvalidOperationException("Sigfox test flow is missing required commands.");
            }

            MesCache.SigFoxMap.Add("DeviceId", sigfoxIdCmd.InstructName);
            MesCache.SigFoxMap.Add("Rate", rateCmd.InstructName);
        }

        private bool AutoSendCommand(IEnumerable<InstructSetDTO> instructList, Dictionary<string, string> resultDic)
        {
            foreach (var item in instructList)
            {
                var protocolHandler = HandlerProvider.GetProtocolHandler(_controller, item.ProtocolId ?? 0);
                var result = protocolHandler.SetInstruct(item);
                if (result is null || result.Contains("ERROR"))
                {
                    return false;
                }

                result = protocolHandler.FilterATRecived(result);
                var dicPair = protocolHandler.RecordResult(item, result);
                resultDic.Add(dicPair.Key, dicPair.Value);
            }

            return true;
        }

        private static void ValidateResult(Dictionary<string, string> resultDic, int protocolId, string lotNo)
        {
            var verify = VerifyFactory.CreateResultVerifyInstance(protocolId, lotNo);
            verify.VerifyResult(resultDic);
        }

        private string GetDeviceId(int protocolId, Dictionary<string, string> resultDic)
        {
            var protocolHandler = HandlerProvider.GetProtocolHandler(_controller, protocolId);
            return protocolHandler.GetDeviceId(resultDic);
        }

        private void SaveTestRecord(
            Dictionary<string, string> resultDic,
            string meterId,
            string deviceId,
            int setId,
            Products product,
            string lotNo)
        {
            _qcService.SaveOrUpdateDevice(new Devices
            {
                DeviceId = deviceId,
                ProductId = product.ProductId,
                LotNo = lotNo,
                OnTest = true,
                MeterId = meterId,
                SetId = setId,
                OnAMI = resultDic.ContainsKey("remote")
            });
        }
    }
}
