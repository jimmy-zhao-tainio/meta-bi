@echo off
setlocal

if "%META_MULTI_DIMENSIONAL_HIERARCHY_DEMO_SERVER%"=="" set META_MULTI_DIMENSIONAL_HIERARCHY_DEMO_SERVER=localhost\MULTI
if "%META_MULTI_DIMENSIONAL_HIERARCHY_DEMO_DATABASE%"=="" set META_MULTI_DIMENSIONAL_HIERARCHY_DEMO_DATABASE=MetaBiMultiDimensionalHierarchyDemo
if "%META_MULTI_DIMENSIONAL_HIERARCHY_DEMO_SOURCE_DATABASE%"=="" set META_MULTI_DIMENSIONAL_HIERARCHY_DEMO_SOURCE_DATABASE=MetaBiMultiDimensionalHierarchySource
if "%META_MULTI_DIMENSIONAL_HIERARCHY_DEMO_SSAS_LOGIN%"=="" set "META_MULTI_DIMENSIONAL_HIERARCHY_DEMO_SSAS_LOGIN=NT Service\MSOLAP$MULTI"
if "%META_MULTI_DIMENSIONAL_HIERARCHY_DEMO_ADMIN_SQL%"=="" set "META_MULTI_DIMENSIONAL_HIERARCHY_DEMO_ADMIN_SQL=Data Source=localhost;Initial Catalog=master;Integrated Security=True;TrustServerCertificate=True"
if "%META_MULTI_DIMENSIONAL_HIERARCHY_DEMO_SOURCE_SQL%"=="" set "META_MULTI_DIMENSIONAL_HIERARCHY_DEMO_SOURCE_SQL=Data Source=localhost;Initial Catalog=%META_MULTI_DIMENSIONAL_HIERARCHY_DEMO_SOURCE_DATABASE%;Integrated Security=True;TrustServerCertificate=True"
if "%COMMERCE_DW%"=="" set "COMMERCE_DW=%META_MULTI_DIMENSIONAL_HIERARCHY_DEMO_SOURCE_SQL%"

call cleanup.cmd >nul 2>&1

meta-sql execute --connection-env META_MULTI_DIMENSIONAL_HIERARCHY_DEMO_ADMIN_SQL --file setup-source.sql --var DEMO_DB=%META_MULTI_DIMENSIONAL_HIERARCHY_DEMO_SOURCE_DATABASE% --var "SSAS_LOGIN=%META_MULTI_DIMENSIONAL_HIERARCHY_DEMO_SSAS_LOGIN%" || exit /b 1

meta-analytics --new-workspace AnalyticsWorkspace || exit /b 1
pushd AnalyticsWorkspace || exit /b 1

meta-analytics add-model --id Commerce --name Commerce --default-culture en-US || exit /b 1
meta-analytics add-data-source --id Warehouse --model Commerce --name Warehouse --provider SqlServer --connection-reference COMMERCE_DW --source-kind Relational || exit /b 1
meta-analytics add-table --id Date --model Commerce --name Date --kind Dimension --data-category Time || exit /b 1
meta-analytics add-attribute --id DateKey --table Date --name DateKey --data-type-id meta:type:Int32 --is-key true --is-hidden true || exit /b 1
meta-analytics add-attribute --id CalendarYear --table Date --name CalendarYear --data-type-id meta:type:Int32 || exit /b 1
meta-analytics add-attribute --id MonthNumber --table Date --name MonthNumber --data-type-id meta:type:Int32 || exit /b 1
meta-analytics add-attribute --id MonthName --table Date --name MonthName --data-type-id meta:type:String || exit /b 1
meta-analytics add-attribute-relationship --id MonthToYear --relationship-type Rigid --child-attribute MonthName --parent-attribute CalendarYear || exit /b 1
meta-analytics add-hierarchy --id Calendar --table Date --name Calendar --kind Natural || exit /b 1
meta-analytics add-hierarchy-level --id CalendarYearLevel --hierarchy Calendar --attribute CalendarYear --name Year --ordinal 10 || exit /b 1
meta-analytics add-hierarchy-level --id CalendarMonthLevel --hierarchy Calendar --attribute MonthName --name Month --ordinal 20 || exit /b 1
meta-analytics add-table --id Sales --model Commerce --name Sales --kind Fact || exit /b 1
meta-analytics add-attribute --id OrderDateKey --table Sales --name DateKey --data-type-id meta:type:Int32 --is-hidden true || exit /b 1
meta-analytics add-attribute --id SalesAmountColumn --table Sales --name SalesAmount --data-type-id meta:type:Decimal --is-hidden true || exit /b 1
meta-analytics add-relationship --id SalesOrderDate --name OrderDate --role-name OrderDate --relationship-kind Regular --cardinality ManyToOne --from-table Sales --from-attribute OrderDateKey --to-table Date --to-attribute DateKey --granularity-attribute DateKey --is-active true || exit /b 1
meta-analytics add-measure --id SalesAmount --table Sales --source-attribute SalesAmountColumn --name "Sales Amount" --data-type-id meta:type:Decimal --format-string "#,0.00" || exit /b 1
meta-analytics add-aggregation-behavior --id SalesAmountAggregation --measure SalesAmount --function Sum || exit /b 1

popd

meta-convert analytics-to-multi-dimensional --workspace AnalyticsWorkspace --out MultiDimensionalWorkspace || exit /b 1
pushd MultiDimensionalWorkspace || exit /b 1

meta-multi-dimensional add-partition --id SalesCurrent --measure-group "Sales:measure-group" --multi-dimensional-data-source Warehouse --name "Sales Current" --source-expression "SELECT DateKey, SalesAmount FROM dbo.Sales" || exit /b 1

popd

meta-multi-dimensional deploy --workspace MultiDimensionalWorkspace --server "%META_MULTI_DIMENSIONAL_HIERARCHY_DEMO_SERVER%" --database-name "%META_MULTI_DIMENSIONAL_HIERARCHY_DEMO_DATABASE%" --drop-existing || exit /b 1
dotnet run --project QueryMdx\QueryMdx.csproj -- "%META_MULTI_DIMENSIONAL_HIERARCHY_DEMO_SERVER%" "%META_MULTI_DIMENSIONAL_HIERARCHY_DEMO_DATABASE%" || exit /b 1

endlocal
