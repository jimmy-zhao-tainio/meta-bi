CREATE TABLE [REF_Status] (
    [HashKey] binary(16) NOT NULL,
    [StatusCode] nvarchar(20) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_REF_Status] PRIMARY KEY ([HashKey]),
    CONSTRAINT [UQ_REF_Status] UNIQUE ([StatusCode])
);
