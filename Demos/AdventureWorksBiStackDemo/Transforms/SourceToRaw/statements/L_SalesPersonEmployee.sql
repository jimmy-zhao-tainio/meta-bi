INSERT INTO [AdventureWorksRawVault].[dbo].[L_SalesPersonEmployee] ([HashKey], [SalesPersonHashKey], [EmployeeHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[BusinessEntityID]))) + CONVERT(varbinary(max), source.[BusinessEntityID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[BusinessEntityID]))) + CONVERT(varbinary(max), source.[BusinessEntityID]))) AS HashKey,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[BusinessEntityID]))) + CONVERT(varbinary(max), source.[BusinessEntityID]))) AS SalesPersonHashKey,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[BusinessEntityID]))) + CONVERT(varbinary(max), source.[BusinessEntityID]))) AS EmployeeHashKey,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesPerson') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesPerson] AS source
INNER JOIN [AdventureWorksRawVault].[dbo].[H_SalesPerson] AS salesPersonHub
    ON salesPersonHub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[BusinessEntityID]))) + CONVERT(varbinary(max), source.[BusinessEntityID])))
INNER JOIN [AdventureWorksRawVault].[dbo].[H_Employee] AS employeeHub
    ON employeeHub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[BusinessEntityID]))) + CONVERT(varbinary(max), source.[BusinessEntityID])))
LEFT OUTER JOIN [AdventureWorksRawVault].[dbo].[L_SalesPersonEmployee] AS existing
    ON existing.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[BusinessEntityID]))) + CONVERT(varbinary(max), source.[BusinessEntityID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[BusinessEntityID]))) + CONVERT(varbinary(max), source.[BusinessEntityID])))
WHERE existing.HashKey IS NULL AND source.[BusinessEntityID] IS NOT NULL
