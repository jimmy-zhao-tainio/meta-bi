call cleanup.cmd >nul 2>&1

meta-datavault-raw --new-workspace RawDataVaultCliIntegrationWorkspace
cd RawDataVaultCliIntegrationWorkspace
meta-datavault-raw add-source-system --id CRM --name CRM
meta-datavault-raw add-source-system --id ERP --name ERP
meta-datavault-raw add-source-system --id FIN --name FIN
meta-datavault-raw add-source-system --id HR --name HR
meta-datavault-raw add-source-schema --id CrmDbo --system CRM --name dbo
meta-datavault-raw add-source-schema --id ErpDbo --system ERP --name dbo
meta-datavault-raw add-source-schema --id FinDbo --system FIN --name dbo
meta-datavault-raw add-source-schema --id HrDbo --system HR --name dbo
meta-datavault-raw add-source-table --id CustomerTable --schema CrmDbo --name Customer
meta-datavault-raw add-source-table --id ProductTable --schema ErpDbo --name Product
meta-datavault-raw add-source-table --id SupplierTable --schema FinDbo --name Supplier
meta-datavault-raw add-source-table --id OrderTable --schema ErpDbo --name OrderHeader
meta-datavault-raw add-source-table --id InvoiceTable --schema FinDbo --name Invoice
meta-datavault-raw add-source-table --id ShipmentTable --schema ErpDbo --name Shipment
meta-datavault-raw add-source-table --id EmployeeTable --schema HrDbo --name Employee
meta-datavault-raw add-source-table --id DepartmentTable --schema HrDbo --name Department
meta-datavault-raw add-source-field --id CustomerIdField --table CustomerTable --name CustomerId --data-type-id sqlserver:type:nvarchar --ordinal 1 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id CustomerIdLength --field CustomerIdField --name Length --value 50
meta-datavault-raw add-source-field --id CustomerNameField --table CustomerTable --name CustomerName --data-type-id sqlserver:type:nvarchar --ordinal 2 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id CustomerNameLength --field CustomerNameField --name Length --value 200
meta-datavault-raw add-source-field --id CustomerEmailField --table CustomerTable --name EmailAddress --data-type-id sqlserver:type:nvarchar --ordinal 3 --is-nullable true
meta-datavault-raw add-source-field-data-type-detail --id CustomerEmailLength --field CustomerEmailField --name Length --value 200
meta-datavault-raw add-source-field --id ProductIdField --table ProductTable --name ProductId --data-type-id sqlserver:type:nvarchar --ordinal 1 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id ProductIdLength --field ProductIdField --name Length --value 40
meta-datavault-raw add-source-field --id ProductNameField --table ProductTable --name ProductName --data-type-id sqlserver:type:nvarchar --ordinal 2 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id ProductNameLength --field ProductNameField --name Length --value 200
meta-datavault-raw add-source-field --id ProductCategoryField --table ProductTable --name ProductCategory --data-type-id sqlserver:type:nvarchar --ordinal 3 --is-nullable true
meta-datavault-raw add-source-field-data-type-detail --id ProductCategoryLength --field ProductCategoryField --name Length --value 50
meta-datavault-raw add-source-field --id ProductSupplierIdField --table ProductTable --name SupplierId --data-type-id sqlserver:type:nvarchar --ordinal 4 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id ProductSupplierIdLength --field ProductSupplierIdField --name Length --value 40
meta-datavault-raw add-source-field --id SupplierProductCodeField --table ProductTable --name SupplierProductCode --data-type-id sqlserver:type:nvarchar --ordinal 5 --is-nullable true
meta-datavault-raw add-source-field-data-type-detail --id SupplierProductCodeLength --field SupplierProductCodeField --name Length --value 50
meta-datavault-raw add-source-field --id SupplierIdField --table SupplierTable --name SupplierId --data-type-id sqlserver:type:nvarchar --ordinal 1 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id SupplierIdLength --field SupplierIdField --name Length --value 40
meta-datavault-raw add-source-field --id SupplierNameField --table SupplierTable --name SupplierName --data-type-id sqlserver:type:nvarchar --ordinal 2 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id SupplierNameLength --field SupplierNameField --name Length --value 200
meta-datavault-raw add-source-field --id SupplierRiskClassField --table SupplierTable --name RiskClass --data-type-id sqlserver:type:nvarchar --ordinal 3 --is-nullable true
meta-datavault-raw add-source-field-data-type-detail --id SupplierRiskClassLength --field SupplierRiskClassField --name Length --value 30
meta-datavault-raw add-source-field --id OrderIdField --table OrderTable --name OrderId --data-type-id sqlserver:type:nvarchar --ordinal 1 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id OrderIdLength --field OrderIdField --name Length --value 40
meta-datavault-raw add-source-field --id OrderCustomerIdField --table OrderTable --name CustomerId --data-type-id sqlserver:type:nvarchar --ordinal 2 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id OrderCustomerIdLength --field OrderCustomerIdField --name Length --value 50
meta-datavault-raw add-source-field --id OrderDateField --table OrderTable --name OrderDate --data-type-id sqlserver:type:datetime2 --ordinal 3 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id OrderDatePrecision --field OrderDateField --name Precision --value 7
meta-datavault-raw add-source-field --id OrderAmountField --table OrderTable --name OrderAmount --data-type-id sqlserver:type:decimal --ordinal 4 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id OrderAmountPrecision --field OrderAmountField --name Precision --value 18
meta-datavault-raw add-source-field-data-type-detail --id OrderAmountScale --field OrderAmountField --name Scale --value 2
meta-datavault-raw add-source-field --id OrderStatusCodeField --table OrderTable --name OrderStatusCode --data-type-id sqlserver:type:nvarchar --ordinal 5 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id OrderStatusCodeLength --field OrderStatusCodeField --name Length --value 20
meta-datavault-raw add-source-field --id InvoiceIdField --table InvoiceTable --name InvoiceId --data-type-id sqlserver:type:nvarchar --ordinal 1 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id InvoiceIdLength --field InvoiceIdField --name Length --value 40
meta-datavault-raw add-source-field --id InvoiceOrderIdField --table InvoiceTable --name OrderId --data-type-id sqlserver:type:nvarchar --ordinal 2 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id InvoiceOrderIdLength --field InvoiceOrderIdField --name Length --value 40
meta-datavault-raw add-source-field --id InvoiceSupplierIdField --table InvoiceTable --name SupplierId --data-type-id sqlserver:type:nvarchar --ordinal 3 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id InvoiceSupplierIdLength --field InvoiceSupplierIdField --name Length --value 40
meta-datavault-raw add-source-field --id InvoiceDateField --table InvoiceTable --name InvoiceDate --data-type-id sqlserver:type:datetime2 --ordinal 4 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id InvoiceDatePrecision --field InvoiceDateField --name Precision --value 7
meta-datavault-raw add-source-field --id InvoiceAmountField --table InvoiceTable --name InvoiceAmount --data-type-id sqlserver:type:decimal --ordinal 5 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id InvoiceAmountPrecision --field InvoiceAmountField --name Precision --value 18
meta-datavault-raw add-source-field-data-type-detail --id InvoiceAmountScale --field InvoiceAmountField --name Scale --value 2
meta-datavault-raw add-source-field --id ShipmentIdField --table ShipmentTable --name ShipmentId --data-type-id sqlserver:type:nvarchar --ordinal 1 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id ShipmentIdLength --field ShipmentIdField --name Length --value 40
meta-datavault-raw add-source-field --id ShipmentOrderIdField --table ShipmentTable --name OrderId --data-type-id sqlserver:type:nvarchar --ordinal 2 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id ShipmentOrderIdLength --field ShipmentOrderIdField --name Length --value 40
meta-datavault-raw add-source-field --id ShipmentCarrierNameField --table ShipmentTable --name CarrierName --data-type-id sqlserver:type:nvarchar --ordinal 3 --is-nullable true
meta-datavault-raw add-source-field-data-type-detail --id ShipmentCarrierNameLength --field ShipmentCarrierNameField --name Length --value 100
meta-datavault-raw add-source-field --id ShipmentDateField --table ShipmentTable --name ShipmentDate --data-type-id sqlserver:type:datetime2 --ordinal 4 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id ShipmentDatePrecision --field ShipmentDateField --name Precision --value 7
meta-datavault-raw add-source-field --id EmployeeIdField --table EmployeeTable --name EmployeeId --data-type-id sqlserver:type:nvarchar --ordinal 1 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id EmployeeIdLength --field EmployeeIdField --name Length --value 30
meta-datavault-raw add-source-field --id EmployeeNameField --table EmployeeTable --name EmployeeName --data-type-id sqlserver:type:nvarchar --ordinal 2 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id EmployeeNameLength --field EmployeeNameField --name Length --value 150
meta-datavault-raw add-source-field --id EmployeeDepartmentIdField --table EmployeeTable --name DepartmentId --data-type-id sqlserver:type:nvarchar --ordinal 3 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id EmployeeDepartmentIdLength --field EmployeeDepartmentIdField --name Length --value 30
meta-datavault-raw add-source-field --id EmployeeHireDateField --table EmployeeTable --name HireDate --data-type-id sqlserver:type:datetime2 --ordinal 4 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id EmployeeHireDatePrecision --field EmployeeHireDateField --name Precision --value 7
meta-datavault-raw add-source-field --id DepartmentIdField --table DepartmentTable --name DepartmentId --data-type-id sqlserver:type:nvarchar --ordinal 1 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id DepartmentIdLength --field DepartmentIdField --name Length --value 30
meta-datavault-raw add-source-field --id DepartmentNameField --table DepartmentTable --name DepartmentName --data-type-id sqlserver:type:nvarchar --ordinal 2 --is-nullable false
meta-datavault-raw add-source-field-data-type-detail --id DepartmentNameLength --field DepartmentNameField --name Length --value 150
meta-datavault-raw add-source-table-relationship --id OrderCustomerRelationship --source-table OrderTable --target-table CustomerTable --name FK_OrderHeader_Customer
meta-datavault-raw add-source-table-relationship-field --id OrderCustomerRelationshipField --relationship OrderCustomerRelationship --source-field OrderCustomerIdField --target-field CustomerIdField --ordinal 1
meta-datavault-raw add-source-table-relationship --id ProductSupplierRelationship --source-table ProductTable --target-table SupplierTable --name FK_Product_Supplier
meta-datavault-raw add-source-table-relationship-field --id ProductSupplierRelationshipField --relationship ProductSupplierRelationship --source-field ProductSupplierIdField --target-field SupplierIdField --ordinal 1
meta-datavault-raw add-source-table-relationship --id InvoiceOrderRelationship --source-table InvoiceTable --target-table OrderTable --name FK_Invoice_OrderHeader
meta-datavault-raw add-source-table-relationship-field --id InvoiceOrderRelationshipField --relationship InvoiceOrderRelationship --source-field InvoiceOrderIdField --target-field OrderIdField --ordinal 1
meta-datavault-raw add-source-table-relationship --id InvoiceSupplierRelationship --source-table InvoiceTable --target-table SupplierTable --name FK_Invoice_Supplier
meta-datavault-raw add-source-table-relationship-field --id InvoiceSupplierRelationshipField --relationship InvoiceSupplierRelationship --source-field InvoiceSupplierIdField --target-field SupplierIdField --ordinal 1
meta-datavault-raw add-source-table-relationship --id ShipmentOrderRelationship --source-table ShipmentTable --target-table OrderTable --name FK_Shipment_OrderHeader
meta-datavault-raw add-source-table-relationship-field --id ShipmentOrderRelationshipField --relationship ShipmentOrderRelationship --source-field ShipmentOrderIdField --target-field OrderIdField --ordinal 1
meta-datavault-raw add-source-table-relationship --id EmployeeDepartmentRelationship --source-table EmployeeTable --target-table DepartmentTable --name FK_Employee_Department
meta-datavault-raw add-source-table-relationship-field --id EmployeeDepartmentRelationshipField --relationship EmployeeDepartmentRelationship --source-field EmployeeDepartmentIdField --target-field DepartmentIdField --ordinal 1
meta-datavault-raw add-hub --id CustomerHub --source-table CustomerTable --name Customer
meta-datavault-raw add-hub-key-part --id CustomerHubKey --hub CustomerHub --source-field CustomerIdField --name CustomerId --ordinal 1
meta-datavault-raw add-hub --id ProductHub --source-table ProductTable --name Product
meta-datavault-raw add-hub-key-part --id ProductHubKey --hub ProductHub --source-field ProductIdField --name ProductId --ordinal 1
meta-datavault-raw add-hub --id SupplierHub --source-table SupplierTable --name Supplier
meta-datavault-raw add-hub-key-part --id SupplierHubKey --hub SupplierHub --source-field SupplierIdField --name SupplierId --ordinal 1
meta-datavault-raw add-hub --id OrderHub --source-table OrderTable --name Order
meta-datavault-raw add-hub-key-part --id OrderHubKey --hub OrderHub --source-field OrderIdField --name OrderId --ordinal 1
meta-datavault-raw add-hub --id InvoiceHub --source-table InvoiceTable --name Invoice
meta-datavault-raw add-hub-key-part --id InvoiceHubKey --hub InvoiceHub --source-field InvoiceIdField --name InvoiceId --ordinal 1
meta-datavault-raw add-hub --id ShipmentHub --source-table ShipmentTable --name Shipment
meta-datavault-raw add-hub-key-part --id ShipmentHubKey --hub ShipmentHub --source-field ShipmentIdField --name ShipmentId --ordinal 1
meta-datavault-raw add-hub --id EmployeeHub --source-table EmployeeTable --name Employee
meta-datavault-raw add-hub-key-part --id EmployeeHubKey --hub EmployeeHub --source-field EmployeeIdField --name EmployeeId --ordinal 1
meta-datavault-raw add-hub --id DepartmentHub --source-table DepartmentTable --name Department
meta-datavault-raw add-hub-key-part --id DepartmentHubKey --hub DepartmentHub --source-field DepartmentIdField --name DepartmentId --ordinal 1
meta-datavault-raw add-hub-satellite --id CustomerProfileSat --hub CustomerHub --source-table CustomerTable --name CustomerProfile --satellite-kind standard
meta-datavault-raw add-hub-satellite-attribute --id CustomerNameAttr --hub-satellite CustomerProfileSat --source-field CustomerNameField --name CustomerName --ordinal 1
meta-datavault-raw add-hub-satellite-attribute --id CustomerEmailAttr --hub-satellite CustomerProfileSat --source-field CustomerEmailField --name EmailAddress --ordinal 2
meta-datavault-raw add-hub-satellite --id ProductProfileSat --hub ProductHub --source-table ProductTable --name ProductProfile --satellite-kind standard
meta-datavault-raw add-hub-satellite-attribute --id ProductNameAttr --hub-satellite ProductProfileSat --source-field ProductNameField --name ProductName --ordinal 1
meta-datavault-raw add-hub-satellite-attribute --id ProductCategoryAttr --hub-satellite ProductProfileSat --source-field ProductCategoryField --name ProductCategory --ordinal 2
meta-datavault-raw add-hub-satellite --id SupplierProfileSat --hub SupplierHub --source-table SupplierTable --name SupplierProfile --satellite-kind standard
meta-datavault-raw add-hub-satellite-attribute --id SupplierNameAttr --hub-satellite SupplierProfileSat --source-field SupplierNameField --name SupplierName --ordinal 1
meta-datavault-raw add-hub-satellite-attribute --id SupplierRiskClassAttr --hub-satellite SupplierProfileSat --source-field SupplierRiskClassField --name RiskClass --ordinal 2
meta-datavault-raw add-hub-satellite --id OrderHeaderSat --hub OrderHub --source-table OrderTable --name OrderHeader --satellite-kind standard
meta-datavault-raw add-hub-satellite-attribute --id OrderDateAttr --hub-satellite OrderHeaderSat --source-field OrderDateField --name OrderDate --ordinal 1
meta-datavault-raw add-hub-satellite-attribute --id OrderAmountAttr --hub-satellite OrderHeaderSat --source-field OrderAmountField --name OrderAmount --ordinal 2
meta-datavault-raw add-hub-satellite --id InvoiceHeaderSat --hub InvoiceHub --source-table InvoiceTable --name InvoiceHeader --satellite-kind standard
meta-datavault-raw add-hub-satellite-attribute --id InvoiceDateAttr --hub-satellite InvoiceHeaderSat --source-field InvoiceDateField --name InvoiceDate --ordinal 1
meta-datavault-raw add-hub-satellite-attribute --id InvoiceAmountAttr --hub-satellite InvoiceHeaderSat --source-field InvoiceAmountField --name InvoiceAmount --ordinal 2
meta-datavault-raw add-hub-satellite --id ShipmentHeaderSat --hub ShipmentHub --source-table ShipmentTable --name ShipmentHeader --satellite-kind standard
meta-datavault-raw add-hub-satellite-attribute --id ShipmentCarrierAttr --hub-satellite ShipmentHeaderSat --source-field ShipmentCarrierNameField --name CarrierName --ordinal 1
meta-datavault-raw add-hub-satellite-attribute --id ShipmentDateAttr --hub-satellite ShipmentHeaderSat --source-field ShipmentDateField --name ShipmentDate --ordinal 2
meta-datavault-raw add-hub-satellite --id EmployeeProfileSat --hub EmployeeHub --source-table EmployeeTable --name EmployeeProfile --satellite-kind standard
meta-datavault-raw add-hub-satellite-attribute --id EmployeeNameAttr --hub-satellite EmployeeProfileSat --source-field EmployeeNameField --name EmployeeName --ordinal 1
meta-datavault-raw add-hub-satellite-attribute --id EmployeeHireDateAttr --hub-satellite EmployeeProfileSat --source-field EmployeeHireDateField --name HireDate --ordinal 2
meta-datavault-raw add-hub-satellite --id DepartmentProfileSat --hub DepartmentHub --source-table DepartmentTable --name DepartmentProfile --satellite-kind standard
meta-datavault-raw add-hub-satellite-attribute --id DepartmentNameAttr --hub-satellite DepartmentProfileSat --source-field DepartmentNameField --name DepartmentName --ordinal 1
meta-datavault-raw add-link --id OrderCustomerLink --source-relationship OrderCustomerRelationship --name OrderCustomer --link-kind standard
meta-datavault-raw add-link-hub --id OrderCustomerLinkOrder --link OrderCustomerLink --hub OrderHub --ordinal 1 --role-name Order
meta-datavault-raw add-link-hub --id OrderCustomerLinkCustomer --link OrderCustomerLink --hub CustomerHub --ordinal 2 --role-name Customer
meta-datavault-raw add-link-satellite --id OrderCustomerSat --link OrderCustomerLink --source-table OrderTable --name OrderCustomerStatus --satellite-kind standard
meta-datavault-raw add-link-satellite-attribute --id OrderCustomerStatusAttr --link-satellite OrderCustomerSat --source-field OrderStatusCodeField --name OrderStatusCode --ordinal 1
meta-datavault-raw add-link --id ProductSupplierLink --source-relationship ProductSupplierRelationship --name ProductSupplier --link-kind standard
meta-datavault-raw add-link-hub --id ProductSupplierLinkProduct --link ProductSupplierLink --hub ProductHub --ordinal 1 --role-name Product
meta-datavault-raw add-link-hub --id ProductSupplierLinkSupplier --link ProductSupplierLink --hub SupplierHub --ordinal 2 --role-name Supplier
meta-datavault-raw add-link-satellite --id ProductSupplierSat --link ProductSupplierLink --source-table ProductTable --name ProductSupplierTerms --satellite-kind standard
meta-datavault-raw add-link-satellite-attribute --id SupplierProductCodeAttr --link-satellite ProductSupplierSat --source-field SupplierProductCodeField --name SupplierProductCode --ordinal 1
meta-datavault-raw add-link --id InvoiceOrderLink --source-relationship InvoiceOrderRelationship --name InvoiceOrder --link-kind standard
meta-datavault-raw add-link-hub --id InvoiceOrderLinkInvoice --link InvoiceOrderLink --hub InvoiceHub --ordinal 1 --role-name Invoice
meta-datavault-raw add-link-hub --id InvoiceOrderLinkOrder --link InvoiceOrderLink --hub OrderHub --ordinal 2 --role-name Order
meta-datavault-raw add-link-satellite --id InvoiceOrderSat --link InvoiceOrderLink --source-table InvoiceTable --name InvoiceOrderAmount --satellite-kind standard
meta-datavault-raw add-link-satellite-attribute --id InvoiceOrderAmountAttr --link-satellite InvoiceOrderSat --source-field InvoiceAmountField --name InvoiceAmount --ordinal 1
meta-datavault-raw add-link --id InvoiceSupplierLink --source-relationship InvoiceSupplierRelationship --name InvoiceSupplier --link-kind standard
meta-datavault-raw add-link-hub --id InvoiceSupplierLinkInvoice --link InvoiceSupplierLink --hub InvoiceHub --ordinal 1 --role-name Invoice
meta-datavault-raw add-link-hub --id InvoiceSupplierLinkSupplier --link InvoiceSupplierLink --hub SupplierHub --ordinal 2 --role-name Supplier
meta-datavault-raw add-link --id ShipmentOrderLink --source-relationship ShipmentOrderRelationship --name ShipmentOrder --link-kind standard
meta-datavault-raw add-link-hub --id ShipmentOrderLinkShipment --link ShipmentOrderLink --hub ShipmentHub --ordinal 1 --role-name Shipment
meta-datavault-raw add-link-hub --id ShipmentOrderLinkOrder --link ShipmentOrderLink --hub OrderHub --ordinal 2 --role-name Order
meta-datavault-raw add-link-satellite --id ShipmentOrderSat --link ShipmentOrderLink --source-table ShipmentTable --name ShipmentOrderTracking --satellite-kind standard
meta-datavault-raw add-link-satellite-attribute --id ShipmentOrderCarrierAttr --link-satellite ShipmentOrderSat --source-field ShipmentCarrierNameField --name CarrierName --ordinal 1
meta-datavault-raw add-link-satellite-attribute --id ShipmentOrderDateAttr --link-satellite ShipmentOrderSat --source-field ShipmentDateField --name ShipmentDate --ordinal 2
meta-datavault-raw add-link --id EmployeeDepartmentLink --source-relationship EmployeeDepartmentRelationship --name EmployeeDepartment --link-kind standard
meta-datavault-raw add-link-hub --id EmployeeDepartmentLinkEmployee --link EmployeeDepartmentLink --hub EmployeeHub --ordinal 1 --role-name Employee
meta-datavault-raw add-link-hub --id EmployeeDepartmentLinkDepartment --link EmployeeDepartmentLink --hub DepartmentHub --ordinal 2 --role-name Department
meta-datavault-raw add-link-satellite --id EmployeeDepartmentSat --link EmployeeDepartmentLink --source-table EmployeeTable --name EmployeeDepartmentAssignment --satellite-kind standard
meta-datavault-raw add-link-satellite-attribute --id EmployeeDepartmentHireDateAttr --link-satellite EmployeeDepartmentSat --source-field EmployeeHireDateField --name HireDate --ordinal 1

echo.
echo Generating current MetaSql from raw workspace...
meta-datavault-raw generate-metasql --implementation-workspace ..\..\..\..\MetaDataVault\Workspaces\MetaDataVaultImplementation --database-name RawDataVaultCliIntegrationWorkspace --out CurrentMetaSqlWorkspace

echo Planning MetaSql deploy...
meta-sql deploy-plan --source-workspace CurrentMetaSqlWorkspace --connection-string "Server=.;Database=RawDataVaultCliIntegrationWorkspace;Integrated Security=true;TrustServerCertificate=true;Encrypt=false" --out MetaSqlDeployManifest

echo Deploying MetaSql manifest...
meta-sql deploy --manifest-workspace MetaSqlDeployManifest --source-workspace CurrentMetaSqlWorkspace --connection-string "Server=.;Database=RawDataVaultCliIntegrationWorkspace;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"

echo Verifying live schema against current MetaSql...
meta-sql deploy-plan --source-workspace CurrentMetaSqlWorkspace --connection-string "Server=.;Database=RawDataVaultCliIntegrationWorkspace;Integrated Security=true;TrustServerCertificate=true;Encrypt=false" --out MetaSqlVerifyManifest

echo.
echo Workspace authored: RawDataVaultCliIntegrationWorkspace
echo Database deployed: RawDataVaultCliIntegrationWorkspace
echo Current MetaSql workspace: CurrentMetaSqlWorkspace
echo Deploy manifest: MetaSqlDeployManifest
echo Verification manifest: MetaSqlVerifyManifest

