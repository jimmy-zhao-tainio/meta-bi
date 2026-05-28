@call env.cmd

@call cleanup.cmd >nul 2>&1

meta-sql execute --connection-env META_ORCHESTRATION_DEMO_ADMIN_SQL --file setup.sql
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline create-pipeline-db --pipeline-db-connection-env META_ORCHESTRATION_DEMO_ADMIN_SQL --pipeline-db-name MetaOrchestrationCliIntegration
@if errorlevel 1 exit /b %errorlevel%

meta-schema extract sqlserver --new-workspace SchemaWS --connection-env META_ORCHESTRATION_DEMO_SQL --system MetaOrchestrationCliIntegration --all-schemas --all-tables
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\stage_customer.sql --target dbo.StageCustomer --new-workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\stage_order.sql --target dbo.StageOrder --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\dim_customer.sql --target dbo.DimCustomer --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\fact_sales.sql --target dbo.FactSales --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\refresh_rates_truncate.sql --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\refresh_rates_load.sql --target dbo.WorkExchangeRate --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\private_scratch_cleanup.sql --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\truncate_shared_stage.sql --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\read_shared_stage.sql --target dbo.DimCustomer --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\shared_writer_a.sql --target dbo.SharedLanding --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\shared_writer_b.sql --target dbo.SharedLanding --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\update_shared_landing.sql --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\merge_shared_landing.sql --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\failure_handler.sql --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\independent_failure_seed.sql --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\independent_failure_followup.sql --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-binding bind --transform-workspace TransformWS --source-schema SchemaWS --target-schema SchemaWS --execute-system MetaOrchestrationCliIntegration --new-workspace BindingWS
@if errorlevel 1 exit /b %errorlevel%
