-- Deterministic schema script

CREATE TABLE [dbo].[Shipment] (
    [HashKey] binary(16) NOT NULL,
    [ShipmentIdentifier] nvarchar(40) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_Shipment] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_Shipment] UNIQUE ([ShipmentIdentifier] ASC)
);
GO

