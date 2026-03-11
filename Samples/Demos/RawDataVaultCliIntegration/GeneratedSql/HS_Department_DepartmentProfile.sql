-- Deterministic schema script

CREATE TABLE [dbo].[HS_Department_DepartmentProfile] (
    [HubHashKey] binary(16) NOT NULL,
    [DepartmentName] nvarchar(150) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_HS_Department_DepartmentProfile] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[HS_Department_DepartmentProfile] WITH CHECK ADD CONSTRAINT [FK_HS_Department_DepartmentProfile_Department] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[H_Department]([HashKey]);
GO

