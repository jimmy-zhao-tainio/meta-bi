-- Deterministic schema script

CREATE TABLE [dbo].[BHS_CostCenter_CostCenterProfile] (
    [HubHashKey] binary(16) NOT NULL,
    [CostCenterName] nvarchar(150) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BHS_CostCenter_CostCenterProfile] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[BHS_CostCenter_CostCenterProfile] WITH CHECK ADD CONSTRAINT [FK_CostCenterProfile_CostCenter] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[BH_CostCenter]([HashKey]);
GO

