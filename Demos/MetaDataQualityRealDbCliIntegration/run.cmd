@echo off
setlocal

set "SQL_SERVER=."
set "DEMO_DB=MetaDataQualityRealDbDemo"
set "METADQ_DB=MetaDQ"

if not "%~1"=="" set "SQL_SERVER=%~1"
set "META_DQ_REAL_DEMO_MASTER_SQL=Server=%SQL_SERVER%;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
set "META_DQ_REAL_DEMO_SOURCE_SQL=Server=%SQL_SERVER%;Database=%DEMO_DB%;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
set "META_DQ_REAL_DEMO_OPERATIONAL_SQL=Server=%SQL_SERVER%;Database=%METADQ_DB%;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"

if exist MetaTransformScript.Workspace rmdir /s /q MetaTransformScript.Workspace
if exist MetaDataQuality.Workspace rmdir /s /q MetaDataQuality.Workspace
if exist DataQualityViews.sql del /q DataQualityViews.sql
if exist supported-candidate-ids.txt del /q supported-candidate-ids.txt

meta-sql execute --connection-env META_DQ_REAL_DEMO_MASTER_SQL --quiet --file cleanup-demo-db.sql --var DEMO_DB=%DEMO_DB% --var METADQ_DB=%METADQ_DB% >nul 2>&1

meta-sql execute --connection-env META_DQ_REAL_DEMO_MASTER_SQL --file setup-demo-db.sql --var DEMO_DB=%DEMO_DB%
@if errorlevel 1 exit /b %errorlevel%

for %%F in (SourceViews\*.sql) do (
  meta-sql execute --connection-env META_DQ_REAL_DEMO_SOURCE_SQL --file "%%F"
  @if errorlevel 1 exit /b %errorlevel%
)

powershell -NoProfile -ExecutionPolicy Bypass -File import-sourceviews-to-transformws.ps1 -SourceViewsPath .\SourceViews -NewWorkspacePath .\MetaTransformScript.Workspace
@if errorlevel 1 exit /b %errorlevel%

meta-data-quality from-transform-workspace --transform-workspace .\MetaTransformScript.Workspace --new-workspace .\MetaDataQuality.Workspace
@if errorlevel 1 exit /b %errorlevel%

meta-data-quality inspect --workspace .\MetaDataQuality.Workspace --show-cases --show-candidate-ids
@if errorlevel 1 exit /b %errorlevel%

powershell -NoProfile -ExecutionPolicy Bypass -File collect-supported-candidate-ids.ps1 -WorkspacePath .\MetaDataQuality.Workspace -OutFile supported-candidate-ids.txt
@if errorlevel 1 exit /b %errorlevel%

for /f %%I in (supported-candidate-ids.txt) do (
  meta-data-quality promote --workspace .\MetaDataQuality.Workspace --candidate-id %%I
  @if errorlevel 1 exit /b %errorlevel%
)

meta-convert data-quality-to-sql --workspace .\MetaDataQuality.Workspace --out .\DataQualityViews.sql
@if errorlevel 1 exit /b %errorlevel%

meta-sql execute --connection-env META_DQ_REAL_DEMO_SOURCE_SQL --file .\DataQualityViews.sql
@if errorlevel 1 exit /b %errorlevel%

meta-sql execute --connection-env META_DQ_REAL_DEMO_OPERATIONAL_SQL --quiet --query "SET NOCOUNT ON; DECLARE @RunId bigint; EXEC dbo.Run @SourceDatabaseName = N'%DEMO_DB%', @RunId = @RunId OUTPUT;"
@if errorlevel 1 exit /b %errorlevel%
