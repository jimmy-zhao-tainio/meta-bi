-- Deterministic schema script

CREATE TABLE [dbo].[OrderStatus] (
    [HashKey] binary(16) NOT NULL,
    [OrderStatusCode] nvarchar(20) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_OrderStatus] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_OrderStatus] UNIQUE ([OrderStatusCode] ASC)
);
GO

