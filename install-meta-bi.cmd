@echo off
setlocal

set "TARGET_DIR=%LOCALAPPDATA%\meta\bin"
set "META_SCHEMA_EXE=%~dp0MetaSchema.Cli\bin\publish\win-x64\meta-schema.exe"
set "META_TYPE_EXE=%~dp0MetaType.Cli\bin\publish\win-x64\meta-type.exe"
set "META_TYPE_CONVERSION_EXE=%~dp0MetaTypeConversion.Cli\bin\publish\win-x64\meta-type-conversion.exe"
set "META_DATAVAULT_EXE=%~dp0MetaDataVault.Cli\bin\publish\win-x64\meta-datavault.exe"

if not exist "%META_SCHEMA_EXE%" (
  echo Error: missing "%META_SCHEMA_EXE%".
  echo Build MetaSchema.sln first so meta-schema.exe is published.
  exit /b 1
)

if not exist "%META_TYPE_EXE%" (
  echo Error: missing "%META_TYPE_EXE%".
  echo Build MetaType.sln first so meta-type.exe is published.
  exit /b 1
)

if not exist "%META_TYPE_CONVERSION_EXE%" (
  echo Error: missing "%META_TYPE_CONVERSION_EXE%".
  echo Build MetaTypeConversion.sln first so meta-type-conversion.exe is published.
  exit /b 1
)

if not exist "%META_DATAVAULT_EXE%" (
  echo Error: missing "%META_DATAVAULT_EXE%".
  echo Build MetaDataVault.sln first so meta-datavault.exe is published.
  exit /b 1
)

if not exist "%TARGET_DIR%" mkdir "%TARGET_DIR%"

copy /Y "%META_SCHEMA_EXE%" "%TARGET_DIR%\meta-schema.exe" >nul
copy /Y "%META_TYPE_EXE%" "%TARGET_DIR%\meta-type.exe" >nul
copy /Y "%META_TYPE_CONVERSION_EXE%" "%TARGET_DIR%\meta-type-conversion.exe" >nul
copy /Y "%META_DATAVAULT_EXE%" "%TARGET_DIR%\meta-datavault.exe" >nul

for /f "tokens=2,*" %%A in ('reg query HKCU\Environment /v Path 2^>nul ^| findstr /R /C:"Path"') do set "USER_PATH=%%B"

echo;%USER_PATH%; | find /I ";%TARGET_DIR%;" >nul
if errorlevel 1 (
  if defined USER_PATH (
    reg add HKCU\Environment /v Path /t REG_EXPAND_SZ /d "%USER_PATH%;%TARGET_DIR%" /f >nul
  ) else (
    reg add HKCU\Environment /v Path /t REG_EXPAND_SZ /d "%TARGET_DIR%" /f >nul
  )
)

echo Installed:
echo   %TARGET_DIR%\meta-schema.exe
echo   %TARGET_DIR%\meta-type.exe
echo   %TARGET_DIR%\meta-type-conversion.exe
echo   %TARGET_DIR%\meta-datavault.exe
echo.
echo Restart cmd to pick up PATH changes.
