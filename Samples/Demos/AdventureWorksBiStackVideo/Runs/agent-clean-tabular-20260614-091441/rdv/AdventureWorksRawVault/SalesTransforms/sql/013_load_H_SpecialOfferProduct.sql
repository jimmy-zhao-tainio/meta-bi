CREATE VIEW dbo.v_load_H_SpecialOfferProduct
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), sop.SpecialOfferID), N'|', CONVERT(nvarchar(256), sop.ProductID)))) AS HashKey,
    CONVERT(nvarchar(256), sop.SpecialOfferID) AS SpecialOfferID,
    CONVERT(nvarchar(256), sop.ProductID) AS ProductID,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SpecialOfferProduct') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SpecialOfferProduct] AS sop;
GO
