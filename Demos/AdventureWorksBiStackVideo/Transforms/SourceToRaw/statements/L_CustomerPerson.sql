INSERT INTO [AdventureWorksRawVault].[dbo].[L_CustomerPerson] ([HashKey], [CustomerHashKey], [PersonHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CustomerID]))) + CONVERT(varbinary(max), source.[CustomerID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[PersonID]))) + CONVERT(varbinary(max), source.[PersonID]))) AS HashKey,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CustomerID]))) + CONVERT(varbinary(max), source.[CustomerID]))) AS CustomerHashKey,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[PersonID]))) + CONVERT(varbinary(max), source.[PersonID]))) AS PersonHashKey,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.Customer') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Sales].[Customer] AS source
INNER JOIN [AdventureWorksRawVault].[dbo].[H_Customer] AS customerHub
    ON customerHub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CustomerID]))) + CONVERT(varbinary(max), source.[CustomerID])))
INNER JOIN [AdventureWorksRawVault].[dbo].[H_Person] AS personHub
    ON personHub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[PersonID]))) + CONVERT(varbinary(max), source.[PersonID])))
LEFT OUTER JOIN [AdventureWorksRawVault].[dbo].[L_CustomerPerson] AS existing
    ON existing.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[CustomerID]))) + CONVERT(varbinary(max), source.[CustomerID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[PersonID]))) + CONVERT(varbinary(max), source.[PersonID])))
WHERE existing.HashKey IS NULL AND (source.[PersonID] IS NOT NULL)