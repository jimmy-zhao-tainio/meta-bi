DECLARE @Expected TABLE(TargetTable sysname, CustomerId int, CustomerName varbinary(max), TotalAmount decimal(18,2));
INSERT @Expected VALUES
 (N'TargetCustomer', 1, CONVERT(varbinary(max), N'Acme North'), 125.50),
 (N'TargetCustomer', 2, CONVERT(varbinary(max), N'Beacon Retail'), 980.00),
 (N'TargetCustomer', 3, CONVERT(varbinary(max), N'Contoso Labs'), 42.75),
 (N'InsertTargetCustomer', 10, CONVERT(varbinary(max), N'Inserted Alpha'), 10.10),
 (N'InsertTargetCustomer', 11, CONVERT(varbinary(max), N'Inserted Beta'), 11.11),
 (N'UpdateCustomer', 20, CONVERT(varbinary(max), N'Update Keep'), 20.00),
 (N'UpdateCustomer', 21, CONVERT(varbinary(max), N'Update After'), 222.22),
 (N'MergeCustomer', 40, CONVERT(varbinary(max), N'Merge Updated'), 400.00),
 (N'MergeCustomer', 42, CONVERT(varbinary(max), N'Merge Inserted'), 420.00),
 (N'DeleteCustomer', 30, CONVERT(varbinary(max), N'Delete Keep A'), 30.00),
 (N'DeleteCustomer', 32, CONVERT(varbinary(max), N'Delete Keep B'), 32.00);

DECLARE @Actual TABLE(TargetTable sysname, CustomerId int, CustomerName varbinary(max), TotalAmount decimal(18,2), AuditId bigint, InsertDateTime2 datetime2(7));
INSERT @Actual
 SELECT N'TargetCustomer', CustomerId, CONVERT(varbinary(max), CustomerName), TotalAmount, AuditId, InsertDateTime2 FROM dbo.TargetCustomer
 UNION ALL SELECT N'InsertTargetCustomer', CustomerId, CONVERT(varbinary(max), CustomerName), TotalAmount, AuditId, InsertDateTime2 FROM dbo.InsertTargetCustomer
 UNION ALL SELECT N'UpdateCustomer', CustomerId, CONVERT(varbinary(max), CustomerName), TotalAmount, AuditId, InsertDateTime2 FROM dbo.UpdateCustomer
 UNION ALL SELECT N'MergeCustomer', CustomerId, CONVERT(varbinary(max), CustomerName), TotalAmount, AuditId, InsertDateTime2 FROM dbo.MergeCustomer
 UNION ALL SELECT N'DeleteCustomer', CustomerId, CONVERT(varbinary(max), CustomerName), TotalAmount, AuditId, InsertDateTime2 FROM dbo.DeleteCustomer;
IF (SELECT COUNT(*) FROM @Actual) <> 11
 OR EXISTS (SELECT TargetTable, CustomerId, CustomerName, TotalAmount FROM @Actual EXCEPT SELECT * FROM @Expected)
 OR EXISTS (SELECT * FROM @Expected EXCEPT SELECT TargetTable, CustomerId, CustomerName, TotalAmount FROM @Actual)
 THROW 50000, 'Pipeline target rows differ from the exact authored expectations.', 1;
IF EXISTS (SELECT 1 FROM @Actual WHERE AuditId IS NULL OR InsertDateTime2 IS NULL)
 OR EXISTS (SELECT 1 FROM @Actual WHERE TargetTable IN (N'TargetCustomer', N'InsertTargetCustomer') AND AuditId <= 0)
 THROW 50000, 'Pipeline task audit defaults were not supplied.', 1;
PRINT 'Exact rows and audit defaults verified for all five target tables.';
