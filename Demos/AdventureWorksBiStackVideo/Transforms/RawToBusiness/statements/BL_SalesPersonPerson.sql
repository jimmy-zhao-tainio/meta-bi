INSERT INTO [AdventureWorksBusinessVault].[dbo].[BL_SalesPersonPerson] ([HashKey], [PersonHashKey], [SalesPersonHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), employeePerson.[PersonHashKey]))) + CONVERT(varbinary(max), employeePerson.[PersonHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), salesPersonEmployee.[SalesPersonHashKey]))) + CONVERT(varbinary(max), salesPersonEmployee.[SalesPersonHashKey]))),
    employeePerson.[PersonHashKey],
    salesPersonEmployee.[SalesPersonHashKey],
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.L_SalesPersonEmployee + L_EmployeePerson'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[L_SalesPersonEmployee] AS salesPersonEmployee
INNER JOIN [AdventureWorksRawVault].[dbo].[L_EmployeePerson] AS employeePerson
    ON employeePerson.[EmployeeHashKey] = salesPersonEmployee.[EmployeeHashKey]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Person] AS personHub
    ON personHub.[HashKey] = employeePerson.[PersonHashKey]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS salesPersonHub
    ON salesPersonHub.[HashKey] = salesPersonEmployee.[SalesPersonHashKey]
LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesPersonPerson] AS existing
    ON existing.[HashKey] = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), employeePerson.[PersonHashKey]))) + CONVERT(varbinary(max), employeePerson.[PersonHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), salesPersonEmployee.[SalesPersonHashKey]))) + CONVERT(varbinary(max), salesPersonEmployee.[SalesPersonHashKey])))
WHERE existing.[HashKey] IS NULL
