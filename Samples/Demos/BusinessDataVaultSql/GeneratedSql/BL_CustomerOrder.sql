CREATE TABLE [BL_CustomerOrder] (
    [HashKey] binary(16) NOT NULL,
    [CustomerHashKey] binary(16) NOT NULL,
    [OrderHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    CONSTRAINT [PK_BL_CustomerOrder] PRIMARY KEY ([HashKey]),
    CONSTRAINT [UQ_BL_CustomerOrder] UNIQUE ([CustomerHashKey], [OrderHashKey]),
    CONSTRAINT [FK_BL_CustomerOrder_BH_Customer_CustomerHashKey] FOREIGN KEY ([CustomerHashKey]) REFERENCES [BH_Customer] ([HashKey]),
    CONSTRAINT [FK_BL_CustomerOrder_BH_Order_OrderHashKey] FOREIGN KEY ([OrderHashKey]) REFERENCES [BH_Order] ([HashKey])
);
