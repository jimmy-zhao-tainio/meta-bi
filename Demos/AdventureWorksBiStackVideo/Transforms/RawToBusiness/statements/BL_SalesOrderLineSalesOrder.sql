INSERT INTO [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineSalesOrder] ([HashKey], [SalesOrderLineHashKey], [SalesOrderHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderDetailHashKey]))) + CONVERT(varbinary(max), source.[SalesOrderDetailHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderHeaderHashKey]))) + CONVERT(varbinary(max), source.[SalesOrderHeaderHashKey]))),
    source.[SalesOrderDetailHashKey],
    source.[SalesOrderHeaderHashKey],
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.L_SalesOrderDetailSalesOrderHeader'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[L_SalesOrderDetailSalesOrderHeader] AS source
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesOrderLine] AS endpoint1
    ON endpoint1.[HashKey] = source.[SalesOrderDetailHashKey]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS endpoint2
    ON endpoint2.[HashKey] = source.[SalesOrderHeaderHashKey]
LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineSalesOrder] AS existing
    ON existing.[HashKey] = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderDetailHashKey]))) + CONVERT(varbinary(max), source.[SalesOrderDetailHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderHeaderHashKey]))) + CONVERT(varbinary(max), source.[SalesOrderHeaderHashKey])))
WHERE existing.[HashKey] IS NULL