-- Deterministic schema script

CREATE TABLE [dbo].[REF_OrderStatus] (
    [HashKey] binary(16) NOT NULL,
    [OrderStatusCode] nvarchar(20) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_REF_OrderStatus] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_REF_OrderStatus] UNIQUE ([OrderStatusCode] ASC)
);
GO

