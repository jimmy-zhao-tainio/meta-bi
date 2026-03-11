-- Deterministic schema script

CREATE TABLE [dbo].[BLS_EmployeeDepartment_EmployeeDepartmentAssignment] (
    [LinkHashKey] binary(16) NOT NULL,
    [StartDate] datetime NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BLS_EmployeeDepartment_EmployeeDepartmentAssignment] PRIMARY KEY CLUSTERED ([LinkHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[BLS_EmployeeDepartment_EmployeeDepartmentAssignment] WITH CHECK ADD CONSTRAINT [FK_EmployeeDepartmentAssignment_EmployeeDepartment] FOREIGN KEY([LinkHashKey]) REFERENCES [dbo].[BL_EmployeeDepartment]([HashKey]);
GO

