-- Deterministic schema script

CREATE TABLE [dbo].[BHS_Shipment_ShipmentHeader] (
    [HubHashKey] binary(16) NOT NULL,
    [ShipmentDate] datetime NOT NULL,
    [CarrierName] nvarchar(100) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BHS_Shipment_ShipmentHeader] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[BHS_Shipment_ShipmentHeader] WITH CHECK ADD CONSTRAINT [FK_ShipmentHeader_Shipment] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[BH_Shipment]([HashKey]);
GO

