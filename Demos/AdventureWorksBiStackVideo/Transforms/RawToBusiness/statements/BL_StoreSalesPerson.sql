INSERT INTO [AdventureWorksBusinessVault].[dbo].[BL_StoreSalesPerson] ([HashKey], [StoreHashKey], [SalesPersonHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[StoreHashKey]))) + CONVERT(varbinary(max), source.[StoreHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesPersonHashKey]))) + CONVERT(varbinary(max), source.[SalesPersonHashKey]))),
    source.[StoreHashKey],
    source.[SalesPersonHashKey],
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.L_StoreSalesPerson'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[L_StoreSalesPerson] AS source
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Store] AS endpoint1
    ON endpoint1.[HashKey] = source.[StoreHashKey]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS endpoint2
    ON endpoint2.[HashKey] = source.[SalesPersonHashKey]
LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_StoreSalesPerson] AS existing
    ON existing.[HashKey] = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[StoreHashKey]))) + CONVERT(varbinary(max), source.[StoreHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesPersonHashKey]))) + CONVERT(varbinary(max), source.[SalesPersonHashKey])))
WHERE existing.[HashKey] IS NULL