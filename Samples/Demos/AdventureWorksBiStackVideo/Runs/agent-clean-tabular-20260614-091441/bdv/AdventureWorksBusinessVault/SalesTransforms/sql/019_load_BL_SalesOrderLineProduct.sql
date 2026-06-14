CREATE VIEW dbo.v_load_BL_SalesOrderLineProduct
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(32), lsp.SalesOrderDetailHashKey, 2), N'|', CONVERT(nvarchar(32), spp.ProductHashKey, 2)))) AS HashKey,
    lsp.SalesOrderDetailHashKey AS SalesOrderLineHashKey,
    spp.ProductHashKey,
    lsp.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.SalesOrderLineProduct') AS RecordSource,
    lsp.AuditId
FROM [AdventureWorksRawVault].[dbo].[L_SalesOrderDetailSpecialOfferProduct] AS lsp
INNER JOIN [AdventureWorksRawVault].[dbo].[L_SpecialOfferProductProduct] AS spp
    ON spp.SpecialOfferProductHashKey = lsp.SpecialOfferProductHashKey;
GO
