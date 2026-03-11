-- Deterministic schema script

CREATE TABLE [dbo].[L_InvoiceSupplier] (
    [HashKey] binary(16) NOT NULL,
    [InvoiceHashKey] binary(16) NOT NULL,
    [SupplierHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_L_InvoiceSupplier] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_L_InvoiceSupplier] UNIQUE ([InvoiceHashKey] ASC, [SupplierHashKey] ASC)
);
GO

ALTER TABLE [dbo].[L_InvoiceSupplier] WITH CHECK ADD CONSTRAINT [FK_L_InvoiceSupplier_Invoice_InvoiceHashKey] FOREIGN KEY([InvoiceHashKey]) REFERENCES [dbo].[H_Invoice]([HashKey]);
GO

ALTER TABLE [dbo].[L_InvoiceSupplier] WITH CHECK ADD CONSTRAINT [FK_L_InvoiceSupplier_Supplier_SupplierHashKey] FOREIGN KEY([SupplierHashKey]) REFERENCES [dbo].[H_Supplier]([HashKey]);
GO

