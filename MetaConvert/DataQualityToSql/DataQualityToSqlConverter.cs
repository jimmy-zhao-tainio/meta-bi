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
        var dashboardSql = RenderReviewDashboard(candidateViews, candidateById, summaryViews);

        if (IsSqlFilePath(outputPath))
        {
            var fullPath = Path.GetFullPath(outputPath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            File.WriteAllText(
                fullPath,
                RenderCombinedSql(candidateViews, summaryViews, dashboardSql),
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
            dashboardSql,
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

    private static string RenderReviewDashboard(
        IReadOnlyList<View> candidateViews,
        IReadOnlyDictionary<string, DataQualityCandidate> candidates,
        IReadOnlyDictionary<string, View> summaries)
    {
        var builder = new StringBuilder();
        AppendSchemaGuard(builder);
        builder.AppendLine("CREATE OR ALTER VIEW [dq].[v_ReviewChecks] AS");
        if (candidateViews.Count == 0)
        {
            builder.AppendLine("SELECT CAST(NULL AS nvarchar(256)) AS CandidateId, CAST(NULL AS sysname) AS SummaryView,");
            builder.AppendLine("       CAST(NULL AS sysname) AS DetailView, CAST(NULL AS nvarchar(256)) AS CheckName WHERE 1 = 0;");
        }
        else
        {
            builder.AppendLine("SELECT CAST(v.CandidateId AS nvarchar(256)) AS CandidateId,");
            builder.AppendLine("       CAST(v.SummaryView AS sysname) AS SummaryView, CAST(v.DetailView AS sysname) AS DetailView,");
            builder.AppendLine("       CAST(v.CheckName AS nvarchar(256)) AS CheckName");
            builder.AppendLine("FROM (VALUES");
            for (var index = 0; index < candidateViews.Count; index++)
            {
                var view = candidateViews[index];
                var id = ResolveCandidateId(view);
                if (!candidates.TryGetValue(id, out var candidate))
                    throw new InvalidOperationException($"The Data Quality weave produced a view for unknown candidate '{id}'.");
                builder.Append($"    ({SqlString(id)}, {SqlString(summaries[id].Name)}, {SqlString(view.Name)}, {SqlString(candidate.Name)})");
                builder.AppendLine(index + 1 == candidateViews.Count ? "" : ",");
            }
            builder.AppendLine(") v(CandidateId, SummaryView, DetailView, CheckName);");
        }

        builder.AppendLine("GO");
        builder.AppendLine("""
            CREATE OR ALTER VIEW [dq].[v_DataQualityReview] AS
            WITH LatestRun AS
            (
                SELECT TOP (1) RunId, Status
                FROM [MetaDQ].[dbo].[RunLog]
                WHERE SourceDatabaseName = DB_NAME()
                ORDER BY RunId DESC
            )
            SELECT COALESCE(r.FindingTitle, c.CheckName) AS [Check],
                   COALESCE(r.Outcome, N'Not checked') AS [Outcome],
                   r.CheckedRelationship AS [Relationship],
                   r.Transforms,
                   r.FindingGroups,
                   r.SuspectRows,
                   r.Explanation,
                   r.CheckedAtUtc,
                   CONCAT(N'SELECT * FROM ', QUOTENAME(DB_NAME()), N'.[dq].', QUOTENAME(c.DetailView), N';') AS [InspectCurrentRows],
                   c.CandidateId AS [CheckId],
                   latest.RunId,
                   latest.Status AS RunStatus
            FROM [dq].[v_ReviewChecks] AS c
            LEFT JOIN LatestRun AS latest ON 1 = 1
            LEFT JOIN [MetaDQ].[dbo].[CheckReview] AS r
              ON r.RunId = latest.RunId AND r.CandidateId COLLATE DATABASE_DEFAULT = c.CandidateId;
            GO
            """);
        return builder.ToString();
    }

    private static string RenderCombinedSql(
        IReadOnlyList<View> candidateViews,
        IReadOnlyDictionary<string, View> summaries,
        string dashboardSql)
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

        builder.Append(dashboardSql);
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
