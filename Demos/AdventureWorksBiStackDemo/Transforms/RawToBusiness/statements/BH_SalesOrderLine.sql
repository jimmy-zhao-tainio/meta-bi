INSERT INTO [AdventureWorksBusinessVault].[dbo].[BH_SalesOrderLine] ([HashKey], [SalesOrderID], [SalesOrderDetailID], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    source.[HashKey],
    CONVERT(int, source.[SalesOrderID]),
    CONVERT(int, source.[SalesOrderDetailID]),
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.H_SalesOrderDetail'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[H_SalesOrderDetail] AS source
LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesOrderLine] AS existing
    ON existing.[HashKey] = source.[HashKey]
WHERE existing.[HashKey] IS NULL AND source.[SalesOrderID] IS NOT NULL AND source.[SalesOrderDetailID] IS NOT NULL
