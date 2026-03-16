cd /d C:\Users\jimmy\Desktop\meta-bi\Samples\Demos\RawDataVaultCliIntegration || exit /b 1
meta-datavault-raw --new-workspace .\Workspace || exit /b 1
meta-datavault-raw add-source-system --workspace .\Workspace --id CRM --name CRM || exit /b 1
meta-datavault-raw add-source-system --workspace .\Workspace --id ERP --name ERP || exit /b 1
meta-datavault-raw add-source-system --workspace .\Workspace --id FIN --name FIN || exit /b 1
meta-datavault-raw add-source-system --workspace .\Workspace --id HR --name HR || exit /b 1
meta-datavault-raw add-source-schema --workspace .\Workspace --id CrmDbo --system CRM --name dbo || exit /b 1
meta-datavault-raw add-source-schema --workspace .\Workspace --id ErpDbo --system ERP --name dbo || exit /b 1
meta-datavault-raw add-source-schema --workspace .\Workspace --id FinDbo --system FIN --name dbo || exit /b 1
meta-datavault-raw add-source-schema --workspace .\Workspace --id HrDbo --system HR --name dbo || exit /b 1
meta-datavault-raw add-source-table --workspace .\Workspace --id CustomerTable --schema CrmDbo --name Customer || exit /b 1
meta-datavault-raw add-source-table --workspace .\Workspace --id ProductTable --schema ErpDbo --name Product || exit /b 1
meta-datavault-raw add-source-table --workspace .\Workspace --id SupplierTable --schema FinDbo --name Supplier || exit /b 1
meta-datavault-raw add-source-table --workspace .\Workspace --id OrderTable --schema ErpDbo --name OrderHeader || exit /b 1
meta-datavault-raw add-source-table --workspace .\Workspace --id InvoiceTable --schema FinDbo --name Invoice || exit /b 1
meta-datavault-raw add-source-table --workspace .\Workspace --id ShipmentTable --schema ErpDbo --name Shipment || exit /b 1
meta-datavault-raw add-source-table --workspace .\Workspace --id EmployeeTable --schema HrDbo --name Employee || exit /b 1
meta-datavault-raw add-source-table --workspace .\Workspace --id DepartmentTable --schema HrDbo --name Department || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id CustomerIdField --table CustomerTable --name CustomerId --data-type-id sqlserver:type:nvarchar --ordinal 1 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id CustomerIdLength --field CustomerIdField --name Length --value 50 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id CustomerNameField --table CustomerTable --name CustomerName --data-type-id sqlserver:type:nvarchar --ordinal 2 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id CustomerNameLength --field CustomerNameField --name Length --value 200 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id CustomerEmailField --table CustomerTable --name EmailAddress --data-type-id sqlserver:type:nvarchar --ordinal 3 --is-nullable true || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id CustomerEmailLength --field CustomerEmailField --name Length --value 200 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id ProductIdField --table ProductTable --name ProductId --data-type-id sqlserver:type:nvarchar --ordinal 1 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id ProductIdLength --field ProductIdField --name Length --value 40 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id ProductNameField --table ProductTable --name ProductName --data-type-id sqlserver:type:nvarchar --ordinal 2 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id ProductNameLength --field ProductNameField --name Length --value 200 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id ProductCategoryField --table ProductTable --name ProductCategory --data-type-id sqlserver:type:nvarchar --ordinal 3 --is-nullable true || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id ProductCategoryLength --field ProductCategoryField --name Length --value 50 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id ProductSupplierIdField --table ProductTable --name SupplierId --data-type-id sqlserver:type:nvarchar --ordinal 4 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id ProductSupplierIdLength --field ProductSupplierIdField --name Length --value 40 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id SupplierProductCodeField --table ProductTable --name SupplierProductCode --data-type-id sqlserver:type:nvarchar --ordinal 5 --is-nullable true || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id SupplierProductCodeLength --field SupplierProductCodeField --name Length --value 50 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id SupplierIdField --table SupplierTable --name SupplierId --data-type-id sqlserver:type:nvarchar --ordinal 1 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id SupplierIdLength --field SupplierIdField --name Length --value 40 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id SupplierNameField --table SupplierTable --name SupplierName --data-type-id sqlserver:type:nvarchar --ordinal 2 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id SupplierNameLength --field SupplierNameField --name Length --value 200 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id SupplierRiskClassField --table SupplierTable --name RiskClass --data-type-id sqlserver:type:nvarchar --ordinal 3 --is-nullable true || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id SupplierRiskClassLength --field SupplierRiskClassField --name Length --value 30 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id OrderIdField --table OrderTable --name OrderId --data-type-id sqlserver:type:nvarchar --ordinal 1 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id OrderIdLength --field OrderIdField --name Length --value 40 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id OrderCustomerIdField --table OrderTable --name CustomerId --data-type-id sqlserver:type:nvarchar --ordinal 2 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id OrderCustomerIdLength --field OrderCustomerIdField --name Length --value 50 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id OrderDateField --table OrderTable --name OrderDate --data-type-id sqlserver:type:datetime2 --ordinal 3 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id OrderDatePrecision --field OrderDateField --name Precision --value 7 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id OrderAmountField --table OrderTable --name OrderAmount --data-type-id sqlserver:type:decimal --ordinal 4 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id OrderAmountPrecision --field OrderAmountField --name Precision --value 18 || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id OrderAmountScale --field OrderAmountField --name Scale --value 2 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id OrderStatusCodeField --table OrderTable --name OrderStatusCode --data-type-id sqlserver:type:nvarchar --ordinal 5 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id OrderStatusCodeLength --field OrderStatusCodeField --name Length --value 20 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id InvoiceIdField --table InvoiceTable --name InvoiceId --data-type-id sqlserver:type:nvarchar --ordinal 1 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id InvoiceIdLength --field InvoiceIdField --name Length --value 40 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id InvoiceOrderIdField --table InvoiceTable --name OrderId --data-type-id sqlserver:type:nvarchar --ordinal 2 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id InvoiceOrderIdLength --field InvoiceOrderIdField --name Length --value 40 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id InvoiceSupplierIdField --table InvoiceTable --name SupplierId --data-type-id sqlserver:type:nvarchar --ordinal 3 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id InvoiceSupplierIdLength --field InvoiceSupplierIdField --name Length --value 40 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id InvoiceDateField --table InvoiceTable --name InvoiceDate --data-type-id sqlserver:type:datetime2 --ordinal 4 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id InvoiceDatePrecision --field InvoiceDateField --name Precision --value 7 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id InvoiceAmountField --table InvoiceTable --name InvoiceAmount --data-type-id sqlserver:type:decimal --ordinal 5 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id InvoiceAmountPrecision --field InvoiceAmountField --name Precision --value 18 || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id InvoiceAmountScale --field InvoiceAmountField --name Scale --value 2 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id ShipmentIdField --table ShipmentTable --name ShipmentId --data-type-id sqlserver:type:nvarchar --ordinal 1 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id ShipmentIdLength --field ShipmentIdField --name Length --value 40 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id ShipmentOrderIdField --table ShipmentTable --name OrderId --data-type-id sqlserver:type:nvarchar --ordinal 2 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id ShipmentOrderIdLength --field ShipmentOrderIdField --name Length --value 40 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id ShipmentCarrierNameField --table ShipmentTable --name CarrierName --data-type-id sqlserver:type:nvarchar --ordinal 3 --is-nullable true || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id ShipmentCarrierNameLength --field ShipmentCarrierNameField --name Length --value 100 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id ShipmentDateField --table ShipmentTable --name ShipmentDate --data-type-id sqlserver:type:datetime2 --ordinal 4 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id ShipmentDatePrecision --field ShipmentDateField --name Precision --value 7 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id EmployeeIdField --table EmployeeTable --name EmployeeId --data-type-id sqlserver:type:nvarchar --ordinal 1 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id EmployeeIdLength --field EmployeeIdField --name Length --value 30 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id EmployeeNameField --table EmployeeTable --name EmployeeName --data-type-id sqlserver:type:nvarchar --ordinal 2 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id EmployeeNameLength --field EmployeeNameField --name Length --value 150 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id EmployeeDepartmentIdField --table EmployeeTable --name DepartmentId --data-type-id sqlserver:type:nvarchar --ordinal 3 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id EmployeeDepartmentIdLength --field EmployeeDepartmentIdField --name Length --value 30 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id EmployeeHireDateField --table EmployeeTable --name HireDate --data-type-id sqlserver:type:datetime2 --ordinal 4 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id EmployeeHireDatePrecision --field EmployeeHireDateField --name Precision --value 7 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id DepartmentIdField --table DepartmentTable --name DepartmentId --data-type-id sqlserver:type:nvarchar --ordinal 1 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id DepartmentIdLength --field DepartmentIdField --name Length --value 30 || exit /b 1
meta-datavault-raw add-source-field --workspace .\Workspace --id DepartmentNameField --table DepartmentTable --name DepartmentName --data-type-id sqlserver:type:nvarchar --ordinal 2 --is-nullable false || exit /b 1
meta-datavault-raw add-source-field-data-type-detail --workspace .\Workspace --id DepartmentNameLength --field DepartmentNameField --name Length --value 150 || exit /b 1
meta-datavault-raw add-source-table-relationship --workspace .\Workspace --id OrderCustomerRelationship --source-table OrderTable --target-table CustomerTable --name FK_OrderHeader_Customer || exit /b 1
meta-datavault-raw add-source-table-relationship-field --workspace .\Workspace --id OrderCustomerRelationshipField --relationship OrderCustomerRelationship --source-field OrderCustomerIdField --target-field CustomerIdField --ordinal 1 || exit /b 1
meta-datavault-raw add-source-table-relationship --workspace .\Workspace --id ProductSupplierRelationship --source-table ProductTable --target-table SupplierTable --name FK_Product_Supplier || exit /b 1
meta-datavault-raw add-source-table-relationship-field --workspace .\Workspace --id ProductSupplierRelationshipField --relationship ProductSupplierRelationship --source-field ProductSupplierIdField --target-field SupplierIdField --ordinal 1 || exit /b 1
meta-datavault-raw add-source-table-relationship --workspace .\Workspace --id InvoiceOrderRelationship --source-table InvoiceTable --target-table OrderTable --name FK_Invoice_OrderHeader || exit /b 1
meta-datavault-raw add-source-table-relationship-field --workspace .\Workspace --id InvoiceOrderRelationshipField --relationship InvoiceOrderRelationship --source-field InvoiceOrderIdField --target-field OrderIdField --ordinal 1 || exit /b 1
meta-datavault-raw add-source-table-relationship --workspace .\Workspace --id InvoiceSupplierRelationship --source-table InvoiceTable --target-table SupplierTable --name FK_Invoice_Supplier || exit /b 1
meta-datavault-raw add-source-table-relationship-field --workspace .\Workspace --id InvoiceSupplierRelationshipField --relationship InvoiceSupplierRelationship --source-field InvoiceSupplierIdField --target-field SupplierIdField --ordinal 1 || exit /b 1
meta-datavault-raw add-source-table-relationship --workspace .\Workspace --id ShipmentOrderRelationship --source-table ShipmentTable --target-table OrderTable --name FK_Shipment_OrderHeader || exit /b 1
meta-datavault-raw add-source-table-relationship-field --workspace .\Workspace --id ShipmentOrderRelationshipField --relationship ShipmentOrderRelationship --source-field ShipmentOrderIdField --target-field OrderIdField --ordinal 1 || exit /b 1
meta-datavault-raw add-source-table-relationship --workspace .\Workspace --id EmployeeDepartmentRelationship --source-table EmployeeTable --target-table DepartmentTable --name FK_Employee_Department || exit /b 1
meta-datavault-raw add-source-table-relationship-field --workspace .\Workspace --id EmployeeDepartmentRelationshipField --relationship EmployeeDepartmentRelationship --source-field EmployeeDepartmentIdField --target-field DepartmentIdField --ordinal 1 || exit /b 1
meta-datavault-raw add-hub --workspace .\Workspace --id CustomerHub --source-table CustomerTable --name Customer || exit /b 1
meta-datavault-raw add-hub-key-part --workspace .\Workspace --id CustomerHubKey --hub CustomerHub --source-field CustomerIdField --name CustomerId --ordinal 1 || exit /b 1
meta-datavault-raw add-hub --workspace .\Workspace --id ProductHub --source-table ProductTable --name Product || exit /b 1
meta-datavault-raw add-hub-key-part --workspace .\Workspace --id ProductHubKey --hub ProductHub --source-field ProductIdField --name ProductId --ordinal 1 || exit /b 1
meta-datavault-raw add-hub --workspace .\Workspace --id SupplierHub --source-table SupplierTable --name Supplier || exit /b 1
meta-datavault-raw add-hub-key-part --workspace .\Workspace --id SupplierHubKey --hub SupplierHub --source-field SupplierIdField --name SupplierId --ordinal 1 || exit /b 1
meta-datavault-raw add-hub --workspace .\Workspace --id OrderHub --source-table OrderTable --name Order || exit /b 1
meta-datavault-raw add-hub-key-part --workspace .\Workspace --id OrderHubKey --hub OrderHub --source-field OrderIdField --name OrderId --ordinal 1 || exit /b 1
meta-datavault-raw add-hub --workspace .\Workspace --id InvoiceHub --source-table InvoiceTable --name Invoice || exit /b 1
meta-datavault-raw add-hub-key-part --workspace .\Workspace --id InvoiceHubKey --hub InvoiceHub --source-field InvoiceIdField --name InvoiceId --ordinal 1 || exit /b 1
meta-datavault-raw add-hub --workspace .\Workspace --id ShipmentHub --source-table ShipmentTable --name Shipment || exit /b 1
meta-datavault-raw add-hub-key-part --workspace .\Workspace --id ShipmentHubKey --hub ShipmentHub --source-field ShipmentIdField --name ShipmentId --ordinal 1 || exit /b 1
meta-datavault-raw add-hub --workspace .\Workspace --id EmployeeHub --source-table EmployeeTable --name Employee || exit /b 1
meta-datavault-raw add-hub-key-part --workspace .\Workspace --id EmployeeHubKey --hub EmployeeHub --source-field EmployeeIdField --name EmployeeId --ordinal 1 || exit /b 1
meta-datavault-raw add-hub --workspace .\Workspace --id DepartmentHub --source-table DepartmentTable --name Department || exit /b 1
meta-datavault-raw add-hub-key-part --workspace .\Workspace --id DepartmentHubKey --hub DepartmentHub --source-field DepartmentIdField --name DepartmentId --ordinal 1 || exit /b 1
meta-datavault-raw add-hub-satellite --workspace .\Workspace --id CustomerProfileSat --hub CustomerHub --source-table CustomerTable --name CustomerProfile --satellite-kind standard || exit /b 1
meta-datavault-raw add-hub-satellite-attribute --workspace .\Workspace --id CustomerNameAttr --hub-satellite CustomerProfileSat --source-field CustomerNameField --name CustomerName --ordinal 1 || exit /b 1
meta-datavault-raw add-hub-satellite-attribute --workspace .\Workspace --id CustomerEmailAttr --hub-satellite CustomerProfileSat --source-field CustomerEmailField --name EmailAddress --ordinal 2 || exit /b 1
meta-datavault-raw add-hub-satellite --workspace .\Workspace --id ProductProfileSat --hub ProductHub --source-table ProductTable --name ProductProfile --satellite-kind standard || exit /b 1
meta-datavault-raw add-hub-satellite-attribute --workspace .\Workspace --id ProductNameAttr --hub-satellite ProductProfileSat --source-field ProductNameField --name ProductName --ordinal 1 || exit /b 1
meta-datavault-raw add-hub-satellite-attribute --workspace .\Workspace --id ProductCategoryAttr --hub-satellite ProductProfileSat --source-field ProductCategoryField --name ProductCategory --ordinal 2 || exit /b 1
meta-datavault-raw add-hub-satellite --workspace .\Workspace --id SupplierProfileSat --hub SupplierHub --source-table SupplierTable --name SupplierProfile --satellite-kind standard || exit /b 1
meta-datavault-raw add-hub-satellite-attribute --workspace .\Workspace --id SupplierNameAttr --hub-satellite SupplierProfileSat --source-field SupplierNameField --name SupplierName --ordinal 1 || exit /b 1
meta-datavault-raw add-hub-satellite-attribute --workspace .\Workspace --id SupplierRiskClassAttr --hub-satellite SupplierProfileSat --source-field SupplierRiskClassField --name RiskClass --ordinal 2 || exit /b 1
meta-datavault-raw add-hub-satellite --workspace .\Workspace --id OrderHeaderSat --hub OrderHub --source-table OrderTable --name OrderHeader --satellite-kind standard || exit /b 1
meta-datavault-raw add-hub-satellite-attribute --workspace .\Workspace --id OrderDateAttr --hub-satellite OrderHeaderSat --source-field OrderDateField --name OrderDate --ordinal 1 || exit /b 1
meta-datavault-raw add-hub-satellite-attribute --workspace .\Workspace --id OrderAmountAttr --hub-satellite OrderHeaderSat --source-field OrderAmountField --name OrderAmount --ordinal 2 || exit /b 1
meta-datavault-raw add-hub-satellite --workspace .\Workspace --id InvoiceHeaderSat --hub InvoiceHub --source-table InvoiceTable --name InvoiceHeader --satellite-kind standard || exit /b 1
meta-datavault-raw add-hub-satellite-attribute --workspace .\Workspace --id InvoiceDateAttr --hub-satellite InvoiceHeaderSat --source-field InvoiceDateField --name InvoiceDate --ordinal 1 || exit /b 1
meta-datavault-raw add-hub-satellite-attribute --workspace .\Workspace --id InvoiceAmountAttr --hub-satellite InvoiceHeaderSat --source-field InvoiceAmountField --name InvoiceAmount --ordinal 2 || exit /b 1
meta-datavault-raw add-hub-satellite --workspace .\Workspace --id ShipmentHeaderSat --hub ShipmentHub --source-table ShipmentTable --name ShipmentHeader --satellite-kind standard || exit /b 1
meta-datavault-raw add-hub-satellite-attribute --workspace .\Workspace --id ShipmentCarrierAttr --hub-satellite ShipmentHeaderSat --source-field ShipmentCarrierNameField --name CarrierName --ordinal 1 || exit /b 1
meta-datavault-raw add-hub-satellite-attribute --workspace .\Workspace --id ShipmentDateAttr --hub-satellite ShipmentHeaderSat --source-field ShipmentDateField --name ShipmentDate --ordinal 2 || exit /b 1
meta-datavault-raw add-hub-satellite --workspace .\Workspace --id EmployeeProfileSat --hub EmployeeHub --source-table EmployeeTable --name EmployeeProfile --satellite-kind standard || exit /b 1
meta-datavault-raw add-hub-satellite-attribute --workspace .\Workspace --id EmployeeNameAttr --hub-satellite EmployeeProfileSat --source-field EmployeeNameField --name EmployeeName --ordinal 1 || exit /b 1
meta-datavault-raw add-hub-satellite-attribute --workspace .\Workspace --id EmployeeHireDateAttr --hub-satellite EmployeeProfileSat --source-field EmployeeHireDateField --name HireDate --ordinal 2 || exit /b 1
meta-datavault-raw add-hub-satellite --workspace .\Workspace --id DepartmentProfileSat --hub DepartmentHub --source-table DepartmentTable --name DepartmentProfile --satellite-kind standard || exit /b 1
meta-datavault-raw add-hub-satellite-attribute --workspace .\Workspace --id DepartmentNameAttr --hub-satellite DepartmentProfileSat --source-field DepartmentNameField --name DepartmentName --ordinal 1 || exit /b 1
meta-datavault-raw add-link --workspace .\Workspace --id OrderCustomerLink --source-relationship OrderCustomerRelationship --name OrderCustomer --link-kind standard || exit /b 1
meta-datavault-raw add-link-hub --workspace .\Workspace --id OrderCustomerLinkOrder --link OrderCustomerLink --hub OrderHub --ordinal 1 --role-name Order || exit /b 1
meta-datavault-raw add-link-hub --workspace .\Workspace --id OrderCustomerLinkCustomer --link OrderCustomerLink --hub CustomerHub --ordinal 2 --role-name Customer || exit /b 1
meta-datavault-raw add-link-satellite --workspace .\Workspace --id OrderCustomerSat --link OrderCustomerLink --source-table OrderTable --name OrderCustomerStatus --satellite-kind standard || exit /b 1
meta-datavault-raw add-link-satellite-attribute --workspace .\Workspace --id OrderCustomerStatusAttr --link-satellite OrderCustomerSat --source-field OrderStatusCodeField --name OrderStatusCode --ordinal 1 || exit /b 1
meta-datavault-raw add-link --workspace .\Workspace --id ProductSupplierLink --source-relationship ProductSupplierRelationship --name ProductSupplier --link-kind standard || exit /b 1
meta-datavault-raw add-link-hub --workspace .\Workspace --id ProductSupplierLinkProduct --link ProductSupplierLink --hub ProductHub --ordinal 1 --role-name Product || exit /b 1
meta-datavault-raw add-link-hub --workspace .\Workspace --id ProductSupplierLinkSupplier --link ProductSupplierLink --hub SupplierHub --ordinal 2 --role-name Supplier || exit /b 1
meta-datavault-raw add-link-satellite --workspace .\Workspace --id ProductSupplierSat --link ProductSupplierLink --source-table ProductTable --name ProductSupplierTerms --satellite-kind standard || exit /b 1
meta-datavault-raw add-link-satellite-attribute --workspace .\Workspace --id SupplierProductCodeAttr --link-satellite ProductSupplierSat --source-field SupplierProductCodeField --name SupplierProductCode --ordinal 1 || exit /b 1
meta-datavault-raw add-link --workspace .\Workspace --id InvoiceOrderLink --source-relationship InvoiceOrderRelationship --name InvoiceOrder --link-kind standard || exit /b 1
meta-datavault-raw add-link-hub --workspace .\Workspace --id InvoiceOrderLinkInvoice --link InvoiceOrderLink --hub InvoiceHub --ordinal 1 --role-name Invoice || exit /b 1
meta-datavault-raw add-link-hub --workspace .\Workspace --id InvoiceOrderLinkOrder --link InvoiceOrderLink --hub OrderHub --ordinal 2 --role-name Order || exit /b 1
meta-datavault-raw add-link-satellite --workspace .\Workspace --id InvoiceOrderSat --link InvoiceOrderLink --source-table InvoiceTable --name InvoiceOrderAmount --satellite-kind standard || exit /b 1
meta-datavault-raw add-link-satellite-attribute --workspace .\Workspace --id InvoiceOrderAmountAttr --link-satellite InvoiceOrderSat --source-field InvoiceAmountField --name InvoiceAmount --ordinal 1 || exit /b 1
meta-datavault-raw add-link --workspace .\Workspace --id InvoiceSupplierLink --source-relationship InvoiceSupplierRelationship --name InvoiceSupplier --link-kind standard || exit /b 1
meta-datavault-raw add-link-hub --workspace .\Workspace --id InvoiceSupplierLinkInvoice --link InvoiceSupplierLink --hub InvoiceHub --ordinal 1 --role-name Invoice || exit /b 1
meta-datavault-raw add-link-hub --workspace .\Workspace --id InvoiceSupplierLinkSupplier --link InvoiceSupplierLink --hub SupplierHub --ordinal 2 --role-name Supplier || exit /b 1
meta-datavault-raw add-link --workspace .\Workspace --id ShipmentOrderLink --source-relationship ShipmentOrderRelationship --name ShipmentOrder --link-kind standard || exit /b 1
meta-datavault-raw add-link-hub --workspace .\Workspace --id ShipmentOrderLinkShipment --link ShipmentOrderLink --hub ShipmentHub --ordinal 1 --role-name Shipment || exit /b 1
meta-datavault-raw add-link-hub --workspace .\Workspace --id ShipmentOrderLinkOrder --link ShipmentOrderLink --hub OrderHub --ordinal 2 --role-name Order || exit /b 1
meta-datavault-raw add-link-satellite --workspace .\Workspace --id ShipmentOrderSat --link ShipmentOrderLink --source-table ShipmentTable --name ShipmentOrderTracking --satellite-kind standard || exit /b 1
meta-datavault-raw add-link-satellite-attribute --workspace .\Workspace --id ShipmentOrderCarrierAttr --link-satellite ShipmentOrderSat --source-field ShipmentCarrierNameField --name CarrierName --ordinal 1 || exit /b 1
meta-datavault-raw add-link-satellite-attribute --workspace .\Workspace --id ShipmentOrderDateAttr --link-satellite ShipmentOrderSat --source-field ShipmentDateField --name ShipmentDate --ordinal 2 || exit /b 1
meta-datavault-raw add-link --workspace .\Workspace --id EmployeeDepartmentLink --source-relationship EmployeeDepartmentRelationship --name EmployeeDepartment --link-kind standard || exit /b 1
meta-datavault-raw add-link-hub --workspace .\Workspace --id EmployeeDepartmentLinkEmployee --link EmployeeDepartmentLink --hub EmployeeHub --ordinal 1 --role-name Employee || exit /b 1
meta-datavault-raw add-link-hub --workspace .\Workspace --id EmployeeDepartmentLinkDepartment --link EmployeeDepartmentLink --hub DepartmentHub --ordinal 2 --role-name Department || exit /b 1
meta-datavault-raw add-link-satellite --workspace .\Workspace --id EmployeeDepartmentSat --link EmployeeDepartmentLink --source-table EmployeeTable --name EmployeeDepartmentAssignment --satellite-kind standard || exit /b 1
meta-datavault-raw add-link-satellite-attribute --workspace .\Workspace --id EmployeeDepartmentHireDateAttr --link-satellite EmployeeDepartmentSat --source-field EmployeeHireDateField --name HireDate --ordinal 1 || exit /b 1
echo.
echo Workspace authored: .\Workspace
echo generate-metasql is currently a stub and is not run by this demo.
