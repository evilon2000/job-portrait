package com.zhongce.portfolio.smartagri;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public final class AreaScopedWorkDataQuery {

    private AreaScopedWorkDataQuery() {}

    public static QueryModel build(
            Long queryAreaId,
            Long currentAreaId,
            AreaResolver areaResolver,
            CooperativeResolver cooperativeResolver) {
        List<Long> areaRegionIds = new ArrayList<>();
        if (queryAreaId != null) {
            areaRegionIds.addAll(areaResolver.getAreaRegion(queryAreaId));
        } else {
            areaRegionIds.addAll(areaResolver.getAreaRegion(currentAreaId));
        }
        QueryModel query = new QueryModel();
        if (areaRegionIds.isEmpty()) {
            query.cooperativeNames = Collections.singletonList("$**");
        } else {
            List<String> names = cooperativeResolver.getCooperativeNamesByAreaIds(areaRegionIds);
            query.cooperativeNames = names.isEmpty() ? Collections.singletonList("$**") : names;
        }
        return query;
    }

    public interface AreaResolver {
        List<Long> getAreaRegion(Long areaId);
    }

    public interface CooperativeResolver {
        List<String> getCooperativeNamesByAreaIds(List<Long> areaIds);
    }

    public static final class QueryModel {
        public List<String> cooperativeNames;
    }
}
