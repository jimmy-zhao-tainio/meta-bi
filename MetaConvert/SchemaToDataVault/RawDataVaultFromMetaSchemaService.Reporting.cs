using MS = global::MetaSchema;

namespace MetaConvert.SchemaToDataVault;

public sealed partial class RawDataVaultFromMetaSchemaService
{
    private static RawDataVaultFromMetaSchemaReport BuildReport(
        FromMetaSchemaDraft draft,
        SourceIndex sourceIndex,
        IReadOnlyList<TableMaterializationReportRow> tableReportRows,
        IReadOnlyList<RelationshipMaterializationReportRow> relationshipReportRows,
        FromMetaSchemaOptions options)
    {
        var summary = new RawDataVaultFromMetaSchemaSummary(
            SourceSystemCount: sourceIndex.IncludedSystems.Count,
            SourceSchemaCount: sourceIndex.IncludedSchemas.Count,
            SourceTableCount: sourceIndex.IncludedTables.Count,
            SourceRelationshipCount: sourceIndex.IncludedRelationships.Count,
            RawHubCount: draft.RawHubs.Count,
            RawHubKeyPartCount: draft.RawHubKeyParts.Count,
            RawLinkCount: draft.RawLinks.Count,
            RawHubSatelliteCount: draft.RawHubSatellites.Count,
            RawHubSatelliteAttributeCount: draft.RawHubSatelliteAttributes.Count,
            IgnoredFieldNames: MaterializeList(options.IgnoredFieldNames),
            IgnoredFieldSuffixes: MaterializeList(options.IgnoredFieldSuffixes),
            IncludeViews: options.IncludeViews);

        var tables = tableReportRows
            .OrderBy(row => BuildQualifiedTableName(row.Table, sourceIndex.SchemaById), StringComparer.OrdinalIgnoreCase)
            .Select(row => new RawDataVaultFromMetaSchemaTableReport(
                QualifiedTableName: BuildQualifiedTableName(row.Table, sourceIndex.SchemaById),
                SelectedKey: BuildSelectedKeyReport(row.KeyAssessment),
                HubCreated: row.HubCreated,
                SatelliteAttributeCount: row.SatelliteAttributeCount,
                Reason: !row.HubCreated && !string.IsNullOrWhiteSpace(row.KeyAssessment?.SkipReason)
                    ? row.KeyAssessment.SkipReason
                    : null))
            .ToList();

        var relationships = relationshipReportRows
            .OrderBy(row => BuildRelationshipTitle(row.Relationship, row.SourceTable, row.TargetTable), StringComparer.OrdinalIgnoreCase)
            .Select(row => new RawDataVaultFromMetaSchemaRelationshipReport(
                RawLinkName: row.RawLinkName ?? BuildStructuralLinkName(row.SourceTable, row.TargetTable),
                SourceTableName: row.SourceTable.Name,
                TargetTableName: row.TargetTable.Name,
                LinkCreated: row.LinkCreated,
                NameWasDisambiguated: row.LinkCreated &&
                                     !string.Equals(
                                         row.RawLinkName,
                                         BuildStructuralLinkName(row.SourceTable, row.TargetTable),
                                         StringComparison.Ordinal),
                Reason: row.LinkCreated ? null : row.SkipReason))
            .ToList();

        return new RawDataVaultFromMetaSchemaReport(summary, tables, relationships);
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

    private static RawDataVaultFromMetaSchemaSelectedKeyReport? BuildSelectedKeyReport(TableKeyAssessment? keyAssessment)
    {
        if (keyAssessment?.SelectedKey == null)
        {
            return null;
        }

        var key = keyAssessment.SelectedKey;
        var fieldNames = key.OrderedKeyFields
            .Select(record => record.FieldName)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToList();

        return new RawDataVaultFromMetaSchemaSelectedKeyReport(
            KeyType: key.TableKey.KeyType,
            KeyName: string.IsNullOrWhiteSpace(key.TableKey.Name) ? null : key.TableKey.Name,
            FieldNames: fieldNames);
    }

    private static string BuildRelationshipTitle(MS.TableRelationship relationship, MS.Table sourceTable, MS.Table targetTable)
    {
        return $"{BuildStructuralLinkName(sourceTable, targetTable)} ({sourceTable.Name} -> {targetTable.Name})";
    }

    private static IReadOnlyList<string> MaterializeList(IEnumerable<string> values)
    {
        return values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
            .ThenBy(value => value, StringComparer.Ordinal)
            .ToList();
    }
}
