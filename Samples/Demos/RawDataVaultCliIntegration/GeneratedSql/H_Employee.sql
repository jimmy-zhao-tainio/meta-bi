-- Deterministic schema script

CREATE TABLE [dbo].[H_Employee] (
    [HashKey] binary(16) NOT NULL,
    [EmployeeId] nvarchar(30) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_H_Employee] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_H_Employee] UNIQUE ([EmployeeId] ASC)
);
GO

