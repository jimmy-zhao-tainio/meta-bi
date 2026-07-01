CREATE VIEW dbo.v_distinct_predicate AS
SELECT
    s.Id
FROM dbo.Source AS s
WHERE s.Code IS DISTINCT FROM s.LegacyCode;
