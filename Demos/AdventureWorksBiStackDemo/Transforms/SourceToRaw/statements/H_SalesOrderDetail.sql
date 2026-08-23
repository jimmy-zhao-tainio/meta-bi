INSERT INTO [AdventureWorksRawVault].[dbo].[H_SalesOrderDetail] ([HashKey], [SalesOrderID], [SalesOrderDetailID], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.SalesOrderID))) + CONVERT(varbinary(max), source.SalesOrderID) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.SalesOrderDetailID))) + CONVERT(varbinary(max), source.SalesOrderDetailID))) AS HashKey,
    source.SalesOrderID,
    source.SalesOrderDetailID,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesOrderDetail') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesOrderDetail] AS source
LEFT OUTER JOIN [AdventureWorksRawVault].[dbo].[H_SalesOrderDetail] AS existing
    ON existing.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.SalesOrderID))) + CONVERT(varbinary(max), source.SalesOrderID) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.SalesOrderDetailID))) + CONVERT(varbinary(max), source.SalesOrderDetailID)))
WHERE existing.HashKey IS NULL