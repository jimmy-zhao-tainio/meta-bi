-- Deterministic schema script

CREATE TABLE [dbo].[HS_Supplier_SupplierProfile] (
    [HubHashKey] binary(16) NOT NULL,
    [SupplierName] nvarchar(200) NOT NULL,
    [RiskClass] nvarchar(30) NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_HS_Supplier_SupplierProfile] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[HS_Supplier_SupplierProfile] WITH CHECK ADD CONSTRAINT [FK_HS_Supplier_SupplierProfile_Supplier] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[H_Supplier]([HashKey]);
GO

