CREATE VIEW dbo.v_load_L_ProductProductSubcategory
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT_WS(N'|',
        CONVERT(nvarchar(256), p.ProductID),
        CONVERT(nvarchar(256), p.ProductSubcategoryID)))) AS HashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), p.ProductID))) AS ProductHashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), p.ProductSubcategoryID))) AS ProductSubcategoryHashKey,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Production.Product') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Production].[Product] AS p
WHERE p.ProductSubcategoryID IS NOT NULL;
GO
