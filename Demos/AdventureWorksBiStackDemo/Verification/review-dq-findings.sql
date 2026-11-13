-- Read-only review of the 45 nonempty candidates in this recorded demo run.
-- Candidate IDs identify evidence being investigated, not an acceptance whitelist.
-- Expected business keys and counts are computed independently from the OLTP source.
SET NOCOUNT ON;
DECLARE @Expected TABLE (Scenario nvarchar(64), NaturalKey nvarchar(20), SuspectCount bigint);
INSERT @Expected
SELECT N'UnusedState', CONVERT(nvarchar(20), s.StateProvinceID), 1
FROM AdventureWorks2022.Person.StateProvince s WHERE NOT EXISTS
    (SELECT 1 FROM AdventureWorks2022.Person.Address a WHERE a.StateProvinceID=s.StateProvinceID)
UNION ALL
SELECT N'UnusedPersonForSalesperson', CONVERT(nvarchar(20), p.BusinessEntityID), 1
FROM AdventureWorks2022.Person.Person p WHERE NOT EXISTS
    (SELECT 1 FROM AdventureWorks2022.Sales.SalesPerson s WHERE s.BusinessEntityID=p.BusinessEntityID)
UNION ALL
SELECT N'UnusedPersonForCustomer', CONVERT(nvarchar(20), p.BusinessEntityID), 1
FROM AdventureWorks2022.Person.Person p WHERE NOT EXISTS
    (SELECT 1 FROM AdventureWorks2022.Sales.Customer c WHERE c.PersonID=p.BusinessEntityID)
UNION ALL
SELECT N'UnusedCountryForTerritory', c.CountryRegionCode, 1
FROM AdventureWorks2022.Person.CountryRegion c WHERE NOT EXISTS
    (SELECT 1 FROM AdventureWorks2022.Sales.SalesTerritory t WHERE t.CountryRegionCode=c.CountryRegionCode)
UNION ALL
SELECT N'UnusedProduct', CONVERT(nvarchar(20), p.ProductID), 1
FROM AdventureWorks2022.Production.Product p WHERE NOT EXISTS
    (SELECT 1 FROM AdventureWorks2022.Sales.SalesOrderDetail d WHERE d.ProductID=p.ProductID)
UNION ALL
SELECT N'UnusedCustomer', CONVERT(nvarchar(20), c.CustomerID), 1
FROM AdventureWorks2022.Sales.Customer c WHERE NOT EXISTS
    (SELECT 1 FROM AdventureWorks2022.Sales.SalesOrderHeader h WHERE h.CustomerID=c.CustomerID)
UNION ALL
SELECT N'UnusedShipAddress', CONVERT(nvarchar(20), a.AddressID), 1
FROM AdventureWorks2022.Person.Address a WHERE NOT EXISTS
    (SELECT 1 FROM AdventureWorks2022.Sales.SalesOrderHeader h WHERE h.ShipToAddressID=a.AddressID)
UNION ALL
SELECT N'UnusedBillAddress', CONVERT(nvarchar(20), a.AddressID), 1
FROM AdventureWorks2022.Person.Address a WHERE NOT EXISTS
    (SELECT 1 FROM AdventureWorks2022.Sales.SalesOrderHeader h WHERE h.BillToAddressID=a.AddressID)
UNION ALL
SELECT N'UnusedCountryForState', c.CountryRegionCode, 1
FROM AdventureWorks2022.Person.CountryRegion c WHERE NOT EXISTS
    (SELECT 1 FROM AdventureWorks2022.Person.StateProvince s WHERE s.CountryRegionCode=c.CountryRegionCode)
UNION ALL
SELECT N'NoPerson', CONVERT(nvarchar(20), CustomerID), 1
FROM AdventureWorks2022.Sales.Customer WHERE PersonID IS NULL
UNION ALL
SELECT N'NoStore', CONVERT(nvarchar(20), CustomerID), 1
FROM AdventureWorks2022.Sales.Customer WHERE StoreID IS NULL
UNION ALL
SELECT N'NoSubcategory', CONVERT(nvarchar(20), ProductID), 1
FROM AdventureWorks2022.Production.Product WHERE ProductSubcategoryID IS NULL
UNION ALL
SELECT N'NoSalesperson', CONVERT(nvarchar(20), SalesOrderID), 1
FROM AdventureWorks2022.Sales.SalesOrderHeader WHERE SalesPersonID IS NULL;

INSERT @Expected
SELECT N'AddressesPerState', CONVERT(nvarchar(20), StateProvinceID), COUNT_BIG(*)
FROM AdventureWorks2022.Person.Address GROUP BY StateProvinceID HAVING COUNT_BIG(*)>1
UNION ALL
SELECT N'ProductsPerSubcategory', CONVERT(nvarchar(20), ProductSubcategoryID), COUNT_BIG(*)
FROM AdventureWorks2022.Production.Product WHERE ProductSubcategoryID IS NOT NULL
GROUP BY ProductSubcategoryID HAVING COUNT_BIG(*)>1
UNION ALL
SELECT N'LinesPerOrder', CONVERT(nvarchar(20), SalesOrderID), COUNT_BIG(*)
FROM AdventureWorks2022.Sales.SalesOrderDetail GROUP BY SalesOrderID HAVING COUNT_BIG(*)>1
UNION ALL
SELECT N'LinesPerProduct', CONVERT(nvarchar(20), ProductID), COUNT_BIG(*)
FROM AdventureWorks2022.Sales.SalesOrderDetail GROUP BY ProductID HAVING COUNT_BIG(*)>1
UNION ALL
SELECT N'OrdersPerCustomer', CONVERT(nvarchar(20), CustomerID), COUNT_BIG(*)
FROM AdventureWorks2022.Sales.SalesOrderHeader GROUP BY CustomerID HAVING COUNT_BIG(*)>1
UNION ALL
SELECT N'OrdersPerSalesperson', CONVERT(nvarchar(20), SalesPersonID), COUNT_BIG(*)
FROM AdventureWorks2022.Sales.SalesOrderHeader WHERE SalesPersonID IS NOT NULL
GROUP BY SalesPersonID HAVING COUNT_BIG(*)>1
UNION ALL
SELECT N'OrdersPerTerritory', CONVERT(nvarchar(20), TerritoryID), COUNT_BIG(*)
FROM AdventureWorks2022.Sales.SalesOrderHeader WHERE TerritoryID IS NOT NULL
GROUP BY TerritoryID HAVING COUNT_BIG(*)>1
UNION ALL
SELECT N'OrdersPerShipAddress', CONVERT(nvarchar(20), ShipToAddressID), COUNT_BIG(*)
FROM AdventureWorks2022.Sales.SalesOrderHeader GROUP BY ShipToAddressID HAVING COUNT_BIG(*)>1
UNION ALL
SELECT N'OrdersPerBillAddress', CONVERT(nvarchar(20), BillToAddressID), COUNT_BIG(*)
FROM AdventureWorks2022.Sales.SalesOrderHeader GROUP BY BillToAddressID HAVING COUNT_BIG(*)>1
UNION ALL
SELECT N'QuotasPerSalesperson', CONVERT(nvarchar(20), BusinessEntityID), COUNT_BIG(*)
FROM AdventureWorks2022.Sales.SalesPersonQuotaHistory GROUP BY BusinessEntityID HAVING COUNT_BIG(*)>1;

DECLARE @Checks TABLE
(CandidateId nvarchar(256), Scenario nvarchar(64), Hub sysname, NaturalColumn sysname, KeyPrefix sysname);
INSERT @Checks VALUES
(N'JoinOrphan.JoinPattern.10.112', N'UnusedState', N'BH_StateProvince', N'StateProvinceID', N'StateProvinceHashKey'),
(N'JoinOrphan.JoinPattern.9.116', N'UnusedState', N'BH_StateProvince', N'StateProvinceID', N'HubHashKey'),
(N'JoinOrphan.JoinPattern.17.154', N'UnusedPersonForSalesperson', N'BH_Person', N'BusinessEntityID', N'HubHashKey'),
(N'JoinOrphan.JoinPattern.2.130', N'UnusedPersonForCustomer', N'BH_Person', N'BusinessEntityID', N'HubHashKey'),
(N'JoinOrphan.JoinPattern.20.160', N'UnusedCountryForTerritory', N'BH_CountryRegion', N'CountryRegionCode', N'HashKey'),
(N'JoinOrphan.JoinPattern.26.31', N'UnusedProduct', N'BH_Product', N'ProductID', N'HashKey'),
(N'JoinOrphan.JoinPattern.29.23', N'UnusedCustomer', N'BH_Customer', N'CustomerID', N'HashKey'),
(N'JoinOrphan.JoinPattern.44.40', N'UnusedCustomer', N'BH_Customer', N'CustomerID', N'HashKey'),
(N'JoinOrphan.JoinPattern.38.9', N'UnusedShipAddress', N'BH_Address', N'AddressID', N'HashKey'),
(N'JoinOrphan.JoinPattern.56.2', N'UnusedShipAddress', N'BH_Address', N'AddressID', N'HashKey'),
(N'JoinOrphan.JoinPattern.53.16', N'UnusedBillAddress', N'BH_Address', N'AddressID', N'HashKey'),
(N'JoinOrphan.JoinPattern.5.110', N'UnusedCountryForState', N'BH_CountryRegion', N'CountryRegionCode', N'HashKey'),
(N'OuterJoinNullExpansion.JoinPattern.1.136', N'NoPerson', N'BH_Customer', N'CustomerID', N'HashKey'),
(N'OuterJoinNullExpansion.JoinPattern.3.128', N'NoStore', N'BH_Customer', N'CustomerID', N'HashKey'),
(N'OuterJoinNullExpansion.JoinPattern.12.150', N'NoSubcategory', N'BH_Product', N'ProductID', N'HashKey'),
(N'OuterJoinNullExpansion.JoinPattern.31.86', N'NoSalesperson', N'BH_SalesOrder', N'SalesOrderID', N'HashKey'),
(N'OuterJoinNullExpansion.JoinPattern.46.106', N'NoSalesperson', N'BH_SalesOrder', N'SalesOrderID', N'HashKey');

DECLARE @Reuse TABLE
(PatternId nvarchar(20), MultiplicitySuffix nvarchar(10), DuplicateSuffix nvarchar(10),
 Scenario nvarchar(64), Hub sysname, NaturalColumn sysname, KeyPrefix sysname);
INSERT @Reuse VALUES
(N'10',N'113',N'115',N'AddressesPerState',N'BH_StateProvince',N'StateProvinceID',N'StateProvinceHashKey'),
(N'14',N'143',N'145',N'ProductsPerSubcategory',N'BH_ProductSubcategory',N'ProductSubcategoryID',N'ProductSubcategoryHashKey'),
(N'23',N'50',N'51',N'LinesPerOrder',N'BH_SalesOrder',N'SalesOrderID',N'SalesOrderHashKey'),
(N'26',N'32',N'33',N'LinesPerProduct',N'BH_Product',N'ProductID',N'ProductHashKey'),
(N'29',N'24',N'25',N'OrdersPerCustomer',N'BH_Customer',N'CustomerID',N'CustomerHashKey'),
(N'32',N'81',N'83',N'OrdersPerSalesperson',N'BH_SalesPerson',N'BusinessEntityID',N'SalesPersonHashKey'),
(N'35',N'71',N'73',N'OrdersPerTerritory',N'BH_SalesTerritory',N'TerritoryID',N'SalesTerritoryHashKey'),
(N'38',N'10',N'11',N'OrdersPerShipAddress',N'BH_Address',N'AddressID',N'ShipToAddressHashKey'),
(N'44',N'41',N'42',N'OrdersPerCustomer',N'BH_Customer',N'CustomerID',N'CustomerHashKey'),
(N'47',N'101',N'103',N'OrdersPerSalesperson',N'BH_SalesPerson',N'BusinessEntityID',N'SalesPersonHashKey'),
(N'50',N'91',N'93',N'OrdersPerTerritory',N'BH_SalesTerritory',N'TerritoryID',N'SalesTerritoryHashKey'),
(N'53',N'17',N'18',N'OrdersPerBillAddress',N'BH_Address',N'AddressID',N'BillToAddressHashKey'),
(N'56',N'3',N'4',N'OrdersPerShipAddress',N'BH_Address',N'AddressID',N'ShipToAddressHashKey'),
(N'65',N'60',N'61',N'QuotasPerSalesperson',N'BH_SalesPerson',N'BusinessEntityID',N'SalesPersonHashKey');
INSERT @Checks
SELECT N'JoinMultiplicityExplosion.JoinPattern.'+PatternId+N'.'+MultiplicitySuffix,
    Scenario, Hub, NaturalColumn, KeyPrefix FROM @Reuse
UNION ALL
SELECT N'OutputDuplicateRisk.JoinPattern.'+PatternId+N'.'+DuplicateSuffix,
    Scenario, Hub, NaturalColumn, KeyPrefix FROM @Reuse;

CREATE TABLE #Observed (CandidateId nvarchar(256), NaturalKey nvarchar(20), SuspectCount bigint);
DECLARE @CandidateId nvarchar(256), @Hub sysname, @NaturalColumn sysname, @KeyPrefix sysname, @Sql nvarchar(max);
DECLARE checks CURSOR LOCAL FAST_FORWARD FOR SELECT CandidateId, Hub, NaturalColumn, KeyPrefix FROM @Checks;
OPEN checks;
FETCH NEXT FROM checks INTO @CandidateId,@Hub,@NaturalColumn,@KeyPrefix;
WHILE @@FETCH_STATUS=0
BEGIN
    -- LEFT JOIN retains evidence whose hash key cannot be resolved to a hub.
    -- It must appear as a mismatch, rather than disappearing from the review.
    SET @Sql=N'INSERT #Observed SELECT @CandidateId, CONVERT(nvarchar(20), hub.'
        + QUOTENAME(@NaturalColumn)+N'), finding.SuspectCount FROM AdventureWorksAnalytics.dq.'
        + QUOTENAME(N'v_'+@CandidateId)+N' AS finding LEFT JOIN AdventureWorksBusinessVault.dbo.'
        + QUOTENAME(@Hub)+N' AS hub ON CONVERT(varbinary(max),finding.KeyValues) = CONVERT(varbinary(max),CONCAT(@KeyPrefix,N''='',COALESCE(CONVERT(nvarchar(4000),hub.HashKey),N''<NULL>'')));';
    EXEC sys.sp_executesql @Sql,N'@CandidateId nvarchar(256),@KeyPrefix sysname',@CandidateId,@KeyPrefix;
    FETCH NEXT FROM checks INTO @CandidateId,@Hub,@NaturalColumn,@KeyPrefix;
END;
CLOSE checks;
DEALLOCATE checks;

SELECT c.CandidateId,c.Scenario,
    (SELECT COUNT_BIG(*) FROM @Expected e WHERE e.Scenario=c.Scenario) AS SourceGroups,
    (SELECT COUNT_BIG(*) FROM #Observed o WHERE o.CandidateId=c.CandidateId) AS ObservedGroups,
    (SELECT COUNT_BIG(*) FROM
        (SELECT NaturalKey,SuspectCount FROM @Expected e WHERE e.Scenario=c.Scenario
         EXCEPT SELECT NaturalKey,SuspectCount FROM #Observed o WHERE o.CandidateId=c.CandidateId) missing) AS MissingGroups,
    (SELECT COUNT_BIG(*) FROM
        (SELECT NaturalKey,SuspectCount FROM #Observed o WHERE o.CandidateId=c.CandidateId
         EXCEPT SELECT NaturalKey,SuspectCount FROM @Expected e WHERE e.Scenario=c.Scenario) extra) AS ExtraGroups
FROM @Checks c ORDER BY c.CandidateId;

-- Expose any newly nonempty candidate outside this recorded review.
SELECT r.CandidateId AS NonemptyCandidateOutsideThisReview
FROM AdventureWorksAnalytics.dq.v_DataQualityReview r
WHERE COALESCE(r.ResultRowCount,r.RowsReturned,0)<>0
    AND NOT EXISTS (SELECT 1 FROM @Checks c WHERE c.CandidateId=r.CandidateId);

-- Natural-key examples make the generated hash-key findings inspectable.
WITH examples AS
(
    SELECT c.CandidateId,o.NaturalKey,o.SuspectCount,
        ROW_NUMBER() OVER(PARTITION BY c.CandidateId ORDER BY o.NaturalKey) AS ExampleNumber
    FROM @Checks c JOIN #Observed o ON o.CandidateId=c.CandidateId
)
SELECT CandidateId,NaturalKey,SuspectCount FROM examples WHERE ExampleNumber<=2
ORDER BY CandidateId,ExampleNumber;
DROP TABLE #Observed;
