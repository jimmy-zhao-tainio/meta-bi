INSERT INTO [AdventureWorksBusinessVault].[dbo].[BL_SalesPersonQuotaSalesPerson] ([HashKey], [SalesPersonQuotaHashKey], [SalesPersonHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesPersonQuotaHistoryHashKey]))) + CONVERT(varbinary(max), source.[SalesPersonQuotaHistoryHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesPersonHashKey]))) + CONVERT(varbinary(max), source.[SalesPersonHashKey]))),
    source.[SalesPersonQuotaHistoryHashKey],
    source.[SalesPersonHashKey],
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.L_SalesPersonQuotaHistorySalesPerson'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[L_SalesPersonQuotaHistorySalesPerson] AS source
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesPersonQuota] AS endpoint1
    ON endpoint1.[HashKey] = source.[SalesPersonQuotaHistoryHashKey]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS endpoint2
    ON endpoint2.[HashKey] = source.[SalesPersonHashKey]
LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesPersonQuotaSalesPerson] AS existing
    ON existing.[HashKey] = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesPersonQuotaHistoryHashKey]))) + CONVERT(varbinary(max), source.[SalesPersonQuotaHistoryHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesPersonHashKey]))) + CONVERT(varbinary(max), source.[SalesPersonHashKey])))
WHERE existing.[HashKey] IS NULL