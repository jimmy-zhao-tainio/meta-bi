INSERT INTO [AdventureWorksRawVault].[dbo].[L_StateProvinceCountryRegion] ([HashKey], [StateProvinceHashKey], [CountryRegionHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[StateProvinceID]))) + CONVERT(varbinary(max), source.[StateProvinceID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CountryRegionCode]))) + CONVERT(varbinary(max), source.[CountryRegionCode]))) AS HashKey,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[StateProvinceID]))) + CONVERT(varbinary(max), source.[StateProvinceID]))) AS StateProvinceHashKey,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CountryRegionCode]))) + CONVERT(varbinary(max), source.[CountryRegionCode]))) AS CountryRegionHashKey,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Person.StateProvince') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Person].[StateProvince] AS source
INNER JOIN [AdventureWorksRawVault].[dbo].[H_StateProvince] AS stateProvinceHub
    ON stateProvinceHub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[StateProvinceID]))) + CONVERT(varbinary(max), source.[StateProvinceID])))
INNER JOIN [AdventureWorksRawVault].[dbo].[H_CountryRegion] AS countryRegionHub
    ON countryRegionHub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CountryRegionCode]))) + CONVERT(varbinary(max), source.[CountryRegionCode])))
LEFT OUTER JOIN [AdventureWorksRawVault].[dbo].[L_StateProvinceCountryRegion] AS existing
    ON existing.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[StateProvinceID]))) + CONVERT(varbinary(max), source.[StateProvinceID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CountryRegionCode]))) + CONVERT(varbinary(max), source.[CountryRegionCode])))
WHERE existing.HashKey IS NULL AND source.[StateProvinceID] IS NOT NULL AND source.[CountryRegionCode] IS NOT NULL
