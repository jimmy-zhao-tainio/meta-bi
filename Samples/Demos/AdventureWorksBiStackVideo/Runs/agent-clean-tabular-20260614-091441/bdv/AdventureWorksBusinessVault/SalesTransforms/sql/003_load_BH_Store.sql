CREATE VIEW dbo.v_load_BH_Store
AS
SELECT
    h.HashKey,
    h.BusinessEntityID AS StoreId,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.H_Store') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_Store] AS h;
GO
