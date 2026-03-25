call cleanup.cmd >nul 2>&1

meta-datavault-business --new-workspace BusinessDataVaultCliIntegrationWorkspace
meta-datavault-business add-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id Customer --name Customer
meta-datavault-business add-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id CrmCustomer --name CrmCustomer
meta-datavault-business add-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id Product --name Product
meta-datavault-business add-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id Supplier --name Supplier
meta-datavault-business add-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id Order --name Order
meta-datavault-business add-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id Invoice --name Invoice
meta-datavault-business add-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id Shipment --name Shipment
meta-datavault-business add-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id Employee --name Employee
meta-datavault-business add-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id Department --name Department
meta-datavault-business add-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id CostCenter --name CostCenter
meta-datavault-business add-hub-key-part --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerIdentifier --hub Customer --name CustomerIdentifier --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-hub-key-part-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerIdentifierLength --hub-key-part CustomerIdentifier --name Length --value 50
meta-datavault-business add-hub-key-part --workspace BusinessDataVaultCliIntegrationWorkspace --id CrmCustomerIdentifier --hub CrmCustomer --name CrmCustomerIdentifier --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-hub-key-part-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id CrmCustomerIdentifierLength --hub-key-part CrmCustomerIdentifier --name Length --value 50
meta-datavault-business add-hub-key-part --workspace BusinessDataVaultCliIntegrationWorkspace --id ProductIdentifier --hub Product --name ProductIdentifier --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-hub-key-part-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id ProductIdentifierLength --hub-key-part ProductIdentifier --name Length --value 40
meta-datavault-business add-hub-key-part --workspace BusinessDataVaultCliIntegrationWorkspace --id SupplierIdentifier --hub Supplier --name SupplierIdentifier --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-hub-key-part-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id SupplierIdentifierLength --hub-key-part SupplierIdentifier --name Length --value 40
meta-datavault-business add-hub-key-part --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderIdentifier --hub Order --name OrderIdentifier --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-hub-key-part-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderIdentifierLength --hub-key-part OrderIdentifier --name Length --value 40
meta-datavault-business add-hub-key-part --workspace BusinessDataVaultCliIntegrationWorkspace --id InvoiceIdentifier --hub Invoice --name InvoiceIdentifier --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-hub-key-part-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id InvoiceIdentifierLength --hub-key-part InvoiceIdentifier --name Length --value 40
meta-datavault-business add-hub-key-part --workspace BusinessDataVaultCliIntegrationWorkspace --id ShipmentIdentifier --hub Shipment --name ShipmentIdentifier --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-hub-key-part-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id ShipmentIdentifierLength --hub-key-part ShipmentIdentifier --name Length --value 40
meta-datavault-business add-hub-key-part --workspace BusinessDataVaultCliIntegrationWorkspace --id EmployeeIdentifier --hub Employee --name EmployeeIdentifier --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-hub-key-part-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id EmployeeIdentifierLength --hub-key-part EmployeeIdentifier --name Length --value 30
meta-datavault-business add-hub-key-part --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentIdentifier --hub Department --name DepartmentIdentifier --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-hub-key-part-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentIdentifierLength --hub-key-part DepartmentIdentifier --name Length --value 30
meta-datavault-business add-hub-key-part --workspace BusinessDataVaultCliIntegrationWorkspace --id CostCenterIdentifier --hub CostCenter --name CostCenterIdentifier --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-hub-key-part-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id CostCenterIdentifierLength --hub-key-part CostCenterIdentifier --name Length --value 20
meta-datavault-business add-link --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerOrder --name CustomerOrder
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerOrderCustomer --link CustomerOrder --hub Customer --ordinal 1 --role-name Customer
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerOrderOrder --link CustomerOrder --hub Order --ordinal 2 --role-name Order
meta-datavault-business add-link --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderProduct --name OrderProduct
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderProductOrder --link OrderProduct --hub Order --ordinal 1 --role-name Order
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderProductProduct --link OrderProduct --hub Product --ordinal 2 --role-name Product
meta-datavault-business add-link --workspace BusinessDataVaultCliIntegrationWorkspace --id SupplierProduct --name SupplierProduct
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id SupplierProductSupplier --link SupplierProduct --hub Supplier --ordinal 1 --role-name Supplier
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id SupplierProductProduct --link SupplierProduct --hub Product --ordinal 2 --role-name Product
meta-datavault-business add-link --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderInvoice --name OrderInvoice
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderInvoiceOrder --link OrderInvoice --hub Order --ordinal 1 --role-name Order
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderInvoiceInvoice --link OrderInvoice --hub Invoice --ordinal 2 --role-name Invoice
meta-datavault-business add-link --workspace BusinessDataVaultCliIntegrationWorkspace --id ShipmentOrder --name ShipmentOrder
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id ShipmentOrderShipment --link ShipmentOrder --hub Shipment --ordinal 1 --role-name Shipment
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id ShipmentOrderOrder --link ShipmentOrder --hub Order --ordinal 2 --role-name Order
meta-datavault-business add-link --workspace BusinessDataVaultCliIntegrationWorkspace --id EmployeeDepartment --name EmployeeDepartment
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id EmployeeDepartmentEmployee --link EmployeeDepartment --hub Employee --ordinal 1 --role-name Employee
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id EmployeeDepartmentDepartment --link EmployeeDepartment --hub Department --ordinal 2 --role-name Department
meta-datavault-business add-link --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentCostCenter --name DepartmentCostCenter
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentCostCenterDepartment --link DepartmentCostCenter --hub Department --ordinal 1 --role-name Department
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentCostCenterCostCenter --link DepartmentCostCenter --hub CostCenter --ordinal 2 --role-name CostCenter
meta-datavault-business add-link --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerInvoice --name CustomerInvoice
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerInvoiceCustomer --link CustomerInvoice --hub Customer --ordinal 1 --role-name Customer
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerInvoiceInvoice --link CustomerInvoice --hub Invoice --ordinal 2 --role-name Invoice
meta-datavault-business add-link --workspace BusinessDataVaultCliIntegrationWorkspace --id SupplierInvoice --name SupplierInvoice
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id SupplierInvoiceSupplier --link SupplierInvoice --hub Supplier --ordinal 1 --role-name Supplier
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id SupplierInvoiceInvoice --link SupplierInvoice --hub Invoice --ordinal 2 --role-name Invoice
meta-datavault-business add-link --workspace BusinessDataVaultCliIntegrationWorkspace --id InvoiceShipment --name InvoiceShipment
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id InvoiceShipmentInvoice --link InvoiceShipment --hub Invoice --ordinal 1 --role-name Invoice
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id InvoiceShipmentShipment --link InvoiceShipment --hub Shipment --ordinal 2 --role-name Shipment
meta-datavault-business add-link --workspace BusinessDataVaultCliIntegrationWorkspace --id EmployeeCostCenter --name EmployeeCostCenter
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id EmployeeCostCenterEmployee --link EmployeeCostCenter --hub Employee --ordinal 1 --role-name Employee
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id EmployeeCostCenterCostCenter --link EmployeeCostCenter --hub CostCenter --ordinal 2 --role-name CostCenter
meta-datavault-business add-link --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentManager --name DepartmentManager
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentManagerDepartment --link DepartmentManager --hub Department --ordinal 1 --role-name Department
meta-datavault-business add-link-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentManagerEmployee --link DepartmentManager --hub Employee --ordinal 2 --role-name ManagerEmployee
meta-datavault-business add-same-as-link --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerSameAsCrmCustomer --name CustomerSameAsCrmCustomer --primary-hub Customer --equivalent-hub Customer
meta-datavault-business add-hierarchical-link --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentHierarchy --name DepartmentHierarchy --parent-hub Department --child-hub Department
meta-datavault-business add-reference --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderStatus --name OrderStatus
meta-datavault-business add-reference-key-part --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderStatusCode --reference OrderStatus --name OrderStatusCode --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-reference-key-part-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderStatusCodeLength --reference-key-part OrderStatusCode --name Length --value 20
meta-datavault-business add-reference --workspace BusinessDataVaultCliIntegrationWorkspace --id CurrencyCode --name CurrencyCode
meta-datavault-business add-reference-key-part --workspace BusinessDataVaultCliIntegrationWorkspace --id CurrencyCodeValue --reference CurrencyCode --name CurrencyCode --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-reference-key-part-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id CurrencyCodeValueLength --reference-key-part CurrencyCodeValue --name Length --value 3
meta-datavault-business add-hub-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerProfile --hub Customer --name CustomerProfile --satellite-kind standard
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerName --hub-satellite CustomerProfile --name CustomerName --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerNameLength --hub-satellite-attribute CustomerName --name Length --value 200
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerTier --hub-satellite CustomerProfile --name CustomerTier --data-type-id meta:type:String --ordinal 2
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerTierLength --hub-satellite-attribute CustomerTier --name Length --value 30
meta-datavault-business add-hub-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id ProductProfile --hub Product --name ProductProfile --satellite-kind standard
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id ProductName --hub-satellite ProductProfile --name ProductName --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id ProductNameLength --hub-satellite-attribute ProductName --name Length --value 200
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id ProductCategory --hub-satellite ProductProfile --name ProductCategory --data-type-id meta:type:String --ordinal 2
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id ProductCategoryLength --hub-satellite-attribute ProductCategory --name Length --value 50
meta-datavault-business add-hub-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id SupplierProfile --hub Supplier --name SupplierProfile --satellite-kind standard
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id SupplierName --hub-satellite SupplierProfile --name SupplierName --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id SupplierNameLength --hub-satellite-attribute SupplierName --name Length --value 200
meta-datavault-business add-hub-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderHeader --hub Order --name OrderHeader --satellite-kind standard
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderDate --hub-satellite OrderHeader --name OrderDate --data-type-id meta:type:DateTime --ordinal 1
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderAmount --hub-satellite OrderHeader --name OrderAmount --data-type-id meta:type:Decimal --ordinal 2
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderAmountPrecision --hub-satellite-attribute OrderAmount --name Precision --value 18
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderAmountScale --hub-satellite-attribute OrderAmount --name Scale --value 2
meta-datavault-business add-hub-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id InvoiceHeader --hub Invoice --name InvoiceHeader --satellite-kind standard
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id InvoiceDate --hub-satellite InvoiceHeader --name InvoiceDate --data-type-id meta:type:DateTime --ordinal 1
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id InvoiceAmount --hub-satellite InvoiceHeader --name InvoiceAmount --data-type-id meta:type:Decimal --ordinal 2
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id InvoiceAmountPrecision --hub-satellite-attribute InvoiceAmount --name Precision --value 18
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id InvoiceAmountScale --hub-satellite-attribute InvoiceAmount --name Scale --value 2
meta-datavault-business add-hub-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id ShipmentHeader --hub Shipment --name ShipmentHeader --satellite-kind standard
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id ShipmentDate --hub-satellite ShipmentHeader --name ShipmentDate --data-type-id meta:type:DateTime --ordinal 1
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id CarrierName --hub-satellite ShipmentHeader --name CarrierName --data-type-id meta:type:String --ordinal 2
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id CarrierNameLength --hub-satellite-attribute CarrierName --name Length --value 100
meta-datavault-business add-hub-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id EmployeeProfile --hub Employee --name EmployeeProfile --satellite-kind standard
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id EmployeeName --hub-satellite EmployeeProfile --name EmployeeName --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id EmployeeNameLength --hub-satellite-attribute EmployeeName --name Length --value 150
meta-datavault-business add-hub-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentProfile --hub Department --name DepartmentProfile --satellite-kind standard
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentName --hub-satellite DepartmentProfile --name DepartmentName --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentNameLength --hub-satellite-attribute DepartmentName --name Length --value 150
meta-datavault-business add-hub-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id CostCenterProfile --hub CostCenter --name CostCenterProfile --satellite-kind standard
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id CostCenterName --hub-satellite CostCenterProfile --name CostCenterName --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id CostCenterNameLength --hub-satellite-attribute CostCenterName --name Length --value 150
meta-datavault-business add-hub-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerContact --hub Customer --name CustomerContact --satellite-kind standard
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerEmailAddress --hub-satellite CustomerContact --name EmailAddress --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerEmailAddressLength --hub-satellite-attribute CustomerEmailAddress --name Length --value 200
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerPhoneNumber --hub-satellite CustomerContact --name PhoneNumber --data-type-id meta:type:String --ordinal 2
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerPhoneNumberLength --hub-satellite-attribute CustomerPhoneNumber --name Length --value 40
meta-datavault-business add-hub-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerAddress --hub Customer --name CustomerAddress --satellite-kind standard
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerCountryCode --hub-satellite CustomerAddress --name CountryCode --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerCountryCodeLength --hub-satellite-attribute CustomerCountryCode --name Length --value 3
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerCityName --hub-satellite CustomerAddress --name CityName --data-type-id meta:type:String --ordinal 2
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerCityNameLength --hub-satellite-attribute CustomerCityName --name Length --value 100
meta-datavault-business add-hub-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id ProductCommercial --hub Product --name ProductCommercial --satellite-kind standard
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id ProductBrand --hub-satellite ProductCommercial --name BrandName --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id ProductBrandLength --hub-satellite-attribute ProductBrand --name Length --value 100
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id ProductLifecycleStatus --hub-satellite ProductCommercial --name LifecycleStatus --data-type-id meta:type:String --ordinal 2
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id ProductLifecycleStatusLength --hub-satellite-attribute ProductLifecycleStatus --name Length --value 40
meta-datavault-business add-hub-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id SupplierCompliance --hub Supplier --name SupplierCompliance --satellite-kind standard
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id SupplierRiskClass --hub-satellite SupplierCompliance --name RiskClass --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id SupplierRiskClassLength --hub-satellite-attribute SupplierRiskClass --name Length --value 30
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id SupplierTaxIdentifier --hub-satellite SupplierCompliance --name TaxIdentifier --data-type-id meta:type:String --ordinal 2
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id SupplierTaxIdentifierLength --hub-satellite-attribute SupplierTaxIdentifier --name Length --value 50
meta-datavault-business add-hub-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id EmployeeEmployment --hub Employee --name EmployeeEmployment --satellite-kind standard
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id EmployeeHireDate --hub-satellite EmployeeEmployment --name HireDate --data-type-id meta:type:DateTime --ordinal 1
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id EmployeeEmploymentType --hub-satellite EmployeeEmployment --name EmploymentType --data-type-id meta:type:String --ordinal 2
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id EmployeeEmploymentTypeLength --hub-satellite-attribute EmployeeEmploymentType --name Length --value 30
meta-datavault-business add-hub-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentOperatingModel --hub Department --name DepartmentOperatingModel --satellite-kind standard
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentType --hub-satellite DepartmentOperatingModel --name DepartmentType --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentTypeLength --hub-satellite-attribute DepartmentType --name Length --value 40
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentRegionName --hub-satellite DepartmentOperatingModel --name RegionName --data-type-id meta:type:String --ordinal 2
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentRegionNameLength --hub-satellite-attribute DepartmentRegionName --name Length --value 50
meta-datavault-business add-hub-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id CostCenterBudget --hub CostCenter --name CostCenterBudget --satellite-kind standard
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id CostCenterBudgetAmount --hub-satellite CostCenterBudget --name BudgetAmount --data-type-id meta:type:Decimal --ordinal 1
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id CostCenterBudgetAmountPrecision --hub-satellite-attribute CostCenterBudgetAmount --name Precision --value 18
meta-datavault-business add-hub-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id CostCenterBudgetAmountScale --hub-satellite-attribute CostCenterBudgetAmount --name Scale --value 2
meta-datavault-business add-hub-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id CostCenterFiscalYear --hub-satellite CostCenterBudget --name FiscalYear --data-type-id meta:type:Int32 --ordinal 2
meta-datavault-business add-link-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerOrderStatus --link CustomerOrder --name CustomerOrderStatus --satellite-kind standard
meta-datavault-business add-link-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerOrderStatusCode --link-satellite CustomerOrderStatus --name OrderStatusCode --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-link-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerOrderStatusCodeLength --link-satellite-attribute CustomerOrderStatusCode --name Length --value 20
meta-datavault-business add-link-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerOrderCurrencyCode --link-satellite CustomerOrderStatus --name CurrencyCode --data-type-id meta:type:String --ordinal 2
meta-datavault-business add-link-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerOrderCurrencyCodeLength --link-satellite-attribute CustomerOrderCurrencyCode --name Length --value 3
meta-datavault-business add-link-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderProductLine --link OrderProduct --name OrderProductLine --satellite-kind standard
meta-datavault-business add-link-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderProductQuantity --link-satellite OrderProductLine --name Quantity --data-type-id meta:type:Int32 --ordinal 1
meta-datavault-business add-link-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderProductUnitPrice --link-satellite OrderProductLine --name UnitPrice --data-type-id meta:type:Decimal --ordinal 2
meta-datavault-business add-link-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderProductUnitPricePrecision --link-satellite-attribute OrderProductUnitPrice --name Precision --value 18
meta-datavault-business add-link-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderProductUnitPriceScale --link-satellite-attribute OrderProductUnitPrice --name Scale --value 2
meta-datavault-business add-link-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id SupplierProductTerms --link SupplierProduct --name SupplierProductTerms --satellite-kind standard
meta-datavault-business add-link-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id SupplierProductLeadTimeDays --link-satellite SupplierProductTerms --name LeadTimeDays --data-type-id meta:type:Int32 --ordinal 1
meta-datavault-business add-link-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id EmployeeDepartmentAssignment --link EmployeeDepartment --name EmployeeDepartmentAssignment --satellite-kind standard
meta-datavault-business add-link-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id EmployeeDepartmentStartDate --link-satellite EmployeeDepartmentAssignment --name StartDate --data-type-id meta:type:DateTime --ordinal 1
meta-datavault-business add-link-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentCostCenterAssignment --link DepartmentCostCenter --name DepartmentCostCenterAssignment --satellite-kind standard
meta-datavault-business add-link-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentCostCenterShare --link-satellite DepartmentCostCenterAssignment --name AllocationShare --data-type-id meta:type:Decimal --ordinal 1
meta-datavault-business add-link-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentCostCenterSharePrecision --link-satellite-attribute DepartmentCostCenterShare --name Precision --value 9
meta-datavault-business add-link-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentCostCenterShareScale --link-satellite-attribute DepartmentCostCenterShare --name Scale --value 4
meta-datavault-business add-link-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerInvoiceStatus --link CustomerInvoice --name CustomerInvoiceStatus --satellite-kind standard
meta-datavault-business add-link-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerInvoiceBillingStatus --link-satellite CustomerInvoiceStatus --name BillingStatus --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-link-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerInvoiceBillingStatusLength --link-satellite-attribute CustomerInvoiceBillingStatus --name Length --value 30
meta-datavault-business add-link-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id SupplierInvoiceTerms --link SupplierInvoice --name SupplierInvoiceTerms --satellite-kind standard
meta-datavault-business add-link-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id SupplierInvoicePaymentTermsCode --link-satellite SupplierInvoiceTerms --name PaymentTermsCode --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-link-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id SupplierInvoicePaymentTermsCodeLength --link-satellite-attribute SupplierInvoicePaymentTermsCode --name Length --value 20
meta-datavault-business add-link-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id InvoiceShipmentTracking --link InvoiceShipment --name InvoiceShipmentTracking --satellite-kind standard
meta-datavault-business add-link-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id InvoiceShipmentReceiptDate --link-satellite InvoiceShipmentTracking --name ReceiptDate --data-type-id meta:type:DateTime --ordinal 1
meta-datavault-business add-link-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id EmployeeCostCenterAssignment --link EmployeeCostCenter --name EmployeeCostCenterAssignment --satellite-kind standard
meta-datavault-business add-link-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id EmployeeCostCenterAllocationPercentage --link-satellite EmployeeCostCenterAssignment --name AllocationPercentage --data-type-id meta:type:Decimal --ordinal 1
meta-datavault-business add-link-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id EmployeeCostCenterAllocationPercentagePrecision --link-satellite-attribute EmployeeCostCenterAllocationPercentage --name Precision --value 9
meta-datavault-business add-link-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id EmployeeCostCenterAllocationPercentageScale --link-satellite-attribute EmployeeCostCenterAllocationPercentage --name Scale --value 4
meta-datavault-business add-link-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentManagerAssignment --link DepartmentManager --name DepartmentManagerAssignment --satellite-kind standard
meta-datavault-business add-link-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentManagerAssignmentStartDate --link-satellite DepartmentManagerAssignment --name StartDate --data-type-id meta:type:DateTime --ordinal 1
meta-datavault-business add-same-as-link-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerSameAsCrmCustomerAudit --same-as-link CustomerSameAsCrmCustomer --name CustomerSameAsCrmCustomerAudit --satellite-kind standard
meta-datavault-business add-same-as-link-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerSameAsCrmConfidence --same-as-link-satellite CustomerSameAsCrmCustomerAudit --name MatchConfidence --data-type-id meta:type:Decimal --ordinal 1
meta-datavault-business add-same-as-link-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerSameAsCrmConfidencePrecision --same-as-link-satellite-attribute CustomerSameAsCrmConfidence --name Precision --value 5
meta-datavault-business add-same-as-link-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerSameAsCrmConfidenceScale --same-as-link-satellite-attribute CustomerSameAsCrmConfidence --name Scale --value 2
meta-datavault-business add-hierarchical-link-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentHierarchyInfo --hierarchical-link DepartmentHierarchy --name DepartmentHierarchyInfo --satellite-kind standard
meta-datavault-business add-hierarchical-link-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id DepartmentHierarchyLevel --hierarchical-link-satellite DepartmentHierarchyInfo --name HierarchyLevel --data-type-id meta:type:Int32 --ordinal 1
meta-datavault-business add-reference-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderStatusDescriptionSet --reference OrderStatus --name OrderStatusDescriptionSet --satellite-kind standard
meta-datavault-business add-reference-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderStatusDescription --reference-satellite OrderStatusDescriptionSet --name Description --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-reference-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id OrderStatusDescriptionLength --reference-satellite-attribute OrderStatusDescription --name Length --value 100
meta-datavault-business add-reference-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id CurrencyCodeDescriptionSet --reference CurrencyCode --name CurrencyCodeDescriptionSet --satellite-kind standard
meta-datavault-business add-reference-satellite-attribute --workspace BusinessDataVaultCliIntegrationWorkspace --id CurrencyCodeDescription --reference-satellite CurrencyCodeDescriptionSet --name Description --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-reference-satellite-attribute-data-type-detail --workspace BusinessDataVaultCliIntegrationWorkspace --id CurrencyCodeDescriptionLength --reference-satellite-attribute CurrencyCodeDescription --name Length --value 100
meta-datavault-business add-point-in-time --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerSnapshot --hub Customer --name CustomerSnapshot
meta-datavault-business add-point-in-time-stamp --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerSnapshotBusinessDate --point-in-time CustomerSnapshot --name BusinessDate --data-type-id meta:type:DateTime --ordinal 1
meta-datavault-business add-point-in-time-hub-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerSnapshotProfile --point-in-time CustomerSnapshot --hub-satellite CustomerProfile --ordinal 1
meta-datavault-business add-point-in-time-link-satellite --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerSnapshotOrderStatus --point-in-time CustomerSnapshot --link-satellite CustomerOrderStatus --ordinal 2
meta-datavault-business add-bridge --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerFulfillmentTraversal --anchor-hub Customer --name CustomerFulfillmentTraversal
meta-datavault-business add-bridge-link --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerFulfillmentTraversalCustomerOrder --bridge CustomerFulfillmentTraversal --link CustomerOrder --ordinal 1 --role-name CustomerOrder
meta-datavault-business add-bridge-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerFulfillmentTraversalOrder --bridge CustomerFulfillmentTraversal --hub Order --ordinal 2 --role-name Order
meta-datavault-business add-bridge-link --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerFulfillmentTraversalShipmentOrder --bridge CustomerFulfillmentTraversal --link ShipmentOrder --ordinal 2 --role-name ShipmentOrder
meta-datavault-business add-bridge-hub --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerFulfillmentTraversalShipment --bridge CustomerFulfillmentTraversal --hub Shipment --ordinal 3 --role-name Shipment
meta-datavault-business add-bridge-hub-key-part-projection --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerFulfillmentCustomerIdentifier --bridge CustomerFulfillmentTraversal --hub-key-part CustomerIdentifier --name CustomerIdentifier --ordinal 1
meta-datavault-business add-bridge-hub-key-part-projection --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerFulfillmentOrderIdentifier --bridge CustomerFulfillmentTraversal --hub-key-part OrderIdentifier --name OrderIdentifier --ordinal 2
meta-datavault-business add-bridge-hub-key-part-projection --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerFulfillmentShipmentIdentifier --bridge CustomerFulfillmentTraversal --hub-key-part ShipmentIdentifier --name ShipmentIdentifier --ordinal 3
meta-datavault-business add-bridge-hub-satellite-attribute-projection --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerFulfillmentCustomerName --bridge CustomerFulfillmentTraversal --hub-satellite-attribute CustomerName --name CustomerName --ordinal 4
meta-datavault-business add-bridge-link-satellite-attribute-projection --workspace BusinessDataVaultCliIntegrationWorkspace --id CustomerFulfillmentOrderStatusCode --bridge CustomerFulfillmentTraversal --link-satellite-attribute CustomerOrderStatusCode --name OrderStatusCode --ordinal 5

echo.
echo Generating current MetaSql from business workspace...
meta-datavault-business generate-metasql --workspace "BusinessDataVaultCliIntegrationWorkspace" --implementation-workspace "..\..\..\MetaDataVault\Workspaces\MetaDataVaultImplementation" --database-name "BusinessDataVaultCliIntegrationWorkspace" --schema dbo --out "CurrentMetaSqlWorkspace"

echo Planning MetaSql deploy...
meta-sql deploy-plan --source-workspace "CurrentMetaSqlWorkspace" --connection-string "Server=.;Database=BusinessDataVaultCliIntegrationWorkspace;Integrated Security=true;TrustServerCertificate=true;Encrypt=false" --schema dbo --out "MetaSqlDeployManifest"

echo Deploying MetaSql manifest...
meta-sql deploy --manifest-workspace "MetaSqlDeployManifest" --source-workspace "CurrentMetaSqlWorkspace" --connection-string "Server=.;Database=BusinessDataVaultCliIntegrationWorkspace;Integrated Security=true;TrustServerCertificate=true;Encrypt=false" --schema dbo

echo Verifying live schema against current MetaSql...
meta-sql deploy-plan --source-workspace "CurrentMetaSqlWorkspace" --connection-string "Server=.;Database=BusinessDataVaultCliIntegrationWorkspace;Integrated Security=true;TrustServerCertificate=true;Encrypt=false" --schema dbo --out "MetaSqlVerifyManifest"

echo.
echo Workspace authored: BusinessDataVaultCliIntegrationWorkspace
echo Database deployed: BusinessDataVaultCliIntegrationWorkspace
echo Current MetaSql workspace: CurrentMetaSqlWorkspace
echo Deploy manifest: MetaSqlDeployManifest
echo Verification manifest: MetaSqlVerifyManifest
