CREATE VIEW dbo.v_load_HS_ProductSubcategory_ProductSubcategory
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), ps.ProductSubcategoryID))) AS HubHashKey,
    ps.Name,
    ps.rowguid,
    ps.ModifiedDate,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        ps.Name,
        CONVERT(nvarchar(36), ps.rowguid),
        CONVERT(nvarchar(30), ps.ModifiedDate, 126)))) AS HashDiff,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Production.ProductSubcategory') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Production].[ProductSubcategory] AS ps;
GO
