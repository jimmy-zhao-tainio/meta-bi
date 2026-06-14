@echo off
setlocal EnableExtensions EnableDelayedExpansion

set "RUN_DIR=%~dp0.."
pushd "%RUN_DIR%"
if not "!ERRORLEVEL!"=="0" exit /b !ERRORLEVEL!

echo call "%~dp0..\..\..\00-env.cmd"
call "%~dp0..\..\..\00-env.cmd"
if not "!ERRORLEVEL!"=="0" (
  set "EXIT_CODE=!ERRORLEVEL!"
  echo ERROR: 00-env.cmd failed with exit code !EXIT_CODE!.
  popd
  exit /b !EXIT_CODE!
)

set "AW_RDV_DATABASE=AdventureWorksRawVault"
set "AW_BDV_DATABASE=AdventureWorksBusinessVault"
set "AW_RDV_SQL=Server=%AW_SQL_SERVER%;Database=%AW_RDV_DATABASE%;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False"
set "AW_BDV_SQL=Server=%AW_SQL_SERVER%;Database=%AW_BDV_DATABASE%;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False"
set "DATA_TYPE_CONVERSION_WORKSPACE=%~dp0..\..\..\..\..\..\MetaDataTypeConversion\Workspace"

powershell -NoProfile -ExecutionPolicy Bypass -File "%~dpn0.ps1"
set "EXIT_CODE=!ERRORLEVEL!"

popd
exit /b !EXIT_CODE!
