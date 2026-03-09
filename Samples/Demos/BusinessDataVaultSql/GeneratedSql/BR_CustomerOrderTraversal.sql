CREATE TABLE [BR_CustomerOrderTraversal] (
    [RootHashKey] binary(16) NOT NULL,
    [RelatedHashKey] binary(16) NOT NULL,
    [CustomerIdentifier] nvarchar(50) NOT NULL,
    [OrderIdentifier] nvarchar(50) NOT NULL,
    [CustomerName] nvarchar(200) NOT NULL,
    [StatusCode] nvarchar(20) NOT NULL,
    [Depth] int NOT NULL,
    [Path] nvarchar(4000) NOT NULL,
    [EffectiveFrom] datetime2(7) NOT NULL,
    [EffectiveTo] datetime2(7) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BR_CustomerOrderTraversal] PRIMARY KEY ([RootHashKey], [RelatedHashKey], [EffectiveFrom]),
    CONSTRAINT [FK_BR_CustomerOrderTraversal_BH_Customer_RootHashKey] FOREIGN KEY ([RootHashKey]) REFERENCES [BH_Customer] ([HashKey]),
    CONSTRAINT [FK_BR_CustomerOrderTraversal_BH_Order_RelatedHashKey] FOREIGN KEY ([RelatedHashKey]) REFERENCES [BH_Order] ([HashKey])
);
