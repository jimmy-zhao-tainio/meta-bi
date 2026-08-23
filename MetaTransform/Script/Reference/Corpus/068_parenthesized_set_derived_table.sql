CREATE VIEW dbo.v_parenthesized_set_derived_table AS
SELECT
    d.CustomerZip
FROM
(
    SELECT
        t.CustomerZip
    FROM
    (
        (SELECT
            a.Zip AS CustomerZip
        FROM dbo.CustomerAddress AS a
        WHERE a.IsPreferred = 1)
        INTERSECT
        (SELECT
            b.Zip AS CustomerZip
        FROM dbo.PreferredCustomerZip AS b)
    ) AS t
) AS d;
