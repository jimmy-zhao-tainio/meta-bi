-- Deterministic schema script

CREATE TABLE [dbo].[CostCenter] (
    [HashKey] binary(16) NOT NULL,
    [CostCenterIdentifier] nvarchar(20) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_CostCenter] PRIMARY KEY CLUSTERED ([HashKey] ASC),
    CONSTRAINT [UQ_CostCenter] UNIQUE ([CostCenterIdentifier] ASC)
);
GO

