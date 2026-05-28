CREATE VIEW dbo.v_changetable AS
SELECT
    ct.SYS_CHANGE_VERSION,
    ct.Id
FROM CHANGETABLE(CHANGES dbo.Source, 0) AS ct;
