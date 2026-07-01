CREATE VIEW dbo.v_select_star AS
SELECT
    c.*,
    o.OrderId
FROM dbo.Customers AS c
INNER JOIN dbo.Orders AS o
    ON o.CustomerId = c.CustomerId;
