CREATE VIEW dbo.v_negated_predicates AS
SELECT
    s.CustomerId
FROM dbo.Source AS s
WHERE s.CustomerId NOT IN (1, 2, 3)
    AND s.CustomerName NOT LIKE 'A%'
    AND s.Amount NOT BETWEEN 10 AND 20
    AND s.CustomerId NOT IN
    (
        SELECT
            b.CustomerId
        FROM dbo.BlockedCustomer AS b
    );
