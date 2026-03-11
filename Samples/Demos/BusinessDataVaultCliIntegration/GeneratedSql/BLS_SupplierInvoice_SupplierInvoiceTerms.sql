-- Deterministic schema script

CREATE TABLE [dbo].[BLS_SupplierInvoice_SupplierInvoiceTerms] (
    [LinkHashKey] binary(16) NOT NULL,
    [PaymentTermsCode] nvarchar(20) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BLS_SupplierInvoice_SupplierInvoiceTerms] PRIMARY KEY CLUSTERED ([LinkHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[BLS_SupplierInvoice_SupplierInvoiceTerms] WITH CHECK ADD CONSTRAINT [FK_SupplierInvoiceTerms_SupplierInvoice] FOREIGN KEY([LinkHashKey]) REFERENCES [dbo].[BL_SupplierInvoice]([HashKey]);
GO

