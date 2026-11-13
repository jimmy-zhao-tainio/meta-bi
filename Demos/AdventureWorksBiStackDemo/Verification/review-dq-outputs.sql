-- Independent source-to-output comparisons for the eight transforms named by
-- the nonempty DQ views. Read-only; not a replacement for verify-stack.sql.
-- Text uses binary comparison so collation cannot hide changes. EXCEPT compares
-- NULL as NULL. Audit columns are excluded deliberately.
SET NOCOUNT ON;
DECLARE @Review TABLE
(OutputTable sysname, SourceRows bigint, OutputRows bigint, MissingOrChanged bigint, ExtraOrChanged bigint);

WITH expected AS
(
    SELECT p.ProductID, CONVERT(varbinary(max),p.ProductNumber) AS ProductNumber,
        CONVERT(varbinary(max),p.Name) AS ProductName, CONVERT(varbinary(max),p.Color) AS Color,
        CONVERT(varbinary(max),s.Name) AS SubcategoryName, CONVERT(varbinary(max),c.Name) AS CategoryName
    FROM AdventureWorks2022.Production.Product p
    LEFT JOIN AdventureWorks2022.Production.ProductSubcategory s ON s.ProductSubcategoryID=p.ProductSubcategoryID
    LEFT JOIN AdventureWorks2022.Production.ProductCategory c ON c.ProductCategoryID=s.ProductCategoryID
), actual AS
(
    SELECT ProductID, CONVERT(varbinary(max),ProductNumber) AS ProductNumber,
        CONVERT(varbinary(max),ProductName) AS ProductName, CONVERT(varbinary(max),Color) AS Color,
        CONVERT(varbinary(max),ProductSubcategoryName) AS SubcategoryName,
        CONVERT(varbinary(max),ProductCategoryName) AS CategoryName
    FROM AdventureWorksAnalytics.dw.Dim_Product
)
INSERT @Review SELECT N'Dim_Product',(SELECT COUNT_BIG(*) FROM expected),(SELECT COUNT_BIG(*) FROM actual),
    (SELECT COUNT_BIG(*) FROM (SELECT * FROM expected EXCEPT SELECT * FROM actual) d),
    (SELECT COUNT_BIG(*) FROM (SELECT * FROM actual EXCEPT SELECT * FROM expected) d);

WITH expected AS
(
    SELECT a.AddressID, CONVERT(varbinary(max),a.AddressLine1) AS AddressLine1,
        CONVERT(varbinary(max),a.AddressLine2) AS AddressLine2, CONVERT(varbinary(max),a.City) AS City,
        CONVERT(varbinary(max),a.PostalCode) AS PostalCode, CONVERT(varbinary(max),s.StateProvinceCode) AS StateCode,
        CONVERT(varbinary(max),s.Name) AS StateName, CONVERT(varbinary(max),c.CountryRegionCode) AS CountryCode,
        CONVERT(varbinary(max),c.Name) AS CountryName
    FROM AdventureWorks2022.Person.Address a
    JOIN AdventureWorks2022.Person.StateProvince s ON s.StateProvinceID=a.StateProvinceID
    JOIN AdventureWorks2022.Person.CountryRegion c ON c.CountryRegionCode=s.CountryRegionCode
), actual AS
(
    SELECT AddressID, CONVERT(varbinary(max),AddressLine1) AS AddressLine1,
        CONVERT(varbinary(max),AddressLine2) AS AddressLine2, CONVERT(varbinary(max),City) AS City,
        CONVERT(varbinary(max),PostalCode) AS PostalCode, CONVERT(varbinary(max),StateProvinceCode) AS StateCode,
        CONVERT(varbinary(max),StateProvinceName) AS StateName, CONVERT(varbinary(max),CountryRegionCode) AS CountryCode,
        CONVERT(varbinary(max),CountryRegionName) AS CountryName
    FROM AdventureWorksAnalytics.dw.Dim_Geography
)
INSERT @Review SELECT N'Dim_Geography',(SELECT COUNT_BIG(*) FROM expected),(SELECT COUNT_BIG(*) FROM actual),
    (SELECT COUNT_BIG(*) FROM (SELECT * FROM expected EXCEPT SELECT * FROM actual) d),
    (SELECT COUNT_BIG(*) FROM (SELECT * FROM actual EXCEPT SELECT * FROM expected) d);

WITH expected AS
(
    -- The authored SourceToRaw conversion deliberately changes varchar to
    -- nvarchar. Compare Unicode values after that declared representation change.
    SELECT c.CustomerID, CONVERT(varbinary(max),CONVERT(nvarchar(15),c.AccountNumber)) AS AccountNumber,
        CONVERT(varbinary(max),CASE WHEN s.Name IS NOT NULL THEN s.Name
            WHEN p.FirstName IS NOT NULL OR p.LastName IS NOT NULL THEN CONCAT(p.FirstName,N' ',p.LastName) END) AS CustomerName,
        CONVERT(varbinary(max),CASE WHEN c.StoreID IS NOT NULL THEN N'Store'
            WHEN c.PersonID IS NOT NULL THEN N'Individual' END) AS CustomerType,
        CONVERT(varbinary(max),s.Name) AS StoreName
    FROM AdventureWorks2022.Sales.Customer c
    LEFT JOIN AdventureWorks2022.Person.Person p ON p.BusinessEntityID=c.PersonID
    LEFT JOIN AdventureWorks2022.Sales.Store s ON s.BusinessEntityID=c.StoreID
), actual AS
(
    SELECT CustomerID, CONVERT(varbinary(max),AccountNumber) AS AccountNumber,
        CONVERT(varbinary(max),CustomerName) AS CustomerName, CONVERT(varbinary(max),CustomerType) AS CustomerType,
        CONVERT(varbinary(max),StoreName) AS StoreName FROM AdventureWorksAnalytics.dw.Dim_Customer
)
INSERT @Review SELECT N'Dim_Customer',(SELECT COUNT_BIG(*) FROM expected),(SELECT COUNT_BIG(*) FROM actual),
    (SELECT COUNT_BIG(*) FROM (SELECT * FROM expected EXCEPT SELECT * FROM actual) d),
    (SELECT COUNT_BIG(*) FROM (SELECT * FROM actual EXCEPT SELECT * FROM expected) d);

WITH expected AS
(
    SELECT s.BusinessEntityID, CONVERT(varbinary(max),p.FirstName) AS FirstName,
        CONVERT(varbinary(max),p.LastName) AS LastName,
        CONVERT(varbinary(max),CONCAT(p.FirstName,N' ',p.LastName)) AS FullName
    FROM AdventureWorks2022.Sales.SalesPerson s
    JOIN AdventureWorks2022.Person.Person p ON p.BusinessEntityID=s.BusinessEntityID
), actual AS
(
    SELECT BusinessEntityID, CONVERT(varbinary(max),FirstName) AS FirstName,
        CONVERT(varbinary(max),LastName) AS LastName, CONVERT(varbinary(max),FullName) AS FullName
    FROM AdventureWorksAnalytics.dw.Dim_Salesperson
)
INSERT @Review SELECT N'Dim_Salesperson',(SELECT COUNT_BIG(*) FROM expected),(SELECT COUNT_BIG(*) FROM actual),
    (SELECT COUNT_BIG(*) FROM (SELECT * FROM expected EXCEPT SELECT * FROM actual) d),
    (SELECT COUNT_BIG(*) FROM (SELECT * FROM actual EXCEPT SELECT * FROM expected) d);

WITH expected AS
(
    SELECT TerritoryID, CONVERT(varbinary(max),Name) AS TerritoryName,
        CONVERT(varbinary(max),CountryRegionCode) AS CountryCode, CONVERT(varbinary(max),[Group]) AS TerritoryGroup
    FROM AdventureWorks2022.Sales.SalesTerritory
), actual AS
(
    SELECT TerritoryID, CONVERT(varbinary(max),TerritoryName) AS TerritoryName,
        CONVERT(varbinary(max),CountryRegionCode) AS CountryCode, CONVERT(varbinary(max),TerritoryGroup) AS TerritoryGroup
    FROM AdventureWorksAnalytics.dw.Dim_SalesTerritory
)
INSERT @Review SELECT N'Dim_SalesTerritory',(SELECT COUNT_BIG(*) FROM expected),(SELECT COUNT_BIG(*) FROM actual),
    (SELECT COUNT_BIG(*) FROM (SELECT * FROM expected EXCEPT SELECT * FROM actual) d),
    (SELECT COUNT_BIG(*) FROM (SELECT * FROM actual EXCEPT SELECT * FROM expected) d);

WITH expected AS
(
    SELECT h.SalesOrderID,h.CustomerID,h.SalesPersonID,h.TerritoryID,h.BillToAddressID,h.ShipToAddressID,
        CONVERT(date,h.OrderDate) AS OrderDate,CONVERT(date,h.DueDate) AS DueDate,CONVERT(date,h.ShipDate) AS ShipDate,
        CONVERT(decimal(18,4),h.SubTotal) AS SubTotal,CONVERT(decimal(18,4),h.TaxAmt) AS TaxAmount,
        CONVERT(decimal(18,4),h.Freight) AS FreightAmount,CONVERT(decimal(18,4),h.TotalDue) AS TotalDue
    FROM AdventureWorks2022.Sales.SalesOrderHeader h
), actual AS
(
    SELECT f.SalesOrderID,c.CustomerID,p.BusinessEntityID AS SalesPersonID,t.TerritoryID,
        b.AddressID AS BillToAddressID,s.AddressID AS ShipToAddressID,
        od.CalendarDate AS OrderDate,dd.CalendarDate AS DueDate,sd.CalendarDate AS ShipDate,
        f.SubTotal,f.TaxAmount,f.FreightAmount,f.TotalDue
    FROM AdventureWorksAnalytics.dw.Fact_SalesOrder f
    LEFT JOIN AdventureWorksAnalytics.dw.Dim_Customer c ON c.CustomerKey=f.CustomerKey
    LEFT JOIN AdventureWorksAnalytics.dw.Dim_Salesperson p ON p.SalespersonKey=f.SalespersonKey
    LEFT JOIN AdventureWorksAnalytics.dw.Dim_SalesTerritory t ON t.SalesTerritoryKey=f.SalesTerritoryKey
    LEFT JOIN AdventureWorksAnalytics.dw.Dim_Geography b ON b.GeographyKey=f.BillToGeographyKey
    LEFT JOIN AdventureWorksAnalytics.dw.Dim_Geography s ON s.GeographyKey=f.ShipToGeographyKey
    LEFT JOIN AdventureWorksAnalytics.dw.Dim_Date od ON od.DateKey=f.OrderDateKey
    LEFT JOIN AdventureWorksAnalytics.dw.Dim_Date dd ON dd.DateKey=f.DueDateKey
    LEFT JOIN AdventureWorksAnalytics.dw.Dim_Date sd ON sd.DateKey=f.ShipDateKey
)
INSERT @Review SELECT N'Fact_SalesOrder',(SELECT COUNT_BIG(*) FROM expected),(SELECT COUNT_BIG(*) FROM actual),
    (SELECT COUNT_BIG(*) FROM (SELECT * FROM expected EXCEPT SELECT * FROM actual) d),
    (SELECT COUNT_BIG(*) FROM (SELECT * FROM actual EXCEPT SELECT * FROM expected) d);

WITH expected AS
(
    SELECT d.SalesOrderID,d.SalesOrderDetailID,d.ProductID,h.CustomerID,h.SalesPersonID,h.TerritoryID,h.ShipToAddressID,
        CONVERT(int,d.OrderQty) AS OrderQuantity,CONVERT(decimal(18,4),d.UnitPrice) AS UnitPrice,
        CONVERT(decimal(18,4),CONVERT(decimal(18,4),d.UnitPrice)*CONVERT(decimal(18,4),d.OrderQty)
            *CONVERT(decimal(19,4),d.UnitPriceDiscount)) AS DiscountAmount,
        CONVERT(decimal(18,4),d.LineTotal) AS LineSalesAmount
    FROM AdventureWorks2022.Sales.SalesOrderDetail d
    JOIN AdventureWorks2022.Sales.SalesOrderHeader h ON h.SalesOrderID=d.SalesOrderID
), actual AS
(
    SELECT f.SalesOrderID,f.SalesOrderDetailID,p.ProductID,c.CustomerID,sp.BusinessEntityID AS SalesPersonID,
        t.TerritoryID,g.AddressID AS ShipToAddressID,f.OrderQuantity,f.UnitPrice,f.DiscountAmount,f.LineSalesAmount
    FROM AdventureWorksAnalytics.dw.Fact_SalesLine f
    LEFT JOIN AdventureWorksAnalytics.dw.Dim_Product p ON p.ProductKey=f.ProductKey
    LEFT JOIN AdventureWorksAnalytics.dw.Dim_Customer c ON c.CustomerKey=f.CustomerKey
    LEFT JOIN AdventureWorksAnalytics.dw.Dim_Salesperson sp ON sp.SalespersonKey=f.SalespersonKey
    LEFT JOIN AdventureWorksAnalytics.dw.Dim_SalesTerritory t ON t.SalesTerritoryKey=f.SalesTerritoryKey
    LEFT JOIN AdventureWorksAnalytics.dw.Dim_Geography g ON g.GeographyKey=f.ShipToGeographyKey
)
INSERT @Review SELECT N'Fact_SalesLine',(SELECT COUNT_BIG(*) FROM expected),(SELECT COUNT_BIG(*) FROM actual),
    (SELECT COUNT_BIG(*) FROM (SELECT * FROM expected EXCEPT SELECT * FROM actual) d),
    (SELECT COUNT_BIG(*) FROM (SELECT * FROM actual EXCEPT SELECT * FROM expected) d);

WITH expected AS
(
    SELECT BusinessEntityID,CONVERT(date,QuotaDate) AS QuotaDate,CONVERT(decimal(18,4),SalesQuota) AS QuotaAmount
    FROM AdventureWorks2022.Sales.SalesPersonQuotaHistory
), actual AS
(
    SELECT p.BusinessEntityID,d.CalendarDate AS QuotaDate,f.QuotaAmount
    FROM AdventureWorksAnalytics.dw.Fact_SalespersonQuota f
    LEFT JOIN AdventureWorksAnalytics.dw.Dim_Salesperson p ON p.SalespersonKey=f.SalespersonKey
    LEFT JOIN AdventureWorksAnalytics.dw.Dim_Date d ON d.DateKey=f.QuotaPeriodKey
)
INSERT @Review SELECT N'Fact_SalespersonQuota',(SELECT COUNT_BIG(*) FROM expected),(SELECT COUNT_BIG(*) FROM actual),
    (SELECT COUNT_BIG(*) FROM (SELECT * FROM expected EXCEPT SELECT * FROM actual) d),
    (SELECT COUNT_BIG(*) FROM (SELECT * FROM actual EXCEPT SELECT * FROM expected) d);

SELECT * FROM @Review ORDER BY OutputTable;

-- Isolate the unexpected text differences rather than treating a changed row
-- as proof that every field in that row is wrong.
SELECT N'Dim_Customer.CustomerName' AS Field,
    COUNT_BIG(*) AS ComparedRows,
    SUM(CONVERT(bigint,CASE WHEN compared.ExpectedBytes<>compared.ActualBytes
        OR (compared.ExpectedBytes IS NULL AND compared.ActualBytes IS NOT NULL)
        OR (compared.ExpectedBytes IS NOT NULL AND compared.ActualBytes IS NULL)
        THEN 1 ELSE 0 END)) AS ChangedRows
FROM AdventureWorks2022.Sales.Customer c
LEFT JOIN AdventureWorks2022.Person.Person p ON p.BusinessEntityID=c.PersonID
LEFT JOIN AdventureWorks2022.Sales.Store s ON s.BusinessEntityID=c.StoreID
JOIN AdventureWorksAnalytics.dw.Dim_Customer d ON d.CustomerID=c.CustomerID
CROSS APPLY (VALUES
    (CONVERT(varbinary(max),CASE WHEN s.Name IS NOT NULL THEN s.Name
        WHEN p.FirstName IS NOT NULL OR p.LastName IS NOT NULL THEN CONCAT(p.FirstName,N' ',p.LastName) END),
     CONVERT(varbinary(max),d.CustomerName))) compared(ExpectedBytes,ActualBytes);

SELECT TOP (5) s.BusinessEntityID,p.FirstName,p.LastName,
    CONCAT(p.FirstName,N' ',p.LastName) AS ExpectedFullName,d.FullName AS ActualFullName
FROM AdventureWorks2022.Sales.SalesPerson s
JOIN AdventureWorks2022.Person.Person p ON p.BusinessEntityID=s.BusinessEntityID
JOIN AdventureWorksAnalytics.dw.Dim_Salesperson d ON d.BusinessEntityID=s.BusinessEntityID
WHERE CONVERT(varbinary(max),CONCAT(p.FirstName,N' ',p.LastName))<>CONVERT(varbinary(max),d.FullName)
ORDER BY s.BusinessEntityID;
