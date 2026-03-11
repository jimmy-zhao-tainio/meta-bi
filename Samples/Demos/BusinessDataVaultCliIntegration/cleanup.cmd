cd /d C:\Users\jimmy\Desktop\meta-bi\Samples\Demos\BusinessDataVaultCliIntegration
sqlcmd -S . -Q "IF DB_ID('MetaBiBusinessDataVaultCliModelDemo') IS NOT NULL BEGIN ALTER DATABASE [MetaBiBusinessDataVaultCliModelDemo] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [MetaBiBusinessDataVaultCliModelDemo]; END"
if exist .\Workspace rmdir /s /q .\Workspace
if exist .\GeneratedSql rmdir /s /q .\GeneratedSql


