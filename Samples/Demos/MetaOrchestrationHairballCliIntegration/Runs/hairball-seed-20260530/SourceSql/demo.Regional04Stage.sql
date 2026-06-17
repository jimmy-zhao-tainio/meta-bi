CREATE VIEW demo.Regional04Stage
AS
SELECT
    1 AS Value
FROM audit.Region02LoadLog AS s0
CROSS JOIN bridge.L05L06South AS s1
CROSS JOIN dw.L03Node02 AS s2
CROSS JOIN hub.Layer04Published AS s3
CROSS JOIN hub.Layer05Published AS s4
CROSS JOIN mart.L06Node01 AS s5
CROSS JOIN mart.L07Node01 AS s6
CROSS JOIN mart.Region03Published AS s7
CROSS JOIN stage.L01Node04 AS s8