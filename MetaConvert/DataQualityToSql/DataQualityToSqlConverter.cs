using System.Globalization;
using System.Reflection;
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
    private const string DashboardViewName = "v_DataQualityReview";
    private const string MetaSqlDatabaseName = "DataQuality";
    private const string OperationalSqlResourceName =
        "MetaConvert.DataQualityToSql.MetaDQ.Operational.sql";

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

        var source = TypedWorkspaceModelMapper.Load<MetaDataQualityModel>(
            workspacePath,
            searchUpward: false);
        var metaSql = Materialize(source, progress);
        var dashboard = ResolveDashboard(metaSql);
        var candidateViews = metaSql.ViewList
            .Where(view => !string.Equals(view.Name, DashboardViewName, StringComparison.Ordinal))
            .OrderBy(static view => ParseDeployOrdinal(view.DeployOrdinal))
            .ThenBy(static view => view.Name, StringComparer.Ordinal)
            .ToArray();

        if (IsSqlFilePath(outputPath))
        {
            var fullPath = Path.GetFullPath(outputPath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            File.WriteAllText(
                fullPath,
                RenderCombinedSql(candidateViews, dashboard),
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            return CreateResult(fullPath, candidateViews.Length);
        }

        var outputDirectory = Path.GetFullPath(outputPath);
        Directory.CreateDirectory(outputDirectory);
        var candidateById = source.DataQualityCandidateList.ToDictionary(
            candidate => candidate.Id,
            StringComparer.Ordinal);

        foreach (var view in candidateViews)
        {
            var candidateId = ResolveCandidateId(view);
            if (!candidateById.TryGetValue(candidateId, out var candidate))
            {
                throw new InvalidOperationException(
                    $"The sanctioned Data-Quality-to-SQL weave produced view '{view.Id}' " +
                    $"for unknown candidate '{candidateId}'.");
            }

            var filePath = Path.Combine(
                outputDirectory,
                $"{SanitizeFileName(candidate.Name)}.sql");
            File.WriteAllText(
                filePath,
                RenderViewSql(view, includeSchemaGuard: true),
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        }

        File.WriteAllText(
            Path.Combine(outputDirectory, "v_DataQualityReview.sql"),
            RenderViewSql(dashboard, includeSchemaGuard: true),
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        File.WriteAllText(
            Path.Combine(outputDirectory, "MetaDQ.Operational.sql"),
            OperationalSql.Value,
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        return CreateResult(outputDirectory, candidateViews.Length);
    }

    private static MetaSqlModel Materialize(
        MetaDataQualityModel source,
        Action<MetaWeaveScriptExecutionProgress>? progress)
    {
        var result = new MetaWeaveScriptExecutionService().ExecuteDirection(
            ForwardDirection.Value,
            new Dictionary<string, InMemoryWorkspace>(StringComparer.OrdinalIgnoreCase)
            {
                ["quality"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(source),
            },
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(MetaSqlModel.CreateEmpty()),
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

        return TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result.OutputWorkspace!,
            MetaSqlModel.CreateEmpty);
    }

    private static View ResolveDashboard(MetaSqlModel metaSql)
    {
        var dashboards = metaSql.ViewList
            .Where(view => string.Equals(view.Name, DashboardViewName, StringComparison.Ordinal))
            .ToArray();
        if (dashboards.Length != 1)
        {
            throw new InvalidOperationException(
                $"The sanctioned Data-Quality-to-SQL weave produced {dashboards.Length} " +
                $"'{DashboardViewName}' views; exactly one is required.");
        }

        return dashboards[0];
    }

    private static string RenderCombinedSql(
        IReadOnlyList<View> candidateViews,
        View dashboard)
    {
        var builder = new StringBuilder();
        AppendSchemaGuard(builder);

        foreach (var view in candidateViews)
        {
            AppendViewBatch(builder, view);
            builder.AppendLine();
        }

        AppendViewBatch(builder, dashboard);
        builder.AppendLine();
        builder.Append(OperationalSql.Value);
        return builder.ToString();
    }

    private static string RenderViewSql(View view, bool includeSchemaGuard)
    {
        var builder = new StringBuilder();
        if (includeSchemaGuard)
        {
            AppendSchemaGuard(builder);
        }

        AppendViewBatch(builder, view);
        return builder.ToString();
    }

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
            OperationalTableCount = 2,
            OperationalProcedureCount = 2,
        };

    private static string LoadOperationalSql()
    {
        using var stream = typeof(DataQualityToSqlConverter).Assembly
            .GetManifestResourceStream(OperationalSqlResourceName);
        if (stream is null)
        {
            throw new InvalidOperationException(
                $"The fixed MetaDQ operational SQL resource '{OperationalSqlResourceName}' was not found.");
        }

        using var reader = new StreamReader(
            stream,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true);
        return reader.ReadToEnd();
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
