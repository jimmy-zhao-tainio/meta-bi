INSERT INTO [AdventureWorksBusinessVault].[dbo].[BHS_Person_PersonProfile] ([HubHashKey], [FirstName], [MiddleName], [LastName], [HashDiff], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    source.[HubHashKey],
    source.[FirstName],
    source.[MiddleName],
    source.[LastName],
    CONVERT(binary(32), HASHBYTES('SHA2_256', (CASE
    WHEN source.[FirstName] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[FirstName]))) + CONVERT(varbinary(max), source.[FirstName])
END) + (CASE
    WHEN source.[MiddleName] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[MiddleName]))) + CONVERT(varbinary(max), source.[MiddleName])
END) + (CASE
    WHEN source.[LastName] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[LastName]))) + CONVERT(varbinary(max), source.[LastName])
END))),
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.HS_Person_Person'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[HS_Person_Person] AS source
INNER JOIN (
SELECT
    candidate.[HubHashKey],
    candidate.[LoadTimestamp],
    candidate.[AuditId],
    ROW_NUMBER() OVER (PARTITION BY candidate.[HubHashKey] ORDER BY candidate.[LoadTimestamp] DESC, candidate.[AuditId] DESC) AS [VersionRank]
FROM [AdventureWorksRawVault].[dbo].[HS_Person_Person] AS candidate
) AS latest
    ON latest.[HubHashKey] = source.[HubHashKey] AND latest.[LoadTimestamp] = source.[LoadTimestamp] AND latest.[AuditId] = source.[AuditId] AND latest.[VersionRank] = 1
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Person] AS hub
    ON hub.[HashKey] = source.[HubHashKey]
LEFT OUTER JOIN (
SELECT
    existing.[HubHashKey],
    existing.[HashDiff],
    ROW_NUMBER() OVER (PARTITION BY existing.[HubHashKey] ORDER BY existing.[LoadTimestamp] DESC, existing.[AuditId] DESC) AS [VersionRank]
FROM [AdventureWorksBusinessVault].[dbo].[BHS_Person_PersonProfile] AS existing
) AS currentState
    ON currentState.[HubHashKey] = source.[HubHashKey] AND currentState.[VersionRank] = 1
WHERE (currentState.[HashDiff] IS NULL OR currentState.[HashDiff] <> CONVERT(binary(32), HASHBYTES('SHA2_256', (CASE
    WHEN source.[FirstName] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[FirstName]))) + CONVERT(varbinary(max), source.[FirstName])
END) + (CASE
    WHEN source.[MiddleName] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[MiddleName]))) + CONVERT(varbinary(max), source.[MiddleName])
END) + (CASE
    WHEN source.[LastName] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[LastName]))) + CONVERT(varbinary(max), source.[LastName])
END))))