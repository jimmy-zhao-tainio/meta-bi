-- Deterministic schema script

CREATE TABLE [dbo].[HS_Customer_CustomerProfile] (
    [HubHashKey] binary(16) NOT NULL,
    [CustomerName] nvarchar(200) NOT NULL,
    [EmailAddress] nvarchar(200) NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_HS_Customer_CustomerProfile] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[HS_Customer_CustomerProfile] WITH CHECK ADD CONSTRAINT [FK_HS_Customer_CustomerProfile_Customer] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[H_Customer]([HashKey]);
GO

