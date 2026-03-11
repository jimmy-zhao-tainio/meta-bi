-- Deterministic schema script

CREATE TABLE [dbo].[L_InvoiceOrder] (
    [HashKey] binary(16) NOT NULL,
    [InvoiceHashKey] binary(16) NOT NULL,
    [OrderHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_L_InvoiceOrder] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_L_InvoiceOrder] UNIQUE ([InvoiceHashKey] ASC, [OrderHashKey] ASC)
);
GO

ALTER TABLE [dbo].[L_InvoiceOrder] WITH CHECK ADD CONSTRAINT [FK_L_InvoiceOrder_Invoice_InvoiceHashKey] FOREIGN KEY([InvoiceHashKey]) REFERENCES [dbo].[H_Invoice]([HashKey]);
GO

ALTER TABLE [dbo].[L_InvoiceOrder] WITH CHECK ADD CONSTRAINT [FK_L_InvoiceOrder_Order_OrderHashKey] FOREIGN KEY([OrderHashKey]) REFERENCES [dbo].[H_Order]([HashKey]);
GO

