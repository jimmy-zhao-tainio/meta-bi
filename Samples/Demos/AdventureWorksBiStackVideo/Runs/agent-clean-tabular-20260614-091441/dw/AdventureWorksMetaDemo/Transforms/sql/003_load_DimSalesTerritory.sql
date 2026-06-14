CREATE OR ALTER VIEW awbi.v_load_DimSalesTerritory
AS
SELECT
    st.TerritoryId,
    stp.TerritoryName,
    stp.CountryRegionCode,
    stp.TerritoryGroup
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS st
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BHS_SalesTerritory_SalesTerritoryProfile] AS stp
    ON stp.HubHashKey = st.HashKey;
