-- Deterministic schema script

CREATE TABLE [dbo].[HS_Invoice_InvoiceHeader] (
    [HubHashKey] binary(16) NOT NULL,
    [InvoiceDate] datetime2(7) NOT NULL,
    [InvoiceAmount] decimal(18, 2) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_HS_Invoice_InvoiceHeader] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[HS_Invoice_InvoiceHeader] WITH CHECK ADD CONSTRAINT [FK_HS_Invoice_InvoiceHeader_Invoice] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[H_Invoice]([HashKey]);
GO

