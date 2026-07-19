INSERT INTO [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesTerritory] ([HashKey], [SalesOrderHashKey], [SalesTerritoryHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderHeaderHashKey]))) + CONVERT(varbinary(max), source.[SalesOrderHeaderHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesTerritoryHashKey]))) + CONVERT(varbinary(max), source.[SalesTerritoryHashKey]))),
    source.[SalesOrderHeaderHashKey],
    source.[SalesTerritoryHashKey],
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.L_SalesOrderHeaderSalesTerritory'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[L_SalesOrderHeaderSalesTerritory] AS source
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS endpoint1
    ON endpoint1.[HashKey] = source.[SalesOrderHeaderHashKey]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS endpoint2
    ON endpoint2.[HashKey] = source.[SalesTerritoryHashKey]
LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesTerritory] AS existing
    ON existing.[HashKey] = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderHeaderHashKey]))) + CONVERT(varbinary(max), source.[SalesOrderHeaderHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesTerritoryHashKey]))) + CONVERT(varbinary(max), source.[SalesTerritoryHashKey])))
WHERE existing.[HashKey] IS NULL