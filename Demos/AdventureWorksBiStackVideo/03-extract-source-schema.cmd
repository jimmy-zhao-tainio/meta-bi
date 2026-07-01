@echo off
call "%~dp000-env.cmd" || exit /b %errorlevel%

if not exist "%AW_RUN_ROOT%" mkdir "%AW_RUN_ROOT%"
if not exist "%AW_RUN_ROOT%\source\%AW_SOURCE_DATABASE%" mkdir "%AW_RUN_ROOT%\source\%AW_SOURCE_DATABASE%"

set "AW_SOURCE_SCHEMA_WORKSPACE=%AW_RUN_ROOT%\source\%AW_SOURCE_DATABASE%\Schema"

if exist "%AW_SOURCE_SCHEMA_WORKSPACE%\workspace.xml" (
  echo Source schema workspace already exists:
  echo %AW_SOURCE_SCHEMA_WORKSPACE%
  echo.
  echo Remove it or choose a fresh AW_RUN_ROOT before extracting again.
  exit /b 1
)

echo.
echo meta-schema extract sqlserver --new-workspace "%AW_SOURCE_SCHEMA_WORKSPACE%" --connection-env AW_SOURCE_SQL --system %AW_SOURCE_DATABASE% --all-schemas --all-tables
meta-schema extract sqlserver --new-workspace "%AW_SOURCE_SCHEMA_WORKSPACE%" --connection-env AW_SOURCE_SQL --system %AW_SOURCE_DATABASE% --all-schemas --all-tables || exit /b %errorlevel%

exit /b 0
