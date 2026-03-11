-- Deterministic schema script

CREATE TABLE [dbo].[BHS_Supplier_SupplierCompliance] (
    [HubHashKey] binary(16) NOT NULL,
    [RiskClass] nvarchar(30) NOT NULL,
    [TaxIdentifier] nvarchar(50) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BHS_Supplier_SupplierCompliance] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[BHS_Supplier_SupplierCompliance] WITH CHECK ADD CONSTRAINT [FK_SupplierCompliance_Supplier] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[BH_Supplier]([HashKey]);
GO

