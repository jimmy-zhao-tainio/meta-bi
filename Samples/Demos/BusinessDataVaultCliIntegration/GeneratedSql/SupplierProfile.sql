-- Deterministic schema script

CREATE TABLE [dbo].[SupplierProfile] (
    [HubHashKey] binary(16) NOT NULL,
    [SupplierName] nvarchar(200) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_SupplierProfile] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[SupplierProfile] WITH CHECK ADD CONSTRAINT [FK_SupplierProfile_Supplier] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[Supplier]([HashKey]);
GO

