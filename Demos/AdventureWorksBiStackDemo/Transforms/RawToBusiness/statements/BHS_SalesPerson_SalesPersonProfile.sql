INSERT INTO [AdventureWorksBusinessVault].[dbo].[BHS_SalesPerson_SalesPersonProfile] ([HubHashKey], [SalesQuota], [Bonus], [CommissionPercent], [SalesYearToDate], [SalesLastYear], [HashDiff], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    source.[HubHashKey],
    source.[SalesQuota],
    source.[Bonus],
    source.[CommissionPct],
    source.[SalesYTD],
    source.[SalesLastYear],
    CONVERT(binary(32), HASHBYTES('SHA2_256', (CASE
    WHEN source.[SalesQuota] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesQuota]))) + CONVERT(varbinary(max), source.[SalesQuota])
END) + (CASE
    WHEN source.[Bonus] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Bonus]))) + CONVERT(varbinary(max), source.[Bonus])
END) + (CASE
    WHEN source.[CommissionPct] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CommissionPct]))) + CONVERT(varbinary(max), source.[CommissionPct])
END) + (CASE
    WHEN source.[SalesYTD] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesYTD]))) + CONVERT(varbinary(max), source.[SalesYTD])
END) + (CASE
    WHEN source.[SalesLastYear] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesLastYear]))) + CONVERT(varbinary(max), source.[SalesLastYear])
END))),
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.HS_SalesPerson_SalesPerson'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[HS_SalesPerson_SalesPerson] AS source
INNER JOIN (
SELECT
    candidate.[HubHashKey],
    candidate.[LoadTimestamp],
    candidate.[AuditId],
    ROW_NUMBER() OVER (PARTITION BY candidate.[HubHashKey] ORDER BY candidate.[LoadTimestamp] DESC, candidate.[AuditId] DESC) AS [VersionRank]
FROM [AdventureWorksRawVault].[dbo].[HS_SalesPerson_SalesPerson] AS candidate
) AS latest
    ON latest.[HubHashKey] = source.[HubHashKey] AND latest.[LoadTimestamp] = source.[LoadTimestamp] AND latest.[AuditId] = source.[AuditId] AND latest.[VersionRank] = 1
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS hub
    ON hub.[HashKey] = source.[HubHashKey]
LEFT OUTER JOIN (
SELECT
    existing.[HubHashKey],
    existing.[HashDiff],
    ROW_NUMBER() OVER (PARTITION BY existing.[HubHashKey] ORDER BY existing.[LoadTimestamp] DESC, existing.[AuditId] DESC) AS [VersionRank]
FROM [AdventureWorksBusinessVault].[dbo].[BHS_SalesPerson_SalesPersonProfile] AS existing
) AS currentState
    ON currentState.[HubHashKey] = source.[HubHashKey] AND currentState.[VersionRank] = 1
WHERE (currentState.[HashDiff] IS NULL OR currentState.[HashDiff] <> CONVERT(binary(32), HASHBYTES('SHA2_256', (CASE
    WHEN source.[SalesQuota] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesQuota]))) + CONVERT(varbinary(max), source.[SalesQuota])
END) + (CASE
    WHEN source.[Bonus] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Bonus]))) + CONVERT(varbinary(max), source.[Bonus])
END) + (CASE
    WHEN source.[CommissionPct] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CommissionPct]))) + CONVERT(varbinary(max), source.[CommissionPct])
END) + (CASE
    WHEN source.[SalesYTD] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesYTD]))) + CONVERT(varbinary(max), source.[SalesYTD])
END) + (CASE
    WHEN source.[SalesLastYear] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesLastYear]))) + CONVERT(varbinary(max), source.[SalesLastYear])
END))))