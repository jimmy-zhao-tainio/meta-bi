-- Deterministic schema script

CREATE TABLE [dbo].[OrderStatusDescriptionSet] (
    [ReferenceHashKey] binary(16) NOT NULL,
    [Description] nvarchar(100) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_OrderStatusDescriptionSet] PRIMARY KEY CLUSTERED ([ReferenceHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[OrderStatusDescriptionSet] WITH CHECK ADD CONSTRAINT [FK_OrderStatusDescriptionSet_OrderStatus] FOREIGN KEY([ReferenceHashKey]) REFERENCES [dbo].[OrderStatus]([HashKey]);
GO

