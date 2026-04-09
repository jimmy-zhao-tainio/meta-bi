CREATE VIEW dbo.v_freetext_table AS
SELECT
    ft.[KEY],
    ft.RANK
FROM FREETEXTTABLE(dbo.Products, Description, 'bike') AS ft;
