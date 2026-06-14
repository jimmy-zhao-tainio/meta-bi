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

set "AW_BDV_DATABASE=AdventureWorksBusinessVault"
set "AW_BDV_SQL=Server=%AW_SQL_SERVER%;Database=%AW_BDV_DATABASE%;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False"
set "AW_MASTER_SQL=Server=%AW_SQL_SERVER%;Database=master;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False"
set "BDV_IMPLEMENTATION_WORKSPACE=%~dp0..\..\..\..\..\..\MetaDataVault\Workspaces\MetaDataVaultImplementation"

echo AW_BDV_DATABASE=%AW_BDV_DATABASE%
echo AW_BDV_SQL=%AW_BDV_SQL%
echo BDV_IMPLEMENTATION_WORKSPACE=%BDV_IMPLEMENTATION_WORKSPACE%
echo NOTE: This stage models and deploys BDV structure. RDV-to-BDV load transforms are the next gate.

powershell -NoProfile -ExecutionPolicy Bypass -File "%~dpn0.ps1"
set "EXIT_CODE=!ERRORLEVEL!"

popd
exit /b !EXIT_CODE!
