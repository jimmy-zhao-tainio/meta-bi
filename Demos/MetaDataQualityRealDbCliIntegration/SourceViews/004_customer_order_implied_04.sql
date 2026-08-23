CREATE VIEW [dq].[v_customer_order_implied_04]
AS
SELECT
    c.CompanyId,
    c.CustomerId,
    o.OrderId,
    o.Amount
FROM dqdemo.Customer c
INNER JOIN dqdemo.OrderHeader o
    ON c.CompanyId = o.CompanyId
   AND c.CustomerId = o.CustomerId;
GO
