CREATE TABLE [BL_CustomerInvoice] (
    [HashKey] binary(16) NOT NULL,
    [CustomerHashKey] binary(16) NOT NULL,
    [InvoiceHashKey] binary(16) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BL_CustomerInvoice] PRIMARY KEY ([HashKey]),
    CONSTRAINT [UQ_BL_CustomerInvoice] UNIQUE ([CustomerHashKey], [InvoiceHashKey]),
    CONSTRAINT [FK_BL_CustomerInvoice_BH_Customer_CustomerHashKey] FOREIGN KEY ([CustomerHashKey]) REFERENCES [BH_Customer] ([HashKey]),
    CONSTRAINT [FK_BL_CustomerInvoice_BH_Invoice_InvoiceHashKey] FOREIGN KEY ([InvoiceHashKey]) REFERENCES [BH_Invoice] ([HashKey])
);
