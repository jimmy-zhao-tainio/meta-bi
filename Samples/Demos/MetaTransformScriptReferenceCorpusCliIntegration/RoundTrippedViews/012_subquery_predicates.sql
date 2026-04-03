CREATE VIEW dbo.v_subquery_predicates
AS
SELECT
    s.Id
FROM dbo.Source AS s
WHERE s.Amount > ALL (SELECT
    t.Amount
FROM dbo.Target AS t
WHERE t.GroupId = s.GroupId) AND s.Code = ANY (SELECT
    t.Code
FROM dbo.Target AS t
WHERE t.GroupId = s.GroupId) AND s.Id IN (SELECT
    t.SourceId
FROM dbo.Target AS t)
GO
