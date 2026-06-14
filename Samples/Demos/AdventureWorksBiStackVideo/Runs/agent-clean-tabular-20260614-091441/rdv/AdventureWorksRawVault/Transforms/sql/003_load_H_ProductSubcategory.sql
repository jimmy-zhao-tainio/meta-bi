CREATE VIEW dbo.v_load_H_ProductSubcategory
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), ps.ProductSubcategoryID))) AS HashKey,
    CONVERT(nvarchar(256), ps.ProductSubcategoryID) AS ProductSubcategoryID,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Production.ProductSubcategory') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Production].[ProductSubcategory] AS ps;
GO
