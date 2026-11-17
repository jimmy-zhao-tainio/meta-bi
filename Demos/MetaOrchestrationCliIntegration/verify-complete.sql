-- Independent expectations for setup.sql; binary strings preserve spaces and empty text.
DECLARE @Customers TABLE(CustomerId int, CustomerName varbinary(max));
INSERT @Customers VALUES (1, CONVERT(varbinary(max), N'Åsa äter räksmörgås.')), (2, 0x);
IF (SELECT COUNT(*) FROM dbo.StageCustomer) <> 2
 OR (SELECT COUNT(*) FROM dbo.DimCustomer) <> 2
 OR EXISTS (SELECT CustomerId, CONVERT(varbinary(max), CustomerName) FROM dbo.StageCustomer EXCEPT SELECT * FROM @Customers)
 OR EXISTS (SELECT * FROM @Customers EXCEPT SELECT CustomerId, CONVERT(varbinary(max), CustomerName) FROM dbo.StageCustomer)
 OR EXISTS (SELECT CustomerId, CONVERT(varbinary(max), CustomerName) FROM dbo.DimCustomer EXCEPT SELECT * FROM @Customers)
 OR EXISTS (SELECT * FROM @Customers EXCEPT SELECT CustomerId, CONVERT(varbinary(max), CustomerName) FROM dbo.DimCustomer)
 THROW 50000, 'Customer staging/dimension values differ from the authored source rows.', 1;

DECLARE @Orders TABLE(OrderId int, CustomerId int, Amount decimal(18,2));
INSERT @Orders VALUES (101, 1, 12.50), (102, 2, 0.00);
IF (SELECT COUNT(*) FROM dbo.StageOrder) <> 2
 OR (SELECT COUNT(*) FROM dbo.FactSales) <> 2
 OR EXISTS (SELECT OrderId, CustomerId, Amount FROM dbo.StageOrder EXCEPT SELECT * FROM @Orders)
 OR EXISTS (SELECT * FROM @Orders EXCEPT SELECT OrderId, CustomerId, Amount FROM dbo.StageOrder)
 OR EXISTS (SELECT OrderId, CustomerId, Amount FROM dbo.FactSales EXCEPT SELECT * FROM @Orders)
 OR EXISTS (SELECT * FROM @Orders EXCEPT SELECT OrderId, CustomerId, Amount FROM dbo.FactSales)
 THROW 50000, 'Order staging/fact values differ from the authored source rows.', 1;

DECLARE @Rates TABLE(CurrencyCode varbinary(max), Rate decimal(18,6));
INSERT @Rates VALUES (CONVERT(varbinary(max), N'SEK'), 1.000000), (CONVERT(varbinary(max), N'EUR'), 11.250000);
IF (SELECT COUNT(*) FROM dbo.WorkExchangeRate) <> 2
 OR EXISTS (SELECT CONVERT(varbinary(max), CurrencyCode), Rate FROM dbo.WorkExchangeRate EXCEPT SELECT * FROM @Rates)
 OR EXISTS (SELECT * FROM @Rates EXCEPT SELECT CONVERT(varbinary(max), CurrencyCode), Rate FROM dbo.WorkExchangeRate)
 OR EXISTS (SELECT 1 FROM dbo.PrivateScratch)
 THROW 50000, 'Refresh/truncate results differ from the authored expectations.', 1;
PRINT 'Exact customers, orders, rates and scratch-table results verified.';
