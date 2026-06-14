CREATE VIEW dbo.v_load_BHS_Store_StoreProfile
AS
SELECT
    h.HashKey AS HubHashKey,
    hs.Name AS StoreName,
    CONVERT(binary(32), HASHBYTES('SHA2_256', hs.Name)) AS HashDiff,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.StoreProfile') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_Store] AS h
INNER JOIN [AdventureWorksRawVault].[dbo].[HS_Store_Store] AS hs
    ON hs.HubHashKey = h.HashKey;
GO
