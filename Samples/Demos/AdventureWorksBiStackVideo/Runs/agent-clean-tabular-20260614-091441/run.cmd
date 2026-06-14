@echo off
setlocal EnableExtensions EnableDelayedExpansion

set "RUN_DIR=%~dp0"
pushd "%RUN_DIR%"
if not "!ERRORLEVEL!"=="0" exit /b !ERRORLEVEL!

call :RunStage 00-source-readiness.cmd
if not "!ERRORLEVEL!"=="0" goto :Failed

call :RunStage 01-extract-source-schema.cmd
if not "!ERRORLEVEL!"=="0" goto :Failed

call :RunStage 02-rdv-model-sql-deploy.cmd
if not "!ERRORLEVEL!"=="0" goto :Failed

call :RunStage 03-bdv-model-sql-deploy.cmd
if not "!ERRORLEVEL!"=="0" goto :Failed

call :RunStage 04-load-product-vault-slice.cmd
if not "!ERRORLEVEL!"=="0" goto :Failed

call :RunStage 05-load-sales-vault-slice.cmd
if not "!ERRORLEVEL!"=="0" goto :Failed

call :RunStage 06-build-bdv-mart-dq-orchestration.cmd
if not "!ERRORLEVEL!"=="0" goto :Failed

call :RunStage 07-author-process-tabular.cmd
if not "!ERRORLEVEL!"=="0" goto :Failed

echo Clean run completed through Source, RDV, BDV, DW/mart, DQ, transform-backed orchestration, and Tabular proof.
popd
exit /b 0

:RunStage
set "STAGE=%~1"
echo.
echo === %STAGE% ===
cmd /c "stages\%STAGE% > logs\%~n1.log 2>&1"
set "STAGE_EXIT=!ERRORLEVEL!"
type "logs\%~n1.log"
if not "!STAGE_EXIT!"=="0" (
  echo ERROR: %STAGE% failed with exit code !STAGE_EXIT!.
  exit /b !STAGE_EXIT!
)
exit /b 0

:Failed
set "FAILED_EXIT=!ERRORLEVEL!"
echo Clean run halted.
popd
exit /b !FAILED_EXIT!
