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

echo AW_SQL_SERVER=%AW_SQL_SERVER%
echo AW_SOURCE_DATABASE=%AW_SOURCE_DATABASE%
echo AW_TARGET_DATABASE=%AW_TARGET_DATABASE%
echo AW_TABULAR_SERVER=%AW_TABULAR_SERVER%
echo AW_TABULAR_DATABASE=%AW_TABULAR_DATABASE%

echo meta-sql execute --connection-env AW_SOURCE_SQL --query source readiness row counts
meta-sql execute --connection-env AW_SOURCE_SQL --query "SELECT 'Sales.SalesOrderHeader' AS ObjectName, COUNT_BIG(*) AS RowTotal FROM Sales.SalesOrderHeader UNION ALL SELECT 'Sales.SalesOrderDetail', COUNT_BIG(*) FROM Sales.SalesOrderDetail UNION ALL SELECT 'Production.Product', COUNT_BIG(*) FROM Production.Product UNION ALL SELECT 'Sales.Customer', COUNT_BIG(*) FROM Sales.Customer UNION ALL SELECT 'Sales.Store', COUNT_BIG(*) FROM Sales.Store;"
if not "!ERRORLEVEL!"=="0" (
  set "EXIT_CODE=!ERRORLEVEL!"
  echo ERROR: source readiness query failed with exit code !EXIT_CODE!.
  popd
  exit /b !EXIT_CODE!
)

echo Source readiness passed.
popd
exit /b 0
