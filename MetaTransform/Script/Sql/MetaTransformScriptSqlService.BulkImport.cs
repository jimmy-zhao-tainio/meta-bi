using MetaTransformScript.Sql.Parsing;
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
                    MetaTransformScriptModels.LoadContract(),
                    new MTS.MetaTransformScriptWorkspaceBinding(),
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
        var parser = new MetaTransformScriptSqlParser();
        var builder = new MetaTransformScriptSqlModelBuilder(model);
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

                var checkpoint = builder.CreateCheckpoint();
                MTS.TransformScript importedScript;
                try
                {
                    var existingScriptCount = model.TransformScriptList.Count;
                    var sourcePath = Path.GetFileName(fullPath);
                    ParseSqlSourcesIntoBuilder(
                        [new SqlImportSource(File.ReadAllText(fullPath), sourcePath, BareSelectName: null)],
                        parser,
                        builder);

                    var addedScriptCount = model.TransformScriptList.Count - existingScriptCount;
                    if (addedScriptCount != 1)
                    {
                        throw new MetaTransformScriptSqlImportException(
                            MetaTransformScriptSqlImportFailureKind.InvalidSqlInput,
                            $"SQL import for '{sourcePath}' produced {addedScriptCount} transform scripts; one file must contain one importable script.");
                    }

                    importedScript = model.TransformScriptList[^1];
                    ApplyImportTargetToScript(
                        model,
                        importedScript,
                        request.TargetSqlIdentifier,
                        sourcePath);
                }
                catch
                {
                    builder.Rollback(checkpoint);
                    throw;
                }

                var success = new SqlFileImportSuccess(
                    resultIndex,
                    total,
                    fullPath,
                    request.TargetSqlIdentifier,
                    importedScript.Name,
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
        IDictionary<string, int> nextIds) =>
        MTS.MetaTransformScriptModelAccess.VisitCollections(new RemapIdsVisitor(model, nextIds));

    private static void AppendModelRows(
        MTS.MetaTransformScriptModel target,
        MTS.MetaTransformScriptModel source) =>
        MTS.MetaTransformScriptModelAccess.VisitCollections(new AppendRowsVisitor(target, source));

    private static Dictionary<string, int> ReadNextIdState(MTS.MetaTransformScriptModel model)
    {
        var nextIds = new Dictionary<string, int>(StringComparer.Ordinal);
        MTS.MetaTransformScriptModelAccess.VisitCollections(new ReadIdsVisitor(model, nextIds));
        return nextIds;
    }

    private sealed class RemapIdsVisitor(MTS.MetaTransformScriptModel model, IDictionary<string, int> nextIds)
        : MTS.IMetaTransformScriptCollectionVisitor
    {
        public void Visit<TEntity>(string entityName, Func<MTS.MetaTransformScriptModel, IList<TEntity>> collection,
            Func<TEntity, string> readId, Action<TEntity, string> writeId) where TEntity : class
        {
            foreach (var row in collection(model))
            {
                if (row is null) continue;
                nextIds.TryGetValue(entityName, out var nextId);
                nextId++;
                nextIds[entityName] = nextId;
                writeId(row, $"{entityName}:{nextId}");
            }
        }
    }

    private sealed class AppendRowsVisitor(MTS.MetaTransformScriptModel target, MTS.MetaTransformScriptModel source)
        : MTS.IMetaTransformScriptCollectionVisitor
    {
        public void Visit<TEntity>(string entityName, Func<MTS.MetaTransformScriptModel, IList<TEntity>> collection,
            Func<TEntity, string> readId, Action<TEntity, string> writeId) where TEntity : class
        {
            var targetRows = collection(target)
                ?? throw new InvalidOperationException($"MetaTransformScript model list '{entityName}List' is not appendable.");
            foreach (var row in collection(source))
            {
                if (row is not null) targetRows.Add(row);
            }
        }
    }

    private sealed class ReadIdsVisitor(MTS.MetaTransformScriptModel model, IDictionary<string, int> nextIds)
        : MTS.IMetaTransformScriptCollectionVisitor
    {
        public void Visit<TEntity>(string entityName, Func<MTS.MetaTransformScriptModel, IList<TEntity>> collection,
            Func<TEntity, string> readId, Action<TEntity, string> writeId) where TEntity : class
        {
            foreach (var row in collection(model))
            {
                if (row is null) continue;
                if (TryReadEntityId(readId(row), entityName, out var numericId) &&
                    (!nextIds.TryGetValue(entityName, out var current) || numericId > current))
                {
                    nextIds[entityName] = numericId;
                }
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
