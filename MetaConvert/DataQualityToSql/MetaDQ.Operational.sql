/* MetaDataQuality operational store */
IF DB_ID(N'MetaDQ') IS NULL
BEGIN
    CREATE DATABASE [MetaDQ];
END
GO

EXEC [MetaDQ].sys.sp_executesql N'
IF OBJECT_ID(N''[dbo].[RunLog]'', N''U'') IS NULL
BEGIN
    CREATE TABLE [dbo].[RunLog]
    (
        [RunId] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_dbo_RunLog] PRIMARY KEY,
        [StartedAtUtc] datetime2(3) NOT NULL CONSTRAINT [DF_dbo_RunLog_StartedAtUtc] DEFAULT (SYSUTCDATETIME()),
        [CompletedAtUtc] datetime2(3) NULL,
        [SourceDatabaseName] sysname NOT NULL,
        [Status] nvarchar(32) NOT NULL,
        [ErrorMessage] nvarchar(4000) NULL
    );
END;

IF COL_LENGTH(N''dbo.RunLog'', N''SourceDatabaseName'') IS NULL
BEGIN
    ALTER TABLE [dbo].[RunLog] ADD [SourceDatabaseName] sysname NULL;
END;
IF COL_LENGTH(N''dbo.RunLog'', N''Status'') IS NULL
BEGIN
    ALTER TABLE [dbo].[RunLog] ADD [Status] nvarchar(32) NULL;
END;
IF COL_LENGTH(N''dbo.RunLog'', N''ErrorMessage'') IS NULL
BEGIN
    ALTER TABLE [dbo].[RunLog] ADD [ErrorMessage] nvarchar(4000) NULL;
END;

IF OBJECT_ID(N''[dbo].[FindingLog]'', N''U'') IS NULL
BEGIN
    CREATE TABLE [dbo].[FindingLog]
    (
        [FindingId] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_dbo_FindingLog] PRIMARY KEY,
        [RunId] bigint NOT NULL,
        [DQView] nvarchar(128) NOT NULL,
        [Issue] nvarchar(128) NOT NULL,
        [FindingTitle] nvarchar(128) NULL,
        [FindingCategory] nvarchar(128) NULL,
        [OutputMode] nvarchar(64) NULL,
        [CandidateId] nvarchar(256) NULL,
        [CandidateKind] nvarchar(128) NULL,
        [Relationship] nvarchar(512) NOT NULL,
        [RelationshipLabel] nvarchar(512) NULL,
        [ReferencingObject] nvarchar(512) NULL,
        [ReferencedObject] nvarchar(512) NULL,
        [CheckedObject] nvarchar(512) NULL,
        [SuspectSide] nvarchar(512) NULL,
        [SuspectObject] nvarchar(512) NULL,
        [LookupObject] nvarchar(512) NULL,
        [RelatedObject] nvarchar(512) NULL,
        [CorpusRelationship] nvarchar(512) NULL,
        [CorpusRelationshipPattern] nvarchar(max) NULL,
        [DominantPattern] nvarchar(max) NULL,
        [OutlierPattern] nvarchar(max) NULL,
        [TransformViews] nvarchar(max) NOT NULL,
        [GeneratedView] nvarchar(512) NOT NULL,
        [RowsReturned] bigint NULL,
        [ResultRowCount] bigint NULL,
        [FindingGroupCount] bigint NULL,
        [TotalSuspectCount] bigint NULL,
        [SuspectRowCount] bigint NULL,
        [Explanation] nvarchar(max) NULL,
        [FindingExplanation] nvarchar(max) NULL,
        [EvidenceSummary] nvarchar(max) NULL,
        [EvidenceOccurrenceCount] bigint NULL,
        [OutlierOccurrenceCount] bigint NULL,
        [EvidenceTransformCount] bigint NULL,
        [OutlierTransformCount] bigint NULL,
        [EvidenceConsensusRatio] decimal(18,6) NULL,
        [DominantConsensusRatio] decimal(18,6) NULL,
        [EvidenceOutlierRatio] decimal(18,6) NULL,
        [OutlierRatio] decimal(18,6) NULL,
        [EvidenceQuality] nvarchar(16) NULL,
        [ConfidenceBand] nvarchar(16) NULL,
        [ConfidenceReason] nvarchar(max) NULL,
        [EvidenceDiversitySummary] nvarchar(max) NULL,
        [ConfidenceSummary] nvarchar(max) NULL,
        [DistinctTransformCount] bigint NULL,
        [DistinctSourceTransformCount] bigint NULL,
        [DistinctSourceObjectCount] bigint NULL,
        [DistinctRelationshipPatternCount] bigint NULL,
        [EffectiveTransformCount] bigint NULL,
        [RecommendedAction] nvarchar(128) NOT NULL,
        [RuntimeCountStatus] nvarchar(64) NULL,
        [ReviewQuery] nvarchar(max) NULL,
        [DetailQuery] nvarchar(max) NULL,
        [TransformViewQuery] nvarchar(max) NULL,
        [SupportingTransformQuery] nvarchar(max) NULL,
        [CapturedAtUtc] datetime2(3) NOT NULL CONSTRAINT [DF_dbo_FindingLog_CapturedAtUtc] DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [FK_dbo_FindingLog_RunId] FOREIGN KEY ([RunId]) REFERENCES [dbo].[RunLog]([RunId])
    );
END;

IF COL_LENGTH(N''dbo.FindingLog'', N''Issue'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [Issue] nvarchar(128) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''FindingTitle'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [FindingTitle] nvarchar(128) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''FindingCategory'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [FindingCategory] nvarchar(128) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''ReviewQuery'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [ReviewQuery] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''DetailQuery'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [DetailQuery] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''TransformViewQuery'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [TransformViewQuery] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''SupportingTransformQuery'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [SupportingTransformQuery] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''OutputMode'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [OutputMode] nvarchar(64) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''CandidateId'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [CandidateId] nvarchar(256) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''CandidateKind'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [CandidateKind] nvarchar(128) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''RelationshipLabel'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [RelationshipLabel] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''ReferencingObject'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [ReferencingObject] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''ReferencedObject'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [ReferencedObject] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''CheckedObject'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [CheckedObject] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''SuspectSide'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [SuspectSide] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''SuspectObject'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [SuspectObject] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''LookupObject'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [LookupObject] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''RelatedObject'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [RelatedObject] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''CorpusRelationship'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [CorpusRelationship] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''CorpusRelationshipPattern'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [CorpusRelationshipPattern] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''DominantPattern'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [DominantPattern] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''OutlierPattern'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [OutlierPattern] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''ResultRowCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [ResultRowCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''FindingGroupCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [FindingGroupCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''SuspectRowCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [SuspectRowCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''Explanation'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [Explanation] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''FindingExplanation'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [FindingExplanation] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceSummary'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceSummary] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceOccurrenceCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceOccurrenceCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''OutlierOccurrenceCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [OutlierOccurrenceCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceTransformCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceTransformCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''OutlierTransformCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [OutlierTransformCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceConsensusRatio'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceConsensusRatio] decimal(18,6) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''DominantConsensusRatio'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [DominantConsensusRatio] decimal(18,6) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceOutlierRatio'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceOutlierRatio] decimal(18,6) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''OutlierRatio'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [OutlierRatio] decimal(18,6) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceQuality'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceQuality] nvarchar(16) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''ConfidenceBand'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [ConfidenceBand] nvarchar(16) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''ConfidenceReason'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [ConfidenceReason] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceDiversitySummary'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceDiversitySummary] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''ConfidenceSummary'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [ConfidenceSummary] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''DistinctTransformCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [DistinctTransformCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''DistinctSourceTransformCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [DistinctSourceTransformCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''DistinctSourceObjectCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [DistinctSourceObjectCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''DistinctRelationshipPatternCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [DistinctRelationshipPatternCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EffectiveTransformCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EffectiveTransformCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''RuntimeCountStatus'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [RuntimeCountStatus] nvarchar(64) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''RowsReturned'') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ALTER COLUMN [RowsReturned] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''TotalSuspectCount'') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ALTER COLUMN [TotalSuspectCount] bigint NULL;
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N''IX_dbo_FindingLog_RunId''
      AND [object_id] = OBJECT_ID(N''[dbo].[FindingLog]'')
)
BEGIN
    CREATE INDEX [IX_dbo_FindingLog_RunId] ON [dbo].[FindingLog]([RunId]);
END;
';
GO
EXEC [MetaDQ].sys.sp_executesql N'
CREATE OR ALTER PROCEDURE [dbo].[Run]
    @SourceDatabaseName sysname,
    @RunId bigint OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NULLIF(LTRIM(RTRIM(@SourceDatabaseName)), N'''') IS NULL
    BEGIN
        THROW 51000, N''@SourceDatabaseName is required.'', 1;
    END;

    IF DB_ID(@SourceDatabaseName) IS NULL
    BEGIN
        THROW 51000, N''Source database was not found.'', 1;
    END;

    DECLARE @runIdLocal bigint;
    INSERT INTO [dbo].[RunLog] ([SourceDatabaseName], [Status])
    VALUES (@SourceDatabaseName, N''Running'');
    SET @runIdLocal = SCOPE_IDENTITY();
    SET @RunId = @runIdLocal;

    BEGIN TRY
        DECLARE @sql nvarchar(max) =
            N''INSERT INTO [dbo].[FindingLog] ([RunId], [DQView], [Issue], [FindingTitle], [FindingCategory], [OutputMode], [CandidateId], [CandidateKind], [Relationship], [RelationshipLabel], [ReferencingObject], [ReferencedObject], [CheckedObject], [SuspectSide], [SuspectObject], [LookupObject], [RelatedObject], [CorpusRelationship], [CorpusRelationshipPattern], [DominantPattern], [OutlierPattern], [TransformViews], [GeneratedView], [RowsReturned], [ResultRowCount], [FindingGroupCount], [TotalSuspectCount], [SuspectRowCount], [Explanation], [FindingExplanation], [EvidenceSummary], [EvidenceOccurrenceCount], [OutlierOccurrenceCount], [EvidenceTransformCount], [OutlierTransformCount], [EvidenceConsensusRatio], [DominantConsensusRatio], [EvidenceOutlierRatio], [OutlierRatio], [EvidenceQuality], [ConfidenceBand], [ConfidenceReason], [EvidenceDiversitySummary], [ConfidenceSummary], [DistinctTransformCount], [DistinctSourceTransformCount], [DistinctSourceObjectCount], [DistinctRelationshipPatternCount], [EffectiveTransformCount], [RecommendedAction], [RuntimeCountStatus], [ReviewQuery], [DetailQuery], [TransformViewQuery], [SupportingTransformQuery]) '' +
            N''SELECT @RunId, [DQView], [Issue], [FindingTitle], [FindingCategory], [OutputMode], [CandidateId], [CandidateKind], [Relationship], [RelationshipLabel], [ReferencingObject], [ReferencedObject], [CheckedObject], [SuspectSide], [SuspectObject], [LookupObject], [RelatedObject], [CorpusRelationship], [CorpusRelationshipPattern], [DominantPattern], [OutlierPattern], [TransformViews], [GeneratedView], [RowsReturned], [ResultRowCount], [FindingGroupCount], [TotalSuspectCount], [SuspectRowCount], [Explanation], [FindingExplanation], [EvidenceSummary], [EvidenceOccurrenceCount], [OutlierOccurrenceCount], [EvidenceTransformCount], [OutlierTransformCount], [EvidenceConsensusRatio], [DominantConsensusRatio], [EvidenceOutlierRatio], [OutlierRatio], [EvidenceQuality], [ConfidenceBand], [ConfidenceReason], [EvidenceDiversitySummary], [ConfidenceSummary], [DistinctTransformCount], [DistinctSourceTransformCount], [DistinctSourceObjectCount], [DistinctRelationshipPatternCount], [EffectiveTransformCount], [RecommendedAction], [RuntimeCountStatus], [ReviewQuery], [DetailQuery], [TransformViewQuery], [SupportingTransformQuery] '' +
            N''FROM '' + QUOTENAME(@SourceDatabaseName) + N''.[dq].[v_DataQualityReview];'';

        EXEC sys.sp_executesql
            @sql,
            N''@RunId bigint'',
            @RunId = @runIdLocal;

        UPDATE [dbo].[RunLog]
        SET [CompletedAtUtc] = SYSUTCDATETIME(),
            [Status] = N''Completed''
        WHERE [RunId] = @runIdLocal;
    END TRY
    BEGIN CATCH
        UPDATE [dbo].[RunLog]
        SET [CompletedAtUtc] = SYSUTCDATETIME(),
            [Status] = N''Failed'',
            [ErrorMessage] = ERROR_MESSAGE()
        WHERE [RunId] = @runIdLocal;
        THROW;
    END CATCH;

    SELECT
        r.[RunId],
        r.[SourceDatabaseName],
        SUM(CASE WHEN f.[RowsReturned] IS NULL THEN 0 ELSE f.[RowsReturned] END) AS [RowsReturned],
        SUM(CASE WHEN f.[RowsReturned] IS NULL THEN 0 ELSE f.[RowsReturned] END) AS [ResultRowCount],
        SUM(CASE WHEN f.[RowsReturned] IS NULL THEN 0 ELSE f.[RowsReturned] END) AS [FindingGroupCount],
        SUM(CASE WHEN f.[TotalSuspectCount] IS NULL THEN 0 ELSE f.[TotalSuspectCount] END) AS [TotalSuspectCount],
        SUM(CASE WHEN f.[TotalSuspectCount] IS NULL THEN 0 ELSE f.[TotalSuspectCount] END) AS [SuspectRowCount],
        COUNT(f.[FindingId]) AS [ChecksExecuted],
        COUNT(f.[FindingId]) AS [FindingsExecuted],
        SUM(CASE WHEN f.[RowsReturned] IS NULL THEN 0 ELSE f.[RowsReturned] END) AS [RuntimeFindingGroupCount],
        SUM(CASE WHEN f.[TotalSuspectCount] IS NULL THEN 0 ELSE f.[TotalSuspectCount] END) AS [RuntimeSuspectRowCount]
    FROM [dbo].[RunLog] AS r
    LEFT JOIN [dbo].[FindingLog] AS f
      ON f.[RunId] = r.[RunId]
    WHERE r.[RunId] = @runIdLocal
    GROUP BY r.[RunId], r.[SourceDatabaseName];

    SELECT
        [DQView],
        [Issue],
        [FindingTitle],
        [FindingCategory],
        [OutputMode],
        [CandidateId],
        [CandidateKind],
        [Relationship],
        [RelationshipLabel],
        [ReferencingObject],
        [ReferencedObject],
        [CheckedObject],
        [SuspectSide],
        [SuspectObject],
        [LookupObject],
        [RelatedObject],
        [CorpusRelationship],
        [CorpusRelationshipPattern],
        [DominantPattern],
        [OutlierPattern],
        [RowsReturned],
        [ResultRowCount],
        [FindingGroupCount],
        [TotalSuspectCount],
        [SuspectRowCount],
        [Explanation],
        [FindingExplanation],
        [EvidenceSummary],
        [EvidenceOccurrenceCount],
        [OutlierOccurrenceCount],
        [EvidenceTransformCount],
        [OutlierTransformCount],
        [EvidenceConsensusRatio],
        [DominantConsensusRatio],
        [EvidenceOutlierRatio],
        [OutlierRatio],
        [EvidenceQuality],
        [ConfidenceBand],
        [ConfidenceReason],
        [EvidenceDiversitySummary],
        [ConfidenceSummary],
        [DistinctTransformCount],
        [DistinctSourceTransformCount],
        [DistinctSourceObjectCount],
        [DistinctRelationshipPatternCount],
        [EffectiveTransformCount],
        [RecommendedAction],
        [RuntimeCountStatus],
        [GeneratedView],
        [ReviewQuery],
        [DetailQuery],
        [TransformViewQuery],
        [SupportingTransformQuery]
    FROM [dbo].[FindingLog]
    WHERE [RunId] = @runIdLocal
      AND ([RowsReturned] > 0 OR [RowsReturned] IS NULL)
    ORDER BY CASE WHEN [RowsReturned] IS NULL THEN 1 ELSE 0 END, [RowsReturned] DESC, [DQView] ASC;
END;
';
GO

EXEC [MetaDQ].sys.sp_executesql N'
CREATE OR ALTER PROCEDURE [dbo].[Findings]
    @RunId bigint = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @RunId IS NULL
    BEGIN
        SELECT TOP (1) @RunId = [RunId]
        FROM [dbo].[RunLog]
        ORDER BY [RunId] DESC;
    END;

    IF @RunId IS NULL
    BEGIN
        SELECT
            CAST(NULL AS bigint) AS [RunId],
            CAST(NULL AS nvarchar(128)) AS [DQView],
            CAST(NULL AS nvarchar(128)) AS [Issue],
            CAST(NULL AS nvarchar(128)) AS [FindingTitle],
            CAST(NULL AS nvarchar(128)) AS [FindingCategory],
            CAST(NULL AS nvarchar(64)) AS [OutputMode],
            CAST(NULL AS nvarchar(256)) AS [CandidateId],
            CAST(NULL AS nvarchar(128)) AS [CandidateKind],
            CAST(NULL AS nvarchar(512)) AS [Relationship],
            CAST(NULL AS nvarchar(512)) AS [RelationshipLabel],
            CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
            CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
            CAST(NULL AS nvarchar(512)) AS [CheckedObject],
            CAST(NULL AS nvarchar(512)) AS [SuspectSide],
            CAST(NULL AS nvarchar(512)) AS [SuspectObject],
            CAST(NULL AS nvarchar(512)) AS [LookupObject],
            CAST(NULL AS nvarchar(512)) AS [RelatedObject],
            CAST(NULL AS nvarchar(512)) AS [CorpusRelationship],
            CAST(NULL AS nvarchar(max)) AS [CorpusRelationshipPattern],
            CAST(NULL AS nvarchar(max)) AS [DominantPattern],
            CAST(NULL AS nvarchar(max)) AS [OutlierPattern],
            CAST(NULL AS bigint) AS [RowsReturned],
            CAST(NULL AS bigint) AS [ResultRowCount],
            CAST(NULL AS bigint) AS [FindingGroupCount],
            CAST(NULL AS bigint) AS [TotalSuspectCount],
            CAST(NULL AS bigint) AS [SuspectRowCount],
            CAST(NULL AS nvarchar(max)) AS [Explanation],
            CAST(NULL AS nvarchar(max)) AS [FindingExplanation],
            CAST(NULL AS nvarchar(max)) AS [EvidenceSummary],
            CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
            CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
            CAST(NULL AS bigint) AS [EvidenceTransformCount],
            CAST(NULL AS bigint) AS [OutlierTransformCount],
            CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
            CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
            CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
            CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
            CAST(NULL AS nvarchar(16)) AS [EvidenceQuality],
            CAST(NULL AS nvarchar(16)) AS [ConfidenceBand],
            CAST(NULL AS nvarchar(max)) AS [ConfidenceReason],
            CAST(NULL AS nvarchar(max)) AS [EvidenceDiversitySummary],
            CAST(NULL AS nvarchar(max)) AS [ConfidenceSummary],
            CAST(NULL AS bigint) AS [DistinctTransformCount],
            CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
            CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
            CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
            CAST(NULL AS bigint) AS [EffectiveTransformCount],
            CAST(NULL AS nvarchar(128)) AS [RecommendedAction],
            CAST(NULL AS nvarchar(64)) AS [RuntimeCountStatus],
            CAST(NULL AS nvarchar(512)) AS [GeneratedView],
            CAST(NULL AS nvarchar(max)) AS [ReviewQuery],
            CAST(NULL AS nvarchar(max)) AS [DetailQuery],
            CAST(NULL AS nvarchar(max)) AS [TransformViewQuery],
            CAST(NULL AS nvarchar(max)) AS [SupportingTransformQuery]
        WHERE 1 = 0;
        RETURN;
    END;

    SELECT
        [RunId],
        [DQView],
        [Issue],
        [FindingTitle],
        [FindingCategory],
        [OutputMode],
        [CandidateId],
        [CandidateKind],
        [Relationship],
        [RelationshipLabel],
        [ReferencingObject],
        [ReferencedObject],
        [CheckedObject],
        [SuspectSide],
        [SuspectObject],
        [LookupObject],
        [RelatedObject],
        [CorpusRelationship],
        [CorpusRelationshipPattern],
        [DominantPattern],
        [OutlierPattern],
        [RowsReturned],
        [ResultRowCount],
        [FindingGroupCount],
        [TotalSuspectCount],
        [SuspectRowCount],
        [Explanation],
        [FindingExplanation],
        [EvidenceSummary],
        [EvidenceOccurrenceCount],
        [OutlierOccurrenceCount],
        [EvidenceTransformCount],
        [OutlierTransformCount],
        [EvidenceConsensusRatio],
        [DominantConsensusRatio],
        [EvidenceOutlierRatio],
        [OutlierRatio],
        [EvidenceQuality],
        [ConfidenceBand],
        [ConfidenceReason],
        [EvidenceDiversitySummary],
        [ConfidenceSummary],
        [DistinctTransformCount],
        [DistinctSourceTransformCount],
        [DistinctSourceObjectCount],
        [DistinctRelationshipPatternCount],
        [EffectiveTransformCount],
        [RecommendedAction],
        [RuntimeCountStatus],
        [GeneratedView],
        [ReviewQuery],
        [DetailQuery],
        [TransformViewQuery],
        [SupportingTransformQuery]
    FROM [dbo].[FindingLog]
    WHERE [RunId] = @RunId
      AND ([RowsReturned] > 0 OR [RowsReturned] IS NULL)
    ORDER BY CASE WHEN [RowsReturned] IS NULL THEN 1 ELSE 0 END, [RowsReturned] DESC, [DQView] ASC;
END;
';
GO
