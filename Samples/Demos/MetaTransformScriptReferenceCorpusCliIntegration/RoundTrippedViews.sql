CREATE VIEW dbo.v_basic
AS
SELECT
    s.CustomerId,
    s.CustomerName AS CustomerName,
    s.CreatedAt AS CreatedAtAlias,
    'literal' AS LiteralValue
FROM dbo.SourceTable AS s
GO

CREATE VIEW dbo.v_select_star
AS
SELECT
    c.*,
    o.OrderId
FROM dbo.Customers AS c
INNER JOIN dbo.Orders AS o
    ON o.CustomerId = c.CustomerId
GO

CREATE VIEW dbo.v_join_variants
AS
SELECT
    a.Id,
    b.Name,
    c.Flag,
    e.Code
FROM dbo.A AS a
INNER JOIN dbo.B AS b
    ON a.BId = b.Id
LEFT OUTER JOIN dbo.C AS c
    ON c.AId = a.Id
RIGHT OUTER JOIN dbo.D AS d
    ON d.Id = a.DId
FULL OUTER JOIN dbo.E AS e
    ON e.Id = d.EId
CROSS JOIN dbo.F AS f
GO

CREATE VIEW dbo.v_apply_sources
AS
SELECT
    s.Id,
    splitItem.ValueText,
    applySource.MaxAmount
FROM dbo.SourceTable AS s
CROSS APPLY dbo.fnSplit(s.CsvValue) AS splitItem
OUTER APPLY (
SELECT
    MAX(o.Amount) AS MaxAmount
FROM dbo.Orders AS o
WHERE o.SourceId = s.Id
) AS applySource
GO

CREATE VIEW dbo.v_pivot
AS
SELECT
    p.CustomerId,
    p.[A],
    p.[B]
FROM (
SELECT
    s.CustomerId,
    s.CategoryCode,
    s.Amount
FROM dbo.Sales AS s
) AS src
PIVOT
(
    SUM(Amount)
    FOR CategoryCode IN ([A], [B])
) AS p
GO

CREATE VIEW dbo.v_unpivot
AS
SELECT
    u.CustomerId,
    u.CategoryCode,
    u.Amount
FROM (
SELECT
    p.CustomerId,
    p.[A],
    p.[B]
FROM dbo.PivotSource AS p
) AS src
UNPIVOT
(
    Amount FOR CategoryCode IN ([A], [B])
) AS u
GO

CREATE VIEW dbo.v_where_predicates
AS
SELECT
    s.Id
FROM dbo.Source AS s
WHERE NOT (s.Status = 'X' OR s.Amount BETWEEN 10 AND 20 OR s.Code IN ('A', 'B', 'C') OR s.Name LIKE 'AB%' OR s.DeletedAt IS NULL) AND s.Score > 0
GO

CREATE VIEW dbo.v_group_by_having
AS
SELECT
    s.CustomerId,
    COUNT(*) AS OrderCount,
    SUM(s.Amount) AS TotalAmount
FROM dbo.Sales AS s
GROUP BY s.CustomerId
HAVING SUM(s.Amount) > 1000
GO

CREATE VIEW dbo.v_grouping_sets
AS
SELECT
    s.RegionId,
    s.CustomerId,
    SUM(s.Amount) AS TotalAmount,
    GROUPING(s.RegionId) AS GroupingRegionId,
    GROUPING_ID(s.RegionId, s.CustomerId) AS GroupingIdValue
FROM dbo.Sales AS s
GROUP BY GROUPING SETS ((s.RegionId, s.CustomerId), (s.RegionId), ())
GO

CREATE VIEW dbo.v_rollup_cube
AS
SELECT
    s.RegionId,
    s.CustomerId,
    SUM(s.Amount) AS TotalAmount
FROM dbo.Sales AS s
GROUP BY ROLLUP (s.RegionId, s.CustomerId)
UNION ALL
SELECT
    s.RegionId,
    s.CustomerId,
    SUM(s.Amount) AS TotalAmount
FROM dbo.Sales AS s
GROUP BY CUBE (s.RegionId, s.CustomerId)
GO

CREATE VIEW dbo.v_subqueries_and_correlation
AS
SELECT
    c.CustomerId,
    (SELECT
    MAX(o.Amount)
FROM dbo.Orders AS o
WHERE o.CustomerId = c.CustomerId) AS MaxAmount,
    CASE
    WHEN EXISTS (SELECT
    1
FROM dbo.Orders AS o2
WHERE o2.CustomerId = c.CustomerId) THEN 1
    ELSE 0
END AS HasOrders
FROM dbo.Customers AS c
GO

CREATE VIEW dbo.v_subquery_predicates
AS
SELECT
    s.Id
FROM dbo.Source AS s
WHERE s.Amount > ALL (SELECT
    t.Amount
FROM dbo.Target AS t
WHERE t.GroupId = s.GroupId) AND s.Code = ANY (SELECT
    t.Code
FROM dbo.Target AS t
WHERE t.GroupId = s.GroupId) AND s.Id IN (SELECT
    t.SourceId
FROM dbo.Target AS t)
GO

CREATE VIEW dbo.v_set_operations
AS
SELECT
    a.Id,
    a.Code
FROM dbo.A AS a
UNION ALL
SELECT
    b.Id,
    b.Code
FROM dbo.B AS b
UNION
SELECT
    c.Id,
    c.Code
FROM dbo.C AS c
EXCEPT
SELECT
    d.Id,
    d.Code
FROM dbo.D AS d
INTERSECT
SELECT
    e.Id,
    e.Code
FROM dbo.E AS e
GO

CREATE VIEW dbo.v_value_expressions
AS
SELECT
    s.Id,
    CASE s.Status
    WHEN 'A' THEN 'Active'
    WHEN 'I' THEN 'Inactive'
    ELSE 'Other'
END AS StatusText,
    CASE
    WHEN s.Amount > 100 THEN 'High'
    ELSE 'Low'
END AS AmountBand,
    COALESCE(s.PreferredName, s.LegalName, 'Unknown') AS DisplayName,
    NULLIF(s.Code, '') AS NormalizedCode,
    IIF(s.IsActive = 1, s.Amount, 0) AS ActiveAmount,
    CHOOSE(s.Priority, 'Low', 'Medium', 'High') AS PriorityText,
    CAST(s.Amount AS decimal(18, 2)) AS AmountDecimal,
    TRY_CAST(s.Score AS int) AS ScoreInt,
    CONVERT(varchar(10), s.CreatedAt, 126) AS CreatedAtText,
    TRY_CONVERT(datetime2, s.CreatedAtText, 126) AS ParsedCreatedAt,
    (s.CustomerName COLLATE Latin1_General_100_BIN2) AS CollatedCustomerName
FROM dbo.Source AS s
GO

CREATE VIEW dbo.v_window_functions
AS
SELECT
    s.CustomerId,
    s.OrderId,
    SUM(s.Amount) OVER (PARTITION BY s.CustomerId ORDER BY s.OrderDate ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS RunningAmount,
    AVG(s.Amount) OVER (PARTITION BY s.CustomerId) AS AvgAmount,
    ROW_NUMBER() OVER (PARTITION BY s.CustomerId ORDER BY s.OrderDate DESC) AS RowNum,
    LAG(s.Amount, 1, 0) OVER (PARTITION BY s.CustomerId ORDER BY s.OrderDate) AS PrevAmount
FROM dbo.Sales AS s
GO

CREATE VIEW dbo.v_named_window
AS
SELECT
    s.CustomerId,
    SUM(s.Amount) OVER win AS RunningAmount,
    AVG(s.Amount) OVER win2 AS AvgAmount
FROM dbo.Sales AS s
WINDOW
    win AS (PARTITION BY s.CustomerId ORDER BY s.OrderDate),
    win2 AS (win ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)
GO

CREATE VIEW dbo.v_cte
AS
WITH base_cte AS (SELECT
    s.Id,
    s.ParentId,
    0 AS Depth
FROM dbo.Source AS s
WHERE s.ParentId IS NULL), recursive_cte AS (SELECT
    b.Id,
    b.ParentId,
    b.Depth
FROM base_cte AS b
UNION ALL
SELECT
    s.Id,
    s.ParentId,
    r.Depth + 1
FROM dbo.Source AS s
INNER JOIN recursive_cte AS r
    ON s.ParentId = r.Id)
SELECT
    r.Id,
    r.ParentId,
    r.Depth
FROM recursive_cte AS r
GO

CREATE VIEW dbo.v_ordering_and_top
AS
SELECT TOP (100) PERCENT WITH TIES
    s.Id,
    s.OrderDate
FROM dbo.Source AS s
ORDER BY s.OrderDate DESC, s.Id ASC
GO

CREATE VIEW dbo.v_offset_fetch
AS
SELECT
    s.Id,
    s.OrderDate
FROM dbo.Source AS s
ORDER BY s.OrderDate DESC
OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY
GO

CREATE VIEW dbo.v_xml_namespaces_and_methods
AS
WITH XMLNAMESPACES ('urn:test' AS ns)
SELECT
    s.Id,
    s.XmlPayload.value('(/ns:Root/ns:Id/text())[1]', 'int') AS XmlId,
    s.XmlPayload.query('/ns:Root') AS XmlFragment,
    s.XmlPayload.exist('/ns:Root') AS HasRoot
FROM dbo.XmlSource AS s
GO

CREATE VIEW dbo.v_inline_values
AS
SELECT
    src.Id,
    src.Name
FROM (
VALUES
    (1, 'One'),
    (2, 'Two')
) AS src(Id, Name)
GO

CREATE VIEW dbo.v_table_sample
AS
SELECT
    s.Id
FROM dbo.SampleSource AS s TABLESAMPLE (10 PERCENT) REPEATABLE (123)
GO

CREATE VIEW dbo.v_query_parentheses
AS
(SELECT DISTINCT
    s.Id,
    s.Code
FROM dbo.Source AS s)
UNION ALL
(SELECT DISTINCT
    t.Id,
    t.Code
FROM dbo.Target AS t)
GO

CREATE VIEW dbo.v_distinct_predicate
AS
SELECT
    s.Id
FROM dbo.Source AS s
WHERE s.Code IS DISTINCT FROM s.LegacyCode
GO

CREATE VIEW dbo.v_builtin_table_functions
AS
SELECT
    g.value
FROM GENERATE_SERIES(1, 10) AS g
UNION ALL
SELECT
    x.value
FROM STRING_SPLIT('a,b,c', ',') AS x
GO

CREATE VIEW dbo.v_fulltext
AS
SELECT
    p.ProductId
FROM dbo.Products AS p
WHERE CONTAINS(p.Description, 'bike')
GO

CREATE VIEW dbo.v_fulltext_table
AS
SELECT
    ft.[KEY],
    ft.RANK
FROM CONTAINSTABLE(dbo.Products, Description, 'bike') AS ft
GO

CREATE VIEW dbo.v_literals_and_special_calls
AS
SELECT
    -1 AS NegativeInt,
    +1.25 AS PositiveNumeric,
    1E0 AS RealValue,
    0xCAFE AS BinaryValue,
    NULL AS NullValue,
    PARSE('2026-01-01' AS datetime2 USING 'en-US') AS ParsedDate,
    TRY_PARSE('12.34' AS decimal(10, 2) USING 'en-US') AS TryParsedNumber,
    LEFT('abcdef', 3) AS LeftValue,
    RIGHT('abcdef', 2) AS RightValue,
    CURRENT_TIMESTAMP AS CurrentTimestamp
FROM dbo.Source AS s
GO

CREATE VIEW dbo.v_time_zone_extract
AS
SELECT
    s.Id,
    s.CreatedAt AT TIME ZONE 'UTC' AS CreatedAtUtc,
    EXTRACT(MONTH, s.CreatedAt) AS CreatedMonth
FROM dbo.Source AS s
GO

CREATE VIEW dbo.v_join_parentheses
AS
SELECT
    a.Id,
    c.Name
FROM (dbo.A AS a
INNER JOIN dbo.B AS b
    ON a.BId = b.Id)
LEFT OUTER JOIN dbo.C AS c
    ON c.Id = a.CId
GO

CREATE VIEW dbo.v_sequence_and_globals
AS
SELECT
    NEXT VALUE FOR dbo.Seq AS SeqValue,
    @@SPID AS SessionId,
    CAST('abc' AS varchar(max)) AS MaxText
FROM dbo.Source AS s
GO

CREATE VIEW dbo.v_view_column_list
(
    OutputCustomerId,
    OutputCustomerName
)
AS
SELECT
    s.CustomerId,
    s.CustomerName
FROM dbo.Source AS s
GO
