CREATE VIEW demo.SelectL03N04
AS
SELECT
    1 AS Value
FROM bridge.L01L02North AS s0
CROSS JOIN hub.Layer01Published AS s1
CROSS JOIN hub.Layer01Stage AS s2
CROSS JOIN stage.L01Node05 AS s3