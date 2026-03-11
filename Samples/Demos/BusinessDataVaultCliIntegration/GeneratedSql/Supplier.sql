-- Deterministic schema script

CREATE TABLE [dbo].[Supplier] (
    [HashKey] binary(16) NOT NULL,
    [SupplierIdentifier] nvarchar(40) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_Supplier] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_Supplier] UNIQUE ([SupplierIdentifier] ASC)
);
GO

