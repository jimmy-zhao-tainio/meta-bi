SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW dq.CustomerOrderCoverage
AS
SELECT
    c.CustomerId,
    c.RegionId,
    o.OrderId,
    o.Amount
FROM sales.Customer AS c
LEFT OUTER JOIN sales.[Order] AS o
    ON c.CustomerId = o.CustomerId
   AND c.RegionId = o.RegionId;
GO

