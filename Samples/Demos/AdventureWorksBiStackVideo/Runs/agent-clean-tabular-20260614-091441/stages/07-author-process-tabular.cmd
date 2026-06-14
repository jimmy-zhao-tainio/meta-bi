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

set "AW_TARGET_SQL=Server=%AW_SQL_SERVER%;Database=%AW_TARGET_DATABASE%;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False"
set "AW_MASTER_SQL=Server=%AW_SQL_SERVER%;Database=master;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False"

powershell -NoProfile -ExecutionPolicy Bypass -File "%~dpn0.ps1"
set "EXIT_CODE=!ERRORLEVEL!"

popd
exit /b !EXIT_CODE!
