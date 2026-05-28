CREATE VIEW dbo.v_inline_values AS
SELECT
    src.Id,
    src.Name
FROM
(
    VALUES
        (1, 'One'),
        (2, 'Two')
) AS src(Id, Name);
