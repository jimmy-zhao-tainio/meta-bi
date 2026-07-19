INSERT INTO [AdventureWorksRawVault].[dbo].[H_SpecialOfferProduct] ([HashKey], [SpecialOfferID], [ProductID], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.SpecialOfferID))) + CONVERT(varbinary(max), source.SpecialOfferID) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.ProductID))) + CONVERT(varbinary(max), source.ProductID))) AS HashKey,
    source.SpecialOfferID,
    source.ProductID,
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SpecialOfferProduct') AS RecordSource,
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')) AS AuditId
FROM [AdventureWorks2022].[Sales].[SpecialOfferProduct] AS source
LEFT OUTER JOIN [AdventureWorksRawVault].[dbo].[H_SpecialOfferProduct] AS existing
    ON existing.HashKey = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.SpecialOfferID))) + CONVERT(varbinary(max), source.SpecialOfferID) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), source.ProductID))) + CONVERT(varbinary(max), source.ProductID)))
WHERE existing.HashKey IS NULL