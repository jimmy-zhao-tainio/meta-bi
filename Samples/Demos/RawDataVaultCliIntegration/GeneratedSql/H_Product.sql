-- Deterministic schema script

CREATE TABLE [dbo].[H_Product] (
    [HashKey] binary(16) NOT NULL,
    [ProductId] nvarchar(40) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_H_Product] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_H_Product] UNIQUE ([ProductId] ASC)
);
GO

