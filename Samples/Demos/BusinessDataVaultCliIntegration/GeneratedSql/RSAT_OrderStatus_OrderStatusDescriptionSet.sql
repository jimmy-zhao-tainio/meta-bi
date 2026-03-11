-- Deterministic schema script

CREATE TABLE [dbo].[RSAT_OrderStatus_OrderStatusDescriptionSet] (
    [ReferenceHashKey] binary(16) NOT NULL,
    [Description] nvarchar(100) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_RSAT_OrderStatus_OrderStatusDescriptionSet] PRIMARY KEY CLUSTERED ([ReferenceHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[RSAT_OrderStatus_OrderStatusDescriptionSet] WITH CHECK ADD CONSTRAINT [FK_RSAT_OrderStatus_OrderStatusDescriptionSet_REF_OrderStatus] FOREIGN KEY([ReferenceHashKey]) REFERENCES [dbo].[REF_OrderStatus]([HashKey]);
GO

