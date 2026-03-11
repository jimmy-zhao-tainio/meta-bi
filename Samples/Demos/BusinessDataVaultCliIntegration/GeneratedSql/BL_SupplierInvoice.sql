-- Deterministic schema script

CREATE TABLE [dbo].[BL_SupplierInvoice] (
    [HashKey] binary(16) NOT NULL,
    [SupplierHashKey] binary(16) NOT NULL,
    [InvoiceHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BL_SupplierInvoice] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_BL_SupplierInvoice] UNIQUE ([SupplierHashKey] ASC, [InvoiceHashKey] ASC)
);
GO

ALTER TABLE [dbo].[BL_SupplierInvoice] WITH CHECK ADD CONSTRAINT [FK_SupplierInvoice_Invoice_InvoiceHashKey] FOREIGN KEY([InvoiceHashKey]) REFERENCES [dbo].[BH_Invoice]([HashKey]);
GO

ALTER TABLE [dbo].[BL_SupplierInvoice] WITH CHECK ADD CONSTRAINT [FK_SupplierInvoice_Supplier_SupplierHashKey] FOREIGN KEY([SupplierHashKey]) REFERENCES [dbo].[BH_Supplier]([HashKey]);
GO

