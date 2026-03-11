-- Deterministic schema script

CREATE TABLE [dbo].[L_ProductSupplier] (
    [HashKey] binary(16) NOT NULL,
    [ProductHashKey] binary(16) NOT NULL,
    [SupplierHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_L_ProductSupplier] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_L_ProductSupplier] UNIQUE ([ProductHashKey] ASC, [SupplierHashKey] ASC)
);
GO

ALTER TABLE [dbo].[L_ProductSupplier] WITH CHECK ADD CONSTRAINT [FK_L_ProductSupplier_Product_ProductHashKey] FOREIGN KEY([ProductHashKey]) REFERENCES [dbo].[H_Product]([HashKey]);
GO

ALTER TABLE [dbo].[L_ProductSupplier] WITH CHECK ADD CONSTRAINT [FK_L_ProductSupplier_Supplier_SupplierHashKey] FOREIGN KEY([SupplierHashKey]) REFERENCES [dbo].[H_Supplier]([HashKey]);
GO

