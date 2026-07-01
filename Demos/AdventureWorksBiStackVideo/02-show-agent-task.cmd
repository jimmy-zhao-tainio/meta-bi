@echo off
call "%~dp000-env.cmd" || exit /b %errorlevel%

echo.
echo type "%~dp0BUSINESS-REQUIREMENTS.md"
type "%~dp0BUSINESS-REQUIREMENTS.md" || exit /b %errorlevel%

echo.
echo type "%~dp0agent-meta.md"
type "%~dp0agent-meta.md" || exit /b %errorlevel%

echo.
echo type "%~dp0AGENT-TASK.md"
type "%~dp0AGENT-TASK.md" || exit /b %errorlevel%

exit /b 0
