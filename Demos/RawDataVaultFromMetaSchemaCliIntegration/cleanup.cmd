@set "META_BI_DEMO_MASTER_SQL=Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"

meta-sql execute --connection-env META_BI_DEMO_MASTER_SQL --quiet --query "IF DB_ID(N'RawDataVaultFromMetaSchemaCliIntegrationWorkspace') IS NOT NULL BEGIN ALTER DATABASE [RawDataVaultFromMetaSchemaCliIntegrationWorkspace] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [RawDataVaultFromMetaSchemaCliIntegrationWorkspace]; END"
meta-sql execute --connection-env META_BI_DEMO_MASTER_SQL --quiet --query "IF DB_ID(N'BusinessDataVaultSample_RawDvBootstrap') IS NOT NULL BEGIN ALTER DATABASE [BusinessDataVaultSample_RawDvBootstrap] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [BusinessDataVaultSample_RawDvBootstrap]; END"

if exist MetaSchemaWorkspace rmdir /s /q MetaSchemaWorkspace
if exist RawDataVaultFromMetaSchemaCliIntegrationWorkspace rmdir /s /q RawDataVaultFromMetaSchemaCliIntegrationWorkspace
