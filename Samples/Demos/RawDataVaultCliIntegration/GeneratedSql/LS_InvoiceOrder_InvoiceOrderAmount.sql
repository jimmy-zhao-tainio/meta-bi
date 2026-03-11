-- Deterministic schema script

CREATE TABLE [dbo].[LS_InvoiceOrder_InvoiceOrderAmount] (
    [LinkHashKey] binary(16) NOT NULL,
    [InvoiceAmount] decimal(18, 2) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_LS_InvoiceOrder_InvoiceOrderAmount] PRIMARY KEY CLUSTERED ([LinkHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[LS_InvoiceOrder_InvoiceOrderAmount] WITH CHECK ADD CONSTRAINT [FK_LS_InvoiceOrder_InvoiceOrderAmount_InvoiceOrder] FOREIGN KEY([LinkHashKey]) REFERENCES [dbo].[L_InvoiceOrder]([HashKey]);
GO

