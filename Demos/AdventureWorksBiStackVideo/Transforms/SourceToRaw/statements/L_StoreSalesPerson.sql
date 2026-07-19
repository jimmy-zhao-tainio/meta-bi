INSERT INTO [AdventureWorksRawVault].[dbo].[L_StoreSalesPerson] ([HashKey], [StoreHashKey], [SalesPersonHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[BusinessEntityID]))) + CONVERT(varbinary(max), source.[BusinessEntityID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesPersonID]))) + CONVERT(varbinary(max), source.[SalesPersonID]))) AS HashKey,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[BusinessEntityID]))) + CONVERT(varbinary(max), source.[BusinessEntityID]))) AS StoreHashKey,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesPersonID]))) + CONVERT(varbinary(max), source.[SalesPersonID]))) AS SalesPersonHashKey,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.Store') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Sales].[Store] AS source
INNER JOIN [AdventureWorksRawVault].[dbo].[H_Store] AS storeHub
    ON storeHub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[BusinessEntityID]))) + CONVERT(varbinary(max), source.[BusinessEntityID])))
INNER JOIN [AdventureWorksRawVault].[dbo].[H_SalesPerson] AS salesPersonHub
    ON salesPersonHub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesPersonID]))) + CONVERT(varbinary(max), source.[SalesPersonID])))
LEFT OUTER JOIN [AdventureWorksRawVault].[dbo].[L_StoreSalesPerson] AS existing
    ON existing.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[BusinessEntityID]))) + CONVERT(varbinary(max), source.[BusinessEntityID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesPersonID]))) + CONVERT(varbinary(max), source.[SalesPersonID])))
WHERE existing.HashKey IS NULL AND (source.[SalesPersonID] IS NOT NULL)