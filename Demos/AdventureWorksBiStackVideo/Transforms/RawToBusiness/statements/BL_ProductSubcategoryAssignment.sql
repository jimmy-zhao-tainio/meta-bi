INSERT INTO [AdventureWorksBusinessVault].[dbo].[BL_ProductSubcategoryAssignment] ([HashKey], [ProductHashKey], [ProductSubcategoryHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductHashKey]))) + CONVERT(varbinary(max), source.[ProductHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductSubcategoryHashKey]))) + CONVERT(varbinary(max), source.[ProductSubcategoryHashKey]))),
    source.[ProductHashKey],
    source.[ProductSubcategoryHashKey],
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.L_ProductProductSubcategory'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[L_ProductProductSubcategory] AS source
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Product] AS endpoint1
    ON endpoint1.[HashKey] = source.[ProductHashKey]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_ProductSubcategory] AS endpoint2
    ON endpoint2.[HashKey] = source.[ProductSubcategoryHashKey]
LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_ProductSubcategoryAssignment] AS existing
    ON existing.[HashKey] = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductHashKey]))) + CONVERT(varbinary(max), source.[ProductHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductSubcategoryHashKey]))) + CONVERT(varbinary(max), source.[ProductSubcategoryHashKey])))
WHERE existing.[HashKey] IS NULL