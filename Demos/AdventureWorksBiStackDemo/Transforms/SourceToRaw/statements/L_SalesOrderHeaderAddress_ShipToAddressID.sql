INSERT INTO [AdventureWorksRawVault].[dbo].[L_SalesOrderHeaderAddress_ShipToAddressID] ([HashKey], [SalesOrderHeaderHashKey], [AddressHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderID]))) + CONVERT(varbinary(max), source.[SalesOrderID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ShipToAddressID]))) + CONVERT(varbinary(max), source.[ShipToAddressID]))) AS HashKey,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderID]))) + CONVERT(varbinary(max), source.[SalesOrderID]))) AS SalesOrderHeaderHashKey,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ShipToAddressID]))) + CONVERT(varbinary(max), source.[ShipToAddressID]))) AS AddressHashKey,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesOrderHeader') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesOrderHeader] AS source
INNER JOIN [AdventureWorksRawVault].[dbo].[H_SalesOrderHeader] AS salesOrderHeaderHub
    ON salesOrderHeaderHub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderID]))) + CONVERT(varbinary(max), source.[SalesOrderID])))
INNER JOIN [AdventureWorksRawVault].[dbo].[H_Address] AS addressHub
    ON addressHub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ShipToAddressID]))) + CONVERT(varbinary(max), source.[ShipToAddressID])))
LEFT OUTER JOIN [AdventureWorksRawVault].[dbo].[L_SalesOrderHeaderAddress_ShipToAddressID] AS existing
    ON existing.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderID]))) + CONVERT(varbinary(max), source.[SalesOrderID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ShipToAddressID]))) + CONVERT(varbinary(max), source.[ShipToAddressID])))
WHERE existing.HashKey IS NULL AND source.[SalesOrderID] IS NOT NULL AND source.[ShipToAddressID] IS NOT NULL
