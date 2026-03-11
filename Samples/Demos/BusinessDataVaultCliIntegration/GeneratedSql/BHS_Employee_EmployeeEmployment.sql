-- Deterministic schema script

CREATE TABLE [dbo].[BHS_Employee_EmployeeEmployment] (
    [HubHashKey] binary(16) NOT NULL,
    [HireDate] datetime NOT NULL,
    [EmploymentType] nvarchar(30) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BHS_Employee_EmployeeEmployment] PRIMARY KEY CLUSTERED ([HubHashKey] ASC, [LoadTimestamp] ASC)
);
GO

ALTER TABLE [dbo].[BHS_Employee_EmployeeEmployment] WITH CHECK ADD CONSTRAINT [FK_EmployeeEmployment_Employee] FOREIGN KEY([HubHashKey]) REFERENCES [dbo].[BH_Employee]([HashKey]);
GO

