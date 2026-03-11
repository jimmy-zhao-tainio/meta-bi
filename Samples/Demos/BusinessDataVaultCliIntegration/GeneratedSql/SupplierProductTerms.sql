-- Deterministic schema script

CREATE TABLE [dbo].[SupplierProductTerms] (
    [LinkHashKey] binary(16) NOT NULL,
    [LeadTimeDays] int NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_SupplierProductTerms] PRIMARY KEY CLUSTERED ([LinkHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[SupplierProductTerms] WITH CHECK ADD CONSTRAINT [FK_SupplierProductTerms_SupplierProduct] FOREIGN KEY([LinkHashKey]) REFERENCES [dbo].[SupplierProduct]([HashKey]);
GO

