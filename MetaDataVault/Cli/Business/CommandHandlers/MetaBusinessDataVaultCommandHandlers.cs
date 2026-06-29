using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaBusinessDataVault;
using MetaCli.Core;
using MetaDataVault.Core;

internal sealed class MetaBusinessDataVaultCommandHandlers
{
    private static readonly IReadOnlyDictionary<string, AuthoringCommandSpec> AuthoringCommandsByName =
        BuildAuthoringCommands()
            .ToDictionary(static spec => spec.CommandName, StringComparer.OrdinalIgnoreCase);

    private readonly ConsolePresenter presenter;
    private readonly string appName;
    private readonly IBusinessDataVaultAuthoringService authoringService;

    public MetaBusinessDataVaultCommandHandlers(
        ConsolePresenter presenter,
        string appName,
        IBusinessDataVaultAuthoringService authoringService)
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
                "MetaBusinessDataVault workspace created",
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
                "Cannot create business DataVault workspace.",
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
                $"No business DataVault authoring mapping exists for command '{invocation.Command.Name}'.",
                $"{appName} help");
        }

        var request = new BusinessDataVaultAuthoringRequest
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
                request.Relationships.Add(new BusinessDataVaultRelationshipAssignment(
                    relationship.ColumnName,
                    relationship.TargetEntityName,
                    value));
            }
        }

        if (spec.SupportsDataTypeDetails)
        {
            AddDataTypeDetail(request, invocation, "length", "Length");
            AddDataTypeDetail(request, invocation, "precision", "Precision");
            AddDataTypeDetail(request, invocation, "scale", "Scale");
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
                "Cannot update business DataVault workspace.",
                $"{appName} help {spec.CommandName}",
                4,
                [$"  {ex.Message}"]);
        }
    }

    private static void AddDataTypeDetail(
        BusinessDataVaultAuthoringRequest request,
        MetaCliInvocation invocation,
        string optionName,
        string detailName)
    {
        var value = invocation.Optional(optionName);
        if (!string.IsNullOrWhiteSpace(value))
        {
            request.DataTypeDetails[detailName] = value;
        }
    }

    private static IReadOnlyList<AuthoringCommandSpec> BuildAuthoringCommands()
    {
        var modelType = typeof(MetaBusinessDataVaultModel);
        var allEntityTypes = modelType.Assembly.GetTypes()
            .Where(type => type.Namespace == "MetaBusinessDataVault" &&
                           type.IsClass &&
                           type.GetProperty("Id", BindingFlags.Instance | BindingFlags.Public)?.PropertyType == typeof(string) &&
                           modelType.GetProperty($"{type.Name}List", BindingFlags.Instance | BindingFlags.Public) is not null)
            .OrderBy(static type => type.Name, StringComparer.Ordinal)
            .ToArray();
        var entityNames = allEntityTypes.Select(static type => type.Name).ToHashSet(StringComparer.Ordinal);

        return allEntityTypes
            .Where(static type => !type.Name.EndsWith("DataTypeDetail", StringComparison.Ordinal))
            .Select(type => BuildAuthoringCommand(type, entityNames))
            .ToArray();
    }

    private static AuthoringCommandSpec BuildAuthoringCommand(Type entityType, IReadOnlySet<string> entityNames)
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
            .Select(property => new RelationshipSpec(
                ToRelationshipOptionName(property.Name),
                $"{property.Name}Id",
                RelationshipTargetType(property.PropertyType).Name))
            .ToArray();

        return new AuthoringCommandSpec(
            "add-" + ToKebabCase(CommandStem(entityType.Name)),
            entityType.Name,
            "exec-add-" + ToKebabCase(entityType.Name),
            properties,
            relationships,
            entityNames.Contains($"{entityType.Name}DataTypeDetail"));
    }

    private static bool IsRelationshipProperty(PropertyInfo property)
    {
        if (!property.CanWrite || property.PropertyType == typeof(string))
        {
            return false;
        }

        var targetType = RelationshipTargetType(property.PropertyType);
        return targetType.IsClass && targetType.Namespace == "MetaBusinessDataVault";
    }

    private static Type RelationshipTargetType(Type propertyType) =>
        Nullable.GetUnderlyingType(propertyType) ?? propertyType;

    private static string CommandStem(string entityName) =>
        entityName.StartsWith("Business", StringComparison.Ordinal)
            ? entityName[8..]
            : entityName;

    private static string ToRelationshipOptionName(string propertyName) =>
        propertyName.StartsWith("Business", StringComparison.Ordinal)
            ? ToKebabCase(propertyName[8..])
            : ToKebabCase(propertyName);

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
        IReadOnlyList<RelationshipSpec> Relationships,
        bool SupportsDataTypeDetails);

    private sealed record PropertySpec(string OptionName, string PropertyName);

    private sealed record RelationshipSpec(
        string OptionName,
        string ColumnName,
        string TargetEntityName);
}
