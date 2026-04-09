CREATE VIEW dbo.v_remaining_aggregate_functions AS
SELECT
    s.CustomerId,
    COUNT_BIG(*) AS RowCountBig,
    MIN(s.Amount) AS MinAmount,
    CHECKSUM_AGG(s.OrderId) AS OrderChecksum,
    STRING_AGG(s.CategoryCode, ',') WITHIN GROUP (ORDER BY s.CategoryCode) AS CategoryList,
    STDEV(s.Amount) AS AmountStdev,
    STDEVP(s.Amount) AS AmountStdevp,
    VAR(s.Amount) AS AmountVar,
    VARP(s.Amount) AS AmountVarp,
    APPROX_COUNT_DISTINCT(s.CategoryCode) AS DistinctCategoryEstimate
FROM dbo.Sales AS s
GROUP BY s.CustomerId;
