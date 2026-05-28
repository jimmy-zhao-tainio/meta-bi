CREATE VIEW dbo.v_cross_database_names AS
SELECT
    src.CustomerId,
    NEXT VALUE FOR UtilityDb.dbo.CustomerSequence AS NextCustomerSequence,
    splitItem.ValueText
FROM SalesDb.sales.Customer AS src
CROSS APPLY UtilityDb.dbo.fnSplit(src.TagList) AS splitItem
WHERE EXISTS
(
    SELECT 1
    FROM ArchiveDb.sales.CustomerArchive AS arc
    WHERE arc.CustomerId = src.CustomerId
);
