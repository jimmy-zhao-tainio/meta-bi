WITH latest AS (
SELECT
    existing.[HubHashKey],
    existing.[HashDiff],
    ROW_NUMBER() OVER (PARTITION BY existing.[HubHashKey] ORDER BY existing.[LoadTimestamp] DESC, existing.[AuditId] DESC) AS [VersionRank]
FROM [AdventureWorksRawVault].[dbo].[HS_Person_Person] AS existing
)
INSERT INTO [AdventureWorksRawVault].[dbo].[HS_Person_Person] ([HubHashKey], [PersonType], [NameStyle], [Title], [FirstName], [MiddleName], [LastName], [Suffix], [EmailPromotion], [AdditionalContactInfo], [Demographics], [rowguid], [ModifiedDate], [HashDiff], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[BusinessEntityID]))) + CONVERT(varbinary(max), source.[BusinessEntityID]))) AS HubHashKey,
    source.[PersonType],
    source.[NameStyle],
    source.[Title],
    source.[FirstName],
    source.[MiddleName],
    source.[LastName],
    source.[Suffix],
    source.[EmailPromotion],
    source.[AdditionalContactInfo],
    source.[Demographics],
    source.[rowguid],
    source.[ModifiedDate],
    CONVERT(binary(32), HASHBYTES('SHA2_256', (CASE
    WHEN source.[PersonType] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[PersonType]))) + CONVERT(varbinary(max), source.[PersonType])
END) + (CASE
    WHEN source.[NameStyle] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[NameStyle]))) + CONVERT(varbinary(max), source.[NameStyle])
END) + (CASE
    WHEN source.[Title] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Title]))) + CONVERT(varbinary(max), source.[Title])
END) + (CASE
    WHEN source.[FirstName] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[FirstName]))) + CONVERT(varbinary(max), source.[FirstName])
END) + (CASE
    WHEN source.[MiddleName] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[MiddleName]))) + CONVERT(varbinary(max), source.[MiddleName])
END) + (CASE
    WHEN source.[LastName] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[LastName]))) + CONVERT(varbinary(max), source.[LastName])
END) + (CASE
    WHEN source.[Suffix] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Suffix]))) + CONVERT(varbinary(max), source.[Suffix])
END) + (CASE
    WHEN source.[EmailPromotion] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[EmailPromotion]))) + CONVERT(varbinary(max), source.[EmailPromotion])
END) + (CASE
    WHEN source.[AdditionalContactInfo] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[AdditionalContactInfo]))) + CONVERT(varbinary(max), source.[AdditionalContactInfo])
END) + (CASE
    WHEN source.[Demographics] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Demographics]))) + CONVERT(varbinary(max), source.[Demographics])
END) + (CASE
    WHEN source.[rowguid] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[rowguid]))) + CONVERT(varbinary(max), source.[rowguid])
END) + (CASE
    WHEN source.[ModifiedDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ModifiedDate]))) + CONVERT(varbinary(max), source.[ModifiedDate])
END))) AS HashDiff,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Person.Person') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Person].[Person] AS source
INNER JOIN [AdventureWorksRawVault].[dbo].[H_Person] AS hub
    ON hub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[BusinessEntityID]))) + CONVERT(varbinary(max), source.[BusinessEntityID])))
LEFT OUTER JOIN latest
    ON latest.[HubHashKey] = hub.[HashKey] AND latest.[VersionRank] = 1
WHERE latest.HashDiff IS NULL OR latest.HashDiff <> CONVERT(binary(32), HASHBYTES('SHA2_256', (CASE
    WHEN source.[PersonType] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[PersonType]))) + CONVERT(varbinary(max), source.[PersonType])
END) + (CASE
    WHEN source.[NameStyle] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[NameStyle]))) + CONVERT(varbinary(max), source.[NameStyle])
END) + (CASE
    WHEN source.[Title] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Title]))) + CONVERT(varbinary(max), source.[Title])
END) + (CASE
    WHEN source.[FirstName] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[FirstName]))) + CONVERT(varbinary(max), source.[FirstName])
END) + (CASE
    WHEN source.[MiddleName] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[MiddleName]))) + CONVERT(varbinary(max), source.[MiddleName])
END) + (CASE
    WHEN source.[LastName] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[LastName]))) + CONVERT(varbinary(max), source.[LastName])
END) + (CASE
    WHEN source.[Suffix] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Suffix]))) + CONVERT(varbinary(max), source.[Suffix])
END) + (CASE
    WHEN source.[EmailPromotion] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[EmailPromotion]))) + CONVERT(varbinary(max), source.[EmailPromotion])
END) + (CASE
    WHEN source.[AdditionalContactInfo] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[AdditionalContactInfo]))) + CONVERT(varbinary(max), source.[AdditionalContactInfo])
END) + (CASE
    WHEN source.[Demographics] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Demographics]))) + CONVERT(varbinary(max), source.[Demographics])
END) + (CASE
    WHEN source.[rowguid] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[rowguid]))) + CONVERT(varbinary(max), source.[rowguid])
END) + (CASE
    WHEN source.[ModifiedDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ModifiedDate]))) + CONVERT(varbinary(max), source.[ModifiedDate])
END)))