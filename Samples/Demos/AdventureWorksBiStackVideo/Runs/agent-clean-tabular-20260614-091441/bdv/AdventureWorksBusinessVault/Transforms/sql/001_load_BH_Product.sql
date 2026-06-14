CREATE VIEW dbo.v_load_BH_Product
AS
SELECT
    h.HashKey,
    h.ProductID AS ProductId,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.H_Product') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_Product] AS h;
GO
