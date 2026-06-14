CREATE VIEW dbo.v_load_HS_SalesTerritory_SalesTerritory
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), st.TerritoryID))) AS HubHashKey,
    st.Name,
    st.[Group],
    st.SalesYTD,
    st.SalesLastYear,
    st.CostYTD,
    st.CostLastYear,
    st.rowguid,
    st.ModifiedDate,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        st.Name,
        st.[Group],
        CONVERT(nvarchar(40), st.SalesYTD),
        CONVERT(nvarchar(40), st.SalesLastYear),
        CONVERT(nvarchar(40), st.CostYTD),
        CONVERT(nvarchar(40), st.CostLastYear),
        CONVERT(nvarchar(36), st.rowguid),
        CONVERT(nvarchar(30), st.ModifiedDate, 126)))) AS HashDiff,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesTerritory') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesTerritory] AS st;
GO
