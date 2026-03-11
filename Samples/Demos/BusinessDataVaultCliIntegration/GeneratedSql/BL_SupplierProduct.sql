-- Deterministic schema script

CREATE TABLE [dbo].[BL_SupplierProduct] (
    [HashKey] binary(16) NOT NULL,
    [SupplierHashKey] binary(16) NOT NULL,
    [ProductHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BL_SupplierProduct] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_BL_SupplierProduct] UNIQUE ([SupplierHashKey] ASC, [ProductHashKey] ASC)
);
GO

ALTER TABLE [dbo].[BL_SupplierProduct] WITH CHECK ADD CONSTRAINT [FK_SupplierProduct_Product_ProductHashKey] FOREIGN KEY([ProductHashKey]) REFERENCES [dbo].[BH_Product]([HashKey]);
GO

ALTER TABLE [dbo].[BL_SupplierProduct] WITH CHECK ADD CONSTRAINT [FK_SupplierProduct_Supplier_SupplierHashKey] FOREIGN KEY([SupplierHashKey]) REFERENCES [dbo].[BH_Supplier]([HashKey]);
GO

