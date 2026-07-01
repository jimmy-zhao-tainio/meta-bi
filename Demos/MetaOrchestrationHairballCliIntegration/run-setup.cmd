@echo off
if exist Runs rmdir /s /q Runs

call :run dotnet run --project MetaOrchestrationHairballDemo.csproj -- generate --out-root Runs --seed 20260530 || goto :fail
call :run Runs\hairball-seed-20260530\generated-setup.cmd || goto :fail
call :run dotnet run --project MetaOrchestrationHairballDemo.csproj -- verify --run-root Runs\hairball-seed-20260530 --seed 20260530 || goto :fail

exit /b 0

:fail
exit /b %errorlevel%

:run
echo.
echo %*
call %*
exit /b %errorlevel%
