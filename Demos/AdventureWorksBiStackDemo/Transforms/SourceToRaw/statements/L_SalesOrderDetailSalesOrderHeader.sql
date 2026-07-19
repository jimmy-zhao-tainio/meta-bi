INSERT INTO [AdventureWorksRawVault].[dbo].[L_SalesOrderDetailSalesOrderHeader] ([HashKey], [SalesOrderDetailHashKey], [SalesOrderHeaderHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderID]))) + CONVERT(varbinary(max), source.[SalesOrderID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderDetailID]))) + CONVERT(varbinary(max), source.[SalesOrderDetailID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderID]))) + CONVERT(varbinary(max), source.[SalesOrderID]))) AS HashKey,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderID]))) + CONVERT(varbinary(max), source.[SalesOrderID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderDetailID]))) + CONVERT(varbinary(max), source.[SalesOrderDetailID]))) AS SalesOrderDetailHashKey,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderID]))) + CONVERT(varbinary(max), source.[SalesOrderID]))) AS SalesOrderHeaderHashKey,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesOrderDetail') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesOrderDetail] AS source
INNER JOIN [AdventureWorksRawVault].[dbo].[H_SalesOrderDetail] AS salesOrderDetailHub
    ON salesOrderDetailHub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderID]))) + CONVERT(varbinary(max), source.[SalesOrderID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderDetailID]))) + CONVERT(varbinary(max), source.[SalesOrderDetailID])))
INNER JOIN [AdventureWorksRawVault].[dbo].[H_SalesOrderHeader] AS salesOrderHeaderHub
    ON salesOrderHeaderHub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderID]))) + CONVERT(varbinary(max), source.[SalesOrderID])))
LEFT OUTER JOIN [AdventureWorksRawVault].[dbo].[L_SalesOrderDetailSalesOrderHeader] AS existing
    ON existing.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderID]))) + CONVERT(varbinary(max), source.[SalesOrderID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderDetailID]))) + CONVERT(varbinary(max), source.[SalesOrderDetailID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderID]))) + CONVERT(varbinary(max), source.[SalesOrderID])))
WHERE existing.HashKey IS NULL