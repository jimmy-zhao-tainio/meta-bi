CREATE VIEW dbo.v_load_HS_Customer_Customer
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), c.CustomerID))) AS HubHashKey,
    c.AccountNumber,
    c.rowguid,
    c.ModifiedDate,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        c.AccountNumber,
        CONVERT(nvarchar(36), c.rowguid),
        CONVERT(nvarchar(30), c.ModifiedDate, 126)))) AS HashDiff,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.Customer') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[Customer] AS c;
GO
