-- Deterministic schema script

CREATE TABLE [dbo].[OrderProduct] (
    [HashKey] binary(16) NOT NULL,
    [OrderHashKey] binary(16) NOT NULL,
    [ProductHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_OrderProduct] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_OrderProduct] UNIQUE ([OrderHashKey] ASC, [ProductHashKey] ASC)
);
GO

ALTER TABLE [dbo].[OrderProduct] WITH CHECK ADD CONSTRAINT [FK_OrderProduct_Order_OrderHashKey] FOREIGN KEY([OrderHashKey]) REFERENCES [dbo].[Order]([HashKey]);
GO

ALTER TABLE [dbo].[OrderProduct] WITH CHECK ADD CONSTRAINT [FK_OrderProduct_Product_ProductHashKey] FOREIGN KEY([ProductHashKey]) REFERENCES [dbo].[Product]([HashKey]);
GO

