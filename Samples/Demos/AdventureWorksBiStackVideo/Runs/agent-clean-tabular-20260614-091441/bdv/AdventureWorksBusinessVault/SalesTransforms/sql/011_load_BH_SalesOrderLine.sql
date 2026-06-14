CREATE VIEW dbo.v_load_BH_SalesOrderLine
AS
SELECT
    h.HashKey,
    h.SalesOrderID AS SalesOrderId,
    h.SalesOrderDetailID AS SalesOrderDetailId,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.H_SalesOrderDetail') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_SalesOrderDetail] AS h;
GO
