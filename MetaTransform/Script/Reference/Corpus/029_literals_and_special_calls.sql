CREATE VIEW dbo.v_literals_and_special_calls AS
SELECT
    -1 AS NegativeInt,
    +1.25 AS PositiveNumeric,
    1E0 AS RealValue,
    0xCAFE AS BinaryValue,
    NULL AS NullValue,
    PARSE('2026-01-01' AS datetime2 USING 'en-US') AS ParsedDate,
    TRY_PARSE('12.34' AS decimal(10, 2) USING 'en-US') AS TryParsedNumber,
    LEFT('abcdef', 3) AS LeftValue,
    RIGHT('abcdef', 2) AS RightValue,
    CURRENT_TIMESTAMP AS CurrentTimestamp
FROM dbo.Source AS s;
