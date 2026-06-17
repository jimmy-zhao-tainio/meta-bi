@echo off
pushd "%~dp0" || exit /b 1
if not defined HAIRBALL_EXECUTION_SQL set "HAIRBALL_EXECUTION_SQL=Server=localhost;Database=MetaOrchestrationHairball;Trusted_Connection=True;TrustServerCertificate=True;"
if not defined HAIRBALL_TARGET_SQL set "HAIRBALL_TARGET_SQL=Server=localhost;Database=MetaOrchestrationHairball;Trusted_Connection=True;TrustServerCertificate=True;"
echo.
echo meta-orchestration execute --workspace OrchestrationWS --pipeline-workspace PipelineWS --transform-workspace TransformWS --binding-workspace BindingWS --max-degree-of-parallelism 12 --run-artifacts-root RunArtifacts
echo Capturing output to orchestration-execute-output.txt
call meta-orchestration execute --workspace OrchestrationWS --pipeline-workspace PipelineWS --transform-workspace TransformWS --binding-workspace BindingWS --max-degree-of-parallelism 12 --run-artifacts-root RunArtifacts > "orchestration-execute-output.txt" 2>&1
set "__hairball_capture_exit=%errorlevel%"
type "orchestration-execute-output.txt"
if not "%__hairball_capture_exit%"=="0" (
  set "__hairball_exit=%__hairball_capture_exit%"
  goto :fail
)
popd
exit /b 0
:fail
if not defined __hairball_exit set "__hairball_exit=%errorlevel%"
popd
exit /b %__hairball_exit%
:run
echo.
echo %*
call %*
exit /b %errorlevel%
