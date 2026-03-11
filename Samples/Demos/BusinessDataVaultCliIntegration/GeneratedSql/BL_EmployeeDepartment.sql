-- Deterministic schema script

CREATE TABLE [dbo].[BL_EmployeeDepartment] (
    [HashKey] binary(16) NOT NULL,
    [EmployeeHashKey] binary(16) NOT NULL,
    [DepartmentHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BL_EmployeeDepartment] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_BL_EmployeeDepartment] UNIQUE ([EmployeeHashKey] ASC, [DepartmentHashKey] ASC)
);
GO

ALTER TABLE [dbo].[BL_EmployeeDepartment] WITH CHECK ADD CONSTRAINT [FK_EmployeeDepartment_Department_DepartmentHashKey] FOREIGN KEY([DepartmentHashKey]) REFERENCES [dbo].[BH_Department]([HashKey]);
GO

ALTER TABLE [dbo].[BL_EmployeeDepartment] WITH CHECK ADD CONSTRAINT [FK_EmployeeDepartment_Employee_EmployeeHashKey] FOREIGN KEY([EmployeeHashKey]) REFERENCES [dbo].[BH_Employee]([HashKey]);
GO

