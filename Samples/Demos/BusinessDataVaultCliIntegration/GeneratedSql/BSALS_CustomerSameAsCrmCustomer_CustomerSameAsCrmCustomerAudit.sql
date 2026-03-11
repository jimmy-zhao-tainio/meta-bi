-- Deterministic schema script

CREATE TABLE [dbo].[BSALS_CustomerSameAsCrmCustomer_CustomerSameAsCrmCustomerAudit] (
    [LinkHashKey] binary(16) NOT NULL,
    [MatchConfidence] decimal(5, 2) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BSALS_CustomerSameAsCrmCustomer_CustomerSameAsCrmCustomerAudit] PRIMARY KEY CLUSTERED ([LinkHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[BSALS_CustomerSameAsCrmCustomer_CustomerSameAsCrmCustomerAudit] WITH CHECK ADD CONSTRAINT [FK_BSALS_CustomerSameAsCrmCustomer_CustomerSameAsCrmCustomerAudit_BSAL_CustomerSameAsCrmCustomer] FOREIGN KEY([LinkHashKey]) REFERENCES [dbo].[BSAL_CustomerSameAsCrmCustomer]([HashKey]);
GO

