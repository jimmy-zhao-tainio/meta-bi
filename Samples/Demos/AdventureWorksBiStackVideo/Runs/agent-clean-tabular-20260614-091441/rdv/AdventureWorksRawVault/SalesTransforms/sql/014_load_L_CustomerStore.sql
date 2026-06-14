CREATE VIEW dbo.v_load_L_CustomerStore
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), c.CustomerID), N'|', CONVERT(nvarchar(256), c.StoreID)))) AS HashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), c.CustomerID))) AS CustomerHashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), c.StoreID))) AS StoreHashKey,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.Customer') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[Customer] AS c
WHERE c.StoreID IS NOT NULL;
GO
