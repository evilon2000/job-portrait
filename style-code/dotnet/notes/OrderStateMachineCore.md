# Autchon Backend - Order State Machine

## Source

- Project: Autchon Backend
- Files:
  - `AfOrderAndPayment/Model/Order/stateMachine/OrderState.cs`
  - `AfOrderAndPayment/Model/Order/stateMachine/CommonOrderChangeHandler.cs`

## Why it is worth keeping

- 状态和变更处理器被明确分开。
- 状态迁移能挂接副作用，例如队列通知。
- 它把复杂订单流程变成了可追踪的对象关系。
