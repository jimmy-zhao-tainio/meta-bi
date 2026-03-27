using Meta.Core.Presentation;
using MetaDataVault.Core;

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

    private sealed record ParsedAddCommand(bool Ok, string WorkspacePath, string RecordId, Dictionary<string, string> Values, List<BusinessDataVaultRelationshipAssignment> Relationships, string ErrorMessage);

    private static readonly IReadOnlyDictionary<string, AddCommandSpec> AddCommands = BuildAddCommands();

    private static IReadOnlyDictionary<string, AddCommandSpec> BuildAddCommands()
    {
        AddCommandSpec Cmd(string command, string entity, string description, AddOptionSpec[] props, AddRelationshipOptionSpec[] rels) => new(command, entity, description, props, rels);
        var specs = new[]
        {
            Cmd("add-hub", "BusinessHub", "Add a business hub.", [ new("--name", "Name", true), new("--description", "Description", false) ], []),
            Cmd("add-hub-key-part", "BusinessHubKeyPart", "Add a business hub key part.", [ new("--name", "Name", true), new("--data-type-id", "DataTypeId", true), new("--ordinal", "Ordinal", true) ], [ new("--hub", "BusinessHubId", "BusinessHub") ]),
            Cmd("add-hub-key-part-data-type-detail", "BusinessHubKeyPartDataTypeDetail", "Add a business hub key-part datatype detail.", [ new("--name", "Name", true), new("--value", "Value", true) ], [ new("--hub-key-part", "BusinessHubKeyPartId", "BusinessHubKeyPart") ]),
            Cmd("add-link", "BusinessLink", "Add a standard business link.", [ new("--name", "Name", true), new("--description", "Description", false) ], []),
            Cmd("add-link-hub", "BusinessLinkHub", "Add a participating hub to a standard business link.", [ new("--ordinal", "Ordinal", true), new("--role-name", "RoleName", false) ], [ new("--link", "BusinessLinkId", "BusinessLink"), new("--hub", "BusinessHubId", "BusinessHub") ]),
            Cmd("add-same-as-link", "BusinessSameAsLink", "Add a same-as link.", [ new("--name", "Name", true), new("--description", "Description", false) ], [ new("--primary-hub", "PrimaryHubId", "BusinessHub"), new("--equivalent-hub", "EquivalentHubId", "BusinessHub") ]),
            Cmd("add-hierarchical-link", "BusinessHierarchicalLink", "Add a hierarchical link.", [ new("--name", "Name", true), new("--description", "Description", false) ], [ new("--parent-hub", "ParentHubId", "BusinessHub"), new("--child-hub", "ChildHubId", "BusinessHub") ]),
            Cmd("add-reference", "BusinessReference", "Add a business reference.", [ new("--name", "Name", true), new("--description", "Description", false) ], []),
            Cmd("add-reference-key-part", "BusinessReferenceKeyPart", "Add a business reference key part.", [ new("--name", "Name", true), new("--data-type-id", "DataTypeId", true), new("--ordinal", "Ordinal", true) ], [ new("--reference", "BusinessReferenceId", "BusinessReference") ]),
            Cmd("add-reference-key-part-data-type-detail", "BusinessReferenceKeyPartDataTypeDetail", "Add a business reference key-part datatype detail.", [ new("--name", "Name", true), new("--value", "Value", true) ], [ new("--reference-key-part", "BusinessReferenceKeyPartId", "BusinessReferenceKeyPart") ]),
            Cmd("add-hub-satellite", "BusinessHubSatellite", "Add a business hub satellite.", [ new("--name", "Name", true), new("--description", "Description", false) ], [ new("--hub", "BusinessHubId", "BusinessHub") ]),
            Cmd("add-hub-satellite-attribute", "BusinessHubSatelliteAttribute", "Add a business hub satellite attribute.", [ new("--name", "Name", true), new("--data-type-id", "DataTypeId", true), new("--ordinal", "Ordinal", true) ], [ new("--hub-satellite", "BusinessHubSatelliteId", "BusinessHubSatellite") ]),
            Cmd("add-hub-satellite-attribute-data-type-detail", "BusinessHubSatelliteAttributeDataTypeDetail", "Add a business hub satellite attribute datatype detail.", [ new("--name", "Name", true), new("--value", "Value", true) ], [ new("--hub-satellite-attribute", "BusinessHubSatelliteAttributeId", "BusinessHubSatelliteAttribute") ]),
            Cmd("add-link-satellite", "BusinessLinkSatellite", "Add a business link satellite.", [ new("--name", "Name", true), new("--description", "Description", false) ], [ new("--link", "BusinessLinkId", "BusinessLink") ]),
            Cmd("add-link-satellite-attribute", "BusinessLinkSatelliteAttribute", "Add a business link satellite attribute.", [ new("--name", "Name", true), new("--data-type-id", "DataTypeId", true), new("--ordinal", "Ordinal", true) ], [ new("--link-satellite", "BusinessLinkSatelliteId", "BusinessLinkSatellite") ]),
            Cmd("add-link-satellite-attribute-data-type-detail", "BusinessLinkSatelliteAttributeDataTypeDetail", "Add a business link satellite attribute datatype detail.", [ new("--name", "Name", true), new("--value", "Value", true) ], [ new("--link-satellite-attribute", "BusinessLinkSatelliteAttributeId", "BusinessLinkSatelliteAttribute") ]),
            Cmd("add-same-as-link-satellite", "BusinessSameAsLinkSatellite", "Add a same-as link satellite.", [ new("--name", "Name", true), new("--description", "Description", false) ], [ new("--same-as-link", "BusinessSameAsLinkId", "BusinessSameAsLink") ]),
            Cmd("add-same-as-link-satellite-attribute", "BusinessSameAsLinkSatelliteAttribute", "Add a same-as link satellite attribute.", [ new("--name", "Name", true), new("--data-type-id", "DataTypeId", true), new("--ordinal", "Ordinal", true) ], [ new("--same-as-link-satellite", "BusinessSameAsLinkSatelliteId", "BusinessSameAsLinkSatellite") ]),
            Cmd("add-same-as-link-satellite-attribute-data-type-detail", "BusinessSameAsLinkSatelliteAttributeDataTypeDetail", "Add a same-as link satellite attribute datatype detail.", [ new("--name", "Name", true), new("--value", "Value", true) ], [ new("--same-as-link-satellite-attribute", "BusinessSameAsLinkSatelliteAttributeId", "BusinessSameAsLinkSatelliteAttribute") ]),
            Cmd("add-hierarchical-link-satellite", "BusinessHierarchicalLinkSatellite", "Add a hierarchical link satellite.", [ new("--name", "Name", true), new("--description", "Description", false) ], [ new("--hierarchical-link", "BusinessHierarchicalLinkId", "BusinessHierarchicalLink") ]),
            Cmd("add-hierarchical-link-satellite-attribute", "BusinessHierarchicalLinkSatelliteAttribute", "Add a hierarchical link satellite attribute.", [ new("--name", "Name", true), new("--data-type-id", "DataTypeId", true), new("--ordinal", "Ordinal", true) ], [ new("--hierarchical-link-satellite", "BusinessHierarchicalLinkSatelliteId", "BusinessHierarchicalLinkSatellite") ]),
            Cmd("add-hierarchical-link-satellite-attribute-data-type-detail", "BusinessHierarchicalLinkSatelliteAttributeDataTypeDetail", "Add a hierarchical link satellite attribute datatype detail.", [ new("--name", "Name", true), new("--value", "Value", true) ], [ new("--hierarchical-link-satellite-attribute", "BusinessHierarchicalLinkSatelliteAttributeId", "BusinessHierarchicalLinkSatelliteAttribute") ]),
            Cmd("add-reference-satellite", "BusinessReferenceSatellite", "Add a business reference satellite.", [ new("--name", "Name", true), new("--description", "Description", false) ], [ new("--reference", "BusinessReferenceId", "BusinessReference") ]),
            Cmd("add-reference-satellite-attribute", "BusinessReferenceSatelliteAttribute", "Add a business reference satellite attribute.", [ new("--name", "Name", true), new("--data-type-id", "DataTypeId", true), new("--ordinal", "Ordinal", true) ], [ new("--reference-satellite", "BusinessReferenceSatelliteId", "BusinessReferenceSatellite") ]),
            Cmd("add-reference-satellite-attribute-data-type-detail", "BusinessReferenceSatelliteAttributeDataTypeDetail", "Add a business reference satellite attribute datatype detail.", [ new("--name", "Name", true), new("--value", "Value", true) ], [ new("--reference-satellite-attribute", "BusinessReferenceSatelliteAttributeId", "BusinessReferenceSatelliteAttribute") ]),
            Cmd("add-point-in-time", "BusinessPointInTime", "Add a business point-in-time table.", [ new("--name", "Name", true), new("--description", "Description", false) ], [ new("--hub", "BusinessHubId", "BusinessHub") ]),
            Cmd("add-point-in-time-stamp", "BusinessPointInTimeStamp", "Add a business point-in-time stamp column.", [ new("--name", "Name", true), new("--data-type-id", "DataTypeId", true), new("--ordinal", "Ordinal", true) ], [ new("--point-in-time", "BusinessPointInTimeId", "BusinessPointInTime") ]),
            Cmd("add-point-in-time-stamp-data-type-detail", "BusinessPointInTimeStampDataTypeDetail", "Add a business point-in-time stamp datatype detail.", [ new("--name", "Name", true), new("--value", "Value", true) ], [ new("--point-in-time-stamp", "BusinessPointInTimeStampId", "BusinessPointInTimeStamp") ]),
            Cmd("add-point-in-time-hub-satellite", "BusinessPointInTimeHubSatellite", "Add a hub-satellite reference to a point-in-time table.", [ new("--ordinal", "Ordinal", true) ], [ new("--point-in-time", "BusinessPointInTimeId", "BusinessPointInTime"), new("--hub-satellite", "BusinessHubSatelliteId", "BusinessHubSatellite") ]),
            Cmd("add-point-in-time-link-satellite", "BusinessPointInTimeLinkSatellite", "Add a link-satellite reference to a point-in-time table.", [ new("--ordinal", "Ordinal", true) ], [ new("--point-in-time", "BusinessPointInTimeId", "BusinessPointInTime"), new("--link-satellite", "BusinessLinkSatelliteId", "BusinessLinkSatellite") ]),
            Cmd("add-bridge", "BusinessBridge", "Add a business bridge.", [ new("--name", "Name", true), new("--description", "Description", false) ], [ new("--anchor-hub", "AnchorHubId", "BusinessHub") ]),
            Cmd("add-bridge-link", "BusinessBridgeLink", "Add a business link to an ordered bridge path.", [ new("--ordinal", "Ordinal", true), new("--role-name", "RoleName", false) ], [ new("--bridge", "BusinessBridgeId", "BusinessBridge"), new("--link", "BusinessLinkId", "BusinessLink") ]),
            Cmd("add-bridge-hub", "BusinessBridgeHub", "Add a business hub to an ordered bridge path.", [ new("--ordinal", "Ordinal", true), new("--role-name", "RoleName", false) ], [ new("--bridge", "BusinessBridgeId", "BusinessBridge"), new("--hub", "BusinessHubId", "BusinessHub") ]),
        };
        return specs.ToDictionary(spec => spec.CommandName, StringComparer.OrdinalIgnoreCase);
    }

    private static bool TryGetAddCommand(string commandName, out AddCommandSpec? command)
    {
        return AddCommands.TryGetValue(commandName, out command);
    }

    private static IEnumerable<(string Name, string Description)> GetAddCommandCatalog()
    {
        return AddCommands.Values
            .OrderBy(spec => spec.CommandName, StringComparer.Ordinal)
            .Select(spec => (spec.CommandName, spec.Description));
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
            return Fail(parse.ErrorMessage, $"meta-datavault-business {spec.CommandName} --help");
        }

        try
        {
            var request = new BusinessDataVaultAuthoringRequest
            {
                WorkspacePath = Path.GetFullPath(parse.WorkspacePath),
                EntityName = spec.EntityName,
                RecordId = parse.RecordId,
            };

            foreach (var value in parse.Values)
            {
                request.Values[value.Key] = value.Value;
            }

            request.Relationships.AddRange(parse.Relationships);

            await new BusinessDataVaultAuthoringService().AddRecordAsync(request).ConfigureAwait(false);

            Presenter.WriteOk(
                "business datavault row added",
                ("Workspace", Path.GetFullPath(parse.WorkspacePath)),
                ("Entity", spec.EntityName),
                ("Id", parse.RecordId));
            return 0;
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or IOException or UnauthorizedAccessException)
        {
            return Fail(ex.Message, $"meta-datavault-business {spec.CommandName} --help", 4);
        }
    }

    private static ParsedAddCommand ParseAddCommand(AddCommandSpec spec, string[] args, int startIndex)
    {
        var workspacePath = ".";
        var recordId = string.Empty;
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var relationships = new List<BusinessDataVaultRelationshipAssignment>();
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
                relationships.Add(new BusinessDataVaultRelationshipAssignment(rel.ColumnName, rel.TargetEntityName, value));
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
        var parts = new List<string> { $"meta-datavault-business {spec.CommandName}", "[--workspace <path>]", "--id <id>" };
        parts.AddRange(spec.PropertyOptions.Select(item => item.Required ? $"{item.OptionName} {item.ValueLabel}" : $"[{item.OptionName} {item.ValueLabel}]") );
        parts.AddRange(spec.RelationshipOptions.Select(item => $"{item.OptionName} {item.ValueLabel}"));
        Presenter.WriteInfo($"Command: {spec.CommandName}");
        Presenter.WriteUsage(string.Join(" ", parts));
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo($"  Adds one {spec.EntityName} row to a MetaBusinessDataVault workspace.");
        Presenter.WriteInfo("  Defaults to the current working directory when --workspace is omitted.");
    }
}
