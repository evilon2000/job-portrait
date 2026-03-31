# air-network-platform-cloud-server-device-tree-query

## Source

- `auth/src/main/java/com/zcyx/auth/service/DeviceRegistryService.java`

## Snippet

```java
List<DeviceQueryDTO> allDevices = baseMapper.selectJoinList(DeviceQueryDTO.class, wrapper);
List<DeviceQueryDTO> tree = buildTree(allDevices);
List<DeviceQueryDTO> pagedTree = tree.subList((int) Math.max(0, startIndex), (int) endIndex);
return PageDTO.of(pageNo, pageSize, total, pagedTree);
```

## Why Keep It

- Reusable pattern for tree read-model + pagination composition
