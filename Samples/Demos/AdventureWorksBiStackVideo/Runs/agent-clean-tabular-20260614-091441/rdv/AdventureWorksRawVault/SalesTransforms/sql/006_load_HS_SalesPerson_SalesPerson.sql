CREATE VIEW dbo.v_load_HS_SalesPerson_SalesPerson
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), sp.BusinessEntityID))) AS HubHashKey,
    sp.SalesQuota,
    sp.Bonus,
    sp.CommissionPct,
    sp.SalesYTD,
    sp.SalesLastYear,
    sp.rowguid,
    sp.ModifiedDate,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        CONVERT(nvarchar(40), sp.SalesQuota),
        CONVERT(nvarchar(40), sp.Bonus),
        CONVERT(nvarchar(40), sp.CommissionPct),
        CONVERT(nvarchar(40), sp.SalesYTD),
        CONVERT(nvarchar(40), sp.SalesLastYear),
        CONVERT(nvarchar(36), sp.rowguid),
        CONVERT(nvarchar(30), sp.ModifiedDate, 126)))) AS HashDiff,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesPerson') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesPerson] AS sp;
GO
