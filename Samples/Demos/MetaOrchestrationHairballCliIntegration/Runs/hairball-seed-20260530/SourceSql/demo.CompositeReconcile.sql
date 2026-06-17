CREATE VIEW demo.CompositeReconcile
AS
SELECT
    1 AS Value
FROM wrk.CompositeCurated AS s0
CROSS JOIN dw.L03Node03 AS s1
CROSS JOIN dw.L04Node02 AS s2
CROSS JOIN dw.L04Node05 AS s3
CROSS JOIN hub.Layer04Published AS s4
CROSS JOIN mart.L06Node10 AS s5