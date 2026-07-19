INSERT INTO [AdventureWorksBusinessVault].[dbo].[BL_AddressStateProvince] ([HashKey], [AddressHashKey], [StateProvinceHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[AddressHashKey]))) + CONVERT(varbinary(max), source.[AddressHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[StateProvinceHashKey]))) + CONVERT(varbinary(max), source.[StateProvinceHashKey]))),
    source.[AddressHashKey],
    source.[StateProvinceHashKey],
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.L_AddressStateProvince'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[L_AddressStateProvince] AS source
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Address] AS addressHub
    ON addressHub.[HashKey] = source.[AddressHashKey]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_StateProvince] AS stateProvinceHub
    ON stateProvinceHub.[HashKey] = source.[StateProvinceHashKey]
LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_AddressStateProvince] AS existing
    ON existing.[HashKey] = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[AddressHashKey]))) + CONVERT(varbinary(max), source.[AddressHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[StateProvinceHashKey]))) + CONVERT(varbinary(max), source.[StateProvinceHashKey])))
WHERE existing.[HashKey] IS NULL
