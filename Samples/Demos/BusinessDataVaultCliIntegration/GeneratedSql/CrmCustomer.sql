-- Deterministic schema script

CREATE TABLE [dbo].[CrmCustomer] (
    [HashKey] binary(16) NOT NULL,
    [CrmCustomerIdentifier] nvarchar(50) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_CrmCustomer] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_CrmCustomer] UNIQUE ([CrmCustomerIdentifier] ASC)
);
GO

