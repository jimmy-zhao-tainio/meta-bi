CREATE VIEW dbo.v_changetable_version AS
SELECT
    ct.SYS_CHANGE_VERSION
FROM CHANGETABLE(VERSION dbo.Source, (Id), (1)) AS ct;
