-- Deterministic schema script

CREATE TABLE [dbo].[EmployeeDepartmentAssignment] (
    [LinkHashKey] binary(16) NOT NULL,
    [StartDate] datetime NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_EmployeeDepartmentAssignment] PRIMARY KEY CLUSTERED ([LinkHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[EmployeeDepartmentAssignment] WITH CHECK ADD CONSTRAINT [FK_EmployeeDepartmentAssignment_EmployeeDepartment] FOREIGN KEY([LinkHashKey]) REFERENCES [dbo].[EmployeeDepartment]([HashKey]);
GO

