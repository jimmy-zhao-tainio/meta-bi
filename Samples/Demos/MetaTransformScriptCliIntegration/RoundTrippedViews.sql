CREATE VIEW sales.CustomerOrderSummary
(
    CustomerId,
    CustomerName,
    OrderCount,
    TotalAmount
)
AS
WITH CompletedOrders AS (SELECT
    c.CustomerId,
    c.CustomerName,
    o.OrderId,
    o.Amount
FROM sales.Customer AS c
INNER JOIN sales.[Order] AS o
    ON o.CustomerId = c.CustomerId
WHERE o.Status = 'Completed')
SELECT
    CompletedOrders.CustomerId,
    CompletedOrders.CustomerName,
    COUNT(*) AS OrderCount,
    SUM(CompletedOrders.Amount) AS TotalAmount
FROM CompletedOrders
GROUP BY CompletedOrders.CustomerId, CompletedOrders.CustomerName
GO

CREATE VIEW reporting.InvoiceWindow
AS
SELECT
    i.CustomerId,
    i.InvoiceId,
    i.InvoiceDate,
    SUM(i.Amount) OVER (PARTITION BY i.CustomerId ORDER BY i.InvoiceDate ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS RunningAmount,
    ROW_NUMBER() OVER (PARTITION BY i.CustomerId ORDER BY i.InvoiceDate DESC, i.InvoiceId DESC) AS InvoiceSequence
FROM sales.Invoice AS i
GO
