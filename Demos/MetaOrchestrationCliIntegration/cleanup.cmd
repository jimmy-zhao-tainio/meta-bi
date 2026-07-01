@call env.cmd

meta-sql execute --connection-env META_ORCHESTRATION_DEMO_ADMIN_SQL --quiet --query "IF DB_ID(N'MetaOrchestrationCliIntegration') IS NOT NULL BEGIN ALTER DATABASE [MetaOrchestrationCliIntegration] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [MetaOrchestrationCliIntegration]; END"

if exist SchemaWS rmdir /s /q SchemaWS
if exist TransformWS rmdir /s /q TransformWS
if exist BindingWS rmdir /s /q BindingWS
if exist CompletePipelineWS rmdir /s /q CompletePipelineWS
if exist CompleteOrchestrationWS rmdir /s /q CompleteOrchestrationWS
if exist PolicyPipelineWS rmdir /s /q PolicyPipelineWS
if exist PolicyOrchestrationWS rmdir /s /q PolicyOrchestrationWS
if exist InvalidPipelineWS rmdir /s /q InvalidPipelineWS
if exist InvalidOrchestrationWS rmdir /s /q InvalidOrchestrationWS
if exist FailurePipelineWS rmdir /s /q FailurePipelineWS
if exist FailureOrchestrationWS rmdir /s /q FailureOrchestrationWS
