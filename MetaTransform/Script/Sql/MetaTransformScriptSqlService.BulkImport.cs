using System.Collections;
using System.Reflection;
using MetaTransformScript.Instance;
using MTS = global::MetaTransformScript;

namespace MetaTransformScript.Sql;

public sealed partial class MetaTransformScriptSqlService
{
    public async Task<ImportSqlFilesToWorkspaceResult> ImportSqlFilesToNewXmlWorkspaceAsync(
        IEnumerable<SqlFileImportRequest> requests,
        string newWorkspacePath,
        Action<SqlFileImportProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requests);
        ArgumentException.ThrowIfNullOrWhiteSpace(newWorkspacePath);

        var workspaceFullPath = Path.GetFullPath(newWorkspacePath);
        EnsureTargetDirectoryIsEmpty(workspaceFullPath);

        var result = await ImportSqlFilesAsync(
                requests,
                MTS.MetaTransformScriptModel.CreateEmpty(),
                progress,
                cancellationToken)
            .ConfigureAwait(false);
        if (result.Successes.Count > 0)
        {
            Directory.CreateDirectory(workspaceFullPath);
            await Meta.Integration.TypedWorkspaceModelMapper.CreateAsync(
                    result.Model,
                    workspaceFullPath,
                    "xml",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        return result with { WorkspacePath = workspaceFullPath };
    }

    public async Task<ImportSqlFilesToWorkspaceResult> AddSqlFilesToWorkspaceAsync(
        IEnumerable<SqlFileImportRequest> requests,
        string workspacePath,
        Action<SqlFileImportProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requests);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);

        var workspaceFullPath = Path.GetFullPath(workspacePath);
        var model = await MetaTransformScriptInstance.LoadFromWorkspaceAsync(
                workspaceFullPath,
                searchUpward: false,
                cancellationToken)
            .ConfigureAwait(false);

        var result = await ImportSqlFilesAsync(
                requests,
                model,
                progress,
                cancellationToken)
            .ConfigureAwait(false);
        if (result.Successes.Count > 0)
        {
            await MetaTransformScriptInstance.SaveToWorkspaceAsync(result.Model, workspaceFullPath, cancellationToken)
                .ConfigureAwait(false);
        }

        return result with { WorkspacePath = workspaceFullPath };
    }

    public Task<ImportSqlFilesToWorkspaceResult> ImportSqlFilesAsync(
        IEnumerable<SqlFileImportRequest> requests,
        MTS.MetaTransformScriptModel model,
        Action<SqlFileImportProgress>? progress,
        CancellationToken cancellationToken)
    {
        var importRequests = requests.ToArray();
        if (importRequests.Length == 0)
        {
            throw new MetaTransformScriptSqlImportException(
                MetaTransformScriptSqlImportFailureKind.InvalidSqlInput,
                "SQL files import manifest did not contain any files.");
        }

        var successes = new List<SqlFileImportSuccess>();
        var failures = new List<SqlFileImportFailure>();
        var nextIds = ReadNextIdState(model);
        var total = importRequests.Length;

        for (var index = 0; index < importRequests.Length; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = importRequests[index];
            var resultIndex = index + 1;
            var fullPath = string.IsNullOrWhiteSpace(request.Path)
                ? string.Empty
                : Path.GetFullPath(request.Path);

            try
            {
                if (string.IsNullOrWhiteSpace(fullPath))
                {
                    throw new MetaTransformScriptSqlImportException(
                        MetaTransformScriptSqlImportFailureKind.InvalidSqlInput,
                        "SQL files import request has a blank Path.");
                }

                if (!File.Exists(fullPath))
                {
                    throw new MetaTransformScriptSqlImportException(
                        MetaTransformScriptSqlImportFailureKind.SourcePathNotFound,
                        $"SQL file '{fullPath}' was not found.");
                }

                if (!string.Equals(Path.GetExtension(fullPath), ".sql", StringComparison.OrdinalIgnoreCase))
                {
                    throw new MetaTransformScriptSqlImportException(
                        MetaTransformScriptSqlImportFailureKind.InvalidSqlInput,
                        $"SQL file '{fullPath}' must use a .sql extension.");
                }

                var imported = ImportFromSingleSqlFile(fullPath);
                ApplySingleScriptImportTarget(imported, request.TargetSqlIdentifier, Path.GetFileName(fullPath));
                if (imported.TransformScriptList.Count != 1)
                {
                    throw new MetaTransformScriptSqlImportException(
                        MetaTransformScriptSqlImportFailureKind.InvalidSqlInput,
                        $"SQL import for '{Path.GetFileName(fullPath)}' produced {imported.TransformScriptList.Count} transform scripts; one file must contain one importable script.");
                }

                RemapModelIds(imported, nextIds);
                AppendModelRows(model, imported);

                var success = new SqlFileImportSuccess(
                    resultIndex,
                    total,
                    fullPath,
                    request.TargetSqlIdentifier,
                    imported.TransformScriptList[0].Name,
                    model.TransformScriptList.Count);
                successes.Add(success);
                NotifyProgress(progress, SqlFileImportProgress.Succeeded(success));
            }
            catch (MetaTransformScriptSqlImportException ex)
            {
                var failure = new SqlFileImportFailure(
                    resultIndex,
                    total,
                    string.IsNullOrWhiteSpace(fullPath) ? request.Path : fullPath,
                    request.TargetSqlIdentifier,
                    ex.Kind,
                    ex.Message)
                {
                    Line = ex.Line,
                    Column = ex.Column,
                    Offset = ex.Offset
                };
                failures.Add(failure);
                NotifyProgress(progress, SqlFileImportProgress.Failed(failure));
            }
            catch (Exception ex)
            {
                var failure = new SqlFileImportFailure(
                    resultIndex,
                    total,
                    string.IsNullOrWhiteSpace(fullPath) ? request.Path : fullPath,
                    request.TargetSqlIdentifier,
                    MetaTransformScriptSqlImportFailureKind.InvalidSqlInput,
                    ex.Message);
                failures.Add(failure);
                NotifyProgress(progress, SqlFileImportProgress.Failed(failure));
            }
        }

        return Task.FromResult(new ImportSqlFilesToWorkspaceResult(
            model,
            model.TransformScriptList.Count,
            string.Empty,
            successes,
            failures));
    }

    private static void NotifyProgress(
        Action<SqlFileImportProgress>? progress,
        SqlFileImportProgress importProgress)
    {
        if (progress is null)
        {
            return;
        }

        try
        {
            progress(importProgress);
        }
        catch
        {
            // Progress is observational only. A broken console or caller progress hook
            // must not turn a successfully modeled import into a failed SQL import.
        }
    }

    private static void RemapModelIds(
        MTS.MetaTransformScriptModel model,
        IDictionary<string, int> nextIds)
    {
        foreach (var property in GetModelListProperties())
        {
            var entityName = property.Name[..^"List".Length];
            foreach (var row in ReadRows(property, model))
            {
                var idProperty = row.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
                if (idProperty is null || !idProperty.CanWrite || idProperty.PropertyType != typeof(string))
                {
                    continue;
                }

                nextIds.TryGetValue(entityName, out var nextId);
                nextId++;
                nextIds[entityName] = nextId;
                idProperty.SetValue(row, $"{entityName}:{nextId}");
            }
        }
    }

    private static void AppendModelRows(
        MTS.MetaTransformScriptModel target,
        MTS.MetaTransformScriptModel source)
    {
        foreach (var property in GetModelListProperties())
        {
            if (property.GetValue(target) is not IList targetRows)
            {
                throw new InvalidOperationException($"MetaTransformScript model list '{property.Name}' is not appendable.");
            }

            foreach (var row in ReadRows(property, source))
            {
                targetRows.Add(row);
            }
        }
    }

    private static Dictionary<string, int> ReadNextIdState(MTS.MetaTransformScriptModel model)
    {
        var nextIds = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var property in GetModelListProperties())
        {
            var entityName = property.Name[..^"List".Length];
            foreach (var row in ReadRows(property, model))
            {
                var idValue = row.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance)?.GetValue(row) as string;
                if (TryReadEntityId(idValue, entityName, out var numericId) &&
                    (!nextIds.TryGetValue(entityName, out var current) || numericId > current))
                {
                    nextIds[entityName] = numericId;
                }
            }
        }

        return nextIds;
    }

    private static IReadOnlyList<PropertyInfo> GetModelListProperties() =>
        typeof(MTS.MetaTransformScriptModel)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(static property =>
                property.Name.EndsWith("List", StringComparison.Ordinal) &&
                typeof(IList).IsAssignableFrom(property.PropertyType) &&
                property.GetIndexParameters().Length == 0)
            .OrderBy(static property => property.Name, StringComparer.Ordinal)
            .ToArray();

    private static IEnumerable<object> ReadRows(PropertyInfo property, MTS.MetaTransformScriptModel model)
    {
        if (property.GetValue(model) is not IEnumerable rows)
        {
            yield break;
        }

        foreach (var row in rows)
        {
            if (row is not null)
            {
                yield return row;
            }
        }
    }

    private static bool TryReadEntityId(string? value, string expectedEntityName, out int numericId)
    {
        numericId = 0;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var separator = value.LastIndexOf(':');
        if (separator <= 0 || separator >= value.Length - 1)
        {
            return false;
        }

        var entityName = value[..separator];
        if (!string.Equals(entityName, expectedEntityName, StringComparison.Ordinal))
        {
            return false;
        }

        return int.TryParse(value[(separator + 1)..], out numericId);
    }
}

public sealed record SqlFileImportRequest(
    string Path,
    string? TargetSqlIdentifier);

public sealed record SqlFileImportSuccess(
    int Index,
    int Total,
    string Path,
    string? TargetSqlIdentifier,
    string ScriptName,
    int WorkspaceScriptCount);

public sealed record SqlFileImportFailure(
    int Index,
    int Total,
    string Path,
    string? TargetSqlIdentifier,
    MetaTransformScriptSqlImportFailureKind Kind,
    string Message)
{
    public int? Line { get; init; }

    public int? Column { get; init; }

    public int? Offset { get; init; }
}

public sealed record SqlFileImportProgress(
    int Index,
    int Total,
    string Path,
    string? TargetSqlIdentifier,
    bool Success,
    string? ScriptName,
    MetaTransformScriptSqlImportFailureKind? FailureKind,
    string? Message)
{
    public static SqlFileImportProgress Succeeded(SqlFileImportSuccess success) =>
        new(
            success.Index,
            success.Total,
            success.Path,
            success.TargetSqlIdentifier,
            Success: true,
            success.ScriptName,
            FailureKind: null,
            Message: null);

    public static SqlFileImportProgress Failed(SqlFileImportFailure failure) =>
        new(
            failure.Index,
            failure.Total,
            failure.Path,
            failure.TargetSqlIdentifier,
            Success: false,
            ScriptName: null,
            failure.Kind,
            failure.Message)
        {
            Line = failure.Line,
            Column = failure.Column,
            Offset = failure.Offset
        };

    public int? Line { get; init; }

    public int? Column { get; init; }

    public int? Offset { get; init; }
}

public sealed record ImportSqlFilesToWorkspaceResult(
    MTS.MetaTransformScriptModel Model,
    int ScriptCount,
    string WorkspacePath,
    IReadOnlyList<SqlFileImportSuccess> Successes,
    IReadOnlyList<SqlFileImportFailure> Failures);
