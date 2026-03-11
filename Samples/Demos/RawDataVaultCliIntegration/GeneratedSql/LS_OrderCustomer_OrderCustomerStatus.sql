-- Deterministic schema script

CREATE TABLE [dbo].[LS_OrderCustomer_OrderCustomerStatus] (
    [LinkHashKey] binary(16) NOT NULL,
    [OrderStatusCode] nvarchar(20) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_LS_OrderCustomer_OrderCustomerStatus] PRIMARY KEY CLUSTERED ([LinkHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[LS_OrderCustomer_OrderCustomerStatus] WITH CHECK ADD CONSTRAINT [FK_LS_OrderCustomer_OrderCustomerStatus_OrderCustomer] FOREIGN KEY([LinkHashKey]) REFERENCES [dbo].[L_OrderCustomer]([HashKey]);
GO

