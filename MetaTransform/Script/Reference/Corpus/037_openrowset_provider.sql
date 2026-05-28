CREATE VIEW dbo.v_openrowset_provider AS
SELECT
    r.*
FROM OPENROWSET('MSOLEDBSQL', 'Server=.;Trusted_Connection=yes;', 'SELECT 1 AS Id') AS r;
