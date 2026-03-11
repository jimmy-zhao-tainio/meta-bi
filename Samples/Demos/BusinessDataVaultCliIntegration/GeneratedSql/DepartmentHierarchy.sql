-- Deterministic schema script

CREATE TABLE [dbo].[DepartmentHierarchy] (
    [HashKey] binary(16) NOT NULL,
    [ParentHashKey] binary(16) NOT NULL,
    [ChildHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_DepartmentHierarchy] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_DepartmentHierarchy] UNIQUE ([ParentHashKey] ASC, [ChildHashKey] ASC)
);
GO

ALTER TABLE [dbo].[DepartmentHierarchy] WITH CHECK ADD CONSTRAINT [FK_DepartmentHierarchy_Department_ChildHashKey] FOREIGN KEY([ChildHashKey]) REFERENCES [dbo].[Department]([HashKey]);
GO

ALTER TABLE [dbo].[DepartmentHierarchy] WITH CHECK ADD CONSTRAINT [FK_DepartmentHierarchy_Department_ParentHashKey] FOREIGN KEY([ParentHashKey]) REFERENCES [dbo].[Department]([HashKey]);
GO

