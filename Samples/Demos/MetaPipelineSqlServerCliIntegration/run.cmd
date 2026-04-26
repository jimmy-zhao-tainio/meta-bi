call cleanup.cmd >nul 2>&1

sqlcmd -S . -d master -b -i setup.sql
@if errorlevel 1 exit /b %errorlevel%

@set "META_PIPELINE_DEMO_SOURCE_SQL=Server=.;Database=MetaPipelineSqlServerCliIntegration;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
@set "META_PIPELINE_DEMO_TARGET_SQL=Server=.;Database=MetaPipelineSqlServerCliIntegration;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
@set "META_PIPELINE=meta-pipeline"
@if exist "..\..\..\MetaPipeline\Cli\bin\Debug\net8.0\meta-pipeline.exe" set "META_PIPELINE=..\..\..\MetaPipeline\Cli\bin\Debug\net8.0\meta-pipeline.exe"

meta-schema extract sqlserver --new-workspace SchemaWS --connection-env META_PIPELINE_DEMO_SOURCE_SQL --system MetaPipelineSqlServerCliIntegration --all-schemas --all-tables
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path TransformScripts\customer_load.sql --target dbo.TargetCustomer --new-workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-binding bind --transform-workspace TransformWS --source-schema SchemaWS --target-schema SchemaWS --execute-system MetaPipelineSqlServerCliIntegration --new-workspace BindingWS
@if errorlevel 1 exit /b %errorlevel%

%META_PIPELINE% init --new-workspace PipelineWS
@if errorlevel 1 exit /b %errorlevel%

%META_PIPELINE% add-pipeline --workspace PipelineWS --name CustomerLoad --description "Load customer totals"
@if errorlevel 1 exit /b %errorlevel%

%META_PIPELINE% add-transform --workspace PipelineWS --pipeline CustomerLoad --task load-customers --transform-workspace TransformWS --binding-workspace BindingWS --transform-script-id TransformScript:1 --transform-binding-id TransformScript:1:binding --source-connection-ref source --source-connection-env META_PIPELINE_DEMO_SOURCE_SQL --target-connection-ref target --target-connection-env META_PIPELINE_DEMO_TARGET_SQL --target dbo.TargetCustomer --batch-size 2
@if errorlevel 1 exit /b %errorlevel%

%META_PIPELINE% inspect --workspace PipelineWS
@if errorlevel 1 exit /b %errorlevel%

%META_PIPELINE% execute --workspace PipelineWS --pipeline CustomerLoad --task load-customers --transform-workspace TransformWS --binding-workspace BindingWS
@if errorlevel 1 exit /b %errorlevel%

sqlcmd -S . -d MetaPipelineSqlServerCliIntegration -b -Q "SET NOCOUNT ON; SELECT CustomerId, CustomerName, TotalAmount FROM dbo.TargetCustomer ORDER BY CustomerId;"
@if errorlevel 1 exit /b %errorlevel%
