WITH latestLine AS (
    SELECT source.[HubHashKey], source.[OrderQuantity], source.[UnitPrice], source.[UnitPriceDiscount], source.[LineTotal],
        ROW_NUMBER() OVER (PARTITION BY source.[HubHashKey] ORDER BY source.[LoadTimestamp] DESC, source.[AuditId] DESC) AS [VersionRank]
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_SalesOrderLine_SalesOrderLineDetail] AS source
), latestOrder AS (
    SELECT source.[HubHashKey], source.[OrderDate], source.[OnlineOrderFlag],
        ROW_NUMBER() OVER (PARTITION BY source.[HubHashKey] ORDER BY source.[LoadTimestamp] DESC, source.[AuditId] DESC) AS [VersionRank]
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_SalesOrder_SalesOrderHeader] AS source
)
MERGE INTO [AdventureWorksAnalytics].[dw].[Fact_SalesLine] AS target
USING (
    SELECT orderDate.[DateKey] AS [OrderDateKey], product.[ProductKey], customer.[CustomerKey], salesperson.[SalespersonKey],
        territory.[SalesTerritoryKey], channel.[SalesChannelKey], shipToGeography.[GeographyKey] AS [ShipToGeographyKey],
        salesLine.[SalesOrderID], salesLine.[SalesOrderDetailID],
        CONVERT(int, lineDetail.[OrderQuantity]) AS [OrderQuantity],
        CONVERT(decimal(18,4), lineDetail.[UnitPrice]) AS [UnitPrice],
        CONVERT(decimal(18,4), lineDetail.[UnitPrice] * CONVERT(decimal(18,4), lineDetail.[OrderQuantity]) * lineDetail.[UnitPriceDiscount]) AS [DiscountAmount],
        CONVERT(decimal(18,4), lineDetail.[LineTotal]) AS [LineSalesAmount]
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrderLine] AS salesLine
    INNER JOIN latestLine AS lineDetail
        ON lineDetail.[HubHashKey] = salesLine.[HashKey] AND lineDetail.[VersionRank] = 1
    INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineSalesOrder] AS salesLineOrder
        ON salesLineOrder.[SalesOrderLineHashKey] = salesLine.[HashKey]
    INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS salesOrder
        ON salesOrder.[HashKey] = salesLineOrder.[SalesOrderHashKey]
    INNER JOIN latestOrder AS orderDetail
        ON orderDetail.[HubHashKey] = salesOrder.[HashKey] AND orderDetail.[VersionRank] = 1
    INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineProduct] AS salesLineProduct
        ON salesLineProduct.[SalesOrderLineHashKey] = salesLine.[HashKey]
    INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Product] AS businessProduct
        ON businessProduct.[HashKey] = salesLineProduct.[ProductHashKey]
    INNER JOIN [AdventureWorksAnalytics].[dw].[Dim_Product] AS product
        ON product.[ProductID] = businessProduct.[ProductID]
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
    INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderShipToAddress] AS salesOrderShipToAddress
        ON salesOrderShipToAddress.[SalesOrderHashKey] = salesOrder.[HashKey]
    INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Address] AS businessShipToAddress
        ON businessShipToAddress.[HashKey] = salesOrderShipToAddress.[ShipToAddressHashKey]
    INNER JOIN [AdventureWorksAnalytics].[dw].[Dim_Geography] AS shipToGeography
        ON shipToGeography.[AddressID] = businessShipToAddress.[AddressID]
    INNER JOIN [AdventureWorksAnalytics].[dw].[Dim_Date] AS orderDate
        ON orderDate.[CalendarDate] = CONVERT(date, orderDetail.[OrderDate])
    INNER JOIN [AdventureWorksAnalytics].[dw].[Dim_SalesChannel] AS channel
        ON channel.[ChannelCode] = CASE WHEN orderDetail.[OnlineOrderFlag] = 1 THEN N'Online' ELSE N'Store' END
    WHERE lineDetail.[OrderQuantity] IS NOT NULL AND lineDetail.[UnitPrice] IS NOT NULL
        AND lineDetail.[UnitPriceDiscount] IS NOT NULL AND lineDetail.[LineTotal] IS NOT NULL
        AND orderDetail.[OrderDate] IS NOT NULL AND orderDetail.[OnlineOrderFlag] IS NOT NULL
) AS source
ON target.[SalesOrderID] = source.[SalesOrderID] AND target.[SalesOrderDetailID] = source.[SalesOrderDetailID]
WHEN MATCHED THEN UPDATE SET
    [OrderDateKey] = source.[OrderDateKey], [ProductKey] = source.[ProductKey], [CustomerKey] = source.[CustomerKey],
    [SalespersonKey] = source.[SalespersonKey], [SalesTerritoryKey] = source.[SalesTerritoryKey],
    [SalesChannelKey] = source.[SalesChannelKey], [ShipToGeographyKey] = source.[ShipToGeographyKey],
    [OrderQuantity] = source.[OrderQuantity], [UnitPrice] = source.[UnitPrice],
    [DiscountAmount] = source.[DiscountAmount], [LineSalesAmount] = source.[LineSalesAmount]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([OrderDateKey], [ProductKey], [CustomerKey], [SalespersonKey], [SalesTerritoryKey], [SalesChannelKey], [ShipToGeographyKey],
        [SalesOrderID], [SalesOrderDetailID], [OrderQuantity], [UnitPrice], [DiscountAmount], [LineSalesAmount], [AuditId], [InsertDateTime2])
    VALUES (source.[OrderDateKey], source.[ProductKey], source.[CustomerKey], source.[SalespersonKey], source.[SalesTerritoryKey],
        source.[SalesChannelKey], source.[ShipToGeographyKey], source.[SalesOrderID], source.[SalesOrderDetailID], source.[OrderQuantity],
        source.[UnitPrice], source.[DiscountAmount], source.[LineSalesAmount],
        CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')),
        CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required'))
    );
