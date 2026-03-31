# ZCYX Snippet: Global Query Filter Infrastructure

## 为什么值得保留

这一组代码是 `ZCYX` 最值得收藏的部分之一。它开始体现你在 EF Core 层面组织默认行为，而不只是写业务查询。

## 它体现的风格

- 统一给一类实体追加过滤条件
- 通过表达式转换适配不同具体实体
- 将默认规则放进模型构建层

## 今天仍然会保留什么

- `SetQueryFilterOnAllEntities`
- 表达式转换工具
- 可叠加 query filter 的思路

## 今天会改什么

- 会补更多测试来验证复杂实体继承关系
- 会更严格地区分软删除、状态过滤和租户过滤等不同维度
