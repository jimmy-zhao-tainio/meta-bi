-- Deterministic schema script

CREATE TABLE [dbo].[BHS_Department_DepartmentOperatingModel] (
    [HubHashKey] binary(16) NOT NULL,
    [DepartmentType] nvarchar(40) NOT NULL,
    [RegionName] nvarchar(50) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BHS_Department_DepartmentOperatingModel] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[BHS_Department_DepartmentOperatingModel] WITH CHECK ADD CONSTRAINT [FK_DepartmentOperatingModel_Department] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[BH_Department]([HashKey]);
GO

