CREATE VIEW dbo.v_load_BL_SalesOrderTerritory
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(32), l.SalesOrderHeaderHashKey, 2), N'|', CONVERT(nvarchar(32), l.SalesTerritoryHashKey, 2)))) AS HashKey,
    l.SalesOrderHeaderHashKey AS SalesOrderHashKey,
    l.SalesTerritoryHashKey,
    l.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.L_SalesOrderHeaderSalesTerritory') AS RecordSource,
    l.AuditId
FROM [AdventureWorksRawVault].[dbo].[L_SalesOrderHeaderSalesTerritory] AS l;
GO
