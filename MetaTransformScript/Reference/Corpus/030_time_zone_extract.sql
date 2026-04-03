CREATE VIEW dbo.v_time_zone_extract AS
SELECT
    s.Id,
    s.CreatedAt AT TIME ZONE 'UTC' AS CreatedAtUtc,
    EXTRACT(MONTH, s.CreatedAt) AS CreatedMonth
FROM dbo.Source AS s;
