CREATE VIEW demo.Regional03Stage
AS
SELECT
    1 AS Value
FROM audit.Region02LoadLog AS s0
CROSS JOIN bridge.L05L06Stage AS s1
CROSS JOIN bridge.L07L08North AS s2
CROSS JOIN dw.L03Node01 AS s3
CROSS JOIN mart.L05Node04 AS s4
CROSS JOIN mart.L06Node01 AS s5
CROSS JOIN mart.Region02Published AS s6
CROSS JOIN wrk.Region01Curated AS s7