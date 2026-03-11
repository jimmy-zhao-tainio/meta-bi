-- Deterministic schema script

CREATE TABLE [dbo].[PIT_CustomerSnapshot] (
    [HubHashKey] binary(16) NOT NULL,
    [SnapshotTimestamp] datetime2(7) NOT NULL,
    [CustomerProfileLoadTimestamp] datetime2(7) NOT NULL,
    [CustomerOrderStatusLoadTimestamp] datetime2(7) NOT NULL,
    [BusinessDate] datetime NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_PIT_CustomerSnapshot] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [SnapshotTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[PIT_CustomerSnapshot] WITH CHECK ADD CONSTRAINT [FK_CustomerSnapshot_Customer] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[BH_Customer]([HashKey]);
GO

