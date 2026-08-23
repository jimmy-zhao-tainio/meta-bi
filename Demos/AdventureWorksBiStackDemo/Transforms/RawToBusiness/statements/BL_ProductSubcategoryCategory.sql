INSERT INTO [AdventureWorksBusinessVault].[dbo].[BL_ProductSubcategoryCategory] ([HashKey], [ProductSubcategoryHashKey], [ProductCategoryHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductSubcategoryHashKey]))) + CONVERT(varbinary(max), source.[ProductSubcategoryHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductCategoryHashKey]))) + CONVERT(varbinary(max), source.[ProductCategoryHashKey]))),
    source.[ProductSubcategoryHashKey],
    source.[ProductCategoryHashKey],
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.L_ProductSubcategoryProductCategory'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[L_ProductSubcategoryProductCategory] AS source
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_ProductSubcategory] AS endpoint1
    ON endpoint1.[HashKey] = source.[ProductSubcategoryHashKey]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_ProductCategory] AS endpoint2
    ON endpoint2.[HashKey] = source.[ProductCategoryHashKey]
LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_ProductSubcategoryCategory] AS existing
    ON existing.[HashKey] = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductSubcategoryHashKey]))) + CONVERT(varbinary(max), source.[ProductSubcategoryHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductCategoryHashKey]))) + CONVERT(varbinary(max), source.[ProductCategoryHashKey])))
WHERE existing.[HashKey] IS NULL