@echo off
setlocal

set "SCRIPT_DIR=%~dp0"
pushd "%SCRIPT_DIR%" || exit /b 1

set "FRESH_DB=MetaSqlWorkflowFresh"
set "BLOCKED_DB=MetaSqlWorkflowBlocked"
set "META_SQL=%SCRIPT_DIR%..\..\..\MetaSql.Cli\bin\publish\win-x64\meta-sql.exe"

echo == Publishing MetaSql CLI ==
dotnet publish ..\..\..\MetaSql.Cli\MetaSql.Cli.csproj -c Debug -r win-x64 --self-contained false >nul || exit /b 1

echo == Resetting migration folders ==
if exist "deploy\migrate\target\blocked" rmdir /s /q "deploy\migrate\target\blocked"
if exist "deploy\migrate\archive" rmdir /s /q "deploy\migrate\archive"
mkdir "deploy\migrate\baseline" 2>nul
mkdir "deploy\migrate\target\blocked" 2>nul
mkdir "deploy\migrate\archive" 2>nul

echo == Recreating demo databases ==
sqlcmd -S . -E -C -b -d master -Q "IF DB_ID(N'%FRESH_DB%') IS NOT NULL BEGIN ALTER DATABASE [%FRESH_DB%] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [%FRESH_DB%]; END; CREATE DATABASE [%FRESH_DB%];" || exit /b 1
sqlcmd -S . -E -C -b -d master -Q "IF DB_ID(N'%BLOCKED_DB%') IS NOT NULL BEGIN ALTER DATABASE [%BLOCKED_DB%] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [%BLOCKED_DB%]; END; CREATE DATABASE [%BLOCKED_DB%];" || exit /b 1

echo == Seeding blocked target with drift ==
sqlcmd -S . -E -C -b -d "%BLOCKED_DB%" -Q "CREATE TABLE dbo.S_CustomerProfile (CustomerId int NOT NULL PRIMARY KEY, LegacyName nvarchar(200) NULL); INSERT INTO dbo.S_CustomerProfile (CustomerId, LegacyName) VALUES (1, N'Legacy Customer');" || exit /b 1

echo.
echo == Fresh target: deploy-test ==
"%META_SQL%" deploy-test fresh || exit /b 1

echo.
echo == Fresh target: deploy ==
"%META_SQL%" deploy fresh || exit /b 1

echo.
echo == Fresh target: deploy-test after deploy ==
"%META_SQL%" deploy-test fresh || exit /b 1

echo.
echo == Blocked target: deploy-test ==
"%META_SQL%" deploy-test blocked || exit /b 1

echo.
echo == Next manual workflow for the blocked target ==
echo meta-sql resolve blocked
echo edit the created SQL script under deploy\migrate\target\blocked
echo meta-sql deploy-test blocked
echo meta-sql deploy blocked

popd
endlocal
