-- Deterministic schema script

CREATE TABLE [dbo].[L_ShipmentOrder] (
    [HashKey] binary(16) NOT NULL,
    [ShipmentHashKey] binary(16) NOT NULL,
    [OrderHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_L_ShipmentOrder] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_L_ShipmentOrder] UNIQUE ([ShipmentHashKey] ASC, [OrderHashKey] ASC)
);
GO

ALTER TABLE [dbo].[L_ShipmentOrder] WITH CHECK ADD CONSTRAINT [FK_L_ShipmentOrder_Order_OrderHashKey] FOREIGN KEY([OrderHashKey]) REFERENCES [dbo].[H_Order]([HashKey]);
GO

ALTER TABLE [dbo].[L_ShipmentOrder] WITH CHECK ADD CONSTRAINT [FK_L_ShipmentOrder_Shipment_ShipmentHashKey] FOREIGN KEY([ShipmentHashKey]) REFERENCES [dbo].[H_Shipment]([HashKey]);
GO

