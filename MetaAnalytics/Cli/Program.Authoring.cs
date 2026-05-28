using Meta.Core.Presentation.Cli;
using MetaAnalytics.Core;

internal static partial class Program
{
    private sealed record AddOptionSpec(string OptionName, string PropertyName, bool Required, string ValueLabel = "<value>");
    private sealed record AddRelationshipOptionSpec(string OptionName, string ColumnName, string TargetEntityName, bool Required, string ValueLabel = "<id>");
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
        List<AnalyticsRelationshipAssignment> Relationships,
        string ErrorMessage);

    private static readonly IReadOnlyDictionary<string, AddCommandSpec> AddCommands = BuildAddCommands();

    private static IReadOnlyDictionary<string, AddCommandSpec> BuildAddCommands()
    {
        AddCommandSpec Cmd(string command, string entity, string description, AddOptionSpec[] props, AddRelationshipOptionSpec[] rels)
            => new(command, entity, description, props, rels);
        AddOptionSpec Prop(string option, string property, bool required, string label = "<value>") => new(option, property, required, label);
        AddRelationshipOptionSpec Rel(string option, string column, string target, bool required, string label = "<id>") => new(option, column, target, required, label);

        var specs = new[]
        {
            Cmd("add-model", "AnalyticsModel", "Add an analytics model.", [ Prop("--name", "Name", true), Prop("--default-culture", "DefaultCulture", false), Prop("--description", "Description", false) ], []),
            Cmd("add-data-source", "DataSource", "Add an analytics source declaration.", [ Prop("--name", "Name", true), Prop("--provider", "Provider", false), Prop("--connection-reference", "ConnectionReference", false), Prop("--source-kind", "SourceKind", false), Prop("--description", "Description", false) ], [ Rel("--model", "AnalyticsModelId", "AnalyticsModel", true) ]),
            Cmd("add-table", "Table", "Add an analytics table.", [ Prop("--name", "Name", true), Prop("--kind", "Kind", true), Prop("--data-category", "DataCategory", false), Prop("--is-hidden", "IsHidden", false), Prop("--display-folder", "DisplayFolder", false), Prop("--description", "Description", false) ], [ Rel("--model", "AnalyticsModelId", "AnalyticsModel", true) ]),
            Cmd("add-attribute", "Attribute", "Add a typed table attribute or calculated attribute.", [ Prop("--name", "Name", true), Prop("--data-type-id", "DataTypeId", true), Prop("--ordinal", "Ordinal", false), Prop("--kind", "Kind", false), Prop("--source-name", "SourceName", false), Prop("--expression-language", "ExpressionLanguage", false), Prop("--expression", "Expression", false), Prop("--is-key", "IsKey", false), Prop("--is-nullable", "IsNullable", false), Prop("--is-hidden", "IsHidden", false), Prop("--format-string", "FormatString", false), Prop("--summarize-by", "SummarizeBy", false), Prop("--data-category", "DataCategory", false), Prop("--description", "Description", false) ], [ Rel("--table", "TableId", "Table", true) ]),
            Cmd("add-sort-by-attribute", "SortByAttribute", "Declare one attribute as the sort key for another.", [ Prop("--description", "Description", false) ], [ Rel("--source-attribute", "SourceAttributeId", "Attribute", true), Rel("--sort-attribute", "SortAttributeId", "Attribute", true) ]),
            Cmd("add-attribute-relationship", "AttributeRelationship", "Declare an attribute relationship inside a table.", [ Prop("--relationship-type", "RelationshipType", false), Prop("--description", "Description", false) ], [ Rel("--child-attribute", "ChildAttributeId", "Attribute", true), Rel("--parent-attribute", "ParentAttributeId", "Attribute", true) ]),
            Cmd("add-hierarchy", "Hierarchy", "Add a hierarchy.", [ Prop("--name", "Name", true), Prop("--kind", "Kind", false), Prop("--is-hidden", "IsHidden", false), Prop("--display-folder", "DisplayFolder", false), Prop("--description", "Description", false) ], [ Rel("--table", "TableId", "Table", true) ]),
            Cmd("add-hierarchy-level", "HierarchyLevel", "Add an ordered hierarchy level.", [ Prop("--name", "Name", true), Prop("--ordinal", "Ordinal", false) ], [ Rel("--hierarchy", "HierarchyId", "Hierarchy", true), Rel("--attribute", "AttributeId", "Attribute", true) ]),
            Cmd("add-relationship", "Relationship", "Add a relationship between analytics tables.", [ Prop("--name", "Name", true), Prop("--role-name", "RoleName", false), Prop("--relationship-kind", "RelationshipKind", true), Prop("--cardinality", "Cardinality", true), Prop("--cross-filter-direction", "CrossFilterDirection", false), Prop("--is-active", "IsActive", false), Prop("--is-required", "IsRequired", false), Prop("--description", "Description", false) ], [ Rel("--from-table", "FromTableId", "Table", true), Rel("--from-attribute", "FromAttributeId", "Attribute", true), Rel("--to-table", "ToTableId", "Table", true), Rel("--to-attribute", "ToAttributeId", "Attribute", true), Rel("--granularity-attribute", "GranularityAttributeId", "Attribute", false), Rel("--intermediate-table", "IntermediateTableId", "Table", false) ]),
            Cmd("add-measure", "Measure", "Add a source-backed base measure.", [ Prop("--name", "Name", true), Prop("--data-type-id", "DataTypeId", false), Prop("--format-string", "FormatString", false), Prop("--display-folder", "DisplayFolder", false), Prop("--is-hidden", "IsHidden", false), Prop("--description", "Description", false) ], [ Rel("--table", "TableId", "Table", true), Rel("--source-attribute", "SourceAttributeId", "Attribute", true) ]),
            Cmd("add-aggregation-behavior", "AggregationBehavior", "Declare a base measure aggregate function.", [ Prop("--function", "Function", true), Prop("--description", "Description", false) ], [ Rel("--measure", "MeasureId", "Measure", true) ]),
            Cmd("add-perspective", "Perspective", "Add a perspective.", [ Prop("--name", "Name", true), Prop("--description", "Description", false) ], [ Rel("--model", "AnalyticsModelId", "AnalyticsModel", true) ]),
            Cmd("add-perspective-table", "PerspectiveTable", "Expose a table in a perspective.", [], [ Rel("--perspective", "PerspectiveId", "Perspective", true), Rel("--table", "TableId", "Table", true) ]),
            Cmd("add-perspective-attribute", "PerspectiveAttribute", "Expose an attribute in a perspective.", [], [ Rel("--perspective", "PerspectiveId", "Perspective", true), Rel("--attribute", "AttributeId", "Attribute", true) ]),
            Cmd("add-perspective-hierarchy", "PerspectiveHierarchy", "Expose a hierarchy in a perspective.", [], [ Rel("--perspective", "PerspectiveId", "Perspective", true), Rel("--hierarchy", "HierarchyId", "Hierarchy", true) ]),
            Cmd("add-perspective-measure", "PerspectiveMeasure", "Expose a measure in a perspective.", [], [ Rel("--perspective", "PerspectiveId", "Perspective", true), Rel("--measure", "MeasureId", "Measure", true) ]),
            Cmd("add-security-role", "SecurityRole", "Add a security role.", [ Prop("--name", "Name", true), Prop("--permission", "Permission", true), Prop("--description", "Description", false) ], [ Rel("--model", "AnalyticsModelId", "AnalyticsModel", true) ]),
            Cmd("add-role-member", "RoleMember", "Add a member to a security role.", [ Prop("--member-name", "MemberName", true), Prop("--member-kind", "MemberKind", false) ], [ Rel("--role", "SecurityRoleId", "SecurityRole", true) ]),
            Cmd("add-role-filter", "RoleFilter", "Add row-level security over a table.", [ Prop("--expression-language", "ExpressionLanguage", true), Prop("--expression", "Expression", true), Prop("--description", "Description", false) ], [ Rel("--role", "SecurityRoleId", "SecurityRole", true), Rel("--table", "TableId", "Table", true) ]),
            Cmd("add-table-permission", "TablePermission", "Add object-level security for a table.", [ Prop("--metadata-permission", "MetadataPermission", true), Prop("--description", "Description", false) ], [ Rel("--role", "SecurityRoleId", "SecurityRole", true), Rel("--table", "TableId", "Table", true) ]),
            Cmd("add-attribute-permission", "AttributePermission", "Add object-level security for an attribute.", [ Prop("--metadata-permission", "MetadataPermission", true), Prop("--description", "Description", false) ], [ Rel("--role", "SecurityRoleId", "SecurityRole", true), Rel("--attribute", "AttributeId", "Attribute", true) ]),
            Cmd("add-culture", "Culture", "Add a model culture.", [ Prop("--name", "Name", true), Prop("--description", "Description", false) ], [ Rel("--model", "AnalyticsModelId", "AnalyticsModel", true) ]),
            Cmd("add-table-translation", "TableTranslation", "Translate table metadata.", [ Prop("--caption", "Caption", false), Prop("--description", "Description", false) ], [ Rel("--culture", "CultureId", "Culture", true), Rel("--table", "TableId", "Table", true) ]),
            Cmd("add-attribute-translation", "AttributeTranslation", "Translate attribute metadata.", [ Prop("--caption", "Caption", false), Prop("--description", "Description", false) ], [ Rel("--culture", "CultureId", "Culture", true), Rel("--attribute", "AttributeId", "Attribute", true) ]),
            Cmd("add-hierarchy-translation", "HierarchyTranslation", "Translate hierarchy metadata.", [ Prop("--caption", "Caption", false), Prop("--description", "Description", false) ], [ Rel("--culture", "CultureId", "Culture", true), Rel("--hierarchy", "HierarchyId", "Hierarchy", true) ]),
            Cmd("add-measure-translation", "MeasureTranslation", "Translate measure metadata.", [ Prop("--caption", "Caption", false), Prop("--description", "Description", false) ], [ Rel("--culture", "CultureId", "Culture", true), Rel("--measure", "MeasureId", "Measure", true) ]),
            Cmd("add-perspective-translation", "PerspectiveTranslation", "Translate perspective metadata.", [ Prop("--caption", "Caption", false), Prop("--description", "Description", false) ], [ Rel("--culture", "CultureId", "Culture", true), Rel("--perspective", "PerspectiveId", "Perspective", true) ]),
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
        usageParts.AddRange(spec.RelationshipOptions.Select(item => item.Required ? $"{item.OptionName} {item.ValueLabel}" : $"[{item.OptionName} {item.ValueLabel}]"));

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
                $"{(item.Required ? "Required" : "Optional")}. {item.TargetEntityName} id for {item.ColumnName}.")));

        return new CliCommandDefinition(
            spec.CommandName,
            spec.Description,
            new[] { string.Join(" ", usageParts) },
            options,
            new[]
            {
                $"Adds one {spec.EntityName} row to a MetaAnalytics workspace.",
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
            var request = new AnalyticsAuthoringRequest
            {
                WorkspacePath = Path.GetFullPath(parse.WorkspacePath),
                EntityName = spec.EntityName,
                RecordId = parse.RecordId,
            };
            foreach (var value in parse.Values) request.Values[value.Key] = value.Value;
            request.Relationships.AddRange(parse.Relationships);
            await new AnalyticsAuthoringService().AddRecordAsync(request).ConfigureAwait(false);

            Presenter.WriteOk($"Added {parse.RecordId} to {spec.EntityName}");
            return 0;
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or IOException or UnauthorizedAccessException)
        {
            return Fail(
                "Cannot update analytics workspace.",
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
        var relationships = new List<AnalyticsRelationshipAssignment>();
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
                relationships.Add(new AnalyticsRelationshipAssignment(rel.ColumnName, rel.TargetEntityName, value));
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

        foreach (var rel in spec.RelationshipOptions.Where(item => item.Required))
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
