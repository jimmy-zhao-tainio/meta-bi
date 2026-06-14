@echo off
setlocal EnableExtensions EnableDelayedExpansion

set "RUN_DIR=%~dp0.."
pushd "%RUN_DIR%"
if not "!ERRORLEVEL!"=="0" exit /b !ERRORLEVEL!

echo call "%~dp0..\..\..\00-env.cmd"
call "%~dp0..\..\..\00-env.cmd"
if not "!ERRORLEVEL!"=="0" (
  set "EXIT_CODE=!ERRORLEVEL!"
  echo ERROR: 00-env.cmd failed with exit code !EXIT_CODE!.
  popd
  exit /b !EXIT_CODE!
)

set "AW_RDV_DATABASE=AdventureWorksRawVault"
set "AW_RDV_SQL=Server=%AW_SQL_SERVER%;Database=%AW_RDV_DATABASE%;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False"
set "AW_MASTER_SQL=Server=%AW_SQL_SERVER%;Database=master;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False"
set "RDV_IMPLEMENTATION_WORKSPACE=%~dp0..\..\..\..\..\..\MetaDataVault\Workspaces\MetaDataVaultImplementation"

echo AW_RDV_DATABASE=%AW_RDV_DATABASE%
echo AW_RDV_SQL=%AW_RDV_SQL%
echo RDV_IMPLEMENTATION_WORKSPACE=%RDV_IMPLEMENTATION_WORKSPACE%

if not exist "source\AdventureWorks2022\Schema\workspace.xml" (
  echo ERROR: source schema workspace is missing.
  popd
  exit /b 1
)

echo meta-convert schema-to-raw-datavault --source-workspace source\AdventureWorks2022\Schema --new-workspace rdv\AdventureWorksRawVault\RawVault --verbose
meta-convert schema-to-raw-datavault --source-workspace source\AdventureWorks2022\Schema --new-workspace rdv\AdventureWorksRawVault\RawVault --verbose
if not "!ERRORLEVEL!"=="0" (
  set "EXIT_CODE=!ERRORLEVEL!"
  echo ERROR: schema-to-raw-datavault failed with exit code !EXIT_CODE!.
  popd
  exit /b !EXIT_CODE!
)

if not exist "rdv\AdventureWorksRawVault\RawVault\workspace.xml" (
  echo ERROR: raw vault workspace was not created.
  popd
  exit /b 1
)

echo Raw vault counts:
powershell -NoProfile -ExecutionPolicy Bypass -Command "$ErrorActionPreference='Stop'; $base='rdv\AdventureWorksRawVault\RawVault\instances'; foreach($name in 'RawHub','RawHubKeyPart','RawHubSatellite','RawHubSatelliteAttribute','RawLink','RawLinkHub','SourceTable','SourceField'){ $path=Join-Path $base ($name + '.xml'); if(Test-Path -LiteralPath $path){ [xml]$xml=Get-Content -Raw -LiteralPath $path; $count=@($xml.SelectNodes('//' + $name)).Count } else { $count=0 }; Write-Host ($name + ': ' + $count) }"
if not "!ERRORLEVEL!"=="0" (
  set "EXIT_CODE=!ERRORLEVEL!"
  echo ERROR: raw vault count check failed with exit code !EXIT_CODE!.
  popd
  exit /b !EXIT_CODE!
)

echo meta-convert raw-datavault-to-sql --workspace rdv\AdventureWorksRawVault\RawVault --implementation-workspace "%RDV_IMPLEMENTATION_WORKSPACE%" --database-name AdventureWorksRawVault --out rdv\AdventureWorksRawVault\Sql
meta-convert raw-datavault-to-sql --workspace rdv\AdventureWorksRawVault\RawVault --implementation-workspace "%RDV_IMPLEMENTATION_WORKSPACE%" --database-name AdventureWorksRawVault --out rdv\AdventureWorksRawVault\Sql
if not "!ERRORLEVEL!"=="0" (
  set "EXIT_CODE=!ERRORLEVEL!"
  echo ERROR: raw-datavault-to-sql failed with exit code !EXIT_CODE!.
  popd
  exit /b !EXIT_CODE!
)

if not exist "rdv\AdventureWorksRawVault\Sql\workspace.xml" (
  echo ERROR: RDV SQL workspace was not created.
  popd
  exit /b 1
)

echo RDV SQL identifier scan:
powershell -NoProfile -ExecutionPolicy Bypass -Command "$ErrorActionPreference='Stop'; $base='rdv\AdventureWorksRawVault\Sql\instances'; $files='Table.xml','TableColumn.xml','PrimaryKey.xml','ForeignKey.xml'; $bad=@(); foreach($file in $files){ [xml]$xml=Get-Content -Raw -LiteralPath (Join-Path $base $file); foreach($node in @($xml.SelectNodes('//*[@Id]'))){ $name=$node.SelectSingleNode('Name'); if($name -and $name.InnerText.Length -gt 128){ $bad += [pscustomobject]@{File=$file; Name=$name.InnerText; Length=$name.InnerText.Length} } } }; if($bad.Count){ $bad | Format-Table -AutoSize | Out-String -Width 220 | Write-Host; throw 'Identifier scan failed.' }; Write-Host 'Identifier scan OK.'"
if not "!ERRORLEVEL!"=="0" (
  set "EXIT_CODE=!ERRORLEVEL!"
  echo ERROR: RDV SQL identifier scan failed with exit code !EXIT_CODE!.
  popd
  exit /b !EXIT_CODE!
)

echo meta-sql execute --connection-env AW_MASTER_SQL --quiet --query drop AdventureWorksRawVault if it exists
meta-sql execute --connection-env AW_MASTER_SQL --quiet --query "IF DB_ID(N'AdventureWorksRawVault') IS NOT NULL BEGIN ALTER DATABASE [AdventureWorksRawVault] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [AdventureWorksRawVault]; END"
if not "!ERRORLEVEL!"=="0" (
  set "EXIT_CODE=!ERRORLEVEL!"
  echo ERROR: RDV database cleanup failed with exit code !EXIT_CODE!.
  popd
  exit /b !EXIT_CODE!
)

echo meta-sql deploy-plan --source-workspace rdv\AdventureWorksRawVault\Sql --connection-env AW_RDV_SQL --out rdv\AdventureWorksRawVault\DeployManifest
meta-sql deploy-plan --source-workspace rdv\AdventureWorksRawVault\Sql --connection-env AW_RDV_SQL --out rdv\AdventureWorksRawVault\DeployManifest
if not "!ERRORLEVEL!"=="0" (
  set "EXIT_CODE=!ERRORLEVEL!"
  echo ERROR: RDV deploy-plan failed with exit code !EXIT_CODE!.
  popd
  exit /b !EXIT_CODE!
)

echo meta-sql deploy --manifest-workspace rdv\AdventureWorksRawVault\DeployManifest --source-workspace rdv\AdventureWorksRawVault\Sql --connection-env AW_RDV_SQL
meta-sql deploy --manifest-workspace rdv\AdventureWorksRawVault\DeployManifest --source-workspace rdv\AdventureWorksRawVault\Sql --connection-env AW_RDV_SQL
if not "!ERRORLEVEL!"=="0" (
  set "EXIT_CODE=!ERRORLEVEL!"
  echo ERROR: RDV deploy failed with exit code !EXIT_CODE!.
  popd
  exit /b !EXIT_CODE!
)

echo meta-sql deploy-plan --source-workspace rdv\AdventureWorksRawVault\Sql --connection-env AW_RDV_SQL --out rdv\AdventureWorksRawVault\DeployVerifyManifest
meta-sql deploy-plan --source-workspace rdv\AdventureWorksRawVault\Sql --connection-env AW_RDV_SQL --out rdv\AdventureWorksRawVault\DeployVerifyManifest
if not "!ERRORLEVEL!"=="0" (
  set "EXIT_CODE=!ERRORLEVEL!"
  echo ERROR: RDV verify deploy-plan failed with exit code !EXIT_CODE!.
  popd
  exit /b !EXIT_CODE!
)

echo RDV gate passed.
popd
exit /b 0
