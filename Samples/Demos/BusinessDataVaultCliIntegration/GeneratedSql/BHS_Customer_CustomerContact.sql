-- Deterministic schema script

CREATE TABLE [dbo].[BHS_Customer_CustomerContact] (
    [HubHashKey] binary(16) NOT NULL,
    [EmailAddress] nvarchar(200) NOT NULL,
    [PhoneNumber] nvarchar(40) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BHS_Customer_CustomerContact] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[BHS_Customer_CustomerContact] WITH CHECK ADD CONSTRAINT [FK_CustomerContact_Customer] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[BH_Customer]([HashKey]);
GO

