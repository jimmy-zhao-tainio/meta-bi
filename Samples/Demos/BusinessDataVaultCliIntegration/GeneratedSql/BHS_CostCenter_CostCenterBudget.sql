-- Deterministic schema script

CREATE TABLE [dbo].[BHS_CostCenter_CostCenterBudget] (
    [HubHashKey] binary(16) NOT NULL,
    [BudgetAmount] decimal(18, 2) NOT NULL,
    [FiscalYear] int NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BHS_CostCenter_CostCenterBudget] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[BHS_CostCenter_CostCenterBudget] WITH CHECK ADD CONSTRAINT [FK_CostCenterBudget_CostCenter] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[BH_CostCenter]([HashKey]);
GO

