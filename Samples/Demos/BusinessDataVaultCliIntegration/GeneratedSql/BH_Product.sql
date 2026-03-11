-- Deterministic schema script

CREATE TABLE [dbo].[BH_Product] (
    [HashKey] binary(16) NOT NULL,
    [ProductIdentifier] nvarchar(40) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BH_Product] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_BH_Product] UNIQUE ([ProductIdentifier] ASC)
);
GO

