INSERT INTO [AdventureWorksBusinessVault].[dbo].[BH_CountryRegion] ([HashKey], [CountryRegionCode], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    source.[HashKey],
    source.[CountryRegionCode],
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.H_CountryRegion'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[H_CountryRegion] AS source
LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BH_CountryRegion] AS existing
    ON existing.[HashKey] = source.[HashKey]
WHERE existing.[HashKey] IS NULL AND source.[CountryRegionCode] IS NOT NULL
