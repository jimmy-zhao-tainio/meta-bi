@set "META_PIPELINE_DEMO_PIPELINE_DB_ADMIN_SQL=Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"

meta-sql execute --connection-env META_PIPELINE_DEMO_PIPELINE_DB_ADMIN_SQL --quiet --query "IF DB_ID(N'MetaPipelineSqlServerCliIntegration') IS NOT NULL BEGIN ALTER DATABASE [MetaPipelineSqlServerCliIntegration] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [MetaPipelineSqlServerCliIntegration]; END"

rem Keep the MetaPipeline operational DB intact so operational run history is not lost.

if exist SchemaWS rmdir /s /q SchemaWS
if exist TransformWS rmdir /s /q TransformWS
if exist BindingWS rmdir /s /q BindingWS
if exist PipelineWS rmdir /s /q PipelineWS
