-- Deterministic schema script

CREATE TABLE [dbo].[CurrencyCode] (
    [HashKey] binary(16) NOT NULL,
    [CurrencyCode] nvarchar(3) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_CurrencyCode] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_CurrencyCode] UNIQUE ([CurrencyCode] ASC)
);
GO

