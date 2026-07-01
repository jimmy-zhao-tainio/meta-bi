@set "META_BI_DEMO_MASTER_SQL=Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"

meta-sql execute --connection-env META_BI_DEMO_MASTER_SQL --quiet --query "IF DB_ID(N'DataWarehouseCliIntegration') IS NOT NULL BEGIN ALTER DATABASE [DataWarehouseCliIntegration] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [DataWarehouseCliIntegration]; END"

if exist MetaDataWarehouseCliIntegrationWorkspace rmdir /s /q MetaDataWarehouseCliIntegrationWorkspace
if exist CurrentMetaSqlWorkspace rmdir /s /q CurrentMetaSqlWorkspace
if exist MetaSqlVerifyManifest rmdir /s /q MetaSqlVerifyManifest
if exist MetaSqlDeployManifest rmdir /s /q MetaSqlDeployManifest
if exist MetaSqlOutput rmdir /s /q MetaSqlOutput
if exist DeployManifest rmdir /s /q DeployManifest
if exist GeneratedSql rmdir /s /q GeneratedSql
