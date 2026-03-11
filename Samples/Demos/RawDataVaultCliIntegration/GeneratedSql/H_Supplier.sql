-- Deterministic schema script

CREATE TABLE [dbo].[H_Supplier] (
    [HashKey] binary(16) NOT NULL,
    [SupplierId] nvarchar(40) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_H_Supplier] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_H_Supplier] UNIQUE ([SupplierId] ASC)
);
GO

