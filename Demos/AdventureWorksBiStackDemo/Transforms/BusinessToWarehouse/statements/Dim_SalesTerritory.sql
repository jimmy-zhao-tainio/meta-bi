WITH latestSalesTerritory AS (
    SELECT source.[HubHashKey], source.[Name], source.[GroupName],
        ROW_NUMBER() OVER (PARTITION BY source.[HubHashKey] ORDER BY source.[LoadTimestamp] DESC, source.[AuditId] DESC) AS [VersionRank]
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_SalesTerritory_SalesTerritoryProfile] AS source
)
MERGE INTO [AdventureWorksAnalytics].[dw].[Dim_SalesTerritory] AS target
USING (
    SELECT territory.[TerritoryID], profile.[Name] AS [TerritoryName], countryRegion.[CountryRegionCode], profile.[GroupName] AS [TerritoryGroup]
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS territory
    LEFT OUTER JOIN latestSalesTerritory AS profile
        ON profile.[HubHashKey] = territory.[HashKey] AND profile.[VersionRank] = 1
    LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesTerritoryCountryRegion] AS territoryCountryRegion
        ON territoryCountryRegion.[SalesTerritoryHashKey] = territory.[HashKey]
    LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BH_CountryRegion] AS countryRegion
        ON countryRegion.[HashKey] = territoryCountryRegion.[CountryRegionHashKey]
) AS source
ON target.[TerritoryID] = source.[TerritoryID]
WHEN MATCHED THEN UPDATE SET
    [TerritoryName] = source.[TerritoryName], [CountryRegionCode] = source.[CountryRegionCode], [TerritoryGroup] = source.[TerritoryGroup]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([SalesTerritoryKey], [TerritoryID], [TerritoryName], [CountryRegionCode], [TerritoryGroup], [AuditId], [InsertDateTime2])
    VALUES (
        CONVERT(bigint, source.[TerritoryID]), source.[TerritoryID], source.[TerritoryName], source.[CountryRegionCode], source.[TerritoryGroup],
        CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')),
        CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required'))
    );
