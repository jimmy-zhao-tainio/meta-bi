CREATE VIEW dbo.v_aggregate_distinct AS
SELECT
    s.CustomerId,
    COUNT(DISTINCT s.CategoryCode) AS DistinctCategoryCount,
    SUM(DISTINCT s.Amount) AS DistinctAmountTotal
FROM dbo.Sales AS s
GROUP BY s.CustomerId;
