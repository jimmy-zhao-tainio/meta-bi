-- Deterministic schema script

CREATE TABLE [dbo].[DepartmentCostCenterAssignment] (
    [LinkHashKey] binary(16) NOT NULL,
    [AllocationShare] decimal(9, 4) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_DepartmentCostCenterAssignment] PRIMARY KEY CLUSTERED ([LinkHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[DepartmentCostCenterAssignment] WITH CHECK ADD CONSTRAINT [FK_DepartmentCostCenterAssignment_DepartmentCostCenter] FOREIGN KEY([LinkHashKey]) REFERENCES [dbo].[DepartmentCostCenter]([HashKey]);
GO

