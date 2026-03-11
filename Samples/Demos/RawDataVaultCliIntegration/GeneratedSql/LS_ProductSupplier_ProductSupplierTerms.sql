-- Deterministic schema script

CREATE TABLE [dbo].[LS_ProductSupplier_ProductSupplierTerms] (
    [LinkHashKey] binary(16) NOT NULL,
    [SupplierProductCode] nvarchar(50) NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_LS_ProductSupplier_ProductSupplierTerms] PRIMARY KEY CLUSTERED ([LinkHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[LS_ProductSupplier_ProductSupplierTerms] WITH CHECK ADD CONSTRAINT [FK_LS_ProductSupplier_ProductSupplierTerms_ProductSupplier] FOREIGN KEY([LinkHashKey]) REFERENCES [dbo].[L_ProductSupplier]([HashKey]);
GO

