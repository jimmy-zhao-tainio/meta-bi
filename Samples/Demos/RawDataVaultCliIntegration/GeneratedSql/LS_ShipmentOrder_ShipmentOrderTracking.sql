-- Deterministic schema script

CREATE TABLE [dbo].[LS_ShipmentOrder_ShipmentOrderTracking] (
    [LinkHashKey] binary(16) NOT NULL,
    [CarrierName] nvarchar(100) NULL,
    [ShipmentDate] datetime2(7) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_LS_ShipmentOrder_ShipmentOrderTracking] PRIMARY KEY CLUSTERED ([LinkHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[LS_ShipmentOrder_ShipmentOrderTracking] WITH CHECK ADD CONSTRAINT [FK_LS_ShipmentOrder_ShipmentOrderTracking_ShipmentOrder] FOREIGN KEY([LinkHashKey]) REFERENCES [dbo].[L_ShipmentOrder]([HashKey]);
GO

