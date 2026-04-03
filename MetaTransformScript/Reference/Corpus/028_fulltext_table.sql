CREATE VIEW dbo.v_fulltext_table AS
SELECT
    ft.[KEY],
    ft.RANK
FROM CONTAINSTABLE(dbo.Products, Description, 'bike') AS ft;
