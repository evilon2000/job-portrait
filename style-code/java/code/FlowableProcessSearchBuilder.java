package com.zhongce.portfolio.agricbiz;

import org.flowable.common.engine.api.query.Query;
import org.flowable.engine.history.HistoricProcessInstanceQuery;
import org.flowable.engine.repository.ProcessDefinitionQuery;
import org.flowable.task.api.TaskQuery;
import org.flowable.task.api.history.HistoricTaskInstanceQuery;

/**
 * Snippet from agric_biz:
 * Unified entry for shaping process-related query conditions.
 */
public final class FlowableProcessSearchBuilder {

    private FlowableProcessSearchBuilder() {}

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

    private static void buildProcessDefinitionSearch(ProcessDefinitionQuery query, ProcessQuery process) {}
    private static void buildTaskSearch(TaskQuery query, ProcessQuery process) {}
    private static void buildHistoricTaskInstanceSearch(HistoricTaskInstanceQuery query, ProcessQuery process) {}
    private static void buildHistoricProcessInstanceSearch(HistoricProcessInstanceQuery query, ProcessQuery process) {}

    public interface ProcessQuery {}
}
