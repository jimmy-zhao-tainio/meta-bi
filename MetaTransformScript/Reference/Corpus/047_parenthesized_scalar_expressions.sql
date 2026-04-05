CREATE VIEW dbo.v_parenthesized_scalar AS
SELECT
    (s.Amount + 1) AS AdjustedAmount,
    ((s.Amount + 1)) AS NestedAdjustedAmount,
    (CAST(s.Amount AS decimal(18, 2))) AS CastAmount
FROM dbo.Sales AS s
WHERE (s.Amount + 1) > 0;
