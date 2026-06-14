CREATE OR ALTER VIEW awbi.v_load_DimProduct
AS
SELECT
    hp.ProductId,
    hpc.ProductNumber,
    hpc.ProductName,
    hpc.ProductCategory,
    hpc.ProductSubcategory
FROM [AdventureWorksBusinessVault].[dbo].[BH_Product] AS hp
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BHS_Product_ProductClassification] AS hpc
    ON hpc.HubHashKey = hp.HashKey;
