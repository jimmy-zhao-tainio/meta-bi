sqlcmd -S . -d master -b -Q "IF DB_ID(N'MetaPipelineSqlServerCliIntegration') IS NOT NULL BEGIN ALTER DATABASE [MetaPipelineSqlServerCliIntegration] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [MetaPipelineSqlServerCliIntegration]; END"

if exist SchemaWS rmdir /s /q SchemaWS
if exist TransformWS rmdir /s /q TransformWS
if exist BindingWS rmdir /s /q BindingWS
if exist PipelineWS rmdir /s /q PipelineWS
