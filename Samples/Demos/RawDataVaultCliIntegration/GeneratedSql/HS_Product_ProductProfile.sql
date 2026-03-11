-- Deterministic schema script

CREATE TABLE [dbo].[HS_Product_ProductProfile] (
    [HubHashKey] binary(16) NOT NULL,
    [ProductName] nvarchar(200) NOT NULL,
    [ProductCategory] nvarchar(50) NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_HS_Product_ProductProfile] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[HS_Product_ProductProfile] WITH CHECK ADD CONSTRAINT [FK_HS_Product_ProductProfile_Product] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[H_Product]([HashKey]);
GO

