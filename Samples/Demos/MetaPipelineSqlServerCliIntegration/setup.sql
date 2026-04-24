IF DB_ID(N'MetaPipelineSqlServerCliIntegration') IS NULL
BEGIN
    CREATE DATABASE [MetaPipelineSqlServerCliIntegration];
END
GO

USE [MetaPipelineSqlServerCliIntegration];
GO

IF OBJECT_ID(N'dbo.TargetCustomer', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.TargetCustomer;
END
GO

IF OBJECT_ID(N'dbo.SourceCustomer', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.SourceCustomer;
END
GO

CREATE TABLE dbo.SourceCustomer
(
    CustomerId int NOT NULL,
    CustomerName nvarchar(100) NOT NULL,
    TotalAmount decimal(18, 2) NOT NULL
);
GO

CREATE TABLE dbo.TargetCustomer
(
    CustomerId int NOT NULL,
    CustomerName nvarchar(100) NOT NULL,
    TotalAmount decimal(18, 2) NOT NULL
);
GO

INSERT INTO dbo.SourceCustomer
(
    CustomerId,
    CustomerName,
    TotalAmount
)
VALUES
    (1, N'Acme North', 125.50),
    (2, N'Beacon Retail', 980.00),
    (3, N'Contoso Labs', 42.75);
GO
