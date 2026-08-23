CREATE VIEW dbo.v_openrowset AS
SELECT
    r.*
FROM OPENROWSET(BULK 'C:\data\sample.csv', FORMAT = 'CSV') AS r;
