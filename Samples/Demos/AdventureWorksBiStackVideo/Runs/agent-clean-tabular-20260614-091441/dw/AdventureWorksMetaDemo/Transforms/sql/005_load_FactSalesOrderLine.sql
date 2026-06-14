CREATE OR ALTER VIEW awbi.v_load_FactSalesOrderLine
AS
SELECT
    sol.SalesOrderId,
    sol.SalesOrderDetailId,
    so.SalesOrderNumber,
    od.OrderDate,
    p.ProductId,
    pc.ProductNumber,
    pc.ProductName,
    pc.ProductCategory,
    pc.ProductSubcategory,
    c.CustomerId,
    cp.CustomerType,
    st.TerritoryId,
    stp.TerritoryName,
    stp.TerritoryGroup,
    sp.SalesPersonBusinessEntityId,
    spp.SalesPersonName,
    so.OnlineOrderFlag,
    m.OrderQuantity,
    m.UnitPrice,
    m.LineTotal AS SalesAmount,
    m.DiscountAmount,
    m.TaxAmount,
    m.FreightAmount
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineProduct] AS lop
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BLS_SalesOrderLineProduct_SalesOrderLineMeasures] AS m
    ON m.LinkHashKey = lop.HashKey
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesOrderLine] AS sol
    ON sol.HashKey = lop.SalesOrderLineHashKey
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Product] AS p
    ON p.HashKey = lop.ProductHashKey
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BHS_Product_ProductClassification] AS pc
    ON pc.HubHashKey = p.HashKey
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineOrder] AS loo
    ON loo.SalesOrderLineHashKey = sol.HashKey
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS soHub
    ON soHub.HashKey = loo.SalesOrderHashKey
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BHS_SalesOrder_SalesOrderProfile] AS so
    ON so.HubHashKey = soHub.HashKey
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderDate] AS sod
    ON sod.SalesOrderHashKey = soHub.HashKey
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_OrderDate] AS od
    ON od.HashKey = sod.OrderDateHashKey
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderCustomer] AS soc
    ON soc.SalesOrderHashKey = soHub.HashKey
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS c
    ON c.HashKey = soc.CustomerHashKey
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BHS_Customer_CustomerProfile] AS cp
    ON cp.HubHashKey = c.HashKey
LEFT JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderTerritory] AS sot
    ON sot.SalesOrderHashKey = soHub.HashKey
LEFT JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS st
    ON st.HashKey = sot.SalesTerritoryHashKey
LEFT JOIN [AdventureWorksBusinessVault].[dbo].[BHS_SalesTerritory_SalesTerritoryProfile] AS stp
    ON stp.HubHashKey = st.HashKey
LEFT JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesPerson] AS sosp
    ON sosp.SalesOrderHashKey = soHub.HashKey
LEFT JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS sp
    ON sp.HashKey = sosp.SalesPersonHashKey
LEFT JOIN [AdventureWorksBusinessVault].[dbo].[BHS_SalesPerson_SalesPersonProfile] AS spp
    ON spp.HubHashKey = sp.HashKey;
