@echo off
setlocal

set "SCRIPT_DIR=%~dp0"
pushd "%SCRIPT_DIR%" || exit /b 1

set "REPO_ROOT=%SCRIPT_DIR%..\..\.."
set "META_SCHEMA_DLL=%REPO_ROOT%\MetaSchema.Cli\bin\Debug\net8.0\meta-schema.dll"
set "META_DATAVAULT_RAW_DLL=%REPO_ROOT%\MetaDataVault.Raw.Cli\bin\Debug\net8.0\meta-datavault-raw.dll"
set "SOURCE_DATABASE=BusinessDataVaultSample"
set "TARGET_DATABASE=BusinessDataVaultSample_RawDvBootstrap"
set "SERVER_CONNECTION=Server=.;Integrated Security=true;TrustServerCertificate=true"
set "SOURCE_CONNECTION=Server=.;Database=%SOURCE_DATABASE%;Integrated Security=true;TrustServerCertificate=true"
set "SCHEMA_WORKSPACE=.\MetaSchemaWorkspace"
set "RAW_WORKSPACE=.\RawDataVaultWorkspace"
set "SQL_OUTPUT=.\GeneratedSql"
set "IMPLEMENTATION_WORKSPACE=%REPO_ROOT%\MetaDataVault.Workspaces\MetaDataVaultImplementation"
set "DATA_TYPE_CONVERSION_WORKSPACE=%REPO_ROOT%\MetaDataTypeConversion.Workspaces\MetaDataTypeConversion"

echo Source database: %SOURCE_DATABASE%
echo Target database: %TARGET_DATABASE%

if exist "%SCHEMA_WORKSPACE%" rmdir /s /q "%SCHEMA_WORKSPACE%"
if exist "%RAW_WORKSPACE%" rmdir /s /q "%RAW_WORKSPACE%"
if exist "%SQL_OUTPUT%" rmdir /s /q "%SQL_OUTPUT%"

sqlcmd -S . -Q "IF DB_ID('%TARGET_DATABASE%') IS NOT NULL BEGIN ALTER DATABASE [%TARGET_DATABASE%] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [%TARGET_DATABASE%]; END" >nul || exit /b 1

dotnet "%META_SCHEMA_DLL%" extract sqlserver --new-workspace "%SCHEMA_WORKSPACE%" --connection "%SOURCE_CONNECTION%" --system "%SOURCE_DATABASE%" --all-schemas --all-tables || exit /b 1

dotnet "%META_DATAVAULT_RAW_DLL%" from-metaschema --source-workspace "%SCHEMA_WORKSPACE%" --implementation-workspace "%IMPLEMENTATION_WORKSPACE%" --new-workspace "%RAW_WORKSPACE%" || exit /b 1

dotnet "%META_DATAVAULT_RAW_DLL%" generate-sql --workspace "%RAW_WORKSPACE%" --implementation-workspace "%IMPLEMENTATION_WORKSPACE%" --data-type-conversion-workspace "%DATA_TYPE_CONVERSION_WORKSPACE%" --out "%SQL_OUTPUT%" || exit /b 1

meta deploy sqlserver --scripts "%SQL_OUTPUT%" --connection-string "%SERVER_CONNECTION%" --database "%TARGET_DATABASE%" || exit /b 1

echo.
echo End-to-end raw bootstrap completed.
echo MetaSchema workspace: %SCHEMA_WORKSPACE%
echo MetaRawDataVault workspace: %RAW_WORKSPACE%
echo Generated SQL: %SQL_OUTPUT%
echo Deployed database: %TARGET_DATABASE%

popd
