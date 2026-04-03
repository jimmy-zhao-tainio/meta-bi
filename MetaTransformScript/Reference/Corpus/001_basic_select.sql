CREATE VIEW dbo.v_basic AS
SELECT
    s.CustomerId,
    CustomerName = s.CustomerName,
    s.CreatedAt AS CreatedAtAlias,
    'literal' AS LiteralValue
FROM dbo.SourceTable AS s;
