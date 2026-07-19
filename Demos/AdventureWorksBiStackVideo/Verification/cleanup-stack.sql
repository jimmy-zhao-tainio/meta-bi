SET NOCOUNT ON;
USE master;

IF DB_ID(N'AdventureWorksAnalytics') IS NOT NULL
BEGIN
    ALTER DATABASE AdventureWorksAnalytics SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE AdventureWorksAnalytics;
END;

IF DB_ID(N'AdventureWorksBusinessVault') IS NOT NULL
BEGIN
    ALTER DATABASE AdventureWorksBusinessVault SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE AdventureWorksBusinessVault;
END;

IF DB_ID(N'AdventureWorksRawVault') IS NOT NULL
BEGIN
    ALTER DATABASE AdventureWorksRawVault SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE AdventureWorksRawVault;
END;

IF DB_ID(N'AdventureWorksMetaPipeline') IS NOT NULL
BEGIN
    ALTER DATABASE AdventureWorksMetaPipeline SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE AdventureWorksMetaPipeline;
END;

SELECT N'AdventureWorks demo databases removed' AS Status;
