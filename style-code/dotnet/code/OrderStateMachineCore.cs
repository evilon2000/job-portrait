using System;
using System.Collections.Generic;

namespace PersonalCodeWiki.AutchonBackend
{
    public abstract class OrderChangeHandler
    {
        public abstract void Request(StateType type);
        public abstract void SetState(OrderState state);
        public abstract List<int> GetCouldManageStauts(OrderState state);
    }

    public abstract class OrderState
    {
        public Dictionary<StateType, int[]> StateMachine = new Dictionary<StateType, int[]>();
        public OrderChangeHandler Handler;

        protected OrderState(OrderChangeHandler handler)
        {
            Handler = handler;
            StateMachine.Add(StateType.None, new[] { 0, 0, 0, 0 });
        }

        public abstract void Handle(StateType stateType);
        public abstract List<int> GetCouldManageState();
        protected abstract void NotifyWarehouseHandleOrder(OrderChangeHandler handler, string queueName, bool isPayed);
    }

    public class CommonOrderChangeHandler : OrderChangeHandler
    {
        private OrderState _state;
        public OrderForSave OrderForSave;

        public CommonOrderChangeHandler(OrderForSave order)
        {
            _state = new CommonOrderNone(this);
            OrderForSave = order;
        }

        public void SaveOrderState(int[] stateInfo)
        {
            OrderForSave.Order.StatusPayment = stateInfo[0];
            OrderForSave.Order.StatusManagement = stateInfo[1];
            OrderForSave.Order.StatusInventory = stateInfo[2];
            OrderForSave.Order.StatusShipping = stateInfo[3];
        }

        public override void Request(StateType type) => _state.Handle(type);
        public override void SetState(OrderState state) => _state = state;
        public override List<int> GetCouldManageStauts(OrderState state) => state.GetCouldManageState();
    }

    public class CommonOrderInitState : CommonOrderState
    {
        public CommonOrderInitState(OrderChangeHandler handler) : base(handler) { }
        public override List<int> GetCouldManageState() => new List<int> { 0 };

        public override void Handle(StateType stateType)
        {
            var commonHandler = (CommonOrderChangeHandler)Handler;
            switch (stateType)
            {
                case StateType.ReserveAllState:
                case StateType.ReservePartState:
                case StateType.Cancel:
                    commonHandler.SaveOrderState(StateMachine[stateType]);
                    break;
                case StateType.IsPayed:
                    NotifyWarehouseHandleOrder(Handler, "NotifyAllocateInventoryQueue", true);
                    commonHandler.SaveOrderState(StateMachine[stateType]);
                    break;
                default:
                    throw new Exception("ErrorStatus");
            }
        }
    }

    public abstract class CommonOrderState : OrderState
    {
        protected CommonOrderState(OrderChangeHandler handler) : base(handler) { }
    }

    public class CommonOrderNone : CommonOrderState
    {
        public CommonOrderNone(OrderChangeHandler handler) : base(handler) { }
        public override void Handle(StateType stateType) { }
        public override List<int> GetCouldManageState() => new List<int>();
        protected override void NotifyWarehouseHandleOrder(OrderChangeHandler handler, string queueName, bool isPayed) { }
    }

    public class OrderForSave { public Order Order { get; set; } }
    public class Order { public int StatusPayment { get; set; } public int StatusManagement { get; set; } public int StatusInventory { get; set; } public int StatusShipping { get; set; } }
    public enum StateType { None, ReserveAllState, ReservePartState, IsPayed, Cancel }
}
