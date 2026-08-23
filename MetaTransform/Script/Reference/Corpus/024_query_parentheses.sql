CREATE VIEW dbo.v_query_parentheses AS
(SELECT DISTINCT s.Id, s.Code FROM dbo.Source AS s)
UNION ALL
(SELECT DISTINCT t.Id, t.Code FROM dbo.Target AS t);
