SET NOCOUNT ON;

DECLARE @RunId bigint;
EXEC dq.Run @All = 1, @RunId = @RunId OUTPUT;

IF NOT EXISTS (SELECT 1 FROM MetaDQ.dbo.RunLog WHERE RunId = @RunId AND Status = N'Completed')
    THROW 51000, N'The demo review did not complete every check.', 1;
