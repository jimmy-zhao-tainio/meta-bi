-- Deterministic schema script

CREATE TABLE [dbo].[EmployeeDepartment] (
    [HashKey] binary(16) NOT NULL,
    [EmployeeHashKey] binary(16) NOT NULL,
    [DepartmentHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_EmployeeDepartment] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_EmployeeDepartment] UNIQUE ([EmployeeHashKey] ASC, [DepartmentHashKey] ASC)
);
GO

ALTER TABLE [dbo].[EmployeeDepartment] WITH CHECK ADD CONSTRAINT [FK_EmployeeDepartment_Department_DepartmentHashKey] FOREIGN KEY([DepartmentHashKey]) REFERENCES [dbo].[Department]([HashKey]);
GO

ALTER TABLE [dbo].[EmployeeDepartment] WITH CHECK ADD CONSTRAINT [FK_EmployeeDepartment_Employee_EmployeeHashKey] FOREIGN KEY([EmployeeHashKey]) REFERENCES [dbo].[Employee]([HashKey]);
GO

