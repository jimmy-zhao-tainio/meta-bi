IF DB_ID(N'MetaDataQualityCliIntegration') IS NULL
BEGIN
    CREATE DATABASE [MetaDataQualityCliIntegration];
END
GO

USE [MetaDataQualityCliIntegration];
GO

IF SCHEMA_ID(N'dq') IS NULL
BEGIN
    EXEC(N'CREATE SCHEMA [dq]');
END
GO

IF SCHEMA_ID(N'sales') IS NULL
BEGIN
    EXEC(N'CREATE SCHEMA [sales]');
END
GO

DECLARE @dropDqViews nvarchar(max) = N'';

SELECT @dropDqViews = @dropDqViews + N'DROP VIEW ' + QUOTENAME(SCHEMA_NAME(schema_id)) + N'.' + QUOTENAME(name) + N';' + CHAR(13) + CHAR(10)
FROM sys.views
WHERE schema_id = SCHEMA_ID(N'dq');

IF @dropDqViews <> N''
BEGIN
    EXEC sp_executesql @dropDqViews;
END
GO

IF OBJECT_ID(N'sales.[Order]', N'U') IS NOT NULL DROP TABLE sales.[Order];
IF OBJECT_ID(N'sales.Invoice', N'U') IS NOT NULL DROP TABLE sales.Invoice;
IF OBJECT_ID(N'sales.Customer', N'U') IS NOT NULL DROP TABLE sales.Customer;
GO

CREATE TABLE sales.Customer
(
    CustomerId int NOT NULL,
    RegionId int NOT NULL,
    CustomerName nvarchar(100) NOT NULL,
    CONSTRAINT PK_sales_Customer PRIMARY KEY (CustomerId, RegionId)
);
GO

CREATE TABLE sales.[Order]
(
    OrderId int NOT NULL,
    CustomerId int NOT NULL,
    RegionId int NOT NULL,
    Amount decimal(18, 2) NOT NULL,
    CONSTRAINT PK_sales_Order PRIMARY KEY (OrderId)
);
GO

CREATE TABLE sales.Invoice
(
    InvoiceId int NOT NULL,
    CustomerId int NOT NULL,
    RegionId int NOT NULL,
    Amount decimal(18, 2) NOT NULL,
    CONSTRAINT PK_sales_Invoice PRIMARY KEY (InvoiceId)
);
GO

INSERT INTO sales.Customer
(
    CustomerId,
    RegionId,
    CustomerName
)
VALUES
    (1, 10, N'Acme North'),
    (2, 10, N'Beacon Retail'),
    (3, 20, N'Contoso Labs'),
    (4, 30, N'Datum Field');
GO

INSERT INTO sales.[Order]
(
    OrderId,
    CustomerId,
    RegionId,
    Amount
)
VALUES
    (1001, 1, 10, 125.00),
    (1002, 1, 10, 50.00),
    (1003, 2, 10, 42.00),
    (1999, 999, 10, 13.37);
GO

INSERT INTO sales.Invoice
(
    InvoiceId,
    CustomerId,
    RegionId,
    Amount
)
VALUES
    (2001, 1, 10, 125.00),
    (2002, 1, 10, 75.00),
    (2999, 888, 20, 22.22);
GO
