-- Deterministic schema script

CREATE TABLE [dbo].[DepartmentHierarchyInfo] (
    [LinkHashKey] binary(16) NOT NULL,
    [HierarchyLevel] int NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_DepartmentHierarchyInfo] PRIMARY KEY CLUSTERED ([LinkHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[DepartmentHierarchyInfo] WITH CHECK ADD CONSTRAINT [FK_DepartmentHierarchyInfo_DepartmentHierarchy] FOREIGN KEY([LinkHashKey]) REFERENCES [dbo].[DepartmentHierarchy]([HashKey]);
GO

