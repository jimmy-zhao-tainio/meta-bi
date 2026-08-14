using System.Diagnostics.CodeAnalysis;
using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaAnalytics.Core;
using MetaCli.Core;

internal sealed class MetaAnalyticsCommandHandlers
{
    private static readonly IReadOnlyDictionary<string, AuthoringCommandSpec> AuthoringCommandsByName =
        BuildAuthoringCommands()
            .ToDictionary(static spec => spec.CommandName, StringComparer.OrdinalIgnoreCase);

    private readonly ConsolePresenter presenter;
    private readonly string appName;
    private readonly IAnalyticsAuthoringService service;

    public MetaAnalyticsCommandHandlers(
        ConsolePresenter presenter,
        string appName,
        IAnalyticsAuthoringService service)
    {
        this.presenter = presenter;
        this.appName = appName;
        this.service = service;
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
        await workspaces.CreateAsync("output", service.CreateWorkspace()).ConfigureAwait(false);
        presenter.WriteKeyValueBlock(
            "MetaAnalytics workspace created",
            [("Path", MetaCliWorkspace.OutputLocation(invocation)), ("Model", "MetaAnalytics"), ("Rows", "0")]);
    }

    public void RunAddRecord(MetaCliInvocation invocation)
    {
        if (!AuthoringCommandsByName.TryGetValue(invocation.Command.Name, out var spec))
        {
            Fail(
                $"No analytics authoring mapping exists for command '{invocation.Command.Name}'.",
                $"{appName} help");
        }

        var request = new AnalyticsAuthoringRequest
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
                request.Relationships.Add(new AnalyticsRelationshipAssignment(
                    relationship.ColumnName,
                    relationship.TargetEntityName,
                    value));
            }
        }

        try
        {
            if (spec.AggregateFunctionOptionName is { } aggregateFunctionOptionName)
            {
                service.AddMeasure(request, invocation.Required(aggregateFunctionOptionName));
            }
            else
            {
                service.AddRecord(request);
            }
            presenter.WriteOk($"Added {request.RecordId} to {spec.EntityName}");
        }
        catch (Exception ex) when (ex is not MetaCliExitException and
                                   (InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException))
        {
            Fail(
                "Cannot update analytics workspace.",
                $"{appName} help {spec.CommandName}",
                4,
                [$"  {ex.Message}"]);
        }
    }

    private static IReadOnlyList<AuthoringCommandSpec> BuildAuthoringCommands()
    {
        static AuthoringCommandSpec Cmd(
            string commandName,
            string entityName,
            PropertySpec[] properties,
            RelationshipSpec[] relationships,
            string? aggregateFunctionOptionName = null) =>
            new(commandName, entityName, properties, relationships, aggregateFunctionOptionName);

        static PropertySpec Prop(string optionName, string propertyName) =>
            new(optionName, propertyName);

        static RelationshipSpec Rel(string optionName, string columnName, string targetEntityName) =>
            new(optionName, columnName, targetEntityName);

        return
        [
            Cmd("add-model", "AnalyticsModel",
                [Prop("name", "Name"), Prop("default-culture", "DefaultCulture"), Prop("description", "Description")],
                []),
            Cmd("add-data-source", "DataSource",
                [Prop("name", "Name"), Prop("provider", "Provider"), Prop("connection-reference", "ConnectionReference"), Prop("source-kind", "SourceKind"), Prop("description", "Description")],
                [Rel("model", "AnalyticsModelId", "AnalyticsModel")]),
            Cmd("add-table", "Table",
                [Prop("name", "Name"), Prop("kind", "Kind"), Prop("data-category", "DataCategory"), Prop("is-hidden", "IsHidden"), Prop("display-folder", "DisplayFolder"), Prop("description", "Description")],
                [Rel("model", "AnalyticsModelId", "AnalyticsModel")]),
            Cmd("add-attribute", "Attribute",
                [Prop("name", "Name"), Prop("data-type-id", "DataTypeId"), Prop("ordinal", "Ordinal"), Prop("kind", "Kind"), Prop("source-name", "SourceName"), Prop("is-key", "IsKey"), Prop("is-nullable", "IsNullable"), Prop("is-hidden", "IsHidden"), Prop("format-string", "FormatString"), Prop("summarize-by", "SummarizeBy"), Prop("data-category", "DataCategory"), Prop("description", "Description")],
                [Rel("table", "TableId", "Table")]),
            Cmd("add-sort-by-attribute", "SortByAttribute",
                [Prop("description", "Description")],
                [Rel("source-attribute", "SourceAttributeId", "Attribute"), Rel("sort-attribute", "SortAttributeId", "Attribute")]),
            Cmd("add-attribute-relationship", "AttributeRelationship",
                [Prop("relationship-type", "RelationshipType"), Prop("description", "Description")],
                [Rel("child-attribute", "ChildAttributeId", "Attribute"), Rel("parent-attribute", "ParentAttributeId", "Attribute")]),
            Cmd("add-hierarchy", "Hierarchy",
                [Prop("name", "Name"), Prop("kind", "Kind"), Prop("is-hidden", "IsHidden"), Prop("display-folder", "DisplayFolder"), Prop("description", "Description")],
                [Rel("table", "TableId", "Table")]),
            Cmd("add-hierarchy-level", "HierarchyLevel",
                [Prop("name", "Name"), Prop("ordinal", "Ordinal")],
                [Rel("hierarchy", "HierarchyId", "Hierarchy"), Rel("attribute", "AttributeId", "Attribute")]),
            Cmd("add-relationship", "Relationship",
                [Prop("name", "Name"), Prop("role-name", "RoleName"), Prop("relationship-kind", "RelationshipKind"), Prop("cardinality", "Cardinality"), Prop("cross-filter-direction", "CrossFilterDirection"), Prop("is-active", "IsActive"), Prop("is-required", "IsRequired"), Prop("description", "Description")],
                [Rel("from-table", "FromTableId", "Table"), Rel("from-attribute", "FromAttributeId", "Attribute"), Rel("to-table", "ToTableId", "Table"), Rel("to-attribute", "ToAttributeId", "Attribute"), Rel("granularity-attribute", "GranularityAttributeId", "Attribute"), Rel("intermediate-table", "IntermediateTableId", "Table")]),
            Cmd("add-measure", "Measure",
                [Prop("name", "Name"), Prop("data-type-id", "DataTypeId"), Prop("format-string", "FormatString"), Prop("display-folder", "DisplayFolder"), Prop("is-hidden", "IsHidden"), Prop("description", "Description")],
                [Rel("table", "TableId", "Table"), Rel("source-attribute", "SourceAttributeId", "Attribute")],
                aggregateFunctionOptionName: "aggregate-function"),
            Cmd("add-perspective", "Perspective",
                [Prop("name", "Name"), Prop("description", "Description")],
                [Rel("model", "AnalyticsModelId", "AnalyticsModel")]),
            Cmd("add-perspective-table", "PerspectiveTable",
                [],
                [Rel("perspective", "PerspectiveId", "Perspective"), Rel("table", "TableId", "Table")]),
            Cmd("add-perspective-attribute", "PerspectiveAttribute",
                [],
                [Rel("perspective", "PerspectiveId", "Perspective"), Rel("attribute", "AttributeId", "Attribute")]),
            Cmd("add-perspective-hierarchy", "PerspectiveHierarchy",
                [],
                [Rel("perspective", "PerspectiveId", "Perspective"), Rel("hierarchy", "HierarchyId", "Hierarchy")]),
            Cmd("add-perspective-measure", "PerspectiveMeasure",
                [],
                [Rel("perspective", "PerspectiveId", "Perspective"), Rel("measure", "MeasureId", "Measure")]),
            Cmd("add-security-role", "SecurityRole",
                [Prop("name", "Name"), Prop("permission", "Permission"), Prop("description", "Description")],
                [Rel("model", "AnalyticsModelId", "AnalyticsModel")]),
            Cmd("add-role-member", "RoleMember",
                [Prop("member-name", "MemberName"), Prop("member-kind", "MemberKind")],
                [Rel("role", "SecurityRoleId", "SecurityRole")]),
            Cmd("add-table-permission", "TablePermission",
                [Prop("metadata-permission", "MetadataPermission"), Prop("description", "Description")],
                [Rel("role", "SecurityRoleId", "SecurityRole"), Rel("table", "TableId", "Table")]),
            Cmd("add-attribute-permission", "AttributePermission",
                [Prop("metadata-permission", "MetadataPermission"), Prop("description", "Description")],
                [Rel("role", "SecurityRoleId", "SecurityRole"), Rel("attribute", "AttributeId", "Attribute")]),
            Cmd("add-culture", "Culture",
                [Prop("name", "Name"), Prop("description", "Description")],
                [Rel("model", "AnalyticsModelId", "AnalyticsModel")]),
            Cmd("add-table-translation", "TableTranslation",
                [Prop("caption", "Caption"), Prop("description", "Description")],
                [Rel("culture", "CultureId", "Culture"), Rel("table", "TableId", "Table")]),
            Cmd("add-attribute-translation", "AttributeTranslation",
                [Prop("caption", "Caption"), Prop("description", "Description")],
                [Rel("culture", "CultureId", "Culture"), Rel("attribute", "AttributeId", "Attribute")]),
            Cmd("add-hierarchy-translation", "HierarchyTranslation",
                [Prop("caption", "Caption"), Prop("description", "Description")],
                [Rel("culture", "CultureId", "Culture"), Rel("hierarchy", "HierarchyId", "Hierarchy")]),
            Cmd("add-measure-translation", "MeasureTranslation",
                [Prop("caption", "Caption"), Prop("description", "Description")],
                [Rel("culture", "CultureId", "Culture"), Rel("measure", "MeasureId", "Measure")]),
            Cmd("add-perspective-translation", "PerspectiveTranslation",
                [Prop("caption", "Caption"), Prop("description", "Description")],
                [Rel("culture", "CultureId", "Culture"), Rel("perspective", "PerspectiveId", "Perspective")]),
        ];
    }

    private string HelpCommand(string commandName) => $"{appName} help {commandName}";

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
        IReadOnlyList<PropertySpec> Properties,
        IReadOnlyList<RelationshipSpec> Relationships,
        string? AggregateFunctionOptionName)
    {
        public string ExecutableCommandId => $"exec-{CommandName}";
    }

    private sealed record PropertySpec(string OptionName, string PropertyName);

    private sealed record RelationshipSpec(
        string OptionName,
        string ColumnName,
        string TargetEntityName);
}
