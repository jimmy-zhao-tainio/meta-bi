CREATE VIEW dbo.v_load_BL_SalesOrderCustomer
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(32), l.SalesOrderHeaderHashKey, 2), N'|', CONVERT(nvarchar(32), l.CustomerHashKey, 2)))) AS HashKey,
    l.SalesOrderHeaderHashKey AS SalesOrderHashKey,
    l.CustomerHashKey,
    l.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.L_SalesOrderHeaderCustomer') AS RecordSource,
    l.AuditId
FROM [AdventureWorksRawVault].[dbo].[L_SalesOrderHeaderCustomer] AS l;
GO
