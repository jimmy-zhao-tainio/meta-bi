@echo off
call "%~dp000-env.cmd" || exit /b %errorlevel%

echo.
echo powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0prepare-adventureworks-db.ps1"
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0prepare-adventureworks-db.ps1" || exit /b %errorlevel%

exit /b 0
