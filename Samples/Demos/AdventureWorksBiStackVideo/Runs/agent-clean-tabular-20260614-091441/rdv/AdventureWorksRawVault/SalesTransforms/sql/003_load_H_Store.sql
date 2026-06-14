CREATE VIEW dbo.v_load_H_Store
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), s.BusinessEntityID))) AS HashKey,
    CONVERT(nvarchar(256), s.BusinessEntityID) AS BusinessEntityID,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.Store') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[Store] AS s;
GO
