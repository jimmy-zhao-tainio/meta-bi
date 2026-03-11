-- Deterministic schema script

CREATE TABLE [dbo].[BHS_Customer_CustomerAddress] (
    [HubHashKey] binary(16) NOT NULL,
    [CountryCode] nvarchar(3) NOT NULL,
    [CityName] nvarchar(100) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BHS_Customer_CustomerAddress] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[BHS_Customer_CustomerAddress] WITH CHECK ADD CONSTRAINT [FK_CustomerAddress_Customer] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[BH_Customer]([HashKey]);
GO

