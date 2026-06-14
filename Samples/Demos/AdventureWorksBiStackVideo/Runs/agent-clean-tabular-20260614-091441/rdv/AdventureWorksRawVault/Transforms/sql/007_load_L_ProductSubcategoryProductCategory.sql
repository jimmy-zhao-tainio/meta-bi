CREATE VIEW dbo.v_load_L_ProductSubcategoryProductCategory
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT_WS(N'|',
        CONVERT(nvarchar(256), ps.ProductSubcategoryID),
        CONVERT(nvarchar(256), ps.ProductCategoryID)))) AS HashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), ps.ProductSubcategoryID))) AS ProductSubcategoryHashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), ps.ProductCategoryID))) AS ProductCategoryHashKey,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Production.ProductSubcategory') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Production].[ProductSubcategory] AS ps;
GO
