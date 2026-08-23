INSERT INTO [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderCustomer] ([HashKey], [SalesOrderHashKey], [CustomerHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderHeaderHashKey]))) + CONVERT(varbinary(max), source.[SalesOrderHeaderHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CustomerHashKey]))) + CONVERT(varbinary(max), source.[CustomerHashKey]))),
    source.[SalesOrderHeaderHashKey],
    source.[CustomerHashKey],
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.L_SalesOrderHeaderCustomer'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[L_SalesOrderHeaderCustomer] AS source
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS endpoint1
    ON endpoint1.[HashKey] = source.[SalesOrderHeaderHashKey]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS endpoint2
    ON endpoint2.[HashKey] = source.[CustomerHashKey]
LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderCustomer] AS existing
    ON existing.[HashKey] = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderHeaderHashKey]))) + CONVERT(varbinary(max), source.[SalesOrderHeaderHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CustomerHashKey]))) + CONVERT(varbinary(max), source.[CustomerHashKey])))
WHERE existing.[HashKey] IS NULL