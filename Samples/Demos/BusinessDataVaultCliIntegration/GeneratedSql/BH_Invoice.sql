-- Deterministic schema script

CREATE TABLE [dbo].[BH_Invoice] (
    [HashKey] binary(16) NOT NULL,
    [InvoiceIdentifier] nvarchar(40) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BH_Invoice] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_BH_Invoice] UNIQUE ([InvoiceIdentifier] ASC)
);
GO

