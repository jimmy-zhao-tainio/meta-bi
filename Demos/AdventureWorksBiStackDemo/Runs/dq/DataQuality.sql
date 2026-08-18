IF SCHEMA_ID(N'dq') IS NULL EXEC(N'CREATE SCHEMA [dq]');
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.1.135]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_CustomerPerson] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[CustomerHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.10.113]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince -> AdventureWorksBusinessVault.dbo.BL_StateProvinceCountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'StateProvinceHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[StateProvinceHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_AddressStateProvince] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_StateProvinceCountryRegion] AS [dq_right]
    ON [dq_left].[StateProvinceHashKey] = [dq_right].[StateProvinceHashKey]
GROUP BY [dq_left].[StateProvinceHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.12.149]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_Product] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_ProductSubcategoryAssignment] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[ProductHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.14.143]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryCategory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'ProductSubcategoryHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[ProductSubcategoryHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_ProductSubcategoryAssignment] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_ProductSubcategoryCategory] AS [dq_right]
    ON [dq_left].[ProductSubcategoryHashKey] = [dq_right].[ProductSubcategoryHashKey]
GROUP BY [dq_left].[ProductSubcategoryHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.16.157]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Salesperson' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesPersonPerson] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesPersonHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.19.163]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksBusinessVault.dbo.BL_SalesTerritoryCountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_SalesTerritory' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesTerritoryCountryRegion] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesTerritoryHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.2.131]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerPerson -> AdventureWorksBusinessVault.dbo.BHS_Person_PersonProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'PersonHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[PersonHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_CustomerPerson] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BHS_Person_PersonProfile] AS [dq_right]
    ON [dq_left].[PersonHashKey] = [dq_right].[HubHashKey]
GROUP BY [dq_left].[PersonHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.21.56]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BHS_SalesOrderLine_SalesOrderLineDetail' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrderLine] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BHS_SalesOrderLine_SalesOrderLineDetail] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[HubHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.22.53]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BL_SalesOrderLineSalesOrder' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrderLine] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineSalesOrder] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesOrderLineHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.23.50]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineSalesOrder -> AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineSalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesOrderHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[SalesOrderHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineSalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_right]
    ON [dq_left].[SalesOrderHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[SalesOrderHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.24.38]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BHS_SalesOrder_SalesOrderHeader' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BHS_SalesOrder_SalesOrderHeader] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[HubHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.25.35]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BL_SalesOrderLineProduct' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrderLine] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineProduct] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesOrderLineHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.26.32]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineProduct -> AdventureWorksBusinessVault.dbo.BH_Product' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineProduct' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'ProductHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[ProductHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineProduct] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Product] AS [dq_right]
    ON [dq_left].[ProductHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[ProductHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.28.27]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderCustomer] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.29.24]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer -> AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'CustomerHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[CustomerHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderCustomer] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS [dq_right]
    ON [dq_left].[CustomerHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[CustomerHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.3.127]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerStore' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_CustomerStore] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[CustomerHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.31.85]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesPerson] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.32.81]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesPersonHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[SalesPersonHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesPerson] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_right]
    ON [dq_left].[SalesPersonHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[SalesPersonHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.34.75]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesTerritory] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.35.71]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesTerritoryHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[SalesTerritoryHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesTerritory] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS [dq_right]
    ON [dq_left].[SalesTerritoryHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[SalesTerritoryHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.37.13]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderShipToAddress] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.38.10]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'ShipToAddressHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[ShipToAddressHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderShipToAddress] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Address] AS [dq_right]
    ON [dq_left].[ShipToAddressHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[ShipToAddressHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.42.47]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BHS_SalesOrder_SalesOrderHeader' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BHS_SalesOrder_SalesOrderHeader] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[HubHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.43.44]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderCustomer] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.44.41]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer -> AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'CustomerHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[CustomerHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderCustomer] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS [dq_right]
    ON [dq_left].[CustomerHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[CustomerHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.46.105]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesPerson] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.47.101]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesPersonHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[SalesPersonHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesPerson] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_right]
    ON [dq_left].[SalesPersonHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[SalesPersonHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.49.95]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesTerritory] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.50.91]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesTerritoryHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[SalesTerritoryHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesTerritory] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS [dq_right]
    ON [dq_left].[SalesTerritoryHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[SalesTerritoryHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.52.20]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderBillToAddress' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderBillToAddress] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.53.17]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderBillToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderBillToAddress' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'BillToAddressHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[BillToAddressHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderBillToAddress] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Address] AS [dq_right]
    ON [dq_left].[BillToAddressHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[BillToAddressHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.55.6]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderShipToAddress] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.56.3]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'ShipToAddressHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[ShipToAddressHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderShipToAddress] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Address] AS [dq_right]
    ON [dq_left].[ShipToAddressHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[ShipToAddressHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.63.66]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPersonQuota -> AdventureWorksBusinessVault.dbo.BHS_SalesPersonQuota_SalesPersonQuotaProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalespersonQuota' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPersonQuota' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPersonQuota] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BHS_SalesPersonQuota_SalesPersonQuotaProfile] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[HubHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.64.63]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPersonQuota -> AdventureWorksBusinessVault.dbo.BL_SalesPersonQuotaSalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalespersonQuota' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPersonQuota' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPersonQuota] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesPersonQuotaSalesPerson] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesPersonQuotaHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.65.60]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesPersonQuotaSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalespersonQuota' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesPersonQuotaSalesPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesPersonHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[SalesPersonHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesPersonQuotaSalesPerson] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_right]
    ON [dq_left].[SalesPersonHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[SalesPersonHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion.JoinPattern.8.119]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksBusinessVault.dbo.BL_AddressStateProvince' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_Address] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_AddressStateProvince] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[AddressHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.1.134]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'CustomerHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[CustomerHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_CustomerPerson] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[CustomerHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.10.112]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince -> AdventureWorksBusinessVault.dbo.BL_StateProvinceCountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_StateProvinceCountryRegion' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'StateProvinceHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[StateProvinceHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_StateProvinceCountryRegion] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_AddressStateProvince] AS [dq_left]
    WHERE [dq_left].[StateProvinceHashKey] = [dq_right].[StateProvinceHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.11.152]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product -> AdventureWorksBusinessVault.dbo.BHS_Product_ProductProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BHS_Product_ProductProfile' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HubHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HubHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BHS_Product_ProductProfile] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_Product] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.12.148]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'ProductHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[ProductHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_ProductSubcategoryAssignment] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_Product] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[ProductHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.13.146]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment -> AdventureWorksBusinessVault.dbo.BHS_ProductSubcategory_ProductSubcategoryProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BHS_ProductSubcategory_ProductSubcategoryProfile' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HubHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HubHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BHS_ProductSubcategory_ProductSubcategoryProfile] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_ProductSubcategoryAssignment] AS [dq_left]
    WHERE [dq_left].[ProductSubcategoryHashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.138]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BHS_Customer_CustomerProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BHS_Customer_CustomerProfile' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HubHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HubHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BHS_Customer_CustomerProfile] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.14.142]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryCategory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryCategory' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'ProductSubcategoryHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[ProductSubcategoryHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_ProductSubcategoryCategory] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_ProductSubcategoryAssignment] AS [dq_left]
    WHERE [dq_left].[ProductSubcategoryHashKey] = [dq_right].[ProductSubcategoryHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.15.140]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryCategory -> AdventureWorksBusinessVault.dbo.BHS_ProductCategory_ProductCategoryProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BHS_ProductCategory_ProductCategoryProfile' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HubHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HubHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BHS_ProductCategory_ProductCategoryProfile] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_ProductSubcategoryCategory] AS [dq_left]
    WHERE [dq_left].[ProductCategoryHashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.16.156]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Salesperson' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesPersonHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[SalesPersonHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesPersonPerson] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[SalesPersonHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.17.154]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson -> AdventureWorksBusinessVault.dbo.BHS_Person_PersonProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Salesperson' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BHS_Person_PersonProfile' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HubHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HubHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BHS_Person_PersonProfile] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesPersonPerson] AS [dq_left]
    WHERE [dq_left].[PersonHashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.18.166]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksBusinessVault.dbo.BHS_SalesTerritory_SalesTerritoryProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_SalesTerritory' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BHS_SalesTerritory_SalesTerritoryProfile' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HubHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HubHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BHS_SalesTerritory_SalesTerritoryProfile] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.19.162]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksBusinessVault.dbo.BL_SalesTerritoryCountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_SalesTerritory' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesTerritoryCountryRegion' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesTerritoryHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[SalesTerritoryHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesTerritoryCountryRegion] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[SalesTerritoryHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.2.130]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerPerson -> AdventureWorksBusinessVault.dbo.BHS_Person_PersonProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BHS_Person_PersonProfile' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HubHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HubHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BHS_Person_PersonProfile] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_CustomerPerson] AS [dq_left]
    WHERE [dq_left].[PersonHashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.20.160]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesTerritoryCountryRegion -> AdventureWorksBusinessVault.dbo.BH_CountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_SalesTerritory' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_CountryRegion' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_CountryRegion] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesTerritoryCountryRegion] AS [dq_left]
    WHERE [dq_left].[CountryRegionHashKey] = [dq_right].[HashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.21.55]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BHS_SalesOrderLine_SalesOrderLineDetail' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BHS_SalesOrderLine_SalesOrderLineDetail' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HubHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HubHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BHS_SalesOrderLine_SalesOrderLineDetail] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrderLine] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.22.52]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BL_SalesOrderLineSalesOrder' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineSalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesOrderLineHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[SalesOrderLineHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineSalesOrder] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrderLine] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[SalesOrderLineHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.23.49]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineSalesOrder -> AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineSalesOrder] AS [dq_left]
    WHERE [dq_left].[SalesOrderHashKey] = [dq_right].[HashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.24.37]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BHS_SalesOrder_SalesOrderHeader' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BHS_SalesOrder_SalesOrderHeader' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HubHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HubHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BHS_SalesOrder_SalesOrderHeader] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.25.34]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BL_SalesOrderLineProduct' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineProduct' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesOrderLineHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[SalesOrderLineHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineProduct] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrderLine] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[SalesOrderLineHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.26.31]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineProduct -> AdventureWorksBusinessVault.dbo.BH_Product' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_Product] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineProduct] AS [dq_left]
    WHERE [dq_left].[ProductHashKey] = [dq_right].[HashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.27.30]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product -> AdventureWorksAnalytics.dw.Dim_Product' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksAnalytics.dw.Dim_Product' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'ProductID=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[ProductID]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksAnalytics].[dw].[Dim_Product] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_Product] AS [dq_left]
    WHERE [dq_left].[ProductID] = [dq_right].[ProductID]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.28.26]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesOrderHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[SalesOrderHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderCustomer] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.29.23]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer -> AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderCustomer] AS [dq_left]
    WHERE [dq_left].[CustomerHashKey] = [dq_right].[HashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.3.126]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerStore' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerStore' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'CustomerHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[CustomerHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_CustomerStore] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[CustomerHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.30.22]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksAnalytics.dw.Dim_Customer' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksAnalytics.dw.Dim_Customer' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'CustomerID=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[CustomerID]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksAnalytics].[dw].[Dim_Customer] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS [dq_left]
    WHERE [dq_left].[CustomerID] = [dq_right].[CustomerID]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.31.84]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesOrderHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[SalesOrderHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesPerson] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.32.80]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesPerson] AS [dq_left]
    WHERE [dq_left].[SalesPersonHashKey] = [dq_right].[HashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.33.78]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksAnalytics.dw.Dim_Salesperson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksAnalytics.dw.Dim_Salesperson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'BusinessEntityID=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[BusinessEntityID]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksAnalytics].[dw].[Dim_Salesperson] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_left]
    WHERE [dq_left].[BusinessEntityID] = [dq_right].[BusinessEntityID]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.34.74]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesOrderHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[SalesOrderHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesTerritory] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.35.70]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesTerritory] AS [dq_left]
    WHERE [dq_left].[SalesTerritoryHashKey] = [dq_right].[HashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.36.68]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksAnalytics.dw.Dim_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksAnalytics.dw.Dim_SalesTerritory' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'TerritoryID=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[TerritoryID]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksAnalytics].[dw].[Dim_SalesTerritory] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS [dq_left]
    WHERE [dq_left].[TerritoryID] = [dq_right].[TerritoryID]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.37.12]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesOrderHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[SalesOrderHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderShipToAddress] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.38.9]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_Address] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderShipToAddress] AS [dq_left]
    WHERE [dq_left].[ShipToAddressHashKey] = [dq_right].[HashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.39.8]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksAnalytics.dw.Dim_Geography' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksAnalytics.dw.Dim_Geography' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'AddressID=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[AddressID]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksAnalytics].[dw].[Dim_Geography] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_Address] AS [dq_left]
    WHERE [dq_left].[AddressID] = [dq_right].[AddressID]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.4.124]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerStore -> AdventureWorksBusinessVault.dbo.BHS_Store_StoreProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BHS_Store_StoreProfile' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HubHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HubHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BHS_Store_StoreProfile] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_CustomerStore] AS [dq_left]
    WHERE [dq_left].[StoreHashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.42.46]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BHS_SalesOrder_SalesOrderHeader' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BHS_SalesOrder_SalesOrderHeader' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HubHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HubHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BHS_SalesOrder_SalesOrderHeader] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.43.43]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesOrderHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[SalesOrderHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderCustomer] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.44.40]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer -> AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderCustomer] AS [dq_left]
    WHERE [dq_left].[CustomerHashKey] = [dq_right].[HashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.45.29]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksAnalytics.dw.Dim_Customer' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksAnalytics.dw.Dim_Customer' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'CustomerID=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[CustomerID]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksAnalytics].[dw].[Dim_Customer] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS [dq_left]
    WHERE [dq_left].[CustomerID] = [dq_right].[CustomerID]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.46.104]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesOrderHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[SalesOrderHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesPerson] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.47.100]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesPerson] AS [dq_left]
    WHERE [dq_left].[SalesPersonHashKey] = [dq_right].[HashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.48.98]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksAnalytics.dw.Dim_Salesperson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksAnalytics.dw.Dim_Salesperson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'BusinessEntityID=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[BusinessEntityID]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksAnalytics].[dw].[Dim_Salesperson] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_left]
    WHERE [dq_left].[BusinessEntityID] = [dq_right].[BusinessEntityID]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.49.94]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesOrderHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[SalesOrderHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesTerritory] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.5.110]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_StateProvinceCountryRegion -> AdventureWorksBusinessVault.dbo.BH_CountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_CountryRegion' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_CountryRegion] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_StateProvinceCountryRegion] AS [dq_left]
    WHERE [dq_left].[CountryRegionHashKey] = [dq_right].[HashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.50.90]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesTerritory] AS [dq_left]
    WHERE [dq_left].[SalesTerritoryHashKey] = [dq_right].[HashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.51.88]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksAnalytics.dw.Dim_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksAnalytics.dw.Dim_SalesTerritory' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'TerritoryID=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[TerritoryID]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksAnalytics].[dw].[Dim_SalesTerritory] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS [dq_left]
    WHERE [dq_left].[TerritoryID] = [dq_right].[TerritoryID]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.52.19]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderBillToAddress' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderBillToAddress' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesOrderHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[SalesOrderHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderBillToAddress] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.53.16]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderBillToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_Address] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderBillToAddress] AS [dq_left]
    WHERE [dq_left].[BillToAddressHashKey] = [dq_right].[HashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.54.15]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksAnalytics.dw.Dim_Geography' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksAnalytics.dw.Dim_Geography' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'AddressID=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[AddressID]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksAnalytics].[dw].[Dim_Geography] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_Address] AS [dq_left]
    WHERE [dq_left].[AddressID] = [dq_right].[AddressID]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.55.5]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesOrderHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[SalesOrderHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderShipToAddress] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.56.2]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_Address] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderShipToAddress] AS [dq_left]
    WHERE [dq_left].[ShipToAddressHashKey] = [dq_right].[HashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.57.1]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksAnalytics.dw.Dim_Geography' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksAnalytics.dw.Dim_Geography' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'AddressID=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[AddressID]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksAnalytics].[dw].[Dim_Geography] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_Address] AS [dq_left]
    WHERE [dq_left].[AddressID] = [dq_right].[AddressID]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.6.108]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_CountryRegion -> AdventureWorksBusinessVault.dbo.BHS_CountryRegion_CountryRegionProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BHS_CountryRegion_CountryRegionProfile' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HubHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HubHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BHS_CountryRegion_CountryRegionProfile] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_CountryRegion] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.63.65]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPersonQuota -> AdventureWorksBusinessVault.dbo.BHS_SalesPersonQuota_SalesPersonQuotaProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalespersonQuota' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BHS_SalesPersonQuota_SalesPersonQuotaProfile' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HubHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HubHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BHS_SalesPersonQuota_SalesPersonQuotaProfile] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPersonQuota] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.64.62]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPersonQuota -> AdventureWorksBusinessVault.dbo.BL_SalesPersonQuotaSalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalespersonQuota' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesPersonQuotaSalesPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesPersonQuotaHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[SalesPersonQuotaHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesPersonQuotaSalesPerson] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPersonQuota] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[SalesPersonQuotaHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.65.59]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesPersonQuotaSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalespersonQuota' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesPersonQuotaSalesPerson] AS [dq_left]
    WHERE [dq_left].[SalesPersonHashKey] = [dq_right].[HashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.66.58]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksAnalytics.dw.Dim_Salesperson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalespersonQuota' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksAnalytics.dw.Dim_Salesperson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'BusinessEntityID=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[BusinessEntityID]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksAnalytics].[dw].[Dim_Salesperson] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_left]
    WHERE [dq_left].[BusinessEntityID] = [dq_right].[BusinessEntityID]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.7.122]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksBusinessVault.dbo.BHS_Address_AddressProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BHS_Address_AddressProfile' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HubHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HubHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BHS_Address_AddressProfile] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_Address] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.8.118]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksBusinessVault.dbo.BL_AddressStateProvince' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'AddressHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[AddressHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_AddressStateProvince] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_Address] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[AddressHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.9.116]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince -> AdventureWorksBusinessVault.dbo.BHS_StateProvince_StateProvinceProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BHS_StateProvince_StateProvinceProfile' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HubHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HubHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BHS_StateProvince_StateProvinceProfile] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_AddressStateProvince] AS [dq_left]
    WHERE [dq_left].[StateProvinceHashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.1.136]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_CustomerPerson] AS [dq_right]
    WHERE [dq_left].[HashKey] = [dq_right].[CustomerHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.10.114]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince -> AdventureWorksBusinessVault.dbo.BL_StateProvinceCountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'StateProvinceHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[StateProvinceHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_AddressStateProvince] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_StateProvinceCountryRegion] AS [dq_right]
    WHERE [dq_left].[StateProvinceHashKey] = [dq_right].[StateProvinceHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.11.153]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product -> AdventureWorksBusinessVault.dbo.BHS_Product_ProductProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_Product] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_Product_ProductProfile] AS [dq_right]
    WHERE [dq_left].[HashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.12.150]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_Product] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_ProductSubcategoryAssignment] AS [dq_right]
    WHERE [dq_left].[HashKey] = [dq_right].[ProductHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.13.147]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment -> AdventureWorksBusinessVault.dbo.BHS_ProductSubcategory_ProductSubcategoryProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'ProductSubcategoryHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[ProductSubcategoryHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_ProductSubcategoryAssignment] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_ProductSubcategory_ProductSubcategoryProfile] AS [dq_right]
    WHERE [dq_left].[ProductSubcategoryHashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.139]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BHS_Customer_CustomerProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_Customer_CustomerProfile] AS [dq_right]
    WHERE [dq_left].[HashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.14.144]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryCategory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'ProductSubcategoryHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[ProductSubcategoryHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_ProductSubcategoryAssignment] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_ProductSubcategoryCategory] AS [dq_right]
    WHERE [dq_left].[ProductSubcategoryHashKey] = [dq_right].[ProductSubcategoryHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.15.141]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryCategory -> AdventureWorksBusinessVault.dbo.BHS_ProductCategory_ProductCategoryProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryCategory' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'ProductCategoryHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[ProductCategoryHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_ProductSubcategoryCategory] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_ProductCategory_ProductCategoryProfile] AS [dq_right]
    WHERE [dq_left].[ProductCategoryHashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.16.158]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Salesperson' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesPersonPerson] AS [dq_right]
    WHERE [dq_left].[HashKey] = [dq_right].[SalesPersonHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.17.155]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson -> AdventureWorksBusinessVault.dbo.BHS_Person_PersonProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Salesperson' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'PersonHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[PersonHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesPersonPerson] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_Person_PersonProfile] AS [dq_right]
    WHERE [dq_left].[PersonHashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.18.167]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksBusinessVault.dbo.BHS_SalesTerritory_SalesTerritoryProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_SalesTerritory' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_SalesTerritory_SalesTerritoryProfile] AS [dq_right]
    WHERE [dq_left].[HashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.19.164]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksBusinessVault.dbo.BL_SalesTerritoryCountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_SalesTerritory' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesTerritoryCountryRegion] AS [dq_right]
    WHERE [dq_left].[HashKey] = [dq_right].[SalesTerritoryHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.2.132]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerPerson -> AdventureWorksBusinessVault.dbo.BHS_Person_PersonProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'PersonHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[PersonHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_CustomerPerson] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_Person_PersonProfile] AS [dq_right]
    WHERE [dq_left].[PersonHashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.20.161]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesTerritoryCountryRegion -> AdventureWorksBusinessVault.dbo.BH_CountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_SalesTerritory' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesTerritoryCountryRegion' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'CountryRegionHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[CountryRegionHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesTerritoryCountryRegion] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_CountryRegion] AS [dq_right]
    WHERE [dq_left].[CountryRegionHashKey] = [dq_right].[HashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.3.128]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerStore' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_CustomerStore] AS [dq_right]
    WHERE [dq_left].[HashKey] = [dq_right].[CustomerHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.31.86]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesPerson] AS [dq_right]
    WHERE [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.32.82]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesPersonHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[SalesPersonHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesPerson] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_right]
    WHERE [dq_left].[SalesPersonHashKey] = [dq_right].[HashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.33.79]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksAnalytics.dw.Dim_Salesperson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'BusinessEntityID=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[BusinessEntityID]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksAnalytics].[dw].[Dim_Salesperson] AS [dq_right]
    WHERE [dq_left].[BusinessEntityID] = [dq_right].[BusinessEntityID]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.34.76]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesTerritory] AS [dq_right]
    WHERE [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.35.72]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesTerritoryHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[SalesTerritoryHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesTerritory] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS [dq_right]
    WHERE [dq_left].[SalesTerritoryHashKey] = [dq_right].[HashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.36.69]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksAnalytics.dw.Dim_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'TerritoryID=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[TerritoryID]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksAnalytics].[dw].[Dim_SalesTerritory] AS [dq_right]
    WHERE [dq_left].[TerritoryID] = [dq_right].[TerritoryID]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.4.125]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerStore -> AdventureWorksBusinessVault.dbo.BHS_Store_StoreProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerStore' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'StoreHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[StoreHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_CustomerStore] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_Store_StoreProfile] AS [dq_right]
    WHERE [dq_left].[StoreHashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.46.106]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesPerson] AS [dq_right]
    WHERE [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.47.102]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesPersonHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[SalesPersonHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesPerson] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_right]
    WHERE [dq_left].[SalesPersonHashKey] = [dq_right].[HashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.48.99]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksAnalytics.dw.Dim_Salesperson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'BusinessEntityID=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[BusinessEntityID]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksAnalytics].[dw].[Dim_Salesperson] AS [dq_right]
    WHERE [dq_left].[BusinessEntityID] = [dq_right].[BusinessEntityID]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.49.96]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesTerritory] AS [dq_right]
    WHERE [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.5.111]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_StateProvinceCountryRegion -> AdventureWorksBusinessVault.dbo.BH_CountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_StateProvinceCountryRegion' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'CountryRegionHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[CountryRegionHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_StateProvinceCountryRegion] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_CountryRegion] AS [dq_right]
    WHERE [dq_left].[CountryRegionHashKey] = [dq_right].[HashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.50.92]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesTerritoryHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[SalesTerritoryHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesTerritory] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS [dq_right]
    WHERE [dq_left].[SalesTerritoryHashKey] = [dq_right].[HashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.51.89]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksAnalytics.dw.Dim_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'TerritoryID=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[TerritoryID]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksAnalytics].[dw].[Dim_SalesTerritory] AS [dq_right]
    WHERE [dq_left].[TerritoryID] = [dq_right].[TerritoryID]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.6.109]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_CountryRegion -> AdventureWorksBusinessVault.dbo.BHS_CountryRegion_CountryRegionProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_CountryRegion' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_CountryRegion] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_CountryRegion_CountryRegionProfile] AS [dq_right]
    WHERE [dq_left].[HashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.7.123]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksBusinessVault.dbo.BHS_Address_AddressProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_Address] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_Address_AddressProfile] AS [dq_right]
    WHERE [dq_left].[HashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.8.120]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksBusinessVault.dbo.BL_AddressStateProvince' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_Address] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_AddressStateProvince] AS [dq_right]
    WHERE [dq_left].[HashKey] = [dq_right].[AddressHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion.JoinPattern.9.117]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince -> AdventureWorksBusinessVault.dbo.BHS_StateProvince_StateProvinceProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'StateProvinceHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[StateProvinceHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_AddressStateProvince] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_StateProvince_StateProvinceProfile] AS [dq_right]
    WHERE [dq_left].[StateProvinceHashKey] = [dq_right].[HubHashKey]
)
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.1.137]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_CustomerPerson] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[CustomerHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.10.115]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince -> AdventureWorksBusinessVault.dbo.BL_StateProvinceCountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'StateProvinceHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[StateProvinceHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_AddressStateProvince] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_StateProvinceCountryRegion] AS [dq_right]
    ON [dq_left].[StateProvinceHashKey] = [dq_right].[StateProvinceHashKey]
GROUP BY [dq_left].[StateProvinceHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.12.151]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_Product] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_ProductSubcategoryAssignment] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[ProductHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.14.145]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryCategory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'ProductSubcategoryHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[ProductSubcategoryHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_ProductSubcategoryAssignment] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_ProductSubcategoryCategory] AS [dq_right]
    ON [dq_left].[ProductSubcategoryHashKey] = [dq_right].[ProductSubcategoryHashKey]
GROUP BY [dq_left].[ProductSubcategoryHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.16.159]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Salesperson' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesPersonPerson] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesPersonHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.19.165]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksBusinessVault.dbo.BL_SalesTerritoryCountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_SalesTerritory' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesTerritoryCountryRegion] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesTerritoryHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.2.133]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerPerson -> AdventureWorksBusinessVault.dbo.BHS_Person_PersonProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'PersonHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[PersonHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_CustomerPerson] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BHS_Person_PersonProfile] AS [dq_right]
    ON [dq_left].[PersonHashKey] = [dq_right].[HubHashKey]
GROUP BY [dq_left].[PersonHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.21.57]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BHS_SalesOrderLine_SalesOrderLineDetail' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrderLine] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BHS_SalesOrderLine_SalesOrderLineDetail] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[HubHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.22.54]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BL_SalesOrderLineSalesOrder' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrderLine] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineSalesOrder] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesOrderLineHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.23.51]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineSalesOrder -> AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineSalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesOrderHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[SalesOrderHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineSalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_right]
    ON [dq_left].[SalesOrderHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[SalesOrderHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.24.39]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BHS_SalesOrder_SalesOrderHeader' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BHS_SalesOrder_SalesOrderHeader] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[HubHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.25.36]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BL_SalesOrderLineProduct' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrderLine] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineProduct] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesOrderLineHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.26.33]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineProduct -> AdventureWorksBusinessVault.dbo.BH_Product' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineProduct' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'ProductHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[ProductHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineProduct] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Product] AS [dq_right]
    ON [dq_left].[ProductHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[ProductHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.28.28]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderCustomer] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.29.25]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer -> AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'CustomerHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[CustomerHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderCustomer] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS [dq_right]
    ON [dq_left].[CustomerHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[CustomerHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.3.129]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerStore' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_CustomerStore] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[CustomerHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.31.87]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesPerson] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.32.83]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesPersonHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[SalesPersonHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesPerson] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_right]
    ON [dq_left].[SalesPersonHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[SalesPersonHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.34.77]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesTerritory] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.35.73]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesTerritoryHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[SalesTerritoryHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesTerritory] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS [dq_right]
    ON [dq_left].[SalesTerritoryHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[SalesTerritoryHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.37.14]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderShipToAddress] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.38.11]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'ShipToAddressHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[ShipToAddressHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderShipToAddress] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Address] AS [dq_right]
    ON [dq_left].[ShipToAddressHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[ShipToAddressHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.42.48]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BHS_SalesOrder_SalesOrderHeader' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BHS_SalesOrder_SalesOrderHeader] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[HubHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.43.45]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderCustomer] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.44.42]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer -> AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'CustomerHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[CustomerHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderCustomer] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS [dq_right]
    ON [dq_left].[CustomerHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[CustomerHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.46.107]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesPerson] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.47.103]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesPersonHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[SalesPersonHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesPerson] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_right]
    ON [dq_left].[SalesPersonHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[SalesPersonHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.49.97]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesTerritory] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.50.93]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesTerritoryHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[SalesTerritoryHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesTerritory] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS [dq_right]
    ON [dq_left].[SalesTerritoryHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[SalesTerritoryHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.52.21]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderBillToAddress' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderBillToAddress] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.53.18]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderBillToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderBillToAddress' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'BillToAddressHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[BillToAddressHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderBillToAddress] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Address] AS [dq_right]
    ON [dq_left].[BillToAddressHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[BillToAddressHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.55.7]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderShipToAddress] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesOrderHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.56.4]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'ShipToAddressHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[ShipToAddressHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderShipToAddress] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Address] AS [dq_right]
    ON [dq_left].[ShipToAddressHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[ShipToAddressHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.63.67]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPersonQuota -> AdventureWorksBusinessVault.dbo.BHS_SalesPersonQuota_SalesPersonQuotaProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalespersonQuota' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPersonQuota' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPersonQuota] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BHS_SalesPersonQuota_SalesPersonQuotaProfile] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[HubHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.64.64]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPersonQuota -> AdventureWorksBusinessVault.dbo.BL_SalesPersonQuotaSalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalespersonQuota' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPersonQuota' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPersonQuota] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesPersonQuotaSalesPerson] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesPersonQuotaHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.65.61]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesPersonQuotaSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Fact_SalespersonQuota' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesPersonQuotaSalesPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesPersonHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[SalesPersonHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesPersonQuotaSalesPerson] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_right]
    ON [dq_left].[SalesPersonHashKey] = [dq_right].[HashKey]
GROUP BY [dq_left].[SalesPersonHashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk.JoinPattern.8.121]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksBusinessVault.dbo.BL_AddressStateProvince' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_Address] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_AddressStateProvince] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[AddressHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

CREATE OR ALTER VIEW [dq].[v_DataQualityReview]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.1.135' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.1.135' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.1.135] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.1.135] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.1.135]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.10.113' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince -> AdventureWorksBusinessVault.dbo.BL_StateProvinceCountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince -> AdventureWorksBusinessVault.dbo.BL_StateProvinceCountryRegion' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.10.113' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.10.113] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.10.113] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.10.113]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.12.149' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.12.149' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.12.149] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.12.149] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.12.149]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.14.143' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryCategory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryCategory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.14.143' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.14.143] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.14.143] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.14.143]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.16.157' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Salesperson' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.16.157' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.16.157] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.16.157] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.16.157]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.19.163' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksBusinessVault.dbo.BL_SalesTerritoryCountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksBusinessVault.dbo.BL_SalesTerritoryCountryRegion' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_SalesTerritory' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.19.163' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.19.163] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.19.163] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.19.163]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.2.131' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerPerson -> AdventureWorksBusinessVault.dbo.BHS_Person_PersonProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerPerson -> AdventureWorksBusinessVault.dbo.BHS_Person_PersonProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.2.131' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.2.131] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.2.131] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.2.131]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.21.56' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BHS_SalesOrderLine_SalesOrderLineDetail' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BHS_SalesOrderLine_SalesOrderLineDetail' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.21.56' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.21.56] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.21.56] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.21.56]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.22.53' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BL_SalesOrderLineSalesOrder' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BL_SalesOrderLineSalesOrder' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.22.53' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.22.53] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.22.53] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.22.53]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.23.50' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineSalesOrder -> AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineSalesOrder -> AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.23.50' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.23.50] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.23.50] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.23.50]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.24.38' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BHS_SalesOrder_SalesOrderHeader' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BHS_SalesOrder_SalesOrderHeader' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.24.38' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.24.38] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.24.38] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.24.38]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.25.35' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BL_SalesOrderLineProduct' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BL_SalesOrderLineProduct' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.25.35' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.25.35] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.25.35] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.25.35]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.26.32' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineProduct -> AdventureWorksBusinessVault.dbo.BH_Product' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineProduct -> AdventureWorksBusinessVault.dbo.BH_Product' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.26.32' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.26.32] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.26.32] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.26.32]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.28.27' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.28.27' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.28.27] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.28.27] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.28.27]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.29.24' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer -> AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer -> AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.29.24' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.29.24] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.29.24] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.29.24]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.3.127' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerStore' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerStore' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.3.127' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.3.127] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.3.127] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.3.127]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.31.85' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.31.85' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.31.85] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.31.85] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.31.85]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.32.81' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.32.81' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.32.81] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.32.81] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.32.81]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.34.75' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.34.75' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.34.75] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.34.75] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.34.75]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.35.71' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.35.71' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.35.71] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.35.71] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.35.71]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.37.13' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.37.13' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.37.13] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.37.13] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.37.13]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.38.10' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.38.10' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.38.10] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.38.10] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.38.10]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.42.47' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BHS_SalesOrder_SalesOrderHeader' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BHS_SalesOrder_SalesOrderHeader' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.42.47' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.42.47] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.42.47] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.42.47]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.43.44' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.43.44' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.43.44] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.43.44] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.43.44]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.44.41' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer -> AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer -> AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.44.41' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.44.41] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.44.41] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.44.41]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.46.105' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.46.105' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.46.105] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.46.105] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.46.105]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.47.101' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.47.101' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.47.101] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.47.101] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.47.101]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.49.95' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.49.95' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.49.95] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.49.95] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.49.95]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.50.91' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.50.91' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.50.91] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.50.91] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.50.91]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.52.20' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderBillToAddress' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderBillToAddress' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.52.20' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.52.20] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.52.20] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.52.20]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.53.17' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderBillToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderBillToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.53.17' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.53.17] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.53.17] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.53.17]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.55.6' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.55.6' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.55.6] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.55.6] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.55.6]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.56.3' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.56.3' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.56.3] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.56.3] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.56.3]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.63.66' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPersonQuota -> AdventureWorksBusinessVault.dbo.BHS_SalesPersonQuota_SalesPersonQuotaProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPersonQuota -> AdventureWorksBusinessVault.dbo.BHS_SalesPersonQuota_SalesPersonQuotaProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalespersonQuota' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.63.66' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.63.66] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.63.66] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.63.66]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.64.63' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPersonQuota -> AdventureWorksBusinessVault.dbo.BL_SalesPersonQuotaSalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPersonQuota -> AdventureWorksBusinessVault.dbo.BL_SalesPersonQuotaSalesPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalespersonQuota' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.64.63' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.64.63] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.64.63] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.64.63]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.65.60' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesPersonQuotaSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesPersonQuotaSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalespersonQuota' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.65.60' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.65.60] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.65.60] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.65.60]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.8.119' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksBusinessVault.dbo.BL_AddressStateProvince' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksBusinessVault.dbo.BL_AddressStateProvince' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion.JoinPattern.8.119' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.8.119] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.8.119] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.8.119]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.1.134' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.1.134' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.1.134] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.1.134] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.1.134]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.10.112' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince -> AdventureWorksBusinessVault.dbo.BL_StateProvinceCountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince -> AdventureWorksBusinessVault.dbo.BL_StateProvinceCountryRegion' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.10.112' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.10.112] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.10.112] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.10.112]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.11.152' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product -> AdventureWorksBusinessVault.dbo.BHS_Product_ProductProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product -> AdventureWorksBusinessVault.dbo.BHS_Product_ProductProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.11.152' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.11.152] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.11.152] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.11.152]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.12.148' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.12.148' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.12.148] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.12.148] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.12.148]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.13.146' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment -> AdventureWorksBusinessVault.dbo.BHS_ProductSubcategory_ProductSubcategoryProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment -> AdventureWorksBusinessVault.dbo.BHS_ProductSubcategory_ProductSubcategoryProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.13.146' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.13.146] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.13.146] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.13.146]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.138' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BHS_Customer_CustomerProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BHS_Customer_CustomerProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.138' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.138] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.138] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.138]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.14.142' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryCategory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryCategory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.14.142' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.14.142] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.14.142] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.14.142]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.15.140' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryCategory -> AdventureWorksBusinessVault.dbo.BHS_ProductCategory_ProductCategoryProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryCategory -> AdventureWorksBusinessVault.dbo.BHS_ProductCategory_ProductCategoryProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.15.140' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.15.140] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.15.140] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.15.140]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.16.156' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Salesperson' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.16.156' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.16.156] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.16.156] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.16.156]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.17.154' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson -> AdventureWorksBusinessVault.dbo.BHS_Person_PersonProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson -> AdventureWorksBusinessVault.dbo.BHS_Person_PersonProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Salesperson' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.17.154' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.17.154] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.17.154] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.17.154]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.18.166' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksBusinessVault.dbo.BHS_SalesTerritory_SalesTerritoryProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksBusinessVault.dbo.BHS_SalesTerritory_SalesTerritoryProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_SalesTerritory' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.18.166' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.18.166] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.18.166] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.18.166]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.19.162' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksBusinessVault.dbo.BL_SalesTerritoryCountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksBusinessVault.dbo.BL_SalesTerritoryCountryRegion' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_SalesTerritory' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.19.162' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.19.162] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.19.162] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.19.162]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.2.130' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerPerson -> AdventureWorksBusinessVault.dbo.BHS_Person_PersonProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerPerson -> AdventureWorksBusinessVault.dbo.BHS_Person_PersonProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.2.130' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.2.130] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.2.130] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.2.130]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.20.160' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesTerritoryCountryRegion -> AdventureWorksBusinessVault.dbo.BH_CountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesTerritoryCountryRegion -> AdventureWorksBusinessVault.dbo.BH_CountryRegion' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_SalesTerritory' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.20.160' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.20.160] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.20.160] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.20.160]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.21.55' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BHS_SalesOrderLine_SalesOrderLineDetail' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BHS_SalesOrderLine_SalesOrderLineDetail' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.21.55' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.21.55] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.21.55] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.21.55]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.22.52' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BL_SalesOrderLineSalesOrder' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BL_SalesOrderLineSalesOrder' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.22.52' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.22.52] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.22.52] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.22.52]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.23.49' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineSalesOrder -> AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineSalesOrder -> AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.23.49' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.23.49] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.23.49] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.23.49]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.24.37' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BHS_SalesOrder_SalesOrderHeader' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BHS_SalesOrder_SalesOrderHeader' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.24.37' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.24.37] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.24.37] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.24.37]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.25.34' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BL_SalesOrderLineProduct' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BL_SalesOrderLineProduct' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.25.34' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.25.34] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.25.34] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.25.34]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.26.31' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineProduct -> AdventureWorksBusinessVault.dbo.BH_Product' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineProduct -> AdventureWorksBusinessVault.dbo.BH_Product' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.26.31' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.26.31] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.26.31] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.26.31]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.27.30' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product -> AdventureWorksAnalytics.dw.Dim_Product' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product -> AdventureWorksAnalytics.dw.Dim_Product' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.27.30' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.27.30] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.27.30] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.27.30]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.28.26' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.28.26' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.28.26] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.28.26] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.28.26]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.29.23' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer -> AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer -> AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.29.23' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.29.23] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.29.23] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.29.23]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.3.126' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerStore' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerStore' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.3.126' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.3.126] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.3.126] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.3.126]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.30.22' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksAnalytics.dw.Dim_Customer' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksAnalytics.dw.Dim_Customer' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.30.22' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.30.22] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.30.22] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.30.22]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.31.84' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.31.84' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.31.84] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.31.84] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.31.84]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.32.80' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.32.80' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.32.80] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.32.80] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.32.80]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.33.78' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksAnalytics.dw.Dim_Salesperson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksAnalytics.dw.Dim_Salesperson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.33.78' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.33.78] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.33.78] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.33.78]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.34.74' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.34.74' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.34.74] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.34.74] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.34.74]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.35.70' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.35.70' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.35.70] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.35.70] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.35.70]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.36.68' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksAnalytics.dw.Dim_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksAnalytics.dw.Dim_SalesTerritory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.36.68' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.36.68] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.36.68] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.36.68]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.37.12' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.37.12' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.37.12] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.37.12] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.37.12]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.38.9' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.38.9' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.38.9] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.38.9] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.38.9]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.39.8' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksAnalytics.dw.Dim_Geography' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksAnalytics.dw.Dim_Geography' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.39.8' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.39.8] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.39.8] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.39.8]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.4.124' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerStore -> AdventureWorksBusinessVault.dbo.BHS_Store_StoreProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerStore -> AdventureWorksBusinessVault.dbo.BHS_Store_StoreProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.4.124' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.4.124] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.4.124] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.4.124]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.42.46' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BHS_SalesOrder_SalesOrderHeader' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BHS_SalesOrder_SalesOrderHeader' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.42.46' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.42.46] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.42.46] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.42.46]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.43.43' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.43.43' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.43.43] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.43.43] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.43.43]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.44.40' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer -> AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer -> AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.44.40' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.44.40] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.44.40] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.44.40]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.45.29' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksAnalytics.dw.Dim_Customer' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksAnalytics.dw.Dim_Customer' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.45.29' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.45.29] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.45.29] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.45.29]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.46.104' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.46.104' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.46.104] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.46.104] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.46.104]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.47.100' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.47.100' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.47.100] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.47.100] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.47.100]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.48.98' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksAnalytics.dw.Dim_Salesperson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksAnalytics.dw.Dim_Salesperson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.48.98' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.48.98] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.48.98] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.48.98]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.49.94' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.49.94' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.49.94] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.49.94] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.49.94]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.5.110' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_StateProvinceCountryRegion -> AdventureWorksBusinessVault.dbo.BH_CountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_StateProvinceCountryRegion -> AdventureWorksBusinessVault.dbo.BH_CountryRegion' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.5.110' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.5.110] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.5.110] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.5.110]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.50.90' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.50.90' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.50.90] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.50.90] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.50.90]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.51.88' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksAnalytics.dw.Dim_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksAnalytics.dw.Dim_SalesTerritory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.51.88' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.51.88] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.51.88] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.51.88]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.52.19' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderBillToAddress' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderBillToAddress' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.52.19' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.52.19] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.52.19] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.52.19]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.53.16' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderBillToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderBillToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.53.16' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.53.16] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.53.16] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.53.16]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.54.15' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksAnalytics.dw.Dim_Geography' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksAnalytics.dw.Dim_Geography' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.54.15' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.54.15] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.54.15] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.54.15]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.55.5' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.55.5' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.55.5] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.55.5] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.55.5]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.56.2' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.56.2' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.56.2] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.56.2] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.56.2]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.57.1' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksAnalytics.dw.Dim_Geography' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksAnalytics.dw.Dim_Geography' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.57.1' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.57.1] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.57.1] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.57.1]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.6.108' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_CountryRegion -> AdventureWorksBusinessVault.dbo.BHS_CountryRegion_CountryRegionProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_CountryRegion -> AdventureWorksBusinessVault.dbo.BHS_CountryRegion_CountryRegionProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.6.108' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.6.108] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.6.108] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.6.108]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.63.65' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPersonQuota -> AdventureWorksBusinessVault.dbo.BHS_SalesPersonQuota_SalesPersonQuotaProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPersonQuota -> AdventureWorksBusinessVault.dbo.BHS_SalesPersonQuota_SalesPersonQuotaProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalespersonQuota' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.63.65' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.63.65] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.63.65] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.63.65]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.64.62' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPersonQuota -> AdventureWorksBusinessVault.dbo.BL_SalesPersonQuotaSalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPersonQuota -> AdventureWorksBusinessVault.dbo.BL_SalesPersonQuotaSalesPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalespersonQuota' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.64.62' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.64.62] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.64.62] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.64.62]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.65.59' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesPersonQuotaSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesPersonQuotaSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalespersonQuota' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.65.59' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.65.59] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.65.59] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.65.59]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.66.58' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksAnalytics.dw.Dim_Salesperson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksAnalytics.dw.Dim_Salesperson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalespersonQuota' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.66.58' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.66.58] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.66.58] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.66.58]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.7.122' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksBusinessVault.dbo.BHS_Address_AddressProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksBusinessVault.dbo.BHS_Address_AddressProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.7.122' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.7.122] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.7.122] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.7.122]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.8.118' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksBusinessVault.dbo.BL_AddressStateProvince' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksBusinessVault.dbo.BL_AddressStateProvince' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.8.118' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.8.118] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.8.118] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.8.118]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.9.116' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince -> AdventureWorksBusinessVault.dbo.BHS_StateProvince_StateProvinceProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince -> AdventureWorksBusinessVault.dbo.BHS_StateProvince_StateProvinceProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan.JoinPattern.9.116' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.9.116] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan.JoinPattern.9.116] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan.JoinPattern.9.116]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.1.136' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.1.136' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.1.136] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.1.136] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.1.136]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.10.114' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince -> AdventureWorksBusinessVault.dbo.BL_StateProvinceCountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince -> AdventureWorksBusinessVault.dbo.BL_StateProvinceCountryRegion' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.10.114' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.10.114] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.10.114] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.10.114]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.11.153' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product -> AdventureWorksBusinessVault.dbo.BHS_Product_ProductProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product -> AdventureWorksBusinessVault.dbo.BHS_Product_ProductProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.11.153' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.11.153] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.11.153] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.11.153]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.12.150' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.12.150' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.12.150] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.12.150] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.12.150]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.13.147' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment -> AdventureWorksBusinessVault.dbo.BHS_ProductSubcategory_ProductSubcategoryProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment -> AdventureWorksBusinessVault.dbo.BHS_ProductSubcategory_ProductSubcategoryProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.13.147' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.13.147] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.13.147] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.13.147]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.139' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BHS_Customer_CustomerProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BHS_Customer_CustomerProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.139' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.139] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.139] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.139]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.14.144' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryCategory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryCategory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.14.144' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.14.144] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.14.144] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.14.144]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.15.141' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryCategory -> AdventureWorksBusinessVault.dbo.BHS_ProductCategory_ProductCategoryProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryCategory -> AdventureWorksBusinessVault.dbo.BHS_ProductCategory_ProductCategoryProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.15.141' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.15.141] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.15.141] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.15.141]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.16.158' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Salesperson' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.16.158' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.16.158] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.16.158] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.16.158]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.17.155' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson -> AdventureWorksBusinessVault.dbo.BHS_Person_PersonProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson -> AdventureWorksBusinessVault.dbo.BHS_Person_PersonProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Salesperson' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.17.155' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.17.155] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.17.155] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.17.155]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.18.167' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksBusinessVault.dbo.BHS_SalesTerritory_SalesTerritoryProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksBusinessVault.dbo.BHS_SalesTerritory_SalesTerritoryProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_SalesTerritory' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.18.167' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.18.167] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.18.167] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.18.167]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.19.164' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksBusinessVault.dbo.BL_SalesTerritoryCountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksBusinessVault.dbo.BL_SalesTerritoryCountryRegion' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_SalesTerritory' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.19.164' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.19.164] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.19.164] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.19.164]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.2.132' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerPerson -> AdventureWorksBusinessVault.dbo.BHS_Person_PersonProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerPerson -> AdventureWorksBusinessVault.dbo.BHS_Person_PersonProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.2.132' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.2.132] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.2.132] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.2.132]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.20.161' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesTerritoryCountryRegion -> AdventureWorksBusinessVault.dbo.BH_CountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesTerritoryCountryRegion -> AdventureWorksBusinessVault.dbo.BH_CountryRegion' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_SalesTerritory' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.20.161' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.20.161] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.20.161] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.20.161]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.3.128' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerStore' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerStore' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.3.128' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.3.128] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.3.128] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.3.128]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.31.86' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.31.86' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.31.86] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.31.86] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.31.86]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.32.82' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.32.82' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.32.82] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.32.82] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.32.82]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.33.79' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksAnalytics.dw.Dim_Salesperson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksAnalytics.dw.Dim_Salesperson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.33.79' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.33.79] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.33.79] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.33.79]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.34.76' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.34.76' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.34.76] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.34.76] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.34.76]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.35.72' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.35.72' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.35.72] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.35.72] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.35.72]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.36.69' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksAnalytics.dw.Dim_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksAnalytics.dw.Dim_SalesTerritory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.36.69' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.36.69] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.36.69] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.36.69]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.4.125' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerStore -> AdventureWorksBusinessVault.dbo.BHS_Store_StoreProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerStore -> AdventureWorksBusinessVault.dbo.BHS_Store_StoreProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.4.125' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.4.125] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.4.125] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.4.125]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.46.106' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.46.106' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.46.106] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.46.106] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.46.106]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.47.102' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.47.102' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.47.102] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.47.102] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.47.102]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.48.99' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksAnalytics.dw.Dim_Salesperson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksAnalytics.dw.Dim_Salesperson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.48.99' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.48.99] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.48.99] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.48.99]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.49.96' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.49.96' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.49.96] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.49.96] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.49.96]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.5.111' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_StateProvinceCountryRegion -> AdventureWorksBusinessVault.dbo.BH_CountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_StateProvinceCountryRegion -> AdventureWorksBusinessVault.dbo.BH_CountryRegion' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.5.111' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.5.111] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.5.111] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.5.111]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.50.92' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.50.92' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.50.92] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.50.92] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.50.92]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.51.89' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksAnalytics.dw.Dim_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksAnalytics.dw.Dim_SalesTerritory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.51.89' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.51.89] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.51.89] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.51.89]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.6.109' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_CountryRegion -> AdventureWorksBusinessVault.dbo.BHS_CountryRegion_CountryRegionProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_CountryRegion -> AdventureWorksBusinessVault.dbo.BHS_CountryRegion_CountryRegionProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.6.109' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.6.109] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.6.109] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.6.109]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.7.123' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksBusinessVault.dbo.BHS_Address_AddressProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksBusinessVault.dbo.BHS_Address_AddressProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.7.123' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.7.123] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.7.123] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.7.123]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.8.120' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksBusinessVault.dbo.BL_AddressStateProvince' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksBusinessVault.dbo.BL_AddressStateProvince' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.8.120' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.8.120] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.8.120] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.8.120]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.9.117' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince -> AdventureWorksBusinessVault.dbo.BHS_StateProvince_StateProvinceProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince -> AdventureWorksBusinessVault.dbo.BHS_StateProvince_StateProvinceProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion.JoinPattern.9.117' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.9.117] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.9.117] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.9.117]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.1.137' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.1.137' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.1.137] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.1.137] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.1.137]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.10.115' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince -> AdventureWorksBusinessVault.dbo.BL_StateProvinceCountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_AddressStateProvince -> AdventureWorksBusinessVault.dbo.BL_StateProvinceCountryRegion' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.10.115' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.10.115] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.10.115] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.10.115]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.12.151' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Product -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.12.151' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.12.151] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.12.151] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.12.151]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.14.145' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryCategory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment -> AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryCategory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Product' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.14.145' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.14.145] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.14.145] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.14.145]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.16.159' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Salesperson' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.16.159' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.16.159] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.16.159] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.16.159]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.19.165' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksBusinessVault.dbo.BL_SalesTerritoryCountryRegion' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesTerritory -> AdventureWorksBusinessVault.dbo.BL_SalesTerritoryCountryRegion' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_SalesTerritory' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.19.165' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.19.165] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.19.165] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.19.165]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.2.133' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerPerson -> AdventureWorksBusinessVault.dbo.BHS_Person_PersonProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_CustomerPerson -> AdventureWorksBusinessVault.dbo.BHS_Person_PersonProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.2.133' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.2.133] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.2.133] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.2.133]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.21.57' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BHS_SalesOrderLine_SalesOrderLineDetail' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BHS_SalesOrderLine_SalesOrderLineDetail' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.21.57' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.21.57] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.21.57] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.21.57]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.22.54' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BL_SalesOrderLineSalesOrder' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BL_SalesOrderLineSalesOrder' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.22.54' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.22.54] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.22.54] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.22.54]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.23.51' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineSalesOrder -> AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineSalesOrder -> AdventureWorksBusinessVault.dbo.BH_SalesOrder' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.23.51' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.23.51] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.23.51] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.23.51]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.24.39' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BHS_SalesOrder_SalesOrderHeader' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BHS_SalesOrder_SalesOrderHeader' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.24.39' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.24.39] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.24.39] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.24.39]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.25.36' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BL_SalesOrderLineProduct' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrderLine -> AdventureWorksBusinessVault.dbo.BL_SalesOrderLineProduct' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.25.36' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.25.36] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.25.36] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.25.36]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.26.33' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineProduct -> AdventureWorksBusinessVault.dbo.BH_Product' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineProduct -> AdventureWorksBusinessVault.dbo.BH_Product' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.26.33' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.26.33] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.26.33] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.26.33]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.28.28' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.28.28' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.28.28] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.28.28] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.28.28]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.29.25' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer -> AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer -> AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.29.25' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.29.25] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.29.25] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.29.25]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.3.129' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerStore' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BL_CustomerStore' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Customer' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.3.129' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.3.129] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.3.129] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.3.129]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.31.87' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.31.87' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.31.87] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.31.87] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.31.87]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.32.83' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.32.83' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.32.83] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.32.83] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.32.83]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.34.77' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.34.77' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.34.77] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.34.77] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.34.77]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.35.73' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.35.73' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.35.73] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.35.73] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.35.73]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.37.14' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.37.14' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.37.14] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.37.14] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.37.14]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.38.11' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesLine' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.38.11' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.38.11] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.38.11] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.38.11]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.42.48' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BHS_SalesOrder_SalesOrderHeader' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BHS_SalesOrder_SalesOrderHeader' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.42.48' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.42.48] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.42.48] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.42.48]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.43.45' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.43.45' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.43.45] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.43.45] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.43.45]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.44.42' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer -> AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderCustomer -> AdventureWorksBusinessVault.dbo.BH_Customer' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.44.42' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.44.42] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.44.42] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.44.42]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.46.107' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.46.107' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.46.107] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.46.107] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.46.107]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.47.103' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.47.103' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.47.103] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.47.103] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.47.103]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.49.97' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.49.97' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.49.97] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.49.97] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.49.97]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.50.93' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesTerritory -> AdventureWorksBusinessVault.dbo.BH_SalesTerritory' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.50.93' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.50.93] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.50.93] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.50.93]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.52.21' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderBillToAddress' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderBillToAddress' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.52.21' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.52.21] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.52.21] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.52.21]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.53.18' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderBillToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderBillToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.53.18' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.53.18] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.53.18] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.53.18]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.55.7' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesOrder -> AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.55.7' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.55.7] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.55.7] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.55.7]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.56.4' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderShipToAddress -> AdventureWorksBusinessVault.dbo.BH_Address' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalesOrder' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.56.4' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.56.4] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.56.4] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.56.4]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.63.67' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPersonQuota -> AdventureWorksBusinessVault.dbo.BHS_SalesPersonQuota_SalesPersonQuotaProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPersonQuota -> AdventureWorksBusinessVault.dbo.BHS_SalesPersonQuota_SalesPersonQuotaProfile' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalespersonQuota' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.63.67' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.63.67] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.63.67] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.63.67]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.64.64' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPersonQuota -> AdventureWorksBusinessVault.dbo.BL_SalesPersonQuotaSalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPersonQuota -> AdventureWorksBusinessVault.dbo.BL_SalesPersonQuotaSalesPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalespersonQuota' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.64.64' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.64.64] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.64.64] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.64.64]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.65.61' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesPersonQuotaSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesPersonQuotaSalesPerson -> AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Fact_SalespersonQuota' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.65.61' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.65.61] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.65.61] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.65.61]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.8.121' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksBusinessVault.dbo.BL_AddressStateProvince' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Address -> AdventureWorksBusinessVault.dbo.BL_AddressStateProvince' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Geography' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk.JoinPattern.8.121' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.8.121] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk.JoinPattern.8.121] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk.JoinPattern.8.121]
GO

/* MetaDataQuality operational store */
IF DB_ID(N'MetaDQ') IS NULL
BEGIN
    CREATE DATABASE [MetaDQ];
END
GO

EXEC [MetaDQ].sys.sp_executesql N'
IF OBJECT_ID(N''[dbo].[RunLog]'', N''U'') IS NULL
BEGIN
    CREATE TABLE [dbo].[RunLog]
    (
        [RunId] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_dbo_RunLog] PRIMARY KEY,
        [StartedAtUtc] datetime2(3) NOT NULL CONSTRAINT [DF_dbo_RunLog_StartedAtUtc] DEFAULT (SYSUTCDATETIME()),
        [CompletedAtUtc] datetime2(3) NULL,
        [SourceDatabaseName] sysname NOT NULL,
        [Status] nvarchar(32) NOT NULL,
        [ErrorMessage] nvarchar(4000) NULL
    );
END;

IF COL_LENGTH(N''dbo.RunLog'', N''SourceDatabaseName'') IS NULL
BEGIN
    ALTER TABLE [dbo].[RunLog] ADD [SourceDatabaseName] sysname NULL;
END;
IF COL_LENGTH(N''dbo.RunLog'', N''Status'') IS NULL
BEGIN
    ALTER TABLE [dbo].[RunLog] ADD [Status] nvarchar(32) NULL;
END;
IF COL_LENGTH(N''dbo.RunLog'', N''ErrorMessage'') IS NULL
BEGIN
    ALTER TABLE [dbo].[RunLog] ADD [ErrorMessage] nvarchar(4000) NULL;
END;

IF OBJECT_ID(N''[dbo].[FindingLog]'', N''U'') IS NULL
BEGIN
    CREATE TABLE [dbo].[FindingLog]
    (
        [FindingId] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_dbo_FindingLog] PRIMARY KEY,
        [RunId] bigint NOT NULL,
        [DQView] nvarchar(128) NOT NULL,
        [Issue] nvarchar(128) NOT NULL,
        [FindingTitle] nvarchar(128) NULL,
        [FindingCategory] nvarchar(128) NULL,
        [OutputMode] nvarchar(64) NULL,
        [CandidateId] nvarchar(256) NULL,
        [CandidateKind] nvarchar(128) NULL,
        [Relationship] nvarchar(512) NOT NULL,
        [RelationshipLabel] nvarchar(512) NULL,
        [ReferencingObject] nvarchar(512) NULL,
        [ReferencedObject] nvarchar(512) NULL,
        [CheckedObject] nvarchar(512) NULL,
        [SuspectSide] nvarchar(512) NULL,
        [SuspectObject] nvarchar(512) NULL,
        [LookupObject] nvarchar(512) NULL,
        [RelatedObject] nvarchar(512) NULL,
        [CorpusRelationship] nvarchar(512) NULL,
        [CorpusRelationshipPattern] nvarchar(max) NULL,
        [DominantPattern] nvarchar(max) NULL,
        [OutlierPattern] nvarchar(max) NULL,
        [TransformViews] nvarchar(max) NOT NULL,
        [GeneratedView] nvarchar(512) NOT NULL,
        [RowsReturned] bigint NULL,
        [ResultRowCount] bigint NULL,
        [FindingGroupCount] bigint NULL,
        [TotalSuspectCount] bigint NULL,
        [SuspectRowCount] bigint NULL,
        [Explanation] nvarchar(max) NULL,
        [FindingExplanation] nvarchar(max) NULL,
        [EvidenceSummary] nvarchar(max) NULL,
        [EvidenceOccurrenceCount] bigint NULL,
        [OutlierOccurrenceCount] bigint NULL,
        [EvidenceTransformCount] bigint NULL,
        [OutlierTransformCount] bigint NULL,
        [EvidenceConsensusRatio] decimal(18,6) NULL,
        [DominantConsensusRatio] decimal(18,6) NULL,
        [EvidenceOutlierRatio] decimal(18,6) NULL,
        [OutlierRatio] decimal(18,6) NULL,
        [EvidenceQuality] nvarchar(16) NULL,
        [ConfidenceBand] nvarchar(16) NULL,
        [ConfidenceReason] nvarchar(max) NULL,
        [EvidenceDiversitySummary] nvarchar(max) NULL,
        [ConfidenceSummary] nvarchar(max) NULL,
        [DistinctTransformCount] bigint NULL,
        [DistinctSourceTransformCount] bigint NULL,
        [DistinctSourceObjectCount] bigint NULL,
        [DistinctRelationshipPatternCount] bigint NULL,
        [EffectiveTransformCount] bigint NULL,
        [RecommendedAction] nvarchar(128) NOT NULL,
        [RuntimeCountStatus] nvarchar(64) NULL,
        [ReviewQuery] nvarchar(max) NULL,
        [DetailQuery] nvarchar(max) NULL,
        [TransformViewQuery] nvarchar(max) NULL,
        [SupportingTransformQuery] nvarchar(max) NULL,
        [CapturedAtUtc] datetime2(3) NOT NULL CONSTRAINT [DF_dbo_FindingLog_CapturedAtUtc] DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [FK_dbo_FindingLog_RunId] FOREIGN KEY ([RunId]) REFERENCES [dbo].[RunLog]([RunId])
    );
END;

IF COL_LENGTH(N''dbo.FindingLog'', N''Issue'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [Issue] nvarchar(128) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''FindingTitle'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [FindingTitle] nvarchar(128) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''FindingCategory'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [FindingCategory] nvarchar(128) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''ReviewQuery'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [ReviewQuery] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''DetailQuery'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [DetailQuery] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''TransformViewQuery'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [TransformViewQuery] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''SupportingTransformQuery'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [SupportingTransformQuery] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''OutputMode'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [OutputMode] nvarchar(64) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''CandidateId'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [CandidateId] nvarchar(256) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''CandidateKind'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [CandidateKind] nvarchar(128) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''RelationshipLabel'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [RelationshipLabel] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''ReferencingObject'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [ReferencingObject] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''ReferencedObject'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [ReferencedObject] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''CheckedObject'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [CheckedObject] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''SuspectSide'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [SuspectSide] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''SuspectObject'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [SuspectObject] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''LookupObject'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [LookupObject] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''RelatedObject'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [RelatedObject] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''CorpusRelationship'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [CorpusRelationship] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''CorpusRelationshipPattern'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [CorpusRelationshipPattern] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''DominantPattern'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [DominantPattern] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''OutlierPattern'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [OutlierPattern] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''ResultRowCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [ResultRowCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''FindingGroupCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [FindingGroupCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''SuspectRowCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [SuspectRowCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''Explanation'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [Explanation] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''FindingExplanation'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [FindingExplanation] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceSummary'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceSummary] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceOccurrenceCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceOccurrenceCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''OutlierOccurrenceCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [OutlierOccurrenceCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceTransformCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceTransformCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''OutlierTransformCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [OutlierTransformCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceConsensusRatio'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceConsensusRatio] decimal(18,6) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''DominantConsensusRatio'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [DominantConsensusRatio] decimal(18,6) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceOutlierRatio'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceOutlierRatio] decimal(18,6) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''OutlierRatio'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [OutlierRatio] decimal(18,6) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceQuality'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceQuality] nvarchar(16) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''ConfidenceBand'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [ConfidenceBand] nvarchar(16) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''ConfidenceReason'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [ConfidenceReason] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceDiversitySummary'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceDiversitySummary] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''ConfidenceSummary'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [ConfidenceSummary] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''DistinctTransformCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [DistinctTransformCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''DistinctSourceTransformCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [DistinctSourceTransformCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''DistinctSourceObjectCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [DistinctSourceObjectCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''DistinctRelationshipPatternCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [DistinctRelationshipPatternCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EffectiveTransformCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EffectiveTransformCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''RuntimeCountStatus'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [RuntimeCountStatus] nvarchar(64) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''RowsReturned'') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ALTER COLUMN [RowsReturned] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''TotalSuspectCount'') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ALTER COLUMN [TotalSuspectCount] bigint NULL;
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N''IX_dbo_FindingLog_RunId''
      AND [object_id] = OBJECT_ID(N''[dbo].[FindingLog]'')
)
BEGIN
    CREATE INDEX [IX_dbo_FindingLog_RunId] ON [dbo].[FindingLog]([RunId]);
END;
';
GO

EXEC [MetaDQ].sys.sp_executesql N'
CREATE OR ALTER PROCEDURE [dbo].[Run]
    @SourceDatabaseName sysname,
    @RunId bigint OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NULLIF(LTRIM(RTRIM(@SourceDatabaseName)), N'''') IS NULL
    BEGIN
        THROW 51000, N''@SourceDatabaseName is required.'', 1;
    END;

    IF DB_ID(@SourceDatabaseName) IS NULL
    BEGIN
        THROW 51000, N''Source database was not found.'', 1;
    END;

    DECLARE @runIdLocal bigint;
    INSERT INTO [dbo].[RunLog] ([SourceDatabaseName], [Status])
    VALUES (@SourceDatabaseName, N''Running'');
    SET @runIdLocal = SCOPE_IDENTITY();
    SET @RunId = @runIdLocal;

    BEGIN TRY
        DECLARE @sql nvarchar(max) =
            N''INSERT INTO [dbo].[FindingLog] ([RunId], [DQView], [Issue], [FindingTitle], [FindingCategory], [OutputMode], [CandidateId], [CandidateKind], [Relationship], [RelationshipLabel], [ReferencingObject], [ReferencedObject], [CheckedObject], [SuspectSide], [SuspectObject], [LookupObject], [RelatedObject], [CorpusRelationship], [CorpusRelationshipPattern], [DominantPattern], [OutlierPattern], [TransformViews], [GeneratedView], [RowsReturned], [ResultRowCount], [FindingGroupCount], [TotalSuspectCount], [SuspectRowCount], [Explanation], [FindingExplanation], [EvidenceSummary], [EvidenceOccurrenceCount], [OutlierOccurrenceCount], [EvidenceTransformCount], [OutlierTransformCount], [EvidenceConsensusRatio], [DominantConsensusRatio], [EvidenceOutlierRatio], [OutlierRatio], [EvidenceQuality], [ConfidenceBand], [ConfidenceReason], [EvidenceDiversitySummary], [ConfidenceSummary], [DistinctTransformCount], [DistinctSourceTransformCount], [DistinctSourceObjectCount], [DistinctRelationshipPatternCount], [EffectiveTransformCount], [RecommendedAction], [RuntimeCountStatus], [ReviewQuery], [DetailQuery], [TransformViewQuery], [SupportingTransformQuery]) '' +
            N''SELECT @RunId, [DQView], [Issue], [FindingTitle], [FindingCategory], [OutputMode], [CandidateId], [CandidateKind], [Relationship], [RelationshipLabel], [ReferencingObject], [ReferencedObject], [CheckedObject], [SuspectSide], [SuspectObject], [LookupObject], [RelatedObject], [CorpusRelationship], [CorpusRelationshipPattern], [DominantPattern], [OutlierPattern], [TransformViews], [GeneratedView], [RowsReturned], [ResultRowCount], [FindingGroupCount], [TotalSuspectCount], [SuspectRowCount], [Explanation], [FindingExplanation], [EvidenceSummary], [EvidenceOccurrenceCount], [OutlierOccurrenceCount], [EvidenceTransformCount], [OutlierTransformCount], [EvidenceConsensusRatio], [DominantConsensusRatio], [EvidenceOutlierRatio], [OutlierRatio], [EvidenceQuality], [ConfidenceBand], [ConfidenceReason], [EvidenceDiversitySummary], [ConfidenceSummary], [DistinctTransformCount], [DistinctSourceTransformCount], [DistinctSourceObjectCount], [DistinctRelationshipPatternCount], [EffectiveTransformCount], [RecommendedAction], [RuntimeCountStatus], [ReviewQuery], [DetailQuery], [TransformViewQuery], [SupportingTransformQuery] '' +
            N''FROM '' + QUOTENAME(@SourceDatabaseName) + N''.[dq].[v_DataQualityReview];'';

        EXEC sys.sp_executesql
            @sql,
            N''@RunId bigint'',
            @RunId = @runIdLocal;

        UPDATE [dbo].[RunLog]
        SET [CompletedAtUtc] = SYSUTCDATETIME(),
            [Status] = N''Completed''
        WHERE [RunId] = @runIdLocal;
    END TRY
    BEGIN CATCH
        UPDATE [dbo].[RunLog]
        SET [CompletedAtUtc] = SYSUTCDATETIME(),
            [Status] = N''Failed'',
            [ErrorMessage] = ERROR_MESSAGE()
        WHERE [RunId] = @runIdLocal;
        THROW;
    END CATCH;

    SELECT
        r.[RunId],
        r.[SourceDatabaseName],
        SUM(CASE WHEN f.[RowsReturned] IS NULL THEN 0 ELSE f.[RowsReturned] END) AS [RowsReturned],
        SUM(CASE WHEN f.[RowsReturned] IS NULL THEN 0 ELSE f.[RowsReturned] END) AS [ResultRowCount],
        SUM(CASE WHEN f.[RowsReturned] IS NULL THEN 0 ELSE f.[RowsReturned] END) AS [FindingGroupCount],
        SUM(CASE WHEN f.[TotalSuspectCount] IS NULL THEN 0 ELSE f.[TotalSuspectCount] END) AS [TotalSuspectCount],
        SUM(CASE WHEN f.[TotalSuspectCount] IS NULL THEN 0 ELSE f.[TotalSuspectCount] END) AS [SuspectRowCount],
        COUNT(f.[FindingId]) AS [ChecksExecuted],
        COUNT(f.[FindingId]) AS [FindingsExecuted],
        SUM(CASE WHEN f.[RowsReturned] IS NULL THEN 0 ELSE f.[RowsReturned] END) AS [RuntimeFindingGroupCount],
        SUM(CASE WHEN f.[TotalSuspectCount] IS NULL THEN 0 ELSE f.[TotalSuspectCount] END) AS [RuntimeSuspectRowCount]
    FROM [dbo].[RunLog] AS r
    LEFT JOIN [dbo].[FindingLog] AS f
      ON f.[RunId] = r.[RunId]
    WHERE r.[RunId] = @runIdLocal
    GROUP BY r.[RunId], r.[SourceDatabaseName];

    SELECT
        [DQView],
        [Issue],
        [FindingTitle],
        [FindingCategory],
        [OutputMode],
        [CandidateId],
        [CandidateKind],
        [Relationship],
        [RelationshipLabel],
        [ReferencingObject],
        [ReferencedObject],
        [CheckedObject],
        [SuspectSide],
        [SuspectObject],
        [LookupObject],
        [RelatedObject],
        [CorpusRelationship],
        [CorpusRelationshipPattern],
        [DominantPattern],
        [OutlierPattern],
        [RowsReturned],
        [ResultRowCount],
        [FindingGroupCount],
        [TotalSuspectCount],
        [SuspectRowCount],
        [Explanation],
        [FindingExplanation],
        [EvidenceSummary],
        [EvidenceOccurrenceCount],
        [OutlierOccurrenceCount],
        [EvidenceTransformCount],
        [OutlierTransformCount],
        [EvidenceConsensusRatio],
        [DominantConsensusRatio],
        [EvidenceOutlierRatio],
        [OutlierRatio],
        [EvidenceQuality],
        [ConfidenceBand],
        [ConfidenceReason],
        [EvidenceDiversitySummary],
        [ConfidenceSummary],
        [DistinctTransformCount],
        [DistinctSourceTransformCount],
        [DistinctSourceObjectCount],
        [DistinctRelationshipPatternCount],
        [EffectiveTransformCount],
        [RecommendedAction],
        [RuntimeCountStatus],
        [GeneratedView],
        [ReviewQuery],
        [DetailQuery],
        [TransformViewQuery],
        [SupportingTransformQuery]
    FROM [dbo].[FindingLog]
    WHERE [RunId] = @runIdLocal
      AND ([RowsReturned] > 0 OR [RowsReturned] IS NULL)
    ORDER BY CASE WHEN [RowsReturned] IS NULL THEN 1 ELSE 0 END, [RowsReturned] DESC, [DQView] ASC;
END;
';
GO

EXEC [MetaDQ].sys.sp_executesql N'
CREATE OR ALTER PROCEDURE [dbo].[Findings]
    @RunId bigint = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @RunId IS NULL
    BEGIN
        SELECT TOP (1) @RunId = [RunId]
        FROM [dbo].[RunLog]
        ORDER BY [RunId] DESC;
    END;

    IF @RunId IS NULL
    BEGIN
        SELECT
            CAST(NULL AS bigint) AS [RunId],
            CAST(NULL AS nvarchar(128)) AS [DQView],
            CAST(NULL AS nvarchar(128)) AS [Issue],
            CAST(NULL AS nvarchar(128)) AS [FindingTitle],
            CAST(NULL AS nvarchar(128)) AS [FindingCategory],
            CAST(NULL AS nvarchar(64)) AS [OutputMode],
            CAST(NULL AS nvarchar(256)) AS [CandidateId],
            CAST(NULL AS nvarchar(128)) AS [CandidateKind],
            CAST(NULL AS nvarchar(512)) AS [Relationship],
            CAST(NULL AS nvarchar(512)) AS [RelationshipLabel],
            CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
            CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
            CAST(NULL AS nvarchar(512)) AS [CheckedObject],
            CAST(NULL AS nvarchar(512)) AS [SuspectSide],
            CAST(NULL AS nvarchar(512)) AS [SuspectObject],
            CAST(NULL AS nvarchar(512)) AS [LookupObject],
            CAST(NULL AS nvarchar(512)) AS [RelatedObject],
            CAST(NULL AS nvarchar(512)) AS [CorpusRelationship],
            CAST(NULL AS nvarchar(max)) AS [CorpusRelationshipPattern],
            CAST(NULL AS nvarchar(max)) AS [DominantPattern],
            CAST(NULL AS nvarchar(max)) AS [OutlierPattern],
            CAST(NULL AS bigint) AS [RowsReturned],
            CAST(NULL AS bigint) AS [ResultRowCount],
            CAST(NULL AS bigint) AS [FindingGroupCount],
            CAST(NULL AS bigint) AS [TotalSuspectCount],
            CAST(NULL AS bigint) AS [SuspectRowCount],
            CAST(NULL AS nvarchar(max)) AS [Explanation],
            CAST(NULL AS nvarchar(max)) AS [FindingExplanation],
            CAST(NULL AS nvarchar(max)) AS [EvidenceSummary],
            CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
            CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
            CAST(NULL AS bigint) AS [EvidenceTransformCount],
            CAST(NULL AS bigint) AS [OutlierTransformCount],
            CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
            CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
            CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
            CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
            CAST(NULL AS nvarchar(16)) AS [EvidenceQuality],
            CAST(NULL AS nvarchar(16)) AS [ConfidenceBand],
            CAST(NULL AS nvarchar(max)) AS [ConfidenceReason],
            CAST(NULL AS nvarchar(max)) AS [EvidenceDiversitySummary],
            CAST(NULL AS nvarchar(max)) AS [ConfidenceSummary],
            CAST(NULL AS bigint) AS [DistinctTransformCount],
            CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
            CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
            CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
            CAST(NULL AS bigint) AS [EffectiveTransformCount],
            CAST(NULL AS nvarchar(128)) AS [RecommendedAction],
            CAST(NULL AS nvarchar(64)) AS [RuntimeCountStatus],
            CAST(NULL AS nvarchar(512)) AS [GeneratedView],
            CAST(NULL AS nvarchar(max)) AS [ReviewQuery],
            CAST(NULL AS nvarchar(max)) AS [DetailQuery],
            CAST(NULL AS nvarchar(max)) AS [TransformViewQuery],
            CAST(NULL AS nvarchar(max)) AS [SupportingTransformQuery]
        WHERE 1 = 0;
        RETURN;
    END;

    SELECT
        [RunId],
        [DQView],
        [Issue],
        [FindingTitle],
        [FindingCategory],
        [OutputMode],
        [CandidateId],
        [CandidateKind],
        [Relationship],
        [RelationshipLabel],
        [ReferencingObject],
        [ReferencedObject],
        [CheckedObject],
        [SuspectSide],
        [SuspectObject],
        [LookupObject],
        [RelatedObject],
        [CorpusRelationship],
        [CorpusRelationshipPattern],
        [DominantPattern],
        [OutlierPattern],
        [RowsReturned],
        [ResultRowCount],
        [FindingGroupCount],
        [TotalSuspectCount],
        [SuspectRowCount],
        [Explanation],
        [FindingExplanation],
        [EvidenceSummary],
        [EvidenceOccurrenceCount],
        [OutlierOccurrenceCount],
        [EvidenceTransformCount],
        [OutlierTransformCount],
        [EvidenceConsensusRatio],
        [DominantConsensusRatio],
        [EvidenceOutlierRatio],
        [OutlierRatio],
        [EvidenceQuality],
        [ConfidenceBand],
        [ConfidenceReason],
        [EvidenceDiversitySummary],
        [ConfidenceSummary],
        [DistinctTransformCount],
        [DistinctSourceTransformCount],
        [DistinctSourceObjectCount],
        [DistinctRelationshipPatternCount],
        [EffectiveTransformCount],
        [RecommendedAction],
        [RuntimeCountStatus],
        [GeneratedView],
        [ReviewQuery],
        [DetailQuery],
        [TransformViewQuery],
        [SupportingTransformQuery]
    FROM [dbo].[FindingLog]
    WHERE [RunId] = @RunId
      AND ([RowsReturned] > 0 OR [RowsReturned] IS NULL)
    ORDER BY CASE WHEN [RowsReturned] IS NULL THEN 1 ELSE 0 END, [RowsReturned] DESC, [DQView] ASC;
END;
';
GO

