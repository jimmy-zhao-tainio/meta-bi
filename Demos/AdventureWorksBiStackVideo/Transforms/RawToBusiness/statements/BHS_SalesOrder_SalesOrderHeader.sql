INSERT INTO [AdventureWorksBusinessVault].[dbo].[BHS_SalesOrder_SalesOrderHeader] ([HubHashKey], [OrderDate], [DueDate], [ShipDate], [Status], [OnlineOrderFlag], [SubTotal], [TaxAmount], [FreightAmount], [TotalDue], [HashDiff], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    source.[HubHashKey],
    source.[OrderDate],
    source.[DueDate],
    source.[ShipDate],
    CONVERT(int, source.[Status]),
    source.[OnlineOrderFlag],
    source.[SubTotal],
    source.[TaxAmt],
    source.[Freight],
    source.[TotalDue],
    CONVERT(binary(32), HASHBYTES('SHA2_256', (CASE
    WHEN source.[OrderDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[OrderDate]))) + CONVERT(varbinary(max), source.[OrderDate])
END) + (CASE
    WHEN source.[DueDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[DueDate]))) + CONVERT(varbinary(max), source.[DueDate])
END) + (CASE
    WHEN source.[ShipDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ShipDate]))) + CONVERT(varbinary(max), source.[ShipDate])
END) + (CASE
    WHEN source.[Status] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), CONVERT(int, source.[Status])))) + CONVERT(varbinary(max), CONVERT(int, source.[Status]))
END) + (CASE
    WHEN source.[OnlineOrderFlag] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[OnlineOrderFlag]))) + CONVERT(varbinary(max), source.[OnlineOrderFlag])
END) + (CASE
    WHEN source.[SubTotal] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SubTotal]))) + CONVERT(varbinary(max), source.[SubTotal])
END) + (CASE
    WHEN source.[TaxAmt] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[TaxAmt]))) + CONVERT(varbinary(max), source.[TaxAmt])
END) + (CASE
    WHEN source.[Freight] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Freight]))) + CONVERT(varbinary(max), source.[Freight])
END) + (CASE
    WHEN source.[TotalDue] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[TotalDue]))) + CONVERT(varbinary(max), source.[TotalDue])
END))),
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.HS_SalesOrderHeader_SalesOrderHeader'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[HS_SalesOrderHeader_SalesOrderHeader] AS source
INNER JOIN (
SELECT
    candidate.[HubHashKey],
    candidate.[LoadTimestamp],
    candidate.[AuditId],
    ROW_NUMBER() OVER (PARTITION BY candidate.[HubHashKey] ORDER BY candidate.[LoadTimestamp] DESC, candidate.[AuditId] DESC) AS [VersionRank]
FROM [AdventureWorksRawVault].[dbo].[HS_SalesOrderHeader_SalesOrderHeader] AS candidate
) AS latest
    ON latest.[HubHashKey] = source.[HubHashKey] AND latest.[LoadTimestamp] = source.[LoadTimestamp] AND latest.[AuditId] = source.[AuditId] AND latest.[VersionRank] = 1
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS hub
    ON hub.[HashKey] = source.[HubHashKey]
LEFT OUTER JOIN (
SELECT
    existing.[HubHashKey],
    existing.[HashDiff],
    ROW_NUMBER() OVER (PARTITION BY existing.[HubHashKey] ORDER BY existing.[LoadTimestamp] DESC, existing.[AuditId] DESC) AS [VersionRank]
FROM [AdventureWorksBusinessVault].[dbo].[BHS_SalesOrder_SalesOrderHeader] AS existing
) AS currentState
    ON currentState.[HubHashKey] = source.[HubHashKey] AND currentState.[VersionRank] = 1
WHERE (currentState.[HashDiff] IS NULL OR currentState.[HashDiff] <> CONVERT(binary(32), HASHBYTES('SHA2_256', (CASE
    WHEN source.[OrderDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[OrderDate]))) + CONVERT(varbinary(max), source.[OrderDate])
END) + (CASE
    WHEN source.[DueDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[DueDate]))) + CONVERT(varbinary(max), source.[DueDate])
END) + (CASE
    WHEN source.[ShipDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ShipDate]))) + CONVERT(varbinary(max), source.[ShipDate])
END) + (CASE
    WHEN source.[Status] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), CONVERT(int, source.[Status])))) + CONVERT(varbinary(max), CONVERT(int, source.[Status]))
END) + (CASE
    WHEN source.[OnlineOrderFlag] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[OnlineOrderFlag]))) + CONVERT(varbinary(max), source.[OnlineOrderFlag])
END) + (CASE
    WHEN source.[SubTotal] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SubTotal]))) + CONVERT(varbinary(max), source.[SubTotal])
END) + (CASE
    WHEN source.[TaxAmt] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[TaxAmt]))) + CONVERT(varbinary(max), source.[TaxAmt])
END) + (CASE
    WHEN source.[Freight] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Freight]))) + CONVERT(varbinary(max), source.[Freight])
END) + (CASE
    WHEN source.[TotalDue] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[TotalDue]))) + CONVERT(varbinary(max), source.[TotalDue])
END))))
