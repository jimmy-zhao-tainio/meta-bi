CREATE VIEW dbo.v_group_by_all AS
SELECT
    s.CustomerId,
    COUNT(*) AS RowCount
FROM dbo.Sales AS s
GROUP BY ALL s.CustomerId;
