-- Deterministic schema script

CREATE TABLE [dbo].[BHS_Invoice_InvoiceHeader] (
    [HubHashKey] binary(16) NOT NULL,
    [InvoiceDate] datetime NOT NULL,
    [InvoiceAmount] decimal(18, 2) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BHS_Invoice_InvoiceHeader] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[BHS_Invoice_InvoiceHeader] WITH CHECK ADD CONSTRAINT [FK_InvoiceHeader_Invoice] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[BH_Invoice]([HashKey]);
GO

