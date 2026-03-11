-- Deterministic schema script

CREATE TABLE [dbo].[RSAT_CurrencyCode_CurrencyCodeDescriptionSet] (
    [ReferenceHashKey] binary(16) NOT NULL,
    [Description] nvarchar(100) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_RSAT_CurrencyCode_CurrencyCodeDescriptionSet] PRIMARY KEY CLUSTERED ([ReferenceHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[RSAT_CurrencyCode_CurrencyCodeDescriptionSet] WITH CHECK ADD CONSTRAINT [FK_RSAT_CurrencyCode_CurrencyCodeDescriptionSet_REF_CurrencyCode] FOREIGN KEY([ReferenceHashKey]) REFERENCES [dbo].[REF_CurrencyCode]([HashKey]);
GO

