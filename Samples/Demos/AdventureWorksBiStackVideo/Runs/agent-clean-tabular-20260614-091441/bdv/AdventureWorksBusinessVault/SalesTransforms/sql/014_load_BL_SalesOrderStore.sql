CREATE VIEW dbo.v_load_BL_SalesOrderStore
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(32), loc.SalesOrderHeaderHashKey, 2), N'|', CONVERT(nvarchar(32), lcs.StoreHashKey, 2)))) AS HashKey,
    loc.SalesOrderHeaderHashKey AS SalesOrderHashKey,
    lcs.StoreHashKey,
    loc.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.SalesOrderStore') AS RecordSource,
    loc.AuditId
FROM [AdventureWorksRawVault].[dbo].[L_SalesOrderHeaderCustomer] AS loc
INNER JOIN [AdventureWorksRawVault].[dbo].[L_CustomerStore] AS lcs
    ON lcs.CustomerHashKey = loc.CustomerHashKey;
GO
