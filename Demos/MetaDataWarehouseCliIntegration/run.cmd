@set "META_BI_DW_SQL=Server=.;Database=DataWarehouseCliIntegration;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"

call cleanup.cmd >nul 2>&1

meta-data-warehouse --new-workspace MetaDataWarehouseCliIntegrationWorkspace
pushd MetaDataWarehouseCliIntegrationWorkspace

meta-data-warehouse add-warehouse --id Commerce --name Commerce

meta-data-warehouse add-dimension --id Date --warehouse Commerce --name Date
meta-data-warehouse add-dimension-attribute --id CalendarDateKey --dimension Date --name CalendarDateKey --data-type-id meta:type:Int32
meta-data-warehouse add-dimension-attribute --id CalendarDate --dimension Date --name CalendarDate --data-type-id meta:type:Date
meta-data-warehouse add-dimension-business-key --id DateBusinessKey --dimension Date --name DateBusinessKey
meta-data-warehouse add-dimension-business-key-part --id DateBusinessKeyPart --business-key DateBusinessKey --attribute CalendarDateKey
meta-data-warehouse add-conformed-dimension --id EnterpriseDate --dimension Date --conformance-name EnterpriseDate

meta-data-warehouse add-dimension --id Customer --warehouse Commerce --name Customer
meta-data-warehouse add-dimension-attribute --id CustomerNumber --dimension Customer --name CustomerNumber --data-type-id meta:type:String
meta-data-warehouse add-dimension-attribute --id CustomerName --dimension Customer --name CustomerName --data-type-id meta:type:String
meta-data-warehouse add-dimension-attribute --id CustomerTier --dimension Customer --name CustomerTier --data-type-id meta:type:String
meta-data-warehouse add-dimension-business-key --id CustomerBusinessKey --dimension Customer --name CustomerBusinessKey
meta-data-warehouse add-dimension-business-key-part --id CustomerBusinessKeyPart --business-key CustomerBusinessKey --attribute CustomerNumber
meta-data-warehouse add-slowly-changing-dimension --id CustomerHistory --dimension Customer --name CustomerHistory
meta-data-warehouse add-type2-dimension-attribute --id CustomerHistoryName --slowly-changing-dimension CustomerHistory --attribute CustomerName
meta-data-warehouse add-type1-dimension-attribute --id CustomerHistoryTier --slowly-changing-dimension CustomerHistory --attribute CustomerTier

meta-data-warehouse add-dimension --id Product --warehouse Commerce --name Product
meta-data-warehouse add-dimension-attribute --id ProductNumber --dimension Product --name ProductNumber --data-type-id meta:type:String
meta-data-warehouse add-dimension-attribute --id ProductName --dimension Product --name ProductName --data-type-id meta:type:String
meta-data-warehouse add-dimension-business-key --id ProductBusinessKey --dimension Product --name ProductBusinessKey
meta-data-warehouse add-dimension-business-key-part --id ProductBusinessKeyPart --business-key ProductBusinessKey --attribute ProductNumber

meta-data-warehouse add-fact --id SalesOrder --warehouse Commerce --name SalesOrder
meta-data-warehouse add-transaction-fact --id SalesOrderTransaction --fact SalesOrder
meta-data-warehouse add-fact-grain --id SalesOrderLineGrain --fact SalesOrder --name SalesOrderLine --description One-row-per-sales-order-line
meta-data-warehouse add-fact-dimension --id SalesOrderOrderDate --fact SalesOrder --dimension Date --role-name OrderDate
meta-data-warehouse add-fact-dimension --id SalesOrderShipDate --fact SalesOrder --dimension Date --role-name ShipDate --is-required false
meta-data-warehouse add-fact-dimension --id SalesOrderCustomer --fact SalesOrder --dimension Customer --role-name Customer
meta-data-warehouse add-fact-dimension --id SalesOrderProduct --fact SalesOrder --dimension Product --role-name Product
meta-data-warehouse add-degenerate-dimension --id SalesOrderNumber --fact SalesOrder --name OrderNumber --data-type-id meta:type:String
meta-data-warehouse add-fact-measure --id SalesQuantity --fact SalesOrder --name Quantity --data-type-id meta:type:Int32
meta-data-warehouse add-fact-measure --id SalesAmount --fact SalesOrder --name SalesAmount --data-type-id meta:type:Decimal

meta-convert data-warehouse-to-sql --workspace . --implementation-workspace ..\..\..\..\MetaDataWarehouse\Workspaces\MetaDataWarehouseImplementation --database-name DataWarehouseCliIntegration --out CurrentMetaSqlWorkspace
if errorlevel 1 exit /b 1

meta-sql deploy-plan --source-workspace CurrentMetaSqlWorkspace --connection-env META_BI_DW_SQL --out MetaSqlDeployManifest
if errorlevel 1 exit /b 1

meta-sql deploy --manifest-workspace MetaSqlDeployManifest --source-workspace CurrentMetaSqlWorkspace --connection-env META_BI_DW_SQL
if errorlevel 1 exit /b 1

meta-sql deploy-plan --source-workspace CurrentMetaSqlWorkspace --connection-env META_BI_DW_SQL --out MetaSqlVerifyManifest
if errorlevel 1 exit /b 1

popd
