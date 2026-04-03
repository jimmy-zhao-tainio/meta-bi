CREATE VIEW dbo.v_unpivot AS
SELECT
    u.CustomerId,
    u.CategoryCode,
    u.Amount
FROM
(
    SELECT
        p.CustomerId,
        p.[A],
        p.[B]
    FROM dbo.PivotSource AS p
) AS src
UNPIVOT
(
    Amount FOR CategoryCode IN ([A], [B])
) AS u;
