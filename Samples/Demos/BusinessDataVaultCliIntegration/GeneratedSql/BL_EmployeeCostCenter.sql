-- Deterministic schema script

CREATE TABLE [dbo].[BL_EmployeeCostCenter] (
    [HashKey] binary(16) NOT NULL,
    [EmployeeHashKey] binary(16) NOT NULL,
    [CostCenterHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BL_EmployeeCostCenter] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_BL_EmployeeCostCenter] UNIQUE ([EmployeeHashKey] ASC, [CostCenterHashKey] ASC)
);
GO

ALTER TABLE [dbo].[BL_EmployeeCostCenter] WITH CHECK ADD CONSTRAINT [FK_EmployeeCostCenter_CostCenter_CostCenterHashKey] FOREIGN KEY([CostCenterHashKey]) REFERENCES [dbo].[BH_CostCenter]([HashKey]);
GO

ALTER TABLE [dbo].[BL_EmployeeCostCenter] WITH CHECK ADD CONSTRAINT [FK_EmployeeCostCenter_Employee_EmployeeHashKey] FOREIGN KEY([EmployeeHashKey]) REFERENCES [dbo].[BH_Employee]([HashKey]);
GO

