INSERT INTO [AdventureWorksBusinessVault].[dbo].[BHS_Product_ProductProfile] ([HubHashKey], [Name], [ProductNumber], [Color], [StandardCost], [ListPrice], [Size], [Weight], [SellStartDate], [SellEndDate], [DiscontinuedDate], [HashDiff], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    source.[HubHashKey],
    source.[Name],
    source.[ProductNumber],
    source.[Color],
    source.[StandardCost],
    source.[ListPrice],
    source.[Size],
    source.[Weight],
    source.[SellStartDate],
    source.[SellEndDate],
    source.[DiscontinuedDate],
    CONVERT(binary(32), HASHBYTES('SHA2_256', (CASE
    WHEN source.[Name] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Name]))) + CONVERT(varbinary(max), source.[Name])
END) + (CASE
    WHEN source.[ProductNumber] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductNumber]))) + CONVERT(varbinary(max), source.[ProductNumber])
END) + (CASE
    WHEN source.[Color] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Color]))) + CONVERT(varbinary(max), source.[Color])
END) + (CASE
    WHEN source.[StandardCost] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[StandardCost]))) + CONVERT(varbinary(max), source.[StandardCost])
END) + (CASE
    WHEN source.[ListPrice] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ListPrice]))) + CONVERT(varbinary(max), source.[ListPrice])
END) + (CASE
    WHEN source.[Size] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Size]))) + CONVERT(varbinary(max), source.[Size])
END) + (CASE
    WHEN source.[Weight] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Weight]))) + CONVERT(varbinary(max), source.[Weight])
END) + (CASE
    WHEN source.[SellStartDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SellStartDate]))) + CONVERT(varbinary(max), source.[SellStartDate])
END) + (CASE
    WHEN source.[SellEndDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SellEndDate]))) + CONVERT(varbinary(max), source.[SellEndDate])
END) + (CASE
    WHEN source.[DiscontinuedDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[DiscontinuedDate]))) + CONVERT(varbinary(max), source.[DiscontinuedDate])
END))),
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.HS_Product_Product'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[HS_Product_Product] AS source
INNER JOIN (
SELECT
    candidate.[HubHashKey],
    candidate.[LoadTimestamp],
    candidate.[AuditId],
    ROW_NUMBER() OVER (PARTITION BY candidate.[HubHashKey] ORDER BY candidate.[LoadTimestamp] DESC, candidate.[AuditId] DESC) AS [VersionRank]
FROM [AdventureWorksRawVault].[dbo].[HS_Product_Product] AS candidate
) AS latest
    ON latest.[HubHashKey] = source.[HubHashKey] AND latest.[LoadTimestamp] = source.[LoadTimestamp] AND latest.[AuditId] = source.[AuditId] AND latest.[VersionRank] = 1
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Product] AS hub
    ON hub.[HashKey] = source.[HubHashKey]
LEFT OUTER JOIN (
SELECT
    existing.[HubHashKey],
    existing.[HashDiff],
    ROW_NUMBER() OVER (PARTITION BY existing.[HubHashKey] ORDER BY existing.[LoadTimestamp] DESC, existing.[AuditId] DESC) AS [VersionRank]
FROM [AdventureWorksBusinessVault].[dbo].[BHS_Product_ProductProfile] AS existing
) AS currentState
    ON currentState.[HubHashKey] = source.[HubHashKey] AND currentState.[VersionRank] = 1
WHERE (currentState.[HashDiff] IS NULL OR currentState.[HashDiff] <> CONVERT(binary(32), HASHBYTES('SHA2_256', (CASE
    WHEN source.[Name] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Name]))) + CONVERT(varbinary(max), source.[Name])
END) + (CASE
    WHEN source.[ProductNumber] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductNumber]))) + CONVERT(varbinary(max), source.[ProductNumber])
END) + (CASE
    WHEN source.[Color] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Color]))) + CONVERT(varbinary(max), source.[Color])
END) + (CASE
    WHEN source.[StandardCost] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[StandardCost]))) + CONVERT(varbinary(max), source.[StandardCost])
END) + (CASE
    WHEN source.[ListPrice] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ListPrice]))) + CONVERT(varbinary(max), source.[ListPrice])
END) + (CASE
    WHEN source.[Size] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Size]))) + CONVERT(varbinary(max), source.[Size])
END) + (CASE
    WHEN source.[Weight] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Weight]))) + CONVERT(varbinary(max), source.[Weight])
END) + (CASE
    WHEN source.[SellStartDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SellStartDate]))) + CONVERT(varbinary(max), source.[SellStartDate])
END) + (CASE
    WHEN source.[SellEndDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SellEndDate]))) + CONVERT(varbinary(max), source.[SellEndDate])
END) + (CASE
    WHEN source.[DiscontinuedDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[DiscontinuedDate]))) + CONVERT(varbinary(max), source.[DiscontinuedDate])
END))))