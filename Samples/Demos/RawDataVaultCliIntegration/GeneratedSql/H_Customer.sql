-- Deterministic schema script

CREATE TABLE [dbo].[H_Customer] (
    [HashKey] binary(16) NOT NULL,
    [CustomerId] nvarchar(50) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_H_Customer] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_H_Customer] UNIQUE ([CustomerId] ASC)
);
GO

