-- Deterministic schema script

CREATE TABLE [dbo].[BH_CostCenter] (
    [HashKey] binary(16) NOT NULL,
    [CostCenterIdentifier] nvarchar(20) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BH_CostCenter] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_BH_CostCenter] UNIQUE ([CostCenterIdentifier] ASC)
);
GO

