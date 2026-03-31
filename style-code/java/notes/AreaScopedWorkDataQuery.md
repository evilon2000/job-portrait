# smart-agriculture-saas-area-scoped-workdata-query

## Source

- `business/src/main/java/com/zhongce/business/service/WorkDataService.java`

## Snippet

```java
if (roleLevel == 1 || roleLevel == 2 || roleLevel == 3) {
    List<Long> areaRegionIds = new ArrayList<>();
    if (query.getAreaId() != null) {
        areaRegionIds.addAll(getAreaRegion(query.getAreaId()));
    } else {
        areaRegionIds.addAll(getAreaRegion(currentUser.getAreaId()));
    }
    if (CollectionUtil.isEmpty(areaRegionIds)) {
        newQuery.setCooperativeNames(Collections.singletonList("$**"));
    } else {
        List<Cooperative> cooperativeList = cooperativeService.getCooperativeListByAreaIds(areaRegionIds);
        if (CollectionUtil.isNotEmpty(cooperativeList)) {
            newQuery.setCooperativeNames(cooperativeList.stream().map(Cooperative::getName).collect(Collectors.toList()));
        } else {
            newQuery.setCooperativeNames(Collections.singletonList("$**"));
        }
    }
}
```

## Why Keep It

- Good example of role/area scope being translated into downstream IoT query constraints
