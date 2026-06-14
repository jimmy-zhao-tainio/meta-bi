CREATE VIEW dbo.v_load_BHS_SalesTerritory_SalesTerritoryProfile
AS
SELECT
    h.HashKey AS HubHashKey,
    hs.Name AS TerritoryName,
    CONVERT(nvarchar(10), N'Unknown') AS CountryRegionCode,
    hs.[Group] AS TerritoryGroup,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|', hs.Name, N'Unknown', hs.[Group]))) AS HashDiff,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.SalesTerritoryProfile') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_SalesTerritory] AS h
INNER JOIN [AdventureWorksRawVault].[dbo].[HS_SalesTerritory_SalesTerritory] AS hs
    ON hs.HubHashKey = h.HashKey;
GO
