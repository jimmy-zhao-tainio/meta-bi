CREATE VIEW dbo.v_adhoc_datasource AS
SELECT
    d.*
FROM OPENDATASOURCE('MSOLEDBSQL', 'Data Source=.;Integrated Security=SSPI').master.sys.databases AS d;
