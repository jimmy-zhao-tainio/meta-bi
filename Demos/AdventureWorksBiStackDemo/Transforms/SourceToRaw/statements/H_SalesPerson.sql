INSERT INTO [AdventureWorksRawVault].[dbo].[H_SalesPerson] ([HashKey], [BusinessEntityID], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.BusinessEntityID))) + CONVERT(varbinary(max), source.BusinessEntityID))) AS HashKey,
    source.BusinessEntityID,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesPerson') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesPerson] AS source
LEFT OUTER JOIN [AdventureWorksRawVault].[dbo].[H_SalesPerson] AS existing
    ON existing.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.BusinessEntityID))) + CONVERT(varbinary(max), source.BusinessEntityID)))
WHERE existing.HashKey IS NULL