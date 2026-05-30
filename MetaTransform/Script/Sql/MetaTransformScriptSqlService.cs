using System.Text;
using System.Reflection;
using MetaTransformScript.Instance;
using MetaTransformScript.Sql.Parsing;
using MTS = global::MetaTransformScript;

namespace MetaTransformScript.Sql;

public sealed partial class MetaTransformScriptSqlService
{
    public MetaTransformScriptSqlService()
    {
    }

    public MTS.MetaTransformScriptModel ImportFromSqlFile(string sqlFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sqlFilePath);

        var fullPath = Path.GetFullPath(sqlFilePath);
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

        return ImportFromSingleSqlFile(fullPath);
    }

    public MTS.MetaTransformScriptModel ImportFromSqlCode(string sqlCode, string? scriptName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sqlCode);
        return ImportFromSqlSources([new SqlImportSource(sqlCode, SourcePath: null, BareSelectName: scriptName)]);
    }

    public async Task<ImportToWorkspaceResult> ImportSingleSqlFileToWorkspaceAsync(
        string sqlFilePath,
        string? targetSqlIdentifier,
        string newWorkspacePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sqlFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(newWorkspacePath);
        cancellationToken.ThrowIfCancellationRequested();

        var workspaceFullPath = Path.GetFullPath(newWorkspacePath);
        EnsureTargetDirectoryIsEmpty(workspaceFullPath);
        Directory.CreateDirectory(workspaceFullPath);

        var model = ImportFromSqlFile(sqlFilePath);
        ApplySingleScriptImportTarget(model, targetSqlIdentifier, sourceLabel: Path.GetFileName(sqlFilePath));
        await MetaTransformScriptInstance.SaveToWorkspaceAsync(model, workspaceFullPath, cancellationToken).ConfigureAwait(false);

        return new ImportToWorkspaceResult(model, model.TransformScriptList.Count, workspaceFullPath);
    }

    public async Task<ImportToWorkspaceResult> ImportFromSqlCodeToWorkspaceAsync(
        string sqlCode,
        string? targetSqlIdentifier,
        string newWorkspacePath,
        string? scriptName = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sqlCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(newWorkspacePath);
        cancellationToken.ThrowIfCancellationRequested();

        var workspaceFullPath = Path.GetFullPath(newWorkspacePath);
        EnsureTargetDirectoryIsEmpty(workspaceFullPath);
        Directory.CreateDirectory(workspaceFullPath);

        var model = ImportFromSqlCode(sqlCode, scriptName);
        ApplySingleScriptImportTarget(model, targetSqlIdentifier, sourceLabel: "<sql-code>");
        await MetaTransformScriptInstance.SaveToWorkspaceAsync(model, workspaceFullPath, cancellationToken).ConfigureAwait(false);

        return new ImportToWorkspaceResult(model, model.TransformScriptList.Count, workspaceFullPath);
    }

    public async Task<ImportToWorkspaceResult> AddSqlFileToWorkspaceAsync(
        string sqlFilePath,
        string? targetSqlIdentifier,
        string workspacePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sqlFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);

        var fileFullPath = Path.GetFullPath(sqlFilePath);
        if (!File.Exists(fileFullPath))
        {
            throw new MetaTransformScriptSqlImportException(
                MetaTransformScriptSqlImportFailureKind.SourcePathNotFound,
                $"SQL file '{fileFullPath}' was not found.");
        }

        var sql = await File.ReadAllTextAsync(fileFullPath, cancellationToken).ConfigureAwait(false);
        return await AddSqlSourcesToWorkspaceAsync(
                [new SqlImportSource(sql, SourcePath: Path.GetFileName(fileFullPath), BareSelectName: null)],
                targetSqlIdentifier,
                workspacePath,
                cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<ImportToWorkspaceResult> AddSqlCodeToWorkspaceAsync(
        string sqlCode,
        string? targetSqlIdentifier,
        string workspacePath,
        string? scriptName = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sqlCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);

        return await AddSqlSourcesToWorkspaceAsync(
                [new SqlImportSource(sqlCode, SourcePath: null, BareSelectName: scriptName)],
                targetSqlIdentifier,
                workspacePath,
                cancellationToken)
            .ConfigureAwait(false);
    }

    public string ExportToSqlCode(string workspacePath, string? scriptName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        var model = MetaTransformScriptInstance.LoadFromWorkspace(Path.GetFullPath(workspacePath), searchUpward: false);
        return ExportToSqlCode(model, scriptName);
    }

    public string ExportToSqlCode(MTS.MetaTransformScriptModel model, string? scriptName = null)
    {
        ArgumentNullException.ThrowIfNull(model);
        EnsureModelIsBound(model);

        var script = ResolveSingleScript(model, scriptName);
        var emitter = new MetaTransformScriptSqlEmitter(model);
        var scriptObjectType = ResolveScriptObjectType(model, script);
        if (scriptObjectType == ScriptObjectType.StoredProcedure)
        {
            return RenderStoredProcedureInvocationSql(model, script);
        }

        if (scriptObjectType == ScriptObjectType.ScalarFunction)
        {
            return WrapInCreateScalarFunctionEnvelope(model, script, emitter);
        }

        return emitter.Render(ResolveStatement(model, script));
    }

    public IReadOnlyList<MetaTransformScriptSqlModuleDefinition> ExportModuleDefinitions(
        string workspacePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        var model = MetaTransformScriptInstance.LoadFromWorkspace(Path.GetFullPath(workspacePath), searchUpward: false);
        return ExportModuleDefinitions(model);
    }

    public IReadOnlyList<MetaTransformScriptSqlModuleDefinition> ExportModuleDefinitions(
        MTS.MetaTransformScriptModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        EnsureModelIsBound(model);

        var scripts = model.TransformScriptList.ToArray();
        if (scripts.Length == 0)
        {
            throw new InvalidOperationException("MetaTransformScript workspace does not contain any TransformScript rows.");
        }

        var emitter = new MetaTransformScriptSqlEmitter(model);
        var modules = new List<MetaTransformScriptSqlModuleDefinition>();
        for (var i = 0; i < scripts.Length; i++)
        {
            var script = scripts[i];
            var scriptObjectType = ResolveScriptObjectType(model, script);
            if (scriptObjectType == ScriptObjectType.RawStatement)
            {
                throw new InvalidOperationException(
                    $"Transform script '{script.Name}' is a raw statement and cannot be lowered to a MetaSql SQL module.");
            }

            var identity = ResolveSqlModuleIdentity(model, script);
            var createObjectName = FormatSchemaObjectName(identity.SchemaName, identity.ObjectName);
            var definitionSql = RenderScriptForExport(
                    model,
                    script,
                    emitter,
                    includeBatchSeparator: false,
                    createObjectNameOverride: createObjectName)
                .Trim();

            modules.Add(new MetaTransformScriptSqlModuleDefinition(
                TransformScriptId: script.Id,
                ScriptName: script.Name,
                ModuleKind: ToPublicModuleKind(scriptObjectType),
                SchemaName: identity.SchemaName,
                ObjectName: identity.ObjectName,
                DefinitionSql: definitionSql,
                DeployOrdinal: i + 1));
        }

        return modules;
    }

    public async Task<ExportToPathResult> ExportToSqlPathAsync(
        string workspacePath,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);
        cancellationToken.ThrowIfCancellationRequested();

        var model = await MetaTransformScriptInstance.LoadFromWorkspaceAsync(
            Path.GetFullPath(workspacePath),
            searchUpward: false,
            cancellationToken).ConfigureAwait(false);

        return await ExportToSqlPathAsync(model, outputPath, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ExportToPathResult> ExportToSqlPathAsync(
        MTS.MetaTransformScriptModel model,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);
        cancellationToken.ThrowIfCancellationRequested();
        EnsureModelIsBound(model);

        var scripts = model.TransformScriptList.ToArray();
        if (scripts.Length == 0)
        {
            throw new InvalidOperationException("MetaTransformScript workspace does not contain any TransformScript rows.");
        }

        var emitter = new MetaTransformScriptSqlEmitter(model);
        var fullOutputPath = Path.GetFullPath(outputPath);

        if (string.Equals(Path.GetExtension(fullOutputPath), ".sql", StringComparison.OrdinalIgnoreCase))
        {
            var parentDirectory = Path.GetDirectoryName(fullOutputPath);
            if (!string.IsNullOrWhiteSpace(parentDirectory))
            {
                Directory.CreateDirectory(parentDirectory);
            }

            var combinedSql = string.Join(
                Environment.NewLine,
                scripts.Select(script => RenderScriptForExport(model, script, emitter)));
            await File.WriteAllTextAsync(fullOutputPath, combinedSql, Encoding.UTF8, cancellationToken).ConfigureAwait(false);
            return new ExportToPathResult(scripts.Length, fullOutputPath);
        }

        EnsureTargetDirectoryIsEmpty(fullOutputPath);
        Directory.CreateDirectory(fullOutputPath);

        var usedFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < scripts.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var script = scripts[i];
            var sql = RenderScriptForExport(model, script, emitter);
            var relativePath = BuildUniqueOutputRelativePath(script, usedFileNames, i + 1);
            var filePath = Path.Combine(fullOutputPath, relativePath);
            var fileDirectory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

            await File.WriteAllTextAsync(filePath, sql, Encoding.UTF8, cancellationToken).ConfigureAwait(false);
        }

        return new ExportToPathResult(scripts.Length, fullOutputPath);
    }

    private MTS.MetaTransformScriptModel ImportFromSingleSqlFile(string fullPath)
    {
        var sql = File.ReadAllText(fullPath);
        var sourcePath = Path.GetFileName(fullPath);
        return ImportFromSqlSources([new SqlImportSource(sql, SourcePath: sourcePath, BareSelectName: null)]);
    }

    private async Task<ImportToWorkspaceResult> AddSqlSourcesToWorkspaceAsync(
        IEnumerable<SqlImportSource> sources,
        string? targetSqlIdentifier,
        string workspacePath,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(sources);
        cancellationToken.ThrowIfCancellationRequested();

        var workspaceFullPath = Path.GetFullPath(workspacePath);
        var model = await MetaTransformScriptInstance.LoadFromWorkspaceAsync(
            workspaceFullPath,
            searchUpward: false,
            cancellationToken).ConfigureAwait(false);

        var existingScriptIds = model.TransformScriptList
            .Select(static item => item.Id)
            .ToHashSet(StringComparer.Ordinal);

        var parser = new MetaTransformScriptSqlParser();
        var builder = new MetaTransformScriptSqlModelBuilder(model);
        MetaTransformScriptSqlParser.TopLevelStatementShape? statementShape = null;
        var parsedStatementCount = 0;

        foreach (var source in sources)
        {
            EnsureSqlTextEncodingLooksIntentional(source);

            foreach (var batch in SplitSqlBatches(source.Sql, source.SourcePath, source.BareSelectName))
            {
                if (string.IsNullOrWhiteSpace(batch.Sql) || IsIgnorableSetBatch(batch.Sql))
                {
                    continue;
                }

                if (TryGetUnsupportedAuxiliaryBatchKeyword(batch.Sql, out var auxiliaryBatchKeyword))
                {
                    var sourceLabel = string.IsNullOrWhiteSpace(batch.SourcePath) ? "<sql-code>" : batch.SourcePath;
                    throw new MetaTransformScriptSqlImportException(
                        MetaTransformScriptSqlImportFailureKind.UnsupportedSql,
                        $"SQL import failed for '{sourceLabel}'.{Environment.NewLine}  Auxiliary batch '{auxiliaryBatchKeyword}' is not supported. Only SET-only auxiliary batches are ignored.");
                }

                try
                {
                    var parsedShape = parser.ParseSqlCodeIntoBuilder(batch.Sql, builder, batch.SourcePath, batch.BareSelectName);
                    if (statementShape is null)
                    {
                        statementShape = parsedShape;
                    }
                    else if (statementShape != parsedShape)
                    {
                        var sourceLabel = string.IsNullOrWhiteSpace(batch.SourcePath) ? "<sql-code>" : batch.SourcePath;
                        throw new MetaTransformScriptSqlImportException(
                            MetaTransformScriptSqlImportFailureKind.InvalidSqlInput,
                            $"SQL input '{sourceLabel}' mixes top-level statement shapes. Split the inputs so one logical import source uses one top-level shape.");
                    }

                    parsedStatementCount++;
                }
                catch (MetaTransformScriptSqlParserException ex)
                {
                    throw CreateImportException(ex, batch.SourcePath);
                }
            }
        }

        if (parsedStatementCount == 0)
        {
            throw new MetaTransformScriptSqlImportException(
                MetaTransformScriptSqlImportFailureKind.InvalidSqlInput,
                "SQL input did not contain a supported transform statement, CREATE VIEW wrapper, CREATE FUNCTION wrapper, or CREATE PROCEDURE wrapper.");
        }

        var merged = builder.Build();
        var addedScripts = merged.TransformScriptList
            .Where(item => !existingScriptIds.Contains(item.Id))
            .ToArray();
        if (addedScripts.Length != 1)
        {
            throw new InvalidOperationException(
                $"Expected exactly one transform script from add operation, but found {addedScripts.Length}.");
        }

        ApplyImportTargetToScript(
            merged,
            addedScripts[0],
            targetSqlIdentifier,
            sourceLabel: addedScripts[0].SourcePath);
        await MetaTransformScriptInstance.SaveToWorkspaceAsync(merged, workspaceFullPath, cancellationToken).ConfigureAwait(false);
        return new ImportToWorkspaceResult(merged, merged.TransformScriptList.Count, workspaceFullPath);
    }

    private static void ApplySingleScriptImportTarget(
        MTS.MetaTransformScriptModel model,
        string? targetSqlIdentifier,
        string? sourceLabel)
    {
        ArgumentNullException.ThrowIfNull(model);
        var script = model.TransformScriptList.Count switch
        {
            1 => model.TransformScriptList[0],
            _ => throw new InvalidOperationException(
                $"Expected exactly one transform script for this import operation, but found {model.TransformScriptList.Count}.")
        };

        ApplyImportTargetToScript(model, script, targetSqlIdentifier, sourceLabel);
    }

    private static void ApplyImportTargetToScript(
        MTS.MetaTransformScriptModel model,
        MTS.TransformScript script,
        string? targetSqlIdentifier,
        string? sourceLabel)
    {
        ArgumentNullException.ThrowIfNull(model);
        var normalizedSourceLabel = string.IsNullOrWhiteSpace(sourceLabel)
            ? "<sql-input>"
            : sourceLabel;
        var scriptObjectType = ResolveScriptObjectType(model, script);
        var hasTarget = !string.IsNullOrWhiteSpace(targetSqlIdentifier);

        if (scriptObjectType == ScriptObjectType.InlineTableValuedFunction)
        {
            if (hasTarget)
            {
                throw new MetaTransformScriptSqlImportException(
                    MetaTransformScriptSqlImportFailureKind.InvalidSqlInput,
                    $"SQL import for '{normalizedSourceLabel}' does not allow --target for inline CREATE FUNCTION imports.");
            }

            EnsureScriptObjectTvf(model, script);
            return;
        }

        if (scriptObjectType == ScriptObjectType.ScalarFunction)
        {
            if (hasTarget)
            {
                throw new MetaTransformScriptSqlImportException(
                    MetaTransformScriptSqlImportFailureKind.InvalidSqlInput,
                    $"SQL import for '{normalizedSourceLabel}' does not allow --target for scalar CREATE FUNCTION imports.");
            }

            return;
        }

        if (scriptObjectType == ScriptObjectType.StoredProcedure)
        {
            if (hasTarget)
            {
                throw new MetaTransformScriptSqlImportException(
                    MetaTransformScriptSqlImportFailureKind.InvalidSqlInput,
                    $"SQL import for '{normalizedSourceLabel}' does not allow --target for CREATE PROCEDURE imports.");
            }

            return;
        }

        if (scriptObjectType == ScriptObjectType.RawStatement)
        {
            if (hasTarget)
            {
                throw new MetaTransformScriptSqlImportException(
                    MetaTransformScriptSqlImportFailureKind.InvalidSqlInput,
                    $"SQL import for '{normalizedSourceLabel}' does not allow --target for non-SELECT mutation statements.");
            }

            return;
        }

        if (!hasTarget)
        {
            if (!HasDeclaredCreateObjectName(model, script))
            {
                throw new MetaTransformScriptSqlImportException(
                    MetaTransformScriptSqlImportFailureKind.InvalidSqlInput,
                    $"SQL import for '{normalizedSourceLabel}' requires --target for bare SELECT imports.");
            }

            return;
        }

        var normalizedTarget = NormalizeTargetSqlIdentifier(targetSqlIdentifier!);
        EnsureScriptObjectView(model, script, normalizedTarget);
    }

    private static string NormalizeTargetSqlIdentifier(string targetSqlIdentifier)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(targetSqlIdentifier);

        var trimmed = targetSqlIdentifier.Trim();
        var parts = trimmed.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length is < 1 or > 3)
        {
            throw new InvalidOperationException(
                $"target SQL identifier '{targetSqlIdentifier}' uses {parts.Length} identifier parts; expected table, schema.table, or database.schema.table.");
        }

        return trimmed;
    }

    private MTS.MetaTransformScriptModel ImportFromSqlSources(IEnumerable<SqlImportSource> sources)
    {
        ArgumentNullException.ThrowIfNull(sources);

        var parser = new MetaTransformScriptSqlParser();
        var builder = new MetaTransformScriptSqlModelBuilder();
        MetaTransformScriptSqlParser.TopLevelStatementShape? statementShape = null;
        var parsedStatementCount = 0;

        foreach (var source in sources)
        {
            EnsureSqlTextEncodingLooksIntentional(source);

            foreach (var batch in SplitSqlBatches(source.Sql, source.SourcePath, source.BareSelectName))
            {
                if (string.IsNullOrWhiteSpace(batch.Sql) || IsIgnorableSetBatch(batch.Sql))
                {
                    continue;
                }

                if (TryGetUnsupportedAuxiliaryBatchKeyword(batch.Sql, out var auxiliaryBatchKeyword))
                {
                    var sourceLabel = string.IsNullOrWhiteSpace(batch.SourcePath) ? "<sql-code>" : batch.SourcePath;
                    throw new MetaTransformScriptSqlImportException(
                        MetaTransformScriptSqlImportFailureKind.UnsupportedSql,
                        $"SQL import failed for '{sourceLabel}'.{Environment.NewLine}  Auxiliary batch '{auxiliaryBatchKeyword}' is not supported. Only SET-only auxiliary batches are ignored.");
                }

                try
                {
                    var parsedShape = parser.ParseSqlCodeIntoBuilder(batch.Sql, builder, batch.SourcePath, batch.BareSelectName);
                    if (statementShape is null)
                    {
                        statementShape = parsedShape;
                    }
                    else if (statementShape != parsedShape)
                    {
                        var sourceLabel = string.IsNullOrWhiteSpace(batch.SourcePath) ? "<sql-code>" : batch.SourcePath;
                        throw new MetaTransformScriptSqlImportException(
                            MetaTransformScriptSqlImportFailureKind.InvalidSqlInput,
                            $"SQL input '{sourceLabel}' mixes top-level statement shapes. Split the inputs so one logical import source uses one top-level shape.");
                    }

                    parsedStatementCount++;
                }
                catch (MetaTransformScriptSqlParserException ex)
                {
                    throw CreateImportException(ex, batch.SourcePath);
                }
            }
        }

        if (parsedStatementCount == 0)
        {
            throw new MetaTransformScriptSqlImportException(
                MetaTransformScriptSqlImportFailureKind.InvalidSqlInput,
                "SQL input did not contain a supported transform statement, CREATE VIEW wrapper, CREATE FUNCTION wrapper, or CREATE PROCEDURE wrapper.");
        }

        var model = builder.Build();
        if (statementShape == MetaTransformScriptSqlParser.TopLevelStatementShape.BareSelect && model.TransformScriptList.Count > 1)
        {
            throw new MetaTransformScriptSqlImportException(
                MetaTransformScriptSqlImportFailureKind.InvalidSqlInput,
                "SQL input contains multiple bare SELECT statements. Wrap them in CREATE VIEW or split them into separate files.");
        }

        return model;
    }

    private static void EnsureModelIsBound(MTS.MetaTransformScriptModel model)
    {
        var modelFactoryType = typeof(MTS.MetaTransformScriptModel).Assembly.GetType("MetaTransformScript.MetaTransformScriptModelFactory");
        var bindMethod = modelFactoryType?.GetMethod("Bind", BindingFlags.Static | BindingFlags.NonPublic);
        bindMethod?.Invoke(null, [model]);
    }

    private static MetaTransformScriptSqlImportException CreateImportException(
        MetaTransformScriptSqlParserException exception,
        string? sourcePath)
    {
        var kind = exception.FailureKind switch
        {
            MetaTransformScriptSqlParserFailureKind.ParseError => MetaTransformScriptSqlImportFailureKind.ParseFailed,
            MetaTransformScriptSqlParserFailureKind.UnsupportedSyntax => MetaTransformScriptSqlImportFailureKind.UnsupportedSql,
            MetaTransformScriptSqlParserFailureKind.UnsupportedFunctionWrapper => MetaTransformScriptSqlImportFailureKind.UnsupportedFunctionWrapper,
            _ => MetaTransformScriptSqlImportFailureKind.InvalidSqlInput
        };

        return new MetaTransformScriptSqlImportException(
            kind,
            $"SQL import failed for '{(string.IsNullOrWhiteSpace(sourcePath) ? "<sql-code>" : sourcePath)}'.{Environment.NewLine}  {exception.Message}",
            exception,
            exception.Line,
            exception.Column,
            exception.Offset);
    }

    private static IReadOnlyList<SqlImportBatch> SplitSqlBatches(string sql, string? sourcePath, string? bareSelectName)
    {
        var batches = new List<SqlImportBatch>();
        var builder = new StringBuilder();
        var reader = new StringReader(sql);
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            if (string.Equals(line.Trim(), "GO", StringComparison.OrdinalIgnoreCase))
            {
                batches.Add(new SqlImportBatch(builder.ToString(), sourcePath, bareSelectName));
                builder.Clear();
                continue;
            }

            builder.AppendLine(line);
        }

        batches.Add(new SqlImportBatch(builder.ToString(), sourcePath, bareSelectName));
        return batches;
    }

    private static bool IsIgnorableSetBatch(string sql)
    {
        IReadOnlyList<MetaTransformScriptSqlToken> tokens;
        try
        {
            tokens = new MetaTransformScriptSqlLexer(sql).Tokenize();
        }
        catch (MetaTransformScriptSqlParserException)
        {
            return false;
        }

        var position = 0;
        var sawSetStatement = false;
        while (position < tokens.Count)
        {
            while (position < tokens.Count && tokens[position].Kind == MetaTransformScriptSqlTokenKind.Semicolon)
            {
                position++;
            }

            if (position >= tokens.Count || tokens[position].Kind == MetaTransformScriptSqlTokenKind.EndOfFile)
            {
                return sawSetStatement;
            }

            if (!IsUnquotedKeyword(tokens[position], "SET"))
            {
                return false;
            }

            sawSetStatement = true;
            position++;
            var sawPayloadToken = false;
            while (position < tokens.Count
                && tokens[position].Kind != MetaTransformScriptSqlTokenKind.Semicolon
                && tokens[position].Kind != MetaTransformScriptSqlTokenKind.EndOfFile)
            {
                sawPayloadToken = true;
                position++;
            }

            if (!sawPayloadToken)
            {
                return false;
            }
        }

        return sawSetStatement;
    }

    private static bool TryGetUnsupportedAuxiliaryBatchKeyword(string sql, out string keyword)
    {
        keyword = string.Empty;

        IReadOnlyList<MetaTransformScriptSqlToken> tokens;
        try
        {
            tokens = new MetaTransformScriptSqlLexer(sql).Tokenize();
        }
        catch (MetaTransformScriptSqlParserException)
        {
            return false;
        }

        var firstToken = tokens.FirstOrDefault(static token =>
            token.Kind != MetaTransformScriptSqlTokenKind.Semicolon &&
            token.Kind != MetaTransformScriptSqlTokenKind.EndOfFile);

        if (firstToken.Kind != MetaTransformScriptSqlTokenKind.Identifier)
        {
            return false;
        }

        if (IsUnquotedKeyword(firstToken, "CREATE") ||
            IsUnquotedKeyword(firstToken, "SELECT") ||
            IsUnquotedKeyword(firstToken, "WITH") ||
            IsUnquotedKeyword(firstToken, "INSERT") ||
            IsUnquotedKeyword(firstToken, "UPDATE") ||
            IsUnquotedKeyword(firstToken, "DELETE") ||
            IsUnquotedKeyword(firstToken, "TRUNCATE") ||
            IsUnquotedKeyword(firstToken, "MERGE") ||
            IsUnquotedKeyword(firstToken, "SET"))
        {
            return false;
        }

        keyword = firstToken.Value.ToUpperInvariant();
        return true;
    }

    private static bool IsUnquotedKeyword(MetaTransformScriptSqlToken token, string keyword) =>
        token.Kind == MetaTransformScriptSqlTokenKind.Identifier
        && string.Equals(token.QuoteType, "NotQuoted", StringComparison.Ordinal)
        && string.Equals(token.Value, keyword, StringComparison.OrdinalIgnoreCase);

    private static readonly string[] LikelyMojibakeMarkers =
    [
        "\u00c3\u00a5",
        "\u00c3\u00a4",
        "\u00c3\u00b6",
        "\u00c3\u0085",
        "\u00c3\u0084",
        "\u00c3\u0096",
        "\u00c3\u2026",
        "\u00c3\u201e",
        "\u00c3\u2013",
        "\u00c3\u00a9",
        "\u00c3\u00a8",
        "\u00c3\u00bc",
        "\u00c3\u00b8",
        "\u00c3\u00a6",
        "\u00c3\u00a1",
        "\u00c3\u00b3",
        "\u00c3\u00b1",
        "\u00c2\u00a0",
        "\u00e2\u20ac\u201c",
        "\u00e2\u20ac\u201d",
        "\u00e2\u20ac\u02dc",
        "\u00e2\u20ac\u2122",
        "\u00e2\u20ac\u0153",
        "\u00e2\u20ac\ufffd",
        "\u00e2\u20ac\u00a6",
        "\u00e2\u20ac\u00a2",
        "\u00ef\u00bf\u00bd",
        "\ufffd"
    ];

    private static void EnsureSqlTextEncodingLooksIntentional(SqlImportSource source)
    {
        if (!TryFindLikelyTextEncodingMismatch(source.Sql, out var marker, out var line, out var column, out var offset))
        {
            return;
        }

        var sourceLabel = string.IsNullOrWhiteSpace(source.SourcePath) ? "<sql-code>" : source.SourcePath;
        throw new MetaTransformScriptSqlImportException(
            MetaTransformScriptSqlImportFailureKind.LikelyTextEncodingMismatch,
            $"SQL import failed for '{sourceLabel}'.{Environment.NewLine}  Input text contains likely mojibake or replacement characters near marker {FormatSuspiciousMarker(marker)} (line {line}, column {column}). Re-export or split the SQL as Unicode text before importing.",
            line: line,
            column: column,
            offset: offset);
    }

    private static bool TryFindLikelyTextEncodingMismatch(
        string sql,
        out string marker,
        out int line,
        out int column,
        out int offset)
    {
        marker = string.Empty;
        line = 0;
        column = 0;
        offset = -1;

        var bestOffset = -1;
        var bestMarker = string.Empty;
        foreach (var candidate in LikelyMojibakeMarkers)
        {
            var candidateOffset = sql.IndexOf(candidate, StringComparison.Ordinal);
            if (candidateOffset < 0 || (bestOffset >= 0 && candidateOffset >= bestOffset))
            {
                continue;
            }

            bestOffset = candidateOffset;
            bestMarker = candidate;
        }

        for (var i = 0; i + 1 < sql.Length; i++)
        {
            if (sql[i] != '\u00c2' || !char.IsWhiteSpace(sql[i + 1]))
            {
                continue;
            }

            if (bestOffset >= 0 && i >= bestOffset)
            {
                continue;
            }

            bestOffset = i;
            bestMarker = "\u00c2 ";
        }

        if (bestOffset < 0)
        {
            return false;
        }

        marker = bestMarker;
        offset = bestOffset;
        CalculateLineColumn(sql, bestOffset, out line, out column);
        return true;
    }

    private static string FormatSuspiciousMarker(string marker) =>
        string.Join(" ", marker.Select(static character => $"U+{(int)character:X4}"));

    private static void CalculateLineColumn(string text, int offset, out int line, out int column)
    {
        line = 1;
        column = 1;
        for (var i = 0; i < offset; i++)
        {
            if (text[i] == '\r')
            {
                if (i + 1 < offset && text[i + 1] == '\n')
                {
                    i++;
                }

                line++;
                column = 1;
                continue;
            }

            if (text[i] == '\n')
            {
                line++;
                column = 1;
                continue;
            }

            column++;
        }
    }

    private sealed record SqlImportSource(string Sql, string? SourcePath, string? BareSelectName);
    private sealed record SqlImportBatch(string Sql, string? SourcePath, string? BareSelectName);

    private static bool ContainsGoBatchSeparator(string sql)
    {
        using var reader = new StringReader(sql);
        while (reader.ReadLine() is { } line)
        {
            if (string.Equals(line.Trim(), "GO", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static MTS.TransformScript ResolveSingleScript(MTS.MetaTransformScriptModel model, string? scriptName)
    {
        var scripts = model.TransformScriptList.ToArray();
        if (scripts.Length == 0)
        {
            throw new InvalidOperationException("MetaTransformScript workspace does not contain any TransformScript rows.");
        }

        if (!string.IsNullOrWhiteSpace(scriptName))
        {
            var matches = scripts.Where(script => string.Equals(script.Name, scriptName, StringComparison.OrdinalIgnoreCase)).ToArray();
            return matches.Length switch
            {
                0 => throw new InvalidOperationException($"Transform script '{scriptName}' was not found."),
                > 1 => throw new InvalidOperationException($"Transform script name '{scriptName}' is ambiguous."),
                _ => matches[0]
            };
        }

        if (scripts.Length != 1)
        {
            throw new InvalidOperationException(
                $"Workspace contains {scripts.Length} transform scripts. Use --name to select which one to emit as SQL code.");
        }

        return scripts[0];
    }

    private static MTS.TSqlStatement ResolveStatement(MTS.MetaTransformScriptModel model, MTS.TransformScript script)
    {
        var link = model.TransformScriptStatementLinkList.SingleOrDefault(item => string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal))
            ?? throw new InvalidOperationException($"Transform script '{script.Name}' is missing its TSqlStatement link.");
        return model.TSqlStatementList.SingleOrDefault(item => string.Equals(item.Id, link.TSqlStatement.Id, StringComparison.Ordinal))
            ?? throw new InvalidOperationException($"Transform script '{script.Name}' points to a missing TSqlStatement '{link.TSqlStatement.Id}'.");
    }

    private static MTS.SelectStatement ResolveSelectStatement(MTS.MetaTransformScriptModel model, MTS.TransformScript script)
    {
        var statement = ResolveStatement(model, script);
        return TryResolveSelectStatement(model, statement)
            ?? throw new InvalidOperationException($"Transform script '{script.Name}' does not point to a SelectStatement.");
    }

    private static MTS.SelectStatement? TryResolveSelectStatement(MTS.MetaTransformScriptModel model, MTS.TSqlStatement statement)
    {
        var statementWithCtes = model.StatementWithCtesAndXmlNamespacesList.SingleOrDefault(item =>
            string.Equals(item.TSqlStatement.Id, statement.Id, StringComparison.Ordinal));
        if (statementWithCtes is null)
        {
            return null;
        }

        return model.SelectStatementList.SingleOrDefault(item =>
            string.Equals(item.StatementWithCtesAndXmlNamespaces.Id, statementWithCtes.Id, StringComparison.Ordinal));
    }

    private static string RenderScriptForExport(
        MTS.MetaTransformScriptModel model,
        MTS.TransformScript script,
        MetaTransformScriptSqlEmitter emitter,
        bool includeBatchSeparator = true,
        string? createObjectNameOverride = null)
    {
        var scriptObjectType = ResolveScriptObjectType(model, script);
        if (scriptObjectType == ScriptObjectType.ScalarFunction)
        {
            return WrapInCreateScalarFunctionEnvelope(
                model,
                script,
                emitter,
                includeBatchSeparator,
                createObjectNameOverride);
        }

        if (scriptObjectType == ScriptObjectType.StoredProcedure)
        {
            var storedProcedure = TryGetScriptObjectStoredProcedure(model, script.Id)
                ?? throw new InvalidOperationException($"Transform script '{script.Name}' is missing its stored procedure object row.");
            var builder = new StringBuilder();
            builder.AppendLine(storedProcedure.DefinitionSql.Trim());
            AppendBatchSeparator(builder, includeBatchSeparator);
            return builder.ToString();
        }

        var statement = ResolveStatement(model, script);
        var body = emitter.Render(statement);
        return scriptObjectType switch
        {
            ScriptObjectType.View => WrapInCreateEnvelope(model, script, body, includeBatchSeparator, createObjectNameOverride),
            ScriptObjectType.InlineTableValuedFunction => WrapInCreateEnvelope(model, script, body, includeBatchSeparator, createObjectNameOverride),
            ScriptObjectType.RawStatement => body,
            _ => throw new InvalidOperationException($"Unsupported script object type for transform script '{script.Name}'.")
        };
    }

    private static string RenderStoredProcedureInvocationSql(
        MTS.MetaTransformScriptModel model,
        MTS.TransformScript script)
    {
        return $"EXEC {ResolveCreateObjectName(model, script)};";
    }

    private static void EnsureTargetDirectoryIsEmpty(string targetDirectoryPath)
    {
        if (File.Exists(targetDirectoryPath))
        {
            throw new InvalidOperationException($"target path '{targetDirectoryPath}' must be a directory path.");
        }

        if (Directory.Exists(targetDirectoryPath) && Directory.EnumerateFileSystemEntries(targetDirectoryPath).Any())
        {
            throw new InvalidOperationException($"target directory '{targetDirectoryPath}' must be empty.");
        }
    }

    private static string WrapInCreateEnvelope(
        MTS.MetaTransformScriptModel model,
        MTS.TransformScript script,
        string bodySql,
        bool includeBatchSeparator = true,
        string? createObjectNameOverride = null)
    {
        var scriptObjectType = ResolveScriptObjectType(model, script);
        return scriptObjectType switch
        {
            ScriptObjectType.View => WrapInCreateViewEnvelope(model, script, bodySql, includeBatchSeparator, createObjectNameOverride),
            ScriptObjectType.InlineTableValuedFunction => WrapInCreateInlineTableValuedFunctionEnvelope(model, script, bodySql, includeBatchSeparator, createObjectNameOverride),
            ScriptObjectType.ScalarFunction => WrapInCreateScalarFunctionEnvelope(model, script, new MetaTransformScriptSqlEmitter(model), includeBatchSeparator, createObjectNameOverride),
            _ => throw new InvalidOperationException($"Unsupported script object type '{scriptObjectType}'.")
        };
    }

    private static string WrapInCreateViewEnvelope(
        MTS.MetaTransformScriptModel model,
        MTS.TransformScript script,
        string bodySql,
        bool includeBatchSeparator = true,
        string? createObjectNameOverride = null)
    {
        var trimmedBody = bodySql.Trim();
        var createObjectName = createObjectNameOverride ?? ResolveCreateObjectName(model, script);
        var columnList = RenderViewColumnList(model, script);

        var builder = new StringBuilder();
        builder.Append("CREATE VIEW ");
        builder.AppendLine(createObjectName);
        if (!string.IsNullOrWhiteSpace(columnList))
        {
            builder.AppendLine(columnList);
        }

        builder.AppendLine("AS");
        builder.AppendLine(trimmedBody);
        AppendBatchSeparator(builder, includeBatchSeparator);
        return builder.ToString();
    }

    private static string WrapInCreateInlineTableValuedFunctionEnvelope(
        MTS.MetaTransformScriptModel model,
        MTS.TransformScript script,
        string bodySql,
        bool includeBatchSeparator = true,
        string? createObjectNameOverride = null)
    {
        var createObjectName = createObjectNameOverride ?? ResolveCreateObjectName(model, script);
        var parameterList = RenderFunctionParameterList(model, script);
        var trimmedBody = bodySql.Trim();

        var builder = new StringBuilder();
        builder.Append("CREATE FUNCTION ");
        builder.AppendLine(createObjectName);
        builder.AppendLine(parameterList);
        builder.AppendLine("RETURNS TABLE");
        builder.AppendLine("AS");
        builder.AppendLine("RETURN");
        builder.AppendLine("(");
        foreach (var line in SplitLines(trimmedBody))
        {
            builder.Append("    ");
            builder.AppendLine(line);
        }

        builder.AppendLine(")");
        AppendBatchSeparator(builder, includeBatchSeparator);
        return builder.ToString();
    }

    private static string WrapInCreateScalarFunctionEnvelope(
        MTS.MetaTransformScriptModel model,
        MTS.TransformScript script,
        MetaTransformScriptSqlEmitter emitter,
        bool includeBatchSeparator = true,
        string? createObjectNameOverride = null)
    {
        var scalarFunction = TryGetScriptObjectScalarFunction(model, script.Id)
            ?? throw new InvalidOperationException($"Transform script '{script.Name}' is missing its scalar function object row.");
        var createObjectName = createObjectNameOverride ?? ResolveCreateObjectName(model, script);
        var parameterList = RenderFunctionParameterList(model, script);
        var returnDataType = RenderDataTypeReference(model, ResolveDataTypeReference(model, scalarFunction.DataTypeReference.Id));
        var returnExpression = emitter.RenderScalarExpressionForScriptObject(scalarFunction.ScalarExpression);

        var builder = new StringBuilder();
        builder.Append("CREATE FUNCTION ");
        builder.AppendLine(createObjectName);
        builder.AppendLine(parameterList);
        builder.Append("RETURNS ");
        builder.AppendLine(returnDataType);
        builder.AppendLine("AS");
        builder.AppendLine("BEGIN");
        foreach (var line in SplitLines("RETURN " + returnExpression.Trim()))
        {
            builder.Append("    ");
            builder.AppendLine(line);
        }

        builder.AppendLine("END");
        AppendBatchSeparator(builder, includeBatchSeparator);
        return builder.ToString();
    }

    private static void AppendBatchSeparator(StringBuilder builder, bool includeBatchSeparator)
    {
        if (includeBatchSeparator)
        {
            builder.AppendLine("GO");
        }
    }

    private static string BuildUniqueOutputRelativePath(MTS.TransformScript script, ISet<string> usedRelativePaths, int index)
    {
        var preferredName = string.IsNullOrWhiteSpace(script.SourcePath)
            ? script.Name
            : Path.GetFileName(script.SourcePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar));
        var baseName = SanitizeFileName(Path.GetFileNameWithoutExtension(preferredName));
        if (string.IsNullOrWhiteSpace(baseName))
        {
            baseName = $"Script{index}";
        }

        var candidate = baseName + ".sql";
        var suffix = 2;
        while (!usedRelativePaths.Add(candidate))
        {
            candidate = $"{baseName}_{suffix}.sql";
            suffix++;
        }

        return candidate;
    }

    private static string SanitizeFileName(string value)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var builder = new StringBuilder(value.Length);
        foreach (var ch in value)
        {
            builder.Append(invalidChars.Contains(ch) ? '_' : ch);
        }

        return builder.ToString().Trim().TrimEnd('.');
    }

    private static bool IsPlainIdentifier(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (!(char.IsLetter(value[0]) || value[0] == '_' || value[0] == '@' || value[0] == '#'))
        {
            return false;
        }

        for (var i = 1; i < value.Length; i++)
        {
            var ch = value[i];
            if (!(char.IsLetterOrDigit(ch) || ch == '_' || ch == '@' || ch == '#' || ch == '$'))
            {
                return false;
            }
        }

        return true;
    }

    private static string ResolveCreateObjectName(MTS.MetaTransformScriptModel model, MTS.TransformScript script)
    {
        var scriptObjectView = TryGetScriptObjectView(model, script.Id);
        if (!string.IsNullOrWhiteSpace(scriptObjectView?.TargetSqlIdentifier))
        {
            return scriptObjectView!.TargetSqlIdentifier;
        }

        var objectIdentifier = ResolveOptionalIdentifier(
            model,
            model.TransformScriptObjectIdentifierLinkList.SingleOrDefault(item => string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal)));
        if (objectIdentifier is null)
        {
            if (string.IsNullOrWhiteSpace(script.Name))
            {
                throw new InvalidOperationException("Transform script is missing its CREATE VIEW name.");
            }

            return script.Name;
        }

        var schemaIdentifier = ResolveOptionalIdentifier(
            model,
            model.TransformScriptSchemaIdentifierLinkList.SingleOrDefault(item => string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal)));

        return schemaIdentifier is null
            ? RenderIdentifierFromModel(objectIdentifier)
            : $"{RenderIdentifierFromModel(schemaIdentifier)}.{RenderIdentifierFromModel(objectIdentifier)}";
    }

    private static SqlModuleIdentity ResolveSqlModuleIdentity(
        MTS.MetaTransformScriptModel model,
        MTS.TransformScript script)
    {
        var objectIdentifier = ResolveOptionalIdentifier(
            model,
            model.TransformScriptObjectIdentifierLinkList.SingleOrDefault(item => string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal)));
        if (objectIdentifier is not null)
        {
            var schemaIdentifier = ResolveOptionalIdentifier(
                model,
                model.TransformScriptSchemaIdentifierLinkList.SingleOrDefault(item => string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal)));
            if (schemaIdentifier is null)
            {
                throw new InvalidOperationException(
                    $"Transform script '{script.Name}' is missing its schema identifier; SQL module declarations must be schema-qualified.");
            }

            return new SqlModuleIdentity(
                RequireIdentifierValue(schemaIdentifier, script.Name, "schema"),
                RequireIdentifierValue(objectIdentifier, script.Name, "object"));
        }

        if (!string.IsNullOrWhiteSpace(script.Name))
        {
            return ParseSqlModuleIdentity(script.Name, script.Name);
        }

        throw new InvalidOperationException(
            $"Transform script '{script.Id}' does not declare a SQL module object name and cannot be lowered to MetaSql.");
    }

    private static SqlModuleIdentity ParseSqlModuleIdentity(
        string value,
        string scriptName)
    {
        var parts = ParseSqlIdentifierParts(value);
        if (parts.Count != 2)
        {
            throw new InvalidOperationException(
                $"Transform script '{scriptName}' has SQL module identifier '{value}'; MetaSql lowering requires schema.object module names.");
        }

        return new SqlModuleIdentity(parts[0], parts[1]);
    }

    private static IReadOnlyList<string> ParseSqlIdentifierParts(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException("SQL module identifier cannot be empty.");
        }

        var parts = new List<string>();
        var i = 0;
        while (i < value.Length)
        {
            SkipWhitespace(value, ref i);
            if (i >= value.Length)
            {
                break;
            }

            parts.Add(ParseSqlIdentifierPart(value, ref i));
            SkipWhitespace(value, ref i);
            if (i >= value.Length)
            {
                break;
            }

            if (value[i] != '.')
            {
                throw new InvalidOperationException($"SQL module identifier '{value}' contains unexpected text after identifier part.");
            }

            i++;
        }

        if (parts.Count == 0 || parts.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidOperationException($"SQL module identifier '{value}' is invalid.");
        }

        return parts;
    }

    private static string ParseSqlIdentifierPart(string value, ref int index)
    {
        if (value[index] == '[')
        {
            return ParseBracketedIdentifierPart(value, ref index);
        }

        if (value[index] == '"')
        {
            return ParseDoubleQuotedIdentifierPart(value, ref index);
        }

        var start = index;
        while (index < value.Length && value[index] != '.')
        {
            index++;
        }

        var part = value[start..index].Trim();
        if (part.Length == 0)
        {
            throw new InvalidOperationException($"SQL module identifier '{value}' contains an empty identifier part.");
        }

        return part;
    }

    private static string ParseBracketedIdentifierPart(string value, ref int index)
    {
        index++;
        var builder = new StringBuilder();
        while (index < value.Length)
        {
            var ch = value[index++];
            if (ch != ']')
            {
                builder.Append(ch);
                continue;
            }

            if (index < value.Length && value[index] == ']')
            {
                builder.Append(']');
                index++;
                continue;
            }

            return builder.ToString();
        }

        throw new InvalidOperationException($"SQL module identifier '{value}' contains an unterminated bracketed identifier.");
    }

    private static string ParseDoubleQuotedIdentifierPart(string value, ref int index)
    {
        index++;
        var builder = new StringBuilder();
        while (index < value.Length)
        {
            var ch = value[index++];
            if (ch != '"')
            {
                builder.Append(ch);
                continue;
            }

            if (index < value.Length && value[index] == '"')
            {
                builder.Append('"');
                index++;
                continue;
            }

            return builder.ToString();
        }

        throw new InvalidOperationException($"SQL module identifier '{value}' contains an unterminated quoted identifier.");
    }

    private static void SkipWhitespace(string value, ref int index)
    {
        while (index < value.Length && char.IsWhiteSpace(value[index]))
        {
            index++;
        }
    }

    private static string RequireIdentifierValue(MTS.Identifier identifier, string scriptName, string identifierRole)
    {
        if (string.IsNullOrWhiteSpace(identifier.Value))
        {
            throw new InvalidOperationException(
                $"Transform script '{scriptName}' has an empty {identifierRole} identifier and cannot be lowered to MetaSql.");
        }

        return identifier.Value.Trim();
    }

    private static string FormatSchemaObjectName(string schemaName, string objectName)
    {
        return $"{RenderSqlIdentifierValue(schemaName)}.{RenderSqlIdentifierValue(objectName)}";
    }

    private static string RenderSqlIdentifierValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "[]";
        }

        var trimmed = value.Trim();
        return IsPlainIdentifier(trimmed)
            ? trimmed
            : "[" + trimmed.Replace("]", "]]", StringComparison.Ordinal) + "]";
    }

    private static string RenderFunctionParameterList(MTS.MetaTransformScriptModel model, MTS.TransformScript script)
    {
        var parameters = model.TransformScriptFunctionParametersItemList
            .Where(item => string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .ToArray();

        if (parameters.Length == 0)
        {
            return "()";
        }

        var builder = new StringBuilder();
        builder.AppendLine("(");
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            var parameterName = RenderIdentifierFromModel(ResolveIdentifier(model, parameter.Identifier.Id));
            var dataType = RenderDataTypeReference(model, ResolveDataTypeReference(model, parameter.DataTypeReference.Id));
            builder.Append("    ");
            builder.Append(parameterName);
            builder.Append(' ');
            builder.Append(dataType);
            if (i < parameters.Length - 1)
            {
                builder.Append(',');
            }

            builder.AppendLine();
        }

        builder.Append(')');
        return builder.ToString();
    }

    private static string RenderViewColumnList(MTS.MetaTransformScriptModel model, MTS.TransformScript script)
    {
        var columns = model.TransformScriptViewColumnsItemList
            .Where(item => string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => ResolveIdentifier(model, item.Identifier.Id))
            .Select(RenderIdentifierFromModel)
            .ToArray();

        if (columns.Length == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        builder.AppendLine("(");
        for (var i = 0; i < columns.Length; i++)
        {
            builder.Append("    ");
            builder.Append(columns[i]);
            if (i < columns.Length - 1)
            {
                builder.Append(',');
            }

            builder.AppendLine();
        }

        builder.Append(')');
        return builder.ToString();
    }

    private static MTS.DataTypeReference ResolveDataTypeReference(MTS.MetaTransformScriptModel model, string dataTypeReferenceId)
    {
        return model.DataTypeReferenceList.SingleOrDefault(item => string.Equals(item.Id, dataTypeReferenceId, StringComparison.Ordinal))
            ?? throw new InvalidOperationException($"DataTypeReference '{dataTypeReferenceId}' was not found.");
    }

    private static string RenderDataTypeReference(MTS.MetaTransformScriptModel model, MTS.DataTypeReference dataTypeReference)
    {
        string renderedName;

        var nameLink = model.DataTypeReferenceNameLinkList
            .SingleOrDefault(item => string.Equals(item.DataTypeReference.Id, dataTypeReference.Id, StringComparison.Ordinal));
        if (nameLink is not null)
        {
            renderedName = RenderSchemaObjectName(model, nameLink.SchemaObjectName.Id);
        }
        else
        {
            var parameterizedDataTypeReference = model.ParameterizedDataTypeReferenceList
                .SingleOrDefault(item => string.Equals(item.DataTypeReference.Id, dataTypeReference.Id, StringComparison.Ordinal));
            if (parameterizedDataTypeReference is null)
            {
                throw new InvalidOperationException($"Unsupported DataTypeReference '{dataTypeReference.Id}'.");
            }

            var sqlDataTypeReference = model.SqlDataTypeReferenceList
                .SingleOrDefault(item => string.Equals(item.ParameterizedDataTypeReference.Id, parameterizedDataTypeReference.Id, StringComparison.Ordinal))
                ?? throw new InvalidOperationException($"SqlDataTypeReference for '{parameterizedDataTypeReference.Id}' was not found.");
            renderedName = MetaTransformScriptSqlServerDataTypes.RenderSqlName(sqlDataTypeReference.SqlDataTypeOption);

            var parameters = model.ParameterizedDataTypeReferenceParametersItemList
                .Where(item => string.Equals(item.ParameterizedDataTypeReference.Id, parameterizedDataTypeReference.Id, StringComparison.Ordinal))
                .OrderBy(item => ParseOrdinal(item.Ordinal))
                .Select(item => RenderLiteral(model, item.Literal.Id))
                .ToArray();

            return parameters.Length == 0
                ? renderedName
                : renderedName + "(" + string.Join(", ", parameters) + ")";
        }

        var parameterizedByName = model.ParameterizedDataTypeReferenceList
            .SingleOrDefault(item => string.Equals(item.DataTypeReference.Id, dataTypeReference.Id, StringComparison.Ordinal));
        if (parameterizedByName is null)
        {
            return renderedName;
        }

        var renderedParameters = model.ParameterizedDataTypeReferenceParametersItemList
            .Where(item => string.Equals(item.ParameterizedDataTypeReference.Id, parameterizedByName.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => RenderLiteral(model, item.Literal.Id))
            .ToArray();
        return renderedParameters.Length == 0
            ? renderedName
            : renderedName + "(" + string.Join(", ", renderedParameters) + ")";
    }

    private static string RenderSchemaObjectName(MTS.MetaTransformScriptModel model, string schemaObjectNameId)
    {
        var schemaObjectName = model.SchemaObjectNameList.SingleOrDefault(item => string.Equals(item.Id, schemaObjectNameId, StringComparison.Ordinal))
            ?? throw new InvalidOperationException($"SchemaObjectName '{schemaObjectNameId}' was not found.");

        var parts = model.MultiPartIdentifierIdentifiersItemList
            .Where(item => string.Equals(item.MultiPartIdentifier.Id, schemaObjectName.MultiPartIdentifier.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => RenderIdentifierFromModel(ResolveIdentifier(model, item.Identifier.Id)))
            .ToArray();

        if (parts.Length == 0)
        {
            throw new InvalidOperationException($"SchemaObjectName '{schemaObjectNameId}' had no identifier parts.");
        }

        return string.Join(".", parts);
    }

    private static string RenderLiteral(MTS.MetaTransformScriptModel model, string literalId)
    {
        var literal = model.LiteralList.SingleOrDefault(item => string.Equals(item.Id, literalId, StringComparison.Ordinal))
            ?? throw new InvalidOperationException($"Literal '{literalId}' was not found.");

        if (model.MaxLiteralList.Any(item => string.Equals(item.Literal.Id, literal.Id, StringComparison.Ordinal)))
        {
            return "max";
        }

        return literal.Value;
    }

    private static IEnumerable<string> SplitLines(string text)
    {
        using var reader = new StringReader(text);
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            yield return line;
        }
    }

    private static MTS.Identifier? ResolveOptionalIdentifier(MTS.MetaTransformScriptModel model, object? link)
    {
        if (link is null)
        {
            return null;
        }

        if (link.GetType().GetProperty("Identifier")?.GetValue(link) is MTS.Identifier identifier)
        {
            return identifier;
        }

        var identifierId = (string?)link.GetType().GetProperty("IdentifierId")?.GetValue(link);
        return string.IsNullOrWhiteSpace(identifierId) ? null : ResolveIdentifier(model, identifierId);
    }

    private static MTS.Identifier ResolveIdentifier(MTS.MetaTransformScriptModel model, string valueId)
    {
        return model.IdentifierList.SingleOrDefault(item => string.Equals(item.Id, valueId, StringComparison.Ordinal))
            ?? throw new InvalidOperationException($"Identifier '{valueId}' was not found.");
    }

    private static string RenderIdentifierFromModel(MTS.Identifier identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier.Value))
        {
            return "[]";
        }

        return string.Equals(identifier.QuoteType, "SquareBracket", StringComparison.Ordinal)
            ? "[" + identifier.Value.Replace("]", "]]", StringComparison.Ordinal) + "]"
            : !string.IsNullOrWhiteSpace(identifier.QuoteType) && string.Equals(identifier.QuoteType, "Backtick", StringComparison.Ordinal)
                ? "[" + identifier.Value.Replace("]", "]]", StringComparison.Ordinal) + "]"
            : !string.IsNullOrWhiteSpace(identifier.QuoteType) && string.Equals(identifier.QuoteType, "DoubleQuote", StringComparison.Ordinal)
                ? "\"" + identifier.Value.Replace("\"", "\"\"", StringComparison.Ordinal) + "\""
                : IsPlainIdentifier(identifier.Value)
                    ? identifier.Value
                    : "[" + identifier.Value.Replace("]", "]]", StringComparison.Ordinal) + "]";
    }

    private static int ParseOrdinal(string ordinal) =>
        int.TryParse(ordinal, out var value) ? value : 0;

    private static MetaTransformScriptSqlModuleKind ToPublicModuleKind(ScriptObjectType scriptObjectType)
    {
        return scriptObjectType switch
        {
            ScriptObjectType.View => MetaTransformScriptSqlModuleKind.View,
            ScriptObjectType.InlineTableValuedFunction => MetaTransformScriptSqlModuleKind.InlineTableValuedFunction,
            ScriptObjectType.ScalarFunction => MetaTransformScriptSqlModuleKind.ScalarFunction,
            ScriptObjectType.StoredProcedure => MetaTransformScriptSqlModuleKind.StoredProcedure,
            _ => throw new InvalidOperationException($"Script object type '{scriptObjectType}' is not a SQL module.")
        };
    }

    private static ScriptObjectType ResolveScriptObjectType(MTS.MetaTransformScriptModel model, MTS.TransformScript script)
    {
        var hasView = model.ScriptObjectViewList.Any(item => string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal));
        var hasTvf = model.ScriptObjectTVFList.Any(item => string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal));
        var hasScalarFunction = model.ScriptObjectScalarFunctionList.Any(item => string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal));
        var hasStoredProcedure = model.ScriptObjectStoredProcedureList.Any(item => string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal));

        if ((hasView ? 1 : 0) + (hasTvf ? 1 : 0) + (hasScalarFunction ? 1 : 0) + (hasStoredProcedure ? 1 : 0) > 1)
        {
            throw new InvalidOperationException(
                $"Transform script '{script.Name}' has more than one script object row. Exactly one script object type is allowed.");
        }

        if (hasTvf)
        {
            return ScriptObjectType.InlineTableValuedFunction;
        }

        if (hasScalarFunction)
        {
            return ScriptObjectType.ScalarFunction;
        }

        if (hasStoredProcedure)
        {
            return ScriptObjectType.StoredProcedure;
        }

        if (hasView)
        {
            return ScriptObjectType.View;
        }

        if (model.TransformScriptFunctionParametersItemList.Any(item => string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal)))
        {
            return ScriptObjectType.InlineTableValuedFunction;
        }

        var statement = ResolveStatement(model, script);
        return TryResolveSelectStatement(model, statement) is null
            ? ScriptObjectType.RawStatement
            : ScriptObjectType.View;
    }

    private static MTS.ScriptObjectView? TryGetScriptObjectView(MTS.MetaTransformScriptModel model, string transformScriptId)
    {
        return model.ScriptObjectViewList.SingleOrDefault(item => string.Equals(item.TransformScript.Id, transformScriptId, StringComparison.Ordinal));
    }

    private static MTS.ScriptObjectTVF? TryGetScriptObjectTvf(MTS.MetaTransformScriptModel model, string transformScriptId)
    {
        return model.ScriptObjectTVFList.SingleOrDefault(item => string.Equals(item.TransformScript.Id, transformScriptId, StringComparison.Ordinal));
    }

    private static MTS.ScriptObjectScalarFunction? TryGetScriptObjectScalarFunction(MTS.MetaTransformScriptModel model, string transformScriptId)
    {
        return model.ScriptObjectScalarFunctionList.SingleOrDefault(item => string.Equals(item.TransformScript.Id, transformScriptId, StringComparison.Ordinal));
    }

    private static MTS.ScriptObjectStoredProcedure? TryGetScriptObjectStoredProcedure(MTS.MetaTransformScriptModel model, string transformScriptId)
    {
        return model.ScriptObjectStoredProcedureList.SingleOrDefault(item => string.Equals(item.TransformScript.Id, transformScriptId, StringComparison.Ordinal));
    }

    private static bool HasDeclaredCreateObjectName(MTS.MetaTransformScriptModel model, MTS.TransformScript script)
    {
        return model.TransformScriptObjectIdentifierLinkList.Any(item =>
            string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal));
    }

    private static void EnsureScriptObjectView(MTS.MetaTransformScriptModel model, MTS.TransformScript script, string targetSqlIdentifier)
    {
        model.ScriptObjectTVFList.RemoveAll(item => string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal));
        model.ScriptObjectScalarFunctionList.RemoveAll(item => string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal));
        model.ScriptObjectStoredProcedureList.RemoveAll(item => string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal));

        var scriptObjectView = TryGetScriptObjectView(model, script.Id);
        if (scriptObjectView is null)
        {
            model.ScriptObjectViewList.Add(new MTS.ScriptObjectView
            {
                Id = Guid.NewGuid().ToString("N"),
                TransformScript = script,
                TargetSqlIdentifier = targetSqlIdentifier
            });
            return;
        }

        scriptObjectView.TargetSqlIdentifier = targetSqlIdentifier;
    }

    private static void EnsureScriptObjectTvf(MTS.MetaTransformScriptModel model, MTS.TransformScript script)
    {
        model.ScriptObjectViewList.RemoveAll(item => string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal));
        model.ScriptObjectScalarFunctionList.RemoveAll(item => string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal));
        model.ScriptObjectStoredProcedureList.RemoveAll(item => string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal));
        if (TryGetScriptObjectTvf(model, script.Id) is not null)
        {
            return;
        }

        model.ScriptObjectTVFList.Add(new MTS.ScriptObjectTVF
        {
            Id = Guid.NewGuid().ToString("N"),
            TransformScript = script
        });
    }
}

internal enum ScriptObjectType
{
    View,
    InlineTableValuedFunction,
    ScalarFunction,
    StoredProcedure,
    RawStatement
}

public enum MetaTransformScriptSqlModuleKind
{
    View,
    InlineTableValuedFunction,
    ScalarFunction,
    StoredProcedure
}

public sealed record MetaTransformScriptSqlModuleDefinition(
    string TransformScriptId,
    string ScriptName,
    MetaTransformScriptSqlModuleKind ModuleKind,
    string SchemaName,
    string ObjectName,
    string DefinitionSql,
    int DeployOrdinal);

public sealed record ImportToWorkspaceResult(
    MTS.MetaTransformScriptModel Model,
    int ScriptCount,
    string WorkspacePath);

public sealed record ExportToPathResult(
    int ScriptCount,
    string OutputPath);

internal sealed record SqlModuleIdentity(
    string SchemaName,
    string ObjectName);
