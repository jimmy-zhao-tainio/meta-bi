CREATE VIEW dbo.v_fulltext
AS
SELECT
    p.ProductId
FROM dbo.Products AS p
WHERE CONTAINS(p.Description, 'bike')
GO
