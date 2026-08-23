WITH latestProduct AS (
    SELECT source.[HubHashKey], source.[ProductNumber], source.[Name], source.[Color],
        ROW_NUMBER() OVER (PARTITION BY source.[HubHashKey] ORDER BY source.[LoadTimestamp] DESC, source.[AuditId] DESC) AS [VersionRank]
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_Product_ProductProfile] AS source
), latestSubcategory AS (
    SELECT source.[HubHashKey], source.[Name],
        ROW_NUMBER() OVER (PARTITION BY source.[HubHashKey] ORDER BY source.[LoadTimestamp] DESC, source.[AuditId] DESC) AS [VersionRank]
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_ProductSubcategory_ProductSubcategoryProfile] AS source
), latestCategory AS (
    SELECT source.[HubHashKey], source.[Name],
        ROW_NUMBER() OVER (PARTITION BY source.[HubHashKey] ORDER BY source.[LoadTimestamp] DESC, source.[AuditId] DESC) AS [VersionRank]
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_ProductCategory_ProductCategoryProfile] AS source
)
MERGE INTO [AdventureWorksAnalytics].[dw].[Dim_Product] AS target
USING (
    SELECT hub.[ProductID], profile.[ProductNumber], profile.[Name] AS [ProductName], profile.[Color],
        subcategoryProfile.[Name] AS [ProductSubcategoryName], categoryProfile.[Name] AS [ProductCategoryName]
    FROM [AdventureWorksBusinessVault].[dbo].[BH_Product] AS hub
    LEFT OUTER JOIN latestProduct AS profile
        ON profile.[HubHashKey] = hub.[HashKey] AND profile.[VersionRank] = 1
    LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_ProductSubcategoryAssignment] AS assignment
        ON assignment.[ProductHashKey] = hub.[HashKey]
    LEFT OUTER JOIN latestSubcategory AS subcategoryProfile
        ON subcategoryProfile.[HubHashKey] = assignment.[ProductSubcategoryHashKey] AND subcategoryProfile.[VersionRank] = 1
    LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_ProductSubcategoryCategory] AS categoryAssignment
        ON categoryAssignment.[ProductSubcategoryHashKey] = assignment.[ProductSubcategoryHashKey]
    LEFT OUTER JOIN latestCategory AS categoryProfile
        ON categoryProfile.[HubHashKey] = categoryAssignment.[ProductCategoryHashKey] AND categoryProfile.[VersionRank] = 1
) AS source
ON target.[ProductID] = source.[ProductID]
WHEN MATCHED THEN UPDATE SET
    [ProductNumber] = source.[ProductNumber],
    [ProductName] = source.[ProductName],
    [Color] = source.[Color],
    [ProductSubcategoryName] = source.[ProductSubcategoryName],
    [ProductCategoryName] = source.[ProductCategoryName]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([ProductKey], [ProductID], [ProductNumber], [ProductName], [Color], [ProductSubcategoryName], [ProductCategoryName], [AuditId], [InsertDateTime2])
    VALUES (
        CONVERT(bigint, source.[ProductID]), source.[ProductID], source.[ProductNumber], source.[ProductName], source.[Color],
        source.[ProductSubcategoryName], source.[ProductCategoryName],
        CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')),
        CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required'))
    );
