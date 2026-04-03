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
