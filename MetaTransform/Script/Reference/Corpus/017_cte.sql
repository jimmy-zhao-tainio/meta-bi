CREATE VIEW dbo.v_cte AS
WITH base_cte AS
(
    SELECT
        s.Id,
        s.ParentId,
        0 AS Depth
    FROM dbo.Source AS s
    WHERE s.ParentId IS NULL
),
recursive_cte AS
(
    SELECT
        b.Id,
        b.ParentId,
        b.Depth
    FROM base_cte AS b
    UNION ALL
    SELECT
        s.Id,
        s.ParentId,
        r.Depth + 1
    FROM dbo.Source AS s
    INNER JOIN recursive_cte AS r
        ON s.ParentId = r.Id
)
SELECT
    r.Id,
    r.ParentId,
    r.Depth
FROM recursive_cte AS r;
