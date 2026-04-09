SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW sales.CustomerOrderSummary
(
    CustomerId,
    CustomerName,
    OrderCount,
    TotalAmount
)
AS
WITH CompletedOrders AS
(
    SELECT
        c.CustomerId,
        c.CustomerName,
        o.OrderId,
        o.Amount
    FROM sales.Customer AS c
    INNER JOIN sales.[Order] AS o
        ON o.CustomerId = c.CustomerId
    WHERE o.Status = 'Completed'
)
SELECT
    CompletedOrders.CustomerId,
    CompletedOrders.CustomerName,
    COUNT(*) AS OrderCount,
    SUM(CompletedOrders.Amount) AS TotalAmount
FROM CompletedOrders
GROUP BY
    CompletedOrders.CustomerId,
    CompletedOrders.CustomerName;
GO

