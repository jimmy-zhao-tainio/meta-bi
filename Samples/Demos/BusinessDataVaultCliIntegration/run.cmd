cd /d C:\Users\jimmy\Desktop\meta-bi\Samples\Demos\BusinessDataVaultCliIntegration || exit /b 1
meta-datavault-business --new-workspace .\Workspace || exit /b 1
meta-datavault-business add-hub --workspace .\Workspace --id Customer --name Customer || exit /b 1
meta-datavault-business add-hub --workspace .\Workspace --id CrmCustomer --name CrmCustomer || exit /b 1
meta-datavault-business add-hub --workspace .\Workspace --id Product --name Product || exit /b 1
meta-datavault-business add-hub --workspace .\Workspace --id Supplier --name Supplier || exit /b 1
meta-datavault-business add-hub --workspace .\Workspace --id Order --name Order || exit /b 1
meta-datavault-business add-hub --workspace .\Workspace --id Invoice --name Invoice || exit /b 1
meta-datavault-business add-hub --workspace .\Workspace --id Shipment --name Shipment || exit /b 1
meta-datavault-business add-hub --workspace .\Workspace --id Employee --name Employee || exit /b 1
meta-datavault-business add-hub --workspace .\Workspace --id Department --name Department || exit /b 1
meta-datavault-business add-hub --workspace .\Workspace --id CostCenter --name CostCenter || exit /b 1
meta-datavault-business add-hub-key-part --workspace .\Workspace --id CustomerIdentifier --hub Customer --name CustomerIdentifier --data-type-id meta:type:String --ordinal 1 || exit /b 1
meta-datavault-business add-hub-key-part-data-type-detail --workspace .\Workspace --id CustomerIdentifierLength --hub-key-part CustomerIdentifier --name Length --value 50 || exit /b 1
meta-datavault-business add-hub-key-part --workspace .\Workspace --id CrmCustomerIdentifier --hub CrmCustomer --name CrmCustomerIdentifier --data-type-id meta:type:String --ordinal 1 || exit /b 1
meta-datavault-business add-hub-key-part-data-type-detail --workspace .\Workspace --id CrmCustomerIdentifierLength --hub-key-part CrmCustomerIdentifier --name Length --value 50 || exit /b 1
meta-datavault-business add-hub-key-part --workspace .\Workspace --id ProductIdentifier --hub Product --name ProductIdentifier --data-type-id meta:type:String --ordinal 1 || exit /b 1
meta-datavault-business add-hub-key-part-data-type-detail --workspace .\Workspace --id ProductIdentifierLength --hub-key-part ProductIdentifier --name Length --value 40 || exit /b 1
meta-datavault-business add-hub-key-part --workspace .\Workspace --id SupplierIdentifier --hub Supplier --name SupplierIdentifier --data-type-id meta:type:String --ordinal 1 || exit /b 1
meta-datavault-business add-hub-key-part-data-type-detail --workspace .\Workspace --id SupplierIdentifierLength --hub-key-part SupplierIdentifier --name Length --value 40 || exit /b 1
meta-datavault-business add-hub-key-part --workspace .\Workspace --id OrderIdentifier --hub Order --name OrderIdentifier --data-type-id meta:type:String --ordinal 1 || exit /b 1
meta-datavault-business add-hub-key-part-data-type-detail --workspace .\Workspace --id OrderIdentifierLength --hub-key-part OrderIdentifier --name Length --value 40 || exit /b 1
meta-datavault-business add-hub-key-part --workspace .\Workspace --id InvoiceIdentifier --hub Invoice --name InvoiceIdentifier --data-type-id meta:type:String --ordinal 1 || exit /b 1
meta-datavault-business add-hub-key-part-data-type-detail --workspace .\Workspace --id InvoiceIdentifierLength --hub-key-part InvoiceIdentifier --name Length --value 40 || exit /b 1
meta-datavault-business add-hub-key-part --workspace .\Workspace --id ShipmentIdentifier --hub Shipment --name ShipmentIdentifier --data-type-id meta:type:String --ordinal 1 || exit /b 1
meta-datavault-business add-hub-key-part-data-type-detail --workspace .\Workspace --id ShipmentIdentifierLength --hub-key-part ShipmentIdentifier --name Length --value 40 || exit /b 1
meta-datavault-business add-hub-key-part --workspace .\Workspace --id EmployeeIdentifier --hub Employee --name EmployeeIdentifier --data-type-id meta:type:String --ordinal 1 || exit /b 1
meta-datavault-business add-hub-key-part-data-type-detail --workspace .\Workspace --id EmployeeIdentifierLength --hub-key-part EmployeeIdentifier --name Length --value 30 || exit /b 1
meta-datavault-business add-hub-key-part --workspace .\Workspace --id DepartmentIdentifier --hub Department --name DepartmentIdentifier --data-type-id meta:type:String --ordinal 1 || exit /b 1
meta-datavault-business add-hub-key-part-data-type-detail --workspace .\Workspace --id DepartmentIdentifierLength --hub-key-part DepartmentIdentifier --name Length --value 30 || exit /b 1
meta-datavault-business add-hub-key-part --workspace .\Workspace --id CostCenterIdentifier --hub CostCenter --name CostCenterIdentifier --data-type-id meta:type:String --ordinal 1 || exit /b 1
meta-datavault-business add-hub-key-part-data-type-detail --workspace .\Workspace --id CostCenterIdentifierLength --hub-key-part CostCenterIdentifier --name Length --value 20 || exit /b 1
meta-datavault-business add-link --workspace .\Workspace --id CustomerOrder --name CustomerOrder || exit /b 1
meta-datavault-business add-link-hub --workspace .\Workspace --id CustomerOrderCustomer --link CustomerOrder --hub Customer --ordinal 1 --role-name Customer || exit /b 1
meta-datavault-business add-link-hub --workspace .\Workspace --id CustomerOrderOrder --link CustomerOrder --hub Order --ordinal 2 --role-name Order || exit /b 1
meta-datavault-business add-link --workspace .\Workspace --id OrderProduct --name OrderProduct || exit /b 1
meta-datavault-business add-link-hub --workspace .\Workspace --id OrderProductOrder --link OrderProduct --hub Order --ordinal 1 --role-name Order || exit /b 1
meta-datavault-business add-link-hub --workspace .\Workspace --id OrderProductProduct --link OrderProduct --hub Product --ordinal 2 --role-name Product || exit /b 1
meta-datavault-business add-link --workspace .\Workspace --id SupplierProduct --name SupplierProduct || exit /b 1
meta-datavault-business add-link-hub --workspace .\Workspace --id SupplierProductSupplier --link SupplierProduct --hub Supplier --ordinal 1 --role-name Supplier || exit /b 1
meta-datavault-business add-link-hub --workspace .\Workspace --id SupplierProductProduct --link SupplierProduct --hub Product --ordinal 2 --role-name Product || exit /b 1
meta-datavault-business add-link --workspace .\Workspace --id OrderInvoice --name OrderInvoice || exit /b 1
meta-datavault-business add-link-hub --workspace .\Workspace --id OrderInvoiceOrder --link OrderInvoice --hub Order --ordinal 1 --role-name Order || exit /b 1
meta-datavault-business add-link-hub --workspace .\Workspace --id OrderInvoiceInvoice --link OrderInvoice --hub Invoice --ordinal 2 --role-name Invoice || exit /b 1
meta-datavault-business add-link --workspace .\Workspace --id ShipmentOrder --name ShipmentOrder || exit /b 1
meta-datavault-business add-link-hub --workspace .\Workspace --id ShipmentOrderShipment --link ShipmentOrder --hub Shipment --ordinal 1 --role-name Shipment || exit /b 1
meta-datavault-business add-link-hub --workspace .\Workspace --id ShipmentOrderOrder --link ShipmentOrder --hub Order --ordinal 2 --role-name Order || exit /b 1
meta-datavault-business add-link --workspace .\Workspace --id EmployeeDepartment --name EmployeeDepartment || exit /b 1
meta-datavault-business add-link-hub --workspace .\Workspace --id EmployeeDepartmentEmployee --link EmployeeDepartment --hub Employee --ordinal 1 --role-name Employee || exit /b 1
meta-datavault-business add-link-hub --workspace .\Workspace --id EmployeeDepartmentDepartment --link EmployeeDepartment --hub Department --ordinal 2 --role-name Department || exit /b 1
meta-datavault-business add-link --workspace .\Workspace --id DepartmentCostCenter --name DepartmentCostCenter || exit /b 1
meta-datavault-business add-link-hub --workspace .\Workspace --id DepartmentCostCenterDepartment --link DepartmentCostCenter --hub Department --ordinal 1 --role-name Department || exit /b 1
meta-datavault-business add-link-hub --workspace .\Workspace --id DepartmentCostCenterCostCenter --link DepartmentCostCenter --hub CostCenter --ordinal 2 --role-name CostCenter || exit /b 1
meta-datavault-business add-same-as-link --workspace .\Workspace --id CustomerSameAsCrmCustomer --name CustomerSameAsCrmCustomer --primary-hub Customer --equivalent-hub Customer || exit /b 1
meta-datavault-business add-hierarchical-link --workspace .\Workspace --id DepartmentHierarchy --name DepartmentHierarchy --parent-hub Department --child-hub Department || exit /b 1
meta-datavault-business add-reference --workspace .\Workspace --id OrderStatus --name OrderStatus || exit /b 1
meta-datavault-business add-reference-key-part --workspace .\Workspace --id OrderStatusCode --reference OrderStatus --name OrderStatusCode --data-type-id meta:type:String --ordinal 1 || exit /b 1
meta-datavault-business add-reference-key-part-data-type-detail --workspace .\Workspace --id OrderStatusCodeLength --reference-key-part OrderStatusCode --name Length --value 20 || exit /b 1
meta-datavault-business add-reference --workspace .\Workspace --id CurrencyCode --name CurrencyCode || exit /b 1
meta-datavault-business add-reference-key-part --workspace .\Workspace --id CurrencyCodeValue --reference CurrencyCode --name CurrencyCode --data-type-id meta:type:String --ordinal 1 || exit /b 1
meta-datavault-business add-reference-key-part-data-type-detail --workspace .\Workspace --id CurrencyCodeValueLength --reference-key-part CurrencyCodeValue --name Length --value 3 || exit /b 1
meta-datavault-business add-hub-satellite --workspace .\Workspace --id CustomerProfile --hub Customer --name CustomerProfile --satellite-kind standard || exit /b 1
meta-datavault-business add-hub-satellite-attribute --workspace .\Workspace --id CustomerName --hub-satellite CustomerProfile --name CustomerName --data-type-id meta:type:String --ordinal 1 || exit /b 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace .\Workspace --id CustomerNameLength --hub-satellite-attribute CustomerName --name Length --value 200 || exit /b 1
meta-datavault-business add-hub-satellite-attribute --workspace .\Workspace --id CustomerTier --hub-satellite CustomerProfile --name CustomerTier --data-type-id meta:type:String --ordinal 2 || exit /b 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace .\Workspace --id CustomerTierLength --hub-satellite-attribute CustomerTier --name Length --value 30 || exit /b 1
meta-datavault-business add-hub-satellite --workspace .\Workspace --id ProductProfile --hub Product --name ProductProfile --satellite-kind standard || exit /b 1
meta-datavault-business add-hub-satellite-attribute --workspace .\Workspace --id ProductName --hub-satellite ProductProfile --name ProductName --data-type-id meta:type:String --ordinal 1 || exit /b 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace .\Workspace --id ProductNameLength --hub-satellite-attribute ProductName --name Length --value 200 || exit /b 1
meta-datavault-business add-hub-satellite-attribute --workspace .\Workspace --id ProductCategory --hub-satellite ProductProfile --name ProductCategory --data-type-id meta:type:String --ordinal 2 || exit /b 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace .\Workspace --id ProductCategoryLength --hub-satellite-attribute ProductCategory --name Length --value 50 || exit /b 1
meta-datavault-business add-hub-satellite --workspace .\Workspace --id SupplierProfile --hub Supplier --name SupplierProfile --satellite-kind standard || exit /b 1
meta-datavault-business add-hub-satellite-attribute --workspace .\Workspace --id SupplierName --hub-satellite SupplierProfile --name SupplierName --data-type-id meta:type:String --ordinal 1 || exit /b 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace .\Workspace --id SupplierNameLength --hub-satellite-attribute SupplierName --name Length --value 200 || exit /b 1
meta-datavault-business add-hub-satellite --workspace .\Workspace --id OrderHeader --hub Order --name OrderHeader --satellite-kind standard || exit /b 1
meta-datavault-business add-hub-satellite-attribute --workspace .\Workspace --id OrderDate --hub-satellite OrderHeader --name OrderDate --data-type-id meta:type:DateTime --ordinal 1 || exit /b 1
meta-datavault-business add-hub-satellite-attribute --workspace .\Workspace --id OrderAmount --hub-satellite OrderHeader --name OrderAmount --data-type-id meta:type:Decimal --ordinal 2 || exit /b 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace .\Workspace --id OrderAmountPrecision --hub-satellite-attribute OrderAmount --name Precision --value 18 || exit /b 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace .\Workspace --id OrderAmountScale --hub-satellite-attribute OrderAmount --name Scale --value 2 || exit /b 1
meta-datavault-business add-hub-satellite --workspace .\Workspace --id InvoiceHeader --hub Invoice --name InvoiceHeader --satellite-kind standard || exit /b 1
meta-datavault-business add-hub-satellite-attribute --workspace .\Workspace --id InvoiceDate --hub-satellite InvoiceHeader --name InvoiceDate --data-type-id meta:type:DateTime --ordinal 1 || exit /b 1
meta-datavault-business add-hub-satellite-attribute --workspace .\Workspace --id InvoiceAmount --hub-satellite InvoiceHeader --name InvoiceAmount --data-type-id meta:type:Decimal --ordinal 2 || exit /b 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace .\Workspace --id InvoiceAmountPrecision --hub-satellite-attribute InvoiceAmount --name Precision --value 18 || exit /b 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace .\Workspace --id InvoiceAmountScale --hub-satellite-attribute InvoiceAmount --name Scale --value 2 || exit /b 1
meta-datavault-business add-hub-satellite --workspace .\Workspace --id ShipmentHeader --hub Shipment --name ShipmentHeader --satellite-kind standard || exit /b 1
meta-datavault-business add-hub-satellite-attribute --workspace .\Workspace --id ShipmentDate --hub-satellite ShipmentHeader --name ShipmentDate --data-type-id meta:type:DateTime --ordinal 1 || exit /b 1
meta-datavault-business add-hub-satellite-attribute --workspace .\Workspace --id CarrierName --hub-satellite ShipmentHeader --name CarrierName --data-type-id meta:type:String --ordinal 2 || exit /b 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace .\Workspace --id CarrierNameLength --hub-satellite-attribute CarrierName --name Length --value 100 || exit /b 1
meta-datavault-business add-hub-satellite --workspace .\Workspace --id EmployeeProfile --hub Employee --name EmployeeProfile --satellite-kind standard || exit /b 1
meta-datavault-business add-hub-satellite-attribute --workspace .\Workspace --id EmployeeName --hub-satellite EmployeeProfile --name EmployeeName --data-type-id meta:type:String --ordinal 1 || exit /b 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace .\Workspace --id EmployeeNameLength --hub-satellite-attribute EmployeeName --name Length --value 150 || exit /b 1
meta-datavault-business add-hub-satellite --workspace .\Workspace --id DepartmentProfile --hub Department --name DepartmentProfile --satellite-kind standard || exit /b 1
meta-datavault-business add-hub-satellite-attribute --workspace .\Workspace --id DepartmentName --hub-satellite DepartmentProfile --name DepartmentName --data-type-id meta:type:String --ordinal 1 || exit /b 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace .\Workspace --id DepartmentNameLength --hub-satellite-attribute DepartmentName --name Length --value 150 || exit /b 1
meta-datavault-business add-hub-satellite --workspace .\Workspace --id CostCenterProfile --hub CostCenter --name CostCenterProfile --satellite-kind standard || exit /b 1
meta-datavault-business add-hub-satellite-attribute --workspace .\Workspace --id CostCenterName --hub-satellite CostCenterProfile --name CostCenterName --data-type-id meta:type:String --ordinal 1 || exit /b 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace .\Workspace --id CostCenterNameLength --hub-satellite-attribute CostCenterName --name Length --value 150 || exit /b 1
meta-datavault-business add-link-satellite --workspace .\Workspace --id CustomerOrderStatus --link CustomerOrder --name CustomerOrderStatus --satellite-kind standard || exit /b 1
meta-datavault-business add-link-satellite-attribute --workspace .\Workspace --id CustomerOrderStatusCode --link-satellite CustomerOrderStatus --name OrderStatusCode --data-type-id meta:type:String --ordinal 1 || exit /b 1
meta-datavault-business add-link-satellite-attribute-data-type-detail --workspace .\Workspace --id CustomerOrderStatusCodeLength --link-satellite-attribute CustomerOrderStatusCode --name Length --value 20 || exit /b 1
meta-datavault-business add-link-satellite-attribute --workspace .\Workspace --id CustomerOrderCurrencyCode --link-satellite CustomerOrderStatus --name CurrencyCode --data-type-id meta:type:String --ordinal 2 || exit /b 1
meta-datavault-business add-link-satellite-attribute-data-type-detail --workspace .\Workspace --id CustomerOrderCurrencyCodeLength --link-satellite-attribute CustomerOrderCurrencyCode --name Length --value 3 || exit /b 1
meta-datavault-business add-link-satellite --workspace .\Workspace --id OrderProductLine --link OrderProduct --name OrderProductLine --satellite-kind standard || exit /b 1
meta-datavault-business add-link-satellite-attribute --workspace .\Workspace --id OrderProductQuantity --link-satellite OrderProductLine --name Quantity --data-type-id meta:type:Int32 --ordinal 1 || exit /b 1
meta-datavault-business add-link-satellite-attribute --workspace .\Workspace --id OrderProductUnitPrice --link-satellite OrderProductLine --name UnitPrice --data-type-id meta:type:Decimal --ordinal 2 || exit /b 1
meta-datavault-business add-link-satellite-attribute-data-type-detail --workspace .\Workspace --id OrderProductUnitPricePrecision --link-satellite-attribute OrderProductUnitPrice --name Precision --value 18 || exit /b 1
meta-datavault-business add-link-satellite-attribute-data-type-detail --workspace .\Workspace --id OrderProductUnitPriceScale --link-satellite-attribute OrderProductUnitPrice --name Scale --value 2 || exit /b 1
meta-datavault-business add-link-satellite --workspace .\Workspace --id SupplierProductTerms --link SupplierProduct --name SupplierProductTerms --satellite-kind standard || exit /b 1
meta-datavault-business add-link-satellite-attribute --workspace .\Workspace --id SupplierProductLeadTimeDays --link-satellite SupplierProductTerms --name LeadTimeDays --data-type-id meta:type:Int32 --ordinal 1 || exit /b 1
meta-datavault-business add-link-satellite --workspace .\Workspace --id EmployeeDepartmentAssignment --link EmployeeDepartment --name EmployeeDepartmentAssignment --satellite-kind standard || exit /b 1
meta-datavault-business add-link-satellite-attribute --workspace .\Workspace --id EmployeeDepartmentStartDate --link-satellite EmployeeDepartmentAssignment --name StartDate --data-type-id meta:type:DateTime --ordinal 1 || exit /b 1
meta-datavault-business add-link-satellite --workspace .\Workspace --id DepartmentCostCenterAssignment --link DepartmentCostCenter --name DepartmentCostCenterAssignment --satellite-kind standard || exit /b 1
meta-datavault-business add-link-satellite-attribute --workspace .\Workspace --id DepartmentCostCenterShare --link-satellite DepartmentCostCenterAssignment --name AllocationShare --data-type-id meta:type:Decimal --ordinal 1 || exit /b 1
meta-datavault-business add-link-satellite-attribute-data-type-detail --workspace .\Workspace --id DepartmentCostCenterSharePrecision --link-satellite-attribute DepartmentCostCenterShare --name Precision --value 9 || exit /b 1
meta-datavault-business add-link-satellite-attribute-data-type-detail --workspace .\Workspace --id DepartmentCostCenterShareScale --link-satellite-attribute DepartmentCostCenterShare --name Scale --value 4 || exit /b 1
meta-datavault-business add-same-as-link-satellite --workspace .\Workspace --id CustomerSameAsCrmCustomerAudit --same-as-link CustomerSameAsCrmCustomer --name CustomerSameAsCrmCustomerAudit --satellite-kind standard || exit /b 1
meta-datavault-business add-same-as-link-satellite-attribute --workspace .\Workspace --id CustomerSameAsCrmConfidence --same-as-link-satellite CustomerSameAsCrmCustomerAudit --name MatchConfidence --data-type-id meta:type:Decimal --ordinal 1 || exit /b 1
meta-datavault-business add-same-as-link-satellite-attribute-data-type-detail --workspace .\Workspace --id CustomerSameAsCrmConfidencePrecision --same-as-link-satellite-attribute CustomerSameAsCrmConfidence --name Precision --value 5 || exit /b 1
meta-datavault-business add-same-as-link-satellite-attribute-data-type-detail --workspace .\Workspace --id CustomerSameAsCrmConfidenceScale --same-as-link-satellite-attribute CustomerSameAsCrmConfidence --name Scale --value 2 || exit /b 1
meta-datavault-business add-hierarchical-link-satellite --workspace .\Workspace --id DepartmentHierarchyInfo --hierarchical-link DepartmentHierarchy --name DepartmentHierarchyInfo --satellite-kind standard || exit /b 1
meta-datavault-business add-hierarchical-link-satellite-attribute --workspace .\Workspace --id DepartmentHierarchyLevel --hierarchical-link-satellite DepartmentHierarchyInfo --name HierarchyLevel --data-type-id meta:type:Int32 --ordinal 1 || exit /b 1
meta-datavault-business add-reference-satellite --workspace .\Workspace --id OrderStatusDescriptionSet --reference OrderStatus --name OrderStatusDescriptionSet --satellite-kind standard || exit /b 1
meta-datavault-business add-reference-satellite-attribute --workspace .\Workspace --id OrderStatusDescription --reference-satellite OrderStatusDescriptionSet --name Description --data-type-id meta:type:String --ordinal 1 || exit /b 1
meta-datavault-business add-reference-satellite-attribute-data-type-detail --workspace .\Workspace --id OrderStatusDescriptionLength --reference-satellite-attribute OrderStatusDescription --name Length --value 100 || exit /b 1
meta-datavault-business add-reference-satellite --workspace .\Workspace --id CurrencyCodeDescriptionSet --reference CurrencyCode --name CurrencyCodeDescriptionSet --satellite-kind standard || exit /b 1
meta-datavault-business add-reference-satellite-attribute --workspace .\Workspace --id CurrencyCodeDescription --reference-satellite CurrencyCodeDescriptionSet --name Description --data-type-id meta:type:String --ordinal 1 || exit /b 1
meta-datavault-business add-reference-satellite-attribute-data-type-detail --workspace .\Workspace --id CurrencyCodeDescriptionLength --reference-satellite-attribute CurrencyCodeDescription --name Length --value 100 || exit /b 1
meta-datavault-business add-point-in-time --workspace .\Workspace --id CustomerSnapshot --hub Customer --name CustomerSnapshot || exit /b 1
meta-datavault-business add-point-in-time-stamp --workspace .\Workspace --id CustomerSnapshotBusinessDate --point-in-time CustomerSnapshot --name BusinessDate --data-type-id meta:type:DateTime --ordinal 1 || exit /b 1
meta-datavault-business add-point-in-time-hub-satellite --workspace .\Workspace --id CustomerSnapshotProfile --point-in-time CustomerSnapshot --hub-satellite CustomerProfile --ordinal 1 || exit /b 1
meta-datavault-business add-point-in-time-link-satellite --workspace .\Workspace --id CustomerSnapshotOrderStatus --point-in-time CustomerSnapshot --link-satellite CustomerOrderStatus --ordinal 2 || exit /b 1
meta-datavault-business add-bridge --workspace .\Workspace --id CustomerFulfillmentTraversal --anchor-hub Customer --name CustomerFulfillmentTraversal || exit /b 1
meta-datavault-business add-bridge-link --workspace .\Workspace --id CustomerFulfillmentTraversalCustomerOrder --bridge CustomerFulfillmentTraversal --link CustomerOrder --ordinal 1 --role-name CustomerOrder || exit /b 1
meta-datavault-business add-bridge-hub --workspace .\Workspace --id CustomerFulfillmentTraversalOrder --bridge CustomerFulfillmentTraversal --hub Order --ordinal 2 --role-name Order || exit /b 1
meta-datavault-business add-bridge-link --workspace .\Workspace --id CustomerFulfillmentTraversalShipmentOrder --bridge CustomerFulfillmentTraversal --link ShipmentOrder --ordinal 2 --role-name ShipmentOrder || exit /b 1
meta-datavault-business add-bridge-hub --workspace .\Workspace --id CustomerFulfillmentTraversalShipment --bridge CustomerFulfillmentTraversal --hub Shipment --ordinal 3 --role-name Shipment || exit /b 1
meta-datavault-business add-bridge-hub-key-part-projection --workspace .\Workspace --id CustomerFulfillmentCustomerIdentifier --bridge CustomerFulfillmentTraversal --hub-key-part CustomerIdentifier --name CustomerIdentifier --ordinal 1 || exit /b 1
meta-datavault-business add-bridge-hub-key-part-projection --workspace .\Workspace --id CustomerFulfillmentOrderIdentifier --bridge CustomerFulfillmentTraversal --hub-key-part OrderIdentifier --name OrderIdentifier --ordinal 2 || exit /b 1
meta-datavault-business add-bridge-hub-key-part-projection --workspace .\Workspace --id CustomerFulfillmentShipmentIdentifier --bridge CustomerFulfillmentTraversal --hub-key-part ShipmentIdentifier --name ShipmentIdentifier --ordinal 3 || exit /b 1
meta-datavault-business add-bridge-hub-satellite-attribute-projection --workspace .\Workspace --id CustomerFulfillmentCustomerName --bridge CustomerFulfillmentTraversal --hub-satellite-attribute CustomerName --name CustomerName --ordinal 4 || exit /b 1
meta-datavault-business add-bridge-link-satellite-attribute-projection --workspace .\Workspace --id CustomerFulfillmentOrderStatusCode --bridge CustomerFulfillmentTraversal --link-satellite-attribute CustomerOrderStatusCode --name OrderStatusCode --ordinal 5 || exit /b 1
meta-datavault-business generate-sql --workspace .\Workspace --implementation-workspace C:\Users\jimmy\Desktop\meta-bi\MetaDataVault.Workspaces\MetaDataVaultImplementation --data-type-conversion-workspace C:\Users\jimmy\Desktop\meta-bi\MetaDataTypeConversion.Workspaces\MetaDataTypeConversion --out .\GeneratedSql || exit /b 1
meta deploy sqlserver --scripts .\GeneratedSql --connection-string "Server=.;Integrated Security=true;TrustServerCertificate=true" --database MetaBiBusinessDataVaultCliModelDemo || exit /b 1
