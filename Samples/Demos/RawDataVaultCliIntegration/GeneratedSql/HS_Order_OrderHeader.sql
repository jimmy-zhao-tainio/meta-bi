-- Deterministic schema script

CREATE TABLE [dbo].[HS_Order_OrderHeader] (
    [HubHashKey] binary(16) NOT NULL,
    [OrderDate] datetime2(7) NOT NULL,
    [OrderAmount] decimal(18, 2) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_HS_Order_OrderHeader] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[HS_Order_OrderHeader] WITH CHECK ADD CONSTRAINT [FK_HS_Order_OrderHeader_Order] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[H_Order]([HashKey]);
GO

