SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW reporting.InvoiceWindow
AS
SELECT
    i.CustomerId,
    i.InvoiceId,
    i.InvoiceDate,
    SUM(i.Amount) OVER (
        PARTITION BY i.CustomerId
        ORDER BY i.InvoiceDate
        ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
    ) AS RunningAmount,
    ROW_NUMBER() OVER (
        PARTITION BY i.CustomerId
        ORDER BY i.InvoiceDate DESC, i.InvoiceId DESC
    ) AS InvoiceSequence
FROM sales.Invoice AS i;
GO

