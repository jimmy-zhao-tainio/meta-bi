WITH latestCustomer AS (
    SELECT source.[HubHashKey], source.[AccountNumber],
        ROW_NUMBER() OVER (PARTITION BY source.[HubHashKey] ORDER BY source.[LoadTimestamp] DESC, source.[AuditId] DESC) AS [VersionRank]
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_Customer_CustomerProfile] AS source
), latestPerson AS (
    SELECT source.[HubHashKey], source.[FirstName], source.[MiddleName], source.[LastName],
        ROW_NUMBER() OVER (PARTITION BY source.[HubHashKey] ORDER BY source.[LoadTimestamp] DESC, source.[AuditId] DESC) AS [VersionRank]
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_Person_PersonProfile] AS source
), latestStore AS (
    SELECT source.[HubHashKey], source.[Name],
        ROW_NUMBER() OVER (PARTITION BY source.[HubHashKey] ORDER BY source.[LoadTimestamp] DESC, source.[AuditId] DESC) AS [VersionRank]
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_Store_StoreProfile] AS source
)
MERGE INTO [AdventureWorksAnalytics].[dw].[Dim_Customer] AS target
USING (
    SELECT customer.[CustomerID], customerProfile.[AccountNumber],
        CONVERT(nvarchar(256), CASE
            WHEN storeProfile.[Name] IS NOT NULL THEN storeProfile.[Name]
            WHEN personProfile.[FirstName] IS NOT NULL OR personProfile.[LastName] IS NOT NULL
                THEN CONCAT(personProfile.[FirstName], N' ', personProfile.[LastName])
        END) AS [CustomerName],
        CONVERT(nvarchar(256), CASE
            WHEN customerStore.[StoreHashKey] IS NOT NULL THEN N'Store'
            WHEN customerPerson.[PersonHashKey] IS NOT NULL THEN N'Individual'
        END) AS [CustomerType],
        storeProfile.[Name] AS [StoreName]
    FROM [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS customer
    LEFT OUTER JOIN latestCustomer AS customerProfile
        ON customerProfile.[HubHashKey] = customer.[HashKey] AND customerProfile.[VersionRank] = 1
    LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_CustomerPerson] AS customerPerson
        ON customerPerson.[CustomerHashKey] = customer.[HashKey]
    LEFT OUTER JOIN latestPerson AS personProfile
        ON personProfile.[HubHashKey] = customerPerson.[PersonHashKey] AND personProfile.[VersionRank] = 1
    LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_CustomerStore] AS customerStore
        ON customerStore.[CustomerHashKey] = customer.[HashKey]
    LEFT OUTER JOIN latestStore AS storeProfile
        ON storeProfile.[HubHashKey] = customerStore.[StoreHashKey] AND storeProfile.[VersionRank] = 1
) AS source
ON target.[CustomerID] = source.[CustomerID]
WHEN MATCHED THEN UPDATE SET
    [AccountNumber] = source.[AccountNumber], [CustomerName] = source.[CustomerName],
    [CustomerType] = source.[CustomerType], [StoreName] = source.[StoreName]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([CustomerKey], [CustomerID], [AccountNumber], [CustomerName], [CustomerType], [StoreName], [AuditId], [InsertDateTime2])
    VALUES (
        CONVERT(bigint, source.[CustomerID]), source.[CustomerID], source.[AccountNumber], source.[CustomerName], source.[CustomerType], source.[StoreName],
        CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')),
        CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required'))
    );
