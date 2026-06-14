CREATE VIEW dbo.v_load_BL_SalesOrderSalesPerson
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(32), l.SalesOrderHeaderHashKey, 2), N'|', CONVERT(nvarchar(32), l.SalesPersonHashKey, 2)))) AS HashKey,
    l.SalesOrderHeaderHashKey AS SalesOrderHashKey,
    l.SalesPersonHashKey,
    l.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.L_SalesOrderHeaderSalesPerson') AS RecordSource,
    l.AuditId
FROM [AdventureWorksRawVault].[dbo].[L_SalesOrderHeaderSalesPerson] AS l;
GO
