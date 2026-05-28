@set "META_BI_DEMO_MASTER_SQL=Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"

meta-sql execute --connection-env META_BI_DEMO_MASTER_SQL --quiet --query "IF DB_ID(N'RawDataVaultCliIntegrationWorkspace') IS NOT NULL BEGIN ALTER DATABASE [RawDataVaultCliIntegrationWorkspace] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [RawDataVaultCliIntegrationWorkspace]; END"
meta-sql execute --connection-env META_BI_DEMO_MASTER_SQL --quiet --query "IF DB_ID(N'RawDataVaultSample') IS NOT NULL BEGIN ALTER DATABASE [RawDataVaultSample] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [RawDataVaultSample]; END"

if exist RawDataVaultCliIntegrationWorkspace rmdir /s /q RawDataVaultCliIntegrationWorkspace
if exist CurrentMetaSqlWorkspace rmdir /s /q CurrentMetaSqlWorkspace
if exist MetaSqlVerifyManifest rmdir /s /q MetaSqlVerifyManifest
if exist MetaSqlDeployManifest rmdir /s /q MetaSqlDeployManifest
if exist MetaSqlOutput rmdir /s /q MetaSqlOutput
if exist DeployManifest rmdir /s /q DeployManifest
if exist GeneratedSql rmdir /s /q GeneratedSql
