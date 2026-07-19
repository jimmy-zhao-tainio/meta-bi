WITH latestPerson AS (
    SELECT source.[HubHashKey], source.[FirstName], source.[LastName],
        ROW_NUMBER() OVER (PARTITION BY source.[HubHashKey] ORDER BY source.[LoadTimestamp] DESC, source.[AuditId] DESC) AS [VersionRank]
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_Person_PersonProfile] AS source
)
MERGE INTO [AdventureWorksAnalytics].[dw].[Dim_Salesperson] AS target
USING (
    SELECT salesperson.[BusinessEntityID], person.[FirstName], person.[LastName],
        CONVERT(nvarchar(256), CONCAT(person.[FirstName], N' ', person.[LastName])) AS [FullName]
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS salesperson
    LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesPersonPerson] AS salespersonPerson
        ON salespersonPerson.[SalesPersonHashKey] = salesperson.[HashKey]
    LEFT OUTER JOIN latestPerson AS person
        ON person.[HubHashKey] = salespersonPerson.[PersonHashKey] AND person.[VersionRank] = 1
) AS source
ON target.[BusinessEntityID] = source.[BusinessEntityID]
WHEN MATCHED THEN UPDATE SET
    [FirstName] = source.[FirstName], [LastName] = source.[LastName], [FullName] = source.[FullName]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([SalespersonKey], [BusinessEntityID], [FirstName], [LastName], [FullName], [AuditId], [InsertDateTime2])
    VALUES (
        CONVERT(bigint, source.[BusinessEntityID]), source.[BusinessEntityID], source.[FirstName], source.[LastName], source.[FullName],
        CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')),
        CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required'))
    );
