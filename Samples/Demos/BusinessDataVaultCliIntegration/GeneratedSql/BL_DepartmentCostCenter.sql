-- Deterministic schema script

CREATE TABLE [dbo].[BL_DepartmentCostCenter] (
    [HashKey] binary(16) NOT NULL,
    [DepartmentHashKey] binary(16) NOT NULL,
    [CostCenterHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BL_DepartmentCostCenter] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_BL_DepartmentCostCenter] UNIQUE ([DepartmentHashKey] ASC, [CostCenterHashKey] ASC)
);
GO

ALTER TABLE [dbo].[BL_DepartmentCostCenter] WITH CHECK ADD CONSTRAINT [FK_DepartmentCostCenter_CostCenter_CostCenterHashKey] FOREIGN KEY([CostCenterHashKey]) REFERENCES [dbo].[BH_CostCenter]([HashKey]);
GO

ALTER TABLE [dbo].[BL_DepartmentCostCenter] WITH CHECK ADD CONSTRAINT [FK_DepartmentCostCenter_Department_DepartmentHashKey] FOREIGN KEY([DepartmentHashKey]) REFERENCES [dbo].[BH_Department]([HashKey]);
GO

