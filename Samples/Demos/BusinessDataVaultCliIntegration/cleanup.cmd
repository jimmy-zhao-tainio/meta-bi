cd /d C:\Users\jimmy\Desktop\meta-bi\Samples\Demos\BusinessDataVaultCliIntegration
sqlcmd -S . -Q "IF DB_ID('MetaBiBusinessDataVaultCliDemo') IS NOT NULL BEGIN ALTER DATABASE [MetaBiBusinessDataVaultCliDemo] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [MetaBiBusinessDataVaultCliDemo]; END"
if exist .\Workspace rmdir /s /q .\Workspace
if exist .\GeneratedSql rmdir /s /q .\GeneratedSql
