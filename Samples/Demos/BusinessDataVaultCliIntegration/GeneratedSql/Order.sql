-- Deterministic schema script

CREATE TABLE [dbo].[Order] (
    [HashKey] binary(16) NOT NULL,
    [OrderIdentifier] nvarchar(40) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_Order] UNIQUE ([OrderIdentifier] ASC)
);
GO

