-- Deterministic schema script

CREATE TABLE [dbo].[Department] (
    [HashKey] binary(16) NOT NULL,
    [DepartmentIdentifier] nvarchar(30) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_Department] UNIQUE ([DepartmentIdentifier] ASC)
);
GO

