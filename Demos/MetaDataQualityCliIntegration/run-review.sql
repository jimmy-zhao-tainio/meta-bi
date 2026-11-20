SET NOCOUNT ON;

DECLARE @SourceDatabase sysname = DB_NAME();
DECLARE @RunId bigint;
EXEC MetaDQ.dbo.Run @SourceDatabaseName = @SourceDatabase, @RunId = @RunId OUTPUT;

SELECT [Check], Outcome, Relationship, Transforms, FindingGroups, SuspectRows,
       Explanation, CheckedAtUtc, InspectCurrentRows
FROM dq.v_DataQualityReview
ORDER BY SuspectRows DESC, CheckId;

IF NOT EXISTS (SELECT 1 FROM MetaDQ.dbo.RunLog WHERE RunId = @RunId AND Status = N'Completed')
    THROW 51000, N'The demo review did not complete every check.', 1;
