CREATE VIEW dbo.v_named_window AS
SELECT
    s.CustomerId,
    SUM(s.Amount) OVER win AS RunningAmount,
    AVG(s.Amount) OVER win2 AS AvgAmount
FROM dbo.Sales AS s
WINDOW
    win AS (PARTITION BY s.CustomerId ORDER BY s.OrderDate),
    win2 AS (win ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW);
