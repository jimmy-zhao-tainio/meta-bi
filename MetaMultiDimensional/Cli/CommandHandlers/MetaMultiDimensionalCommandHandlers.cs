using System.Diagnostics.CodeAnalysis;
using System.Reflection;
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

    public void RunNewWorkspace(MetaCliInvocation invocation)
    {
        var targetValidation = CliNewWorkspaceTargetValidator.Validate(invocation.Required("path"));
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
            var result = authoringService.CreateWorkspace(targetValidation.FullPath);
            presenter.WriteKeyValueBlock(
                "MetaMultiDimensional workspace created",
                new[]
                {
                    ("Path", result.WorkspacePath),
                    ("Model", result.ModelName),
                    ("Rows", result.RowCount.ToString())
                });
        }
        catch (Exception ex) when (ex is not MetaCliExitException and
                                   (InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException))
        {
            Fail(
                "Cannot create multidimensional workspace.",
                "choose a new folder or empty the target directory and retry.",
                4,
                [$"  {ex.Message}"]);
        }
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
        var modelType = typeof(MetaMultiDimensionalModel);
        return modelType.Assembly.GetTypes()
            .Where(type => type.Namespace == "MetaMultiDimensional" &&
                           type.IsClass &&
                           type.GetProperty("Id", BindingFlags.Instance | BindingFlags.Public)?.PropertyType == typeof(string) &&
                           modelType.GetProperty($"{type.Name}List", BindingFlags.Instance | BindingFlags.Public) is not null)
            .OrderBy(static type => type.Name, StringComparer.Ordinal)
            .Select(BuildAuthoringCommand)
            .ToArray();
    }

    private static AuthoringCommandSpec BuildAuthoringCommand(Type entityType)
    {
        var properties = entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(static property => property.CanWrite &&
                                      property.PropertyType == typeof(string) &&
                                      !string.Equals(property.Name, "Id", StringComparison.Ordinal))
            .OrderBy(static property => property.Name, StringComparer.Ordinal)
            .Select(static property => new PropertySpec(ToKebabCase(property.Name), property.Name))
            .ToArray();

        var relationships = entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(IsRelationshipProperty)
            .OrderBy(static property => $"{property.Name}Id", StringComparer.Ordinal)
            .Select(static property => new RelationshipSpec(
                ToKebabCase(property.Name),
                $"{property.Name}Id",
                property.PropertyType.Name))
            .ToArray();

        return new AuthoringCommandSpec(
            "add-" + ToKebabCase(entityType.Name),
            entityType.Name,
            properties,
            relationships);
    }

    private static bool IsRelationshipProperty(PropertyInfo property)
    {
        if (!property.CanWrite || property.PropertyType == typeof(string))
        {
            return false;
        }

        if (!property.PropertyType.IsClass)
        {
            return false;
        }

        return property.PropertyType.Namespace == "MetaMultiDimensional";
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
        var fullName = ex.GetType().FullName ?? string.Empty;
        return ex is ArgumentException or InvalidOperationException or IOException or UnauthorizedAccessException ||
               fullName.Contains("Adomd", StringComparison.OrdinalIgnoreCase) ||
               fullName.Contains("AnalysisServices", StringComparison.OrdinalIgnoreCase);
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
