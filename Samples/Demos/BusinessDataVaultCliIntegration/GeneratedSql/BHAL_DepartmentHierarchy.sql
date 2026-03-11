-- Deterministic schema script

CREATE TABLE [dbo].[BHAL_DepartmentHierarchy] (
    [HashKey] binary(16) NOT NULL,
    [ParentHashKey] binary(16) NOT NULL,
    [ChildHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BHAL_DepartmentHierarchy] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_BHAL_DepartmentHierarchy] UNIQUE ([ParentHashKey] ASC, [ChildHashKey] ASC)
);
GO

ALTER TABLE [dbo].[BHAL_DepartmentHierarchy] WITH CHECK ADD CONSTRAINT [FK_DepartmentHierarchy_Department_ChildHashKey] FOREIGN KEY([ChildHashKey]) REFERENCES [dbo].[BH_Department]([HashKey]);
GO

ALTER TABLE [dbo].[BHAL_DepartmentHierarchy] WITH CHECK ADD CONSTRAINT [FK_DepartmentHierarchy_Department_ParentHashKey] FOREIGN KEY([ParentHashKey]) REFERENCES [dbo].[BH_Department]([HashKey]);
GO

