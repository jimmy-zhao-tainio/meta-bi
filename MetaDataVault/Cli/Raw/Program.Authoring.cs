using Meta.Core.Presentation;
using MetaDataVault.Core;

internal static partial class Program
{
    private sealed record AddOptionSpec(string OptionName, string PropertyName, bool Required, string ValueLabel = "<value>");
    private sealed record AddRelationshipOptionSpec(string OptionName, string ColumnName, string TargetEntityName, string ValueLabel = "<id>");
    private sealed record AddCommandSpec(string CommandName, string EntityName, string Description, IReadOnlyList<AddOptionSpec> PropertyOptions, IReadOnlyList<AddRelationshipOptionSpec> RelationshipOptions);
    private sealed record ParsedAddCommand(bool Ok, string WorkspacePath, string RecordId, Dictionary<string, string> Values, List<RawDataVaultRelationshipAssignment> Relationships, string ErrorMessage);

    private static readonly IReadOnlyDictionary<string, AddCommandSpec> AddCommands = BuildAddCommands();

    private static IReadOnlyDictionary<string, AddCommandSpec> BuildAddCommands()
    {
        AddCommandSpec Cmd(string command, string entity, string description, AddOptionSpec[] props, AddRelationshipOptionSpec[] rels) => new(command, entity, description, props, rels);
        var specs = new[]
        {
            Cmd("add-source-system", "SourceSystem", "Add a source system.", [ new("--name", "Name", true), new("--description", "Description", false) ], []),
            Cmd("add-source-schema", "SourceSchema", "Add a source schema.", [ new("--name", "Name", true) ], [ new("--system", "SourceSystemId", "SourceSystem") ]),
            Cmd("add-source-table", "SourceTable", "Add a source table.", [ new("--name", "Name", true) ], [ new("--schema", "SourceSchemaId", "SourceSchema") ]),
            Cmd("add-source-field", "SourceField", "Add a source field.", [ new("--name", "Name", true), new("--data-type-id", "DataTypeId", true), new("--ordinal", "Ordinal", false), new("--is-nullable", "IsNullable", false) ], [ new("--table", "SourceTableId", "SourceTable") ]),
            Cmd("add-source-field-data-type-detail", "SourceFieldDataTypeDetail", "Add a source field datatype detail.", [ new("--name", "Name", true), new("--value", "Value", true) ], [ new("--field", "SourceFieldId", "SourceField") ]),
            Cmd("add-source-table-relationship", "SourceTableRelationship", "Add a source table relationship.", [ new("--name", "Name", true) ], [ new("--source-table", "SourceTableId", "SourceTable"), new("--target-table", "TargetTableId", "SourceTable") ]),
            Cmd("add-source-table-relationship-field", "SourceTableRelationshipField", "Add a source table relationship field.", [ new("--ordinal", "Ordinal", true) ], [ new("--relationship", "SourceTableRelationshipId", "SourceTableRelationship"), new("--source-field", "SourceFieldId", "SourceField"), new("--target-field", "TargetFieldId", "SourceField") ]),
            Cmd("add-hub", "RawHub", "Add a raw hub.", [ new("--name", "Name", true) ], [ new("--source-table", "SourceTableId", "SourceTable") ]),
            Cmd("add-hub-key-part", "RawHubKeyPart", "Add a raw hub key part.", [ new("--name", "Name", true), new("--ordinal", "Ordinal", true) ], [ new("--hub", "RawHubId", "RawHub"), new("--source-field", "SourceFieldId", "SourceField") ]),
            Cmd("add-hub-satellite", "RawHubSatellite", "Add a raw hub satellite.", [ new("--name", "Name", true), new("--satellite-kind", "SatelliteKind", true) ], [ new("--hub", "RawHubId", "RawHub"), new("--source-table", "SourceTableId", "SourceTable") ]),
            Cmd("add-hub-satellite-attribute", "RawHubSatelliteAttribute", "Add a raw hub satellite attribute.", [ new("--name", "Name", true), new("--ordinal", "Ordinal", true) ], [ new("--hub-satellite", "RawHubSatelliteId", "RawHubSatellite"), new("--source-field", "SourceFieldId", "SourceField") ]),
            Cmd("add-link", "RawLink", "Add a raw link.", [ new("--name", "Name", true), new("--link-kind", "LinkKind", false) ], [ new("--source-relationship", "SourceTableRelationshipId", "SourceTableRelationship") ]),
            Cmd("add-link-hub", "RawLinkHub", "Add a participating hub to a raw link.", [ new("--ordinal", "Ordinal", true), new("--role-name", "RoleName", false) ], [ new("--link", "RawLinkId", "RawLink"), new("--hub", "RawHubId", "RawHub") ]),
            Cmd("add-link-satellite", "RawLinkSatellite", "Add a raw link satellite.", [ new("--name", "Name", true), new("--satellite-kind", "SatelliteKind", true) ], [ new("--link", "RawLinkId", "RawLink"), new("--source-table", "SourceTableId", "SourceTable") ]),
            Cmd("add-link-satellite-attribute", "RawLinkSatelliteAttribute", "Add a raw link satellite attribute.", [ new("--name", "Name", true), new("--ordinal", "Ordinal", true) ], [ new("--link-satellite", "RawLinkSatelliteId", "RawLinkSatellite"), new("--source-field", "SourceFieldId", "SourceField") ])
        };
        return specs.ToDictionary(spec => spec.CommandName, StringComparer.OrdinalIgnoreCase);
    }

    private static bool TryGetAddCommand(string commandName, out AddCommandSpec? command) => AddCommands.TryGetValue(commandName, out command);

    private static IEnumerable<(string Name, string Description)> GetAddCommandCatalog() => AddCommands.Values.OrderBy(spec => spec.CommandName, StringComparer.Ordinal).Select(spec => (spec.CommandName, spec.Description));

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
            return Fail(parse.ErrorMessage, $"meta-datavault-raw {spec.CommandName} --help");
        }

        try
        {
            var request = new RawDataVaultAuthoringRequest
            {
                WorkspacePath = Path.GetFullPath(parse.WorkspacePath),
                EntityName = spec.EntityName,
                RecordId = parse.RecordId,
            };
            foreach (var value in parse.Values) request.Values[value.Key] = value.Value;
            request.Relationships.AddRange(parse.Relationships);
            await new RawDataVaultAuthoringService().AddRecordAsync(request).ConfigureAwait(false);

            Presenter.WriteOk($"Added {parse.RecordId} to {spec.EntityName}");
            return 0;
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or IOException or UnauthorizedAccessException)
        {
            return Fail(ex.Message, $"meta-datavault-raw {spec.CommandName} --help", 4);
        }
    }

    private static ParsedAddCommand ParseAddCommand(AddCommandSpec spec, string[] args, int startIndex)
    {
        var workspacePath = ".";
        var recordId = string.Empty;
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var relationships = new List<RawDataVaultRelationshipAssignment>();
        var propertyOptions = spec.PropertyOptions.ToDictionary(item => item.OptionName, StringComparer.OrdinalIgnoreCase);
        var relationshipOptions = spec.RelationshipOptions.ToDictionary(item => item.OptionName, StringComparer.OrdinalIgnoreCase);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (i + 1 >= args.Length) return new ParsedAddCommand(false, workspacePath, recordId, values, relationships, $"missing value for {arg}.");
            var value = args[++i];
            if (!seen.Add(arg)) return new ParsedAddCommand(false, workspacePath, recordId, values, relationships, $"{arg} can only be provided once.");
            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase)) { workspacePath = value; continue; }
            if (string.Equals(arg, "--id", StringComparison.OrdinalIgnoreCase)) { recordId = value; continue; }
            if (propertyOptions.TryGetValue(arg, out var prop)) { values[prop.PropertyName] = value; continue; }
            if (relationshipOptions.TryGetValue(arg, out var rel)) { relationships.Add(new RawDataVaultRelationshipAssignment(rel.ColumnName, rel.TargetEntityName, value)); continue; }
            return new ParsedAddCommand(false, workspacePath, recordId, values, relationships, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(recordId)) return new ParsedAddCommand(false, workspacePath, recordId, values, relationships, "missing required option --id <id>.");
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
        var parts = new List<string> { $"meta-datavault-raw {spec.CommandName}", "[--workspace <path>]", "--id <id>" };
        parts.AddRange(spec.PropertyOptions.Select(item => item.Required ? $"{item.OptionName} {item.ValueLabel}" : $"[{item.OptionName} {item.ValueLabel}]") );
        parts.AddRange(spec.RelationshipOptions.Select(item => $"{item.OptionName} {item.ValueLabel}"));
        Presenter.WriteInfo($"Command: {spec.CommandName}");
        Presenter.WriteUsage(string.Join(" ", parts));
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo($"  Adds one {spec.EntityName} row to a MetaRawDataVault workspace.");
        Presenter.WriteInfo("  Defaults to the current working directory when --workspace is omitted.");
    }
}
