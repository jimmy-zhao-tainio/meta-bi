-- Deterministic schema script

CREATE TABLE [dbo].[BHS_Product_ProductCommercial] (
    [HubHashKey] binary(16) NOT NULL,
    [BrandName] nvarchar(100) NOT NULL,
    [LifecycleStatus] nvarchar(40) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BHS_Product_ProductCommercial] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[BHS_Product_ProductCommercial] WITH CHECK ADD CONSTRAINT [FK_ProductCommercial_Product] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[BH_Product]([HashKey]);
GO

