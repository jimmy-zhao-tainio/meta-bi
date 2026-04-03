CREATE VIEW dbo.v_builtin_table_functions
AS
SELECT
    g.value
FROM GENERATE_SERIES(1, 10) AS g
UNION ALL
SELECT
    x.value
FROM STRING_SPLIT('a,b,c', ',') AS x
GO
