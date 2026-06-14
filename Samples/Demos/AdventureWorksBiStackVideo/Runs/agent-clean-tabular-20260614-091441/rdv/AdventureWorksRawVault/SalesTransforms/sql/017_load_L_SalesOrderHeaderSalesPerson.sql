CREATE VIEW dbo.v_load_L_SalesOrderHeaderSalesPerson
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), soh.SalesOrderID), N'|', CONVERT(nvarchar(256), soh.SalesPersonID)))) AS HashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), soh.SalesOrderID))) AS SalesOrderHeaderHashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), soh.SalesPersonID))) AS SalesPersonHashKey,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesOrderHeader') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesOrderHeader] AS soh
WHERE soh.SalesPersonID IS NOT NULL;
GO
