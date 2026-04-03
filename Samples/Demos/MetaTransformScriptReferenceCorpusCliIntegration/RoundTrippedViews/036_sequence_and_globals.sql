CREATE VIEW dbo.v_sequence_and_globals
AS
SELECT
    NEXT VALUE FOR dbo.Seq AS SeqValue,
    @@SPID AS SessionId,
    CAST('abc' AS varchar(max)) AS MaxText
FROM dbo.Source AS s
GO
