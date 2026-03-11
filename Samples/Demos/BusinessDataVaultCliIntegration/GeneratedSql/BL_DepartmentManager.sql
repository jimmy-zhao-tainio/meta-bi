-- Deterministic schema script

CREATE TABLE [dbo].[BL_DepartmentManager] (
    [HashKey] binary(16) NOT NULL,
    [DepartmentHashKey] binary(16) NOT NULL,
    [ManagerEmployeeHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BL_DepartmentManager] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_BL_DepartmentManager] UNIQUE ([DepartmentHashKey] ASC, [ManagerEmployeeHashKey] ASC)
);
GO

ALTER TABLE [dbo].[BL_DepartmentManager] WITH CHECK ADD CONSTRAINT [FK_DepartmentManager_Department_DepartmentHashKey] FOREIGN KEY([DepartmentHashKey]) REFERENCES [dbo].[BH_Department]([HashKey]);
GO

ALTER TABLE [dbo].[BL_DepartmentManager] WITH CHECK ADD CONSTRAINT [FK_DepartmentManager_Employee_ManagerEmployeeHashKey] FOREIGN KEY([ManagerEmployeeHashKey]) REFERENCES [dbo].[BH_Employee]([HashKey]);
GO

