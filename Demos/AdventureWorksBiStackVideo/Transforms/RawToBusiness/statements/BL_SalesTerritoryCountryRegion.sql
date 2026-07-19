INSERT INTO [AdventureWorksBusinessVault].[dbo].[BL_SalesTerritoryCountryRegion] ([HashKey], [SalesTerritoryHashKey], [CountryRegionHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesTerritoryHashKey]))) + CONVERT(varbinary(max), source.[SalesTerritoryHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CountryRegionHashKey]))) + CONVERT(varbinary(max), source.[CountryRegionHashKey]))),
    source.[SalesTerritoryHashKey],
    source.[CountryRegionHashKey],
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.L_SalesTerritoryCountryRegion'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[L_SalesTerritoryCountryRegion] AS source
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS endpoint1
    ON endpoint1.[HashKey] = source.[SalesTerritoryHashKey]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_CountryRegion] AS endpoint2
    ON endpoint2.[HashKey] = source.[CountryRegionHashKey]
LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesTerritoryCountryRegion] AS existing
    ON existing.[HashKey] = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SalesTerritoryHashKey]))) + CONVERT(varbinary(max), source.[SalesTerritoryHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CountryRegionHashKey]))) + CONVERT(varbinary(max), source.[CountryRegionHashKey])))
WHERE existing.[HashKey] IS NULL