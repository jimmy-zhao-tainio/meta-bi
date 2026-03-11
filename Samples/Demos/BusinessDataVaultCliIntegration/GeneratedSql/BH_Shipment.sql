-- Deterministic schema script

CREATE TABLE [dbo].[BH_Shipment] (
    [HashKey] binary(16) NOT NULL,
    [ShipmentIdentifier] nvarchar(40) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BH_Shipment] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_BH_Shipment] UNIQUE ([ShipmentIdentifier] ASC)
);
GO

