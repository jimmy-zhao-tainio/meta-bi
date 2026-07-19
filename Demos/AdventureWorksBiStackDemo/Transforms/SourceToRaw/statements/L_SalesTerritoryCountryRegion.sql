INSERT INTO [AdventureWorksRawVault].[dbo].[L_SalesTerritoryCountryRegion] ([HashKey], [SalesTerritoryHashKey], [CountryRegionHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[TerritoryID]))) + CONVERT(varbinary(max), source.[TerritoryID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CountryRegionCode]))) + CONVERT(varbinary(max), source.[CountryRegionCode]))) AS HashKey,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[TerritoryID]))) + CONVERT(varbinary(max), source.[TerritoryID]))) AS SalesTerritoryHashKey,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CountryRegionCode]))) + CONVERT(varbinary(max), source.[CountryRegionCode]))) AS CountryRegionHashKey,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesTerritory') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesTerritory] AS source
INNER JOIN [AdventureWorksRawVault].[dbo].[H_SalesTerritory] AS salesTerritoryHub
    ON salesTerritoryHub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[TerritoryID]))) + CONVERT(varbinary(max), source.[TerritoryID])))
INNER JOIN [AdventureWorksRawVault].[dbo].[H_CountryRegion] AS countryRegionHub
    ON countryRegionHub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CountryRegionCode]))) + CONVERT(varbinary(max), source.[CountryRegionCode])))
LEFT OUTER JOIN [AdventureWorksRawVault].[dbo].[L_SalesTerritoryCountryRegion] AS existing
    ON existing.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[TerritoryID]))) + CONVERT(varbinary(max), source.[TerritoryID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CountryRegionCode]))) + CONVERT(varbinary(max), source.[CountryRegionCode])))
WHERE existing.HashKey IS NULL