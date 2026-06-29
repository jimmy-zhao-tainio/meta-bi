using System.Diagnostics.CodeAnalysis;
using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaDataWarehouse.Core;

internal sealed class MetaDataWarehouseCommandHandlers
{
    private static readonly IReadOnlyDictionary<string, AuthoringCommandSpec> AuthoringCommandsByName =
        BuildAuthoringCommands()
            .ToDictionary(static spec => spec.CommandName, StringComparer.OrdinalIgnoreCase);

    private readonly ConsolePresenter presenter;
    private readonly string appName;
    private readonly IDataWarehouseAuthoringService service;

    public MetaDataWarehouseCommandHandlers(
        ConsolePresenter presenter,
        string appName,
        IDataWarehouseAuthoringService service)
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
            var result = service.CreateWorkspace(targetValidation.FullPath);
            presenter.WriteKeyValueBlock(
                "MetaDataWarehouse workspace created",
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
                "Cannot create data-warehouse workspace.",
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
                $"No data-warehouse authoring mapping exists for command '{invocation.Command.Name}'.",
                $"{appName} help");
        }

        var request = new DataWarehouseAuthoringRequest
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
                request.Relationships.Add(new DataWarehouseRelationshipAssignment(
                    relationship.ColumnName,
                    relationship.TargetEntityName,
                    value));
            }
        }

        try
        {
            service.AddRecord(request);
            presenter.WriteOk($"Added {request.RecordId} to {spec.EntityName}");
        }
        catch (Exception ex) when (ex is not MetaCliExitException and
                                   (InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException))
        {
            Fail(
                "Cannot update data-warehouse workspace.",
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
            RelationshipSpec[] relationships) =>
            new(commandName, entityName, properties, relationships);

        static PropertySpec Prop(string optionName, string propertyName) =>
            new(optionName, propertyName);

        static RelationshipSpec Rel(string optionName, string columnName, string targetEntityName) =>
            new(optionName, columnName, targetEntityName);

        return
        [
            Cmd("add-warehouse", "Warehouse",
                [Prop("name", "Name"), Prop("description", "Description")],
                []),
            Cmd("add-dimension", "Dimension",
                [Prop("name", "Name"), Prop("description", "Description")],
                [Rel("warehouse", "WarehouseId", "Warehouse")]),
            Cmd("add-conformed-dimension", "ConformedDimension",
                [Prop("conformance-name", "ConformanceName"), Prop("description", "Description")],
                [Rel("dimension", "DimensionId", "Dimension")]),
            Cmd("add-dimension-attribute", "DimensionAttribute",
                [Prop("name", "Name"), Prop("data-type-id", "DataTypeId"), Prop("ordinal", "Ordinal"), Prop("is-nullable", "IsNullable"), Prop("description", "Description")],
                [Rel("dimension", "DimensionId", "Dimension")]),
            Cmd("add-dimension-business-key", "DimensionBusinessKey",
                [Prop("name", "Name"), Prop("description", "Description")],
                [Rel("dimension", "DimensionId", "Dimension")]),
            Cmd("add-dimension-business-key-part", "DimensionBusinessKeyPart",
                [Prop("ordinal", "Ordinal")],
                [Rel("business-key", "DimensionBusinessKeyId", "DimensionBusinessKey"), Rel("attribute", "DimensionAttributeId", "DimensionAttribute")]),
            Cmd("add-slowly-changing-dimension", "SlowlyChangingDimension",
                [Prop("name", "Name"), Prop("description", "Description")],
                [Rel("dimension", "DimensionId", "Dimension")]),
            Cmd("add-type1-dimension-attribute", "Type1DimensionAttribute",
                [Prop("description", "Description")],
                [Rel("slowly-changing-dimension", "SlowlyChangingDimensionId", "SlowlyChangingDimension"), Rel("attribute", "DimensionAttributeId", "DimensionAttribute")]),
            Cmd("add-type2-dimension-attribute", "Type2DimensionAttribute",
                [Prop("description", "Description")],
                [Rel("slowly-changing-dimension", "SlowlyChangingDimensionId", "SlowlyChangingDimension"), Rel("attribute", "DimensionAttributeId", "DimensionAttribute")]),
            Cmd("add-dimension-hierarchy", "DimensionHierarchy",
                [Prop("name", "Name"), Prop("description", "Description")],
                [Rel("dimension", "DimensionId", "Dimension")]),
            Cmd("add-dimension-hierarchy-level", "DimensionHierarchyLevel",
                [Prop("name", "Name"), Prop("ordinal", "Ordinal")],
                [Rel("hierarchy", "DimensionHierarchyId", "DimensionHierarchy"), Rel("attribute", "DimensionAttributeId", "DimensionAttribute")]),
            Cmd("add-junk-dimension", "JunkDimension",
                [Prop("description", "Description")],
                [Rel("dimension", "DimensionId", "Dimension")]),
            Cmd("add-junk-dimension-component", "JunkDimensionComponent",
                [Prop("ordinal", "Ordinal"), Prop("description", "Description")],
                [Rel("junk-dimension", "JunkDimensionId", "JunkDimension"), Rel("attribute", "DimensionAttributeId", "DimensionAttribute")]),
            Cmd("add-mini-dimension", "MiniDimension",
                [Prop("role-name", "RoleName"), Prop("description", "Description")],
                [Rel("source-dimension", "SourceDimensionId", "Dimension"), Rel("profile-dimension", "ProfileDimensionId", "Dimension")]),
            Cmd("add-outrigger-dimension", "OutriggerDimension",
                [Prop("role-name", "RoleName"), Prop("ordinal", "Ordinal"), Prop("is-required", "IsRequired"), Prop("description", "Description")],
                [Rel("parent-dimension", "ParentDimensionId", "Dimension"), Rel("child-dimension", "ChildDimensionId", "Dimension")]),
            Cmd("add-fact", "Fact",
                [Prop("name", "Name"), Prop("description", "Description")],
                [Rel("warehouse", "WarehouseId", "Warehouse")]),
            Cmd("add-fact-grain", "FactGrain",
                [Prop("name", "Name"), Prop("description", "Description")],
                [Rel("fact", "FactId", "Fact")]),
            Cmd("add-fact-dimension", "FactDimension",
                [Prop("role-name", "RoleName"), Prop("ordinal", "Ordinal"), Prop("is-required", "IsRequired"), Prop("description", "Description")],
                [Rel("fact", "FactId", "Fact"), Rel("dimension", "DimensionId", "Dimension")]),
            Cmd("add-fact-measure", "FactMeasure",
                [Prop("name", "Name"), Prop("data-type-id", "DataTypeId"), Prop("ordinal", "Ordinal"), Prop("is-nullable", "IsNullable"), Prop("description", "Description")],
                [Rel("fact", "FactId", "Fact")]),
            Cmd("add-degenerate-dimension", "DegenerateDimension",
                [Prop("name", "Name"), Prop("data-type-id", "DataTypeId"), Prop("ordinal", "Ordinal"), Prop("description", "Description")],
                [Rel("fact", "FactId", "Fact")]),
            Cmd("add-transaction-fact", "TransactionFact",
                [Prop("description", "Description")],
                [Rel("fact", "FactId", "Fact")]),
            Cmd("add-periodic-snapshot-fact", "PeriodicSnapshotFact",
                [Prop("period-name", "PeriodName"), Prop("description", "Description")],
                [Rel("fact", "FactId", "Fact")]),
            Cmd("add-accumulating-snapshot-fact", "AccumulatingSnapshotFact",
                [Prop("description", "Description")],
                [Rel("fact", "FactId", "Fact")]),
            Cmd("add-accumulating-snapshot-milestone", "AccumulatingSnapshotMilestone",
                [Prop("name", "Name"), Prop("ordinal", "Ordinal"), Prop("date-role-name", "DateRoleName"), Prop("description", "Description")],
                [Rel("accumulating-snapshot", "AccumulatingSnapshotFactId", "AccumulatingSnapshotFact")]),
            Cmd("add-factless-fact", "FactlessFact",
                [Prop("description", "Description")],
                [Rel("fact", "FactId", "Fact")]),
            Cmd("add-aggregate-fact", "AggregateFact",
                [Prop("description", "Description")],
                [Rel("aggregated-fact", "AggregatedFactId", "Fact"), Rel("source-fact", "SourceFactId", "Fact")]),
            Cmd("add-bridge", "BridgeTable",
                [Prop("name", "Name"), Prop("description", "Description")],
                [Rel("warehouse", "WarehouseId", "Warehouse")]),
            Cmd("add-bridge-participant", "BridgeParticipant",
                [Prop("role-name", "RoleName"), Prop("ordinal", "Ordinal"), Prop("is-required", "IsRequired")],
                [Rel("bridge", "BridgeTableId", "BridgeTable"), Rel("dimension", "DimensionId", "Dimension")]),
            Cmd("add-bridge-weight", "BridgeWeight",
                [Prop("name", "Name"), Prop("data-type-id", "DataTypeId"), Prop("description", "Description")],
                [Rel("bridge", "BridgeTableId", "BridgeTable")]),
            Cmd("add-fact-bridge", "FactBridge",
                [Prop("role-name", "RoleName"), Prop("ordinal", "Ordinal"), Prop("description", "Description")],
                [Rel("fact", "FactId", "Fact"), Rel("bridge", "BridgeTableId", "BridgeTable")]),
        ];
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
