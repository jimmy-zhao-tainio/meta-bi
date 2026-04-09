CREATE VIEW dbo.v_freetext AS
SELECT
    p.ProductId
FROM dbo.Products AS p
WHERE FREETEXT(p.Description, 'bike');
