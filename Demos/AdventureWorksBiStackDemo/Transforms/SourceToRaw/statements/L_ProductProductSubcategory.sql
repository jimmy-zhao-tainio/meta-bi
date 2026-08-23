INSERT INTO [AdventureWorksRawVault].[dbo].[L_ProductProductSubcategory] ([HashKey], [ProductHashKey], [ProductSubcategoryHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductID]))) + CONVERT(varbinary(max), source.[ProductID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductSubcategoryID]))) + CONVERT(varbinary(max), source.[ProductSubcategoryID]))) AS HashKey,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductID]))) + CONVERT(varbinary(max), source.[ProductID]))) AS ProductHashKey,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductSubcategoryID]))) + CONVERT(varbinary(max), source.[ProductSubcategoryID]))) AS ProductSubcategoryHashKey,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Production.Product') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Production].[Product] AS source
INNER JOIN [AdventureWorksRawVault].[dbo].[H_Product] AS productHub
    ON productHub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductID]))) + CONVERT(varbinary(max), source.[ProductID])))
INNER JOIN [AdventureWorksRawVault].[dbo].[H_ProductSubcategory] AS productSubcategoryHub
    ON productSubcategoryHub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductSubcategoryID]))) + CONVERT(varbinary(max), source.[ProductSubcategoryID])))
LEFT OUTER JOIN [AdventureWorksRawVault].[dbo].[L_ProductProductSubcategory] AS existing
    ON existing.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductID]))) + CONVERT(varbinary(max), source.[ProductID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductSubcategoryID]))) + CONVERT(varbinary(max), source.[ProductSubcategoryID])))
WHERE existing.HashKey IS NULL AND (source.[ProductSubcategoryID] IS NOT NULL)