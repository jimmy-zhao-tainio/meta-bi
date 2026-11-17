IF DB_ID(N'RawDataVaultSchemaSourceDemo') IS NULL
    CREATE DATABASE RawDataVaultSchemaSourceDemo;
GO
USE RawDataVaultSchemaSourceDemo;
GO
CREATE TABLE dbo.Customer
(
    CustomerId nvarchar(20) NOT NULL PRIMARY KEY,
    CustomerName nvarchar(100) NOT NULL
);
CREATE TABLE dbo.SalesOrder
(
    OrderId int NOT NULL PRIMARY KEY,
    CustomerId nvarchar(20) NOT NULL REFERENCES dbo.Customer(CustomerId),
    OrderDate date NOT NULL,
    TotalAmount decimal(18,2) NOT NULL
);
INSERT dbo.Customer VALUES (N'C1', N'Åsa'), (N'C2', N'Customer Two');
INSERT dbo.SalesOrder VALUES (1,N'C1','20260101',12.50), (2,N'C2','20260102',45.00);
