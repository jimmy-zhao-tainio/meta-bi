sqlcmd -S . -d master -b -Q "IF DB_ID(N'RawDataVaultFromMetaSchemaCliIntegrationWorkspace') IS NOT NULL BEGIN ALTER DATABASE [RawDataVaultFromMetaSchemaCliIntegrationWorkspace] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [RawDataVaultFromMetaSchemaCliIntegrationWorkspace]; END"

if exist MetaSchemaWorkspace rmdir /s /q MetaSchemaWorkspace
if exist RawDataVaultFromMetaSchemaCliIntegrationWorkspace rmdir /s /q RawDataVaultFromMetaSchemaCliIntegrationWorkspace
