@call env.cmd

meta-pipeline --new-workspace CompletePipelineWS
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline add-pipeline --workspace CompletePipelineWS --name LoadStageCustomer
@if errorlevel 1 exit /b %errorlevel%
meta-pipeline add-step --workspace CompletePipelineWS --pipeline LoadStageCustomer --step-name load-stage-customer --script dbo.v_stage_customer --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_ORCHESTRATION_DEMO_SQL --target-connection-env META_ORCHESTRATION_DEMO_SQL --target dbo.StageCustomer
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline add-pipeline --workspace CompletePipelineWS --name LoadStageOrder
@if errorlevel 1 exit /b %errorlevel%
meta-pipeline add-step --workspace CompletePipelineWS --pipeline LoadStageOrder --step-name load-stage-order --script dbo.v_stage_order --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_ORCHESTRATION_DEMO_SQL --target-connection-env META_ORCHESTRATION_DEMO_SQL --target dbo.StageOrder
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline add-pipeline --workspace CompletePipelineWS --name LoadDimCustomer
@if errorlevel 1 exit /b %errorlevel%
meta-pipeline add-step --workspace CompletePipelineWS --pipeline LoadDimCustomer --step-name load-dim-customer --script dbo.v_dim_customer --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_ORCHESTRATION_DEMO_SQL --target-connection-env META_ORCHESTRATION_DEMO_SQL --target dbo.DimCustomer
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline add-pipeline --workspace CompletePipelineWS --name LoadFactSales
@if errorlevel 1 exit /b %errorlevel%
meta-pipeline add-step --workspace CompletePipelineWS --pipeline LoadFactSales --step-name load-fact-sales --script dbo.v_fact_sales --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_ORCHESTRATION_DEMO_SQL --target-connection-env META_ORCHESTRATION_DEMO_SQL --target dbo.FactSales
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline add-pipeline --workspace CompletePipelineWS --name RefreshExchangeRates
@if errorlevel 1 exit /b %errorlevel%
meta-pipeline add-step --workspace CompletePipelineWS --pipeline RefreshExchangeRates --step-name reset-work-rates --script refresh_rates_truncate --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_ORCHESTRATION_DEMO_SQL
@if errorlevel 1 exit /b %errorlevel%
meta-pipeline add-step --workspace CompletePipelineWS --pipeline RefreshExchangeRates --step-name load-work-rates --script dbo.v_work_exchange_rate --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_ORCHESTRATION_DEMO_SQL --target-connection-env META_ORCHESTRATION_DEMO_SQL --target dbo.WorkExchangeRate
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline add-pipeline --workspace CompletePipelineWS --name CleanupPrivateScratch
@if errorlevel 1 exit /b %errorlevel%
meta-pipeline add-step --workspace CompletePipelineWS --pipeline CleanupPrivateScratch --step-name cleanup-private-scratch --script private_scratch_cleanup --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_ORCHESTRATION_DEMO_SQL
@if errorlevel 1 exit /b %errorlevel%

meta-orchestration infer --pipeline-workspace CompletePipelineWS --new-workspace CompleteOrchestrationWS
@if errorlevel 1 exit /b %errorlevel%

meta-orchestration inspect --workspace CompleteOrchestrationWS
@if errorlevel 1 exit /b %errorlevel%

meta-orchestration refresh-run-plan --workspace CompleteOrchestrationWS
@if errorlevel 1 exit /b %errorlevel%

meta-orchestration inspect-run-plan --workspace CompleteOrchestrationWS
@if errorlevel 1 exit /b %errorlevel%

meta-orchestration execute --workspace CompleteOrchestrationWS --pipeline-workspace CompletePipelineWS --pipeline-db-connection-env META_ORCHESTRATION_DEMO_SQL --max-degree-of-parallelism 2
@if errorlevel 1 exit /b %errorlevel%
