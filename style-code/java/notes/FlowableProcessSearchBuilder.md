# agric-biz-flowable-query-builder

## 来源

- 项目：`agric_biz`
- 文件：`business/src/main/java/com/zhongce/business/util/ProcessUtils.java`
- 类/函数：`buildProcessSearch(...)` + 各类 query builder

## 背景

Flowable 查询有多种 Query 类型（流程定义、任务、历史任务、历史流程），
如果每个 service 各自拼条件，维护成本会持续上升。

## 问题

如何让流程查询条件在不同 Query 类型上复用，同时保持筛选行为一致。

## 代码

```java
public static void buildProcessSearch(Query<?, ?> query, ProcessQuery process) {
    if (query instanceof ProcessDefinitionQuery) {
        buildProcessDefinitionSearch((ProcessDefinitionQuery) query, process);
    } else if (query instanceof TaskQuery) {
        buildTaskSearch((TaskQuery) query, process);
    } else if (query instanceof HistoricTaskInstanceQuery) {
        buildHistoricTaskInstanceSearch((HistoricTaskInstanceQuery) query, process);
    } else if (query instanceof HistoricProcessInstanceQuery) {
        buildHistoricProcessInstanceSearch((HistoricProcessInstanceQuery) query, process);
    }
}
```

## 为什么值得保留

- 查询整形统一入口
- 减少 service 层重复条件拼装
- 易于持续补充筛选字段

## 它体现的个人风格

- 先收口输入，再执行查询
- 把复杂条件逻辑放在工具层，而不是散落业务层

## 今天重看会不会改

- 会改什么：`params` 未使用字段应清理，增加单元测试覆盖各 Query 分支
- 不会改什么：统一入口+分类型构建的总体结构
