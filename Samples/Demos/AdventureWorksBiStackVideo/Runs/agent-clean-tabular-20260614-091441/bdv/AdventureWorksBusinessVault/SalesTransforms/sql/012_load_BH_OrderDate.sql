CREATE VIEW dbo.v_load_BH_OrderDate
AS
SELECT DISTINCT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(10), CONVERT(date, hs.OrderDate), 23))) AS HashKey,
    CONVERT(date, hs.OrderDate) AS OrderDate,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.SalesOrderHeader.OrderDate') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorksRawVault].[dbo].[HS_SalesOrderHeader_SalesOrderHeader] AS hs;
GO
