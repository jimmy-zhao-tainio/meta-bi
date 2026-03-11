-- Deterministic schema script

CREATE TABLE [dbo].[BL_ShipmentOrder] (
    [HashKey] binary(16) NOT NULL,
    [ShipmentHashKey] binary(16) NOT NULL,
    [OrderHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BL_ShipmentOrder] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_BL_ShipmentOrder] UNIQUE ([ShipmentHashKey] ASC, [OrderHashKey] ASC)
);
GO

ALTER TABLE [dbo].[BL_ShipmentOrder] WITH CHECK ADD CONSTRAINT [FK_ShipmentOrder_Order_OrderHashKey] FOREIGN KEY([OrderHashKey]) REFERENCES [dbo].[BH_Order]([HashKey]);
GO

ALTER TABLE [dbo].[BL_ShipmentOrder] WITH CHECK ADD CONSTRAINT [FK_ShipmentOrder_Shipment_ShipmentHashKey] FOREIGN KEY([ShipmentHashKey]) REFERENCES [dbo].[BH_Shipment]([HashKey]);
GO

