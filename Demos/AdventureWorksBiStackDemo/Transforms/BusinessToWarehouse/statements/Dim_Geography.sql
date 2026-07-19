WITH latestAddress AS (
    SELECT source.[HubHashKey], source.[AddressLine1], source.[AddressLine2], source.[City], source.[PostalCode],
        ROW_NUMBER() OVER (PARTITION BY source.[HubHashKey] ORDER BY source.[LoadTimestamp] DESC, source.[AuditId] DESC) AS [VersionRank]
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_Address_AddressProfile] AS source
), latestStateProvince AS (
    SELECT source.[HubHashKey], source.[StateProvinceCode], source.[Name],
        ROW_NUMBER() OVER (PARTITION BY source.[HubHashKey] ORDER BY source.[LoadTimestamp] DESC, source.[AuditId] DESC) AS [VersionRank]
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_StateProvince_StateProvinceProfile] AS source
), latestCountryRegion AS (
    SELECT source.[HubHashKey], source.[Name],
        ROW_NUMBER() OVER (PARTITION BY source.[HubHashKey] ORDER BY source.[LoadTimestamp] DESC, source.[AuditId] DESC) AS [VersionRank]
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_CountryRegion_CountryRegionProfile] AS source
)
MERGE INTO [AdventureWorksAnalytics].[dw].[Dim_Geography] AS target
USING (
    SELECT address.[AddressID], addressProfile.[AddressLine1], addressProfile.[AddressLine2], addressProfile.[City], addressProfile.[PostalCode],
        stateProvinceProfile.[StateProvinceCode], stateProvinceProfile.[Name] AS [StateProvinceName],
        countryRegion.[CountryRegionCode], countryRegionProfile.[Name] AS [CountryRegionName]
    FROM [AdventureWorksBusinessVault].[dbo].[BH_Address] AS address
    LEFT OUTER JOIN latestAddress AS addressProfile
        ON addressProfile.[HubHashKey] = address.[HashKey] AND addressProfile.[VersionRank] = 1
    LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_AddressStateProvince] AS addressStateProvince
        ON addressStateProvince.[AddressHashKey] = address.[HashKey]
    LEFT OUTER JOIN latestStateProvince AS stateProvinceProfile
        ON stateProvinceProfile.[HubHashKey] = addressStateProvince.[StateProvinceHashKey] AND stateProvinceProfile.[VersionRank] = 1
    LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_StateProvinceCountryRegion] AS stateProvinceCountryRegion
        ON stateProvinceCountryRegion.[StateProvinceHashKey] = addressStateProvince.[StateProvinceHashKey]
    LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BH_CountryRegion] AS countryRegion
        ON countryRegion.[HashKey] = stateProvinceCountryRegion.[CountryRegionHashKey]
    LEFT OUTER JOIN latestCountryRegion AS countryRegionProfile
        ON countryRegionProfile.[HubHashKey] = countryRegion.[HashKey] AND countryRegionProfile.[VersionRank] = 1
) AS source
ON target.[AddressID] = source.[AddressID]
WHEN MATCHED THEN UPDATE SET
    [AddressLine1] = source.[AddressLine1], [AddressLine2] = source.[AddressLine2], [City] = source.[City],
    [StateProvinceCode] = source.[StateProvinceCode], [StateProvinceName] = source.[StateProvinceName], [PostalCode] = source.[PostalCode],
    [CountryRegionCode] = source.[CountryRegionCode], [CountryRegionName] = source.[CountryRegionName]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([GeographyKey], [AddressID], [AddressLine1], [AddressLine2], [City], [StateProvinceCode], [StateProvinceName], [PostalCode], [CountryRegionCode], [CountryRegionName], [AuditId], [InsertDateTime2])
    VALUES (
        CONVERT(bigint, source.[AddressID]), source.[AddressID], source.[AddressLine1], source.[AddressLine2], source.[City],
        source.[StateProvinceCode], source.[StateProvinceName], source.[PostalCode], source.[CountryRegionCode], source.[CountryRegionName],
        CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')),
        CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required'))
    );
