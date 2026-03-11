-- Deterministic schema script

CREATE TABLE [dbo].[CustomerOrderStatus] (
    [LinkHashKey] binary(16) NOT NULL,
    [OrderStatusCode] nvarchar(20) NOT NULL,
    [CurrencyCode] nvarchar(3) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_CustomerOrderStatus] PRIMARY KEY CLUSTERED ([LinkHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[CustomerOrderStatus] WITH CHECK ADD CONSTRAINT [FK_CustomerOrderStatus_CustomerOrder] FOREIGN KEY([LinkHashKey]) REFERENCES [dbo].[CustomerOrder]([HashKey]);
GO

