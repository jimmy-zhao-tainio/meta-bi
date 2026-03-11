-- Deterministic schema script

CREATE TABLE [dbo].[LS_EmployeeDepartment_EmployeeDepartmentAssignment] (
    [LinkHashKey] binary(16) NOT NULL,
    [HireDate] datetime2(7) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_LS_EmployeeDepartment_EmployeeDepartmentAssignment] PRIMARY KEY CLUSTERED ([LinkHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[LS_EmployeeDepartment_EmployeeDepartmentAssignment] WITH CHECK ADD CONSTRAINT [FK_LS_EmployeeDepartment_EmployeeDepartmentAssignment_EmployeeDepartment] FOREIGN KEY([LinkHashKey]) REFERENCES [dbo].[L_EmployeeDepartment]([HashKey]);
GO

