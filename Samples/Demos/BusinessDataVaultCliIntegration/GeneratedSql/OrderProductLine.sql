-- Deterministic schema script

CREATE TABLE [dbo].[OrderProductLine] (
    [LinkHashKey] binary(16) NOT NULL,
    [Quantity] int NOT NULL,
    [UnitPrice] decimal(18, 2) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_OrderProductLine] PRIMARY KEY CLUSTERED ([LinkHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[OrderProductLine] WITH CHECK ADD CONSTRAINT [FK_OrderProductLine_OrderProduct] FOREIGN KEY([LinkHashKey]) REFERENCES [dbo].[OrderProduct]([HashKey]);
GO

