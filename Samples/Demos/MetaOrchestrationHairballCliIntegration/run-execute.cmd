@echo off
if not defined HAIRBALL_EXECUTION_SQL set "HAIRBALL_EXECUTION_SQL=Server=localhost;Database=MetaOrchestrationHairball;Trusted_Connection=True;TrustServerCertificate=True;"
if not defined HAIRBALL_TARGET_SQL set "HAIRBALL_TARGET_SQL=Server=localhost;Database=MetaOrchestrationHairball;Trusted_Connection=True;TrustServerCertificate=True;"

call :run Runs\hairball-seed-20260530\generated-execute.cmd || goto :fail

exit /b 0

:fail
exit /b %errorlevel%

:run
echo.
echo %*
call %*
exit /b %errorlevel%
