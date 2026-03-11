-- Deterministic schema script

CREATE TABLE [dbo].[ProductProfile] (
    [HubHashKey] binary(16) NOT NULL,
    [ProductName] nvarchar(200) NOT NULL,
    [ProductCategory] nvarchar(50) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_ProductProfile] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[ProductProfile] WITH CHECK ADD CONSTRAINT [FK_ProductProfile_Product] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[Product]([HashKey]);
GO

