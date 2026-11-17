SET NOCOUNT ON;
IF (SELECT COUNT_BIG(*) FROM dbo.TargetRows) <> 4
    THROW 51000, N'Expected four converted target rows.', 1;
IF EXISTS
(
    SELECT CONVERT(bigint,SequenceNo), CONVERT(bigint,NumericValue), CONVERT(varbinary(max),TextValue)
    FROM dbo.SourceRows
    EXCEPT
    SELECT SequenceNo, NumericValue, CONVERT(varbinary(max),TextValue) FROM dbo.TargetRows
)
OR EXISTS
(
    SELECT SequenceNo, NumericValue, CONVERT(varbinary(max),TextValue) FROM dbo.TargetRows
    EXCEPT
    SELECT CONVERT(bigint,SequenceNo), CONVERT(bigint,NumericValue), CONVERT(varbinary(max),TextValue)
    FROM dbo.SourceRows
)
    THROW 51000, N'Converted target values differ from source values, including Unicode, NULL or empty text.', 1;
IF (SELECT COUNT(*) FROM sys.columns c JOIN sys.types t ON t.user_type_id=c.user_type_id
    WHERE c.object_id=OBJECT_ID(N'dbo.TargetRows') AND c.name IN (N'SequenceNo',N'NumericValue') AND t.name=N'bigint') <> 2
    THROW 51000, N'Target numeric fields must use bigint.', 1;
SELECT SequenceNo, NumericValue, TextValue,
    CASE WHEN TextValue IS NULL THEN N'NULL' WHEN DATALENGTH(TextValue)=0 THEN N'Empty' ELSE N'Text' END AS TextState
FROM dbo.TargetRows ORDER BY SequenceNo;
