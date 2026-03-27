using Meta.Core.Presentation;
using MetaDataVault.Core;

internal static partial class Program
{
    private sealed record AddOptionSpec(string OptionName, string PropertyName, bool Required, string ValueLabel = "<value>");
    private sealed record AddRelationshipOptionSpec(string OptionName, string ColumnName, string TargetEntityName, string ValueLabel = "<id>");
    private sealed record DataTypeDetailSpec(string DetailEntityName, string ParentRelationshipColumnName, string ParentEntityName);
    private sealed record AddCommandSpec(
        string CommandName,
        string EntityName,
        string Description,
        IReadOnlyList<AddOptionSpec> PropertyOptions,
        IReadOnlyList<AddRelationshipOptionSpec> RelationshipOptions,
        DataTypeDetailSpec? DataTypeDetail = null);

    private sealed record ParsedAddCommand(
        bool Ok,
        string WorkspacePath,
        string RecordId,
        Dictionary<string, string> Values,
        List<BusinessDataVaultRelationshipAssignment> Relationships,
        Dictionary<string, string> DataTypeDetails,
        string ErrorMessage);

    private static readonly IReadOnlyDictionary<string, AddCommandSpec> AddCommands = BuildAddCommands();

    private static IReadOnlyDictionary<string, AddCommandSpec> BuildAddCommands()
    {
        AddCommandSpec Cmd(string command, string entity, string description, AddOptionSpec[] props, AddRelationshipOptionSpec[] rels, bool supportsDataTypeDetails = false)
            => new(
                command,
                entity,
                description,
                props,
                rels,
                supportsDataTypeDetails
                    ? new DataTypeDetailSpec($"{entity}DataTypeDetail", $"{entity}Id", entity)
                    : null);
        var specs = new[]
        {
            Cmd("add-hub", "BusinessHub", "Add a business hub.", [ new("--name", "Name", true), new("--description", "Description", false) ], []),
            Cmd("add-hub-key-part", "BusinessHubKeyPart", "Add a business hub key part.", [ new("--name", "Name", true), new("--data-type-id", "DataTypeId", true), new("--ordinal", "Ordinal", true) ], [ new("--hub", "BusinessHubId", "BusinessHub") ], supportsDataTypeDetails: true),
            Cmd("add-link", "BusinessLink", "Add a standard business link.", [ new("--name", "Name", true), new("--description", "Description", false) ], []),
            Cmd("add-link-hub", "BusinessLinkHub", "Add a participating hub to a standard business link.", [ new("--ordinal", "Ordinal", true), new("--role-name", "RoleName", false) ], [ new("--link", "BusinessLinkId", "BusinessLink"), new("--hub", "BusinessHubId", "BusinessHub") ]),
            Cmd("add-same-as-link", "BusinessSameAsLink", "Add a same-as link.", [ new("--name", "Name", true), new("--description", "Description", false) ], [ new("--primary-hub", "PrimaryHubId", "BusinessHub"), new("--equivalent-hub", "EquivalentHubId", "BusinessHub") ]),
            Cmd("add-hierarchical-link", "BusinessHierarchicalLink", "Add a hierarchical link.", [ new("--name", "Name", true), new("--description", "Description", false) ], [ new("--parent-hub", "ParentHubId", "BusinessHub"), new("--child-hub", "ChildHubId", "BusinessHub") ]),
            Cmd("add-reference", "BusinessReference", "Add a business reference.", [ new("--name", "Name", true), new("--description", "Description", false) ], []),
            Cmd("add-reference-key-part", "BusinessReferenceKeyPart", "Add a business reference key part.", [ new("--name", "Name", true), new("--data-type-id", "DataTypeId", true), new("--ordinal", "Ordinal", true) ], [ new("--reference", "BusinessReferenceId", "BusinessReference") ], supportsDataTypeDetails: true),
            Cmd("add-hub-satellite", "BusinessHubSatellite", "Add a business hub satellite.", [ new("--name", "Name", true), new("--description", "Description", false) ], [ new("--hub", "BusinessHubId", "BusinessHub") ]),
            Cmd("add-hub-satellite-attribute", "BusinessHubSatelliteAttribute", "Add a business hub satellite attribute.", [ new("--name", "Name", true), new("--data-type-id", "DataTypeId", true), new("--ordinal", "Ordinal", true) ], [ new("--hub-satellite", "BusinessHubSatelliteId", "BusinessHubSatellite") ], supportsDataTypeDetails: true),
            Cmd("add-link-satellite", "BusinessLinkSatellite", "Add a business link satellite.", [ new("--name", "Name", true), new("--description", "Description", false) ], [ new("--link", "BusinessLinkId", "BusinessLink") ]),
            Cmd("add-link-satellite-attribute", "BusinessLinkSatelliteAttribute", "Add a business link satellite attribute.", [ new("--name", "Name", true), new("--data-type-id", "DataTypeId", true), new("--ordinal", "Ordinal", true) ], [ new("--link-satellite", "BusinessLinkSatelliteId", "BusinessLinkSatellite") ], supportsDataTypeDetails: true),
            Cmd("add-same-as-link-satellite", "BusinessSameAsLinkSatellite", "Add a same-as link satellite.", [ new("--name", "Name", true), new("--description", "Description", false) ], [ new("--same-as-link", "BusinessSameAsLinkId", "BusinessSameAsLink") ]),
            Cmd("add-same-as-link-satellite-attribute", "BusinessSameAsLinkSatelliteAttribute", "Add a same-as link satellite attribute.", [ new("--name", "Name", true), new("--data-type-id", "DataTypeId", true), new("--ordinal", "Ordinal", true) ], [ new("--same-as-link-satellite", "BusinessSameAsLinkSatelliteId", "BusinessSameAsLinkSatellite") ], supportsDataTypeDetails: true),
            Cmd("add-hierarchical-link-satellite", "BusinessHierarchicalLinkSatellite", "Add a hierarchical link satellite.", [ new("--name", "Name", true), new("--description", "Description", false) ], [ new("--hierarchical-link", "BusinessHierarchicalLinkId", "BusinessHierarchicalLink") ]),
            Cmd("add-hierarchical-link-satellite-attribute", "BusinessHierarchicalLinkSatelliteAttribute", "Add a hierarchical link satellite attribute.", [ new("--name", "Name", true), new("--data-type-id", "DataTypeId", true), new("--ordinal", "Ordinal", true) ], [ new("--hierarchical-link-satellite", "BusinessHierarchicalLinkSatelliteId", "BusinessHierarchicalLinkSatellite") ], supportsDataTypeDetails: true),
            Cmd("add-reference-satellite", "BusinessReferenceSatellite", "Add a business reference satellite.", [ new("--name", "Name", true), new("--description", "Description", false) ], [ new("--reference", "BusinessReferenceId", "BusinessReference") ]),
            Cmd("add-reference-satellite-attribute", "BusinessReferenceSatelliteAttribute", "Add a business reference satellite attribute.", [ new("--name", "Name", true), new("--data-type-id", "DataTypeId", true), new("--ordinal", "Ordinal", true) ], [ new("--reference-satellite", "BusinessReferenceSatelliteId", "BusinessReferenceSatellite") ], supportsDataTypeDetails: true),
            Cmd("add-point-in-time", "BusinessPointInTime", "Add a business point-in-time table.", [ new("--name", "Name", true), new("--description", "Description", false) ], [ new("--hub", "BusinessHubId", "BusinessHub") ]),
            Cmd("add-point-in-time-stamp", "BusinessPointInTimeStamp", "Add a business point-in-time stamp column.", [ new("--name", "Name", true), new("--data-type-id", "DataTypeId", true), new("--ordinal", "Ordinal", true) ], [ new("--point-in-time", "BusinessPointInTimeId", "BusinessPointInTime") ], supportsDataTypeDetails: true),
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

            var service = new BusinessDataVaultAuthoringService();
            await service.AddRecordAsync(request).ConfigureAwait(false);

            foreach (var detailRequest in BuildDataTypeDetailRequests(spec, parse))
            {
                await service.AddRecordAsync(detailRequest).ConfigureAwait(false);
            }

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
        var dataTypeDetails = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var propertyOptions = spec.PropertyOptions.ToDictionary(item => item.OptionName, StringComparer.OrdinalIgnoreCase);
        var relationshipOptions = spec.RelationshipOptions.ToDictionary(item => item.OptionName, StringComparer.OrdinalIgnoreCase);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (i + 1 >= args.Length)
            {
                return new ParsedAddCommand(false, workspacePath, recordId, values, relationships, dataTypeDetails, $"missing value for {arg}.");
            }

            var value = args[++i];
            if (!seen.Add(arg))
            {
                return new ParsedAddCommand(false, workspacePath, recordId, values, relationships, dataTypeDetails, $"{arg} can only be provided once.");
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
            if (spec.DataTypeDetail != null && TryGetDataTypeDetailName(arg, out var detailName))
            {
                dataTypeDetails[detailName] = value;
                continue;
            }
            if (relationshipOptions.TryGetValue(arg, out var rel))
            {
                relationships.Add(new BusinessDataVaultRelationshipAssignment(rel.ColumnName, rel.TargetEntityName, value));
                continue;
            }

            return new ParsedAddCommand(false, workspacePath, recordId, values, relationships, dataTypeDetails, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(recordId))
        {
            return new ParsedAddCommand(false, workspacePath, recordId, values, relationships, dataTypeDetails, "missing required option --id <id>.");
        }
        foreach (var prop in spec.PropertyOptions.Where(item => item.Required))
        {
            if (!values.ContainsKey(prop.PropertyName) || string.IsNullOrWhiteSpace(values[prop.PropertyName]))
            {
                return new ParsedAddCommand(false, workspacePath, recordId, values, relationships, dataTypeDetails, $"missing required option {prop.OptionName} {prop.ValueLabel}.");
            }
        }
        foreach (var rel in spec.RelationshipOptions)
        {
            if (!relationships.Any(item => string.Equals(item.ColumnName, rel.ColumnName, StringComparison.OrdinalIgnoreCase)))
            {
                return new ParsedAddCommand(false, workspacePath, recordId, values, relationships, dataTypeDetails, $"missing required option {rel.OptionName} {rel.ValueLabel}.");
            }
        }

        return new ParsedAddCommand(true, workspacePath, recordId, values, relationships, dataTypeDetails, string.Empty);
    }

    private static void PrintAddCommandHelp(AddCommandSpec spec)
    {
        var parts = new List<string> { $"meta-datavault-business {spec.CommandName}", "[--workspace <path>]", "--id <id>" };
        parts.AddRange(spec.PropertyOptions.Select(item => item.Required ? $"{item.OptionName} {item.ValueLabel}" : $"[{item.OptionName} {item.ValueLabel}]") );
        parts.AddRange(spec.RelationshipOptions.Select(item => $"{item.OptionName} {item.ValueLabel}"));
        if (spec.DataTypeDetail != null)
        {
            parts.Add("[--length <value>]");
            parts.Add("[--precision <value>]");
            parts.Add("[--scale <value>]");
        }
        Presenter.WriteInfo($"Command: {spec.CommandName}");
        Presenter.WriteUsage(string.Join(" ", parts));
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo($"  Adds one {spec.EntityName} row to a MetaBusinessDataVault workspace.");
        Presenter.WriteInfo("  Defaults to the current working directory when --workspace is omitted.");
        if (spec.DataTypeDetail != null)
        {
            Presenter.WriteInfo("  Optional datatype facets are authored as internal metadata rows.");
        }
    }

    private static IEnumerable<BusinessDataVaultAuthoringRequest> BuildDataTypeDetailRequests(AddCommandSpec spec, ParsedAddCommand parse)
    {
        if (spec.DataTypeDetail == null || parse.DataTypeDetails.Count == 0)
        {
            yield break;
        }

        foreach (var detail in parse.DataTypeDetails.OrderBy(item => item.Key, StringComparer.Ordinal))
        {
            var request = new BusinessDataVaultAuthoringRequest
            {
                WorkspacePath = Path.GetFullPath(parse.WorkspacePath),
                EntityName = spec.DataTypeDetail.DetailEntityName,
                RecordId = BuildDataTypeDetailRecordId(parse.RecordId, detail.Key),
            };
            request.Values["Name"] = detail.Key;
            request.Values["Value"] = detail.Value;
            request.Relationships.Add(new BusinessDataVaultRelationshipAssignment(
                spec.DataTypeDetail.ParentRelationshipColumnName,
                spec.DataTypeDetail.ParentEntityName,
                parse.RecordId));
            yield return request;
        }
    }

    private static string BuildDataTypeDetailRecordId(string parentRecordId, string detailName)
    {
        return $"{parentRecordId}:datatype-detail:{detailName.ToLowerInvariant()}";
    }

    private static bool TryGetDataTypeDetailName(string optionName, out string detailName)
    {
        if (string.Equals(optionName, "--length", StringComparison.OrdinalIgnoreCase))
        {
            detailName = "Length";
            return true;
        }
        if (string.Equals(optionName, "--precision", StringComparison.OrdinalIgnoreCase))
        {
            detailName = "Precision";
            return true;
        }
        if (string.Equals(optionName, "--scale", StringComparison.OrdinalIgnoreCase))
        {
            detailName = "Scale";
            return true;
        }

        detailName = string.Empty;
        return false;
    }
}
