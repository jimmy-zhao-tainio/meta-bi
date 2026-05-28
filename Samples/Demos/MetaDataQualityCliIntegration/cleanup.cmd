@set "META_DQ_DEMO_MASTER_SQL=Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"

meta-sql execute --connection-env META_DQ_DEMO_MASTER_SQL --quiet --query "IF DB_ID(N'MetaDataQualityCliIntegration') IS NOT NULL BEGIN ALTER DATABASE [MetaDataQualityCliIntegration] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [MetaDataQualityCliIntegration]; END"
meta-sql execute --connection-env META_DQ_DEMO_MASTER_SQL --quiet --query "IF DB_ID(N'MetaDQ') IS NOT NULL BEGIN ALTER DATABASE [MetaDQ] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [MetaDQ]; END"
if exist TransformWS rmdir /s /q TransformWS
if exist DataQualityWS rmdir /s /q DataQualityWS
if exist DataQualityViews.sql del /q DataQualityViews.sql
