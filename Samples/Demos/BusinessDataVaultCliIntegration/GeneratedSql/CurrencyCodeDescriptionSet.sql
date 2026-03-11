-- Deterministic schema script

CREATE TABLE [dbo].[CurrencyCodeDescriptionSet] (
    [ReferenceHashKey] binary(16) NOT NULL,
    [Description] nvarchar(100) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_CurrencyCodeDescriptionSet] PRIMARY KEY CLUSTERED ([ReferenceHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[CurrencyCodeDescriptionSet] WITH CHECK ADD CONSTRAINT [FK_CurrencyCodeDescriptionSet_CurrencyCode] FOREIGN KEY([ReferenceHashKey]) REFERENCES [dbo].[CurrencyCode]([HashKey]);
GO

