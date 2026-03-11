-- Deterministic schema script

CREATE TABLE [dbo].[OrderInvoice] (
    [HashKey] binary(16) NOT NULL,
    [OrderHashKey] binary(16) NOT NULL,
    [InvoiceHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_OrderInvoice] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_OrderInvoice] UNIQUE ([OrderHashKey] ASC, [InvoiceHashKey] ASC)
);
GO

ALTER TABLE [dbo].[OrderInvoice] WITH CHECK ADD CONSTRAINT [FK_OrderInvoice_Invoice_InvoiceHashKey] FOREIGN KEY([InvoiceHashKey]) REFERENCES [dbo].[Invoice]([HashKey]);
GO

ALTER TABLE [dbo].[OrderInvoice] WITH CHECK ADD CONSTRAINT [FK_OrderInvoice_Order_OrderHashKey] FOREIGN KEY([OrderHashKey]) REFERENCES [dbo].[Order]([HashKey]);
GO

