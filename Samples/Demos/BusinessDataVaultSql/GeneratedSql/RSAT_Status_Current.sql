CREATE TABLE [RSAT_Status_Current] (
    [ReferenceHashKey] binary(16) NOT NULL,
    [StatusName] nvarchar(200) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_RSAT_Status_Current] PRIMARY KEY ([ReferenceHashKey], [LoadTimestamp]),
    CONSTRAINT [FK_RSAT_Status_Current_REF_Status] FOREIGN KEY ([ReferenceHashKey]) REFERENCES [REF_Status] ([HashKey])
);
