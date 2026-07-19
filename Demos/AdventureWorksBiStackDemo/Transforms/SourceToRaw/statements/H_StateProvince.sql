INSERT INTO [AdventureWorksRawVault].[dbo].[H_StateProvince] ([HashKey], [StateProvinceID], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.StateProvinceID))) + CONVERT(varbinary(max), source.StateProvinceID))) AS HashKey,
    source.StateProvinceID,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Person.StateProvince') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Person].[StateProvince] AS source
LEFT OUTER JOIN [AdventureWorksRawVault].[dbo].[H_StateProvince] AS existing
    ON existing.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.StateProvinceID))) + CONVERT(varbinary(max), source.StateProvinceID)))
WHERE existing.HashKey IS NULL
