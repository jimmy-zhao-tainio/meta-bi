CREATE VIEW demo.SelectL03N01
AS
SELECT
    1 AS Value
FROM core.L02Node04 AS s0
CROSS JOIN hub.Layer01Stage AS s1
CROSS JOIN hub.Layer02Curated AS s2
CROSS JOIN hub.Layer02Published AS s3
CROSS JOIN stage.Seed08 AS s4