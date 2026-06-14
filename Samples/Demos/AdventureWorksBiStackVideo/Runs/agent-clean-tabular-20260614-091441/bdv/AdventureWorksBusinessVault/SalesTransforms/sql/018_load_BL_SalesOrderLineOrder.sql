CREATE VIEW dbo.v_load_BL_SalesOrderLineOrder
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(32), l.SalesOrderDetailHashKey, 2), N'|', CONVERT(nvarchar(32), l.SalesOrderHeaderHashKey, 2)))) AS HashKey,
    l.SalesOrderDetailHashKey AS SalesOrderLineHashKey,
    l.SalesOrderHeaderHashKey AS SalesOrderHashKey,
    l.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.L_SalesOrderDetailSalesOrderHeader') AS RecordSource,
    l.AuditId
FROM [AdventureWorksRawVault].[dbo].[L_SalesOrderDetailSalesOrderHeader] AS l;
GO
