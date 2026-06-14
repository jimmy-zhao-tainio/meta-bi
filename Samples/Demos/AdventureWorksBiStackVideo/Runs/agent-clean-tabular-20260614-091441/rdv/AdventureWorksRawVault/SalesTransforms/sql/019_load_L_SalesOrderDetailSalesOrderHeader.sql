CREATE VIEW dbo.v_load_L_SalesOrderDetailSalesOrderHeader
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), sod.SalesOrderID), N'|', CONVERT(nvarchar(256), sod.SalesOrderDetailID), N'|', CONVERT(nvarchar(256), sod.SalesOrderID)))) AS HashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), sod.SalesOrderID), N'|', CONVERT(nvarchar(256), sod.SalesOrderDetailID)))) AS SalesOrderDetailHashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), sod.SalesOrderID))) AS SalesOrderHeaderHashKey,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesOrderDetail') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesOrderDetail] AS sod;
GO
