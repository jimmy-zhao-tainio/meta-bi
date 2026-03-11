-- Deterministic schema script

CREATE TABLE [dbo].[DepartmentProfile] (
    [HubHashKey] binary(16) NOT NULL,
    [DepartmentName] nvarchar(150) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_DepartmentProfile] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[DepartmentProfile] WITH CHECK ADD CONSTRAINT [FK_DepartmentProfile_Department] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[Department]([HashKey]);
GO

