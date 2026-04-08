CREATE VIEW dbo.v_openquery AS
SELECT
    q.*
FROM OPENQUERY(RemoteServer, 'SELECT 1 AS Id') AS q;
