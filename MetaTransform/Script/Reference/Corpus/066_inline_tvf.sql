CREATE FUNCTION dbo.fn_customer_orders
(
    @CustomerId int,
    @FromDate date
)
RETURNS TABLE
AS
RETURN
(
    SELECT
        o.CustomerId,
        o.OrderDate,
        o.OrderAmount
    FROM sales.CustomerOrder AS o
    WHERE o.CustomerId = @CustomerId
      AND o.OrderDate > @FromDate
);
