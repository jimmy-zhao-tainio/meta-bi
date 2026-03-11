cd /d C:\Users\jimmy\Desktop\meta-bi\Samples\Demos\RawDataVaultCliIntegration
sqlcmd -S . -Q "IF DB_ID('RawDataVaultSample') IS NOT NULL BEGIN ALTER DATABASE [RawDataVaultSample] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [RawDataVaultSample]; END"
if exist .\Workspace rmdir /s /q .\Workspace
if exist .\GeneratedSql rmdir /s /q .\GeneratedSql
