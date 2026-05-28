@echo off
setlocal

if "%META_MULTI_DIMENSIONAL_DEMO_SERVER%"=="" set META_MULTI_DIMENSIONAL_DEMO_SERVER=localhost\MULTI
if "%META_MULTI_DIMENSIONAL_DEMO_DATABASE%"=="" set META_MULTI_DIMENSIONAL_DEMO_DATABASE=MetaBiMultiDimensionalDeployDemo

call cleanup.cmd >nul 2>&1

meta-analytics --new-workspace AnalyticsWorkspace || exit /b 1
pushd AnalyticsWorkspace || exit /b 1

meta-analytics add-model --id Commerce --name Commerce --default-culture en-US || exit /b 1
meta-analytics add-data-source --id Warehouse --model Commerce --name Warehouse --provider SqlServer --connection-reference COMMERCE_DW --source-kind Relational || exit /b 1
meta-analytics add-table --id Date --model Commerce --name Date --kind Dimension --data-category Time || exit /b 1
meta-analytics add-attribute --id DateKey --table Date --name DateKey --data-type-id meta:type:Int32 --is-key true --is-hidden true || exit /b 1
meta-analytics add-table --id Sales --model Commerce --name Sales --kind Fact || exit /b 1
meta-analytics add-attribute --id OrderDateKey --table Sales --name OrderDateKey --data-type-id meta:type:Int32 --is-hidden true || exit /b 1
meta-analytics add-attribute --id SalesAmountColumn --table Sales --name SalesAmount --data-type-id meta:type:Decimal --is-hidden true || exit /b 1
meta-analytics add-relationship --id SalesOrderDate --name OrderDate --role-name OrderDate --relationship-kind Regular --cardinality ManyToOne --from-table Sales --from-attribute OrderDateKey --to-table Date --to-attribute DateKey --is-active true || exit /b 1
meta-analytics add-measure --id SalesAmount --table Sales --source-attribute SalesAmountColumn --name "Sales Amount" --data-type-id meta:type:Decimal --format-string "#,0.00" || exit /b 1
meta-analytics add-aggregation-behavior --id SalesAmountAggregation --measure SalesAmount --function Sum || exit /b 1
meta-analytics add-security-role --id Readers --model Commerce --name Readers --permission Read || exit /b 1

popd

meta-convert analytics-to-multi-dimensional --workspace AnalyticsWorkspace --out MultiDimensionalWorkspace || exit /b 1
pushd MultiDimensionalWorkspace || exit /b 1

meta-multi-dimensional add-named-set --id TopDates --cube Commerce:cube --name TopDates --expression "TOPCOUNT([Date].[DateKey].MEMBERS, 10, [Measures].[Sales Amount])" || exit /b 1
meta-multi-dimensional add-cube-action --id SalesDrillthrough --cube Commerce:cube --name "Sales Drillthrough" --action-type DrillThrough --target-kind Cells --expression DRILLTHROUGH || exit /b 1
meta-multi-dimensional add-cell-permission --id ReaderSalesCells --security-role Readers --cube Commerce:cube --expression "Measures.CurrentMember IS [Measures].[Sales Amount]" || exit /b 1
meta-multi-dimensional add-partition --id SalesCurrent --measure-group "Sales:measure-group" --multi-dimensional-data-source Warehouse --name "Sales Current" --source-expression "SELECT * FROM mart.FactSales" || exit /b 1

popd

meta-multi-dimensional deploy --workspace MultiDimensionalWorkspace --server "%META_MULTI_DIMENSIONAL_DEMO_SERVER%" --database-name "%META_MULTI_DIMENSIONAL_DEMO_DATABASE%" --drop-existing --no-process || exit /b 1

endlocal
