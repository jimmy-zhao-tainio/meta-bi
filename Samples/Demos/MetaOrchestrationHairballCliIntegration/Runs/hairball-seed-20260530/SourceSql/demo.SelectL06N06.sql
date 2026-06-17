CREATE VIEW demo.SelectL06N06
AS
SELECT
    1 AS Value
FROM core.L02Node06 AS s0
CROSS JOIN hub.Layer01Curated AS s1
CROSS JOIN hub.Layer01Published AS s2
CROSS JOIN stage.L01Node01 AS s3