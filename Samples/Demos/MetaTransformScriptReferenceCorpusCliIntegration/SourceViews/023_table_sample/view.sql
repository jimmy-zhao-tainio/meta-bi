CREATE VIEW dbo.v_table_sample AS
SELECT
    s.Id
FROM dbo.SampleSource AS s TABLESAMPLE (10 PERCENT) REPEATABLE (123);
