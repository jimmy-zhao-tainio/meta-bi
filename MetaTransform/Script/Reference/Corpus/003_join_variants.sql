CREATE VIEW dbo.v_join_variants AS
SELECT
    a.Id,
    b.Name,
    c.Flag,
    e.Code
FROM dbo.A AS a
INNER JOIN dbo.B AS b
    ON a.BId = b.Id
LEFT OUTER JOIN dbo.C AS c
    ON c.AId = a.Id
RIGHT OUTER JOIN dbo.D AS d
    ON d.Id = a.DId
FULL OUTER JOIN dbo.E AS e
    ON e.Id = d.EId
CROSS JOIN dbo.F AS f;
