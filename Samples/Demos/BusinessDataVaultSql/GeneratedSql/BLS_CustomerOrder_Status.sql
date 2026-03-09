CREATE TABLE [BLS_CustomerOrder_Status] (
    [LinkHashKey] binary(16) NOT NULL,
    [StatusCode] nvarchar(20) NOT NULL,
    [HashDiff] binary(32) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BLS_CustomerOrder_Status] PRIMARY KEY ([LinkHashKey], [LoadTimestamp]),
    CONSTRAINT [FK_BLS_CustomerOrder_Status_BL_CustomerOrder] FOREIGN KEY ([LinkHashKey]) REFERENCES [BL_CustomerOrder] ([HashKey])
);
