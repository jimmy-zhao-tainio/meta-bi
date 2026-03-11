-- Deterministic schema script

CREATE TABLE [dbo].[BLS_DepartmentManager_DepartmentManagerAssignment] (
    [LinkHashKey] binary(16) NOT NULL,
    [StartDate] datetime NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BLS_DepartmentManager_DepartmentManagerAssignment] PRIMARY KEY CLUSTERED ([LinkHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[BLS_DepartmentManager_DepartmentManagerAssignment] WITH CHECK ADD CONSTRAINT [FK_DepartmentManagerAssignment_DepartmentManager] FOREIGN KEY([LinkHashKey]) REFERENCES [dbo].[BL_DepartmentManager]([HashKey]);
GO

