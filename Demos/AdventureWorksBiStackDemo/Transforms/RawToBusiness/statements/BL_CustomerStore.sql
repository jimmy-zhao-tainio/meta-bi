INSERT INTO [AdventureWorksBusinessVault].[dbo].[BL_CustomerStore] ([HashKey], [CustomerHashKey], [StoreHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CustomerHashKey]))) + CONVERT(varbinary(max), source.[CustomerHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[StoreHashKey]))) + CONVERT(varbinary(max), source.[StoreHashKey]))),
    source.[CustomerHashKey],
    source.[StoreHashKey],
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.L_CustomerStore'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[L_CustomerStore] AS source
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS endpoint1
    ON endpoint1.[HashKey] = source.[CustomerHashKey]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Store] AS endpoint2
    ON endpoint2.[HashKey] = source.[StoreHashKey]
LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_CustomerStore] AS existing
    ON existing.[HashKey] = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CustomerHashKey]))) + CONVERT(varbinary(max), source.[CustomerHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[StoreHashKey]))) + CONVERT(varbinary(max), source.[StoreHashKey])))
WHERE existing.[HashKey] IS NULL