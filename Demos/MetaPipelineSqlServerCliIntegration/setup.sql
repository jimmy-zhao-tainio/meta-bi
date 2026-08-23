IF DB_ID(N'MetaPipelineSqlServerCliIntegration') IS NULL
BEGIN
    CREATE DATABASE [MetaPipelineSqlServerCliIntegration];
END
GO

USE [MetaPipelineSqlServerCliIntegration];
GO

DECLARE @SetupAuditId bigint = -1;
DECLARE @SetupStartedAtUtc datetime2(7) = SYSUTCDATETIME();
EXEC sys.sp_set_session_context @key = N'MetaPipeline.AuditId', @value = @SetupAuditId;
EXEC sys.sp_set_session_context @key = N'MetaPipeline.TaskStartedAtUtc', @value = @SetupStartedAtUtc;
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

IF OBJECT_ID(N'dbo.InsertTargetCustomer', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.InsertTargetCustomer;
END
GO

IF OBJECT_ID(N'dbo.InsertSourceCustomer', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.InsertSourceCustomer;
END
GO

IF OBJECT_ID(N'dbo.UpdateCustomer', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.UpdateCustomer;
END
GO

IF OBJECT_ID(N'dbo.MergeCustomer', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.MergeCustomer;
END
GO

IF OBJECT_ID(N'dbo.MergeSourceCustomer', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.MergeSourceCustomer;
END
GO

IF OBJECT_ID(N'dbo.DeleteCustomer', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.DeleteCustomer;
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
    TotalAmount decimal(18, 2) NOT NULL,
    AuditId bigint NOT NULL
        CONSTRAINT DF_TargetCustomer_AuditId
        DEFAULT (CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))),
    InsertDateTime2 datetime2(7) NOT NULL
        CONSTRAINT DF_TargetCustomer_InsertDateTime2
        DEFAULT (CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc')))
);
GO

CREATE TABLE dbo.InsertSourceCustomer
(
    CustomerId int NOT NULL,
    CustomerName nvarchar(100) NOT NULL,
    TotalAmount decimal(18, 2) NOT NULL
);
GO

CREATE TABLE dbo.InsertTargetCustomer
(
    CustomerId int NOT NULL,
    CustomerName nvarchar(100) NOT NULL,
    TotalAmount decimal(18, 2) NOT NULL,
    AuditId bigint NOT NULL
        CONSTRAINT DF_InsertTargetCustomer_AuditId
        DEFAULT (CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))),
    InsertDateTime2 datetime2(7) NOT NULL
        CONSTRAINT DF_InsertTargetCustomer_InsertDateTime2
        DEFAULT (CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc')))
);
GO

CREATE TABLE dbo.UpdateCustomer
(
    CustomerId int NOT NULL,
    CustomerName nvarchar(100) NOT NULL,
    TotalAmount decimal(18, 2) NOT NULL,
    AuditId bigint NOT NULL
        CONSTRAINT DF_UpdateCustomer_AuditId
        DEFAULT (CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))),
    InsertDateTime2 datetime2(7) NOT NULL
        CONSTRAINT DF_UpdateCustomer_InsertDateTime2
        DEFAULT (CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc')))
);
GO

CREATE TABLE dbo.MergeSourceCustomer
(
    CustomerId int NOT NULL,
    CustomerName nvarchar(100) NOT NULL,
    TotalAmount decimal(18, 2) NOT NULL
);
GO

CREATE TABLE dbo.MergeCustomer
(
    CustomerId int NOT NULL,
    CustomerName nvarchar(100) NOT NULL,
    TotalAmount decimal(18, 2) NOT NULL,
    AuditId bigint NOT NULL
        CONSTRAINT DF_MergeCustomer_AuditId
        DEFAULT (CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))),
    InsertDateTime2 datetime2(7) NOT NULL
        CONSTRAINT DF_MergeCustomer_InsertDateTime2
        DEFAULT (CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc')))
);
GO

CREATE TABLE dbo.DeleteCustomer
(
    CustomerId int NOT NULL,
    CustomerName nvarchar(100) NOT NULL,
    TotalAmount decimal(18, 2) NOT NULL,
    AuditId bigint NOT NULL
        CONSTRAINT DF_DeleteCustomer_AuditId
        DEFAULT (CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))),
    InsertDateTime2 datetime2(7) NOT NULL
        CONSTRAINT DF_DeleteCustomer_InsertDateTime2
        DEFAULT (CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc')))
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

INSERT INTO dbo.TargetCustomer
(
    CustomerId,
    CustomerName,
    TotalAmount
)
VALUES
    (99, N'Stale target row', 1.00);
GO

INSERT INTO dbo.InsertSourceCustomer
(
    CustomerId,
    CustomerName,
    TotalAmount
)
VALUES
    (10, N'Inserted Alpha', 10.10),
    (11, N'Inserted Beta', 11.11);
GO

INSERT INTO dbo.UpdateCustomer
(
    CustomerId,
    CustomerName,
    TotalAmount
)
VALUES
    (20, N'Update Keep', 20.00),
    (21, N'Update Before', 21.00);
GO

INSERT INTO dbo.MergeCustomer
(
    CustomerId,
    CustomerName,
    TotalAmount
)
VALUES
    (40, N'Merge Before', 40.00),
    (41, N'Merge Stale', 41.00);
GO

INSERT INTO dbo.MergeSourceCustomer
(
    CustomerId,
    CustomerName,
    TotalAmount
)
VALUES
    (40, N'Merge Updated', 400.00),
    (42, N'Merge Inserted', 420.00);
GO

INSERT INTO dbo.DeleteCustomer
(
    CustomerId,
    CustomerName,
    TotalAmount
)
VALUES
    (30, N'Delete Keep A', 30.00),
    (31, N'Delete Remove', 31.00),
    (32, N'Delete Keep B', 32.00);
GO
