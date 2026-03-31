public class GeoServerPublishManager {
    public GSPostGISDatastoreEncoder buildPostGisEncoder(PostGisLayerParamsDTO params) {
        GSPostGISDatastoreEncoder encoder = new GSPostGISDatastoreEncoder(params.getDatastoreName());
        encoder.setHost(postgresHost);
        encoder.setPort(Integer.parseInt(postgresPort));
        encoder.setDatabase(postgresDatabase);
        encoder.setUser(postgresUser);
        encoder.setPassword(postgresPasswd);
        encoder.setType(postgresType);
        encoder.setSchema(postgresSchema);
        return encoder;
    }

    public String publishPostGisLayer(PostGisLayerParamsDTO params) throws IOException {
        GeoServerRESTManager manager = getGeoServerRESTManager();
        GSPostGISDatastoreEncoder encoder = buildPostGisEncoder(params);

        List<String> workspaceNames = manager.getReader().getWorkspaceNames();
        if (!workspaceNames.contains(params.getWorkspaceName())) {
            // workspace warning branch omitted in this extract
        }

        List<String> dataStoreNames = manager.getReader()
                .getDatastores(params.getWorkspaceName())
                .getNames();

        if (!dataStoreNames.contains(params.getDatastoreName())) {
            manager.getStoreManager().create(params.getWorkspaceName(), encoder);
        }

        String srs = gisService.getSRS(params.getVectorName());
        GSFeatureTypeEncoder featureType = new GSFeatureTypeEncoder();
        featureType.setNativeName(params.getVectorName());
        featureType.setTitle(params.getLayerName());
        featureType.setName(params.getLayerName());
        featureType.setSRS(srs);

        GSLayerEncoder layerEncoder = new GSLayerEncoder();
        layerEncoder.setEnabled(true);

        boolean published = manager.getPublisher().publishDBLayer(
                params.getWorkspaceName(),
                params.getDatastoreName(),
                featureType,
                layerEncoder
        );
        return published ? "publish-success" : "publish-failed";
    }
}
