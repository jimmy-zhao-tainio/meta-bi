INSERT INTO [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderBillToAddress] ([HashKey], [BillToAddressHashKey], [SalesOrderHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[AddressHashKey]))) + CONVERT(varbinary(max), source.[AddressHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderHeaderHashKey]))) + CONVERT(varbinary(max), source.[SalesOrderHeaderHashKey]))),
    source.[AddressHashKey],
    source.[SalesOrderHeaderHashKey],
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.L_SalesOrderHeaderAddress_BillToAddressID'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[L_SalesOrderHeaderAddress_BillToAddressID] AS source
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Address] AS addressHub
    ON addressHub.[HashKey] = source.[AddressHashKey]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS salesOrderHub
    ON salesOrderHub.[HashKey] = source.[SalesOrderHeaderHashKey]
LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderBillToAddress] AS existing
    ON existing.[HashKey] = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[AddressHashKey]))) + CONVERT(varbinary(max), source.[AddressHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderHeaderHashKey]))) + CONVERT(varbinary(max), source.[SalesOrderHeaderHashKey])))
WHERE existing.[HashKey] IS NULL
