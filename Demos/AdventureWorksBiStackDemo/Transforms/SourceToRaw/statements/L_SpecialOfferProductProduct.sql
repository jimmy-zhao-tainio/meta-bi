INSERT INTO [AdventureWorksRawVault].[dbo].[L_SpecialOfferProductProduct] ([HashKey], [SpecialOfferProductHashKey], [ProductHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SpecialOfferID]))) + CONVERT(varbinary(max), source.[SpecialOfferID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductID]))) + CONVERT(varbinary(max), source.[ProductID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductID]))) + CONVERT(varbinary(max), source.[ProductID]))) AS HashKey,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SpecialOfferID]))) + CONVERT(varbinary(max), source.[SpecialOfferID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductID]))) + CONVERT(varbinary(max), source.[ProductID]))) AS SpecialOfferProductHashKey,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductID]))) + CONVERT(varbinary(max), source.[ProductID]))) AS ProductHashKey,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SpecialOfferProduct') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Sales].[SpecialOfferProduct] AS source
INNER JOIN [AdventureWorksRawVault].[dbo].[H_SpecialOfferProduct] AS specialOfferProductHub
    ON specialOfferProductHub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SpecialOfferID]))) + CONVERT(varbinary(max), source.[SpecialOfferID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductID]))) + CONVERT(varbinary(max), source.[ProductID])))
INNER JOIN [AdventureWorksRawVault].[dbo].[H_Product] AS productHub
    ON productHub.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductID]))) + CONVERT(varbinary(max), source.[ProductID])))
LEFT OUTER JOIN [AdventureWorksRawVault].[dbo].[L_SpecialOfferProductProduct] AS existing
    ON existing.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[SpecialOfferID]))) + CONVERT(varbinary(max), source.[SpecialOfferID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductID]))) + CONVERT(varbinary(max), source.[ProductID]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.[ProductID]))) + CONVERT(varbinary(max), source.[ProductID])))
WHERE existing.HashKey IS NULL