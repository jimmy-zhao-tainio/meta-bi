INSERT INTO [AdventureWorksRawVault].[dbo].[H_SalesPersonQuotaHistory] ([HashKey], [BusinessEntityID], [QuotaDate], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.BusinessEntityID))) + CONVERT(varbinary(max), source.BusinessEntityID) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.QuotaDate))) + CONVERT(varbinary(max), source.QuotaDate))) AS HashKey,
    source.BusinessEntityID,
    source.QuotaDate,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesPersonQuotaHistory') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesPersonQuotaHistory] AS source
LEFT OUTER JOIN [AdventureWorksRawVault].[dbo].[H_SalesPersonQuotaHistory] AS existing
    ON existing.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.BusinessEntityID))) + CONVERT(varbinary(max), source.BusinessEntityID) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.QuotaDate))) + CONVERT(varbinary(max), source.QuotaDate)))
WHERE existing.HashKey IS NULL