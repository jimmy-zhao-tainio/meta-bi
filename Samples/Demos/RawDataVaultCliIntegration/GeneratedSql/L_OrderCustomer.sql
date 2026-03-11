-- Deterministic schema script

CREATE TABLE [dbo].[L_OrderCustomer] (
    [HashKey] binary(16) NOT NULL,
    [OrderHashKey] binary(16) NOT NULL,
    [CustomerHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_L_OrderCustomer] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_L_OrderCustomer] UNIQUE ([OrderHashKey] ASC, [CustomerHashKey] ASC)
);
GO

ALTER TABLE [dbo].[L_OrderCustomer] WITH CHECK ADD CONSTRAINT [FK_L_OrderCustomer_Customer_CustomerHashKey] FOREIGN KEY([CustomerHashKey]) REFERENCES [dbo].[H_Customer]([HashKey]);
GO

ALTER TABLE [dbo].[L_OrderCustomer] WITH CHECK ADD CONSTRAINT [FK_L_OrderCustomer_Order_OrderHashKey] FOREIGN KEY([OrderHashKey]) REFERENCES [dbo].[H_Order]([HashKey]);
GO

