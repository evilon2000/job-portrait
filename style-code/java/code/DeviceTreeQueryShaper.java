package com.zcyx.portfolio.airnetwork;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public final class DeviceTreeQueryShaper {

    private DeviceTreeQueryShaper() {}

    public static List<DeviceNode> buildTree(List<DeviceNode> deviceList) {
        Map<Long, DeviceNode> nodeMap = new HashMap<>();
        List<DeviceNode> rootNodes = new ArrayList<>();
        for (DeviceNode device : deviceList) {
            nodeMap.put(device.getId(), device);
            if (device.getChildren() == null) {
                device.setChildren(new ArrayList<>());
            }
        }
        for (DeviceNode device : deviceList) {
            Long pid = device.getPid();
            if (pid == 0L || !nodeMap.containsKey(pid)) {
                rootNodes.add(device);
            } else {
                nodeMap.get(pid).getChildren().add(device);
            }
        }
        return rootNodes;
    }

    public interface DeviceNode {
        Long getId();
        Long getPid();
        List<DeviceNode> getChildren();
        void setChildren(List<DeviceNode> children);
    }
}
