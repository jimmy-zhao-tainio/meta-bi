CREATE VIEW dbo.v_ordering_and_top AS
SELECT TOP (100) PERCENT WITH TIES
    s.Id,
    s.OrderDate
FROM dbo.Source AS s
ORDER BY
    s.OrderDate DESC,
    s.Id ASC;
