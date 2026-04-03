CREATE VIEW dbo.v_rollup_cube AS
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
GROUP BY CUBE (s.RegionId, s.CustomerId);
