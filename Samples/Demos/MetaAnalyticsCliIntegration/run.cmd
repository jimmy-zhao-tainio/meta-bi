call cleanup.cmd >nul 2>&1

meta-analytics --new-workspace MetaAnalyticsCliIntegrationWorkspace
pushd MetaAnalyticsCliIntegrationWorkspace

meta-analytics add-model --id Commerce --name Commerce --default-culture en-US
meta-analytics add-data-source --id Warehouse --model Commerce --name Warehouse --provider SqlServer --connection-reference COMMERCE_DW --source-kind Relational

meta-analytics add-table --id Date --model Commerce --name Date --kind Dimension --data-category Time
meta-analytics add-attribute --id DateKey --table Date --name DateKey --data-type-id meta:type:Int32 --is-key true --is-hidden true
meta-analytics add-attribute --id CalendarDate --table Date --name CalendarDate --data-type-id meta:type:Date
meta-analytics add-attribute --id CalendarYear --table Date --name CalendarYear --data-type-id meta:type:Int32
meta-analytics add-hierarchy --id Calendar --table Date --name Calendar
meta-analytics add-hierarchy-level --id CalendarYearLevel --hierarchy Calendar --attribute CalendarYear --name Year
meta-analytics add-hierarchy-level --id CalendarDateLevel --hierarchy Calendar --attribute CalendarDate --name Date

meta-analytics add-table --id Customer --model Commerce --name Customer --kind Dimension --data-category Customers
meta-analytics add-attribute --id CustomerKey --table Customer --name CustomerKey --data-type-id meta:type:Int64 --is-key true --is-hidden true
meta-analytics add-attribute --id CustomerName --table Customer --name CustomerName --data-type-id meta:type:String
meta-analytics add-attribute --id CustomerRegion --table Customer --name Region --data-type-id meta:type:String

meta-analytics add-table --id Sales --model Commerce --name Sales --kind Fact
meta-analytics add-attribute --id OrderDateKey --table Sales --name OrderDateKey --data-type-id meta:type:Int32 --is-hidden true
meta-analytics add-attribute --id CustomerSalesKey --table Sales --name CustomerKey --data-type-id meta:type:Int64 --is-hidden true
meta-analytics add-attribute --id SalesAmountColumn --table Sales --name SalesAmount --data-type-id meta:type:Decimal --is-hidden true --summarize-by Sum
meta-analytics add-attribute --id SalesTargetColumn --table Sales --name SalesTarget --data-type-id meta:type:Decimal --is-hidden true --summarize-by Sum

meta-analytics add-relationship --id SalesOrderDate --name OrderDate --role-name OrderDate --relationship-kind Regular --cardinality ManyToOne --from-table Sales --from-attribute OrderDateKey --to-table Date --to-attribute DateKey --is-active true
meta-analytics add-relationship --id SalesCustomer --name Customer --role-name Customer --relationship-kind Regular --cardinality ManyToOne --from-table Sales --from-attribute CustomerSalesKey --to-table Customer --to-attribute CustomerKey --is-active true

meta-analytics add-measure --id SalesAmount --table Sales --source-attribute SalesAmountColumn --name "Sales Amount" --data-type-id meta:type:Decimal --format-string "#,0.00" --display-folder Sales
meta-analytics add-aggregation-behavior --id SalesAmountAggregation --measure SalesAmount --function Sum

meta-analytics add-measure --id SalesTarget --table Sales --source-attribute SalesTargetColumn --name "Sales Target" --data-type-id meta:type:Decimal --format-string "#,0.00" --display-folder Sales
meta-analytics add-aggregation-behavior --id SalesTargetAggregation --measure SalesTarget --function Sum

meta-analytics add-perspective --id SalesPerspective --model Commerce --name Sales
meta-analytics add-perspective-table --id SalesPerspectiveSalesTable --perspective SalesPerspective --table Sales
meta-analytics add-perspective-hierarchy --id SalesPerspectiveCalendar --perspective SalesPerspective --hierarchy Calendar
meta-analytics add-perspective-measure --id SalesPerspectiveSalesAmount --perspective SalesPerspective --measure SalesAmount
meta-analytics add-perspective-measure --id SalesPerspectiveSalesTarget --perspective SalesPerspective --measure SalesTarget

meta-analytics add-security-role --id SalesReaders --model Commerce --name SalesReaders --permission Read
meta-analytics add-role-member --id SalesReadersGroup --role SalesReaders --member-name CONTOSO\SalesReaders --member-kind WindowsGroup
meta-analytics add-role-filter --id SalesReadersRegionFilter --role SalesReaders --table Customer --expression-language DAX --expression "Customer[Region] = USERNAME()"
meta-analytics add-table-permission --id SalesReadersSalesTablePermission --role SalesReaders --table Sales --metadata-permission Read
meta-analytics add-attribute-permission --id SalesReadersCustomerRegionPermission --role SalesReaders --attribute CustomerRegion --metadata-permission None

meta-analytics add-culture --id svSE --model Commerce --name sv-SE
meta-analytics add-table-translation --id svSESalesTable --culture svSE --table Sales --caption Forsaljning
meta-analytics add-measure-translation --id svSESalesAmount --culture svSE --measure SalesAmount --caption Forsaljningsbelopp
meta-analytics add-hierarchy-translation --id svSECalendar --culture svSE --hierarchy Calendar --caption Kalender

popd
