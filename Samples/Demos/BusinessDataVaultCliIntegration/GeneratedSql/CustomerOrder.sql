-- Deterministic schema script

CREATE TABLE [dbo].[CustomerOrder] (
    [HashKey] binary(16) NOT NULL,
    [CustomerHashKey] binary(16) NOT NULL,
    [OrderHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_CustomerOrder] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_CustomerOrder] UNIQUE ([CustomerHashKey] ASC, [OrderHashKey] ASC)
);
GO

ALTER TABLE [dbo].[CustomerOrder] WITH CHECK ADD CONSTRAINT [FK_CustomerOrder_Customer_CustomerHashKey] FOREIGN KEY([CustomerHashKey]) REFERENCES [dbo].[Customer]([HashKey]);
GO

ALTER TABLE [dbo].[CustomerOrder] WITH CHECK ADD CONSTRAINT [FK_CustomerOrder_Order_OrderHashKey] FOREIGN KEY([OrderHashKey]) REFERENCES [dbo].[Order]([HashKey]);
GO

