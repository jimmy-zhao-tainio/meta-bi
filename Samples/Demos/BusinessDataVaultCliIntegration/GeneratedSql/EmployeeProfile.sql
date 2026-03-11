-- Deterministic schema script

CREATE TABLE [dbo].[EmployeeProfile] (
    [HubHashKey] binary(16) NOT NULL,
    [EmployeeName] nvarchar(150) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_EmployeeProfile] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[EmployeeProfile] WITH CHECK ADD CONSTRAINT [FK_EmployeeProfile_Employee] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[Employee]([HashKey]);
GO

