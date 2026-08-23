CREATE VIEW dbo.v_leading_dot_numeric_literals AS
SELECT
    .5 AS LeadingDotNumeric,
    .5E2 AS LeadingDotUpperExponent,
    .5e-2 AS LeadingDotLowerExponent,
    -.5 AS NegativeLeadingDotNumeric,
    +.5 AS PositiveLeadingDotNumeric,
    s.Value AS QualifiedValue
FROM schema.table AS s;
