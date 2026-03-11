-- Deterministic schema script

CREATE TABLE [dbo].[BL_InvoiceShipment] (
    [HashKey] binary(16) NOT NULL,
    [InvoiceHashKey] binary(16) NOT NULL,
    [ShipmentHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BL_InvoiceShipment] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_BL_InvoiceShipment] UNIQUE ([InvoiceHashKey] ASC, [ShipmentHashKey] ASC)
);
GO

ALTER TABLE [dbo].[BL_InvoiceShipment] WITH CHECK ADD CONSTRAINT [FK_InvoiceShipment_Invoice_InvoiceHashKey] FOREIGN KEY([InvoiceHashKey]) REFERENCES [dbo].[BH_Invoice]([HashKey]);
GO

ALTER TABLE [dbo].[BL_InvoiceShipment] WITH CHECK ADD CONSTRAINT [FK_InvoiceShipment_Shipment_ShipmentHashKey] FOREIGN KEY([ShipmentHashKey]) REFERENCES [dbo].[BH_Shipment]([HashKey]);
GO

