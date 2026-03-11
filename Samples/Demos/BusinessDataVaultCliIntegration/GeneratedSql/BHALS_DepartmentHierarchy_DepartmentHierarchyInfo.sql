-- Deterministic schema script

CREATE TABLE [dbo].[BHALS_DepartmentHierarchy_DepartmentHierarchyInfo] (
    [LinkHashKey] binary(16) NOT NULL,
    [HierarchyLevel] int NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BHALS_DepartmentHierarchy_DepartmentHierarchyInfo] PRIMARY KEY CLUSTERED ([LinkHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[BHALS_DepartmentHierarchy_DepartmentHierarchyInfo] WITH CHECK ADD CONSTRAINT [FK_BHALS_DepartmentHierarchy_DepartmentHierarchyInfo_BHAL_DepartmentHierarchy] FOREIGN KEY([LinkHashKey]) REFERENCES [dbo].[BHAL_DepartmentHierarchy]([HashKey]);
GO

