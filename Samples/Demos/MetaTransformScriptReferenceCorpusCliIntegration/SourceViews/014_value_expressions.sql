CREATE VIEW dbo.v_value_expressions AS
SELECT
    s.Id,
    CASE s.Status
        WHEN 'A' THEN 'Active'
        WHEN 'I' THEN 'Inactive'
        ELSE 'Other'
    END AS StatusText,
    CASE
        WHEN s.Amount > 100 THEN 'High'
        ELSE 'Low'
    END AS AmountBand,
    COALESCE(s.PreferredName, s.LegalName, 'Unknown') AS DisplayName,
    NULLIF(s.Code, '') AS NormalizedCode,
    IIF(s.IsActive = 1, s.Amount, 0) AS ActiveAmount,
    CHOOSE(s.Priority, 'Low', 'Medium', 'High') AS PriorityText,
    CAST(s.Amount AS decimal(18, 2)) AS AmountDecimal,
    TRY_CAST(s.Score AS int) AS ScoreInt,
    CONVERT(varchar(10), s.CreatedAt, 126) AS CreatedAtText,
    TRY_CONVERT(datetime2, s.CreatedAtText, 126) AS ParsedCreatedAt,
    (s.CustomerName COLLATE Latin1_General_100_BIN2) AS CollatedCustomerName
FROM dbo.Source AS s;
