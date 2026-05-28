CREATE VIEW dbo.v_subqueries_and_correlation AS
SELECT
    c.CustomerId,
    (
        SELECT MAX(o.Amount)
        FROM dbo.Orders AS o
        WHERE o.CustomerId = c.CustomerId
    ) AS MaxAmount,
    CASE
        WHEN EXISTS
        (
            SELECT 1
            FROM dbo.Orders AS o2
            WHERE o2.CustomerId = c.CustomerId
        ) THEN 1
        ELSE 0
    END AS HasOrders
FROM dbo.Customers AS c;
