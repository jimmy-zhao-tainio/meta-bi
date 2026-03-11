-- Deterministic schema script

CREATE TABLE [dbo].[Product] (
    [HashKey] binary(16) NOT NULL,
    [ProductIdentifier] nvarchar(40) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_Product] UNIQUE ([ProductIdentifier] ASC)
);
GO

