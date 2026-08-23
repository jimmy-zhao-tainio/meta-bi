INSERT INTO [AdventureWorksBusinessVault].[dbo].[BH_ProductCategory] ([HashKey], [ProductCategoryID], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    source.[HashKey],
    CONVERT(int, source.[ProductCategoryID]),
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.H_ProductCategory'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[H_ProductCategory] AS source
LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BH_ProductCategory] AS existing
    ON existing.[HashKey] = source.[HashKey]
WHERE existing.[HashKey] IS NULL AND source.[ProductCategoryID] IS NOT NULL
