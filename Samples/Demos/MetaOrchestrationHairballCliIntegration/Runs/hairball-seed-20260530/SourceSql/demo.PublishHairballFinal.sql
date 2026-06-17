CREATE VIEW demo.PublishHairballFinal
AS
SELECT
    1 AS Value
FROM dw.L03Node01 AS s0
CROSS JOIN dw.L04Node02 AS s1
CROSS JOIN hub.Layer05Curated AS s2
CROSS JOIN hub.Layer07Published AS s3
CROSS JOIN mart.CompositeFinal AS s4
CROSS JOIN mart.L06Node02 AS s5
CROSS JOIN mart.L06Node03 AS s6
CROSS JOIN mart.L08Node03 AS s7
CROSS JOIN mart.Region02Published AS s8
CROSS JOIN stage.Seed07 AS s9