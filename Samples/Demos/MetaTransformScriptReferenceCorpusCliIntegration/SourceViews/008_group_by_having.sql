CREATE VIEW dbo.v_group_by_having AS
SELECT
    s.CustomerId,
    COUNT(*) AS OrderCount,
    SUM(s.Amount) AS TotalAmount
FROM dbo.Sales AS s
GROUP BY s.CustomerId
HAVING SUM(s.Amount) > 1000;
