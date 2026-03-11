-- Deterministic schema script

CREATE TABLE [dbo].[BHS_Supplier_SupplierProfile] (
    [HubHashKey] binary(16) NOT NULL,
    [SupplierName] nvarchar(200) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BHS_Supplier_SupplierProfile] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[BHS_Supplier_SupplierProfile] WITH CHECK ADD CONSTRAINT [FK_SupplierProfile_Supplier] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[BH_Supplier]([HashKey]);
GO

