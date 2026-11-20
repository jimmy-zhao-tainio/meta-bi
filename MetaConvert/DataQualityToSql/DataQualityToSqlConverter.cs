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
                                       view.Name.Equals("v_ReviewChecks", StringComparison.OrdinalIgnoreCase)))
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
        var reviewSql = RenderReviewSql(candidateViews, candidateById, summaryViews, BuildMatchProbes(source));

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
        IReadOnlyDictionary<string, string> matchProbes)
    {
        var builder = new StringBuilder();
        AppendSchemaGuard(builder);
        builder.AppendLine("CREATE OR ALTER VIEW [dq].[v_ReviewChecks] AS");
        if (candidateViews.Count == 0)
        {
            builder.AppendLine("SELECT CAST(NULL AS nvarchar(256)) AS CandidateId, CAST(NULL AS sysname) AS SummaryView,");
            builder.AppendLine("       CAST(NULL AS sysname) AS DetailView, CAST(NULL AS nvarchar(256)) AS CheckName,");
            builder.AppendLine("       CAST(NULL AS nvarchar(max)) AS MatchProbe WHERE 1 = 0;");
        }
        else
        {
            builder.AppendLine("SELECT CAST(v.CandidateId AS nvarchar(256)) AS CandidateId,");
            builder.AppendLine("       CAST(v.SummaryView AS sysname) AS SummaryView, CAST(v.DetailView AS sysname) AS DetailView,");
            builder.AppendLine("       CAST(v.CheckName AS nvarchar(256)) AS CheckName, CAST(v.MatchProbe AS nvarchar(max)) AS MatchProbe");
            builder.AppendLine("FROM (VALUES");
            for (var index = 0; index < candidateViews.Count; index++)
            {
                var view = candidateViews[index];
                var id = ResolveCandidateId(view);
                if (!candidates.TryGetValue(id, out var candidate))
                    throw new InvalidOperationException($"The Data Quality weave produced a view for unknown candidate '{id}'.");
                var matchProbe = matchProbes.TryGetValue(id, out var probe) ? SqlString(probe) : "NULL";
                builder.Append($"    ({SqlString(id)}, {SqlString(summaries[id].Name)}, {SqlString(view.Name)}, {SqlString(candidate.Name)}, {matchProbe})");
                builder.AppendLine(index + 1 == candidateViews.Count ? "" : ",");
            }
            builder.AppendLine(") v(CandidateId, SummaryView, DetailView, CheckName, MatchProbe);");
        }

        builder.AppendLine("GO");
        builder.AppendLine($"""
            CREATE OR ALTER VIEW [dq].[v_DataQualityReview] AS
            WITH LatestRun AS
            (
                SELECT TOP (1) RunId
                FROM [MetaDQ].[dbo].[RunLog]
                WHERE SourceDatabaseName = DB_NAME()
                ORDER BY RunId DESC
            )
            {RenderReviewRows("LEFT JOIN LatestRun AS run ON 1 = 1")};
            GO

            CREATE OR ALTER PROCEDURE [dq].[Run]
                @IncludeClean bit = 0,
                @RunId bigint = NULL OUTPUT
            AS
            BEGIN
                SET NOCOUNT ON;

                DECLARE @SourceDatabaseName sysname = DB_NAME();
                EXEC [MetaDQ].[dbo].[Run]
                    @SourceDatabaseName = @SourceDatabaseName,
                    @RunId = @RunId OUTPUT,
                    @EmitResults = 0;

                {RenderReviewRows("JOIN [MetaDQ].[dbo].[RunLog] AS run ON run.RunId = @RunId")}
                WHERE @IncludeClean = 1 OR r.Outcome <> N'No rows found' OR r.Outcome IS NULL
                ORDER BY [Transform], [Check], c.CandidateId;
            END;
            GO
            """);
        builder.AppendLine(ExplainSql.Value);
        return builder.ToString();
    }

    private static string RenderReviewRows(string runJoin) => $"""
        SELECT r.Transforms AS [Transform],
               COALESCE(r.FindingTitle, c.CheckName) AS [Check],
               COALESCE(r.Outcome, N'Not checked') AS [Outcome],
               CASE WHEN r.CheckNo IS NOT NULL THEN
                   CONCAT(N'EXEC ', QUOTENAME(DB_NAME()), N'.dq.Explain @CheckNo = ', r.CheckNo,
                          N', @RunId = ', r.RunId, N';')
               END AS [Investigate]
        FROM [dq].[v_ReviewChecks] AS c
        {runJoin}
        LEFT JOIN [MetaDQ].[dbo].[CheckReview] AS r
          ON r.RunId = run.RunId AND r.CandidateId COLLATE DATABASE_DEFAULT = c.CandidateId
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

    private static Dictionary<string, string> BuildMatchProbes(MetaDataQualityModel source)
    {
        var candidateIds = source.JoinMultiplicityExplosionList.Select(row => row.DataQualityCandidate.Id)
            .Concat(source.OutputDuplicateRiskList.Select(row => row.DataQualityCandidate.Id))
            .ToHashSet(StringComparer.Ordinal);
        var keysByPattern = source.JoinPatternKeyPartList.ToLookup(key => key.JoinPattern.Id);
        var identifierParts = source.JoinPatternKeyPartInputObjectIdentifierPartList
            .ToLookup(part => (part.JoinPatternKeyPart.Id, part.InputSide));
        var probes = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var group in source.DataQualityCandidateJoinPatternLinkList
                     .Where(link => candidateIds.Contains(link.DataQualityCandidate.Id))
                     .GroupBy(link => link.DataQualityCandidate.Id))
        {
            var patterns = group.Select(link => link.JoinPattern.Id).Distinct().ToArray();
            if (patterns.Length != 1)
                continue;

            var keys = keysByPattern[patterns[0]].OrderBy(key => ParseDeployOrdinal(key.Ordinal)).ToArray();
            if (keys.Length == 0 || keys.Any(key =>
                    !IsSqlIdentifier(key.FirstJoinInputColumnName) || !IsSqlIdentifier(key.SecondJoinInputColumnName)))
                continue;

            var firstTables = keys.Select(key => RenderObjectName(identifierParts[(key.Id, "First")])).Distinct().ToArray();
            var secondTables = keys.Select(key => RenderObjectName(identifierParts[(key.Id, "Second")])).Distinct().ToArray();
            if (firstTables.Length != 1 || secondTables.Length != 1 || firstTables[0] is null || secondTables[0] is null)
                continue;

            var equalities = string.Join(" AND ", keys.Select(key =>
                $"a.{QuoteIdentifier(key.FirstJoinInputColumnName!)} = b.{QuoteIdentifier(key.SecondJoinInputColumnName!)}"));
            var firstKeys = string.Join(", ", keys.Select(key => $"a.{QuoteIdentifier(key.FirstJoinInputColumnName!)}").Distinct());
            var matchedRows = $"""
                FROM {firstTables[0]} AS a
                CROSS APPLY (SELECT COUNT_BIG(*) AS Matches FROM {secondTables[0]} AS b WHERE {equalities}) AS m
                """;
            // Count matches for each first-side row. Grouping the joined rows by the key alone
            // also counts legitimate reuse by several first-side rows and cannot prove fanout.
            probes[group.Key] = $"""
                IF @ShowKeys = 1
                BEGIN
                    SELECT TOP (20) {firstKeys}, m.Matches AS MatchesPerInputRow, COUNT_BIG(*) AS AffectedInputRows
                    {matchedRows}
                    WHERE m.Matches > 1
                    GROUP BY {firstKeys}, m.Matches
                    ORDER BY m.Matches DESC, {firstKeys};
                    RETURN;
                END;
                SELECT @Matched = COUNT_BIG(CASE WHEN m.Matches > 0 THEN 1 END),
                       @Repeated = COUNT_BIG(CASE WHEN m.Matches > 1 THEN 1 END),
                       @Maximum = MAX(m.Matches)
                {matchedRows};
                """;
        }

        return probes;
    }

    private static string? RenderObjectName(IEnumerable<JoinPatternKeyPartInputObjectIdentifierPart> parts)
    {
        var identifiers = parts.OrderBy(part => ParseDeployOrdinal(part.Ordinal)).Select(part => part.Value).ToArray();
        return identifiers.Length is 2 or 3 && identifiers.All(IsSqlIdentifier)
            ? string.Join(".", identifiers.Select(QuoteIdentifier))
            : null;
    }

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
            OperationalTableCount = 3,
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
