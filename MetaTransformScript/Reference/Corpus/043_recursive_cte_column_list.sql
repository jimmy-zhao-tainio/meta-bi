CREATE VIEW dbo.v_recursive_cte_column_list AS
WITH Numbers (N) AS
(
    SELECT
        1
    UNION ALL
    SELECT
        Numbers.N + 1
    FROM Numbers
)
SELECT
    Numbers.N
FROM Numbers;
