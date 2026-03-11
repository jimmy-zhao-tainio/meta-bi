-- Deterministic schema script

CREATE TABLE [dbo].[H_Order] (
    [HashKey] binary(16) NOT NULL,
    [OrderId] nvarchar(40) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_H_Order] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_H_Order] UNIQUE ([OrderId] ASC)
);
GO

