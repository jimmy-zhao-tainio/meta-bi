CREATE VIEW dbo.v_load_BH_SalesTerritory
AS
SELECT
    h.HashKey,
    h.TerritoryID AS TerritoryId,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.H_SalesTerritory') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_SalesTerritory] AS h;
GO
