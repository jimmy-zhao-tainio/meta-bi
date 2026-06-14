CREATE VIEW dbo.v_load_L_SpecialOfferProductProduct
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), sop.SpecialOfferID), N'|', CONVERT(nvarchar(256), sop.ProductID), N'|', CONVERT(nvarchar(256), sop.ProductID)))) AS HashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), sop.SpecialOfferID), N'|', CONVERT(nvarchar(256), sop.ProductID)))) AS SpecialOfferProductHashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), sop.ProductID))) AS ProductHashKey,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SpecialOfferProduct') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SpecialOfferProduct] AS sop;
GO
