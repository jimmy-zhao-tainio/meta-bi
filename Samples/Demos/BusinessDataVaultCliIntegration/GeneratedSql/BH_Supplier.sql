-- Deterministic schema script

CREATE TABLE [dbo].[BH_Supplier] (
    [HashKey] binary(16) NOT NULL,
    [SupplierIdentifier] nvarchar(40) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BH_Supplier] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_BH_Supplier] UNIQUE ([SupplierIdentifier] ASC)
);
GO

