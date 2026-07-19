WITH latest AS (
SELECT
    existing.[HubHashKey],
    existing.[HashDiff],
    ROW_NUMBER() OVER (PARTITION BY existing.[HubHashKey] ORDER BY existing.[LoadTimestamp] DESC, existing.[AuditId] DESC) AS [VersionRank]
FROM [AdventureWorksRawVault].[dbo].[HS_SalesOrderHeader_SalesOrderHeader] AS existing
)
INSERT INTO [AdventureWorksRawVault].[dbo].[HS_SalesOrderHeader_SalesOrderHeader] ([HubHashKey], [RevisionNumber], [OrderDate], [DueDate], [ShipDate], [Status], [OnlineOrderFlag], [SalesOrderNumber], [PurchaseOrderNumber], [AccountNumber], [CreditCardApprovalCode], [SubTotal], [TaxAmt], [Freight], [TotalDue], [Comment], [rowguid], [ModifiedDate], [HashDiff], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderID]))) + CONVERT(varbinary(max), source.[SalesOrderID]))) AS HubHashKey,
    source.[RevisionNumber],
    source.[OrderDate],
    source.[DueDate],
    source.[ShipDate],
    source.[Status],
    source.[OnlineOrderFlag],
    source.[SalesOrderNumber],
    source.[PurchaseOrderNumber],
    source.[AccountNumber],
    source.[CreditCardApprovalCode],
    source.[SubTotal],
    source.[TaxAmt],
    source.[Freight],
    source.[TotalDue],
    source.[Comment],
    source.[rowguid],
    source.[ModifiedDate],
    CONVERT(binary(32), HASHBYTES('SHA2_256', (CASE
    WHEN source.[RevisionNumber] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[RevisionNumber]))) + CONVERT(varbinary(max), source.[RevisionNumber])
END) + (CASE
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
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Status]))) + CONVERT(varbinary(max), source.[Status])
END) + (CASE
    WHEN source.[OnlineOrderFlag] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[OnlineOrderFlag]))) + CONVERT(varbinary(max), source.[OnlineOrderFlag])
END) + (CASE
    WHEN source.[SalesOrderNumber] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderNumber]))) + CONVERT(varbinary(max), source.[SalesOrderNumber])
END) + (CASE
    WHEN source.[PurchaseOrderNumber] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[PurchaseOrderNumber]))) + CONVERT(varbinary(max), source.[PurchaseOrderNumber])
END) + (CASE
    WHEN source.[AccountNumber] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[AccountNumber]))) + CONVERT(varbinary(max), source.[AccountNumber])
END) + (CASE
    WHEN source.[CreditCardApprovalCode] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CreditCardApprovalCode]))) + CONVERT(varbinary(max), source.[CreditCardApprovalCode])
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
END) + (CASE
    WHEN source.[Comment] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Comment]))) + CONVERT(varbinary(max), source.[Comment])
END) + (CASE
    WHEN source.[rowguid] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[rowguid]))) + CONVERT(varbinary(max), source.[rowguid])
END) + (CASE
    WHEN source.[ModifiedDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ModifiedDate]))) + CONVERT(varbinary(max), source.[ModifiedDate])
END))) AS HashDiff,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesOrderHeader') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesOrderHeader] AS source
INNER JOIN [AdventureWorksRawVault].[dbo].[H_SalesOrderHeader] AS hub
    ON hub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderID]))) + CONVERT(varbinary(max), source.[SalesOrderID])))
LEFT OUTER JOIN latest
    ON latest.[HubHashKey] = hub.[HashKey] AND latest.[VersionRank] = 1
WHERE latest.HashDiff IS NULL OR latest.HashDiff <> CONVERT(binary(32), HASHBYTES('SHA2_256', (CASE
    WHEN source.[RevisionNumber] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[RevisionNumber]))) + CONVERT(varbinary(max), source.[RevisionNumber])
END) + (CASE
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
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Status]))) + CONVERT(varbinary(max), source.[Status])
END) + (CASE
    WHEN source.[OnlineOrderFlag] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[OnlineOrderFlag]))) + CONVERT(varbinary(max), source.[OnlineOrderFlag])
END) + (CASE
    WHEN source.[SalesOrderNumber] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesOrderNumber]))) + CONVERT(varbinary(max), source.[SalesOrderNumber])
END) + (CASE
    WHEN source.[PurchaseOrderNumber] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[PurchaseOrderNumber]))) + CONVERT(varbinary(max), source.[PurchaseOrderNumber])
END) + (CASE
    WHEN source.[AccountNumber] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[AccountNumber]))) + CONVERT(varbinary(max), source.[AccountNumber])
END) + (CASE
    WHEN source.[CreditCardApprovalCode] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CreditCardApprovalCode]))) + CONVERT(varbinary(max), source.[CreditCardApprovalCode])
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
END) + (CASE
    WHEN source.[Comment] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Comment]))) + CONVERT(varbinary(max), source.[Comment])
END) + (CASE
    WHEN source.[rowguid] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[rowguid]))) + CONVERT(varbinary(max), source.[rowguid])
END) + (CASE
    WHEN source.[ModifiedDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ModifiedDate]))) + CONVERT(varbinary(max), source.[ModifiedDate])
END)))