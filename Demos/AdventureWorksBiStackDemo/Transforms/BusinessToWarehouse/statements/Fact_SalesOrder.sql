WITH latestOrder AS (
    SELECT source.[HubHashKey], source.[OrderDate], source.[DueDate], source.[ShipDate], source.[Status], source.[OnlineOrderFlag],
        source.[SubTotal], source.[TaxAmount], source.[FreightAmount], source.[TotalDue],
        ROW_NUMBER() OVER (PARTITION BY source.[HubHashKey] ORDER BY source.[LoadTimestamp] DESC, source.[AuditId] DESC) AS [VersionRank]
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_SalesOrder_SalesOrderHeader] AS source
)
MERGE INTO [AdventureWorksAnalytics].[dw].[Fact_SalesOrder] AS target
USING (
    SELECT orderDate.[DateKey] AS [OrderDateKey], dueDate.[DateKey] AS [DueDateKey], shipDate.[DateKey] AS [ShipDateKey],
        customer.[CustomerKey], salesperson.[SalespersonKey], territory.[SalesTerritoryKey], channel.[SalesChannelKey],
        orderStatus.[OrderStatusKey], billToGeography.[GeographyKey] AS [BillToGeographyKey], shipToGeography.[GeographyKey] AS [ShipToGeographyKey],
        salesOrder.[SalesOrderID], CONVERT(int, 1) AS [OrderCount],
        CONVERT(decimal(18,4), detail.[SubTotal]) AS [SubTotal],
        CONVERT(decimal(18,4), detail.[TaxAmount]) AS [TaxAmount],
        CONVERT(decimal(18,4), detail.[FreightAmount]) AS [FreightAmount],
        CONVERT(decimal(18,4), detail.[TotalDue]) AS [TotalDue]
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS salesOrder
    INNER JOIN latestOrder AS detail
        ON detail.[HubHashKey] = salesOrder.[HashKey] AND detail.[VersionRank] = 1
    INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderCustomer] AS salesOrderCustomer
        ON salesOrderCustomer.[SalesOrderHashKey] = salesOrder.[HashKey]
    INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS businessCustomer
        ON businessCustomer.[HashKey] = salesOrderCustomer.[CustomerHashKey]
    INNER JOIN [AdventureWorksAnalytics].[dw].[Dim_Customer] AS customer
        ON customer.[CustomerID] = businessCustomer.[CustomerID]
    LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesPerson] AS salesOrderSalesperson
        ON salesOrderSalesperson.[SalesOrderHashKey] = salesOrder.[HashKey]
    LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS businessSalesperson
        ON businessSalesperson.[HashKey] = salesOrderSalesperson.[SalesPersonHashKey]
    LEFT OUTER JOIN [AdventureWorksAnalytics].[dw].[Dim_Salesperson] AS salesperson
        ON salesperson.[BusinessEntityID] = businessSalesperson.[BusinessEntityID]
    LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesTerritory] AS salesOrderTerritory
        ON salesOrderTerritory.[SalesOrderHashKey] = salesOrder.[HashKey]
    LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS businessTerritory
        ON businessTerritory.[HashKey] = salesOrderTerritory.[SalesTerritoryHashKey]
    LEFT OUTER JOIN [AdventureWorksAnalytics].[dw].[Dim_SalesTerritory] AS territory
        ON territory.[TerritoryID] = businessTerritory.[TerritoryID]
    INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderBillToAddress] AS salesOrderBillToAddress
        ON salesOrderBillToAddress.[SalesOrderHashKey] = salesOrder.[HashKey]
    INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Address] AS businessBillToAddress
        ON businessBillToAddress.[HashKey] = salesOrderBillToAddress.[BillToAddressHashKey]
    INNER JOIN [AdventureWorksAnalytics].[dw].[Dim_Geography] AS billToGeography
        ON billToGeography.[AddressID] = businessBillToAddress.[AddressID]
    INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderShipToAddress] AS salesOrderShipToAddress
        ON salesOrderShipToAddress.[SalesOrderHashKey] = salesOrder.[HashKey]
    INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Address] AS businessShipToAddress
        ON businessShipToAddress.[HashKey] = salesOrderShipToAddress.[ShipToAddressHashKey]
    INNER JOIN [AdventureWorksAnalytics].[dw].[Dim_Geography] AS shipToGeography
        ON shipToGeography.[AddressID] = businessShipToAddress.[AddressID]
    INNER JOIN [AdventureWorksAnalytics].[dw].[Dim_Date] AS orderDate
        ON orderDate.[CalendarDate] = CONVERT(date, detail.[OrderDate])
    INNER JOIN [AdventureWorksAnalytics].[dw].[Dim_Date] AS dueDate
        ON dueDate.[CalendarDate] = CONVERT(date, detail.[DueDate])
    LEFT OUTER JOIN [AdventureWorksAnalytics].[dw].[Dim_Date] AS shipDate
        ON shipDate.[CalendarDate] = CONVERT(date, detail.[ShipDate])
    INNER JOIN [AdventureWorksAnalytics].[dw].[Dim_SalesChannel] AS channel
        ON channel.[ChannelCode] = CASE WHEN detail.[OnlineOrderFlag] = 1 THEN N'Online' ELSE N'Store' END
    INNER JOIN [AdventureWorksAnalytics].[dw].[Dim_OrderStatus] AS orderStatus
        ON orderStatus.[StatusCode] = detail.[Status]
    WHERE detail.[OrderDate] IS NOT NULL AND detail.[DueDate] IS NOT NULL
        AND detail.[Status] IS NOT NULL AND detail.[OnlineOrderFlag] IS NOT NULL
        AND detail.[SubTotal] IS NOT NULL AND detail.[TaxAmount] IS NOT NULL
        AND detail.[FreightAmount] IS NOT NULL AND detail.[TotalDue] IS NOT NULL
) AS source
ON target.[SalesOrderID] = source.[SalesOrderID]
WHEN MATCHED THEN UPDATE SET
    [OrderDateKey] = source.[OrderDateKey], [DueDateKey] = source.[DueDateKey], [ShipDateKey] = source.[ShipDateKey],
    [CustomerKey] = source.[CustomerKey], [SalespersonKey] = source.[SalespersonKey], [SalesTerritoryKey] = source.[SalesTerritoryKey],
    [SalesChannelKey] = source.[SalesChannelKey], [OrderStatusKey] = source.[OrderStatusKey],
    [BillToGeographyKey] = source.[BillToGeographyKey], [ShipToGeographyKey] = source.[ShipToGeographyKey],
    [OrderCount] = source.[OrderCount], [SubTotal] = source.[SubTotal], [TaxAmount] = source.[TaxAmount],
    [FreightAmount] = source.[FreightAmount], [TotalDue] = source.[TotalDue]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([OrderDateKey], [DueDateKey], [ShipDateKey], [CustomerKey], [SalespersonKey], [SalesTerritoryKey], [SalesChannelKey], [OrderStatusKey],
        [BillToGeographyKey], [ShipToGeographyKey], [SalesOrderID], [OrderCount], [SubTotal], [TaxAmount], [FreightAmount], [TotalDue], [AuditId], [InsertDateTime2])
    VALUES (source.[OrderDateKey], source.[DueDateKey], source.[ShipDateKey], source.[CustomerKey], source.[SalespersonKey], source.[SalesTerritoryKey],
        source.[SalesChannelKey], source.[OrderStatusKey], source.[BillToGeographyKey], source.[ShipToGeographyKey], source.[SalesOrderID], source.[OrderCount],
        source.[SubTotal], source.[TaxAmount], source.[FreightAmount], source.[TotalDue],
        CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')),
        CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required'))
    );
