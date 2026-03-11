-- Deterministic schema script

CREATE TABLE [dbo].[BLS_CustomerInvoice_CustomerInvoiceStatus] (
    [LinkHashKey] binary(16) NOT NULL,
    [BillingStatus] nvarchar(30) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BLS_CustomerInvoice_CustomerInvoiceStatus] PRIMARY KEY CLUSTERED ([LinkHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[BLS_CustomerInvoice_CustomerInvoiceStatus] WITH CHECK ADD CONSTRAINT [FK_CustomerInvoiceStatus_CustomerInvoice] FOREIGN KEY([LinkHashKey]) REFERENCES [dbo].[BL_CustomerInvoice]([HashKey]);
GO

