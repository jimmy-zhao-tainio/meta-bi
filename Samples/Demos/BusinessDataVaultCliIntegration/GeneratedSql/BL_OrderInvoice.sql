-- Deterministic schema script

CREATE TABLE [dbo].[BL_OrderInvoice] (
    [HashKey] binary(16) NOT NULL,
    [OrderHashKey] binary(16) NOT NULL,
    [InvoiceHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BL_OrderInvoice] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_BL_OrderInvoice] UNIQUE ([OrderHashKey] ASC, [InvoiceHashKey] ASC)
);
GO

ALTER TABLE [dbo].[BL_OrderInvoice] WITH CHECK ADD CONSTRAINT [FK_OrderInvoice_Invoice_InvoiceHashKey] FOREIGN KEY([InvoiceHashKey]) REFERENCES [dbo].[BH_Invoice]([HashKey]);
GO

ALTER TABLE [dbo].[BL_OrderInvoice] WITH CHECK ADD CONSTRAINT [FK_OrderInvoice_Order_OrderHashKey] FOREIGN KEY([OrderHashKey]) REFERENCES [dbo].[BH_Order]([HashKey]);
GO

