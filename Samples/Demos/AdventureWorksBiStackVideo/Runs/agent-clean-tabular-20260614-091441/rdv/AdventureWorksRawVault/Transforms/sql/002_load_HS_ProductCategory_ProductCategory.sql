CREATE VIEW dbo.v_load_HS_ProductCategory_ProductCategory
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), pc.ProductCategoryID))) AS HubHashKey,
    pc.Name,
    pc.rowguid,
    pc.ModifiedDate,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        pc.Name,
        CONVERT(nvarchar(36), pc.rowguid),
        CONVERT(nvarchar(30), pc.ModifiedDate, 126)))) AS HashDiff,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Production.ProductCategory') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Production].[ProductCategory] AS pc;
GO
