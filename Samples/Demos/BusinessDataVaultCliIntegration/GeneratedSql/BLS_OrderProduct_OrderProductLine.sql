-- Deterministic schema script

CREATE TABLE [dbo].[BLS_OrderProduct_OrderProductLine] (
    [LinkHashKey] binary(16) NOT NULL,
    [Quantity] int NOT NULL,
    [UnitPrice] decimal(18, 2) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BLS_OrderProduct_OrderProductLine] PRIMARY KEY CLUSTERED ([LinkHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[BLS_OrderProduct_OrderProductLine] WITH CHECK ADD CONSTRAINT [FK_OrderProductLine_OrderProduct] FOREIGN KEY([LinkHashKey]) REFERENCES [dbo].[BL_OrderProduct]([HashKey]);
GO

