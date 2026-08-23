INSERT INTO [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineProduct] ([HashKey], [SalesOrderLineHashKey], [ProductHashKey], [LoadTimestamp], [RecordSource], [AuditId])
SELECT
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), detailOffer.[SalesOrderDetailHashKey]))) + CONVERT(varbinary(max), detailOffer.[SalesOrderDetailHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), offerProduct.[ProductHashKey]))) + CONVERT(varbinary(max), offerProduct.[ProductHashKey]))),
    detailOffer.[SalesOrderDetailHashKey],
    offerProduct.[ProductHashKey],
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required')),
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.dbo.L_SalesOrderDetailSpecialOfferProduct+L_SpecialOfferProductProduct'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required'))
FROM [AdventureWorksRawVault].[dbo].[L_SalesOrderDetailSpecialOfferProduct] AS detailOffer
INNER JOIN [AdventureWorksRawVault].[dbo].[L_SpecialOfferProductProduct] AS offerProduct
    ON offerProduct.[SpecialOfferProductHashKey] = detailOffer.[SpecialOfferProductHashKey]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesOrderLine] AS salesOrderLine
    ON salesOrderLine.[HashKey] = detailOffer.[SalesOrderDetailHashKey]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Product] AS product
    ON product.[HashKey] = offerProduct.[ProductHashKey]
LEFT OUTER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineProduct] AS existing
    ON existing.[HashKey] = CONVERT(binary(32), HASHBYTES('SHA2_256', CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), detailOffer.[SalesOrderDetailHashKey]))) + CONVERT(varbinary(max), detailOffer.[SalesOrderDetailHashKey]) + CONVERT(binary(4), DATALENGTH(CONVERT(varbinary(max), offerProduct.[ProductHashKey]))) + CONVERT(varbinary(max), offerProduct.[ProductHashKey])))
WHERE existing.[HashKey] IS NULL