@set "META_PIPELINE_DEMO_EXECUTION_SQL=Server=.;Database=MetaPipelineSqlServerCliIntegration;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
@set "META_PIPELINE_DEMO_TARGET_SQL=Server=.;Database=MetaPipelineSqlServerCliIntegration;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
@set "META_PIPELINE_DEMO_PIPELINE_DB_ADMIN_SQL=Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
@set "META_PIPELINE_DEMO_OPERATIONAL_SQL=Server=.;Database=MetaPipeline;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"

call cleanup.cmd >nul 2>&1

meta-sql execute --connection-env META_PIPELINE_DEMO_PIPELINE_DB_ADMIN_SQL --file setup.sql
@if errorlevel 1 exit /b %errorlevel%

meta-schema extract sqlserver --new-workspace SchemaWS --connection-env META_PIPELINE_DEMO_EXECUTION_SQL --system MetaPipelineSqlServerCliIntegration --all-schemas --all-tables
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\truncate_target.sql --new-workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\insert_customers.sql --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\update_customers.sql --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\merge_customers.sql --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\delete_customers.sql --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\customer_load.sql --target dbo.TargetCustomer --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-binding bind --transform-workspace TransformWS --source-schema SchemaWS --target-schema SchemaWS --execute-system MetaPipelineSqlServerCliIntegration --ignore-target-columns AuditId,InsertDateTime2 --new-workspace BindingWS
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline --new-workspace PipelineWS
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline add-pipeline --workspace PipelineWS --name CustomerLoad --description "Load customer totals"
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline add-step --workspace PipelineWS --pipeline CustomerLoad --step-name truncate-target --script truncate_target --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_PIPELINE_DEMO_EXECUTION_SQL --timeout-seconds 60
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline add-step --workspace PipelineWS --pipeline CustomerLoad --step-name insert-customers --script insert_customers --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_PIPELINE_DEMO_EXECUTION_SQL --timeout-seconds 60
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline add-step --workspace PipelineWS --pipeline CustomerLoad --step-name update-customers --script update_customers --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_PIPELINE_DEMO_EXECUTION_SQL --timeout-seconds 60
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline add-step --workspace PipelineWS --pipeline CustomerLoad --step-name merge-customers --script merge_customers --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_PIPELINE_DEMO_EXECUTION_SQL --timeout-seconds 60
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline add-step --workspace PipelineWS --pipeline CustomerLoad --step-name delete-customers --script delete_customers --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_PIPELINE_DEMO_EXECUTION_SQL --timeout-seconds 60
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline add-step --workspace PipelineWS --pipeline CustomerLoad --step-name load-customers --script dbo.v_customer_load --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_PIPELINE_DEMO_EXECUTION_SQL --target-connection-env META_PIPELINE_DEMO_TARGET_SQL --target dbo.TargetCustomer --batch-size 2 --timeout-seconds 60
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline inspect --workspace PipelineWS
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline create-pipeline-db --pipeline-db-connection-env META_PIPELINE_DEMO_PIPELINE_DB_ADMIN_SQL
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline execute --workspace PipelineWS --pipeline CustomerLoad --transform-workspace TransformWS --binding-workspace BindingWS --pipeline-db-connection-env META_PIPELINE_DEMO_OPERATIONAL_SQL
@if errorlevel 1 exit /b %errorlevel%
