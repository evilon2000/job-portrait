# fdd-esigin Sign Task Orchestration

## Why Keep This

This snippet is worth keeping because it shows how local business context is translated into a third-party signing workflow:

- current user is resolved from local auth context
- local enterprise authorization status is checked first
- initiator/actor data is reshaped into Fadada request objects
- sign task creation is executed as a business action rather than direct SDK passthrough

## What Style Trait It Reveals

- businessized SDK orchestration
- auth-gated integration flow
- request-assembly service/controller style

## What Should Still Be Kept Today

- explicit gating on local auth/openId state
- reshaping local BO into SDK actor/request structure
- preserving one endpoint per sign-task action

## What Should Be Changed Today

- move more orchestration from controller into service layer
- reduce repeated access-token acquisition blocks
- introduce structured error/result handling instead of repeated success-code branches

## Source Focus

- `controller/SignTaskController.java`
