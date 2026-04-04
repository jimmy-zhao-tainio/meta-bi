CREATE VIEW dbo.v_window_frame_offsets AS
SELECT
    s.CustomerId,
    SUM(s.Amount) OVER (
        ORDER BY s.CustomerId
        ROWS BETWEEN 1 PRECEDING AND 1 FOLLOWING
    ) AS MovingAmount,
    SUM(s.Amount) OVER (
        ORDER BY s.CustomerId
        ROWS 2 PRECEDING
    ) AS RunningAmount
FROM dbo.Sales AS s;
