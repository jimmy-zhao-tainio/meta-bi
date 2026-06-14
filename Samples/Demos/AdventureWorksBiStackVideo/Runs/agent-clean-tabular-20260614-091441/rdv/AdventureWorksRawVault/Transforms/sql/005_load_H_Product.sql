CREATE VIEW dbo.v_load_H_Product
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), p.ProductID))) AS HashKey,
    CONVERT(nvarchar(256), p.ProductID) AS ProductID,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Production.Product') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Production].[Product] AS p;
GO
