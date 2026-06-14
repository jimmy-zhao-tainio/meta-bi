CREATE VIEW dbo.v_load_H_Customer
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), c.CustomerID))) AS HashKey,
    CONVERT(nvarchar(256), c.CustomerID) AS CustomerID,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.Customer') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[Customer] AS c;
GO
