CREATE VIEW dbo.v_grouping_sets AS
SELECT
    s.RegionId,
    s.CustomerId,
    SUM(s.Amount) AS TotalAmount,
    GROUPING(s.RegionId) AS GroupingRegionId,
    GROUPING_ID(s.RegionId, s.CustomerId) AS GroupingIdValue
FROM dbo.Sales AS s
GROUP BY GROUPING SETS
(
    (s.RegionId, s.CustomerId),
    (s.RegionId),
    ()
);
