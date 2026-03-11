-- Deterministic schema script

CREATE TABLE [dbo].[Employee] (
    [HashKey] binary(16) NOT NULL,
    [EmployeeIdentifier] nvarchar(30) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_Employee] UNIQUE ([EmployeeIdentifier] ASC)
);
GO

