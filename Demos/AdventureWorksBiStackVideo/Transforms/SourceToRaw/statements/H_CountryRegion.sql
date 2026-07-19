INSERT INTO [AdventureWorksRawVault].[dbo].[H_CountryRegion] ([HashKey], [CountryRegionCode], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.CountryRegionCode))) + CONVERT(varbinary(max), source.CountryRegionCode))) AS HashKey,
    source.CountryRegionCode,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Person.CountryRegion') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Person].[CountryRegion] AS source
LEFT OUTER JOIN [AdventureWorksRawVault].[dbo].[H_CountryRegion] AS existing
    ON existing.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.CountryRegionCode))) + CONVERT(varbinary(max), source.CountryRegionCode)))
WHERE existing.HashKey IS NULL