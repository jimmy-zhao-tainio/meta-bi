CREATE VIEW dbo.v_load_H_SalesOrderDetail
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), sod.SalesOrderID), N'|', CONVERT(nvarchar(256), sod.SalesOrderDetailID)))) AS HashKey,
    CONVERT(nvarchar(256), sod.SalesOrderID) AS SalesOrderID,
    CONVERT(nvarchar(256), sod.SalesOrderDetailID) AS SalesOrderDetailID,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesOrderDetail') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesOrderDetail] AS sod;
GO
