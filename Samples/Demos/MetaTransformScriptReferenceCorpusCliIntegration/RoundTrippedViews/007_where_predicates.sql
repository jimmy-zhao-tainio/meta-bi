CREATE VIEW dbo.v_where_predicates
AS
SELECT
    s.Id
FROM dbo.Source AS s
WHERE NOT (s.Status = 'X' OR s.Amount BETWEEN 10 AND 20 OR s.Code IN ('A', 'B', 'C') OR s.Name LIKE 'AB%' OR s.DeletedAt IS NULL) AND s.Score > 0
GO
