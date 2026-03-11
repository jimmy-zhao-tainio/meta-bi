-- Deterministic schema script

CREATE TABLE [dbo].[BH_Customer] (
    [HashKey] binary(16) NOT NULL,
    [CustomerIdentifier] nvarchar(50) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BH_Customer] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_BH_Customer] UNIQUE ([CustomerIdentifier] ASC)
);
GO

