CREATE VIEW dbo.v_four_part_names AS
SELECT
    src.CustomerId,
    NEXT VALUE FOR UtilitySrv.UtilityDb.dbo.CustomerSequence AS NextCustomerSequence
FROM ReportingSrv.SalesDb.sales.Customer AS src
WHERE EXISTS
(
    SELECT 1
    FROM ArchiveSrv.ArchiveDb.sales.CustomerArchive AS arc
    WHERE arc.CustomerId = src.CustomerId
);
