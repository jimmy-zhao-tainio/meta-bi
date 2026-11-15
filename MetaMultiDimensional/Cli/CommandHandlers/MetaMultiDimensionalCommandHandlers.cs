using Meta.Operations.Domain;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaMultiDimensional;
using MetaMultiDimensional.Core;
using MetaMultiDimensional.Core.Deploy;

internal sealed class MetaMultiDimensionalCommandHandlers
{
    private static readonly IReadOnlyDictionary<string, AuthoringCommandSpec> AuthoringCommandsByName =
        BuildAuthoringCommands()
            .ToDictionary(static spec => spec.CommandName, StringComparer.OrdinalIgnoreCase);

    private readonly ConsolePresenter presenter;
    private readonly string appName;
    private readonly IMultiDimensionalAuthoringService authoringService;
    private readonly MetaMultiDimensionalDeployService deployService;
    private readonly MetaMultiDimensionalRestoreService restoreService;
    private readonly MetaMultiDimensionalDropService dropService;

    public MetaMultiDimensionalCommandHandlers(
        ConsolePresenter presenter,
        string appName,
        IMultiDimensionalAuthoringService authoringService,
        MetaMultiDimensionalDeployService deployService,
        MetaMultiDimensionalRestoreService restoreService,
        MetaMultiDimensionalDropService dropService)
    {
        this.presenter = presenter;
        this.appName = appName;
        this.authoringService = authoringService;
        this.deployService = deployService;
        this.restoreService = restoreService;
        this.dropService = dropService;
    }

    public static IReadOnlyList<string> AuthoringExecutableCommandIds =>
        AuthoringCommandsByName.Values
            .OrderBy(static spec => spec.CommandName, StringComparer.Ordinal)
            .Select(static spec => spec.ExecutableCommandId)
            .ToArray();

    public async Task RunCreate(
        MetaCliInvocation invocation,
        MetaCliWorkspaces workspaces)
    {
        await workspaces.CreateAsync("output", authoringService.CreateWorkspace(), MultiDimensionalModels.LoadContract(), new MetaMultiDimensionalWorkspaceBinding()).ConfigureAwait(false);
        presenter.WriteKeyValueBlock(
            "MetaMultiDimensional workspace created",
            [("Path", MetaCliWorkspace.OutputLocation(invocation)), ("Model", "MetaMultiDimensional"), ("Rows", "0")]);
    }

    public void RunAddRecord(MetaCliInvocation invocation)
    {
        if (!AuthoringCommandsByName.TryGetValue(invocation.Command.Name, out var spec))
        {
            Fail(
                $"No multidimensional authoring mapping exists for command '{invocation.Command.Name}'.",
                $"{appName} help");
        }

        var request = new MultiDimensionalAuthoringRequest
        {
            WorkspacePath = Path.GetFullPath(invocation.Optional("workspace") ?? Directory.GetCurrentDirectory()),
            EntityName = spec.EntityName,
            RecordId = invocation.Required("id"),
        };

        foreach (var property in spec.Properties)
        {
            var value = invocation.Optional(property.OptionName);
            if (!string.IsNullOrWhiteSpace(value))
            {
                request.Values[property.PropertyName] = value;
            }
        }

        foreach (var relationship in spec.Relationships)
        {
            var value = invocation.Optional(relationship.OptionName);
            if (!string.IsNullOrWhiteSpace(value))
            {
                request.Relationships.Add(new MultiDimensionalRelationshipAssignment(
                    relationship.ColumnName,
                    relationship.TargetEntityName,
                    value));
            }
        }

        try
        {
            authoringService.AddRecord(request);
            presenter.WriteOk($"Added {request.RecordId} to {spec.EntityName}");
        }
        catch (Exception ex) when (ex is not MetaCliExitException and
                                   (InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException))
        {
            Fail(
                "Cannot update multidimensional workspace.",
                $"{appName} help {spec.CommandName}",
                4,
                [$"  {ex.Message}"]);
        }
    }

    public void RunDeploy(MetaCliInvocation invocation)
    {
        try
        {
            var result = deployService
                .DeployAsync(new MetaMultiDimensionalDeployRequest
                {
                    WorkspacePath = Path.GetFullPath(invocation.Optional("workspace") ?? Directory.GetCurrentDirectory()),
                    Server = invocation.Required("server"),
                    DatabaseName = invocation.Optional("database-name"),
                    DataTypeWorkspacePath = invocation.Optional("data-type-workspace"),
                    DropExisting = invocation.Flag("drop-existing"),
                    Process = !invocation.Flag("no-process"),
                })
                .GetAwaiter()
                .GetResult();

            presenter.WriteOk($"Deployed {result.DatabaseName} to {result.Server}");
            presenter.WriteInfo(result.DropExisting ? "Mode: drop, create" : "Mode: create");
            presenter.WriteInfo(result.Processed ? "Process: full" : "Process: skipped");
            presenter.WriteInfo($"Cubes: {result.CubeCount}");
            presenter.WriteInfo($"Dimensions: {result.DimensionCount}");
            presenter.WriteInfo($"Measure groups: {result.MeasureGroupCount}");
            presenter.WriteInfo($"Measures: {result.MeasureCount}");
        }
        catch (Exception ex) when (ex is not MetaCliExitException && IsExpectedDeployException(ex))
        {
            Fail(
                "Cannot deploy multidimensional database.",
                $"{appName} help deploy",
                4,
                [$"  {ex.Message}"]);
        }
    }

    public void RunRestore(MetaCliInvocation invocation)
    {
        try
        {
            var result = restoreService
                .RestoreAsync(new MetaMultiDimensionalRestoreRequest
                {
                    SourceServer = invocation.Required("source-server"),
                    SourceDatabaseName = invocation.Required("source-database-name"),
                    TargetServer = invocation.Required("target-server"),
                    TargetDatabaseName = invocation.Required("target-database-name"),
                    BackupFile = invocation.Required("backup-file"),
                    DropExisting = invocation.Flag("drop-existing"),
                    OverwriteBackupFile = invocation.Flag("overwrite-backup-file"),
                })
                .GetAwaiter()
                .GetResult();

            presenter.WriteOk($"Restored {result.TargetDatabaseName} to {result.TargetServer}");
            presenter.WriteInfo($"Source: {result.SourceServer}/{result.SourceDatabaseName}");
            presenter.WriteInfo($"Backup file: {result.BackupFile}");
            presenter.WriteInfo(result.DroppedExisting ? "Mode: drop target, restore" : "Mode: restore new target");
            presenter.WriteInfo(result.OverwriteBackupFile ? "Backup file mode: overwrite" : "Backup file mode: create");
        }
        catch (Exception ex) when (ex is not MetaCliExitException && IsExpectedDeployException(ex))
        {
            Fail(
                "Cannot restore multidimensional database.",
                $"{appName} help restore",
                4,
                [$"  {ex.Message}"]);
        }
    }

    public void RunDrop(MetaCliInvocation invocation)
    {
        try
        {
            var result = dropService
                .DropAsync(new MetaMultiDimensionalDropRequest
                {
                    Server = invocation.Required("server"),
                    DatabaseName = invocation.Required("database-name"),
                })
                .GetAwaiter()
                .GetResult();

            presenter.WriteOk($"Dropped {result.DatabaseName} from {result.Server}");
        }
        catch (Exception ex) when (ex is not MetaCliExitException && IsExpectedDeployException(ex))
        {
            Fail(
                "Cannot drop multidimensional database.",
                $"{appName} help drop",
                4,
                [$"  {ex.Message}"]);
        }
    }

    private static IReadOnlyList<AuthoringCommandSpec> BuildAuthoringCommands()
    {
        var entities = MultiDimensionalModels.LoadContract().Entities;
        return entities.OrderBy(entity => entity.Name, StringComparer.Ordinal)
            .Select(BuildAuthoringCommand).ToArray();
    }

    private static AuthoringCommandSpec BuildAuthoringCommand(GenericEntity entity)
    {
        var properties = entity.Properties.OrderBy(property => property.Name, StringComparer.Ordinal)
            .Select(property => new PropertySpec(ToKebabCase(property.Name), property.Name)).ToArray();
        var relationships = entity.Relationships.OrderBy(relationship => relationship.GetColumnName(), StringComparer.Ordinal)
            .Select(relationship => new RelationshipSpec(
                ToKebabCase(relationship.GetRoleOrDefault()), relationship.GetColumnName(), relationship.Entity)).ToArray();
        return new AuthoringCommandSpec(
            "add-" + ToKebabCase(entity.Name), entity.Name, properties, relationships );
    }

    private static string ToKebabCase(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var builder = new StringBuilder(value.Length + 8);
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (char.IsUpper(c))
            {
                if (i > 0 && builder.Length > 0 && builder[^1] != '-')
                {
                    builder.Append('-');
                }

                builder.Append(char.ToLowerInvariant(c));
                continue;
            }

            if (c is '_' or ' ')
            {
                if (builder.Length > 0 && builder[^1] != '-')
                {
                    builder.Append('-');
                }

                continue;
            }

            builder.Append(c);
        }

        return builder.ToString();
    }

    [DoesNotReturn]
    private void Fail(string message, string next, int exitCode = 1, IEnumerable<string>? details = null)
    {
        var renderedDetails = new List<string>();
        if (details is not null)
        {
            renderedDetails.AddRange(details.Where(static item => !string.IsNullOrWhiteSpace(item)));
        }

        renderedDetails.Add($"Next: {next}");
        presenter.WriteFailure(message, renderedDetails);
        throw new MetaCliExitException(exitCode);
    }

    private static bool IsExpectedDeployException(Exception ex)
    {
        return ex is ArgumentException or InvalidOperationException or IOException or UnauthorizedAccessException ||
               ex is Microsoft.AnalysisServices.AmoException or Microsoft.AnalysisServices.ConnectionException;
    }

    private sealed record AuthoringCommandSpec(
        string CommandName,
        string EntityName,
        IReadOnlyList<PropertySpec> Properties,
        IReadOnlyList<RelationshipSpec> Relationships)
    {
        public string ExecutableCommandId => $"exec-{CommandName}";
    }

    private sealed record PropertySpec(string OptionName, string PropertyName);

    private sealed record RelationshipSpec(
        string OptionName,
        string ColumnName,
        string TargetEntityName);
}
