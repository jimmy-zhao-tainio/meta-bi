using System.Text;
using System.Reflection;
using MetaTransformScript.Instance;
using MetaTransformScript.Sql.Parsing;
using MTS = global::MetaTransformScript;

namespace MetaTransformScript.Sql;

public sealed class MetaTransformScriptSqlService
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
        string targetSqlIdentifier,
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
        ApplySingleScriptTarget(model, targetSqlIdentifier);
        await MetaTransformScriptInstance.SaveToWorkspaceAsync(model, workspaceFullPath, cancellationToken).ConfigureAwait(false);

        return new ImportToWorkspaceResult(model, model.TransformScriptList.Count, workspaceFullPath);
    }

    public async Task<ImportToWorkspaceResult> ImportFromSqlCodeToWorkspaceAsync(
        string sqlCode,
        string targetSqlIdentifier,
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
        ApplySingleScriptTarget(model, targetSqlIdentifier);
        await MetaTransformScriptInstance.SaveToWorkspaceAsync(model, workspaceFullPath, cancellationToken).ConfigureAwait(false);

        return new ImportToWorkspaceResult(model, model.TransformScriptList.Count, workspaceFullPath);
    }

    public async Task<ImportToWorkspaceResult> AddSqlFileToWorkspaceAsync(
        string sqlFilePath,
        string targetSqlIdentifier,
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
        string targetSqlIdentifier,
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
        var root = ResolveSelectStatement(model, script);
        return emitter.Render(root);
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
                scripts.Select(script => WrapInCreateEnvelope(model, script, emitter.Render(ResolveSelectStatement(model, script)))));
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
            var selectStatement = ResolveSelectStatement(model, script);
            var sql = WrapInCreateEnvelope(model, script, emitter.Render(selectStatement));
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
        string targetSqlIdentifier,
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
                            $"SQL input '{sourceLabel}' mixes bare SELECT statements with CREATE VIEW/CREATE FUNCTION wrappers. Split the inputs so one logical import source uses one top-level shape.");
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
                "SQL input did not contain a supported SELECT statement, CREATE VIEW wrapper, or inline CREATE FUNCTION wrapper.");
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

        addedScripts[0].TargetSqlIdentifier = NormalizeTargetSqlIdentifier(targetSqlIdentifier);
        await MetaTransformScriptInstance.SaveToWorkspaceAsync(merged, workspaceFullPath, cancellationToken).ConfigureAwait(false);
        return new ImportToWorkspaceResult(merged, merged.TransformScriptList.Count, workspaceFullPath);
    }

    private static void ApplySingleScriptTarget(MTS.MetaTransformScriptModel model, string targetSqlIdentifier)
    {
        ArgumentNullException.ThrowIfNull(model);
        var script = model.TransformScriptList.Count switch
        {
            1 => model.TransformScriptList[0],
            _ => throw new InvalidOperationException(
                $"Expected exactly one transform script for this import operation, but found {model.TransformScriptList.Count}.")
        };
        script.TargetSqlIdentifier = NormalizeTargetSqlIdentifier(targetSqlIdentifier);
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
                            $"SQL input '{sourceLabel}' mixes bare SELECT statements with CREATE VIEW/CREATE FUNCTION wrappers. Split the inputs so one logical import source uses one top-level shape.");
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
                "SQL input did not contain a supported SELECT statement, CREATE VIEW wrapper, or inline CREATE FUNCTION wrapper.");
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
            _ => MetaTransformScriptSqlImportFailureKind.InvalidSqlInput
        };

        return new MetaTransformScriptSqlImportException(
            kind,
            $"SQL import failed for '{(string.IsNullOrWhiteSpace(sourcePath) ? "<sql-code>" : sourcePath)}'.{Environment.NewLine}  {exception.Message}",
            exception);
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

    private static MTS.SelectStatement ResolveSelectStatement(MTS.MetaTransformScriptModel model, MTS.TransformScript script)
    {
        var link = model.TransformScriptSelectStatementLinkList.SingleOrDefault(item => string.Equals(item.TransformScriptId, script.Id, StringComparison.Ordinal))
            ?? throw new InvalidOperationException($"Transform script '{script.Name}' is missing its SelectStatement link.");
        return model.SelectStatementList.SingleOrDefault(item => string.Equals(item.Id, link.SelectStatementId, StringComparison.Ordinal))
            ?? throw new InvalidOperationException($"Transform script '{script.Name}' points to a missing SelectStatement '{link.SelectStatementId}'.");
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

    private static string WrapInCreateEnvelope(MTS.MetaTransformScriptModel model, MTS.TransformScript script, string bodySql)
    {
        var scriptObjectKind = ResolveScriptObjectKind(script);
        return scriptObjectKind switch
        {
            "View" => WrapInCreateViewEnvelope(model, script, bodySql),
            "InlineTableValuedFunction" => WrapInCreateInlineTableValuedFunctionEnvelope(model, script, bodySql),
            _ => throw new InvalidOperationException($"Unsupported TransformScript.ScriptObjectKind '{scriptObjectKind}'.")
        };
    }

    private static string ResolveScriptObjectKind(MTS.TransformScript script)
    {
        return string.IsNullOrWhiteSpace(script.ScriptObjectKind)
            ? "View"
            : script.ScriptObjectKind;
    }

    private static string WrapInCreateViewEnvelope(MTS.MetaTransformScriptModel model, MTS.TransformScript script, string bodySql)
    {
        var trimmedBody = bodySql.Trim();
        var createObjectName = ResolveCreateObjectName(model, script);
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
        builder.AppendLine("GO");
        return builder.ToString();
    }

    private static string WrapInCreateInlineTableValuedFunctionEnvelope(
        MTS.MetaTransformScriptModel model,
        MTS.TransformScript script,
        string bodySql)
    {
        var createObjectName = ResolveCreateObjectName(model, script);
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
        builder.AppendLine("GO");
        return builder.ToString();
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
        if (!string.IsNullOrWhiteSpace(script.TargetSqlIdentifier))
        {
            return script.TargetSqlIdentifier;
        }

        var objectIdentifier = ResolveOptionalIdentifier(
            model,
            model.TransformScriptObjectIdentifierLinkList.SingleOrDefault(item => string.Equals(item.TransformScriptId, script.Id, StringComparison.Ordinal)));
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
            model.TransformScriptSchemaIdentifierLinkList.SingleOrDefault(item => string.Equals(item.TransformScriptId, script.Id, StringComparison.Ordinal)));

        return schemaIdentifier is null
            ? RenderIdentifierFromModel(objectIdentifier)
            : $"{RenderIdentifierFromModel(schemaIdentifier)}.{RenderIdentifierFromModel(objectIdentifier)}";
    }

    private static string RenderFunctionParameterList(MTS.MetaTransformScriptModel model, MTS.TransformScript script)
    {
        var parameters = model.TransformScriptFunctionParametersItemList
            .Where(item => string.Equals(item.TransformScriptId, script.Id, StringComparison.Ordinal))
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
            var parameterName = RenderIdentifierFromModel(ResolveIdentifier(model, parameter.IdentifierId));
            var dataType = RenderDataTypeReference(model, ResolveDataTypeReference(model, parameter.DataTypeReferenceId));
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
            .Where(item => string.Equals(item.TransformScriptId, script.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => ResolveIdentifier(model, item.IdentifierId))
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
            .SingleOrDefault(item => string.Equals(item.DataTypeReferenceId, dataTypeReference.Id, StringComparison.Ordinal));
        if (nameLink is not null)
        {
            renderedName = RenderSchemaObjectName(model, nameLink.SchemaObjectNameId);
        }
        else
        {
            var parameterizedDataTypeReference = model.ParameterizedDataTypeReferenceList
                .SingleOrDefault(item => string.Equals(item.DataTypeReferenceId, dataTypeReference.Id, StringComparison.Ordinal));
            if (parameterizedDataTypeReference is null)
            {
                throw new InvalidOperationException($"Unsupported DataTypeReference '{dataTypeReference.Id}'.");
            }

            var sqlDataTypeReference = model.SqlDataTypeReferenceList
                .SingleOrDefault(item => string.Equals(item.ParameterizedDataTypeReferenceId, parameterizedDataTypeReference.Id, StringComparison.Ordinal))
                ?? throw new InvalidOperationException($"SqlDataTypeReference for '{parameterizedDataTypeReference.Id}' was not found.");
            renderedName = MetaTransformScriptSqlServerDataTypes.RenderSqlName(sqlDataTypeReference.SqlDataTypeOption);

            var parameters = model.ParameterizedDataTypeReferenceParametersItemList
                .Where(item => string.Equals(item.ParameterizedDataTypeReferenceId, parameterizedDataTypeReference.Id, StringComparison.Ordinal))
                .OrderBy(item => ParseOrdinal(item.Ordinal))
                .Select(item => RenderLiteral(model, item.LiteralId))
                .ToArray();

            return parameters.Length == 0
                ? renderedName
                : renderedName + "(" + string.Join(", ", parameters) + ")";
        }

        var parameterizedByName = model.ParameterizedDataTypeReferenceList
            .SingleOrDefault(item => string.Equals(item.DataTypeReferenceId, dataTypeReference.Id, StringComparison.Ordinal));
        if (parameterizedByName is null)
        {
            return renderedName;
        }

        var renderedParameters = model.ParameterizedDataTypeReferenceParametersItemList
            .Where(item => string.Equals(item.ParameterizedDataTypeReferenceId, parameterizedByName.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => RenderLiteral(model, item.LiteralId))
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
            .Where(item => string.Equals(item.MultiPartIdentifierId, schemaObjectName.MultiPartIdentifierId, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => RenderIdentifierFromModel(ResolveIdentifier(model, item.IdentifierId)))
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

        if (model.MaxLiteralList.Any(item => string.Equals(item.LiteralId, literal.Id, StringComparison.Ordinal)))
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
}

public sealed record ImportToWorkspaceResult(
    MTS.MetaTransformScriptModel Model,
    int ScriptCount,
    string WorkspacePath);

public sealed record ExportToPathResult(
    int ScriptCount,
    string OutputPath);
