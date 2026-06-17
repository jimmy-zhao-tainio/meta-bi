CREATE VIEW demo.Regional02Stage
AS
SELECT
    1 AS Value
FROM core.L02Node08 AS s0
CROSS JOIN dw.L04Node01 AS s1
CROSS JOIN dw.L04Node09 AS s2
CROSS JOIN hub.Layer03Curated AS s3
CROSS JOIN hub.Layer03Stage AS s4
CROSS JOIN hub.Layer06Published AS s5
CROSS JOIN stage.L01Node07 AS s6