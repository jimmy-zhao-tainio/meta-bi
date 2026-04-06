CREATE VIEW dbo.v_analytic_window_functions AS
SELECT
    s.CustomerId,
    FIRST_VALUE(s.Amount) OVER (PARTITION BY s.CustomerId ORDER BY s.OrderDate) AS FirstAmount,
    LAST_VALUE(s.Amount) OVER
    (
        PARTITION BY s.CustomerId
        ORDER BY s.OrderDate
        ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
    ) AS LastAmount,
    CUME_DIST() OVER (PARTITION BY s.CustomerId ORDER BY s.OrderDate) AS CumeDistValue,
    PERCENT_RANK() OVER (PARTITION BY s.CustomerId ORDER BY s.OrderDate) AS PercentRankValue
FROM dbo.Sales AS s;
