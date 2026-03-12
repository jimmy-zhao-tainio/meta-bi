using System.Text;
using MS = MetaSchema;

namespace MetaDataVault.Core;

public sealed partial class MetaSchemaToRawDataVaultConverter
{
    private static string BuildMaterializationReport(
        SchemaBootstrapDraft draft,
        SourceIndex sourceIndex,
        IReadOnlyList<TableMaterializationReportRow> tableReportRows,
        IReadOnlyList<RelationshipMaterializationReportRow> relationshipReportRows,
        MetaSchemaBootstrapOptions options)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Materialization Summary");
        builder.AppendLine();
        builder.AppendLine("Summary");
        builder.AppendLine($"Source systems: {sourceIndex.IncludedSystems.Count}");
        builder.AppendLine($"Source schemas: {sourceIndex.IncludedSchemas.Count}");
        builder.AppendLine($"Source tables: {sourceIndex.IncludedTables.Count}");
        builder.AppendLine($"Source relationships: {sourceIndex.IncludedRelationships.Count}");
        builder.AppendLine($"Raw hubs: {draft.RawHubs.Count}");
        builder.AppendLine($"Raw hub key parts: {draft.RawHubKeyParts.Count}");
        builder.AppendLine($"Raw links: {draft.RawLinks.Count}");
        builder.AppendLine($"Raw hub satellites: {draft.RawHubSatellites.Count}");
        builder.AppendLine($"Raw hub satellite attributes: {draft.RawHubSatelliteAttributes.Count}");
        builder.AppendLine($"Ignored field names: {FormatListOrNone(options.IgnoredFieldNames)}");
        builder.AppendLine($"Ignored field suffixes: {FormatListOrNone(options.IgnoredFieldSuffixes)}");
        builder.AppendLine($"Included views: {(options.IncludeViews ? "yes" : "no")}");
        builder.AppendLine();
        builder.AppendLine("Tables");

        foreach (var tableRow in tableReportRows.OrderBy(row => BuildQualifiedTableName(row.Table, sourceIndex.SchemaById), StringComparer.OrdinalIgnoreCase))
        {
            builder.AppendLine(BuildQualifiedTableName(tableRow.Table, sourceIndex.SchemaById));
            builder.AppendLine($"  selected key: {BuildSelectedKeyText(tableRow.KeyAssessment)}");
            builder.AppendLine($"  hub created: {(tableRow.HubCreated ? "yes" : "no")}");
            builder.AppendLine($"  satellite created: {(tableRow.SatelliteAttributeCount > 0 ? "yes" : "no")}");
            builder.AppendLine($"  satellite attribute count: {tableRow.SatelliteAttributeCount}");
            if (!tableRow.HubCreated && !string.IsNullOrWhiteSpace(tableRow.KeyAssessment?.SkipReason))
            {
                builder.AppendLine($"  reason: {tableRow.KeyAssessment.SkipReason}");
            }
            builder.AppendLine();
        }

        builder.AppendLine("Relationships");
        foreach (var relationshipRow in relationshipReportRows.OrderBy(row => BuildRelationshipTitle(row.Relationship, row.SourceTable, row.TargetTable), StringComparer.OrdinalIgnoreCase))
        {
            builder.AppendLine(BuildRelationshipTitle(relationshipRow.Relationship, relationshipRow.SourceTable, relationshipRow.TargetTable));
            builder.AppendLine($"  link created: {(relationshipRow.LinkCreated ? "yes" : "no")}");
            if (!relationshipRow.LinkCreated && !string.IsNullOrWhiteSpace(relationshipRow.SkipReason))
            {
                builder.AppendLine($"  reason: {relationshipRow.SkipReason}");
            }
            builder.AppendLine();
        }

        return builder.ToString().TrimEnd() + Environment.NewLine;
    }

    private static string BuildRelationshipSkipReason(string? sourceHubId, string? targetHubId, MS.Table sourceTable, MS.Table targetTable)
    {
        var reasons = new List<string>();
        if (string.IsNullOrWhiteSpace(sourceHubId))
        {
            reasons.Add($"source table `{sourceTable.Name}` did not materialize to a hub");
        }

        if (string.IsNullOrWhiteSpace(targetHubId))
        {
            reasons.Add($"target table `{targetTable.Name}` did not materialize to a hub");
        }

        return reasons.Count == 0
            ? string.Empty
            : string.Join("; ", reasons);
    }

    private static string BuildQualifiedTableName(MS.Table table, IReadOnlyDictionary<string, MS.Schema> schemaById)
    {
        if (schemaById.TryGetValue(table.SchemaId, out var schema) && !string.IsNullOrWhiteSpace(schema.Name))
        {
            return schema.Name + "." + table.Name;
        }

        return table.Name;
    }

    private static string BuildSelectedKeyText(TableKeyAssessment? keyAssessment)
    {
        if (keyAssessment?.SelectedKey == null)
        {
            return "none";
        }

        var key = keyAssessment.SelectedKey;
        var fieldNames = key.OrderedKeyFields
            .Select(record => record.FieldName)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToList();
        var keyType = GetKeyTypeLabel(key.TableKey.KeyType);
        return string.IsNullOrWhiteSpace(key.TableKey.Name)
            ? $"{keyType} -> `{string.Join("`, `", fieldNames)}`"
            : $"{keyType} `{key.TableKey.Name}` -> `{string.Join("`, `", fieldNames)}`";
    }

    private static string GetKeyTypeLabel(string keyType)
    {
        return keyType switch
        {
            "primary" => "primary key",
            "unique" => "unique key",
            _ => "key"
        };
    }

    private static string BuildRelationshipTitle(MS.TableRelationship relationship, MS.Table sourceTable, MS.Table targetTable)
    {
        return $"{BuildStructuralLinkName(sourceTable, targetTable)} ({sourceTable.Name} -> {targetTable.Name})";
    }

    private static string FormatListOrNone(IEnumerable<string> values)
    {
        var materialized = values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
            .ThenBy(value => value, StringComparer.Ordinal)
            .ToList();

        return materialized.Count == 0
            ? "(none)"
            : string.Join(", ", materialized);
    }
}
