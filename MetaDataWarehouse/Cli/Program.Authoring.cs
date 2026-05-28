using Meta.Core.Presentation.Cli;
using MetaDataWarehouse.Core;

internal static partial class Program
{
    private sealed record AddOptionSpec(string OptionName, string PropertyName, bool Required, string ValueLabel = "<value>");
    private sealed record AddRelationshipOptionSpec(string OptionName, string ColumnName, string TargetEntityName, string ValueLabel = "<id>");
    private sealed record AddCommandSpec(
        string CommandName,
        string EntityName,
        string Description,
        IReadOnlyList<AddOptionSpec> PropertyOptions,
        IReadOnlyList<AddRelationshipOptionSpec> RelationshipOptions);
    private sealed record ParsedAddCommand(
        bool Ok,
        string WorkspacePath,
        string RecordId,
        Dictionary<string, string> Values,
        List<DataWarehouseRelationshipAssignment> Relationships,
        string ErrorMessage);

    private static readonly IReadOnlyDictionary<string, AddCommandSpec> AddCommands = BuildAddCommands();

    private static IReadOnlyDictionary<string, AddCommandSpec> BuildAddCommands()
    {
        AddCommandSpec Cmd(string command, string entity, string description, AddOptionSpec[] props, AddRelationshipOptionSpec[] rels)
            => new(command, entity, description, props, rels);

        var specs = new[]
        {
            Cmd("add-warehouse", "Warehouse", "Add a dimensional warehouse.", [ new("--name", "Name", true), new("--description", "Description", false) ], []),
            Cmd("add-dimension", "Dimension", "Add a dimension.", [ new("--name", "Name", true), new("--description", "Description", false) ], [ new("--warehouse", "WarehouseId", "Warehouse") ]),
            Cmd("add-conformed-dimension", "ConformedDimension", "Mark a dimension as conformed.", [ new("--conformance-name", "ConformanceName", true), new("--description", "Description", false) ], [ new("--dimension", "DimensionId", "Dimension") ]),
            Cmd("add-dimension-attribute", "DimensionAttribute", "Add a dimension attribute with a Meta data type.", [ new("--name", "Name", true), new("--data-type-id", "DataTypeId", true), new("--ordinal", "Ordinal", false), new("--is-nullable", "IsNullable", false), new("--description", "Description", false) ], [ new("--dimension", "DimensionId", "Dimension") ]),
            Cmd("add-dimension-business-key", "DimensionBusinessKey", "Add a dimension business key.", [ new("--name", "Name", true), new("--description", "Description", false) ], [ new("--dimension", "DimensionId", "Dimension") ]),
            Cmd("add-dimension-business-key-part", "DimensionBusinessKeyPart", "Add an ordered attribute to a dimension business key.", [ new("--ordinal", "Ordinal", false) ], [ new("--business-key", "DimensionBusinessKeyId", "DimensionBusinessKey"), new("--attribute", "DimensionAttributeId", "DimensionAttribute") ]),
            Cmd("add-slowly-changing-dimension", "SlowlyChangingDimension", "Declare SCD behavior for a dimension.", [ new("--name", "Name", false), new("--description", "Description", false) ], [ new("--dimension", "DimensionId", "Dimension") ]),
            Cmd("add-type1-dimension-attribute", "Type1DimensionAttribute", "Declare a Type 1 attribute in an SCD dimension.", [ new("--description", "Description", false) ], [ new("--slowly-changing-dimension", "SlowlyChangingDimensionId", "SlowlyChangingDimension"), new("--attribute", "DimensionAttributeId", "DimensionAttribute") ]),
            Cmd("add-type2-dimension-attribute", "Type2DimensionAttribute", "Declare a Type 2 attribute in an SCD dimension.", [ new("--description", "Description", false) ], [ new("--slowly-changing-dimension", "SlowlyChangingDimensionId", "SlowlyChangingDimension"), new("--attribute", "DimensionAttributeId", "DimensionAttribute") ]),
            Cmd("add-dimension-hierarchy", "DimensionHierarchy", "Add a dimension hierarchy.", [ new("--name", "Name", true), new("--description", "Description", false) ], [ new("--dimension", "DimensionId", "Dimension") ]),
            Cmd("add-dimension-hierarchy-level", "DimensionHierarchyLevel", "Add a hierarchy level.", [ new("--name", "Name", true), new("--ordinal", "Ordinal", false) ], [ new("--hierarchy", "DimensionHierarchyId", "DimensionHierarchy"), new("--attribute", "DimensionAttributeId", "DimensionAttribute") ]),
            Cmd("add-junk-dimension", "JunkDimension", "Declare a junk dimension.", [ new("--description", "Description", false) ], [ new("--dimension", "DimensionId", "Dimension") ]),
            Cmd("add-junk-dimension-component", "JunkDimensionComponent", "Add an attribute component to a junk dimension.", [ new("--ordinal", "Ordinal", false), new("--description", "Description", false) ], [ new("--junk-dimension", "JunkDimensionId", "JunkDimension"), new("--attribute", "DimensionAttributeId", "DimensionAttribute") ]),
            Cmd("add-mini-dimension", "MiniDimension", "Declare a mini-dimension relationship.", [ new("--role-name", "RoleName", false), new("--description", "Description", false) ], [ new("--source-dimension", "SourceDimensionId", "Dimension"), new("--profile-dimension", "ProfileDimensionId", "Dimension") ]),
            Cmd("add-outrigger-dimension", "OutriggerDimension", "Declare an outrigger dimension relationship.", [ new("--role-name", "RoleName", true), new("--ordinal", "Ordinal", false), new("--is-required", "IsRequired", false), new("--description", "Description", false) ], [ new("--parent-dimension", "ParentDimensionId", "Dimension"), new("--child-dimension", "ChildDimensionId", "Dimension") ]),
            Cmd("add-fact", "Fact", "Add a fact table concept.", [ new("--name", "Name", true), new("--description", "Description", false) ], [ new("--warehouse", "WarehouseId", "Warehouse") ]),
            Cmd("add-fact-grain", "FactGrain", "Declare a fact grain.", [ new("--name", "Name", true), new("--description", "Description", true) ], [ new("--fact", "FactId", "Fact") ]),
            Cmd("add-fact-dimension", "FactDimension", "Add a dimensional role to a fact.", [ new("--role-name", "RoleName", true), new("--ordinal", "Ordinal", false), new("--is-required", "IsRequired", false), new("--description", "Description", false) ], [ new("--fact", "FactId", "Fact"), new("--dimension", "DimensionId", "Dimension") ]),
            Cmd("add-fact-measure", "FactMeasure", "Add a typed fact measure.", [ new("--name", "Name", true), new("--data-type-id", "DataTypeId", true), new("--ordinal", "Ordinal", false), new("--is-nullable", "IsNullable", false), new("--description", "Description", false) ], [ new("--fact", "FactId", "Fact") ]),
            Cmd("add-degenerate-dimension", "DegenerateDimension", "Add a degenerate dimension value to a fact.", [ new("--name", "Name", true), new("--data-type-id", "DataTypeId", true), new("--ordinal", "Ordinal", false), new("--description", "Description", false) ], [ new("--fact", "FactId", "Fact") ]),
            Cmd("add-transaction-fact", "TransactionFact", "Mark a fact as transaction-grain.", [ new("--description", "Description", false) ], [ new("--fact", "FactId", "Fact") ]),
            Cmd("add-periodic-snapshot-fact", "PeriodicSnapshotFact", "Mark a fact as a periodic snapshot.", [ new("--period-name", "PeriodName", true), new("--description", "Description", false) ], [ new("--fact", "FactId", "Fact") ]),
            Cmd("add-accumulating-snapshot-fact", "AccumulatingSnapshotFact", "Mark a fact as an accumulating snapshot.", [ new("--description", "Description", false) ], [ new("--fact", "FactId", "Fact") ]),
            Cmd("add-accumulating-snapshot-milestone", "AccumulatingSnapshotMilestone", "Add a lifecycle milestone to an accumulating snapshot.", [ new("--name", "Name", true), new("--ordinal", "Ordinal", false), new("--date-role-name", "DateRoleName", true), new("--description", "Description", false) ], [ new("--accumulating-snapshot", "AccumulatingSnapshotFactId", "AccumulatingSnapshotFact") ]),
            Cmd("add-factless-fact", "FactlessFact", "Mark a fact as factless.", [ new("--description", "Description", false) ], [ new("--fact", "FactId", "Fact") ]),
            Cmd("add-aggregate-fact", "AggregateFact", "Declare an aggregate fact derived from a source fact.", [ new("--description", "Description", false) ], [ new("--aggregated-fact", "AggregatedFactId", "Fact"), new("--source-fact", "SourceFactId", "Fact") ]),
            Cmd("add-bridge", "BridgeTable", "Add a dimensional bridge table.", [ new("--name", "Name", true), new("--description", "Description", false) ], [ new("--warehouse", "WarehouseId", "Warehouse") ]),
            Cmd("add-bridge-participant", "BridgeParticipant", "Add a dimension participant to a bridge.", [ new("--role-name", "RoleName", true), new("--ordinal", "Ordinal", false), new("--is-required", "IsRequired", false) ], [ new("--bridge", "BridgeTableId", "BridgeTable"), new("--dimension", "DimensionId", "Dimension") ]),
            Cmd("add-bridge-weight", "BridgeWeight", "Add a bridge weighting measure with a Meta data type.", [ new("--name", "Name", true), new("--data-type-id", "DataTypeId", true), new("--description", "Description", false) ], [ new("--bridge", "BridgeTableId", "BridgeTable") ]),
            Cmd("add-fact-bridge", "FactBridge", "Connect a fact to a bridge table.", [ new("--role-name", "RoleName", true), new("--ordinal", "Ordinal", false), new("--description", "Description", false) ], [ new("--fact", "FactId", "Fact"), new("--bridge", "BridgeTableId", "BridgeTable") ]),
        };

        return specs.ToDictionary(spec => spec.CommandName, StringComparer.OrdinalIgnoreCase);
    }

    private static IReadOnlyList<CliCommandRoute> BuildAddCommandRoutes()
    {
        return AddCommands.Values
            .OrderBy(spec => spec.CommandName, StringComparer.Ordinal)
            .Select(spec => new CliCommandRoute(CreateAddCommandDefinition(spec), args => RunAddCommandAsync(spec, args)))
            .ToArray();
    }

    private static CliCommandDefinition CreateAddCommandDefinition(AddCommandSpec spec)
    {
        var usageParts = new List<string> { $"{AppName} {spec.CommandName}", "[--workspace <path>]", "--id <id>" };
        usageParts.AddRange(spec.PropertyOptions.Select(item => item.Required ? $"{item.OptionName} {item.ValueLabel}" : $"[{item.OptionName} {item.ValueLabel}]"));
        usageParts.AddRange(spec.RelationshipOptions.Select(item => $"{item.OptionName} {item.ValueLabel}"));

        var options = new List<CliOptionDefinition>
        {
            new("--workspace <path>", "Optional. Workspace path. Default: current working directory."),
            new("--id <id>", $"Required. {spec.EntityName} row id."),
        };
        options.AddRange(spec.PropertyOptions.Select(item =>
            new CliOptionDefinition(
                $"{item.OptionName} {item.ValueLabel}",
                $"{(item.Required ? "Required" : "Optional")}. {item.PropertyName}.")));
        options.AddRange(spec.RelationshipOptions.Select(item =>
            new CliOptionDefinition(
                $"{item.OptionName} {item.ValueLabel}",
                $"Required. {item.TargetEntityName} id for {item.ColumnName}.")));

        return new CliCommandDefinition(
            spec.CommandName,
            spec.Description,
            new[] { string.Join(" ", usageParts) },
            options,
            new[]
            {
                $"Adds one {spec.EntityName} row to a MetaDataWarehouse workspace.",
                "Defaults to the current working directory when --workspace is omitted."
            });
    }

    private static async Task<int> RunAddCommandAsync(AddCommandSpec spec, string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintAddCommandHelp(spec);
            return 0;
        }

        var parse = ParseAddCommand(spec, args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand(spec.CommandName));
        }

        try
        {
            var request = new DataWarehouseAuthoringRequest
            {
                WorkspacePath = Path.GetFullPath(parse.WorkspacePath),
                EntityName = spec.EntityName,
                RecordId = parse.RecordId,
            };
            foreach (var value in parse.Values) request.Values[value.Key] = value.Value;
            request.Relationships.AddRange(parse.Relationships);
            await new DataWarehouseAuthoringService().AddRecordAsync(request).ConfigureAwait(false);

            Presenter.WriteOk($"Added {parse.RecordId} to {spec.EntityName}");
            return 0;
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or IOException or UnauthorizedAccessException)
        {
            return Fail(
                "Cannot update data-warehouse workspace.",
                HelpCommand(spec.CommandName),
                4,
                [$"  {ex.Message}"]);
        }
    }

    private static ParsedAddCommand ParseAddCommand(AddCommandSpec spec, string[] args, int startIndex)
    {
        var workspacePath = ".";
        var recordId = string.Empty;
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var relationships = new List<DataWarehouseRelationshipAssignment>();
        var propertyOptions = spec.PropertyOptions.ToDictionary(item => item.OptionName, StringComparer.OrdinalIgnoreCase);
        var relationshipOptions = spec.RelationshipOptions.ToDictionary(item => item.OptionName, StringComparer.OrdinalIgnoreCase);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (i + 1 >= args.Length)
            {
                return new ParsedAddCommand(false, workspacePath, recordId, values, relationships, $"missing value for {arg}.");
            }

            var value = args[++i];
            if (!seen.Add(arg))
            {
                return new ParsedAddCommand(false, workspacePath, recordId, values, relationships, $"{arg} can only be provided once.");
            }

            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                workspacePath = value;
                continue;
            }

            if (string.Equals(arg, "--id", StringComparison.OrdinalIgnoreCase))
            {
                recordId = value;
                continue;
            }

            if (propertyOptions.TryGetValue(arg, out var prop))
            {
                values[prop.PropertyName] = value;
                continue;
            }

            if (relationshipOptions.TryGetValue(arg, out var rel))
            {
                relationships.Add(new DataWarehouseRelationshipAssignment(rel.ColumnName, rel.TargetEntityName, value));
                continue;
            }

            return new ParsedAddCommand(false, workspacePath, recordId, values, relationships, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(recordId))
        {
            return new ParsedAddCommand(false, workspacePath, recordId, values, relationships, "missing required option --id <id>.");
        }

        foreach (var prop in spec.PropertyOptions.Where(item => item.Required))
        {
            if (!values.ContainsKey(prop.PropertyName) || string.IsNullOrWhiteSpace(values[prop.PropertyName]))
            {
                return new ParsedAddCommand(false, workspacePath, recordId, values, relationships, $"missing required option {prop.OptionName} {prop.ValueLabel}.");
            }
        }

        foreach (var rel in spec.RelationshipOptions)
        {
            if (!relationships.Any(item => string.Equals(item.ColumnName, rel.ColumnName, StringComparison.OrdinalIgnoreCase)))
            {
                return new ParsedAddCommand(false, workspacePath, recordId, values, relationships, $"missing required option {rel.OptionName} {rel.ValueLabel}.");
            }
        }

        return new ParsedAddCommand(true, workspacePath, recordId, values, relationships, string.Empty);
    }

    private static void PrintAddCommandHelp(AddCommandSpec spec)
    {
        PrintCommandHelp(spec.CommandName);
    }
}
