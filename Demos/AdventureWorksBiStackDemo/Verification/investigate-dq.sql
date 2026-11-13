-- Read-only probes for the generated review pack. These are investigation
-- evidence, not a replacement acceptance policy or a change to DQ discovery.
SET NOCOUNT ON;

SELECT CandidateKind, COUNT_BIG(*) AS ChecksWithFindings,
    SUM(ResultRowCount) AS FindingGroups
FROM AdventureWorksAnalytics.dq.v_DataQualityReview
WHERE COALESCE(ResultRowCount, RowsReturned, 0) <> 0
GROUP BY CandidateKind;

-- These nullable source relationships explain the five outer-join alerts;
-- orders without a salesperson are reported by both order and line transforms.
SELECT N'Customer.PersonID' AS SourceRelationship, COUNT_BIG(*) AS SourceNullRows
FROM AdventureWorks2022.Sales.Customer WHERE PersonID IS NULL
UNION ALL
SELECT N'Customer.StoreID', COUNT_BIG(*)
FROM AdventureWorks2022.Sales.Customer WHERE StoreID IS NULL
UNION ALL
SELECT N'Product.ProductSubcategoryID', COUNT_BIG(*)
FROM AdventureWorks2022.Production.Product WHERE ProductSubcategoryID IS NULL
UNION ALL
SELECT N'SalesOrderHeader.SalesPersonID', COUNT_BIG(*)
FROM AdventureWorks2022.Sales.SalesOrderHeader WHERE SalesPersonID IS NULL;

-- Compare exact natural-key sets, not only counts of optional gaps.
WITH sourceGaps AS
(
    SELECT N'Customer.PersonID' AS Relationship, CustomerID AS NaturalKey
    FROM AdventureWorks2022.Sales.Customer WHERE PersonID IS NULL
    UNION ALL
    SELECT N'Customer.StoreID', CustomerID
    FROM AdventureWorks2022.Sales.Customer WHERE StoreID IS NULL
    UNION ALL
    SELECT N'Product.ProductSubcategoryID', ProductID
    FROM AdventureWorks2022.Production.Product WHERE ProductSubcategoryID IS NULL
    UNION ALL
    SELECT N'SalesOrderHeader.SalesPersonID', SalesOrderID
    FROM AdventureWorks2022.Sales.SalesOrderHeader WHERE SalesPersonID IS NULL
), loadedGaps AS
(
    SELECT N'Customer.PersonID' AS Relationship, customer.CustomerID AS NaturalKey
    FROM AdventureWorksBusinessVault.dbo.BH_Customer AS customer
    WHERE NOT EXISTS (SELECT 1 FROM AdventureWorksBusinessVault.dbo.BL_CustomerPerson AS link
        WHERE link.CustomerHashKey = customer.HashKey)
    UNION ALL
    SELECT N'Customer.StoreID', customer.CustomerID
    FROM AdventureWorksBusinessVault.dbo.BH_Customer AS customer
    WHERE NOT EXISTS (SELECT 1 FROM AdventureWorksBusinessVault.dbo.BL_CustomerStore AS link
        WHERE link.CustomerHashKey = customer.HashKey)
    UNION ALL
    SELECT N'Product.ProductSubcategoryID', product.ProductID
    FROM AdventureWorksBusinessVault.dbo.BH_Product AS product
    WHERE NOT EXISTS (SELECT 1 FROM AdventureWorksBusinessVault.dbo.BL_ProductSubcategoryAssignment AS link
        WHERE link.ProductHashKey = product.HashKey)
    UNION ALL
    SELECT N'SalesOrderHeader.SalesPersonID', salesOrder.SalesOrderID
    FROM AdventureWorksBusinessVault.dbo.BH_SalesOrder AS salesOrder
    WHERE NOT EXISTS (SELECT 1 FROM AdventureWorksBusinessVault.dbo.BL_SalesOrderSalesPerson AS link
        WHERE link.SalesOrderHashKey = salesOrder.HashKey)
), differences AS
(
    SELECT N'Source gap missing after load' AS Difference, Relationship, NaturalKey
    FROM (SELECT Relationship, NaturalKey FROM sourceGaps
        EXCEPT SELECT Relationship, NaturalKey FROM loadedGaps) AS missing
    UNION ALL
    SELECT N'Additional gap after load', Relationship, NaturalKey
    FROM (SELECT Relationship, NaturalKey FROM loadedGaps
        EXCEPT SELECT Relationship, NaturalKey FROM sourceGaps) AS extra
)
SELECT N'Exact optional-gap natural-key sets' AS Probe, COUNT_BIG(*) AS Mismatches
FROM differences;

-- Explicit relationships from the authored demo transforms. Probe the lookup
-- direction rather than guessing it from the order of operands in an equality.
-- Satellite lookups are evaluated at VersionRank=1, as in those transforms.
DECLARE @Relationships TABLE
(
    DetailTable sysname, DetailColumn sysname,
    LookupTable sysname, LookupColumn sysname, LatestVersion bit
);
INSERT @Relationships VALUES
    (N'BL_AddressStateProvince', N'StateProvinceHashKey', N'BL_StateProvinceCountryRegion', N'StateProvinceHashKey', 0),
    (N'BL_AddressStateProvince', N'StateProvinceHashKey', N'BHS_StateProvince_StateProvinceProfile', N'HubHashKey', 1),
    (N'BL_StateProvinceCountryRegion', N'CountryRegionHashKey', N'BH_CountryRegion', N'HashKey', 0),
    (N'BL_ProductSubcategoryAssignment', N'ProductSubcategoryHashKey', N'BL_ProductSubcategoryCategory', N'ProductSubcategoryHashKey', 0),
    (N'BL_SalesPersonPerson', N'PersonHashKey', N'BHS_Person_PersonProfile', N'HubHashKey', 1),
    (N'BL_CustomerPerson', N'PersonHashKey', N'BHS_Person_PersonProfile', N'HubHashKey', 1),
    (N'BL_SalesTerritoryCountryRegion', N'CountryRegionHashKey', N'BH_CountryRegion', N'HashKey', 0),
    (N'BL_SalesOrderLineSalesOrder', N'SalesOrderHashKey', N'BH_SalesOrder', N'HashKey', 0),
    (N'BL_SalesOrderLineProduct', N'ProductHashKey', N'BH_Product', N'HashKey', 0),
    (N'BL_SalesOrderCustomer', N'CustomerHashKey', N'BH_Customer', N'HashKey', 0),
    (N'BL_SalesOrderSalesPerson', N'SalesPersonHashKey', N'BH_SalesPerson', N'HashKey', 0),
    (N'BL_SalesOrderSalesTerritory', N'SalesTerritoryHashKey', N'BH_SalesTerritory', N'HashKey', 0),
    (N'BL_SalesOrderShipToAddress', N'ShipToAddressHashKey', N'BH_Address', N'HashKey', 0),
    (N'BL_SalesOrderBillToAddress', N'BillToAddressHashKey', N'BH_Address', N'HashKey', 0),
    (N'BL_SalesPersonQuotaSalesPerson', N'SalesPersonHashKey', N'BH_SalesPerson', N'HashKey', 0);

DECLARE @DetailTable sysname, @DetailColumn sysname,
    @LookupTable sysname, @LookupColumn sysname, @LatestVersion bit,
    @Sql nvarchar(max), @LookupSql nvarchar(max);
DECLARE relationships CURSOR LOCAL FAST_FORWARD FOR
    SELECT DetailTable, DetailColumn, LookupTable, LookupColumn, LatestVersion
    FROM @Relationships ORDER BY DetailTable, LookupTable;
OPEN relationships;
FETCH NEXT FROM relationships INTO @DetailTable, @DetailColumn, @LookupTable, @LookupColumn, @LatestVersion;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @LookupSql = N'SELECT *, ROW_NUMBER() OVER (PARTITION BY '
        + QUOTENAME(@LookupColumn) + N' ORDER BY LoadTimestamp DESC, AuditId DESC) AS VersionRank FROM '
        + N'AdventureWorksBusinessVault.dbo.' + QUOTENAME(@LookupTable);
    IF @LatestVersion = 0
        SET @LookupSql = N'SELECT *, CONVERT(bigint, 1) AS VersionRank FROM AdventureWorksBusinessVault.dbo.'
            + QUOTENAME(@LookupTable);
    SET @Sql = N'WITH lookupRows AS (' + @LookupSql + N')
SELECT @Relationship AS Relationship,
    (SELECT COUNT_BIG(*) FROM AdventureWorksBusinessVault.dbo.' + QUOTENAME(@DetailTable) + N' AS detail
     WHERE NOT EXISTS (SELECT 1 FROM lookupRows AS lookupRow
        WHERE lookupRow.VersionRank = 1 AND lookupRow.' + QUOTENAME(@LookupColumn)
        + N' = detail.' + QUOTENAME(@DetailColumn) + N')) AS MissingLookupRows,
    (SELECT COUNT_BIG(*) FROM (SELECT ' + QUOTENAME(@LookupColumn) + N' FROM lookupRows
     WHERE VersionRank = 1 GROUP BY ' + QUOTENAME(@LookupColumn) + N' HAVING COUNT_BIG(*) > 1) AS duplicates) AS DuplicateLookupKeys;';
    DECLARE @Relationship nvarchar(512) = @DetailTable + N'.' + @DetailColumn + N' -> ' + @LookupTable + N'.' + @LookupColumn;
    EXEC sys.sp_executesql @Sql, N'@Relationship nvarchar(512)', @Relationship;
    FETCH NEXT FROM relationships INTO @DetailTable, @DetailColumn, @LookupTable, @LookupColumn, @LatestVersion;
END;
CLOSE relationships;
DEALLOCATE relationships;

-- Actual output grain: grouping a foreign key counts reuse, not output copies.
SELECT N'Fact_SalesLine' AS OutputTable, COUNT_BIG(*) AS OutputRows,
    (SELECT COUNT_BIG(*) FROM (SELECT SalesOrderID, SalesOrderDetailID
        FROM AdventureWorksAnalytics.dw.Fact_SalesLine
        GROUP BY SalesOrderID, SalesOrderDetailID) AS grains) AS DistinctGrains
FROM AdventureWorksAnalytics.dw.Fact_SalesLine;

SELECT CandidateId, TransformViews, ResultRowCount, TotalSuspectCount
FROM AdventureWorksAnalytics.dq.v_DataQualityReview
WHERE COALESCE(ResultRowCount, RowsReturned, 0) <> 0
ORDER BY CandidateKind, CandidateId;
