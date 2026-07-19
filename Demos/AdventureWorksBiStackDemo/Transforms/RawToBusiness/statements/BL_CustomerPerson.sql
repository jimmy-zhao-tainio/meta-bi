INSERT INTO [AdventureWorksBusinessVault].[dbo].[BL_CustomerPerson] ([HashKey], [CustomerHashKey], [PersonHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CustomerHashKey]))) + CONVERT(varbinary(max), source.[CustomerHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[PersonHashKey]))) + CONVERT(varbinary(max), source.[PersonHashKey]))),
    source.[CustomerHashKey],
    source.[PersonHashKey],
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.L_CustomerPerson'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[L_CustomerPerson] AS source
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS endpoint1
    ON endpoint1.[HashKey] = source.[CustomerHashKey]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Person] AS endpoint2
    ON endpoint2.[HashKey] = source.[PersonHashKey]
LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_CustomerPerson] AS existing
    ON existing.[HashKey] = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CustomerHashKey]))) + CONVERT(varbinary(max), source.[CustomerHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[PersonHashKey]))) + CONVERT(varbinary(max), source.[PersonHashKey])))
WHERE existing.[HashKey] IS NULL