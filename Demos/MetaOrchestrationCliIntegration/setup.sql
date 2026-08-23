IF DB_ID(N'MetaOrchestrationCliIntegration') IS NOT NULL
BEGIN
    ALTER DATABASE [MetaOrchestrationCliIntegration] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [MetaOrchestrationCliIntegration];
END;
GO

CREATE DATABASE [MetaOrchestrationCliIntegration];
GO

USE [MetaOrchestrationCliIntegration];
GO

CREATE TABLE dbo.RawCustomer
(
    CustomerId int NOT NULL,
    CustomerName nvarchar(100) NOT NULL
);

CREATE TABLE dbo.RawCustomerDelta
(
    CustomerId int NOT NULL,
    CustomerName nvarchar(100) NOT NULL
);

CREATE TABLE dbo.RawOrder
(
    OrderId int NOT NULL,
    CustomerId int NOT NULL,
    Amount decimal(18, 2) NOT NULL
);

CREATE TABLE dbo.RawExchangeRate
(
    CurrencyCode nvarchar(3) NOT NULL,
    Rate decimal(18, 6) NOT NULL
);

CREATE TABLE dbo.StageCustomer
(
    CustomerId int NOT NULL,
    CustomerName nvarchar(100) NOT NULL
);

CREATE TABLE dbo.StageOrder
(
    OrderId int NOT NULL,
    CustomerId int NOT NULL,
    Amount decimal(18, 2) NOT NULL
);

CREATE TABLE dbo.DimCustomer
(
    CustomerId int NOT NULL,
    CustomerName nvarchar(100) NOT NULL
);

CREATE TABLE dbo.FactSales
(
    OrderId int NOT NULL,
    CustomerId int NOT NULL,
    Amount decimal(18, 2) NOT NULL
);

CREATE TABLE dbo.WorkExchangeRate
(
    CurrencyCode nvarchar(3) NOT NULL,
    Rate decimal(18, 6) NOT NULL
);

CREATE TABLE dbo.PrivateScratch
(
    Id int NOT NULL
);

CREATE TABLE dbo.SharedLanding
(
    CustomerId int NOT NULL,
    CustomerName nvarchar(100) NOT NULL
);

CREATE TABLE dbo.OrchestrationFailureLog
(
    Message nvarchar(200) NOT NULL,
    LoggedAtUtc datetime2(7) NOT NULL CONSTRAINT DF_OrchestrationFailureLog_LoggedAtUtc DEFAULT (SYSUTCDATETIME())
);
GO
