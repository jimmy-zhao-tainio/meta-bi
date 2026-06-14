CREATE VIEW dbo.v_load_H_ProductCategory
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), pc.ProductCategoryID))) AS HashKey,
    CONVERT(nvarchar(256), pc.ProductCategoryID) AS ProductCategoryID,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Production.ProductCategory') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Production].[ProductCategory] AS pc;
GO
