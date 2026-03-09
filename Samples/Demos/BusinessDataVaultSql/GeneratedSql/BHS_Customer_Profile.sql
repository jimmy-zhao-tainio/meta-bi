CREATE TABLE [BHS_Customer_Profile] (
    [HubHashKey] binary(16) NOT NULL,
    [CustomerName] nvarchar(200) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    CONSTRAINT [PK_BHS_Customer_Profile] PRIMARY KEY ([HubHashKey], [LoadTimestamp]),
    CONSTRAINT [FK_BHS_Customer_Profile_BH_Customer] FOREIGN KEY ([HubHashKey]) REFERENCES [BH_Customer] ([HashKey])
);
