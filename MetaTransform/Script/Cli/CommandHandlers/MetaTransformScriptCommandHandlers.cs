using System.Globalization;
using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaTransformScript.Sql;
using MTS = global::MetaTransformScript;

internal sealed class MetaTransformScriptCommandHandlers
{
    private readonly ConsolePresenter presenter;
    private readonly MetaTransformScriptSqlService service;
    private readonly string appName;

    public MetaTransformScriptCommandHandlers(
        ConsolePresenter presenter,
        MetaTransformScriptSqlService service,
        string appName)
    {
        this.presenter = presenter;
        this.service = service;
        this.appName = appName;
    }

    public async Task RunFromSqlFileAsync(MetaCliInvocation invocation)
    {
        var path = invocation.Required("path");
        var targetSqlIdentifier = invocation.Optional("target");
        var (newWorkspacePath, workspacePath) = ReadImportWorkspaceChoice(invocation);
        var fullPath = Path.GetFullPath(path);
        if (!File.Exists(fullPath))
        {
            Fail(
                "sql-file import source was not found.",
                "check --path and retry.",
                4,
                [$"  Path: {fullPath}"]);
        }

        if (!string.Equals(Path.GetExtension(fullPath), ".sql", StringComparison.OrdinalIgnoreCase))
        {
            Fail(
                "sql-file import source must be a .sql file.",
                "point --path at a .sql file and retry.",
                4,
                [$"  Path: {fullPath}"]);
        }

        try
        {
            if (!string.IsNullOrWhiteSpace(newWorkspacePath))
            {
                var targetValidation = CliNewWorkspaceTargetValidator.Validate(newWorkspacePath);
                if (!targetValidation.Ok)
                {
                    Fail(
                        targetValidation.ErrorMessage,
                        "choose a new folder or empty the target directory and retry.",
                        4,
                        targetValidation.Details);
                }

                using var activity = CliActivityLine.Start("Importing");
                await service.ImportSingleSqlFileToWorkspaceAsync(
                        fullPath,
                        targetSqlIdentifier,
                        targetValidation.FullPath)
                    .ConfigureAwait(false);
                activity.Succeed();
                return;
            }

            using (var activity = CliActivityLine.Start("Importing"))
            {
                await service.AddSqlFileToWorkspaceAsync(
                        fullPath,
                        targetSqlIdentifier,
                        Path.GetFullPath(workspacePath!))
                    .ConfigureAwait(false);
                activity.Succeed();
            }
        }
        catch (MetaTransformScriptSqlImportException ex)
        {
            Fail(
                GetImportFailureMessage("sql-file", ex.Kind),
                GetImportFailureNext("sql-file", ex.Kind),
                4,
                [
                    $"  Path: {fullPath}",
                    $"  Target: {DisplayTarget(targetSqlIdentifier)}",
                    $"  Workspace: {Path.GetFullPath(newWorkspacePath ?? workspacePath ?? string.Empty)}",
                    $"  {ex.Message}"
                ]);
        }
        catch (Exception ex) when (ex is not MetaCliExitException)
        {
            Fail(
                "Cannot import SQL file.",
                "check --path, --target rules, and workspace options, then retry.",
                4,
                [
                    $"  Path: {fullPath}",
                    $"  Target: {DisplayTarget(targetSqlIdentifier)}",
                    $"  Workspace: {Path.GetFullPath(newWorkspacePath ?? workspacePath ?? string.Empty)}",
                    $"  {ex.Message}"
                ]);
        }
    }

    public async Task RunFromSqlFilesAsync(MetaCliInvocation invocation)
    {
        var manifestPath = invocation.Required("manifest");
        var (newWorkspacePath, workspacePath) = ReadImportWorkspaceChoice(invocation);
        var reportPath = invocation.Optional("report");
        var verbose = invocation.Flag("verbose");
        var manifestFullPath = Path.GetFullPath(manifestPath);
        if (!File.Exists(manifestFullPath))
        {
            Fail(
                "sql-files import manifest was not found.",
                "check --manifest and retry.",
                4,
                [$"  Manifest: {manifestFullPath}"]);
        }

        IReadOnlyList<SqlFileImportRequest> requests;
        try
        {
            requests = ReadSqlFilesManifest(manifestFullPath);
        }
        catch (Exception ex)
        {
            Fail(
                "Cannot read sql-files import manifest.",
                "fix the TSV manifest and retry.",
                4,
                [$"  Manifest: {manifestFullPath}", $"  {ex.Message}"]);
            return;
        }

        if (requests.Count == 0)
        {
            Fail(
                "sql-files import manifest did not contain any files.",
                "add at least one data row with a Path value and retry.",
                4,
                [$"  Manifest: {manifestFullPath}"]);
        }

        try
        {
            var completedCount = 0;
            var failureCount = 0;
            var currentFileName = string.Empty;
            CliLiveLineRenderer? liveLine = null;

            void ReportProgress(SqlFileImportProgress progress)
            {
                completedCount = progress.Index;
                currentFileName = Path.GetFileName(progress.Path);
                if (!progress.Success)
                {
                    failureCount++;
                }

                if (!verbose)
                {
                    return;
                }

                var status = progress.Success ? "ok" : "failed";
                var suffix = progress.Success ? string.Empty : " - " + SummarizeImportFailure(progress.Message);
                Console.Out.WriteLine($"{status} [{progress.Index}/{progress.Total}] {currentFileName}{suffix}");
            }

            ImportSqlFilesToWorkspaceResult result;
            try
            {
                if (!verbose)
                {
                    liveLine = CliLiveLineRenderer.TryStart(
                        () => BuildSqlFilesImportProgressLine(
                            completedCount,
                            requests.Count,
                            failureCount,
                            currentFileName),
                        delay: TimeSpan.Zero);
                }

                if (!string.IsNullOrWhiteSpace(newWorkspacePath))
                {
                    var targetValidation = CliNewWorkspaceTargetValidator.Validate(newWorkspacePath);
                    if (!targetValidation.Ok)
                    {
                        liveLine?.Clear();
                        liveLine = null;
                        Fail(
                            targetValidation.ErrorMessage,
                            "choose a new folder or empty the target directory and retry.",
                            4,
                            targetValidation.Details);
                    }

                    result = await service.ImportSqlFilesToNewWorkspaceAsync(
                            requests,
                            targetValidation.FullPath,
                            ReportProgress)
                        .ConfigureAwait(false);
                }
                else
                {
                    result = await service.AddSqlFilesToWorkspaceAsync(
                            requests,
                            Path.GetFullPath(workspacePath!),
                            ReportProgress)
                        .ConfigureAwait(false);
                }

                liveLine?.Complete(BuildSqlFilesImportProgressLine(
                    result.Successes.Count + result.Failures.Count,
                    requests.Count,
                    result.Failures.Count,
                    string.Empty));
                liveLine = null;
            }
            finally
            {
                liveLine?.Clear();
            }

            if (!string.IsNullOrWhiteSpace(reportPath))
            {
                await WriteSqlFilesImportReportAsync(result, reportPath).ConfigureAwait(false);
            }

            if (result.Failures.Count > 0)
            {
                Fail(
                    "SQL files import completed with failures.",
                    string.IsNullOrWhiteSpace(reportPath)
                        ? "inspect the failed files, fix the SQL or target mapping, and retry."
                        : "inspect the import report, fix the SQL or target mapping, and retry.",
                    4,
                    BuildSqlFilesFailureDetails(result, reportPath));
            }

            presenter.WriteOk($"Imported {result.Successes.Count} SQL file{(result.Successes.Count == 1 ? string.Empty : "s")}");
            presenter.WriteKeyValueBlock("Workspace", [
                ("Scripts", result.ScriptCount.ToString(CultureInfo.InvariantCulture)),
                ("Path", result.WorkspacePath)
            ]);
            if (!string.IsNullOrWhiteSpace(reportPath))
            {
                presenter.WriteKeyValueBlock("Report", [("Path", Path.GetFullPath(reportPath))]);
            }
        }
        catch (Exception ex) when (ex is not MetaCliExitException)
        {
            Fail(
                "Cannot import SQL files.",
                "check --manifest, --workspace options, and report path, then retry.",
                4,
                [
                    $"  Manifest: {manifestFullPath}",
                    $"  Workspace: {Path.GetFullPath(newWorkspacePath ?? workspacePath ?? string.Empty)}",
                    $"  {ex.Message}"
                ]);
        }
    }

    public async Task RunFromSqlCodeAsync(MetaCliInvocation invocation)
    {
        var code = invocation.Required("code");
        var targetSqlIdentifier = invocation.Optional("target");
        var name = invocation.Optional("name");
        var (newWorkspacePath, workspacePath) = ReadImportWorkspaceChoice(invocation);
        try
        {
            if (!string.IsNullOrWhiteSpace(newWorkspacePath))
            {
                var targetValidation = CliNewWorkspaceTargetValidator.Validate(newWorkspacePath);
                if (!targetValidation.Ok)
                {
                    Fail(
                        targetValidation.ErrorMessage,
                        "choose a new folder or empty the target directory and retry.",
                        4,
                        targetValidation.Details);
                }

                using var activity = CliActivityLine.Start("Importing");
                await service.ImportFromSqlCodeToWorkspaceAsync(
                        code,
                        targetSqlIdentifier,
                        targetValidation.FullPath,
                        name)
                    .ConfigureAwait(false);
                activity.Succeed();
                return;
            }

            using (var activity = CliActivityLine.Start("Importing"))
            {
                await service.AddSqlCodeToWorkspaceAsync(
                        code,
                        targetSqlIdentifier,
                        Path.GetFullPath(workspacePath!),
                        name)
                    .ConfigureAwait(false);
                activity.Succeed();
            }
        }
        catch (MetaTransformScriptSqlImportException ex)
        {
            Fail(
                GetImportFailureMessage("sql-code", ex.Kind),
                GetImportFailureNext("sql-code", ex.Kind),
                4,
                [
                    $"  Target: {DisplayTarget(targetSqlIdentifier)}",
                    $"  Workspace: {Path.GetFullPath(newWorkspacePath ?? workspacePath ?? string.Empty)}",
                    $"  {ex.Message}"
                ]);
        }
        catch (Exception ex) when (ex is not MetaCliExitException)
        {
            Fail(
                "Cannot import SQL code.",
                "check --code, --target rules, and workspace options, then retry.",
                4,
                [
                    $"  Target: {DisplayTarget(targetSqlIdentifier)}",
                    $"  Workspace: {Path.GetFullPath(newWorkspacePath ?? workspacePath ?? string.Empty)}",
                    $"  {ex.Message}"
                ]);
        }
    }

    public async Task RunToSqlPathAsync(MetaCliInvocation invocation, MTS.MetaTransformScriptModel model)
    {
        var outputPath = invocation.Required("out");
        var workspacePath = WorkspacePath(invocation);
        try
        {
            var result = await service.ExportToSqlPathAsync(model, outputPath).ConfigureAwait(false);
            presenter.WriteOk($"Exported {result.ScriptCount} transform script{(result.ScriptCount == 1 ? string.Empty : "s")}");
            presenter.WriteKeyValueBlock("Output", [
                ("Scripts", result.ScriptCount.ToString(CultureInfo.InvariantCulture)),
                ("Path", result.OutputPath)
            ]);
        }
        catch (Exception ex)
        {
            Fail(
                "Cannot export SQL files.",
                "check the workspace, output path, and selected script, then retry.",
                4,
                [
                    $"  Workspace: {workspacePath}",
                    $"  Output: {Path.GetFullPath(outputPath)}",
                    $"  {ex.Message}"
                ]);
        }
    }

    public void RunToSqlCode(MetaCliInvocation invocation, MTS.MetaTransformScriptModel model)
    {
        var workspacePath = WorkspacePath(invocation);
        try
        {
            var sql = service.ExportToSqlCode(model, invocation.Optional("name"));
            Console.Out.Write(sql);
            if (!sql.EndsWith(Environment.NewLine, StringComparison.Ordinal))
            {
                Console.Out.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Fail(
                "Cannot export SQL code.",
                "check the workspace and selected script, then retry.",
                4,
                [$"  Workspace: {workspacePath}", $"  {ex.Message}"]);
        }
    }

    public async Task RunTargetIdentifiersFromPatternAsync(MetaCliInvocation invocation)
    {
        var workspacePath = WorkspacePath(invocation);
        var sourcePattern = invocation.Required("source-pattern");
        var targetPattern = invocation.Required("target-pattern");
        var onlyMissing = invocation.Flag("only-missing");
        var dryRun = invocation.Flag("dry-run");
        var allowEmpty = invocation.Flag("allow-empty");
        var verbose = invocation.Flag("verbose");
        try
        {
            var result = await service.UpdateTargetIdentifiersFromPatternAsync(
                    workspacePath,
                    sourcePattern,
                    targetPattern,
                    onlyMissing,
                    dryRun)
                .ConfigureAwait(false);

            if (result.UpdatedCount == 0 && !allowEmpty)
            {
                Fail(
                    "No target identifiers were updated.",
                    "adjust --source-pattern/--target-pattern or pass --allow-empty when no updates are expected.",
                    4,
                    [
                        $"  Workspace: {workspacePath}",
                        $"  SourcePattern: {sourcePattern}",
                        $"  TargetPattern: {targetPattern}",
                        $"  Scripts: {result.ScriptCount}",
                        $"  Matched: {result.MatchedCount}",
                        $"  ExistingSkipped: {result.SkippedExistingCount}",
                        $"  Unchanged: {result.UnchangedCount}"
                    ]);
            }

            presenter.WriteInfo(
                dryRun
                    ? $"Target identifiers that would change: {result.UpdatedCount}"
                    : $"Target identifiers updated: {result.UpdatedCount}");
            presenter.WriteKeyValueBlock("Summary", [
                ("Scripts", result.ScriptCount.ToString(CultureInfo.InvariantCulture)),
                ("Matched", result.MatchedCount.ToString(CultureInfo.InvariantCulture)),
                ("Updated", result.UpdatedCount.ToString(CultureInfo.InvariantCulture)),
                ("ExistingSkipped", result.SkippedExistingCount.ToString(CultureInfo.InvariantCulture)),
                ("Unchanged", result.UnchangedCount.ToString(CultureInfo.InvariantCulture)),
                ("Workspace", result.WorkspacePath)
            ]);

            if (verbose || dryRun)
            {
                foreach (var update in result.Updates)
                {
                    Console.Out.WriteLine(
                        $"{update.TransformScriptName}: {DisplayTarget(update.PreviousTargetSqlIdentifier)} -> {update.TargetSqlIdentifier}");
                }
            }
        }
        catch (Exception ex) when (ex is not MetaCliExitException)
        {
            Fail(
                "Cannot update target identifiers.",
                "check the workspace, patterns, and target identifier shape, then retry.",
                4,
                [
                    $"  Workspace: {workspacePath}",
                    $"  SourcePattern: {sourcePattern}",
                    $"  TargetPattern: {targetPattern}",
                    $"  {ex.Message}"
                ]);
        }
    }

    public async Task RunStoredProcedureViewContractAsync(MetaCliInvocation invocation)
    {
        var workspacePath = WorkspacePath(invocation);
        try
        {
            var result = await service.InspectStoredProcedureContractsAsync(
                    workspacePath,
                    invocation.Optional("name"))
                .ConfigureAwait(false);

            presenter.WriteInfo($"Stored procedures: {result.StoredProcedureCount}");
            presenter.WriteKeyValueBlock("Contracts", [
                ("Present", result.ContractedCount.ToString(CultureInfo.InvariantCulture)),
                ("Missing", result.MissingContractCount.ToString(CultureInfo.InvariantCulture)),
                ("Invalid", result.InvalidContractCount.ToString(CultureInfo.InvariantCulture)),
                ("Workspace", result.WorkspacePath)
            ]);

            foreach (var item in result.Items)
            {
                WriteStoredProcedureContractInspectionItem(item);
            }
        }
        catch (Exception ex)
        {
            Fail(
                "Cannot view stored procedure contracts.",
                "check the workspace path and optional --name value, then retry.",
                4,
                [$"  Workspace: {workspacePath}", $"  {ex.Message}"]);
        }
    }

    public async Task RunStoredProcedureAddContractAsync(MetaCliInvocation invocation)
    {
        var workspacePath = WorkspacePath(invocation);
        var name = invocation.Required("name");
        try
        {
            var declaration = ReadStoredProcedureDeclaration(invocation);
            var result = await service.AddStoredProcedureContractAsync(
                    workspacePath,
                    name,
                    declaration)
                .ConfigureAwait(false);

            presenter.WriteInfo($"Stored procedure contract written: {result.Item.TransformScriptName}");
            presenter.WriteKeyValueBlock("Declared", [
                ("Operations", result.Item.OperationCount.ToString(CultureInfo.InvariantCulture)),
                ("Reads", result.Item.ReadOperationCount.ToString(CultureInfo.InvariantCulture)),
                ("Writes", result.Item.WriteOperationCount.ToString(CultureInfo.InvariantCulture)),
                ("Calls", result.Item.CallOperationCount.ToString(CultureInfo.InvariantCulture)),
                ("ResultRowsets", result.Item.ResultRowsetCount.ToString(CultureInfo.InvariantCulture)),
                ("ResultColumns", result.Item.ResultColumnCount.ToString(CultureInfo.InvariantCulture)),
                ("Workspace", result.WorkspacePath)
            ]);
        }
        catch (Exception ex) when (ex is not MetaCliExitException)
        {
            Fail(
                "Cannot add stored procedure contract.",
                "check the workspace, transform script name, and declaration options, then retry.",
                4,
                [$"  Workspace: {workspacePath}", $"  Name: {name}", $"  {ex.Message}"]);
        }
    }

    public async Task RunStoredProcedureRemoveContractAsync(MetaCliInvocation invocation)
    {
        var workspacePath = WorkspacePath(invocation);
        var name = invocation.Required("name");
        try
        {
            var result = await service.RemoveStoredProcedureContractAsync(workspacePath, name)
                .ConfigureAwait(false);

            presenter.WriteInfo($"Stored procedure contract removed: {result.TransformScriptName}");
            presenter.WriteKeyValueBlock("Removed", [
                ("Contracts", result.ContractCount.ToString(CultureInfo.InvariantCulture)),
                ("Operations", result.OperationCount.ToString(CultureInfo.InvariantCulture)),
                ("ResultRowsets", result.ResultRowsetCount.ToString(CultureInfo.InvariantCulture)),
                ("ResultColumns", result.ResultColumnCount.ToString(CultureInfo.InvariantCulture)),
                ("Workspace", result.WorkspacePath)
            ]);
        }
        catch (Exception ex)
        {
            Fail(
                "Cannot remove stored procedure contract.",
                "check the workspace and transform script name, then retry.",
                4,
                [$"  Workspace: {workspacePath}", $"  Name: {name}", $"  {ex.Message}"]);
        }
    }

    private static (string? NewWorkspacePath, string? WorkspacePath) ReadImportWorkspaceChoice(MetaCliInvocation invocation) =>
        (invocation.Optional("new-workspace"), invocation.Optional("workspace"));

    private static string WorkspacePath(MetaCliInvocation invocation)
    {
        var value = invocation.Optional("workspace");
        return Path.GetFullPath(string.IsNullOrWhiteSpace(value) ? Environment.CurrentDirectory : value);
    }

    private StoredProcedureContractDeclaration ReadStoredProcedureDeclaration(MetaCliInvocation invocation)
    {
        var operations = new List<StoredProcedureContractOperationDeclaration>();
        foreach (var value in invocation.Values("operation"))
        {
            var operation = ParseStoredProcedureOperation(value, out var operationError);
            if (operation is null)
            {
                Fail(operationError, HelpCommand("stored-procedure add-contract"));
                throw new InvalidOperationException(operationError);
            }

            operations.Add(operation);
        }

        var resultRowsets = new List<ResultRowsetBuilder>();
        foreach (var value in invocation.Values("result-rowset"))
        {
            var rowsetName = value.Trim();
            if (string.IsNullOrWhiteSpace(rowsetName))
            {
                Fail("--result-rowset requires a name.", HelpCommand("stored-procedure add-contract"));
            }

            GetOrAddResultRowset(resultRowsets, rowsetName);
        }

        foreach (var value in invocation.Values("result-column"))
        {
            var (rowsetName, columnName) = SplitRequiredAssignment(value);
            if (string.IsNullOrWhiteSpace(rowsetName) || string.IsNullOrWhiteSpace(columnName))
            {
                Fail("--result-column requires <rowset>=<column>.", HelpCommand("stored-procedure add-contract"));
            }

            GetOrAddResultRowset(resultRowsets, rowsetName)
                .Columns
                .Add(new StoredProcedureResultColumnDeclaration(columnName, MetaDataTypeId: null, IsNullable: null));
        }

        if (resultRowsets.Count > 1)
        {
            Fail(
                "stored procedure contracts support at most one result rowset.",
                HelpCommand("stored-procedure add-contract"));
        }

        return new StoredProcedureContractDeclaration(
            Operations: operations,
            ResultRowsets: resultRowsets.Select(static item => item.ToDeclaration()).ToArray(),
            Notes: invocation.Optional("notes"));
    }

    private static IReadOnlyList<SqlFileImportRequest> ReadSqlFilesManifest(string manifestPath)
    {
        var manifestFullPath = Path.GetFullPath(manifestPath);
        var manifestDirectory = Path.GetDirectoryName(manifestFullPath) ?? Environment.CurrentDirectory;
        var rows = new List<SqlFileImportRequest>();
        var lines = File.ReadAllLines(manifestFullPath);
        string[]? headers = null;
        var pathColumnIndex = -1;
        var targetColumnIndex = -1;

        for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            var line = lines[lineIndex];
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            if (headers is null)
            {
                headers = SplitTsvLine(line);
                pathColumnIndex = FindColumnIndex(headers, "Path");
                if (pathColumnIndex < 0)
                {
                    pathColumnIndex = FindColumnIndex(headers, "FilePath");
                }

                targetColumnIndex = FindColumnIndex(headers, "Target");
                if (pathColumnIndex < 0)
                {
                    throw new InvalidOperationException("Manifest header must contain a Path column.");
                }

                continue;
            }

            var cells = SplitTsvLine(line);
            var sourcePath = ReadManifestCell(cells, pathColumnIndex);
            if (string.IsNullOrWhiteSpace(sourcePath))
            {
                throw new InvalidOperationException($"Manifest line {lineIndex + 1} has a blank Path value.");
            }

            var resolvedPath = Path.IsPathFullyQualified(sourcePath)
                ? sourcePath
                : Path.GetFullPath(Path.Combine(manifestDirectory, sourcePath));
            var target = targetColumnIndex >= 0
                ? ReadManifestCell(cells, targetColumnIndex)
                : null;

            rows.Add(new SqlFileImportRequest(
                resolvedPath,
                string.IsNullOrWhiteSpace(target) ? null : target));
        }

        if (headers is null)
        {
            throw new InvalidOperationException("Manifest does not contain a header row.");
        }

        return rows;
    }

    private static int FindColumnIndex(string[] headers, string name)
    {
        for (var i = 0; i < headers.Length; i++)
        {
            if (string.Equals(headers[i].Trim(), name, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return -1;
    }

    private static string[] SplitTsvLine(string line) => line.Split('\t');

    private static string? ReadManifestCell(string[] cells, int index) =>
        index >= 0 && index < cells.Length ? cells[index].Trim() : null;

    private static async Task WriteSqlFilesImportReportAsync(
        ImportSqlFilesToWorkspaceResult result,
        string reportPath)
    {
        var fullPath = Path.GetFullPath(reportPath);
        var parentDirectory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrWhiteSpace(parentDirectory))
        {
            Directory.CreateDirectory(parentDirectory);
        }

        var lines = new List<string>
        {
            "Status\tIndex\tTotal\tPath\tTarget\tScriptName\tFailureKind\tErrorSummary\tLine\tColumn\tMessage"
        };

        foreach (var success in result.Successes)
        {
            lines.Add(string.Join(
                '\t',
                "Success",
                success.Index.ToString(CultureInfo.InvariantCulture),
                success.Total.ToString(CultureInfo.InvariantCulture),
                EscapeTsv(success.Path),
                EscapeTsv(success.TargetSqlIdentifier),
                EscapeTsv(success.ScriptName),
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty));
        }

        foreach (var failure in result.Failures)
        {
            lines.Add(string.Join(
                '\t',
                "Failure",
                failure.Index.ToString(CultureInfo.InvariantCulture),
                failure.Total.ToString(CultureInfo.InvariantCulture),
                EscapeTsv(failure.Path),
                EscapeTsv(failure.TargetSqlIdentifier),
                string.Empty,
                failure.Kind.ToString(),
                EscapeTsv(SummarizeImportFailure(failure.Message)),
                FormatNullableInt(failure.Line),
                FormatNullableInt(failure.Column),
                EscapeTsv(failure.Message)));
        }

        await File.WriteAllLinesAsync(fullPath, lines).ConfigureAwait(false);
    }

    private static IEnumerable<string> BuildSqlFilesFailureDetails(
        ImportSqlFilesToWorkspaceResult result,
        string? reportPath)
    {
        yield return $"  Workspace: {result.WorkspacePath}";
        yield return $"  Successes: {result.Successes.Count}";
        yield return $"  Failures: {result.Failures.Count}";
        if (!string.IsNullOrWhiteSpace(reportPath))
        {
            yield return $"  Report: {Path.GetFullPath(reportPath)}";
        }

        foreach (var failure in result.Failures.Take(10))
        {
            yield return $"  [{failure.Index}/{failure.Total}] {Path.GetFileName(failure.Path)}: {failure.Kind}: {SummarizeImportFailure(failure.Message)}";
        }

        if (result.Failures.Count > 10)
        {
            yield return $"  ...{result.Failures.Count - 10} more failure(s).";
        }
    }

    private static string BuildSqlFilesImportProgressLine(
        int completed,
        int total,
        int failures,
        string? currentFileName)
    {
        var meter = BuildMeter(completed, total);
        var file = string.IsNullOrWhiteSpace(currentFileName) ? string.Empty : " " + currentFileName;
        return $"Importing SQL files {meter} {completed} of {total}, {failures} failed{file}";
    }

    private static string BuildMeter(int completed, int total)
    {
        const int width = 20;
        if (total <= 0)
        {
            return "[" + new string('-', width) + "]";
        }

        var filled = Math.Clamp((int)Math.Round(width * (completed / (double)total)), 0, width);
        return "[" + new string('=', filled) + new string('-', width - filled) + "]";
    }

    private static string SummarizeImportFailure(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var lines = value
            .Split(["\r\n", "\n"], StringSplitOptions.None)
            .Select(static line => line.Trim())
            .Where(static line => line.Length > 0)
            .ToArray();

        foreach (var line in lines)
        {
            if (line.StartsWith("SQL import failed", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (line.Contains("Expected ", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("Unexpected ", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("unsupported", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("not supported", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("does not allow", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("line ", StringComparison.OrdinalIgnoreCase))
            {
                return line;
            }
        }

        return lines.FirstOrDefault() ?? string.Empty;
    }

    private static string EscapeTsv(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ');

    private static string FormatNullableInt(int? value) =>
        value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;

    private static void WriteStoredProcedureContractInspectionItem(
        StoredProcedureContractInspectionItem item)
    {
        Console.Out.WriteLine(item.TransformScriptName);
        Console.Out.WriteLine($"  Contract: {RenderStoredProcedureContractState(item.ContractState, item.ContractRowCount)}");
        Console.Out.WriteLine(
            $"  Declared: operations {item.OperationCount}, reads {item.ReadOperationCount}, writes {item.WriteOperationCount}, calls {item.CallOperationCount}, result rowsets {item.ResultRowsetCount}, result columns {item.ResultColumnCount}");
    }

    private static string RenderStoredProcedureContractState(
        StoredProcedureContractState state,
        int contractRowCount) =>
        state switch
        {
            StoredProcedureContractState.Present => "Present",
            StoredProcedureContractState.Missing => "Missing",
            _ => $"Invalid ({contractRowCount} rows)"
        };

    private static StoredProcedureContractOperationDeclaration? ParseStoredProcedureOperation(
        string value,
        out string errorMessage)
    {
        errorMessage = string.Empty;
        var firstSeparator = value.IndexOf(':');
        var secondSeparator = firstSeparator < 0 ? -1 : value.IndexOf(':', firstSeparator + 1);
        if (firstSeparator <= 0 || secondSeparator <= firstSeparator + 1)
        {
            errorMessage = "--operation requires <ordinal>:<kind>:<sql-id>[=<role>].";
            return null;
        }

        var ordinalText = value[..firstSeparator].Trim();
        if (!int.TryParse(ordinalText, NumberStyles.None, CultureInfo.InvariantCulture, out var ordinal) || ordinal < 0)
        {
            errorMessage = "--operation ordinal must be a non-negative integer.";
            return null;
        }

        var operationKind = value[(firstSeparator + 1)..secondSeparator].Trim();
        var normalizedKind = MetaTransformScriptSqlService.NormalizeStoredProcedureOperationKind(operationKind);
        if (normalizedKind is null)
        {
            errorMessage = $"unsupported --operation kind '{operationKind}'. Supported values: read, append, replace, reset, mutation, call.";
            return null;
        }

        var (sqlIdentifier, accessRole) = SplitOptionalAssignment(value[(secondSeparator + 1)..]);
        if (string.IsNullOrWhiteSpace(sqlIdentifier))
        {
            errorMessage = "--operation requires a SQL identifier.";
            return null;
        }

        return new StoredProcedureContractOperationDeclaration(ordinal, normalizedKind, sqlIdentifier, accessRole);
    }

    private static (string Left, string? Right) SplitOptionalAssignment(string value)
    {
        var index = value.IndexOf('=');
        if (index < 0)
        {
            return (value.Trim(), null);
        }

        var left = value[..index].Trim();
        var right = value[(index + 1)..].Trim();
        return (left, string.IsNullOrWhiteSpace(right) ? null : right);
    }

    private static (string Left, string Right) SplitRequiredAssignment(string value)
    {
        var index = value.IndexOf('=');
        return index < 0
            ? (string.Empty, string.Empty)
            : (value[..index].Trim(), value[(index + 1)..].Trim());
    }

    private static ResultRowsetBuilder GetOrAddResultRowset(
        List<ResultRowsetBuilder> rowsets,
        string name)
    {
        var existing = rowsets.FirstOrDefault(item =>
            string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
        {
            return existing;
        }

        var created = new ResultRowsetBuilder(name);
        rowsets.Add(created);
        return created;
    }

    private string HelpCommand(string commandName) => $"{appName} help {commandName}";

    private void Fail(string message, string next, int exitCode = 1, IEnumerable<string>? details = null)
    {
        var renderedDetails = new List<string>();
        if (details != null)
        {
            renderedDetails.AddRange(details.Where(static detail => !string.IsNullOrWhiteSpace(detail)));
        }

        renderedDetails.Add($"Next: {next}");
        presenter.WriteFailure(message, renderedDetails);
        throw new MetaCliExitException(exitCode);
    }

    private static string GetImportFailureMessage(
        string sourceLabel,
        MetaTransformScriptSqlImportFailureKind kind) =>
        kind switch
        {
            MetaTransformScriptSqlImportFailureKind.SourcePathNotFound => $"{sourceLabel} import source was not found.",
            MetaTransformScriptSqlImportFailureKind.ParseFailed => $"Cannot parse {sourceLabel}.",
            MetaTransformScriptSqlImportFailureKind.UnsupportedSql => $"{sourceLabel} import hit unsupported SQL.",
            MetaTransformScriptSqlImportFailureKind.UnsupportedFunctionWrapper => $"{sourceLabel} import hit an unsupported CREATE FUNCTION wrapper.",
            MetaTransformScriptSqlImportFailureKind.LikelyTextEncodingMismatch => $"{sourceLabel} import found likely text encoding damage.",
            MetaTransformScriptSqlImportFailureKind.InvalidSqlInput => $"{sourceLabel} import found unsupported SQL input shape.",
            _ => $"Cannot import {sourceLabel}."
        };

    private static string GetImportFailureNext(
        string sourceLabel,
        MetaTransformScriptSqlImportFailureKind kind) =>
        kind switch
        {
            MetaTransformScriptSqlImportFailureKind.SourcePathNotFound => sourceLabel == "sql-file"
                ? "check the SQL path and retry."
                : "check the SQL input and retry.",
            MetaTransformScriptSqlImportFailureKind.ParseFailed => "fix the SQL syntax and retry.",
            MetaTransformScriptSqlImportFailureKind.UnsupportedSql => "remove unsupported wrapper options or unsupported SQL surface, then retry.",
            MetaTransformScriptSqlImportFailureKind.UnsupportedFunctionWrapper => "skip scalar or multistatement function files, or import only inline table-valued functions.",
            MetaTransformScriptSqlImportFailureKind.LikelyTextEncodingMismatch => "re-export or split the SQL as Unicode text, such as UTF-8 or UTF-16 with a BOM, then retry.",
            MetaTransformScriptSqlImportFailureKind.InvalidSqlInput => "apply target rules and retry: bare SELECT requires --target; CREATE VIEW may omit --target; inline CREATE FUNCTION forbids --target.",
            _ => $"check the {sourceLabel} input and retry."
        };

    private static string DisplayTarget(string? targetSqlIdentifier) =>
        string.IsNullOrWhiteSpace(targetSqlIdentifier) ? "<none>" : targetSqlIdentifier;

    private sealed record ResultRowsetBuilder(string Name)
    {
        public List<StoredProcedureResultColumnDeclaration> Columns { get; } = [];

        public StoredProcedureResultRowsetDeclaration ToDeclaration() =>
            new(Name, Columns);
    }
}
