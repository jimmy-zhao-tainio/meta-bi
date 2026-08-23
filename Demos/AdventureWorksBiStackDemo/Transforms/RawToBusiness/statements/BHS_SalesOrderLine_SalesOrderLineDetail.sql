INSERT INTO [AdventureWorksBusinessVault].[dbo].[BHS_SalesOrderLine_SalesOrderLineDetail] ([HubHashKey], [OrderQuantity], [UnitPrice], [UnitPriceDiscount], [LineTotal], [HashDiff], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    source.[HubHashKey],
    source.[OrderQty],
    source.[UnitPrice],
    source.[UnitPriceDiscount],
    CONVERT(decimal(19,4), source.[LineTotal]),
    CONVERT(binary(32), HASHBYTES('SHA2_256', (CASE
    WHEN source.[OrderQty] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[OrderQty]))) + CONVERT(varbinary(max), source.[OrderQty])
END) + (CASE
    WHEN source.[UnitPrice] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[UnitPrice]))) + CONVERT(varbinary(max), source.[UnitPrice])
END) + (CASE
    WHEN source.[UnitPriceDiscount] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[UnitPriceDiscount]))) + CONVERT(varbinary(max), source.[UnitPriceDiscount])
END) + (CASE
    WHEN source.[LineTotal] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), CONVERT(decimal(19,4), source.[LineTotal])))) + CONVERT(varbinary(max), CONVERT(decimal(19,4), source.[LineTotal]))
END))),
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.HS_SalesOrderDetail_SalesOrderDetail'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[HS_SalesOrderDetail_SalesOrderDetail] AS source
INNER JOIN (
SELECT
    candidate.[HubHashKey],
    candidate.[LoadTimestamp],
    candidate.[AuditId],
    ROW_NUMBER() OVER (PARTITION BY candidate.[HubHashKey] ORDER BY candidate.[LoadTimestamp] DESC, candidate.[AuditId] DESC) AS [VersionRank]
FROM [AdventureWorksRawVault].[dbo].[HS_SalesOrderDetail_SalesOrderDetail] AS candidate
) AS latest
    ON latest.[HubHashKey] = source.[HubHashKey] AND latest.[LoadTimestamp] = source.[LoadTimestamp] AND latest.[AuditId] = source.[AuditId] AND latest.[VersionRank] = 1
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesOrderLine] AS hub
    ON hub.[HashKey] = source.[HubHashKey]
LEFT OUTER JOIN (
SELECT
    existing.[HubHashKey],
    existing.[HashDiff],
    ROW_NUMBER() OVER (PARTITION BY existing.[HubHashKey] ORDER BY existing.[LoadTimestamp] DESC, existing.[AuditId] DESC) AS [VersionRank]
FROM [AdventureWorksBusinessVault].[dbo].[BHS_SalesOrderLine_SalesOrderLineDetail] AS existing
) AS currentState
    ON currentState.[HubHashKey] = source.[HubHashKey] AND currentState.[VersionRank] = 1
WHERE (currentState.[HashDiff] IS NULL OR currentState.[HashDiff] <> CONVERT(binary(32), HASHBYTES('SHA2_256', (CASE
    WHEN source.[OrderQty] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[OrderQty]))) + CONVERT(varbinary(max), source.[OrderQty])
END) + (CASE
    WHEN source.[UnitPrice] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[UnitPrice]))) + CONVERT(varbinary(max), source.[UnitPrice])
END) + (CASE
    WHEN source.[UnitPriceDiscount] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[UnitPriceDiscount]))) + CONVERT(varbinary(max), source.[UnitPriceDiscount])
END) + (CASE
    WHEN source.[LineTotal] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), CONVERT(decimal(19,4), source.[LineTotal])))) + CONVERT(varbinary(max), CONVERT(decimal(19,4), source.[LineTotal]))
END))))
