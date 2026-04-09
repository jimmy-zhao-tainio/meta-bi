CREATE VIEW dbo.v_remaining_analytic_functions AS
SELECT
    s.CustomerId,
    RANK() OVER (PARTITION BY s.CustomerId ORDER BY s.OrderDate) AS SalesRank,
    DENSE_RANK() OVER (PARTITION BY s.CustomerId ORDER BY s.OrderDate) AS DenseSalesRank,
    NTILE(4) OVER (PARTITION BY s.CustomerId ORDER BY s.OrderDate) AS SalesQuartile,
    LEAD(s.Amount, 1, 0) OVER (PARTITION BY s.CustomerId ORDER BY s.OrderDate) AS NextAmount,
    LAG(s.Amount, 1, 0) OVER (PARTITION BY s.CustomerId ORDER BY s.OrderDate) AS PreviousAmount
FROM dbo.Sales AS s;
