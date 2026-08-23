INSERT INTO [AdventureWorksBusinessVault].[dbo].[BHS_Address_AddressProfile] ([HubHashKey], [AddressLine1], [AddressLine2], [City], [PostalCode], [HashDiff], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    source.[HubHashKey],
    source.[AddressLine1],
    source.[AddressLine2],
    source.[City],
    source.[PostalCode],
    CONVERT(binary(32), HASHBYTES('SHA2_256', (CASE
    WHEN source.[AddressLine1] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[AddressLine1]))) + CONVERT(varbinary(max), source.[AddressLine1])
END) + (CASE
    WHEN source.[AddressLine2] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[AddressLine2]))) + CONVERT(varbinary(max), source.[AddressLine2])
END) + (CASE
    WHEN source.[City] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[City]))) + CONVERT(varbinary(max), source.[City])
END) + (CASE
    WHEN source.[PostalCode] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[PostalCode]))) + CONVERT(varbinary(max), source.[PostalCode])
END))),
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.HS_Address_Address'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[HS_Address_Address] AS source
INNER JOIN (
SELECT
    candidate.[HubHashKey],
    candidate.[LoadTimestamp],
    candidate.[AuditId],
    ROW_NUMBER() OVER (PARTITION BY candidate.[HubHashKey] ORDER BY candidate.[LoadTimestamp] DESC, candidate.[AuditId] DESC) AS [VersionRank]
FROM [AdventureWorksRawVault].[dbo].[HS_Address_Address] AS candidate
) AS latest
    ON latest.[HubHashKey] = source.[HubHashKey] AND latest.[LoadTimestamp] = source.[LoadTimestamp] AND latest.[AuditId] = source.[AuditId] AND latest.[VersionRank] = 1
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Address] AS hub
    ON hub.[HashKey] = source.[HubHashKey]
LEFT OUTER JOIN (
SELECT
    existing.[HubHashKey],
    existing.[HashDiff],
    ROW_NUMBER() OVER (PARTITION BY existing.[HubHashKey] ORDER BY existing.[LoadTimestamp] DESC, existing.[AuditId] DESC) AS [VersionRank]
FROM [AdventureWorksBusinessVault].[dbo].[BHS_Address_AddressProfile] AS existing
) AS currentState
    ON currentState.[HubHashKey] = source.[HubHashKey] AND currentState.[VersionRank] = 1
WHERE currentState.[HashDiff] IS NULL OR currentState.[HashDiff] <> CONVERT(binary(32), HASHBYTES('SHA2_256', (CASE
    WHEN source.[AddressLine1] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[AddressLine1]))) + CONVERT(varbinary(max), source.[AddressLine1])
END) + (CASE
    WHEN source.[AddressLine2] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[AddressLine2]))) + CONVERT(varbinary(max), source.[AddressLine2])
END) + (CASE
    WHEN source.[City] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[City]))) + CONVERT(varbinary(max), source.[City])
END) + (CASE
    WHEN source.[PostalCode] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[PostalCode]))) + CONVERT(varbinary(max), source.[PostalCode])
END)))
