-- Deterministic schema script

CREATE TABLE [dbo].[BHS_Customer_CustomerProfile] (
    [HubHashKey] binary(16) NOT NULL,
    [CustomerName] nvarchar(200) NOT NULL,
    [CustomerTier] nvarchar(30) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BHS_Customer_CustomerProfile] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[BHS_Customer_CustomerProfile] WITH CHECK ADD CONSTRAINT [FK_CustomerProfile_Customer] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[BH_Customer]([HashKey]);
GO

