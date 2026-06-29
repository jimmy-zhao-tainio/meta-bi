@call env.cmd

meta-pipeline --new-workspace InvalidPipelineWS
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline add-pipeline --workspace InvalidPipelineWS --name TruncateSharedStage
@if errorlevel 1 exit /b %errorlevel%
meta-pipeline add-step --workspace InvalidPipelineWS --pipeline TruncateSharedStage --step-name truncate-shared-stage --script truncate_shared_stage --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_ORCHESTRATION_DEMO_SQL
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline add-pipeline --workspace InvalidPipelineWS --name ReadSharedStage
@if errorlevel 1 exit /b %errorlevel%
meta-pipeline add-step --workspace InvalidPipelineWS --pipeline ReadSharedStage --step-name read-shared-stage --script dbo.v_read_shared_stage --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_ORCHESTRATION_DEMO_SQL --target-connection-env META_ORCHESTRATION_DEMO_SQL --target dbo.DimCustomer
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline add-pipeline --workspace InvalidPipelineWS --name SharedWriterA
@if errorlevel 1 exit /b %errorlevel%
meta-pipeline add-step --workspace InvalidPipelineWS --pipeline SharedWriterA --step-name write-shared-a --script dbo.v_shared_writer_a --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_ORCHESTRATION_DEMO_SQL --target-connection-env META_ORCHESTRATION_DEMO_SQL --target dbo.SharedLanding
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline add-pipeline --workspace InvalidPipelineWS --name SharedWriterB
@if errorlevel 1 exit /b %errorlevel%
meta-pipeline add-step --workspace InvalidPipelineWS --pipeline SharedWriterB --step-name write-shared-b --script dbo.v_shared_writer_b --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_ORCHESTRATION_DEMO_SQL --target-connection-env META_ORCHESTRATION_DEMO_SQL --target dbo.SharedLanding
@if errorlevel 1 exit /b %errorlevel%

meta-orchestration infer --pipeline-workspace InvalidPipelineWS --new-workspace InvalidOrchestrationWS
@if "%ERRORLEVEL%"=="0" exit /b 1
@if not "%ERRORLEVEL%"=="4" exit /b %ERRORLEVEL%

meta-orchestration inspect --workspace InvalidOrchestrationWS
@if errorlevel 1 exit /b %errorlevel%
