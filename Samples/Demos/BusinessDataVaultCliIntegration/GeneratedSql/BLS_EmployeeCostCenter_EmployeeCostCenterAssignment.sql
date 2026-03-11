-- Deterministic schema script

CREATE TABLE [dbo].[BLS_EmployeeCostCenter_EmployeeCostCenterAssignment] (
    [LinkHashKey] binary(16) NOT NULL,
    [AllocationPercentage] decimal(9, 4) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BLS_EmployeeCostCenter_EmployeeCostCenterAssignment] PRIMARY KEY CLUSTERED ([LinkHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[BLS_EmployeeCostCenter_EmployeeCostCenterAssignment] WITH CHECK ADD CONSTRAINT [FK_EmployeeCostCenterAssignment_EmployeeCostCenter] FOREIGN KEY([LinkHashKey]) REFERENCES [dbo].[BL_EmployeeCostCenter]([HashKey]);
GO

