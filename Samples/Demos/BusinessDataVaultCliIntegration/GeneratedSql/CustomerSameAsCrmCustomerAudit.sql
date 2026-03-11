-- Deterministic schema script

CREATE TABLE [dbo].[CustomerSameAsCrmCustomerAudit] (
    [LinkHashKey] binary(16) NOT NULL,
    [MatchConfidence] decimal(5, 2) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_CustomerSameAsCrmCustomerAudit] PRIMARY KEY CLUSTERED ([LinkHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[CustomerSameAsCrmCustomerAudit] WITH CHECK ADD CONSTRAINT [FK_CustomerSameAsCrmCustomerAudit_CustomerSameAsCrmCustomer] FOREIGN KEY([LinkHashKey]) REFERENCES [dbo].[CustomerSameAsCrmCustomer]([HashKey]);
GO

