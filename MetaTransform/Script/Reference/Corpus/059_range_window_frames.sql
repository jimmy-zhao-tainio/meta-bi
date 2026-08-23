CREATE VIEW dbo.v_range_window_frames AS
SELECT
    s.CustomerId,
    SUM(s.Amount) OVER (
        ORDER BY s.CustomerId
        RANGE BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
    ) AS RunningAmount,
    COUNT(*) OVER (
        ORDER BY s.CustomerId
        RANGE CURRENT ROW
    ) AS CurrentRowCount
FROM dbo.Sales AS s;
