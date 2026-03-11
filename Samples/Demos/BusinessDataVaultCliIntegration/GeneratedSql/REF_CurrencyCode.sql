-- Deterministic schema script

CREATE TABLE [dbo].[REF_CurrencyCode] (
    [HashKey] binary(16) NOT NULL,
    [CurrencyCode] nvarchar(3) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_REF_CurrencyCode] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_REF_CurrencyCode] UNIQUE ([CurrencyCode] ASC)
);
GO

