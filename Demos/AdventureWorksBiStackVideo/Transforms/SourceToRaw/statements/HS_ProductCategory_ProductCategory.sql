WITH latest AS (
SELECT
    existing.[HubHashKey],
    existing.[HashDiff],
    ROW_NUMBER() OVER (PARTITION BY existing.[HubHashKey] ORDER BY existing.[LoadTimestamp] DESC, existing.[AuditId] DESC) AS [VersionRank]
FROM [AdventureWorksRawVault].[dbo].[HS_ProductCategory_ProductCategory] AS existing
)
INSERT INTO [AdventureWorksRawVault].[dbo].[HS_ProductCategory_ProductCategory] ([HubHashKey], [Name], [rowguid], [ModifiedDate], [HashDiff], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductCategoryID]))) + CONVERT(varbinary(max), source.[ProductCategoryID]))) AS HubHashKey,
    source.[Name],
    source.[rowguid],
    source.[ModifiedDate],
    CONVERT(binary(32), HASHBYTES('SHA2_256', (CASE
    WHEN source.[Name] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Name]))) + CONVERT(varbinary(max), source.[Name])
END) + (CASE
    WHEN source.[rowguid] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[rowguid]))) + CONVERT(varbinary(max), source.[rowguid])
END) + (CASE
    WHEN source.[ModifiedDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ModifiedDate]))) + CONVERT(varbinary(max), source.[ModifiedDate])
END))) AS HashDiff,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Production.ProductCategory') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Production].[ProductCategory] AS source
INNER JOIN [AdventureWorksRawVault].[dbo].[H_ProductCategory] AS hub
    ON hub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductCategoryID]))) + CONVERT(varbinary(max), source.[ProductCategoryID])))
LEFT OUTER JOIN latest
    ON latest.[HubHashKey] = hub.[HashKey] AND latest.[VersionRank] = 1
WHERE latest.HashDiff IS NULL OR latest.HashDiff <> CONVERT(binary(32), HASHBYTES('SHA2_256', (CASE
    WHEN source.[Name] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Name]))) + CONVERT(varbinary(max), source.[Name])
END) + (CASE
    WHEN source.[rowguid] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[rowguid]))) + CONVERT(varbinary(max), source.[rowguid])
END) + (CASE
    WHEN source.[ModifiedDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ModifiedDate]))) + CONVERT(varbinary(max), source.[ModifiedDate])
END)))