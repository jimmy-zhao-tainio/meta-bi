-- Deterministic schema script

CREATE TABLE [dbo].[ShipmentHeader] (
    [HubHashKey] binary(16) NOT NULL,
    [ShipmentDate] datetime NOT NULL,
    [CarrierName] nvarchar(100) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_ShipmentHeader] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[ShipmentHeader] WITH CHECK ADD CONSTRAINT [FK_ShipmentHeader_Shipment] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[Shipment]([HashKey]);
GO

