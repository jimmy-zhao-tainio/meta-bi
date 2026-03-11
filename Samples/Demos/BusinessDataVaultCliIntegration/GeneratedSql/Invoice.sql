-- Deterministic schema script

CREATE TABLE [dbo].[Invoice] (
    [HashKey] binary(16) NOT NULL,
    [InvoiceIdentifier] nvarchar(40) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_Invoice] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_Invoice] UNIQUE ([InvoiceIdentifier] ASC)
);
GO

