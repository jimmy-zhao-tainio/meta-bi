INSERT INTO [AdventureWorksRawVault].[dbo].[L_ProductSubcategoryProductCategory] ([HashKey], [ProductSubcategoryHashKey], [ProductCategoryHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductSubcategoryID]))) + CONVERT(varbinary(max), source.[ProductSubcategoryID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductCategoryID]))) + CONVERT(varbinary(max), source.[ProductCategoryID]))) AS HashKey,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductSubcategoryID]))) + CONVERT(varbinary(max), source.[ProductSubcategoryID]))) AS ProductSubcategoryHashKey,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductCategoryID]))) + CONVERT(varbinary(max), source.[ProductCategoryID]))) AS ProductCategoryHashKey,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Production.ProductSubcategory') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Production].[ProductSubcategory] AS source
INNER JOIN [AdventureWorksRawVault].[dbo].[H_ProductSubcategory] AS productSubcategoryHub
    ON productSubcategoryHub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductSubcategoryID]))) + CONVERT(varbinary(max), source.[ProductSubcategoryID])))
INNER JOIN [AdventureWorksRawVault].[dbo].[H_ProductCategory] AS productCategoryHub
    ON productCategoryHub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductCategoryID]))) + CONVERT(varbinary(max), source.[ProductCategoryID])))
LEFT OUTER JOIN [AdventureWorksRawVault].[dbo].[L_ProductSubcategoryProductCategory] AS existing
    ON existing.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductSubcategoryID]))) + CONVERT(varbinary(max), source.[ProductSubcategoryID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductCategoryID]))) + CONVERT(varbinary(max), source.[ProductCategoryID])))
WHERE existing.HashKey IS NULL