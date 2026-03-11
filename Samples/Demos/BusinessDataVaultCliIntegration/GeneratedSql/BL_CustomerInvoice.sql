-- Deterministic schema script

CREATE TABLE [dbo].[BL_CustomerInvoice] (
    [HashKey] binary(16) NOT NULL,
    [CustomerHashKey] binary(16) NOT NULL,
    [InvoiceHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BL_CustomerInvoice] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_BL_CustomerInvoice] UNIQUE ([CustomerHashKey] ASC, [InvoiceHashKey] ASC)
);
GO

ALTER TABLE [dbo].[BL_CustomerInvoice] WITH CHECK ADD CONSTRAINT [FK_CustomerInvoice_Customer_CustomerHashKey] FOREIGN KEY([CustomerHashKey]) REFERENCES [dbo].[BH_Customer]([HashKey]);
GO

ALTER TABLE [dbo].[BL_CustomerInvoice] WITH CHECK ADD CONSTRAINT [FK_CustomerInvoice_Invoice_InvoiceHashKey] FOREIGN KEY([InvoiceHashKey]) REFERENCES [dbo].[BH_Invoice]([HashKey]);
GO

