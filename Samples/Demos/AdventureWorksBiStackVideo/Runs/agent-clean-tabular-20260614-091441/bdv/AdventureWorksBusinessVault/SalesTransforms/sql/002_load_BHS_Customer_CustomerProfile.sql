CREATE VIEW dbo.v_load_BHS_Customer_CustomerProfile
AS
SELECT
    h.HashKey AS HubHashKey,
    hs.AccountNumber AS CustomerAccountNumber,
    CONVERT(nvarchar(40), CASE WHEN lcs.CustomerHashKey IS NULL THEN N'Individual' ELSE N'Store' END) AS CustomerType,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        hs.AccountNumber,
        CASE WHEN lcs.CustomerHashKey IS NULL THEN N'Individual' ELSE N'Store' END))) AS HashDiff,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.CustomerProfile') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_Customer] AS h
INNER JOIN [AdventureWorksRawVault].[dbo].[HS_Customer_Customer] AS hs
    ON hs.HubHashKey = h.HashKey
LEFT JOIN [AdventureWorksRawVault].[dbo].[L_CustomerStore] AS lcs
    ON lcs.CustomerHashKey = h.HashKey;
GO
