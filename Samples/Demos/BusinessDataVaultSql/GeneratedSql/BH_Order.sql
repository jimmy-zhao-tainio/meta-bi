CREATE TABLE [BH_Order] (
    [HashKey] binary(16) NOT NULL,
    [Identifier] nvarchar(50) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BH_Order] PRIMARY KEY ([HashKey]),
    CONSTRAINT [UQ_BH_Order] UNIQUE ([Identifier])
);
