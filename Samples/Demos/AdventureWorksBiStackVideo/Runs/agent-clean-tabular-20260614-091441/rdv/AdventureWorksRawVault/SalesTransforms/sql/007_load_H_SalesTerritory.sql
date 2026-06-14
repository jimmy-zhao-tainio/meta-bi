CREATE VIEW dbo.v_load_H_SalesTerritory
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), st.TerritoryID))) AS HashKey,
    CONVERT(nvarchar(256), st.TerritoryID) AS TerritoryID,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesTerritory') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesTerritory] AS st;
GO
