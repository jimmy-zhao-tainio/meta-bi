INSERT INTO [AdventureWorksRawVault].[dbo].[H_Address] ([HashKey], [AddressID], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.AddressID))) + CONVERT(varbinary(max), source.AddressID))) AS HashKey,
    source.AddressID,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Person.Address') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Person].[Address] AS source
LEFT OUTER JOIN [AdventureWorksRawVault].[dbo].[H_Address] AS existing
    ON existing.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.AddressID))) + CONVERT(varbinary(max), source.AddressID)))
WHERE existing.HashKey IS NULL
