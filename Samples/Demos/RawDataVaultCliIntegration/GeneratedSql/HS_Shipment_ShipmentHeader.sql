-- Deterministic schema script

CREATE TABLE [dbo].[HS_Shipment_ShipmentHeader] (
    [HubHashKey] binary(16) NOT NULL,
    [CarrierName] nvarchar(100) NULL,
    [ShipmentDate] datetime2(7) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_HS_Shipment_ShipmentHeader] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[HS_Shipment_ShipmentHeader] WITH CHECK ADD CONSTRAINT [FK_HS_Shipment_ShipmentHeader_Shipment] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[H_Shipment]([HashKey]);
GO

