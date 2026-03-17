using System.Globalization;
using MS = global::MetaSchema;
using MRDV = global::MetaRawDataVault;

namespace MetaSchema.ToRawDataVault;

public sealed partial class RawDataVaultBootstrapper
{
    private static SchemaBootstrapDraft ConvertSchemaBootstrap(
        MS.MetaSchemaModel metaSchemaModel,
        RawDataVaultImplementationModel implementation,
        MetaSchemaBootstrapOptions options)
    {
        ArgumentNullException.ThrowIfNull(metaSchemaModel);
        ArgumentNullException.ThrowIfNull(implementation);
        ArgumentNullException.ThrowIfNull(options);

        var sourceIndex = BuildSourceIndex(metaSchemaModel, options.IncludeViews);
        var draft = CopySourceStructure(sourceIndex);
        var candidateKeyAssessmentsByTableId = AssessCandidateKeys(metaSchemaModel, sourceIndex, options);
        var tableReportRows = MaterializeHubsAndSatellites(draft, sourceIndex, candidateKeyAssessmentsByTableId, options);
        var relationshipReportRows = MaterializeLinks(draft, sourceIndex);
        draft.MaterializationReport = BuildMaterializationReport(
            draft,
            sourceIndex,
            tableReportRows,
            relationshipReportRows,
            options);

        return draft;
    }

    private static SourceIndex BuildSourceIndex(MS.MetaSchemaModel metaSchemaModel, bool includeViews)
    {
        var includedTables = metaSchemaModel.TableList
            .Where(table => IsIncludedTable(table, includeViews))
            .OrderBy(table => table.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(table => table.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(table => table.Id, StringComparer.Ordinal)
            .ToList();
        var includedTableIds = includedTables
            .Select(table => table.Id)
            .ToHashSet(StringComparer.Ordinal);

        var includedSchemas = metaSchemaModel.SchemaList
            .Where(schema => includedTables.Any(table => string.Equals(table.SchemaId, schema.Id, StringComparison.Ordinal)))
            .OrderBy(schema => schema.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(schema => schema.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(schema => schema.Id, StringComparer.Ordinal)
            .ToList();

        var includedSystems = metaSchemaModel.SystemList
            .Where(system => includedSchemas.Any(schema => string.Equals(schema.SystemId, system.Id, StringComparison.Ordinal)))
            .OrderBy(system => system.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(system => system.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(system => system.Id, StringComparer.Ordinal)
            .ToList();

        var includedFields = metaSchemaModel.FieldList
            .Where(field => includedTableIds.Contains(field.TableId))
            .OrderBy(field => ParseInt32(field.Ordinal, int.MaxValue))
            .ThenBy(field => field.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(field => field.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(field => field.Id, StringComparer.Ordinal)
            .ToList();
        var includedFieldIds = includedFields
            .Select(field => field.Id)
            .ToHashSet(StringComparer.Ordinal);

        var includedFieldDetails = metaSchemaModel.FieldDataTypeDetailList
            .Where(detail => includedFieldIds.Contains(detail.FieldId))
            .OrderBy(detail => detail.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(detail => detail.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(detail => detail.Id, StringComparer.Ordinal)
            .ToList();

        var includedRelationships = metaSchemaModel.TableRelationshipList
            .Where(relationship => includedTableIds.Contains(relationship.SourceTableId) &&
                                   includedTableIds.Contains(relationship.TargetTableId))
            .OrderBy(relationship => relationship.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(relationship => relationship.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(relationship => relationship.Id, StringComparer.Ordinal)
            .ToList();
        var includedRelationshipIds = includedRelationships
            .Select(relationship => relationship.Id)
            .ToHashSet(StringComparer.Ordinal);

        var includedRelationshipFields = metaSchemaModel.TableRelationshipFieldList
            .Where(field => includedRelationshipIds.Contains(field.TableRelationshipId) &&
                            includedFieldIds.Contains(field.SourceFieldId) &&
                            includedFieldIds.Contains(field.TargetFieldId))
            .OrderBy(field => ParseInt32(field.Ordinal, int.MaxValue))
            .ThenBy(field => field.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(field => field.Id, StringComparer.Ordinal)
            .ToList();

        return new SourceIndex
        {
            IncludedSystems = includedSystems,
            IncludedSchemas = includedSchemas,
            IncludedTables = includedTables,
            IncludedFields = includedFields,
            IncludedFieldDetails = includedFieldDetails,
            IncludedRelationships = includedRelationships,
            IncludedRelationshipFields = includedRelationshipFields,
            SchemaById = includedSchemas.ToDictionary(schema => schema.Id, StringComparer.Ordinal),
            TableById = includedTables.ToDictionary(table => table.Id, StringComparer.Ordinal),
            FieldById = includedFields.ToDictionary(field => field.Id, StringComparer.Ordinal),
            FieldsByTableId = includedFields
                .GroupBy(field => field.TableId, StringComparer.Ordinal)
                .ToDictionary(group => group.Key, group => (IReadOnlyList<MS.Field>)group.ToList(), StringComparer.Ordinal),
            RelationshipFieldsByRelationshipId = includedRelationshipFields
                .GroupBy(field => field.TableRelationshipId, StringComparer.Ordinal)
                .ToDictionary(group => group.Key, group => (IReadOnlyList<MS.TableRelationshipField>)group.ToList(), StringComparer.Ordinal),
            RelationshipSourceFieldIds = includedRelationshipFields
                .Select(field => field.SourceFieldId)
                .ToHashSet(StringComparer.Ordinal),
        };
    }

    private static Dictionary<string, TableKeyAssessment> AssessCandidateKeys(
        MS.MetaSchemaModel metaSchemaModel,
        SourceIndex sourceIndex,
        MetaSchemaBootstrapOptions options)
    {
        var includedTableIds = sourceIndex.IncludedTables.Select(table => table.Id).ToHashSet(StringComparer.Ordinal);
        var includedFieldIds = sourceIndex.IncludedFields.Select(field => field.Id).ToHashSet(StringComparer.Ordinal);

        var keyFieldsByKeyId = metaSchemaModel.TableKeyFieldList
            .Where(record => includedFieldIds.Contains(record.FieldId))
            .GroupBy(record => record.TableKeyId, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<MS.TableKeyField>)group
                    .OrderBy(record => ParseInt32(record.Ordinal, int.MaxValue))
                    .ThenBy(record => record.Id, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(record => record.Id, StringComparer.Ordinal)
                    .ToList(),
                StringComparer.Ordinal);

        var keysByTableId = metaSchemaModel.TableKeyList
            .Where(record => includedTableIds.Contains(record.TableId))
            .GroupBy(record => record.TableId, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<MS.TableKey>)group
                    .OrderBy(record => GetKeyPriority(record.KeyType))
                    .ThenBy(record => record.Name, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(record => record.Id, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(record => record.Id, StringComparer.Ordinal)
                    .ToList(),
                StringComparer.Ordinal);

        var assessments = new Dictionary<string, TableKeyAssessment>(StringComparer.Ordinal);
        foreach (var table in sourceIndex.IncludedTables)
        {
            var sourceKeys = keysByTableId.TryGetValue(table.Id, out var keysForTable)
                ? keysForTable
                : Array.Empty<MS.TableKey>();

            var candidateKeys = sourceKeys
                .Select(record => new CandidateKeySelection(
                    record,
                    keyFieldsByKeyId.TryGetValue(record.Id, out var orderedKeyFields)
                        ? orderedKeyFields
                            .Where(keyField =>
                                sourceIndex.FieldById.TryGetValue(keyField.FieldId, out var field) &&
                                !ShouldIgnoreField(field.Name, options.IgnoredFieldNames, options.IgnoredFieldSuffixes))
                            .ToList()
                        : Array.Empty<MS.TableKeyField>()))
                .ToList();

            var selectedKey = candidateKeys
                .Where(selection => selection.OrderedKeyFields.Count > 0)
                .OrderBy(selection => GetKeyPriority(selection.TableKey.KeyType))
                .ThenBy(selection => selection.TableKey.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(selection => selection.TableKey.Id, StringComparer.OrdinalIgnoreCase)
                .ThenBy(selection => selection.TableKey.Id, StringComparer.Ordinal)
                .FirstOrDefault();

            var skipReason = selectedKey != null
                ? string.Empty
                : sourceKeys.Count == 0
                    ? "no source primary or unique key metadata was available"
                    : "all source key fields were excluded by explicit ignore options";

            assessments[table.Id] = new TableKeyAssessment(candidateKeys, selectedKey, skipReason);
        }

        return assessments;
    }

    private static List<TableMaterializationReportRow> MaterializeHubsAndSatellites(
        SchemaBootstrapDraft draft,
        SourceIndex sourceIndex,
        IReadOnlyDictionary<string, TableKeyAssessment> candidateKeyAssessmentsByTableId,
        MetaSchemaBootstrapOptions options)
    {
        var tableReportRows = new List<TableMaterializationReportRow>();

        foreach (var table in sourceIndex.IncludedTables)
        {
            candidateKeyAssessmentsByTableId.TryGetValue(table.Id, out var keyAssessment);
            var selectedKey = keyAssessment?.SelectedKey;
            if (selectedKey == null)
            {
                tableReportRows.Add(new TableMaterializationReportRow(table, keyAssessment, false, 0));
                continue;
            }

            var sourceTable = draft.SourceTablesById[table.Id];
            var hub = new MRDV.RawHub
            {
                Id = BuildRawHubId(table.Id),
                Name = table.Name,
                SourceTableId = sourceTable.Id,
                SourceTable = sourceTable,
            };
            draft.RawHubs.Add(hub);
            draft.RawHubsById[hub.Id] = hub;
            draft.RawHubIdsBySourceTableId[table.Id] = hub.Id;

            var orderedKeyFields = selectedKey.OrderedKeyFields
                .Select(keyField => sourceIndex.FieldById[keyField.FieldId])
                .ToList();

            if (orderedKeyFields.Count == 0)
            {
                tableReportRows.Add(new TableMaterializationReportRow(table, keyAssessment, false, 0));
                continue;
            }

            for (var index = 0; index < orderedKeyFields.Count; index++)
            {
                var sourceField = draft.SourceFieldsById[orderedKeyFields[index].Id];
                draft.RawHubKeyParts.Add(new MRDV.RawHubKeyPart
                {
                    Id = BuildRawHubKeyPartId(hub.Id, sourceField.Id),
                    Name = sourceField.Name,
                    Ordinal = (index + 1).ToString(CultureInfo.InvariantCulture),
                    RawHubId = hub.Id,
                    RawHub = hub,
                    SourceFieldId = sourceField.Id,
                    SourceField = sourceField,
                });
            }

            var keyFieldIds = orderedKeyFields
                .Select(field => field.Id)
                .ToHashSet(StringComparer.Ordinal);
            var satelliteFields = sourceIndex.FieldsByTableId.TryGetValue(table.Id, out var tableFields)
                ? tableFields
                    .Where(field =>
                        !keyFieldIds.Contains(field.Id) &&
                        !sourceIndex.RelationshipSourceFieldIds.Contains(field.Id) &&
                        !ShouldIgnoreField(field.Name, options.IgnoredFieldNames, options.IgnoredFieldSuffixes))
                    .ToList()
                : new List<MS.Field>();

            if (satelliteFields.Count == 0)
            {
                tableReportRows.Add(new TableMaterializationReportRow(table, keyAssessment, true, 0));
                continue;
            }

            var satellite = new MRDV.RawHubSatellite
            {
                Id = BuildRawHubSatelliteId(hub.Id),
                Name = table.Name,
                SatelliteKind = StandardSatelliteKind,
                RawHubId = hub.Id,
                RawHub = hub,
                SourceTableId = sourceTable.Id,
                SourceTable = sourceTable,
            };
            draft.RawHubSatellites.Add(satellite);

            for (var index = 0; index < satelliteFields.Count; index++)
            {
                var sourceField = draft.SourceFieldsById[satelliteFields[index].Id];
                draft.RawHubSatelliteAttributes.Add(new MRDV.RawHubSatelliteAttribute
                {
                    Id = BuildRawHubSatelliteAttributeId(satellite.Id, sourceField.Id),
                    Name = sourceField.Name,
                    Ordinal = (index + 1).ToString(CultureInfo.InvariantCulture),
                    RawHubSatelliteId = satellite.Id,
                    RawHubSatellite = satellite,
                    SourceFieldId = sourceField.Id,
                    SourceField = sourceField,
                });
            }

            tableReportRows.Add(new TableMaterializationReportRow(table, keyAssessment, true, satelliteFields.Count));
        }

        return tableReportRows;
    }

    private static List<RelationshipMaterializationReportRow> MaterializeLinks(
        SchemaBootstrapDraft draft,
        SourceIndex sourceIndex)
    {
        var relationshipReportRows = new List<RelationshipMaterializationReportRow>();

        foreach (var relationship in sourceIndex.IncludedRelationships)
        {
            var sourceTable = relationship.SourceTable;
            var targetTable = relationship.TargetTable;
            var hasSourceHub = draft.RawHubIdsBySourceTableId.TryGetValue(sourceTable.Id, out var sourceHubId);
            var hasTargetHub = draft.RawHubIdsBySourceTableId.TryGetValue(targetTable.Id, out var targetHubId);

            if (!hasSourceHub || !hasTargetHub)
            {
                relationshipReportRows.Add(new RelationshipMaterializationReportRow(
                    relationship,
                    sourceTable,
                    targetTable,
                    false,
                    BuildRelationshipSkipReason(sourceHubId, targetHubId, sourceTable, targetTable)));
                continue;
            }

            var sourceRelationship = draft.SourceRelationshipsById[relationship.Id];
            var link = new MRDV.RawLink
            {
                Id = BuildRawLinkId(relationship.Id),
                Name = BuildStructuralLinkName(sourceTable, targetTable),
                LinkKind = StandardLinkKind,
                SourceTableRelationshipId = sourceRelationship.Id,
                SourceTableRelationship = sourceRelationship,
            };
            draft.RawLinks.Add(link);

            draft.RawLinkHubs.Add(new MRDV.RawLinkHub
            {
                Id = BuildRawLinkHubId(link.Id, "source"),
                Ordinal = "1",
                RoleName = BuildRoleName(sourceTable, targetTable, isSource: true),
                RawLinkId = link.Id,
                RawLink = link,
                RawHubId = sourceHubId!,
                RawHub = draft.RawHubsById[sourceHubId!],
            });

            draft.RawLinkHubs.Add(new MRDV.RawLinkHub
            {
                Id = BuildRawLinkHubId(link.Id, "target"),
                Ordinal = "2",
                RoleName = BuildRoleName(sourceTable, targetTable, isSource: false),
                RawLinkId = link.Id,
                RawLink = link,
                RawHubId = targetHubId!,
                RawHub = draft.RawHubsById[targetHubId!],
            });

            relationshipReportRows.Add(new RelationshipMaterializationReportRow(
                relationship,
                sourceTable,
                targetTable,
                true,
                null));
        }

        return relationshipReportRows;
    }

    private static bool IsIncludedTable(MS.Table table, bool includeViews)
    {
        if (includeViews)
        {
            return true;
        }

        return !string.Equals(table.ObjectType, "View", StringComparison.OrdinalIgnoreCase);
    }

    private static int GetKeyPriority(string keyType)
    {
        return keyType switch
        {
            "primary" => 0,
            "unique" => 1,
            _ => 2
        };
    }

    private static bool ShouldIgnoreField(string fieldName, ISet<string> ignoredFieldNames, ISet<string> ignoredFieldSuffixes)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            return false;
        }

        if (ignoredFieldNames.Contains(fieldName))
        {
            return true;
        }

        foreach (var suffix in ignoredFieldSuffixes)
        {
            if (fieldName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static int ParseInt32(string value, int defaultValue)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : defaultValue;
    }

    private static string BuildRawHubId(string tableId) => "rawhub:" + tableId;

    private static string BuildRawHubKeyPartId(string hubId, string fieldId) => $"{hubId}:key:{fieldId}";

    private static string BuildRawHubSatelliteId(string hubId) => $"{hubId}:sat";

    private static string BuildRawHubSatelliteAttributeId(string satelliteId, string fieldId) => $"{satelliteId}:attr:{fieldId}";

    private static string BuildRawLinkId(string relationshipId) => "rawlink:" + relationshipId;

    private static string BuildRawLinkHubId(string linkId, string role) => $"{linkId}:{role}";

    private static string BuildStructuralLinkName(MS.Table sourceTable, MS.Table targetTable)
    {
        return BuildRoleName(sourceTable, targetTable, isSource: true) +
               BuildRoleName(sourceTable, targetTable, isSource: false);
    }

    private static string BuildRoleName(MS.Table sourceTable, MS.Table targetTable, bool isSource)
    {
        if (!string.Equals(sourceTable.Name, targetTable.Name, StringComparison.OrdinalIgnoreCase))
        {
            return isSource ? sourceTable.Name : targetTable.Name;
        }

        return isSource ? "Source" + sourceTable.Name : "Target" + targetTable.Name;
    }
}
