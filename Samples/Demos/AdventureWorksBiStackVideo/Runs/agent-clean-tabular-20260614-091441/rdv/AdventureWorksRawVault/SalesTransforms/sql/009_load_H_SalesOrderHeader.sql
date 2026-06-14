CREATE VIEW dbo.v_load_H_SalesOrderHeader
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), soh.SalesOrderID))) AS HashKey,
    CONVERT(nvarchar(256), soh.SalesOrderID) AS SalesOrderID,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesOrderHeader') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesOrderHeader] AS soh;
GO
