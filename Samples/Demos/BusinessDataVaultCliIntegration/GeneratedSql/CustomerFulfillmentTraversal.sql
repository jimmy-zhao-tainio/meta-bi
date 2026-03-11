-- Deterministic schema script

CREATE TABLE [dbo].[CustomerFulfillmentTraversal] (
    [RootHashKey] binary(16) NOT NULL,
    [RelatedHashKey] binary(16) NOT NULL,
    [CustomerIdentifier] nvarchar(50) NOT NULL,
    [OrderIdentifier] nvarchar(40) NOT NULL,
    [ShipmentIdentifier] nvarchar(40) NOT NULL,
    [CustomerName] nvarchar(200) NOT NULL,
    [OrderStatusCode] nvarchar(20) NOT NULL,
    [Depth] int NOT NULL,
    [Path] nvarchar(4000) NOT NULL,
    [EffectiveFrom] datetime2(7) NOT NULL,
    [EffectiveTo] datetime2(7) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_CustomerFulfillmentTraversal] PRIMARY KEY CLUSTERED ([RootHashKey] ASC, [RelatedHashKey] ASC, [EffectiveFrom] ASC)
);
GO

ALTER TABLE [dbo].[CustomerFulfillmentTraversal] WITH CHECK ADD CONSTRAINT [FK_CustomerFulfillmentTraversal_Customer_RootHashKey] FOREIGN KEY([RootHashKey]) REFERENCES [dbo].[Customer]([HashKey]);
GO

ALTER TABLE [dbo].[CustomerFulfillmentTraversal] WITH CHECK ADD CONSTRAINT [FK_CustomerFulfillmentTraversal_Shipment_RelatedHashKey] FOREIGN KEY([RelatedHashKey]) REFERENCES [dbo].[Shipment]([HashKey]);
GO

