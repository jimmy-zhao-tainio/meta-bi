INSERT INTO [AdventureWorksRawVault].[dbo].[L_AddressStateProvince] ([HashKey], [AddressHashKey], [StateProvinceHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[AddressID]))) + CONVERT(varbinary(max), source.[AddressID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[StateProvinceID]))) + CONVERT(varbinary(max), source.[StateProvinceID]))) AS HashKey,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[AddressID]))) + CONVERT(varbinary(max), source.[AddressID]))) AS AddressHashKey,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[StateProvinceID]))) + CONVERT(varbinary(max), source.[StateProvinceID]))) AS StateProvinceHashKey,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Person.Address') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Person].[Address] AS source
INNER JOIN [AdventureWorksRawVault].[dbo].[H_Address] AS addressHub
    ON addressHub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[AddressID]))) + CONVERT(varbinary(max), source.[AddressID])))
INNER JOIN [AdventureWorksRawVault].[dbo].[H_StateProvince] AS stateProvinceHub
    ON stateProvinceHub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[StateProvinceID]))) + CONVERT(varbinary(max), source.[StateProvinceID])))
LEFT OUTER JOIN [AdventureWorksRawVault].[dbo].[L_AddressStateProvince] AS existing
    ON existing.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[AddressID]))) + CONVERT(varbinary(max), source.[AddressID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[StateProvinceID]))) + CONVERT(varbinary(max), source.[StateProvinceID])))
WHERE existing.HashKey IS NULL AND source.[AddressID] IS NOT NULL AND source.[StateProvinceID] IS NOT NULL
