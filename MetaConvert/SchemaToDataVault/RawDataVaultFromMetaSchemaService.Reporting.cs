using System.Globalization;
using MS = global::MetaSchema;
using MRDV = global::MetaRawDataVault;

namespace MetaConvert.SchemaToDataVault;

public sealed partial class RawDataVaultFromMetaSchemaService
{
    private static RawDataVaultFromMetaSchemaReport BuildReport(
        MS.MetaSchemaModel source,
        MRDV.MetaRawDataVaultModel target,
        OptionsInput options)
    {
        var includedTables = source.TableList
            .Select(table => table.SchemaObject)
            .Concat(options.IncludeViews
                ? source.ViewList.Select(view => view.SchemaObject)
                : [])
            .OrderBy(table => table.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(table => table.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(table => table.Id, StringComparer.Ordinal)
            .ToList();
        var includedTableIds = includedTables
            .Select(table => table.Id)
            .ToHashSet(StringComparer.Ordinal);
        var includedSchemas = source.SchemaList
            .Where(schema => includedTables.Any(table =>
                string.Equals(table.Schema.Id, schema.Id, StringComparison.Ordinal)))
            .ToList();
        var includedSystems = source.SystemList
            .Where(system => includedSchemas.Any(schema =>
                string.Equals(schema.System.Id, system.Id, StringComparison.Ordinal)))
            .ToList();
        var includedRelationships = source.TableRelationshipList
            .Where(relationship =>
                includedTableIds.Contains(relationship.SourceTable.SchemaObject.Id) &&
                includedTableIds.Contains(relationship.TargetTable.SchemaObject.Id))
            .ToList();
        var rawHubIds = target.RawHubList
            .Select(hub => hub.Id)
            .ToHashSet(StringComparer.Ordinal);

        var tableReports = includedTables
            .Select(table => BuildTableReport(source, target, table, options))
            .OrderBy(report => report.QualifiedTableName, StringComparer.OrdinalIgnoreCase)
            .ToList();
        var relationshipReports = includedRelationships
            .Select(relationship => BuildRelationshipReport(target, rawHubIds, relationship))
            .OrderBy(
                report => $"{BuildStructuralLinkName(report.SourceTableName, report.TargetTableName)} ({report.SourceTableName} -> {report.TargetTableName})",
                StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new RawDataVaultFromMetaSchemaReport(
            new RawDataVaultFromMetaSchemaSummary(
                SourceSystemCount: includedSystems.Count,
                SourceSchemaCount: includedSchemas.Count,
                SourceTableCount: includedTables.Count,
                SourceRelationshipCount: includedRelationships.Count,
                RawHubCount: target.RawHubList.Count,
                RawHubKeyPartCount: target.RawHubKeyPartList.Count,
                RawLinkCount: target.RawLinkList.Count,
                RawHubSatelliteCount: target.RawHubSatelliteList.Count,
                RawHubSatelliteAttributeCount: target.RawHubSatelliteAttributeList.Count,
                IgnoredFieldNames: MaterializeOptionList(options.IgnoredFieldNames),
                IgnoredFieldSuffixes: MaterializeOptionList(options.IgnoredFieldSuffixes),
                IncludeViews: options.IncludeViews),
            tableReports,
            relationshipReports);
    }

    private static RawDataVaultFromMetaSchemaTableReport BuildTableReport(
        MS.MetaSchemaModel source,
        MRDV.MetaRawDataVaultModel target,
        MS.SchemaObject table,
        OptionsInput options)
    {
        var hubId = "rawhub:" + table.Id;
        var hubCreated = target.RawHubList.Any(hub =>
            string.Equals(hub.Id, hubId, StringComparison.Ordinal));
        var sourceKeys = source.KeyList
            .Where(key => string.Equals(
                key.Table.SchemaObject.Id,
                table.Id,
                StringComparison.Ordinal))
            .OrderBy(key => GetKeyPriority(source, key.Id))
            .ThenBy(key => key.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(key => key.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(key => key.Id, StringComparer.Ordinal)
            .ToList();
        var selectedKey = sourceKeys
            .Select(key => new
            {
                Key = key,
                Fields = source.KeyFieldList
                    .Where(keyField => string.Equals(keyField.Key.Id, key.Id, StringComparison.Ordinal))
                    .Where(keyField => !ShouldIgnoreField(keyField.Field.Name, options))
                    .OrderBy(keyField => ParseInt32(keyField.Ordinal, int.MaxValue))
                    .ThenBy(keyField => keyField.Id, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(keyField => keyField.Id, StringComparer.Ordinal)
                    .ToList(),
            })
            .FirstOrDefault(candidate => candidate.Fields.Count > 0);
        var selectedKeyReport = selectedKey is null
            ? null
            : new RawDataVaultFromMetaSchemaSelectedKeyReport(
                source.PrimaryKeyList.Any(primaryKey =>
                    string.Equals(primaryKey.Key.Id, selectedKey.Key.Id, StringComparison.Ordinal))
                    ? "primary"
                    : "unique",
                string.IsNullOrWhiteSpace(selectedKey.Key.Name) ? null : selectedKey.Key.Name,
                selectedKey.Fields
                    .Select(keyField => keyField.Field.Name)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .ToList());
        var satelliteIds = target.RawHubSatelliteList
            .Where(satellite => string.Equals(satellite.RawHub.Id, hubId, StringComparison.Ordinal))
            .Select(satellite => satellite.Id)
            .ToHashSet(StringComparer.Ordinal);
        var satelliteAttributeCount = target.RawHubSatelliteAttributeList.Count(attribute =>
            satelliteIds.Contains(attribute.RawHubSatellite.Id));
        var reason = hubCreated
            ? null
            : sourceKeys.Count == 0
                ? "no source primary or unique key metadata was available"
                : "all source key fields were excluded by explicit ignore options";

        return new RawDataVaultFromMetaSchemaTableReport(
            QualifiedTableName: string.IsNullOrWhiteSpace(table.Schema.Name)
                ? table.Name
                : table.Schema.Name + "." + table.Name,
            SelectedKey: selectedKeyReport,
            HubCreated: hubCreated,
            SatelliteAttributeCount: satelliteAttributeCount,
            Reason: reason);
    }

    private static RawDataVaultFromMetaSchemaRelationshipReport BuildRelationshipReport(
        MRDV.MetaRawDataVaultModel target,
        ISet<string> rawHubIds,
        MS.TableRelationship relationship)
    {
        var sourceTable = relationship.SourceTable.SchemaObject;
        var targetTable = relationship.TargetTable.SchemaObject;
        var structuralName = BuildStructuralLinkName(sourceTable.Name, targetTable.Name);
        var rawLinkId = "rawlink:" + relationship.Id;
        var rawLink = target.RawLinkList.FirstOrDefault(link =>
            string.Equals(link.Id, rawLinkId, StringComparison.Ordinal));
        var missingHubs = new List<string>();
        if (!rawHubIds.Contains("rawhub:" + sourceTable.Id))
        {
            missingHubs.Add($"source table `{sourceTable.Name}` did not materialize to a hub");
        }
        if (!rawHubIds.Contains("rawhub:" + targetTable.Id))
        {
            missingHubs.Add($"target table `{targetTable.Name}` did not materialize to a hub");
        }

        return new RawDataVaultFromMetaSchemaRelationshipReport(
            RawLinkName: rawLink?.Name ?? structuralName,
            SourceTableName: sourceTable.Name,
            TargetTableName: targetTable.Name,
            LinkCreated: rawLink is not null,
            NameWasDisambiguated: rawLink is not null &&
                                  !string.Equals(rawLink.Name, structuralName, StringComparison.Ordinal),
            Reason: rawLink is null && missingHubs.Count > 0
                ? string.Join("; ", missingHubs)
                : null);
    }

    private static int GetKeyPriority(MS.MetaSchemaModel source, string keyId)
    {
        if (source.PrimaryKeyList.Any(primaryKey =>
                string.Equals(primaryKey.Key.Id, keyId, StringComparison.Ordinal)))
        {
            return 0;
        }

        return source.UniqueKeyList.Any(uniqueKey =>
            string.Equals(uniqueKey.Key.Id, keyId, StringComparison.Ordinal))
            ? 1
            : 2;
    }

    private static bool ShouldIgnoreField(string fieldName, OptionsInput options)
        => options.IgnoredFieldNames.Contains(fieldName, StringComparer.OrdinalIgnoreCase) ||
           options.IgnoredFieldSuffixes.Any(suffix =>
               fieldName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));

    private static int ParseInt32(string? value, int defaultValue)
        => int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : defaultValue;

    private static string BuildStructuralLinkName(string sourceTableName, string targetTableName)
        => string.Equals(sourceTableName, targetTableName, StringComparison.OrdinalIgnoreCase)
            ? "Source" + sourceTableName + "Target" + targetTableName
            : sourceTableName + targetTableName;

    private static IReadOnlyList<string> MaterializeOptionList(IEnumerable<string> values)
        => values
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
            .ThenBy(value => value, StringComparer.Ordinal)
            .ToList();
}
