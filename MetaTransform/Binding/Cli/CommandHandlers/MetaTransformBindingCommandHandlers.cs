using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaTransform.Binding;

internal sealed class MetaTransformBindingCommandHandlers
{
    private readonly ConsolePresenter presenter;
    private readonly TransformBindingWorkspaceService service;
    private readonly TransformBindingPartialReportService partialReportService;
    private readonly string appName;

    public MetaTransformBindingCommandHandlers(
        ConsolePresenter presenter,
        TransformBindingWorkspaceService service,
        TransformBindingPartialReportService partialReportService,
        string appName)
    {
        this.presenter = presenter;
        this.service = service;
        this.partialReportService = partialReportService;
        this.appName = appName;
    }

    public void RunBind(MetaCliInvocation invocation)
    {
        var parse = ReadBindArgs(invocation);
        if (!parse.Ok)
        {
            Fail(parse.ErrorMessage, HelpCommand("bind"));
        }

        var targetValidation = CliNewWorkspaceTargetValidator.Validate(parse.NewWorkspacePath);
        if (!targetValidation.Ok)
        {
            Fail(
                targetValidation.ErrorMessage,
                "choose a new folder or empty the target directory and retry.",
                4,
                targetValidation.Details);
        }

        try
        {
            var options = TransformBindingValidationOptions.Create(
                parse.IgnoredTargetColumns,
                parse.IgnoredTargetColumnsIfPresent,
                parse.ExecuteSystemName,
                parse.ExecuteSystemDefaultSchemaName);

            using var activity = CliActivityLine.Start("Binding");
            var result = service.BindValidatedToWorkspace(
                parse.TransformWorkspacePath,
                parse.SourceSchemaWorkspacePaths,
                parse.TargetSchemaWorkspacePath,
                parse.ExecuteSystemName,
                parse.ExecuteSystemDefaultSchemaName,
                targetValidation.FullPath,
                validationOptions: options,
                dataTypeConversionWorkspacePath: parse.DataTypeConversionWorkspacePath,
                allowPartial: parse.AllowPartial);

            var partialReportFullPath = partialReportService.Write(parse.PartialReportPath, result.ObjectIssues ?? []);

            activity.Succeed(FormatBindingActivityResult(result));

            if (parse.AllowPartial && result.SkippedTransformScriptCount > 0)
            {
                WritePartialBindingSummary(result);
                if (!string.IsNullOrWhiteSpace(partialReportFullPath))
                {
                    presenter.WriteInfo($"Partial report: {partialReportFullPath}");
                }
            }
        }
        catch (TransformBindingValidationException ex)
        {
            Fail(
                "Cannot validate binding.",
                "fix the schema or transform contract mismatch and retry.",
                5,
                new[]
                {
                    $"  TransformWorkspace: {Path.GetFullPath(parse.TransformWorkspacePath)}",
                    $"  SourceSchemas: {string.Join(", ", parse.SourceSchemaWorkspacePaths.Select(Path.GetFullPath))}",
                    $"  TargetSchema: {Path.GetFullPath(parse.TargetSchemaWorkspacePath)}",
                    $"  DataTypeConversionWorkspace: {(string.IsNullOrWhiteSpace(parse.DataTypeConversionWorkspacePath) ? "<default>" : Path.GetFullPath(parse.DataTypeConversionWorkspacePath))}",
                    $"  ExecuteSystem: {parse.ExecuteSystemName}",
                    $"  ExecuteSystemDefaultSchemaName: {(string.IsNullOrWhiteSpace(parse.ExecuteSystemDefaultSchemaName) ? "<none>" : parse.ExecuteSystemDefaultSchemaName)}",
                    $"  Code: {ex.Code}",
                    $"  {ex.Message}",
                });
        }
        catch (Exception ex) when (ex is not MetaCliExitException)
        {
            Fail(
                "Cannot create binding workspace.",
                "check transform/source-schema/target-schema inputs and retry.",
                4,
                new[]
                {
                    $"  TransformWorkspace: {Path.GetFullPath(parse.TransformWorkspacePath)}",
                    $"  SourceSchemas: {string.Join(", ", parse.SourceSchemaWorkspacePaths.Select(Path.GetFullPath))}",
                    $"  TargetSchema: {Path.GetFullPath(parse.TargetSchemaWorkspacePath)}",
                    $"  DataTypeConversionWorkspace: {(string.IsNullOrWhiteSpace(parse.DataTypeConversionWorkspacePath) ? "<default>" : Path.GetFullPath(parse.DataTypeConversionWorkspacePath))}",
                    $"  ExecuteSystem: {parse.ExecuteSystemName}",
                    $"  ExecuteSystemDefaultSchemaName: {(string.IsNullOrWhiteSpace(parse.ExecuteSystemDefaultSchemaName) ? "<none>" : parse.ExecuteSystemDefaultSchemaName)}",
                    $"  BindingWorkspace: {targetValidation.FullPath}",
                    $"  {ex.Message}",
                });
        }
    }

    private static BindArgs ReadBindArgs(MetaCliInvocation invocation)
    {
        var allowPartial = invocation.Flag("allow-partial");
        var partialReportPath = invocation.Optional("partial-report") ?? string.Empty;
        if (!allowPartial && !string.IsNullOrWhiteSpace(partialReportPath))
        {
            return BindArgs.Fail("--partial-report requires --allow-partial.");
        }

        var ignoredTargetColumns = ReadColumnList(
            invocation.Values("ignore-target-columns"),
            "--ignore-target-columns",
            out var ignoredTargetColumnsError);
        if (!string.IsNullOrWhiteSpace(ignoredTargetColumnsError))
        {
            return BindArgs.Fail(ignoredTargetColumnsError);
        }

        var ignoredTargetColumnsIfPresent = ReadColumnList(
            invocation.Values("ignore-target-columns-if-present"),
            "--ignore-target-columns-if-present",
            out var ignoredTargetColumnsIfPresentError);
        if (!string.IsNullOrWhiteSpace(ignoredTargetColumnsIfPresentError))
        {
            return BindArgs.Fail(ignoredTargetColumnsIfPresentError);
        }

        return new BindArgs(
            true,
            invocation.Required("transform-workspace"),
            invocation.Values("source-schema").ToArray(),
            invocation.Required("target-schema"),
            invocation.Required("execute-system"),
            invocation.Optional("execute-system-default-schema-name") ?? string.Empty,
            invocation.Required("new-workspace"),
            ignoredTargetColumns,
            ignoredTargetColumnsIfPresent,
            invocation.Optional("data-type-conversion-workspace") ?? string.Empty,
            allowPartial,
            partialReportPath,
            string.Empty);
    }

    private static string[] ReadColumnList(
        IReadOnlyList<string> rawValues,
        string optionName,
        out string errorMessage)
    {
        var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var rawValue in rawValues)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                errorMessage = $"value for {optionName} cannot be blank.";
                return [];
            }

            foreach (var column in rawValue.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                columns.Add(column);
            }

            if (columns.Count == 0)
            {
                errorMessage = $"value for {optionName} must include at least one column name.";
                return [];
            }
        }

        errorMessage = string.Empty;
        return columns.ToArray();
    }

    private static string FormatBindingActivityResult(BindToWorkspaceResult result)
    {
        if (result.SkippedTransformScriptCount == 0)
        {
            return "Ok";
        }

        return $"Partial: {result.TransformBindingCount}/{result.TransformScriptCount} bound; {result.SkippedTransformScriptCount} skipped due to binding or validation failures";
    }

    private void WritePartialBindingSummary(BindToWorkspaceResult result)
    {
        var issues = result.ObjectIssues ?? [];
        presenter.WriteInfo($"Skipped transform scripts due to binding or validation failures: {result.SkippedTransformScriptCount}");
        foreach (var group in issues
                     .GroupBy(static issue => issue.Stage, StringComparer.OrdinalIgnoreCase)
                     .OrderBy(static group => GetStageOrder(group.Key))
                     .ThenBy(static group => group.Key, StringComparer.OrdinalIgnoreCase))
        {
            presenter.WriteInfo($"  {FormatStageFailureLabel(group.Key)}: {group.Count()}");
        }
    }

    private static int GetStageOrder(string stage) =>
        string.Equals(stage, "Binding", StringComparison.OrdinalIgnoreCase) ? 0 :
        string.Equals(stage, "Validation", StringComparison.OrdinalIgnoreCase) ? 1 :
        2;

    private static string FormatStageFailureLabel(string stage) =>
        string.Equals(stage, "Binding", StringComparison.OrdinalIgnoreCase) ? "Binding failures" :
        string.Equals(stage, "Validation", StringComparison.OrdinalIgnoreCase) ? "Validation failures" :
        $"{stage} failures";

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

    private sealed record BindArgs(
        bool Ok,
        string TransformWorkspacePath,
        string[] SourceSchemaWorkspacePaths,
        string TargetSchemaWorkspacePath,
        string ExecuteSystemName,
        string ExecuteSystemDefaultSchemaName,
        string NewWorkspacePath,
        string[] IgnoredTargetColumns,
        string[] IgnoredTargetColumnsIfPresent,
        string DataTypeConversionWorkspacePath,
        bool AllowPartial,
        string PartialReportPath,
        string ErrorMessage)
    {
        public static BindArgs Fail(string message) =>
            new(
                false,
                string.Empty,
                [],
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                [],
                [],
                string.Empty,
                false,
                string.Empty,
                message);
    }
}
