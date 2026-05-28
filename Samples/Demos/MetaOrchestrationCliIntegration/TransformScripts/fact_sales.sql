CREATE VIEW dbo.v_fact_sales
AS
SELECT
    o.OrderId,
    o.CustomerId,
    o.Amount
FROM dbo.StageOrder AS o
INNER JOIN dbo.DimCustomer AS c
    ON c.CustomerId = o.CustomerId;
GO
