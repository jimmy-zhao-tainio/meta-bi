@echo off
call "%~dp000-env.cmd" || exit /b %errorlevel%

echo.
echo sqlcmd -S "%AW_SQL_SERVER%" -d "%AW_SOURCE_DATABASE%" -E -b -Q "source readiness checks"
sqlcmd -S "%AW_SQL_SERVER%" -d "%AW_SOURCE_DATABASE%" -E -b -Q "SET NOCOUNT ON; SELECT 'AdventureWorks OLTP source OK' AS Status; SELECT COUNT_BIG(*) AS SalesOrderHeaderRows FROM Sales.SalesOrderHeader; SELECT COUNT_BIG(*) AS SalesOrderDetailRows FROM Sales.SalesOrderDetail; SELECT COUNT_BIG(*) AS ProductRows FROM Production.Product; SELECT COUNT_BIG(*) AS CustomerRows FROM Sales.Customer; SELECT COUNT_BIG(*) AS StoreRows FROM Sales.Store;" || exit /b %errorlevel%

exit /b 0
