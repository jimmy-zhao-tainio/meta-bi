CREATE VIEW dbo.v_load_BH_SalesOrder
AS
SELECT
    h.HashKey,
    h.SalesOrderID AS SalesOrderId,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.H_SalesOrderHeader') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_SalesOrderHeader] AS h;
GO
