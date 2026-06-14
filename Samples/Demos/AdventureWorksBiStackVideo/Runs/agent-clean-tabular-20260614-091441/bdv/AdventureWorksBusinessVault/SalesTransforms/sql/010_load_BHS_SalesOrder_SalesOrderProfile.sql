CREATE VIEW dbo.v_load_BHS_SalesOrder_SalesOrderProfile
AS
SELECT
    h.HashKey AS HubHashKey,
    hs.SalesOrderNumber,
    CONVERT(nvarchar(40), hs.Status) AS OrderStatus,
    hs.OnlineOrderFlag,
    CONVERT(date, hs.DueDate) AS DueDate,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        hs.SalesOrderNumber,
        CONVERT(nvarchar(40), hs.Status),
        CONVERT(nvarchar(1), hs.OnlineOrderFlag),
        CONVERT(nvarchar(30), hs.DueDate, 126)))) AS HashDiff,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.SalesOrderProfile') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_SalesOrderHeader] AS h
INNER JOIN [AdventureWorksRawVault].[dbo].[HS_SalesOrderHeader_SalesOrderHeader] AS hs
    ON hs.HubHashKey = h.HashKey;
GO
