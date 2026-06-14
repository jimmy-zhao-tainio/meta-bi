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

echo meta-schema extract sqlserver --new-workspace source\AdventureWorks2022\Schema --connection-env AW_SOURCE_SQL --system AdventureWorks2022 --all-schemas --all-tables
meta-schema extract sqlserver --new-workspace source\AdventureWorks2022\Schema --connection-env AW_SOURCE_SQL --system AdventureWorks2022 --all-schemas --all-tables
if not "!ERRORLEVEL!"=="0" (
  set "EXIT_CODE=!ERRORLEVEL!"
  echo ERROR: source schema extraction failed with exit code !EXIT_CODE!.
  popd
  exit /b !EXIT_CODE!
)

if not exist "source\AdventureWorks2022\Schema\workspace.xml" (
  echo ERROR: source\AdventureWorks2022\Schema\workspace.xml was not created.
  popd
  exit /b 1
)

echo Source schema counts:
powershell -NoProfile -ExecutionPolicy Bypass -Command "$ErrorActionPreference='Stop'; $base='source\AdventureWorks2022\Schema\instances'; foreach($name in 'System','Schema','Table','Field','FieldDataTypeDetail','TableRelationship'){ $path=Join-Path $base ($name + '.xml'); if(Test-Path -LiteralPath $path){ [xml]$xml=Get-Content -Raw -LiteralPath $path; $count=@($xml.SelectNodes('//' + $name)).Count } else { $count=0 }; Write-Host ($name + ': ' + $count) }"
if not "!ERRORLEVEL!"=="0" (
  set "EXIT_CODE=!ERRORLEVEL!"
  echo ERROR: source count check failed with exit code !EXIT_CODE!.
  popd
  exit /b !EXIT_CODE!
)

echo Alias physical detail check:
powershell -NoProfile -ExecutionPolicy Bypass -Command "$ErrorActionPreference='Stop'; [xml]$fields=Get-Content -Raw -LiteralPath 'source\AdventureWorks2022\Schema\instances\Field.xml'; [xml]$details=Get-Content -Raw -LiteralPath 'source\AdventureWorks2022\Schema\instances\FieldDataTypeDetail.xml'; $field=$fields.SelectNodes('//Field') | Where-Object { $_.MetaDataTypeId -eq 'sqlserver:type:Name' } | Select-Object -First 1; if($null -eq $field){ throw 'No sqlserver:type:Name field found.' }; $length=($details.SelectNodes('//FieldDataTypeDetail') | Where-Object { $_.FieldId -eq $field.Id -and $_.Name -eq 'Length' } | Select-Object -First 1).Value; Write-Host ('FieldId: ' + $field.Id); Write-Host ('FieldName: ' + $field.Name); Write-Host ('MetaDataTypeId: ' + $field.MetaDataTypeId); Write-Host ('Length: ' + $length); if($length -ne '50'){ throw 'Expected an extracted Name alias length of 50.' }"
if not "!ERRORLEVEL!"=="0" (
  set "EXIT_CODE=!ERRORLEVEL!"
  echo ERROR: alias physical detail check failed with exit code !EXIT_CODE!.
  popd
  exit /b !EXIT_CODE!
)

echo Source schema extraction passed.
popd
exit /b 0
