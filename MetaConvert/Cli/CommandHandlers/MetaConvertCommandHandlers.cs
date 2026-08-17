using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using Meta.Integration;
using MetaCli.Core;
using MetaConvert.AnalyticsToMultiDimensional;
using MetaConvert.AnalyticsToTabular;
using MetaConvert.DataQualityToSql;
using MetaConvert.DataVaultToSql;
using MetaConvert.DataWarehouseToSql;
using MetaConvert.SchemaToDataVault;
using MetaConvert.SqlToTransformScript;
using MetaConvert.TransformScriptToSql;
using MetaSchema;

internal sealed class MetaConvertCommandHandlers
{
    private readonly ConsolePresenter presenter;
    private readonly string appName;

    public MetaConvertCommandHandlers(ConsolePresenter presenter, string appName)
    {
        this.presenter = presenter;
        this.appName = appName;
    }

    public async Task<int> RunSchemaToRawDataVaultAsync(
        MetaCliInvocation invocation,
        MetaCliWorkspaces workspaces)
    {
        var sourceWorkspace = invocation.Required("source-workspace");
        var targetWorkspace = MetaCliWorkspace.OutputLocation(
            invocation,
            "output-xml",
            "output-csharp",
            "output-sql");

        RawDataVaultFromMetaSchemaService.RawDataVaultFromMetaSchemaResult result;
        try
        {
            result = await RunWithProgressAsync(
                "Converting schema to raw Data Vault",
                async () =>
                {
                    var sourceModel = await workspaces
                        .RequiredAsync<MetaSchemaModel>("source-workspace")
                        .ConfigureAwait(false);

                    var converted = new RawDataVaultFromMetaSchemaService().MaterializeWithReport(
                        sourceModel,
                        invocation.Values("ignore-field-name").ToList(),
                        invocation.Values("ignore-field-suffix").ToList(),
                        invocation.Flag("include-views"));

                    await workspaces.CreateAsync(
                            "output",
                            converted.Model)
                        .ConfigureAwait(false);
                    return converted;
                }).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot convert schema to raw DataVault.",
                "check source workspace, output path, and options, then retry.",
                4,
                new[]
                {
                    $"  Source workspace: {sourceWorkspace}",
                    $"  Target workspace: {targetWorkspace}",
                    $"  {ex.Message}",
                });
        }

        presenter.WriteOk($"Created {targetWorkspace}");
        if (invocation.Flag("verbose"))
        {
            RenderSummary(result.Report.Summary);
        }

        return 0;
    }

    public async Task<int> RunRawDataVaultToSqlAsync(MetaCliInvocation invocation, MetaCliWorkspaces workspaces)
    {
        var request = ReadDataVaultToSqlRequest(invocation);
        try
        {
            _ = await RunWithProgressAsync(
                "Converting raw Data Vault to SQL",
                async () =>
                {
                    var result = await Converter.ConvertAsync(
                        request.WorkspacePath,
                        request.ImplementationWorkspacePath,
                        request.DatabaseName).ConfigureAwait(false);
                    await workspaces.CreateAsync("output", result).ConfigureAwait(false);
                    return result;
                }).ConfigureAwait(false);

            presenter.WriteOk($"Generated {Path.GetFileName(MetaCliWorkspace.OutputLocation(invocation, "output-xml", "output-csharp", "output-sql"))}");
            return 0;
        }
        catch (Exception ex)
        {
            return FailDataVaultToSql(invocation, "Cannot convert raw DataVault to SQL.", "raw-datavault-to-sql", request, ex);
        }
    }

    public async Task<int> RunBusinessDataVaultToSqlAsync(MetaCliInvocation invocation, MetaCliWorkspaces workspaces)
    {
        var request = ReadDataVaultToSqlRequest(invocation);
        try
        {
            _ = await RunWithProgressAsync(
                "Converting business Data Vault to SQL",
                async () =>
                {
                    var result = await Converter.ConvertAsync(
                        request.WorkspacePath,
                        request.ImplementationWorkspacePath,
                        request.DatabaseName).ConfigureAwait(false);
                    await workspaces.CreateAsync("output", result).ConfigureAwait(false);
                    return result;
                }).ConfigureAwait(false);

            presenter.WriteOk($"Generated {Path.GetFileName(MetaCliWorkspace.OutputLocation(invocation, "output-xml", "output-csharp", "output-sql"))}");
            return 0;
        }
        catch (Exception ex)
        {
            return FailDataVaultToSql(invocation, "Cannot convert business DataVault to SQL.", "business-datavault-to-sql", request, ex);
        }
    }

    public async Task<int> RunDataQualityToSqlAsync(MetaCliInvocation invocation)
    {
        var workspacePath = ReadWorkspacePath(invocation);
        var outputPath = Path.GetFullPath(invocation.Required("out"));

        try
        {
            var result = await RunWithProgressAsync(
                "Converting data quality to SQL",
                () => Task.FromResult(new DataQualityToSqlConverter().Convert(workspacePath, outputPath)))
                .ConfigureAwait(false);
            presenter.WriteInfo(
                $"Generated {result.CandidateViewCount} data quality view script{(result.CandidateViewCount == 1 ? string.Empty : "s")}, " +
                $"{result.DashboardViewCount} review dashboard, and MetaDQ operational SQL ({result.OperationalTableCount} tables, {result.OperationalProcedureCount} procedure).");
            presenter.WriteKeyValueBlock("Output", new[]
            {
                ("DataQualityViews", result.CandidateViewCount.ToString()),
                ("Dashboard", "dq.v_DataQualityReview"),
                ("MetaDQTables", result.OperationalTableCount.ToString()),
                ("MetaDQProcedures", result.OperationalProcedureCount.ToString()),
                ("Scripts", result.ScriptCount.ToString()),
                ("Path", result.OutputPath),
            });
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot convert data-quality workspace to SQL.",
                "check the workspace, promoted candidates, and output path, then retry.",
                4,
                new[]
                {
                    $"  Workspace: {workspacePath}",
                    $"  Output: {outputPath}",
                    $"  {ex.Message}",
                });
        }
    }

    public async Task<int> RunDataWarehouseToSqlAsync(MetaCliInvocation invocation, MetaCliWorkspaces workspaces)
    {
        var request = ReadDataVaultToSqlRequest(invocation);
        try
        {
            _ = await RunWithMeterAsync(
                async meter =>
                {
                    var result = await DataWarehouseToSqlConverter.ConvertAsync(
                        request.WorkspacePath,
                        request.ImplementationWorkspacePath,
                        request.DatabaseName,
                        cancellationToken: default,
                        progress: meter is null
                            ? null
                            : value => meter.Report(
                                value.CompletedTaskCount,
                                value.TotalTaskCount,
                                FormatWeaveTask(
                                    value.CompletedTaskKind?.ToString(),
                                    value.CompletedTaskName))).ConfigureAwait(false);
                    await workspaces.CreateAsync("output", result).ConfigureAwait(false);
                    return result;
                }).ConfigureAwait(false);

            presenter.WriteOk($"Generated {Path.GetFileName(MetaCliWorkspace.OutputLocation(invocation, "output-xml", "output-csharp", "output-sql"))}");
            return 0;
        }
        catch (Exception ex)
        {
            return FailDataVaultToSql(invocation, "Cannot convert data warehouse to SQL.", "data-warehouse-to-sql", request, ex);
        }
    }

    public async Task<int> RunTransformScriptToSqlAsync(MetaCliInvocation invocation, MetaCliWorkspaces workspaces)
    {
        var workspacePath = ReadWorkspacePath(invocation);
        var databaseName = invocation.Required("database-name");

        try
        {
            _ = await RunWithMeterAsync(
                async meter =>
                {
                    var result = await TransformScriptToSqlConverter.ConvertAsync(
                        workspacePath,
                        databaseName,
                        progress: meter is null
                            ? null
                            : value => meter.Report(
                                value.CompletedTaskCount,
                                value.TotalTaskCount,
                                FormatWeaveTask(
                                    value.CompletedTaskKind?.ToString(),
                                    value.CompletedTaskName))).ConfigureAwait(false);
                    await workspaces.CreateAsync("output", result).ConfigureAwait(false);
                    return result;
                }).ConfigureAwait(false);

            presenter.WriteInfo($"Generated {Path.GetFileName(MetaCliWorkspace.OutputLocation(invocation, "output-xml", "output-csharp", "output-sql"))}");
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot convert transform scripts to SQL.",
                "check the workspace, database name, schema-qualified module declarations, and output path, then retry.",
                4,
                new[]
                {
                    $"  Workspace: {workspacePath}",
                    $"  Database: {databaseName}",
                    $"  Output: {MetaCliWorkspace.OutputLocation(invocation, "output-xml", "output-csharp", "output-sql")}",
                    $"  {ex.Message}",
                });
        }
    }

    public async Task<int> RunSqlToTransformScriptAsync(MetaCliInvocation invocation, MetaCliWorkspaces workspaces)
    {
        var workspacePath = ReadWorkspacePath(invocation);

        try
        {
            var result = await RunWithProgressAsync(
                "Converting SQL to transform scripts",
                async () =>
                {
                    var converted = await SqlToTransformScriptConverter.ConvertAsync(
                        workspacePath,
                        new SqlToTransformScriptConversionOptions
                        {
                            ModuleKinds = ReadSqlModuleKinds(invocation),
                            AllowEmpty = invocation.Flag("allow-empty"),
                        }).ConfigureAwait(false);
                    await workspaces.CreateAsync("output", converted.Workspace).ConfigureAwait(false);
                    return converted;
                }).ConfigureAwait(false);

            presenter.WriteInfo($"Generated {Path.GetFileName(MetaCliWorkspace.OutputLocation(invocation, "output-xml", "output-csharp", "output-sql"))}");
            presenter.WriteKeyValueBlock("Summary", new[]
            {
                ("Views", result.ViewCount.ToString()),
                ("Functions", result.FunctionCount.ToString()),
                ("StoredProcedures", result.StoredProcedureCount.ToString()),
            });
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot convert MetaSql to transform scripts.",
                "check the MetaSql workspace, module definitions, and output path, then retry.",
                4,
                new[]
                {
                    $"  Workspace: {workspacePath}",
                    $"  Output: {MetaCliWorkspace.OutputLocation(invocation, "output-xml", "output-csharp", "output-sql")}",
                    $"  {ex.Message}",
                });
        }
    }

    public async Task<int> RunAnalyticsToTabularAsync(MetaCliInvocation invocation, MetaCliWorkspaces workspaces)
    {
        var workspacePath = ReadWorkspacePath(invocation);

        try
        {
            var result = await RunWithMeterAsync(
                async meter =>
                {
                    var source = TypedWorkspaceModelMapper.Load<MetaAnalytics.MetaAnalyticsModel>(workspacePath, searchUpward: false);
                    var converted = AnalyticsToTabularConverter.Convert(
                        source,
                        meter is null
                            ? null
                            : value => meter.Report(
                                value.CompletedTaskCount,
                                value.TotalTaskCount,
                                FormatWeaveTask(
                                    value.CompletedTaskKind?.ToString(),
                                    value.CompletedTaskName)));
                    await workspaces.CreateAsync("output", converted).ConfigureAwait(false);
                    return converted;
                }).ConfigureAwait(false);
            presenter.WriteOk($"Generated {Path.GetFileName(MetaCliWorkspace.OutputLocation(invocation, "output-xml", "output-csharp", "output-sql"))}");
            presenter.WriteKeyValueBlock("Summary", new[]
            {
                ("Tables", result.TabularTableList.Count.ToString()),
                ("Columns", result.TabularColumnList.Count.ToString()),
                ("Measures", result.TabularMeasureList.Count.ToString()),
            });
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot convert analytics to tabular.",
                "check the workspace, expression languages, and output path, then retry.",
                4,
                new[]
                {
                    $"  Workspace: {workspacePath}",
                    $"  Output: {MetaCliWorkspace.OutputLocation(invocation, "output-xml", "output-csharp", "output-sql")}",
                    $"  {ex.Message}",
                });
        }
    }

    public async Task<int> RunAnalyticsToMultiDimensionalAsync(MetaCliInvocation invocation, MetaCliWorkspaces workspaces)
    {
        var workspacePath = ReadWorkspacePath(invocation);

        try
        {
            var result = await RunWithMeterAsync(
                async meter =>
                {
                    var source = TypedWorkspaceModelMapper.Load<MetaAnalytics.MetaAnalyticsModel>(workspacePath, searchUpward: false);
                    var converted = AnalyticsToMultiDimensionalConverter.Convert(
                        source,
                        meter is null
                            ? null
                            : value => meter.Report(
                                value.CompletedTaskCount,
                                value.TotalTaskCount,
                                FormatWeaveTask(
                                    value.CompletedTaskKind?.ToString(),
                                    value.CompletedTaskName)));
                    await workspaces.CreateAsync("output", converted).ConfigureAwait(false);
                    return converted;
                }).ConfigureAwait(false);
            presenter.WriteOk($"Generated {Path.GetFileName(MetaCliWorkspace.OutputLocation(invocation, "output-xml", "output-csharp", "output-sql"))}");
            presenter.WriteKeyValueBlock("Summary", new[]
            {
                ("Cubes", result.CubeList.Count.ToString()),
                ("Dimensions", result.DimensionList.Count.ToString()),
                ("MeasureGroups", result.MeasureGroupList.Count.ToString()),
                ("Measures", result.MeasureList.Count.ToString()),
            });
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot convert analytics to multidimensional.",
                "check the workspace, target-specific expressions/security, and output path, then retry.",
                4,
                new[]
                {
                    $"  Workspace: {workspacePath}",
                    $"  Output: {MetaCliWorkspace.OutputLocation(invocation, "output-xml", "output-csharp", "output-sql")}",
                    $"  {ex.Message}",
                });
        }
    }

    private static DataVaultToSqlRequest ReadDataVaultToSqlRequest(MetaCliInvocation invocation)
    {
        return new DataVaultToSqlRequest(
            WorkspacePath: ReadWorkspacePath(invocation),
            ImplementationWorkspacePath: Path.GetFullPath(invocation.Required("implementation-workspace")),
            DatabaseName: invocation.Required("database-name"));
    }

    private int FailDataVaultToSql(
        MetaCliInvocation invocation,
        string message,
        string commandName,
        DataVaultToSqlRequest request,
        Exception exception)
    {
        return Fail(
            message,
            "check the workspace, implementation workspace, and database name, then retry.",
            4,
            new[]
            {
                $"  Workspace: {request.WorkspacePath}",
                $"  Database: {request.DatabaseName}",
                $"  Output: {MetaCliWorkspace.OutputLocation(invocation, "output-xml", "output-csharp", "output-sql")}",
                $"  {exception.Message}",
            });
    }

    private static string ReadWorkspacePath(MetaCliInvocation invocation) =>
        Path.GetFullPath(invocation.Optional("workspace") ?? ".");

    private static SqlToTransformScriptModuleKinds ReadSqlModuleKinds(MetaCliInvocation invocation)
    {
        var includeSwitchProvided = invocation.Flag("include-views") ||
                                    invocation.Flag("include-functions") ||
                                    invocation.Flag("include-stored-procedures");
        if (!includeSwitchProvided)
        {
            return SqlToTransformScriptModuleKinds.All;
        }

        var moduleKinds = SqlToTransformScriptModuleKinds.None;
        if (invocation.Flag("include-views")) moduleKinds |= SqlToTransformScriptModuleKinds.Views;
        if (invocation.Flag("include-functions")) moduleKinds |= SqlToTransformScriptModuleKinds.Functions;
        if (invocation.Flag("include-stored-procedures")) moduleKinds |= SqlToTransformScriptModuleKinds.StoredProcedures;
        return moduleKinds;
    }

    private static async Task<T> RunWithProgressAsync<T>(
        string activity,
        Func<Task<T>> action)
        => await RunWithProgressAsync(activity, _ => action()).ConfigureAwait(false);

    private static async Task<T> RunWithProgressAsync<T>(
        string activity,
        Func<CliActivityLine, Task<T>> action)
    {
        using var progress = CliActivityLine.Start(activity);
        return await action(progress).ConfigureAwait(false);
    }

    private static async Task<T> RunWithMeterAsync<T>(
        Func<MetaCliProgressMeter?, Task<T>> action)
    {
        using var meter = MetaCliProgressMeter.TryStart(initialDetail: "preparing");
        try
        {
            var result = await action(meter).ConfigureAwait(false);
            meter?.Succeed();
            return result;
        }
        catch
        {
            meter?.Fail();
            throw;
        }
    }

    private static string? FormatWeaveTask(string? taskKind, string? taskName)
    {
        if (string.IsNullOrWhiteSpace(taskName))
        {
            return null;
        }

        return taskKind switch
        {
            "Requirement" => $"requirement {taskName}",
            "Relation" => $"relation {taskName}",
            "TargetEntity" => $"target {taskName}",
            _ => taskName,
        };
    }

    private void RenderSummary(RawDataVaultFromMetaSchemaSummary summary)
    {
        presenter.WriteInfo(string.Empty);
        presenter.WriteKeyValueBlock("Summary", new[]
        {
            ("Source systems", summary.SourceSystemCount.ToString()),
            ("Source schemas", summary.SourceSchemaCount.ToString()),
            ("Source tables", summary.SourceTableCount.ToString()),
            ("Source relationships", summary.SourceRelationshipCount.ToString()),
            ("Raw hubs", summary.RawHubCount.ToString()),
            ("Raw hub key parts", summary.RawHubKeyPartCount.ToString()),
            ("Raw links", summary.RawLinkCount.ToString()),
            ("Raw hub satellites", summary.RawHubSatelliteCount.ToString()),
            ("Raw hub satellite attributes", summary.RawHubSatelliteAttributeCount.ToString()),
            ("Ignored field names", FormatSummaryList(summary.IgnoredFieldNames)),
            ("Ignored field suffixes", FormatSummaryList(summary.IgnoredFieldSuffixes)),
            ("Included views", summary.IncludeViews ? "yes" : "no"),
        });
    }

    private static string FormatSummaryList(IEnumerable<string> values)
    {
        var materialized = values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
            .ThenBy(value => value, StringComparer.Ordinal)
            .ToList();

        return materialized.Count == 0
            ? "(none)"
            : string.Join(", ", materialized);
    }

    private string HelpCommand(string commandName) => $"{appName} help {commandName}";

    private int Fail(string message, string next, int exitCode = 1, IEnumerable<string>? details = null)
    {
        var renderedDetails = new List<string>();
        if (details != null)
        {
            renderedDetails.AddRange(details.Where(static detail => !string.IsNullOrWhiteSpace(detail)));
        }

        renderedDetails.Add($"Next: {next}");
        presenter.WriteFailure(message, renderedDetails);
        return exitCode;
    }

    private sealed record DataVaultToSqlRequest(
        string WorkspacePath,
        string ImplementationWorkspacePath,
        string DatabaseName);
}
