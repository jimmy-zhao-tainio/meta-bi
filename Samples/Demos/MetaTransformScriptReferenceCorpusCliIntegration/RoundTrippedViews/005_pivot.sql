CREATE VIEW dbo.v_pivot
AS
SELECT
    p.CustomerId,
    p.[A],
    p.[B]
FROM (
SELECT
    s.CustomerId,
    s.CategoryCode,
    s.Amount
FROM dbo.Sales AS s
) AS src
PIVOT
(
    SUM(Amount)
    FOR CategoryCode IN ([A], [B])
) AS p
GO
