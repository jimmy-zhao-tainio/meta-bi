INSERT INTO [AdventureWorksBusinessVault].[dbo].[BH_ProductSubcategory] ([HashKey], [ProductSubcategoryID], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    source.[HashKey],
    CONVERT(int, source.[ProductSubcategoryID]),
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.H_ProductSubcategory'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[H_ProductSubcategory] AS source
LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BH_ProductSubcategory] AS existing
    ON existing.[HashKey] = source.[HashKey]
WHERE existing.[HashKey] IS NULL AND source.[ProductSubcategoryID] IS NOT NULL
