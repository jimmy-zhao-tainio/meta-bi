CREATE VIEW dbo.v_load_BHS_Product_ProductClassification
AS
SELECT
    h.HashKey AS HubHashKey,
    CONVERT(nvarchar(25), hs.ProductNumber) AS ProductNumber,
    CONVERT(nvarchar(100), hs.Name) AS ProductName,
    CONVERT(nvarchar(100), COALESCE(pc.Name, N'Unclassified')) AS ProductCategory,
    CONVERT(nvarchar(100), COALESCE(ps.Name, N'Unclassified')) AS ProductSubcategory,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        hs.ProductNumber,
        hs.Name,
        COALESCE(pc.Name, N'Unclassified'),
        COALESCE(ps.Name, N'Unclassified')))) AS HashDiff,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.ProductClassification') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_Product] AS h
INNER JOIN [AdventureWorksRawVault].[dbo].[HS_Product_Product] AS hs
    ON hs.HubHashKey = h.HashKey
LEFT JOIN [AdventureWorksRawVault].[dbo].[L_ProductProductSubcategory] AS lps
    ON lps.ProductHashKey = h.HashKey
LEFT JOIN [AdventureWorksRawVault].[dbo].[H_ProductSubcategory] AS hps
    ON hps.HashKey = lps.ProductSubcategoryHashKey
LEFT JOIN [AdventureWorksRawVault].[dbo].[HS_ProductSubcategory_ProductSubcategory] AS ps
    ON ps.HubHashKey = hps.HashKey
LEFT JOIN [AdventureWorksRawVault].[dbo].[L_ProductSubcategoryProductCategory] AS lpc
    ON lpc.ProductSubcategoryHashKey = hps.HashKey
LEFT JOIN [AdventureWorksRawVault].[dbo].[H_ProductCategory] AS hpc
    ON hpc.HashKey = lpc.ProductCategoryHashKey
LEFT JOIN [AdventureWorksRawVault].[dbo].[HS_ProductCategory_ProductCategory] AS pc
    ON pc.HubHashKey = hpc.HashKey;
GO
