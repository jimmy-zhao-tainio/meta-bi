CREATE VIEW dbo.v_basic
AS
SELECT
    s.CustomerId,
    s.CustomerName AS CustomerName,
    s.CreatedAt AS CreatedAtAlias,
    'literal' AS LiteralValue
FROM dbo.SourceTable AS s
GO
