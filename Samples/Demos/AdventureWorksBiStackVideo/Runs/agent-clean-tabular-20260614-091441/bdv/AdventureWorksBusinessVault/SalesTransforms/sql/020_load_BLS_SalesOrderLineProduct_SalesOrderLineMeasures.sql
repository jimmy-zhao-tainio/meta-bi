CREATE VIEW dbo.v_load_BLS_SalesOrderLineProduct_SalesOrderLineMeasures
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(32), lsp.SalesOrderDetailHashKey, 2), N'|', CONVERT(nvarchar(32), spp.ProductHashKey, 2)))) AS LinkHashKey,
    CONVERT(int, hsod.OrderQty) AS OrderQuantity,
    CONVERT(decimal(19, 4), hsod.UnitPrice) AS UnitPrice,
    CONVERT(decimal(38, 6), hsod.LineTotal) AS LineTotal,
    CONVERT(decimal(19, 4), hsod.UnitPrice * hsod.OrderQty * hsod.UnitPriceDiscount) AS DiscountAmount,
    CONVERT(decimal(19, 4), CASE WHEN hsoh.SubTotal = 0 THEN 0 ELSE hsoh.TaxAmt * (hsod.LineTotal / hsoh.SubTotal) END) AS TaxAmount,
    CONVERT(decimal(19, 4), CASE WHEN hsoh.SubTotal = 0 THEN 0 ELSE hsoh.Freight * (hsod.LineTotal / hsoh.SubTotal) END) AS FreightAmount,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        CONVERT(nvarchar(20), hsod.OrderQty),
        CONVERT(nvarchar(40), hsod.UnitPrice),
        CONVERT(nvarchar(40), hsod.LineTotal),
        CONVERT(nvarchar(40), hsod.UnitPrice * hsod.OrderQty * hsod.UnitPriceDiscount),
        CONVERT(nvarchar(40), CASE WHEN hsoh.SubTotal = 0 THEN 0 ELSE hsoh.TaxAmt * (hsod.LineTotal / hsoh.SubTotal) END),
        CONVERT(nvarchar(40), CASE WHEN hsoh.SubTotal = 0 THEN 0 ELSE hsoh.Freight * (hsod.LineTotal / hsoh.SubTotal) END)))) AS HashDiff,
    lsp.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.SalesOrderLineMeasures') AS RecordSource,
    lsp.AuditId
FROM [AdventureWorksRawVault].[dbo].[L_SalesOrderDetailSpecialOfferProduct] AS lsp
INNER JOIN [AdventureWorksRawVault].[dbo].[L_SpecialOfferProductProduct] AS spp
    ON spp.SpecialOfferProductHashKey = lsp.SpecialOfferProductHashKey
INNER JOIN [AdventureWorksRawVault].[dbo].[HS_SalesOrderDetail_SalesOrderDetail] AS hsod
    ON hsod.HubHashKey = lsp.SalesOrderDetailHashKey
INNER JOIN [AdventureWorksRawVault].[dbo].[L_SalesOrderDetailSalesOrderHeader] AS lso
    ON lso.SalesOrderDetailHashKey = lsp.SalesOrderDetailHashKey
INNER JOIN [AdventureWorksRawVault].[dbo].[HS_SalesOrderHeader_SalesOrderHeader] AS hsoh
    ON hsoh.HubHashKey = lso.SalesOrderHeaderHashKey;
GO
