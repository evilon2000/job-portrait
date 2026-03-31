public class SignTaskIntegrationOrchestrator {
    public ResultHelper<?> createSignTask(CreateSignTaskBO request) throws ApiException {
        UserData userData = UserDataUtils.getUserData();
        Users user = usersService.getById(userData.getId());
        if (user == null || !user.getFddAuth() || user.getFddOpenid() == null) {
            throw new ResponseStatusException(HttpStatus.FORBIDDEN, "corp authorization required");
        }

        SignTaskClient signTaskClient = new SignTaskClient(openApiClient);
        CreateWithTemplateReq sdkReq = new CreateWithTemplateReq();

        OpenId initiator = new OpenId();
        initiator.setOpenId(user.getFddOpenid());
        initiator.setIdType(FDDConstants.CORP);
        sdkReq.setInitiator(initiator);
        sdkReq.setSignTaskSubject(request.getSignTaskSubject());
        sdkReq.setSignTemplateId(request.getSignTemplateId());
        sdkReq.setDueDate(request.getDueDate());
        sdkReq.setExpiresTime(request.getExpiresTime());
        sdkReq.setAutoStart(true);
        sdkReq.setTransReferenceId(request.getTransReferenceId());

        List<AddActorsTempInfo> actors = request.getActors().stream().map(actor -> {
            actor.setPermissions(Arrays.asList("sign", "fill"));
            AddActorsTempInfo wrapper = new AddActorsTempInfo();
            wrapper.setActor(actor);
            return wrapper;
        }).collect(Collectors.toList());
        sdkReq.setActors(actors);

        sdkReq.setAccessToken(serviceClient.getAccessToken().getData().getAccessToken());
        BaseRes<CreateSignTaskRes> res = signTaskClient.createWithTemplate(sdkReq);
        return res.isSuccess() && "100000".equals(res.getCode())
                ? ResultHelper.succeed(res.getData())
                : ResultHelper.failed2Msg(res.getMsg());
    }
}
