CREATE VIEW dbo.v_percentile_within_group AS
SELECT
    s.CustomerId,
    PERCENTILE_CONT(0.5) WITHIN GROUP (ORDER BY s.Amount) OVER (PARTITION BY s.CustomerId) AS MedianAmount,
    PERCENTILE_DISC(0.9) WITHIN GROUP (ORDER BY s.Amount) OVER (PARTITION BY s.CustomerId) AS NinetiethPercentile
FROM dbo.Sales AS s;
