CREATE VIEW dbo.v_load_BL_SalesOrderDate
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(32), h.HashKey, 2), N'|', CONVERT(nvarchar(32), CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(10), CONVERT(date, hs.OrderDate), 23))), 2)))) AS HashKey,
    h.HashKey AS SalesOrderHashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(10), CONVERT(date, hs.OrderDate), 23))) AS OrderDateHashKey,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.SalesOrderDate') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_SalesOrderHeader] AS h
INNER JOIN [AdventureWorksRawVault].[dbo].[HS_SalesOrderHeader_SalesOrderHeader] AS hs
    ON hs.HubHashKey = h.HashKey;
GO
