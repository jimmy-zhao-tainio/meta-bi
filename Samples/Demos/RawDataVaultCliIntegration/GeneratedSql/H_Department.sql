-- Deterministic schema script

CREATE TABLE [dbo].[H_Department] (
    [HashKey] binary(16) NOT NULL,
    [DepartmentId] nvarchar(30) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_H_Department] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_H_Department] UNIQUE ([DepartmentId] ASC)
);
GO

