CREATE VIEW dbo.v_load_BH_SalesPerson
AS
SELECT
    h.HashKey,
    h.BusinessEntityID AS SalesPersonBusinessEntityId,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.H_SalesPerson') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_SalesPerson] AS h;
GO
