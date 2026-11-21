using System.Globalization;
using System.Text;
using Meta.Integration;
using Meta.Operations.Domain;
using MetaDataQuality;
using MetaDataQuality.Core;
using MetaSql;
using MetaWeave.Core;
using MetaWeaveScript.Execution;

namespace MetaConvert.DataQualityToSql;

public sealed class DataQualityToSqlConverter
{
    private const string MetaSqlDatabaseName = "DataQuality";
    private const string OperationalSqlFileName = "MetaDQ.Operational.sql";

    private static readonly Lazy<MetaWeaveScriptDirection> ForwardDirection = new(
        static () => new MetaWeaveScriptDirectionLoader().Load(
            ResolveWeaveWorkspacePath(),
            "forward"));

    private static readonly Lazy<string> OperationalSql = new(LoadOperationalSql);
    private static readonly Lazy<string> ExplainSql = new(() =>
        File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "MetaDQ.Explain.sql"), Encoding.UTF8));

    public DataQualityToSqlResult Convert(string workspacePath, string outputPath)
        => Convert(workspacePath, outputPath, progress: null);

    public DataQualityToSqlResult Convert(
        string workspacePath,
        string outputPath,
        Action<MetaWeaveScriptExecutionProgress>? progress)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        var sourceState = TypedWorkspaceModelMapper.LoadStateAsync(workspacePath).GetAwaiter().GetResult();
        var source = new MetaDataQualityWorkspaceBinding().Read(sourceState);
        var metaSql = Materialize(sourceState, progress);
        var candidateViews = metaSql.ViewList
            .Where(view => view.Name.StartsWith("v_", StringComparison.Ordinal))
            .OrderBy(static view => ParseDeployOrdinal(view.DeployOrdinal))
            .ThenBy(static view => view.Name, StringComparer.Ordinal)
            .ToArray();
        var summaryViews = metaSql.ViewList
            .Where(view => view.Name.StartsWith("r_", StringComparison.Ordinal))
            .ToDictionary(view => view.Name[2..], StringComparer.Ordinal);
        if (candidateViews.Any(view => view.Name.Equals("v_DataQualityReview", StringComparison.OrdinalIgnoreCase) ||
                                       view.Name.Equals("v_ReviewChecks", StringComparison.OrdinalIgnoreCase) ||
                                       view.Name.Equals("v_ReviewCheckTransforms", StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("A generated check collides with a reserved Data Quality review view name.");
        }
        if (candidateViews.Length + summaryViews.Count != metaSql.ViewList.Count ||
            candidateViews.Length != summaryViews.Count ||
            candidateViews.Any(view => !summaryViews.ContainsKey(ResolveCandidateId(view))))
        {
            throw new InvalidOperationException("Each generated Data Quality check must have exactly one review summary.");
        }

        var candidateById = source.DataQualityCandidateList.ToDictionary(candidate => candidate.Id, StringComparer.Ordinal);
        var reviewSql = RenderReviewSql(candidateViews, candidateById, summaryViews, BuildJoinReviewQueries(source),
            BuildTransformScope(source, candidateViews.Select(ResolveCandidateId).ToHashSet(StringComparer.Ordinal)));

        if (IsSqlFilePath(outputPath))
        {
            var fullPath = Path.GetFullPath(outputPath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            File.WriteAllText(
                fullPath,
                RenderCombinedSql(candidateViews, summaryViews, reviewSql),
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            return CreateResult(fullPath, candidateViews.Length);
        }

        var outputDirectory = Path.GetFullPath(outputPath);
        Directory.CreateDirectory(outputDirectory);
        foreach (var view in candidateViews)
        {
            var candidateId = ResolveCandidateId(view);
            var candidate = candidateById[candidateId];
            var filePath = Path.Combine(
                outputDirectory,
                $"{SanitizeFileName(candidate.Name)}.sql");
            File.WriteAllText(
                filePath,
                RenderCheckSql(view, summaryViews[candidateId]),
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        }

        File.WriteAllText(
            Path.Combine(outputDirectory, "v_DataQualityReview.sql"),
            reviewSql,
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        File.WriteAllText(
            Path.Combine(outputDirectory, "MetaDQ.Operational.sql"),
            OperationalSql.Value,
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        return CreateResult(outputDirectory, candidateViews.Length);
    }

    private static MetaSqlModel Materialize(
        InMemoryWorkspace source,
        Action<MetaWeaveScriptExecutionProgress>? progress)
    {
        var result = new MetaWeaveScriptExecutionService().ExecuteDirection(
            ForwardDirection.Value,
            new Dictionary<string, InMemoryWorkspace>(StringComparer.OrdinalIgnoreCase)
            {
                ["quality"] = source,
            },
            new InMemoryWorkspace(
                TypedWorkspaceModelMapper.LoadStateAsync(Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaSql")).GetAwaiter().GetResult().Model,
                new GenericInstance { ModelName = "MetaSql" }),
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["databaseName"] = MetaSqlDatabaseName,
            },
            progress);

        if (!result.IsSuccess)
        {
            throw new InvalidOperationException(
                "The sanctioned Data-Quality-to-SQL weave rejected the source workspace:" +
                Environment.NewLine +
                string.Join(
                    Environment.NewLine,
                    result.Issues.Select(static issue =>
                        $"{issue.Code}: {issue.Message}")));
        }

        return new MetaSqlWorkspaceBinding().Read(result.OutputWorkspace!);
    }

    private static string RenderReviewSql(
        IReadOnlyList<View> candidateViews,
        IReadOnlyDictionary<string, DataQualityCandidate> candidates,
        IReadOnlyDictionary<string, View> summaries,
        IReadOnlyDictionary<string, JoinReviewQueries> joinReviewQueries,
        IReadOnlyList<CheckTransform> transforms)
    {
        var builder = new StringBuilder();
        AppendSchemaGuard(builder);
        builder.AppendLine("CREATE OR ALTER VIEW [dq].[v_ReviewChecks] AS");
        if (candidateViews.Count == 0)
        {
            builder.AppendLine("SELECT CAST(NULL AS nvarchar(256)) AS CandidateId, CAST(NULL AS sysname) AS SummaryView,");
            builder.AppendLine("       CAST(NULL AS sysname) AS DetailView, CAST(NULL AS nvarchar(256)) AS CheckName,");
            builder.AppendLine("       CAST(NULL AS nvarchar(max)) AS MatchProbe, CAST(NULL AS nvarchar(max)) AS EvidenceQuery,");
            builder.AppendLine("       CAST(NULL AS nvarchar(max)) AS CheckHint WHERE 1 = 0;");
        }
        else
        {
            builder.AppendLine("SELECT CAST(v.CandidateId AS nvarchar(256)) AS CandidateId,");
            builder.AppendLine("       CAST(v.SummaryView AS sysname) AS SummaryView, CAST(v.DetailView AS sysname) AS DetailView,");
            builder.AppendLine("       CAST(v.CheckName AS nvarchar(256)) AS CheckName, CAST(v.MatchProbe AS nvarchar(max)) AS MatchProbe,");
            builder.AppendLine("       CAST(v.EvidenceQuery AS nvarchar(max)) AS EvidenceQuery, CAST(v.CheckHint AS nvarchar(max)) AS CheckHint");
            builder.AppendLine("FROM (VALUES");
            for (var index = 0; index < candidateViews.Count; index++)
            {
                var view = candidateViews[index];
                var id = ResolveCandidateId(view);
                if (!candidates.TryGetValue(id, out var candidate))
                    throw new InvalidOperationException($"The Data Quality weave produced a view for unknown candidate '{id}'.");
                joinReviewQueries.TryGetValue(id, out var queries);
                builder.Append($"    ({SqlString(id)}, {SqlString(summaries[id].Name)}, {SqlString(view.Name)}, {SqlString(candidate.Name)}, {SqlStringOrNull(queries?.MatchProbe)}, {SqlStringOrNull(queries?.EvidenceQuery)}, {SqlStringOrNull(queries?.CheckHint ?? candidate.Rationale)})");
                builder.AppendLine(index + 1 == candidateViews.Count ? "" : ",");
            }
            builder.AppendLine(") v(CandidateId, SummaryView, DetailView, CheckName, MatchProbe, EvidenceQuery, CheckHint);");
        }

        builder.AppendLine("GO");
        AppendTransformScope(builder, transforms);
        const string latestReviewScope = """
            WITH LatestFullRun AS
            (
                SELECT MAX(RunId) AS RunId
                FROM [MetaDQ].[dbo].[RunLog]
                WHERE SourceDatabaseName = DB_NAME() AND TransformScriptId IS NULL
                  AND Status IN (N'Completed', N'CompletedWithErrors')
            ), LatestTransformRun AS
            (
                SELECT TransformScriptId COLLATE Latin1_General_100_BIN2 AS TransformScriptId, MAX(RunId) AS RunId
                FROM [MetaDQ].[dbo].[RunLog]
                WHERE SourceDatabaseName = DB_NAME() AND TransformScriptId IS NOT NULL
                  AND Status IN (N'Completed', N'CompletedWithErrors')
                GROUP BY TransformScriptId COLLATE Latin1_General_100_BIN2
            ), ReviewScope AS
            (
                SELECT c.CandidateId, c.CheckName, c.CheckHint, s.Transform, s.TransformScriptId,
                       CASE WHEN f.RunId IS NULL OR t.RunId > f.RunId THEN t.RunId ELSE f.RunId END AS RunId
                FROM [dq].[v_ReviewChecks] AS c
                LEFT JOIN [dq].[v_ReviewCheckTransforms] AS s ON s.CandidateId = c.CandidateId
                LEFT JOIN LatestTransformRun AS t ON t.TransformScriptId = s.TransformScriptId
                CROSS JOIN LatestFullRun AS f
            )
            """;
        builder.AppendLine($"""
            CREATE OR ALTER VIEW [dq].[v_DataQualityReview] AS
            {latestReviewScope}
            {RenderReviewRows("ReviewScope AS c", "c.RunId", "c.Transform")};
            GO

            CREATE OR ALTER PROCEDURE [dq].[Run]
                @IncludeClean bit = 0,
                @RunId bigint = NULL OUTPUT,
                @Transform nvarchar(512) = NULL,
                @All bit = 0,
                @Help bit = 0
            AS
            BEGIN
                SET NOCOUNT ON;
                SET @RunId = NULL;

                IF @Help = 1
                BEGIN
                    DECLARE @RunCommand nvarchar(300) = N'EXEC ' + QUOTENAME(DB_NAME()) + N'.dq.Run';
                    DECLARE @ExampleTransform nvarchar(512), @ScopedCommand nvarchar(max);
                    SELECT TOP (1) @ExampleTransform = TransformScriptId
                    FROM [dq].[v_ReviewCheckTransforms]
                    ORDER BY [Transform], TransformScriptId;

                    IF @ExampleTransform IS NOT NULL
                        SET @ScopedCommand = @RunCommand + N' @Transform = N''' + REPLACE(@ExampleTransform, N'''', N'''''') + N'''';

                    SELECT [SQL], [What it does]
                    FROM (VALUES
                        (1, @RunCommand + N';', N'List the checks and their last saved results.'),
                        (2, @ScopedCommand + N';', N'Run the checks for one transform. Pass its unique name or script ID to @Transform.'),
                        (3, @ScopedCommand + N', @IncludeClean = 1;', N'Run a transform and include checks that found no rows.'),
                        (4, @RunCommand + N' @All = 1;', N'Run every deployed check. Use for an intentional full run.'),
                        (5, NULL, N'Use Next step on a saved result to open its evidence. Use Mark no issue there to record a review decision.')
                    ) AS help([Position], [SQL], [What it does])
                    WHERE [SQL] IS NOT NULL OR [Position] = 5
                    ORDER BY [Position];
                    RETURN;
                END;

                IF @Transform IS NULL AND ISNULL(@All, 0) = 0
                BEGIN
                    SELECT N'Copy the SQL from Next step. To run one transform by name, pass its name to @Transform.' AS [How to use],
                           CONCAT(N'EXEC ', QUOTENAME(DB_NAME()), N'.dq.Run @Help = 1;') AS [Help];

                    {latestReviewScope}
                    {RenderReviewRows("ReviewScope AS c", "c.RunId", "c.Transform", showHints: true, scriptId: "c.TransformScriptId")}
                    ORDER BY [Transform], [Check], c.CandidateId, c.TransformScriptId;
                    RETURN;
                END;

                IF @Transform IS NOT NULL AND @All = 1
                    THROW 51000, N'Choose a transform or request all checks.', 1;

                DECLARE @TransformScriptId nvarchar(512), @Matches int;
                IF @Transform IS NOT NULL
                BEGIN
                    SELECT TOP (1) @TransformScriptId = TransformScriptId
                    FROM [dq].[v_ReviewCheckTransforms]
                    WHERE TransformScriptId = @Transform COLLATE Latin1_General_100_BIN2;

                    IF @TransformScriptId IS NULL
                    BEGIN
                        SELECT @TransformScriptId = MIN(TransformScriptId), @Matches = COUNT(DISTINCT TransformScriptId)
                        FROM [dq].[v_ReviewCheckTransforms]
                        WHERE [Transform] = @Transform COLLATE Latin1_General_100_BIN2;
                        IF @Matches <> 1
                            THROW 51000, N'This transform name is unknown or ambiguous. Use a unique name or the transform script ID.', 1;
                    END;
                END;

                DECLARE @SourceDatabaseName sysname = DB_NAME();
                EXEC [MetaDQ].[dbo].[Run]
                    @SourceDatabaseName = @SourceDatabaseName,
                    @RunId = @RunId OUTPUT,
                    @EmitResults = 0,
                    @TransformScriptId = @TransformScriptId;

                {RenderReviewRows("[dq].[v_ReviewChecks] AS c CROSS JOIN [MetaDQ].[dbo].[RunLog] AS run", "run.RunId", "run.TransformName", showHints: true)}
                WHERE run.RunId = @RunId AND r.CheckNo IS NOT NULL
                  AND (@IncludeClean = 1 OR r.Outcome <> N'No rows found')
                ORDER BY [Transform], [Check], c.CandidateId;
            END;
            GO
            """);
        builder.AppendLine(ExplainSql.Value);
        return builder.ToString();
    }

    private static string RenderReviewRows(string from, string runId, string transform, bool showHints = false, string? scriptId = null)
    {
        var status = showHints ? RunHintSql : """
            CASE WHEN r.ReviewedAtUtc IS NOT NULL THEN N'No issue'
                 ELSE COALESCE(r.Outcome, N'Not checked') END AS [Outcome]
            """;
        var runCommand = scriptId is null ? "" : $"""
            WHEN {scriptId} IS NOT NULL THEN
                CONCAT(N'EXEC ', QUOTENAME(DB_NAME()), N'.dq.Run @Transform = N''',
                       REPLACE({scriptId}, N'''', N''''''), N''';')
            """;
        var actionColumn = showHints ? "[Next step]" : "[Investigate]";
        return $"""
            SELECT COALESCE({transform} COLLATE DATABASE_DEFAULT, r.Transforms COLLATE DATABASE_DEFAULT) AS [Transform],
                   COALESCE(r.FindingTitle, c.CheckName) AS [Check],
                   {status},
                   CASE WHEN r.CheckNo IS NOT NULL THEN
                       CONCAT(N'EXEC ', QUOTENAME(DB_NAME()), N'.dq.Explain @CheckNo = ', r.CheckNo,
                              N', @RunId = ', r.RunId, N';')
                   {runCommand}
                   END AS {actionColumn}
            FROM {from}
            LEFT JOIN [MetaDQ].[dbo].[CheckReview] AS r
              ON r.RunId = {runId} AND r.CandidateId COLLATE DATABASE_DEFAULT = c.CandidateId
            """;
    }

    private const string RunHintSql = """
        CASE
            WHEN r.ReviewedAtUtc IS NOT NULL THEN CONCAT_WS(N' ', N'No issue.', r.ReviewReason COLLATE DATABASE_DEFAULT)
            WHEN r.Outcome = N'Check failed' THEN N'The SQL check failed. Open the error before relying on it.'
            WHEN r.Outcome = N'No rows found' THEN N'The last run found no rows for this check.'
            WHEN r.Outcome = N'Review transform pattern' THEN
                CONCAT_WS(N' ', N'The transform differs from the common pattern.', c.CheckHint)
            WHEN r.Outcome = N'Review affected rows' THEN
                CONCAT_WS(N' ',
                    CASE
                        WHEN r.CandidateKind IN (N'JoinOrphan', N'OuterJoinNullExpansion', N'ImpliedForeignKeyMissingReference') AND r.SuspectRows IS NOT NULL THEN
                            CONCAT(r.SuspectRows, CASE WHEN r.SuspectRows = 1 THEN N' row had no match.' ELSE N' rows had no match.' END)
                        WHEN r.CandidateKind IN (N'JoinMultiplicityExplosion', N'OutputDuplicateRisk') AND r.SuspectRows IS NOT NULL THEN
                            CONCAT(r.FindingGroups, CASE WHEN r.FindingGroups = 1 THEN N' shared join key produced ' ELSE N' shared join keys produced ' END,
                                   r.SuspectRows, N' joined rows.')
                        WHEN r.CandidateKind = N'ImpliedUniqueKeyViolation' THEN CONCAT(r.FindingGroups, N' lookup keys repeated.')
                        ELSE N'The last run found evidence to review.'
                    END, c.CheckHint)
            ELSE CONCAT_WS(N' ', N'Not yet checked.', c.CheckHint)
        END AS [Why look]
        """;

    private static string RenderCombinedSql(
        IReadOnlyList<View> candidateViews,
        IReadOnlyDictionary<string, View> summaries,
        string reviewSql)
    {
        var builder = new StringBuilder();
        AppendSchemaGuard(builder);
        builder.AppendLine(OperationalSql.Value);

        foreach (var view in candidateViews)
        {
            AppendViewBatch(builder, view);
            AppendViewBatch(builder, summaries[ResolveCandidateId(view)]);
            builder.AppendLine();
        }

        builder.Append(reviewSql);
        return builder.ToString();
    }

    private static string RenderCheckSql(View view, View summary)
    {
        var builder = new StringBuilder();
        AppendSchemaGuard(builder);
        AppendViewBatch(builder, view);
        AppendViewBatch(builder, summary);
        return builder.ToString();
    }

    private static string SqlString(string value) => "N'" + value.Replace("'", "''", StringComparison.Ordinal) + "'";
    private static string SqlStringOrNull(string? value) => value is null ? "NULL" : SqlString(value);

    private static CheckTransform[] BuildTransformScope(MetaDataQualityModel source, HashSet<string> deployedCandidates)
    {
        var occurrences = source.JoinPatternOccurrenceList.ToLookup(row => row.JoinPattern.Id);
        var corpus = source.CorpusRelationshipPatternOccurrenceLinkList.ToLookup(
            row => row.CorpusRelationshipPattern.Id, row => row.JoinPatternOccurrence);
        var equivalences = source.CorpusColumnEquivalenceOccurrenceLinkList.ToLookup(
            row => row.CorpusColumnEquivalence.Id, row => row.JoinPatternOccurrence);

        IEnumerable<CheckTransform> Match(DataQualityCandidate candidate, IEnumerable<JoinPatternOccurrence> matches) =>
            deployedCandidates.Contains(candidate.Id)
                ? matches.Select(row => new CheckTransform(candidate.Id, row.TransformScriptId, row.TransformScriptName))
                : [];

        var scope = source.DataQualityCandidateJoinPatternLinkList.SelectMany(row => Match(row.DataQualityCandidate, occurrences[row.JoinPattern.Id]))
            .Concat(source.ImpliedForeignKeyMissingReferenceList.SelectMany(row => Match(row.DataQualityCandidate, corpus[row.DominantPattern.Id])))
            .Concat(source.ImpliedUniqueKeyViolationList.SelectMany(row => Match(row.DataQualityCandidate, corpus[row.DominantPattern.Id])))
            .Concat(source.ImpliedJoinFanoutRiskList.SelectMany(row => Match(row.DataQualityCandidate, corpus[row.DominantPattern.Id])))
            .Concat(source.ImpliedOutputDuplicateRiskList.SelectMany(row => Match(row.DataQualityCandidate, corpus[row.DominantPattern.Id])))
            .Concat(source.MinorityJoinPatternList.SelectMany(row => Match(row.DataQualityCandidate, corpus[row.OutlierPattern.Id])))
            .Concat(source.IncompleteCompositeJoinList.SelectMany(row => Match(row.DataQualityCandidate, corpus[row.OutlierPattern.Id])))
            .Concat(source.SuspiciousExtraJoinPredicateList.SelectMany(row => Match(row.DataQualityCandidate, corpus[row.OutlierPattern.Id])))
            .Concat(source.MissingCommonFilterList.SelectMany(row => Match(row.DataQualityCandidate, corpus[row.OutlierPattern.Id])))
            .Concat(source.MinorityColumnEquivalenceList.SelectMany(row => Match(row.DataQualityCandidate, equivalences[row.OutlierEquivalence.Id])))
            .Concat(source.InnerJoinAgainstUsuallyOptionalRelationshipList.SelectMany(row => Match(row.DataQualityCandidate, corpus[row.CorpusRelationshipPattern.Id])))
            .Concat(source.LeftJoinAgainstUsuallyMandatoryRelationshipList.SelectMany(row => Match(row.DataQualityCandidate, corpus[row.CorpusRelationshipPattern.Id])))
            .Distinct()
            .OrderBy(row => row.CandidateId, StringComparer.Ordinal)
            .ThenBy(row => row.ScriptId, StringComparer.Ordinal)
            .ThenBy(row => row.Name, StringComparer.Ordinal)
            .ToArray();

        foreach (var script in scope.GroupBy(row => row.ScriptId, StringComparer.Ordinal))
        {
            if (script.Key.Length > 512 || script.Any(row => row.Name.Length > 512))
                throw new InvalidOperationException($"Transform '{script.Key}' exceeds the 512-character DQ scope identifier or name limit.");
            if (script.Select(row => row.Name).Distinct(StringComparer.Ordinal).Skip(1).Any())
                throw new InvalidOperationException($"Transform '{script.Key}' has inconsistent names in the DQ workspace.");
        }

        return scope;
    }

    private static void AppendTransformScope(StringBuilder builder, IReadOnlyList<CheckTransform> scope)
    {
        builder.AppendLine("CREATE OR ALTER VIEW [dq].[v_ReviewCheckTransforms] AS");
        if (scope.Count == 0)
        {
            builder.AppendLine("SELECT CAST(NULL AS nvarchar(256)) AS CandidateId,");
            builder.AppendLine("       CAST(NULL AS nvarchar(512)) COLLATE Latin1_General_100_BIN2 AS TransformScriptId,");
            builder.AppendLine("       CAST(NULL AS nvarchar(512)) COLLATE Latin1_General_100_BIN2 AS [Transform] WHERE 1 = 0;");
        }
        else
        {
            builder.AppendLine("SELECT CAST(v.CandidateId AS nvarchar(256)) AS CandidateId,");
            builder.AppendLine("       CAST(v.TransformScriptId AS nvarchar(512)) COLLATE Latin1_General_100_BIN2 AS TransformScriptId,");
            builder.AppendLine("       CAST(v.TransformName AS nvarchar(512)) COLLATE Latin1_General_100_BIN2 AS [Transform]");
            builder.AppendLine("FROM (VALUES");
            for (var index = 0; index < scope.Count; index++)
            {
                var row = scope[index];
                builder.Append($"    ({SqlString(row.CandidateId)}, {SqlString(row.ScriptId)}, {SqlString(row.Name)})");
                builder.AppendLine(index + 1 == scope.Count ? "" : ",");
            }
            builder.AppendLine(") v(CandidateId, TransformScriptId, TransformName);");
        }
        builder.AppendLine("GO");
    }

    private sealed record CheckTransform(string CandidateId, string ScriptId, string Name);

    private static Dictionary<string, JoinReviewQueries> BuildJoinReviewQueries(MetaDataQualityModel source)
    {
        var kinds = source.JoinMultiplicityExplosionList.Select(row => (row.DataQualityCandidate.Id, CandidateKinds.JoinMultiplicityExplosion))
            .Concat(source.OutputDuplicateRiskList.Select(row => (row.DataQualityCandidate.Id, CandidateKinds.OutputDuplicateRisk)))
            .Concat(source.JoinOrphanList.Select(row => (row.DataQualityCandidate.Id, CandidateKinds.JoinOrphan)))
            .Concat(source.OuterJoinNullExpansionList.Select(row => (row.DataQualityCandidate.Id, CandidateKinds.OuterJoinNullExpansion)))
            .Concat(source.ImpliedForeignKeyMissingReferenceList.Select(row => (row.DataQualityCandidate.Id, CandidateKinds.ImpliedForeignKeyMissingReference)))
            .Concat(source.ImpliedUniqueKeyViolationList.Select(row => (row.DataQualityCandidate.Id, CandidateKinds.ImpliedUniqueKeyViolation)))
            .ToDictionary(pair => pair.Id, pair => pair.Item2, StringComparer.Ordinal);
        var corpusPatterns = source.CorpusRelationshipPatternOccurrenceLinkList.ToLookup(
            link => link.CorpusRelationshipPattern.Id, link => link.JoinPatternOccurrence.JoinPattern.Id);
        var patternLinks = source.DataQualityCandidateJoinPatternLinkList.Select(link => (link.DataQualityCandidate.Id, Pattern: link.JoinPattern.Id))
            .Concat(source.ImpliedForeignKeyMissingReferenceList.SelectMany(row => corpusPatterns[row.DominantPattern.Id]
                .Select(pattern => (row.DataQualityCandidate.Id, Pattern: pattern))))
            .Concat(source.ImpliedUniqueKeyViolationList.SelectMany(row => corpusPatterns[row.DominantPattern.Id]
                .Select(pattern => (row.DataQualityCandidate.Id, Pattern: pattern))));
        var keysByPattern = source.JoinPatternKeyPartList.ToLookup(key => key.JoinPattern.Id);
        var identifierParts = source.JoinPatternKeyPartInputObjectIdentifierPartList
            .ToLookup(part => (part.JoinPatternKeyPart.Id, part.InputSide));
        var queries = new Dictionary<string, JoinReviewQueries>(StringComparer.Ordinal);

        foreach (var group in patternLinks.Where(link => kinds.ContainsKey(link.Id)).GroupBy(link => link.Id))
        {
            var kind = kinds[group.Key];
            var patterns = group.Select(link => link.Pattern).Distinct().Order(StringComparer.Ordinal).ToArray();
            var fanout = kind is CandidateKinds.JoinMultiplicityExplosion or CandidateKinds.OutputDuplicateRisk;
            // The weave chooses the dominant orientation for implied checks. With multiple
            // patterns, use its deployed detail view instead of choosing a second orientation.
            if (patterns.Length != 1 && (fanout || kind is CandidateKinds.ImpliedForeignKeyMissingReference or CandidateKinds.ImpliedUniqueKeyViolation))
                continue;

            var joins = patterns.Select(pattern => DescribeJoin(keysByPattern[pattern], identifierParts)).ToArray();
            if (joins.Any(join => join is null))
                continue;

            var evidence = string.Join("\n\n", joins.Select(join => RenderEvidence(join!, kind)).Distinct());
            queries[group.Key] = new JoinReviewQueries(fanout ? RenderMatchProbe(joins[0]!) : null, evidence,
                DescribeCheckQuestion(kind, joins.Length == 1 ? joins[0] : null));
        }

        return queries;
    }

    private static ReviewJoin? DescribeJoin(IEnumerable<JoinPatternKeyPart> patternKeys,
        ILookup<(string, string), JoinPatternKeyPartInputObjectIdentifierPart> identifierParts)
    {
        // Like the generated runtime checks, use the modeled base-column equalities.
        // Comparison parts without two resolved columns (for example, a rank filter)
        // do not contribute to the base-table check.
        var keys = patternKeys.Where(key => !string.IsNullOrWhiteSpace(key.FirstJoinInputColumnName) &&
                                            !string.IsNullOrWhiteSpace(key.SecondJoinInputColumnName))
            .OrderBy(key => ParseDeployOrdinal(key.Ordinal)).ToArray();
        if (keys.Length == 0 || keys.Any(key =>
                !IsSqlIdentifier(key.FirstJoinInputColumnName) || !IsSqlIdentifier(key.SecondJoinInputColumnName)))
            return null;

        var first = keys.Select(key => ReadObjectName(identifierParts[(key.Id, "First")])).Distinct().ToArray();
        var second = keys.Select(key => ReadObjectName(identifierParts[(key.Id, "Second")])).Distinct().ToArray();
        if (first.Length != 1 || second.Length != 1 || first[0] is not { } left || second[0] is not { } right)
            return null;

        var firstColumns = keys.Select(key => key.FirstJoinInputColumnName!).Distinct().ToArray();
        var secondColumns = keys.Select(key => key.SecondJoinInputColumnName!).Distinct().ToArray();
        var condition = string.Join(" AND ", keys.Select(key =>
            $"a.{QuoteIdentifier(key.FirstJoinInputColumnName!)} = b.{QuoteIdentifier(key.SecondJoinInputColumnName!)}"));
        return new ReviewJoin(left, right, firstColumns, secondColumns, condition);
    }

    private static string RenderMatchProbe(ReviewJoin join) => $"""
        SELECT @Matched = COUNT_BIG(CASE WHEN m.Matches > 0 THEN 1 END),
               @Repeated = COUNT_BIG(CASE WHEN m.Matches > 1 THEN 1 END),
               @Maximum = MAX(m.Matches)
        FROM {join.Left.SqlName} AS a
        CROSS APPLY (SELECT COUNT_BIG(*) AS Matches FROM {join.Right.SqlName} AS b WHERE {join.Condition}) AS m;
        """;

    private static string DescribeCheckQuestion(string kind, ReviewJoin? join) => kind switch
    {
        CandidateKinds.JoinOrphan or CandidateKinds.ImpliedForeignKeyMissingReference => join is null
            ? "Which of the unmatched keys require a match?"
            : $"Should each {join.Right.Table} row have a match in {join.Left.Table}?",
        CandidateKinds.OuterJoinNullExpansion => join is null
            ? "Which of the rows kept by the outer join require a match?"
            : $"Should each {join.Left.Table} row have a match in {join.Right.Table}?",
        CandidateKinds.JoinMultiplicityExplosion => $"Can one {join!.Left.Table} row match several rows in {join.Right.Table}?",
        CandidateKinds.OutputDuplicateRisk => $"Could joining {join!.Left.Table} to {join.Right.Table} repeat an output key?",
        CandidateKinds.ImpliedUniqueKeyViolation => $"Should this join key identify one row in {join!.Left.Table}?",
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unsupported runtime check kind."),
    };

    private static string RenderEvidence(ReviewJoin join, string kind)
    {
        var leftKeys = string.Join(", ", join.LeftColumns.Select(column => $"a.{QuoteIdentifier(column)}"));
        var rightKeys = string.Join(", ", join.RightColumns.Select(column => $"b.{QuoteIdentifier(column)}"));
        var sameTableName = string.Equals(join.Left.Table, join.Right.Table, StringComparison.OrdinalIgnoreCase);
        var leftProjection = RenderKeyProjection("a", join.Left, join.LeftColumns, sameTableName ? "left" : null);
        if (kind == CandidateKinds.ImpliedUniqueKeyViolation)
            return $"""
                SELECT TOP (20) {leftProjection}, COUNT_BIG(*) AS [Rows with this key]
                FROM {join.Left.SqlName} AS a
                GROUP BY {leftKeys}
                HAVING COUNT_BIG(*) > 1
                ORDER BY COUNT_BIG(*) DESC, {leftKeys};
                """;

        var rightProjection = RenderKeyProjection("b", join.Right, join.RightColumns, sameTableName ? "right" : null);
        if (kind is CandidateKinds.JoinMultiplicityExplosion or CandidateKinds.OutputDuplicateRisk)
            return $"""
                SELECT DISTINCT TOP (20) {leftProjection}, {rightProjection}, m.Matches AS MatchesPerInputRow
                FROM {join.Left.SqlName} AS a
                CROSS APPLY (SELECT COUNT_BIG(*) AS Matches FROM {join.Right.SqlName} AS b WHERE {join.Condition}) AS m
                INNER JOIN {join.Right.SqlName} AS b ON {join.Condition}
                WHERE m.Matches > 1
                ORDER BY MatchesPerInputRow DESC, {leftKeys}, {rightKeys};
                """;

        var leftPreserved = kind == CandidateKinds.OuterJoinNullExpansion;
        var joinOperator = leftPreserved ? "LEFT" : "RIGHT";
        var missingKey = leftPreserved ? $"b.{QuoteIdentifier(join.RightColumns[0])}" : $"a.{QuoteIdentifier(join.LeftColumns[0])}";
        return $"""
            SELECT TOP (20) {leftProjection}, {rightProjection}
            FROM {join.Left.SqlName} AS a
            {joinOperator} JOIN {join.Right.SqlName} AS b ON {join.Condition}
            WHERE {missingKey} IS NULL
            ORDER BY {(leftPreserved ? leftKeys : rightKeys)};
            """;
    }

    private static string RenderKeyProjection(string alias, ReviewObject table, IEnumerable<string> columns, string? side) =>
        string.Join(", ", columns.Select(column =>
        {
            var label = side is null ? $"{table.Table}.{column}" : $"{table.Table}.{column} ({side})";
            // SQL Server limits the length of a column alias as well as an object name.
            return $"{alias}.{QuoteIdentifier(column)} AS {QuoteIdentifier(label.Length <= 128 ? label : alias + "." + column[..Math.Min(126, column.Length)])}";
        }));

    private static ReviewObject? ReadObjectName(IEnumerable<JoinPatternKeyPartInputObjectIdentifierPart> parts)
    {
        var identifiers = parts.OrderBy(part => ParseDeployOrdinal(part.Ordinal)).Select(part => part.Value).ToArray();
        return identifiers.Length is 2 or 3 && identifiers.All(IsSqlIdentifier)
            ? new ReviewObject(string.Join(".", identifiers.Select(QuoteIdentifier)), identifiers[^1])
            : null;
    }

    private sealed record ReviewObject(string SqlName, string Table);
    private sealed record ReviewJoin(ReviewObject Left, ReviewObject Right, string[] LeftColumns, string[] RightColumns, string Condition);
    private sealed record JoinReviewQueries(string? MatchProbe, string EvidenceQuery, string CheckHint);

    private static bool IsSqlIdentifier(string? value) => value is { Length: > 0 and <= 128 };

    private static string QuoteIdentifier(string value) => "[" + value.Replace("]", "]]", StringComparison.Ordinal) + "]";

    private static void AppendSchemaGuard(StringBuilder builder)
    {
        builder.AppendLine("IF SCHEMA_ID(N'dq') IS NULL EXEC(N'CREATE SCHEMA [dq]');");
        builder.AppendLine("GO");
        builder.AppendLine();
    }

    private static void AppendViewBatch(StringBuilder builder, View view)
    {
        if (string.IsNullOrWhiteSpace(view.DefinitionSql))
        {
            throw new InvalidOperationException(
                $"The sanctioned Data-Quality-to-SQL weave produced view '{view.Id}' " +
                "without DefinitionSql.");
        }

        builder.Append(view.DefinitionSql.TrimEnd());
        builder.AppendLine();
        builder.AppendLine("GO");
    }

    private static string ResolveCandidateId(View view)
    {
        const string prefix = "v_";
        if (!view.Name.StartsWith(prefix, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"The sanctioned Data-Quality-to-SQL weave produced candidate view " +
                $"'{view.Id}' without the required '{prefix}' name prefix.");
        }

        return view.Name[prefix.Length..];
    }

    private static int ParseDeployOrdinal(string? value) =>
        int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : int.MaxValue;

    private static bool IsSqlFilePath(string path) =>
        string.Equals(Path.GetExtension(path), ".sql", StringComparison.OrdinalIgnoreCase);

    private static string SanitizeFileName(string value)
    {
        var invalidChars = Path.GetInvalidFileNameChars().ToHashSet();
        var chars = value
            .Select(ch => invalidChars.Contains(ch) ? '_' : ch)
            .ToArray();
        var sanitized = new string(chars).Trim();
        return string.IsNullOrWhiteSpace(sanitized)
            ? "DataQualityCandidate"
            : sanitized;
    }

    private static DataQualityToSqlResult CreateResult(
        string outputPath,
        int candidateViewCount) =>
        new()
        {
            OutputPath = outputPath,
            CandidateViewCount = candidateViewCount,
            DashboardViewCount = 1,
            OperationalTableCount = 4,
            OperationalProcedureCount = 2,
        };

    private static string LoadOperationalSql()
    {
        return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, OperationalSqlFileName), Encoding.UTF8);
    }

    private static string ResolveWeaveWorkspacePath()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Weaves",
            "DataQualityToSql");
        if (!File.Exists(Path.Combine(path, "workspace.meta")))
        {
            throw new InvalidOperationException(
                $"The sanctioned Data-Quality-to-SQL weave was not found at '{path}'.");
        }

        return path;
    }
}

public sealed class DataQualityToSqlResult
{
    public required string OutputPath { get; init; }

    public required int CandidateViewCount { get; init; }

    public required int DashboardViewCount { get; init; }

    public required int OperationalTableCount { get; init; }

    public required int OperationalProcedureCount { get; init; }

    public int ScriptCount => CandidateViewCount + DashboardViewCount + 1;
}
