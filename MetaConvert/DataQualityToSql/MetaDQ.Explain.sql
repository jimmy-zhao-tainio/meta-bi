CREATE OR ALTER PROCEDURE [dq].[Explain]
    @CheckNo bigint,
    @RunId bigint = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SourceDatabase sysname = DB_NAME();
    IF @RunId IS NULL
        SELECT TOP (1) @RunId = [RunId]
        FROM [MetaDQ].[dbo].[RunLog]
        WHERE [SourceDatabaseName] = @SourceDatabase
        ORDER BY [RunId] DESC;

    DECLARE @Kind nvarchar(128), @Mode nvarchar(64), @Outcome nvarchar(64),
            @Title nvarchar(256), @Relationship nvarchar(512), @Transforms nvarchar(max),
            @DetailView sysname, @Error nvarchar(max), @Probe nvarchar(max), @EvidenceQuery nvarchar(max),
            @CandidateId nvarchar(256), @Groups bigint, @Rows bigint;

    SELECT @Kind = r.[CandidateKind], @Mode = r.[OutputMode], @Outcome = r.[Outcome],
           @Title = COALESCE(r.[FindingTitle], c.[CheckName], r.[CandidateId]),
           @Relationship = r.[CheckedRelationship], @Transforms = r.[Transforms],
           @DetailView = c.[DetailView], @Probe = c.[MatchProbe], @EvidenceQuery = c.[EvidenceQuery], @CandidateId = r.[CandidateId],
           @Groups = r.[FindingGroups], @Rows = r.[SuspectRows],
           @Error = CASE WHEN r.[Outcome] = N'Check failed' THEN r.[Explanation] END
    FROM [MetaDQ].[dbo].[CheckReview] AS r
    LEFT JOIN [dq].[v_ReviewChecks] AS c ON c.[CandidateId] = r.[CandidateId] COLLATE DATABASE_DEFAULT
    WHERE r.[RunId] = @RunId AND r.[SourceDatabaseName] = @SourceDatabase AND r.[CheckNo] = @CheckNo;

    IF @Outcome IS NULL
        THROW 51000, N'Check not found for this run and database. Copy the Investigate command from dq.Run or dq.v_DataQualityReview.', 1;

    DECLARE @Observed nvarchar(max), @Decision nvarchar(max), @Current nvarchar(max), @EvidenceError nvarchar(max);
    DECLARE @Matched bigint, @Repeated bigint, @Maximum bigint;
    DECLARE @Parameters nvarchar(256) = N'@Matched bigint OUTPUT, @Repeated bigint OUTPUT, @Maximum bigint OUTPUT';

    IF @Probe IS NOT NULL AND @Outcome IN (N'Review affected rows', N'No rows found')
    BEGIN
        BEGIN TRY
            EXEC sys.sp_executesql @Probe, @Parameters, @Matched OUTPUT, @Repeated OUTPUT, @Maximum OUTPUT;
        END TRY
        BEGIN CATCH
            SET @EvidenceError = ERROR_MESSAGE();
        END CATCH;
    END;

    IF @Outcome = N'Review affected rows'
    BEGIN
        IF @Kind IN (N'JoinMultiplicityExplosion', N'OutputDuplicateRisk', N'ImpliedJoinFanoutRisk', N'ImpliedOutputDuplicateRisk')
        BEGIN
            SET @Observed = CONCAT(@Groups, N' join-key groups returned by the saved check (', @Rows, N' joined rows in those groups).');
            SET @Decision = N'Joined rows sharing a key can come from separate input rows. The saved check leaves the effect on individual rows and the output key unresolved.';
        END
        ELSE IF @Kind IN (N'OuterJoinNullExpansion', N'LeftJoinAgainstUsuallyMandatoryRelationship')
        BEGIN
            SET @Observed = CONCAT(@Rows, N' preserved rows had no match under the checked relationship.');
            SET @Decision = N'Each returned row has no match on the opposite side. If the relationship is required, check the missing lookup row or the join key. If the relationship is optional, keep the preserved row.';
        END
        ELSE IF @Kind IN (N'JoinOrphan', N'ImpliedForeignKeyMissingReference', N'InnerJoinAgainstUsuallyOptionalRelationship')
        BEGIN
            SET @Observed = CONCAT(@Rows, N' rows on the checked side had no match across the relationship.');
            SET @Decision = CASE WHEN @Kind = N'JoinOrphan'
                THEN N'Each returned row has no match on the opposite side. If the row holds a required reference, reconcile its key with the other table. If it is a lookup row, it had no references on the other side.'
                WHEN @Kind = N'ImpliedForeignKeyMissingReference'
                THEN N'Use the keys below to reconcile the checked references with the lookup. Check whether the missing key should be loaded or whether the reference should change.'
                ELSE N'These checked keys have no lookup match. For rows the transform must keep, review the inner join or load the missing lookup before running it.'
                END;
        END
        ELSE IF @Kind = N'ImpliedUniqueKeyViolation'
        BEGIN
            SET @Observed = CONCAT(@Groups, N' lookup key groups repeated in the checked table.');
            SET @Decision = N'If this combination of key columns must be unique, resolve the repeated records for the displayed keys. If the records represent distinct business entities, include the distinguishing column in the join.';
        END
        ELSE
            SET @Observed = CONCAT(@Groups, N' results were saved by this check.');
    END
    ELSE IF @Outcome = N'Review transform pattern'
    BEGIN
        SET @Observed = N'The saved comparison found a relationship or filter written differently in these transforms.';
        SET @Decision = CASE @Kind
            WHEN N'MissingCommonFilter' THEN N'Compare the displayed filter conditions. Decide whether this transform should include the rows that the common filter excludes.'
            WHEN N'IncompleteCompositeJoin' THEN N'Compare the displayed join conditions. Check whether the key column missing from the different condition is required to identify one match.'
            WHEN N'SuspiciousExtraJoinPredicate' THEN N'Compare the displayed conditions. Check whether the additional condition should exclude rows from this transform.'
            WHEN N'MinorityColumnEquivalence' THEN N'Compare the columns in the displayed conditions. Check which of those keys represents the relationship this transform needs.'
            ELSE N'Compare the common and different conditions below. Confirm which one represents the relationship for the listed transform before changing it.'
            END;
    END
    ELSE IF @Outcome = N'No rows found'
        SET @Observed = N'This check returned zero rows in the saved run.';
    ELSE IF @Outcome = N'Check failed'
        SET @Decision = N'SQL Server stopped this check. Resolve the error below, then rerun the checks to obtain a result.';

    IF @Repeated IS NOT NULL
    BEGIN
        SET @Current = CASE WHEN @Matched = 0
            THEN N'No input row matches on the modeled base-table keys now. This data cannot demonstrate how many matches a populated key would get.'
            WHEN @Repeated = 0
            THEN CONCAT(@Matched, N' input rows matched; each had exactly one match on these base-table keys. The join-key groups can be explained by reuse of the same lookup key across input rows.')
            ELSE CONCAT(@Repeated, N' input rows matched more than one row on these base-table keys. The most matches for one input row was ', @Maximum, N'.')
            END;
        SET @Decision = CASE WHEN @Matched = 0
            THEN N'If inputs should match in this relationship, check the join keys or the load of the matching table.'
            WHEN @Repeated = 0
            THEN N'No correction is indicated for reuse of these keys. This measurement covers the named base-table join; output grain is a separate check.'
            ELSE N'For each key below, MatchesPerInputRow is the number of matches per input row. If each input should match once, correct the repeated lookup keys or qualify the join with the key column that distinguishes those rows.'
            END;
        IF @Repeated = 0
            SET @EvidenceQuery = NULL;
    END
    ELSE IF @Outcome = N'Review affected rows' AND @Mode = N'RuntimeCheck' AND @DetailView IS NOT NULL
    BEGIN
        IF @EvidenceQuery IS NULL OR @Probe IS NOT NULL
        BEGIN
            SET @EvidenceQuery = N'SELECT TOP (20) [Relationship], [SuspectSide] AS [Checked table], [SuspectCount] AS [Rows counted]
FROM [dq].' + QUOTENAME(@DetailView) + N'
ORDER BY [SuspectCount] DESC, [Relationship], [KeyValues];';
            SET @Current = N'The sample uses current data. This check has no resolved typed key query; the sample identifies the relationship and checked side.';
        END
        ELSE
            SET @Current = N'The sample uses current data. An empty grid means the evidence query found no matching issues now.';
    END
    ELSE IF @Outcome = N'Review affected rows' AND @DetailView IS NULL
        SET @Current = N'The detail view for this saved check is no longer deployed.';
    ELSE IF @Outcome = N'Review transform pattern'
        SET @EvidenceQuery = N'SELECT TOP (20) [RelationshipLabel] AS [Relationship], [TransformViews] AS [Transforms],
       [DominantPattern] AS [Common condition], [OutlierPattern] AS [Different condition]
FROM [MetaDQ].[dbo].[FindingLog]
WHERE [RunId] = ' + CONVERT(nvarchar(20), @RunId) + N' AND [CandidateId] = N'''
       + REPLACE(@CandidateId, N'''', N'''''') + N'''
ORDER BY [RelationshipLabel], [OutlierPattern], [FindingId];';
    ELSE
        SET @EvidenceQuery = NULL;

    IF @EvidenceQuery IS NOT NULL
        SET @EvidenceQuery = N'USE ' + QUOTENAME(@SourceDatabase) + N';' + NCHAR(10) + @EvidenceQuery;

    SELECT [Topic], [WhatToKnow]
    FROM (VALUES
        (1, N'Check', CONVERT(nvarchar(max), @Title)),
        (2, N'Saved result', COALESCE(@Observed, N'-')),
        (3, N'Transform', @Transforms),
        (4, N'Relationship', @Relationship),
        (5, N'What the data shows now', @Current),
        (6, N'What to do', @Decision),
        (7, N'SQL error', @Error),
        (8, N'Match check error', @EvidenceError),
        (9, N'Evidence query', @EvidenceQuery)
    ) AS notes([Position], [Topic], [WhatToKnow])
    WHERE NULLIF([WhatToKnow], N'') IS NOT NULL
    ORDER BY [Position];

    IF @EvidenceQuery IS NOT NULL
    BEGIN TRY
        EXEC sys.sp_executesql @EvidenceQuery;
    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE() AS [Evidence error];
    END CATCH;
END;
GO
