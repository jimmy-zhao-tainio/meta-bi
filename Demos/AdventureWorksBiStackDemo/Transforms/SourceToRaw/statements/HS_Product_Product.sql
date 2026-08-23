WITH latest AS (
SELECT
    existing.[HubHashKey],
    existing.[HashDiff],
    ROW_NUMBER() OVER (PARTITION BY existing.[HubHashKey] ORDER BY existing.[LoadTimestamp] DESC, existing.[AuditId] DESC) AS [VersionRank]
FROM [AdventureWorksRawVault].[dbo].[HS_Product_Product] AS existing
)
INSERT INTO [AdventureWorksRawVault].[dbo].[HS_Product_Product] ([HubHashKey], [Name], [ProductNumber], [MakeFlag], [FinishedGoodsFlag], [Color], [SafetyStockLevel], [ReorderPoint], [StandardCost], [ListPrice], [Size], [Weight], [DaysToManufacture], [ProductLine], [Class], [Style], [SellStartDate], [SellEndDate], [DiscontinuedDate], [rowguid], [ModifiedDate], [HashDiff], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductID]))) + CONVERT(varbinary(max), source.[ProductID]))) AS HubHashKey,
    source.[Name],
    source.[ProductNumber],
    source.[MakeFlag],
    source.[FinishedGoodsFlag],
    source.[Color],
    source.[SafetyStockLevel],
    source.[ReorderPoint],
    source.[StandardCost],
    source.[ListPrice],
    source.[Size],
    source.[Weight],
    source.[DaysToManufacture],
    source.[ProductLine],
    source.[Class],
    source.[Style],
    source.[SellStartDate],
    source.[SellEndDate],
    source.[DiscontinuedDate],
    source.[rowguid],
    source.[ModifiedDate],
    CONVERT(binary(32), HASHBYTES('SHA2_256', (CASE
    WHEN source.[Name] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Name]))) + CONVERT(varbinary(max), source.[Name])
END) + (CASE
    WHEN source.[ProductNumber] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductNumber]))) + CONVERT(varbinary(max), source.[ProductNumber])
END) + (CASE
    WHEN source.[MakeFlag] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[MakeFlag]))) + CONVERT(varbinary(max), source.[MakeFlag])
END) + (CASE
    WHEN source.[FinishedGoodsFlag] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[FinishedGoodsFlag]))) + CONVERT(varbinary(max), source.[FinishedGoodsFlag])
END) + (CASE
    WHEN source.[Color] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Color]))) + CONVERT(varbinary(max), source.[Color])
END) + (CASE
    WHEN source.[SafetyStockLevel] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SafetyStockLevel]))) + CONVERT(varbinary(max), source.[SafetyStockLevel])
END) + (CASE
    WHEN source.[ReorderPoint] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ReorderPoint]))) + CONVERT(varbinary(max), source.[ReorderPoint])
END) + (CASE
    WHEN source.[StandardCost] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[StandardCost]))) + CONVERT(varbinary(max), source.[StandardCost])
END) + (CASE
    WHEN source.[ListPrice] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ListPrice]))) + CONVERT(varbinary(max), source.[ListPrice])
END) + (CASE
    WHEN source.[Size] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Size]))) + CONVERT(varbinary(max), source.[Size])
END) + (CASE
    WHEN source.[Weight] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Weight]))) + CONVERT(varbinary(max), source.[Weight])
END) + (CASE
    WHEN source.[DaysToManufacture] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[DaysToManufacture]))) + CONVERT(varbinary(max), source.[DaysToManufacture])
END) + (CASE
    WHEN source.[ProductLine] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductLine]))) + CONVERT(varbinary(max), source.[ProductLine])
END) + (CASE
    WHEN source.[Class] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Class]))) + CONVERT(varbinary(max), source.[Class])
END) + (CASE
    WHEN source.[Style] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Style]))) + CONVERT(varbinary(max), source.[Style])
END) + (CASE
    WHEN source.[SellStartDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SellStartDate]))) + CONVERT(varbinary(max), source.[SellStartDate])
END) + (CASE
    WHEN source.[SellEndDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SellEndDate]))) + CONVERT(varbinary(max), source.[SellEndDate])
END) + (CASE
    WHEN source.[DiscontinuedDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[DiscontinuedDate]))) + CONVERT(varbinary(max), source.[DiscontinuedDate])
END) + (CASE
    WHEN source.[rowguid] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[rowguid]))) + CONVERT(varbinary(max), source.[rowguid])
END) + (CASE
    WHEN source.[ModifiedDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ModifiedDate]))) + CONVERT(varbinary(max), source.[ModifiedDate])
END))) AS HashDiff,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Production.Product') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Production].[Product] AS source
INNER JOIN [AdventureWorksRawVault].[dbo].[H_Product] AS hub
    ON hub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductID]))) + CONVERT(varbinary(max), source.[ProductID])))
LEFT OUTER JOIN latest
    ON latest.[HubHashKey] = hub.[HashKey] AND latest.[VersionRank] = 1
WHERE latest.HashDiff IS NULL OR latest.HashDiff <> CONVERT(binary(32), HASHBYTES('SHA2_256', (CASE
    WHEN source.[Name] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Name]))) + CONVERT(varbinary(max), source.[Name])
END) + (CASE
    WHEN source.[ProductNumber] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductNumber]))) + CONVERT(varbinary(max), source.[ProductNumber])
END) + (CASE
    WHEN source.[MakeFlag] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[MakeFlag]))) + CONVERT(varbinary(max), source.[MakeFlag])
END) + (CASE
    WHEN source.[FinishedGoodsFlag] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[FinishedGoodsFlag]))) + CONVERT(varbinary(max), source.[FinishedGoodsFlag])
END) + (CASE
    WHEN source.[Color] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Color]))) + CONVERT(varbinary(max), source.[Color])
END) + (CASE
    WHEN source.[SafetyStockLevel] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SafetyStockLevel]))) + CONVERT(varbinary(max), source.[SafetyStockLevel])
END) + (CASE
    WHEN source.[ReorderPoint] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ReorderPoint]))) + CONVERT(varbinary(max), source.[ReorderPoint])
END) + (CASE
    WHEN source.[StandardCost] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[StandardCost]))) + CONVERT(varbinary(max), source.[StandardCost])
END) + (CASE
    WHEN source.[ListPrice] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ListPrice]))) + CONVERT(varbinary(max), source.[ListPrice])
END) + (CASE
    WHEN source.[Size] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Size]))) + CONVERT(varbinary(max), source.[Size])
END) + (CASE
    WHEN source.[Weight] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Weight]))) + CONVERT(varbinary(max), source.[Weight])
END) + (CASE
    WHEN source.[DaysToManufacture] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[DaysToManufacture]))) + CONVERT(varbinary(max), source.[DaysToManufacture])
END) + (CASE
    WHEN source.[ProductLine] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductLine]))) + CONVERT(varbinary(max), source.[ProductLine])
END) + (CASE
    WHEN source.[Class] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Class]))) + CONVERT(varbinary(max), source.[Class])
END) + (CASE
    WHEN source.[Style] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[Style]))) + CONVERT(varbinary(max), source.[Style])
END) + (CASE
    WHEN source.[SellStartDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SellStartDate]))) + CONVERT(varbinary(max), source.[SellStartDate])
END) + (CASE
    WHEN source.[SellEndDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SellEndDate]))) + CONVERT(varbinary(max), source.[SellEndDate])
END) + (CASE
    WHEN source.[DiscontinuedDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[DiscontinuedDate]))) + CONVERT(varbinary(max), source.[DiscontinuedDate])
END) + (CASE
    WHEN source.[rowguid] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[rowguid]))) + CONVERT(varbinary(max), source.[rowguid])
END) + (CASE
    WHEN source.[ModifiedDate] IS NULL THEN CONVERT(varbinary(max), 0x00)
    ELSE CONVERT(varbinary(max), 0x01) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ModifiedDate]))) + CONVERT(varbinary(max), source.[ModifiedDate])
END)))