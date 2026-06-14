CREATE VIEW dbo.v_load_L_CustomerSalesTerritory
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), c.CustomerID), N'|', CONVERT(nvarchar(256), c.TerritoryID)))) AS HashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), c.CustomerID))) AS CustomerHashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), c.TerritoryID))) AS SalesTerritoryHashKey,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.Customer') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[Customer] AS c
WHERE c.TerritoryID IS NOT NULL;
GO
