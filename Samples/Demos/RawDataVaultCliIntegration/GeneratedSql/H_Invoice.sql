-- Deterministic schema script

CREATE TABLE [dbo].[H_Invoice] (
    [HashKey] binary(16) NOT NULL,
    [InvoiceId] nvarchar(40) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_H_Invoice] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_H_Invoice] UNIQUE ([InvoiceId] ASC)
);
GO

