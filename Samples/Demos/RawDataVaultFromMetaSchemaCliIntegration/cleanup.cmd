sqlcmd -S . -d master -b -Q "IF DB_ID(N'RawDataVaultFromMetaSchemaCliIntegrationWorkspace') IS NOT NULL BEGIN ALTER DATABASE [RawDataVaultFromMetaSchemaCliIntegrationWorkspace] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [RawDataVaultFromMetaSchemaCliIntegrationWorkspace]; END"
sqlcmd -S . -d master -b -Q "IF DB_ID(N'BusinessDataVaultSample_RawDvBootstrap') IS NOT NULL BEGIN ALTER DATABASE [BusinessDataVaultSample_RawDvBootstrap] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [BusinessDataVaultSample_RawDvBootstrap]; END"

if exist MetaSchemaWorkspace rmdir /s /q MetaSchemaWorkspace
if exist RawDataVaultFromMetaSchemaCliIntegrationWorkspace rmdir /s /q RawDataVaultFromMetaSchemaCliIntegrationWorkspace
