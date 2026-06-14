CREATE VIEW dbo.v_load_HS_SalesOrderDetail_SalesOrderDetail
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), sod.SalesOrderID), N'|', CONVERT(nvarchar(256), sod.SalesOrderDetailID)))) AS HubHashKey,
    sod.CarrierTrackingNumber,
    sod.OrderQty,
    sod.UnitPrice,
    sod.UnitPriceDiscount,
    sod.LineTotal,
    sod.rowguid,
    sod.ModifiedDate,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        sod.CarrierTrackingNumber,
        CONVERT(nvarchar(20), sod.OrderQty),
        CONVERT(nvarchar(40), sod.UnitPrice),
        CONVERT(nvarchar(40), sod.UnitPriceDiscount),
        CONVERT(nvarchar(40), sod.LineTotal),
        CONVERT(nvarchar(36), sod.rowguid),
        CONVERT(nvarchar(30), sod.ModifiedDate, 126)))) AS HashDiff,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesOrderDetail') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesOrderDetail] AS sod;
GO
