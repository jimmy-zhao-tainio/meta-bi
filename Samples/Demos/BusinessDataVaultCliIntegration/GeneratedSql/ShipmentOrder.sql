-- Deterministic schema script

CREATE TABLE [dbo].[ShipmentOrder] (
    [HashKey] binary(16) NOT NULL,
    [ShipmentHashKey] binary(16) NOT NULL,
    [OrderHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_ShipmentOrder] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_ShipmentOrder] UNIQUE ([ShipmentHashKey] ASC, [OrderHashKey] ASC)
);
GO

ALTER TABLE [dbo].[ShipmentOrder] WITH CHECK ADD CONSTRAINT [FK_ShipmentOrder_Order_OrderHashKey] FOREIGN KEY([OrderHashKey]) REFERENCES [dbo].[Order]([HashKey]);
GO

ALTER TABLE [dbo].[ShipmentOrder] WITH CHECK ADD CONSTRAINT [FK_ShipmentOrder_Shipment_ShipmentHashKey] FOREIGN KEY([ShipmentHashKey]) REFERENCES [dbo].[Shipment]([HashKey]);
GO

