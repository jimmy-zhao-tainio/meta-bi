sqlcmd -S . -d master -b -Q "IF DB_ID(N'BusinessDataVaultCliIntegrationWorkspace') IS NOT NULL BEGIN ALTER DATABASE [BusinessDataVaultCliIntegrationWorkspace] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [BusinessDataVaultCliIntegrationWorkspace]; END"
sqlcmd -S . -d master -b -Q "IF DB_ID(N'BusinessDataVaultSample') IS NOT NULL BEGIN ALTER DATABASE [BusinessDataVaultSample] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [BusinessDataVaultSample]; END"

if exist BusinessDataVaultCliIntegrationWorkspace rmdir /s /q BusinessDataVaultCliIntegrationWorkspace
if exist CurrentMetaSqlWorkspace rmdir /s /q CurrentMetaSqlWorkspace
if exist MetaSqlVerifyManifest rmdir /s /q MetaSqlVerifyManifest
if exist MetaSqlDeployManifest rmdir /s /q MetaSqlDeployManifest
if exist MetaSqlOutput rmdir /s /q MetaSqlOutput
if exist DeployManifest rmdir /s /q DeployManifest
if exist GeneratedSql rmdir /s /q GeneratedSql
