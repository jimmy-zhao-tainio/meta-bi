-- Deterministic schema script

CREATE TABLE [dbo].[H_Shipment] (
    [HashKey] binary(16) NOT NULL,
    [ShipmentId] nvarchar(40) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_H_Shipment] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_H_Shipment] UNIQUE ([ShipmentId] ASC)
);
GO

