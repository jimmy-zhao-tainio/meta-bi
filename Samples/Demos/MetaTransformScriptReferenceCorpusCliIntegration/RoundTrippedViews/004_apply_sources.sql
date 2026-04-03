CREATE VIEW dbo.v_apply_sources
AS
SELECT
    s.Id,
    splitItem.ValueText,
    applySource.MaxAmount
FROM dbo.SourceTable AS s
CROSS APPLY dbo.fnSplit(s.CsvValue) AS splitItem
OUTER APPLY (
SELECT
    MAX(o.Amount) AS MaxAmount
FROM dbo.Orders AS o
WHERE o.SourceId = s.Id
) AS applySource
GO
