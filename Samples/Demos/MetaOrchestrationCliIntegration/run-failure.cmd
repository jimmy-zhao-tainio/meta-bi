@call env.cmd

meta-pipeline --new-workspace FailurePipelineWS
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline add-pipeline --workspace FailurePipelineWS --name BrokenStageCustomer
@if errorlevel 1 exit /b %errorlevel%
meta-pipeline add-step --workspace FailurePipelineWS --pipeline BrokenStageCustomer --step-name load-stage-customer --script dbo.v_stage_customer --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_ORCHESTRATION_DEMO_SQL --target-connection-env META_ORCHESTRATION_DEMO_SQL --target dbo.StageCustomer
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline add-pipeline --workspace FailurePipelineWS --name IndependentFailurePath
@if errorlevel 1 exit /b %errorlevel%
meta-pipeline add-step --workspace FailurePipelineWS --pipeline IndependentFailurePath --step-name independent-seed --script independent_failure_seed --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_ORCHESTRATION_DEMO_SQL
@if errorlevel 1 exit /b %errorlevel%
meta-pipeline add-step --workspace FailurePipelineWS --pipeline IndependentFailurePath --step-name independent-followup --script independent_failure_followup --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_ORCHESTRATION_DEMO_SQL
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline add-pipeline --workspace FailurePipelineWS --name FailureHandler
@if errorlevel 1 exit /b %errorlevel%
meta-pipeline add-step --workspace FailurePipelineWS --pipeline FailureHandler --step-name record-failure --script failure_handler --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_ORCHESTRATION_DEMO_SQL
@if errorlevel 1 exit /b %errorlevel%

meta-orchestration infer --pipeline-workspace FailurePipelineWS --new-workspace FailureOrchestrationWS
@if errorlevel 1 exit /b %errorlevel%

meta-orchestration add-dependency --workspace FailureOrchestrationWS --from-task BrokenStageCustomer.load-stage-customer --to-task FailureHandler.record-failure --condition failure --reason "Run the modeled failure dependency branch."
@if errorlevel 1 exit /b %errorlevel%

meta-orchestration allow-concurrent-append --workspace FailureOrchestrationWS --object dbo.OrchestrationFailureLog --reason "Failure demo log rows are append-only."
@if errorlevel 1 exit /b %errorlevel%

meta-orchestration refresh-run-plan --workspace FailureOrchestrationWS
@if errorlevel 1 exit /b %errorlevel%

meta-orchestration inspect-run-plan --workspace FailureOrchestrationWS
@if errorlevel 1 exit /b %errorlevel%

meta-sql execute --connection-env META_ORCHESTRATION_DEMO_SQL --quiet --query "DROP TABLE dbo.RawCustomer;"
@if errorlevel 1 exit /b %errorlevel%

meta-orchestration execute --workspace FailureOrchestrationWS --pipeline-workspace FailurePipelineWS --pipeline-db-connection-env META_ORCHESTRATION_DEMO_SQL
@if "%ERRORLEVEL%"=="0" exit /b 1
@if not "%ERRORLEVEL%"=="4" exit /b %ERRORLEVEL%

meta-sql execute --connection-env META_ORCHESTRATION_DEMO_SQL --quiet --query "IF NOT EXISTS (SELECT 1 FROM dbo.OrchestrationFailureLog WHERE Message = N'MetaOrchestration failure dependency branch fired') THROW 50000, 'Failure handler did not run.', 1;"
@if errorlevel 1 exit /b %errorlevel%

meta-sql execute --connection-env META_ORCHESTRATION_DEMO_SQL --quiet --query "IF NOT EXISTS (SELECT 1 FROM dbo.OrchestrationFailureLog WHERE Message = N'Independent failure path followup') THROW 50000, 'Independent viable path did not continue.', 1;"
@if errorlevel 1 exit /b %errorlevel%
