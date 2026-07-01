@echo off
setlocal

if "%META_TABULAR_DEMO_SERVER%"=="" set META_TABULAR_DEMO_SERVER=localhost\TABULAR
if "%META_TABULAR_DEMO_DATABASE%"=="" set META_TABULAR_DEMO_DATABASE=MetaBiTabularDeployDemo

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

meta-convert analytics-to-tabular --workspace AnalyticsWorkspace --out TabularWorkspace || exit /b 1
pushd TabularWorkspace || exit /b 1

meta-tabular add-tabular-partition --id DateCurrent --tabular-table Date --tabular-data-source Warehouse --name "Date Current" --mode Import --expression "SELECT * FROM mart.DimDate" || exit /b 1
meta-tabular add-tabular-partition --id SalesCurrent --tabular-table Sales --tabular-data-source Warehouse --name "Sales Current" --mode Import --expression "SELECT * FROM mart.FactSales" || exit /b 1
meta-tabular add-tabular-calculation-group --id TimeIntelligence --tabular-model Commerce --name "Time Intelligence" --precedence 10 || exit /b 1
meta-tabular add-tabular-calculation-item --id TimeYtd --tabular-calculation-group TimeIntelligence --name YTD --expression "CALCULATE(SELECTEDMEASURE(), DATESYTD('Date'[DateKey]))" || exit /b 1
meta-tabular add-tabular-role-filter --id ReaderSalesFilter --tabular-security-role Readers --tabular-table Sales --expression "Sales[SalesAmount] >= 0" || exit /b 1

popd

meta-tabular deploy --workspace TabularWorkspace --server "%META_TABULAR_DEMO_SERVER%" --database-name "%META_TABULAR_DEMO_DATABASE%" --drop-existing --no-process || exit /b 1

endlocal
