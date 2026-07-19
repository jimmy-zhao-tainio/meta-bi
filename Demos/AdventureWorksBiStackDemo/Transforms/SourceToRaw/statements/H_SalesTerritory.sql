INSERT INTO [AdventureWorksRawVault].[dbo].[H_SalesTerritory] ([HashKey], [TerritoryID], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.TerritoryID))) + CONVERT(varbinary(max), source.TerritoryID))) AS HashKey,
    source.TerritoryID,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesTerritory') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesTerritory] AS source
LEFT OUTER JOIN [AdventureWorksRawVault].[dbo].[H_SalesTerritory] AS existing
    ON existing.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.TerritoryID))) + CONVERT(varbinary(max), source.TerritoryID)))
WHERE existing.HashKey IS NULL