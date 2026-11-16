using Meta.Operations.Domain;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaDataVault.Core;
using MetaRawDataVault;

internal sealed class MetaRawDataVaultCommandHandlers
{
    private static readonly IReadOnlyDictionary<string, AuthoringCommandSpec> AuthoringCommandsByName =
        BuildAuthoringCommands()
            .ToDictionary(static spec => spec.CommandName, StringComparer.OrdinalIgnoreCase);

    private readonly ConsolePresenter presenter;
    private readonly string appName;
    private readonly IRawDataVaultAuthoringService authoringService;

    public MetaRawDataVaultCommandHandlers(
        ConsolePresenter presenter,
        string appName,
        IRawDataVaultAuthoringService authoringService)
    {
        this.presenter = presenter;
        this.appName = appName;
        this.authoringService = authoringService;
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
        await workspaces.CreateAsync("output", authoringService.CreateWorkspace(), RawDataVaultModels.LoadContract(), new MetaRawDataVaultWorkspaceBinding()).ConfigureAwait(false);
        presenter.WriteKeyValueBlock(
            "MetaRawDataVault workspace created",
            [("Path", MetaCliWorkspace.OutputLocation(invocation)), ("Model", "MetaRawDataVault"), ("Rows", "0")]);
    }

    public void RunAddRecord(MetaCliInvocation invocation)
    {
        if (!AuthoringCommandsByName.TryGetValue(invocation.Command.Name, out var spec))
        {
            Fail(
                $"No raw DataVault authoring mapping exists for command '{invocation.Command.Name}'.",
                $"{appName} help");
        }

        var request = new RawDataVaultAuthoringRequest
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
                request.Relationships.Add(new RawDataVaultRelationshipAssignment(
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
                "Cannot update raw DataVault workspace.",
                $"{appName} help {spec.CommandName}",
                4,
                [$"  {ex.Message}"]);
        }
    }

    private static IReadOnlyList<AuthoringCommandSpec> BuildAuthoringCommands()
    {
        var entities = RawDataVaultModels.LoadContract().Entities;
        return entities.OrderBy(entity => entity.Name, StringComparer.Ordinal)
            .Select(BuildAuthoringCommand).ToArray();
    }

    private static AuthoringCommandSpec BuildAuthoringCommand(GenericEntity entity)
    {
        var properties = entity.Properties.OrderBy(property => property.Name, StringComparer.Ordinal)
            .Select(property => new PropertySpec(ToKebabCase(property.Name), property.Name)).ToArray();
        var relationships = entity.Relationships.OrderBy(relationship => relationship.GetColumnName(), StringComparer.Ordinal)
            .Select(relationship => new RelationshipSpec(
                ToRelationshipOptionName(entity.Name, relationship.GetRoleOrDefault()), relationship.GetColumnName(), relationship.Entity)).ToArray();
        return new AuthoringCommandSpec(
            "add-" + ToKebabCase(CommandStem(entity.Name)), entity.Name,
            "exec-add-" + ToKebabCase(entity.Name), properties, relationships );
    }

    private static string CommandStem(string entityName) =>
        entityName.StartsWith("Raw", StringComparison.Ordinal)
            ? entityName[3..]
            : entityName;

    private static string ToRelationshipOptionName(string sourceEntityName, string propertyName)
    {
        var stem = propertyName;
        if (stem.StartsWith("Raw", StringComparison.Ordinal))
        {
            return ToKebabCase(stem[3..]);
        }

        return ToKebabCase(stem);
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

    private sealed record AuthoringCommandSpec(
        string CommandName,
        string EntityName,
        string ExecutableCommandId,
        IReadOnlyList<PropertySpec> Properties,
        IReadOnlyList<RelationshipSpec> Relationships);

    private sealed record PropertySpec(string OptionName, string PropertyName);

    private sealed record RelationshipSpec(
        string OptionName,
        string ColumnName,
        string TargetEntityName);
}
