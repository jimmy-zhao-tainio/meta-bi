-- Deterministic schema script

CREATE TABLE [dbo].[BSAL_CustomerSameAsCrmCustomer] (
    [HashKey] binary(16) NOT NULL,
    [PrimaryHashKey] binary(16) NOT NULL,
    [EquivalentHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BSAL_CustomerSameAsCrmCustomer] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_BSAL_CustomerSameAsCrmCustomer] UNIQUE ([PrimaryHashKey] ASC, [EquivalentHashKey] ASC)
);
GO

ALTER TABLE [dbo].[BSAL_CustomerSameAsCrmCustomer] WITH CHECK ADD CONSTRAINT [FK_CustomerSameAsCrmCustomer_Customer_EquivalentHashKey] FOREIGN KEY([EquivalentHashKey]) REFERENCES [dbo].[BH_Customer]([HashKey]);
GO

ALTER TABLE [dbo].[BSAL_CustomerSameAsCrmCustomer] WITH CHECK ADD CONSTRAINT [FK_CustomerSameAsCrmCustomer_Customer_PrimaryHashKey] FOREIGN KEY([PrimaryHashKey]) REFERENCES [dbo].[BH_Customer]([HashKey]);
GO

