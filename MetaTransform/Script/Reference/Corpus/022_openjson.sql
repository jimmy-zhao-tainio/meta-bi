CREATE VIEW dbo.v_openjson AS
SELECT
    s.Id,
    j.[key],
    j.value,
    j.type
FROM dbo.JsonSource AS s
CROSS APPLY OPENJSON(s.JsonPayload) AS j;
