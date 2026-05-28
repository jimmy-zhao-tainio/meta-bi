CREATE VIEW dbo.v_nested_subqueries AS
SELECT
    s.CustomerId,
    (
        SELECT
            (
                SELECT MAX(i.Amount)
                FROM dbo.Sales AS i
                WHERE i.CustomerId = s.CustomerId
            )
    ) AS NestedMaxAmount
FROM dbo.Customer AS s;
