using System.Globalization;
using System.Text;
using MS = global::MetaSchema;
using MRDV = global::MetaRawDataVault;

namespace MetaConvert.SchemaToDataVault;

public sealed partial class RawDataVaultFromMetaSchemaService
{
    // Contract: this method defines the full MetaSchema -> MetaRawDataVault
    // projection surface for raw table families (H_*, HS_*, L_*, LS_*). Any
    // future expansion of projected raw table families must update lineage
    // coverage/emission in the same change to avoid pipeline drift.
    private static (FromMetaSchemaDraft Draft, RawDataVaultFromMetaSchemaReport Report) ConvertFromMetaSchema(
        MS.MetaSchemaModel metaSchemaModel,
        FromMetaSchemaOptions options)
    {
        ArgumentNullException.ThrowIfNull(metaSchemaModel);
        ArgumentNullException.ThrowIfNull(options);

        var sourceIndex = BuildSourceIndex(metaSchemaModel, options.IncludeViews);
        var draft = CopySourceStructure(sourceIndex);
        var candidateKeyAssessmentsByTableId = AssessCandidateKeys(metaSchemaModel, sourceIndex, options);
        var tableReportRows = MaterializeHubsAndSatellites(draft, sourceIndex, candidateKeyAssessmentsByTableId, options);
        var relationshipReportRows = MaterializeLinks(draft, sourceIndex);
        var report = BuildReport(
            draft,
            sourceIndex,
            tableReportRows,
            relationshipReportRows,
            options);

        return (draft, report);
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
        FromMetaSchemaOptions options)
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
        FromMetaSchemaDraft draft,
        SourceIndex sourceIndex,
        IReadOnlyDictionary<string, TableKeyAssessment> candidateKeyAssessmentsByTableId,
        FromMetaSchemaOptions options)
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
        FromMetaSchemaDraft draft,
        SourceIndex sourceIndex)
    {
        var relationshipReportRows = new List<RelationshipMaterializationReportRow>();
        var rawLinkNamesByRelationshipId = BuildRawLinkNames(sourceIndex);

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
                    null,
                    false,
                    BuildRelationshipSkipReason(sourceHubId, targetHubId, sourceTable, targetTable)));
                continue;
            }

            var sourceRelationship = draft.SourceRelationshipsById[relationship.Id];
            var link = new MRDV.RawLink
            {
                Id = BuildRawLinkId(relationship.Id),
                Name = rawLinkNamesByRelationshipId[relationship.Id],
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
                link.Name,
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

    private static IReadOnlyDictionary<string, string> BuildRawLinkNames(SourceIndex sourceIndex)
    {
        var namesByRelationshipId = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var group in sourceIndex.IncludedRelationships
                     .GroupBy(relationship => BuildStructuralLinkName(relationship.SourceTable, relationship.TargetTable), StringComparer.Ordinal))
        {
            var relationships = group.ToList();
            if (relationships.Count == 1)
            {
                namesByRelationshipId[relationships[0].Id] = group.Key;
                continue;
            }

            var reservedNames = new HashSet<string>(StringComparer.Ordinal);
            foreach (var relationship in relationships)
            {
                var preferredName = group.Key + "_" + BuildRelationshipFieldDisambiguator(relationship, sourceIndex);
                if (reservedNames.Add(preferredName))
                {
                    namesByRelationshipId[relationship.Id] = preferredName;
                    continue;
                }

                var relationshipName = group.Key + "_" + BuildIdentifierToken(relationship.Name, "Relationship");
                if (reservedNames.Add(relationshipName))
                {
                    namesByRelationshipId[relationship.Id] = relationshipName;
                    continue;
                }

                var fallbackName = relationshipName + "_" + BuildIdentifierToken(relationship.Id, "Id");
                reservedNames.Add(fallbackName);
                namesByRelationshipId[relationship.Id] = fallbackName;
            }
        }

        return namesByRelationshipId;
    }

    private static string BuildRelationshipFieldDisambiguator(MS.TableRelationship relationship, SourceIndex sourceIndex)
    {
        if (!sourceIndex.RelationshipFieldsByRelationshipId.TryGetValue(relationship.Id, out var relationshipFields) ||
            relationshipFields.Count == 0)
        {
            return BuildIdentifierToken(relationship.Name, "Relationship");
        }

        var sourceFieldNames = relationshipFields
            .Select(field => sourceIndex.FieldById.TryGetValue(field.SourceFieldId, out var sourceField)
                ? sourceField.Name
                : string.Empty)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList();

        if (sourceFieldNames.Count == 0)
        {
            return BuildIdentifierToken(relationship.Name, "Relationship");
        }

        return BuildIdentifierToken(string.Join("_", sourceFieldNames), "Relationship");
    }

    private static string BuildIdentifierToken(string? value, string fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return fallback;
        }

        var builder = new StringBuilder();
        var previousWasSeparator = false;

        foreach (var character in value.Trim())
        {
            if (char.IsLetterOrDigit(character))
            {
                builder.Append(character);
                previousWasSeparator = false;
                continue;
            }

            if (previousWasSeparator)
            {
                continue;
            }

            builder.Append('_');
            previousWasSeparator = true;
        }

        var token = builder.ToString().Trim('_');
        return string.IsNullOrWhiteSpace(token) ? fallback : token;
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
