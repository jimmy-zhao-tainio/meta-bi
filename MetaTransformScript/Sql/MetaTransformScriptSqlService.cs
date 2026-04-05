using System.Text;
using MetaTransformScript.Instance;
using MetaTransformScript.Sql.Parsing;
using MTS = global::MetaTransformScript;

namespace MetaTransformScript.Sql;

public sealed class MetaTransformScriptSqlService
{
    public MetaTransformScriptSqlService()
    {
    }

    public MTS.MetaTransformScriptModel ImportFromSqlPath(string sqlPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sqlPath);

        var fullPath = Path.GetFullPath(sqlPath);
        if (File.Exists(fullPath))
        {
            return ImportFromSingleSqlFile(fullPath);
        }

        return ImportFromSqlDirectory(fullPath);
    }

    public MTS.MetaTransformScriptModel ImportFromSqlCode(string sqlCode, string? scriptName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sqlCode);
        return ImportFromSqlSources([new SqlImportSource(sqlCode, SourcePath: null, BareSelectName: scriptName)]);
    }

    public async Task<ImportToWorkspaceResult> ImportFromSqlPathToWorkspaceAsync(
        string sqlPath,
        string newWorkspacePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sqlPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(newWorkspacePath);
        cancellationToken.ThrowIfCancellationRequested();

        var workspaceFullPath = Path.GetFullPath(newWorkspacePath);
        EnsureTargetDirectoryIsEmpty(workspaceFullPath);
        Directory.CreateDirectory(workspaceFullPath);

        var model = ImportFromSqlPath(sqlPath);
        await MetaTransformScriptInstance.SaveToWorkspaceAsync(model, workspaceFullPath, cancellationToken).ConfigureAwait(false);

        return new ImportToWorkspaceResult(model, model.TransformScriptList.Count, workspaceFullPath);
    }

    public async Task<ImportToWorkspaceResult> ImportFromSqlCodeToWorkspaceAsync(
        string sqlCode,
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
        await MetaTransformScriptInstance.SaveToWorkspaceAsync(model, workspaceFullPath, cancellationToken).ConfigureAwait(false);

        return new ImportToWorkspaceResult(model, model.TransformScriptList.Count, workspaceFullPath);
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
                scripts.Select(script => WrapInCreateViewEnvelope(model, script, emitter.Render(ResolveSelectStatement(model, script)))));
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
            var sql = WrapInCreateViewEnvelope(model, script, emitter.Render(selectStatement));
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

    private MTS.MetaTransformScriptModel ImportFromSqlDirectory(string fullPath)
    {
        if (!Directory.Exists(fullPath))
        {
            throw new MetaTransformScriptSqlImportException(
                MetaTransformScriptSqlImportFailureKind.SourcePathNotFound,
                $"SQL path '{fullPath}' was not found.");
        }

        var files = Directory.EnumerateFiles(fullPath, "*.sql", SearchOption.TopDirectoryOnly)
            .OrderBy(static path => path, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (files.Length == 0)
        {
            throw new MetaTransformScriptSqlImportException(
                MetaTransformScriptSqlImportFailureKind.SourcePathHasNoSqlFiles,
                $"SQL folder '{fullPath}' does not contain any .sql files.");
        }

        return ImportFromSqlSources(files.Select(file => new SqlImportSource(
            File.ReadAllText(file),
            SourcePath: Path.GetRelativePath(fullPath, file).Replace('\\', '/'),
            BareSelectName: null)));
    }

    private MTS.MetaTransformScriptModel ImportFromSingleSqlFile(string fullPath)
    {
        var sql = File.ReadAllText(fullPath);
        var sourcePath = Path.GetFileName(fullPath);
        return ImportFromSqlSources([new SqlImportSource(sql, SourcePath: sourcePath, BareSelectName: null)]);
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
                            $"SQL input '{sourceLabel}' mixes bare SELECT statements with CREATE VIEW wrappers. Split the inputs so one logical import source uses one top-level shape.");
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
                "SQL input did not contain a supported SELECT statement or CREATE VIEW wrapper.");
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
        var link = model.TransformScriptSelectStatementLinkList.SingleOrDefault(item => string.Equals(item.OwnerId, script.Id, StringComparison.Ordinal))
            ?? throw new InvalidOperationException($"Transform script '{script.Name}' is missing its SelectStatement link.");
        return model.SelectStatementList.SingleOrDefault(item => string.Equals(item.Id, link.ValueId, StringComparison.Ordinal))
            ?? throw new InvalidOperationException($"Transform script '{script.Name}' points to a missing SelectStatement '{link.ValueId}'.");
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

    private static string WrapInCreateViewEnvelope(MTS.MetaTransformScriptModel model, MTS.TransformScript script, string bodySql)
    {
        var trimmedBody = bodySql.Trim();
        var createViewName = ResolveCreateViewName(model, script);
        var columnList = RenderViewColumnList(model, script);

        var builder = new StringBuilder();
        builder.Append("CREATE VIEW ");
        builder.AppendLine(createViewName);
        if (!string.IsNullOrWhiteSpace(columnList))
        {
            builder.AppendLine(columnList);
        }

        builder.AppendLine("AS");
        builder.AppendLine(trimmedBody);
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

    private static string ResolveCreateViewName(MTS.MetaTransformScriptModel model, MTS.TransformScript script)
    {
        var objectIdentifier = ResolveOptionalIdentifier(
            model,
            model.TransformScriptObjectIdentifierLinkList.SingleOrDefault(item => string.Equals(item.OwnerId, script.Id, StringComparison.Ordinal)));
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
            model.TransformScriptSchemaIdentifierLinkList.SingleOrDefault(item => string.Equals(item.OwnerId, script.Id, StringComparison.Ordinal)));

        return schemaIdentifier is null
            ? RenderIdentifierFromModel(objectIdentifier)
            : $"{RenderIdentifierFromModel(schemaIdentifier)}.{RenderIdentifierFromModel(objectIdentifier)}";
    }

    private static string RenderViewColumnList(MTS.MetaTransformScriptModel model, MTS.TransformScript script)
    {
        var columns = model.TransformScriptViewColumnsItemList
            .Where(item => string.Equals(item.OwnerId, script.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => ResolveIdentifier(model, item.ValueId))
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

    private static MTS.Identifier? ResolveOptionalIdentifier(MTS.MetaTransformScriptModel model, object? link)
    {
        if (link is null)
        {
            return null;
        }

        var valueId = (string?)link.GetType().GetProperty("ValueId")?.GetValue(link);
        return string.IsNullOrWhiteSpace(valueId) ? null : ResolveIdentifier(model, valueId);
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
