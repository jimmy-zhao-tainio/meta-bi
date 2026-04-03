CREATE VIEW dbo.v_odbc_surface AS
SELECT
    { fn UCASE('abc') } AS UpperValue,
    { fn CONVERT(1, SQL_INTEGER) } AS ConvertedValue,
    { d '2026-01-01' } AS DateValue
FROM { OJ dbo.A AS a LEFT OUTER JOIN dbo.B AS b ON a.Id = b.AId };
