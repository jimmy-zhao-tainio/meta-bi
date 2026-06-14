CREATE VIEW dbo.v_load_BHS_SalesPerson_SalesPersonProfile
AS
SELECT
    h.HashKey AS HubHashKey,
    CONVERT(nvarchar(150), CONCAT(N'Salesperson ', h.BusinessEntityID)) AS SalesPersonName,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT(N'Salesperson ', h.BusinessEntityID))) AS HashDiff,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.SalesPersonProfile') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_SalesPerson] AS h;
GO
