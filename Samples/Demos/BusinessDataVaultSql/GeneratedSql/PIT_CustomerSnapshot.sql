CREATE TABLE [PIT_CustomerSnapshot] (
    [HubHashKey] binary(16) NOT NULL,
    [SnapshotTimestamp] datetime2(7) NOT NULL,
    [BHS_Customer_ProfileLoadTimestamp] datetime2(7) NOT NULL,
    [BLS_CustomerOrder_StatusLoadTimestamp] datetime2(7) NOT NULL,
    CONSTRAINT [PK_PIT_CustomerSnapshot] PRIMARY KEY ([HubHashKey], [SnapshotTimestamp]),
    CONSTRAINT [FK_PIT_CustomerSnapshot_BH_Customer] FOREIGN KEY ([HubHashKey]) REFERENCES [BH_Customer] ([HashKey])
);
