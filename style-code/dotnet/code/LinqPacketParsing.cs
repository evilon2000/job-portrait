using System;
using System.Linq;

namespace PersonalCodeWiki.AlpacaLink
{
    // Extracted from GPRSServer/GPRSProtol.cs and HHUTcp/HHUProtocol.cs.
    // This snippet is kept because LINQ is used to express packet structure directly.
    public static class LinqPacketParsing
    {
        private const int HeadLen = 1 * 2;
        private const int DeviceTypeLen = 1 * 2;
        private const int AddressLen = 7 * 2;
        private const int CmdLen = 2 * 2;
        private const int SerLen = 1 * 2;
        private const int DataLen = 2 * 2;
        private const int CrcLen = 2 * 2;
        private const int ReservedLen = 7 * 2;
        private const int Cj188Len = 1 * 2;
        private const string Head = "7E";
        private const string End = "16";

        public static string GetCj188Data(string data)
        {
            var payloadLen = int.Parse(
                new string(data.Skip(HeadLen).Skip(DeviceTypeLen).Skip(AddressLen).Skip(CmdLen).Skip(SerLen).Take(DataLen).ToArray()),
                System.Globalization.NumberStyles.HexNumber);

            var gprsPayload = new string(
                data.Skip(HeadLen).Skip(DeviceTypeLen).Skip(AddressLen).Skip(CmdLen).Skip(SerLen).Skip(DataLen).Take(payloadLen * 2).ToArray());

            var cj188Length = int.Parse(
                new string(gprsPayload.Skip(ReservedLen).Take(Cj188Len).ToArray()),
                System.Globalization.NumberStyles.HexNumber);

            return new string(gprsPayload.Skip(ReservedLen).Skip(Cj188Len).Take(cj188Length * 2).ToArray());
        }

        public static bool CheckFrameData(string data, bool checkCj188)
        {
            if (!data.StartsWith(Head))
            {
                return false;
            }

            var payloadLen = int.Parse(
                new string(data.Skip(HeadLen).Skip(DeviceTypeLen).Skip(AddressLen).Skip(CmdLen).Skip(SerLen).Take(DataLen).ToArray()),
                System.Globalization.NumberStyles.HexNumber);

            var waitForVerifier = new string(
                data.Take(HeadLen + DeviceTypeLen + AddressLen + CmdLen + SerLen + DataLen + payloadLen * 2).ToArray());

            var sum = 0;
            for (var i = 0; i < waitForVerifier.Length; i += 2)
            {
                sum += int.Parse(waitForVerifier.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
            }

            var expectedCrc = Convert.ToString(sum, 16).ToUpper();
            expectedCrc = expectedCrc.Length <= 4
                ? expectedCrc.PadLeft(4, '0')
                : expectedCrc.Substring(expectedCrc.Length - 4, 4);

            var crcData = new string(
                data.Skip(HeadLen).Skip(DeviceTypeLen).Skip(AddressLen).Skip(CmdLen).Skip(SerLen).Skip(DataLen).Skip(payloadLen * 2).Take(CrcLen).ToArray());

            var endData = new string(
                data.Skip(HeadLen).Skip(DeviceTypeLen).Skip(AddressLen).Skip(CmdLen).Skip(SerLen).Skip(DataLen).Skip(payloadLen * 2).Skip(CrcLen).Take(2).ToArray());

            if (endData != End || expectedCrc != crcData)
            {
                return false;
            }

            if (!checkCj188)
            {
                return true;
            }

            var cj188Data = GetCj188Data(data);
            return cj188Data.StartsWith("68") && cj188Data.EndsWith("16");
        }

        public static string ReadFrameByPayloadLength(string data)
        {
            var payloadLen = int.Parse(
                new string(data.Skip(HeadLen).Skip(DeviceTypeLen).Skip(AddressLen).Skip(CmdLen).Skip(SerLen).Take(DataLen).ToArray()),
                System.Globalization.NumberStyles.HexNumber);

            return new string(
                data.Take(HeadLen + DeviceTypeLen + AddressLen + CmdLen + SerLen + DataLen + payloadLen * 2 + CrcLen + 2).ToArray());
        }
    }
}
