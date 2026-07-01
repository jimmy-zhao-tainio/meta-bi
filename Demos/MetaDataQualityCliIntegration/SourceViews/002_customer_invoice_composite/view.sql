SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW dq.CustomerInvoiceComposite
AS
WITH InvoiceBase AS
(
    SELECT
        i.CustomerId,
        i.RegionId,
        i.InvoiceId,
        i.Amount
    FROM sales.Invoice AS i
)
SELECT
    c.CustomerId,
    c.RegionId,
    ib.InvoiceId,
    ib.Amount
FROM sales.Customer AS c
INNER JOIN InvoiceBase AS ib
    ON c.CustomerId = ib.CustomerId
   AND c.RegionId = ib.RegionId;
GO

