CREATE VIEW dbo.v_remaining_data_types AS
SELECT
    CAST(s.SmallNumberText AS smallint) AS SmallNumberValue,
    TRY_CONVERT(tinyint, s.TinyNumberText) AS TinyNumberValue,
    CAST(s.AmountText AS numeric(18, 4)) AS PreciseAmount,
    CAST(s.ScoreText AS real) AS ScoreValue,
    CONVERT(char(8), s.CodeText) AS FixedCode,
    CONVERT(nchar(12), s.NameText) AS FixedNationalName
FROM dbo.TypeSource AS s;
