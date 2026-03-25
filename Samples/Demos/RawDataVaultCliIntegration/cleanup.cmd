sqlcmd -S . -d master -b -Q "IF DB_ID(N'RawDataVaultCliIntegrationWorkspace') IS NOT NULL BEGIN ALTER DATABASE [RawDataVaultCliIntegrationWorkspace] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [RawDataVaultCliIntegrationWorkspace]; END"
sqlcmd -S . -d master -b -Q "IF DB_ID(N'RawDataVaultSample') IS NOT NULL BEGIN ALTER DATABASE [RawDataVaultSample] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [RawDataVaultSample]; END"

if exist RawDataVaultCliIntegrationWorkspace rmdir /s /q RawDataVaultCliIntegrationWorkspace
if exist CurrentMetaSqlWorkspace rmdir /s /q CurrentMetaSqlWorkspace
if exist MetaSqlVerifyManifest rmdir /s /q MetaSqlVerifyManifest
if exist MetaSqlDeployManifest rmdir /s /q MetaSqlDeployManifest
if exist MetaSqlOutput rmdir /s /q MetaSqlOutput
if exist DeployManifest rmdir /s /q DeployManifest
if exist GeneratedSql rmdir /s /q GeneratedSql
