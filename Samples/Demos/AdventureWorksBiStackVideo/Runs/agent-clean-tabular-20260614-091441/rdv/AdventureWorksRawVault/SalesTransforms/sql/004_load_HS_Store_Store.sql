CREATE VIEW dbo.v_load_HS_Store_Store
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), s.BusinessEntityID))) AS HubHashKey,
    s.Name,
    s.Demographics,
    s.rowguid,
    s.ModifiedDate,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        s.Name,
        CONVERT(nvarchar(max), s.Demographics),
        CONVERT(nvarchar(36), s.rowguid),
        CONVERT(nvarchar(30), s.ModifiedDate, 126)))) AS HashDiff,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.Store') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[Store] AS s;
GO
