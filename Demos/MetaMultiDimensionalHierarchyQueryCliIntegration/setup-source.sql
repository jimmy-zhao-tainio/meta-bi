IF DB_ID(N'$(DEMO_DB)') IS NULL
BEGIN
    CREATE DATABASE [$(DEMO_DB)];
END
GO

USE [$(DEMO_DB)];
GO

IF OBJECT_ID(N'dbo.Sales', N'U') IS NOT NULL DROP TABLE dbo.Sales;
IF OBJECT_ID(N'dbo.[Date]', N'U') IS NOT NULL DROP TABLE dbo.[Date];
GO

DECLARE @ssasLogin sysname = N'$(SSAS_LOGIN)';
IF @ssasLogin <> N''
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = @ssasLogin)
    BEGIN
        DECLARE @createLoginSql nvarchar(max) = N'CREATE LOGIN ' + QUOTENAME(@ssasLogin) + N' FROM WINDOWS;';
        EXEC sys.sp_executesql @createLoginSql;
    END

    IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = @ssasLogin)
    BEGIN
        DECLARE @createUserSql nvarchar(max) = N'CREATE USER ' + QUOTENAME(@ssasLogin) + N' FOR LOGIN ' + QUOTENAME(@ssasLogin) + N';';
        EXEC sys.sp_executesql @createUserSql;
    END

    IF ISNULL(IS_ROLEMEMBER(N'db_datareader', @ssasLogin), 0) = 0
    BEGIN
        DECLARE @grantSql nvarchar(max) = N'ALTER ROLE db_datareader ADD MEMBER ' + QUOTENAME(@ssasLogin) + N';';
        EXEC sys.sp_executesql @grantSql;
    END
END
GO

CREATE TABLE dbo.[Date]
(
    DateKey int NOT NULL,
    DateKey_Name nvarchar(20) NOT NULL,
    CalendarYear int NOT NULL,
    CalendarYear_Name nvarchar(10) NOT NULL,
    MonthNumber int NOT NULL,
    MonthNumber_Name nvarchar(20) NOT NULL,
    MonthName nvarchar(20) NOT NULL,
    CONSTRAINT PK_Date PRIMARY KEY (DateKey)
);
GO

CREATE TABLE dbo.Sales
(
    DateKey int NOT NULL,
    SalesAmount decimal(19, 4) NOT NULL
);
GO

INSERT INTO dbo.[Date]
(
    DateKey,
    DateKey_Name,
    CalendarYear,
    CalendarYear_Name,
    MonthNumber,
    MonthNumber_Name,
    MonthName
)
VALUES
    (20250101, N'2025-01-01', 2025, N'2025', 1, N'January', N'January'),
    (20250201, N'2025-02-01', 2025, N'2025', 2, N'February', N'February'),
    (20250301, N'2025-03-01', 2025, N'2025', 3, N'March', N'March');
GO

INSERT INTO dbo.Sales
(
    DateKey,
    SalesAmount
)
VALUES
    (20250101, 100.00),
    (20250201, 125.50),
    (20250301, 90.25);
GO
