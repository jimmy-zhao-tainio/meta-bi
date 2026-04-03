CREATE VIEW dbo.v_set_operations
AS
SELECT
    a.Id,
    a.Code
FROM dbo.A AS a
UNION ALL
SELECT
    b.Id,
    b.Code
FROM dbo.B AS b
UNION
SELECT
    c.Id,
    c.Code
FROM dbo.C AS c
EXCEPT
SELECT
    d.Id,
    d.Code
FROM dbo.D AS d
INTERSECT
SELECT
    e.Id,
    e.Code
FROM dbo.E AS e
GO
