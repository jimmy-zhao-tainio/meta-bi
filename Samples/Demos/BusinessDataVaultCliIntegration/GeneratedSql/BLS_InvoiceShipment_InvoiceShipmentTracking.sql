-- Deterministic schema script

CREATE TABLE [dbo].[BLS_InvoiceShipment_InvoiceShipmentTracking] (
    [LinkHashKey] binary(16) NOT NULL,
    [ReceiptDate] datetime NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BLS_InvoiceShipment_InvoiceShipmentTracking] PRIMARY KEY CLUSTERED ([LinkHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[BLS_InvoiceShipment_InvoiceShipmentTracking] WITH CHECK ADD CONSTRAINT [FK_InvoiceShipmentTracking_InvoiceShipment] FOREIGN KEY([LinkHashKey]) REFERENCES [dbo].[BL_InvoiceShipment]([HashKey]);
GO

