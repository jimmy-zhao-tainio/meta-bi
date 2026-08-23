IF DB_ID(N'$(DEMO_DB)') IS NULL
BEGIN
    CREATE DATABASE [$(DEMO_DB)];
END
GO

USE [$(DEMO_DB)];
GO

IF SCHEMA_ID(N'dqdemo') IS NULL
BEGIN
    EXEC(N'CREATE SCHEMA [dqdemo]');
END
GO

IF SCHEMA_ID(N'dq') IS NULL
BEGIN
    EXEC(N'CREATE SCHEMA [dq]');
END
GO

DECLARE @dropDqViews nvarchar(max) = N'';
SELECT @dropDqViews = @dropDqViews + N'DROP VIEW ' + QUOTENAME(SCHEMA_NAME(schema_id)) + N'.' + QUOTENAME(name) + N';' + CHAR(13) + CHAR(10)
FROM sys.views
WHERE schema_id IN (SCHEMA_ID(N'dq'));

IF @dropDqViews <> N''
BEGIN
    EXEC sp_executesql @dropDqViews;
END
GO

IF OBJECT_ID(N'dqdemo.OrderHeader', N'U') IS NOT NULL DROP TABLE dqdemo.OrderHeader;
IF OBJECT_ID(N'dqdemo.Campaign', N'U') IS NOT NULL DROP TABLE dqdemo.Campaign;
IF OBJECT_ID(N'dqdemo.Customer', N'U') IS NOT NULL DROP TABLE dqdemo.Customer;
GO

CREATE TABLE dqdemo.Customer
(
    CompanyId int NOT NULL,
    CustomerId int NOT NULL,
    CustomerNo varchar(25) NULL,
    CustomerName varchar(100) NULL,
    IsDeleted bit NOT NULL CONSTRAINT DF_dqdemo_Customer_IsDeleted DEFAULT (0)
);
GO

CREATE TABLE dqdemo.OrderHeader
(
    CompanyId int NOT NULL,
    OrderId int NOT NULL,
    CustomerId int NOT NULL,
    CampaignId int NULL,
    Amount decimal(18, 2) NOT NULL
);
GO

CREATE TABLE dqdemo.Campaign
(
    CompanyId int NOT NULL,
    CampaignId int NOT NULL,
    CampaignName varchar(100) NULL
);
GO

INSERT INTO dqdemo.Customer
(
    CompanyId,
    CustomerId,
    CustomerNo,
    CustomerName,
    IsDeleted
)
VALUES
    (1, 100, 'C100-A', 'Acme Retail', 0),
    (1, 100, 'C100-B', 'Acme Retail Duplicate', 0),
    (1, 101, 'C101-A', 'Beacon Trade', 0),
    (1, 102, 'C102-A', 'Contoso North', 0),
    (1, 103, 'C103-A', 'Deleted Customer', 1),
    (2, 200, 'C200-A', 'Northwind AB', 0);
GO

INSERT INTO dqdemo.Campaign
(
    CompanyId,
    CampaignId,
    CampaignName
)
VALUES
    (1, 500, 'CMP-500'),
    (1, 501, 'CMP-501'),
    (2, 700, 'CMP-700');
GO

INSERT INTO dqdemo.OrderHeader
(
    CompanyId,
    OrderId,
    CustomerId,
    CampaignId,
    Amount
)
VALUES
    (1, 1000, 100, 500, 120.00),
    (1, 1001, 101, 500, 80.00),
    (1, 1002, 102, 501, 60.00),
    (2, 2000, 200, 700, 50.00),
    (1, 1003, 999, 500, 30.00),   -- missing customer reference
    (1, 1004, 101, 999, 15.00),   -- missing campaign reference
    (1, 1005, 102, NULL, 10.00);  -- optional campaign
GO
