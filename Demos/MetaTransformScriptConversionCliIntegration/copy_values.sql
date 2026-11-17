CREATE VIEW dbo.CopyValues AS
SELECT s.SequenceNo, s.NumericValue, s.TextValue
FROM dbo.SourceRows AS s
WHERE s.SequenceNo >= 1;
