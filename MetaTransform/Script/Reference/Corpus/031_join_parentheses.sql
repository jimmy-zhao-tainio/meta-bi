CREATE VIEW dbo.v_join_parentheses AS
SELECT
    a.Id,
    c.Name
FROM
    (dbo.A AS a INNER JOIN dbo.B AS b ON a.BId = b.Id)
    LEFT JOIN dbo.C AS c ON c.Id = a.CId;
