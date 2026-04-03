CREATE VIEW dbo.v_offset_fetch
AS
SELECT
    s.Id,
    s.OrderDate
FROM dbo.Source AS s
ORDER BY s.OrderDate DESC
OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY
GO
