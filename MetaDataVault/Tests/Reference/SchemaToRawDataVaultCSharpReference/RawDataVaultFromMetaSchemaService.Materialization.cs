using System.Globalization;
using System.Text;
using MS = global::MetaSchema;
using MRDV = global::MetaRawDataVault;

namespace MetaConvert.SchemaToDataVault.Reference;

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
        var draft = CreateRawDraft(sourceIndex);
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
            .Select(table => table.SchemaObject)
            .Concat(includeViews
                ? metaSchemaModel.ViewList.Select(view => view.SchemaObject)
                : [])
            .OrderBy(table => table.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(table => table.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(table => table.Id, StringComparer.Ordinal)
            .ToList();
        var includedTableIds = includedTables
            .Select(table => table.Id)
            .ToHashSet(StringComparer.Ordinal);

        var includedSchemas = metaSchemaModel.SchemaList
            .Where(schema => includedTables.Any(table => string.Equals(table.Schema.Id, schema.Id, StringComparison.Ordinal)))
            .OrderBy(schema => schema.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(schema => schema.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(schema => schema.Id, StringComparer.Ordinal)
            .ToList();

        var includedSystems = metaSchemaModel.SystemList
            .Where(system => includedSchemas.Any(schema => string.Equals(schema.System.Id, system.Id, StringComparison.Ordinal)))
            .OrderBy(system => system.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(system => system.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(system => system.Id, StringComparer.Ordinal)
            .ToList();

        var includedFields = metaSchemaModel.FieldList
            .Where(field => includedTableIds.Contains(field.SchemaObject.Id))
            .OrderBy(field => ParseInt32(field.Ordinal, int.MaxValue))
            .ThenBy(field => field.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(field => field.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(field => field.Id, StringComparer.Ordinal)
            .ToList();
        var includedFieldIds = includedFields
            .Select(field => field.Id)
            .ToHashSet(StringComparer.Ordinal);

        var includedFieldDetails = metaSchemaModel.FieldDataTypeDetailList
            .Where(detail => includedFieldIds.Contains(detail.Field.Id))
            .OrderBy(detail => detail.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(detail => detail.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(detail => detail.Id, StringComparer.Ordinal)
            .ToList();

        var includedRelationships = metaSchemaModel.TableRelationshipList
            .Where(relationship => includedTableIds.Contains(relationship.SourceTable.Id) &&
                                   includedTableIds.Contains(relationship.TargetTable.Id))
            .OrderBy(relationship => relationship.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(relationship => relationship.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(relationship => relationship.Id, StringComparer.Ordinal)
            .ToList();
        var includedRelationshipIds = includedRelationships
            .Select(relationship => relationship.Id)
            .ToHashSet(StringComparer.Ordinal);

        var includedRelationshipFields = metaSchemaModel.TableRelationshipFieldList
            .Where(field => includedRelationshipIds.Contains(field.TableRelationship.Id) &&
                            includedFieldIds.Contains(field.SourceField.Id) &&
                            includedFieldIds.Contains(field.TargetField.Id))
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
                .GroupBy(field => field.SchemaObject.Id, StringComparer.Ordinal)
                .ToDictionary(group => group.Key, group => (IReadOnlyList<MS.Field>)group.ToList(), StringComparer.Ordinal),
            RelationshipFieldsByRelationshipId = includedRelationshipFields
                .GroupBy(field => field.TableRelationship.Id, StringComparer.Ordinal)
                .ToDictionary(group => group.Key, group => (IReadOnlyList<MS.TableRelationshipField>)group.ToList(), StringComparer.Ordinal),
            RelationshipSourceFieldIds = includedRelationshipFields
                .Select(field => field.SourceField.Id)
                .ToHashSet(StringComparer.Ordinal),
            PrimaryKeyIds = metaSchemaModel.PrimaryKeyList
                .Select(primaryKey => primaryKey.Key.Id)
                .ToHashSet(StringComparer.Ordinal),
            UniqueKeyIds = metaSchemaModel.UniqueKeyList
                .Select(uniqueKey => uniqueKey.Key.Id)
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

        var keyFieldsByKeyId = metaSchemaModel.KeyFieldList
            .Where(record => includedFieldIds.Contains(record.Field.Id))
            .GroupBy(record => record.Key.Id, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<MS.KeyField>)group
                    .OrderBy(record => ParseInt32(record.Ordinal, int.MaxValue))
                    .ThenBy(record => record.Id, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(record => record.Id, StringComparer.Ordinal)
                    .ToList(),
                StringComparer.Ordinal);

        var keysByTableId = metaSchemaModel.KeyList
            .Where(record => includedTableIds.Contains(record.Table.SchemaObject.Id))
            .GroupBy(record => record.Table.SchemaObject.Id, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<MS.Key>)group
                    .OrderBy(record => GetKeyPriority(record, sourceIndex))
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
                : Array.Empty<MS.Key>();

            var candidateKeys = sourceKeys
                .Select(record => new CandidateKeySelection(
                    record,
                    keyFieldsByKeyId.TryGetValue(record.Id, out var orderedKeyFields)
                        ? orderedKeyFields
                            .Where(keyField =>
                                sourceIndex.FieldById.TryGetValue(keyField.Field.Id, out var field) &&
                                !ShouldIgnoreField(field.Name, options.IgnoredFieldNames, options.IgnoredFieldSuffixes))
                            .ToList()
                        : Array.Empty<MS.KeyField>()))
                .ToList();

            var selectedKey = candidateKeys
                .Where(selection => selection.OrderedKeyFields.Count > 0)
                .OrderBy(selection => GetKeyPriority(selection.Key, sourceIndex))
                .ThenBy(selection => selection.Key.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(selection => selection.Key.Id, StringComparer.OrdinalIgnoreCase)
                .ThenBy(selection => selection.Key.Id, StringComparer.Ordinal)
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

            var hub = new MRDV.RawHub
            {
                Id = BuildRawHubId(table.Id),
                Name = table.Name,
            };
            draft.RawHubs.Add(hub);
            draft.RawHubsById[hub.Id] = hub;
            draft.RawHubIdsBySourceTableId[table.Id] = hub.Id;

            var orderedKeyFields = selectedKey.OrderedKeyFields
                .Select(keyField => sourceIndex.FieldById[keyField.Field.Id])
                .ToList();

            if (orderedKeyFields.Count == 0)
            {
                tableReportRows.Add(new TableMaterializationReportRow(table, keyAssessment, false, 0));
                continue;
            }

            foreach (var orderedKeyField in orderedKeyFields)
            {
                var field = draft.FieldsById[orderedKeyField.Id];
                draft.RawHubKeyParts.Add(new MRDV.RawHubKeyPart
                {
                    Id = BuildRawHubKeyPartId(hub.Id, field.Id),
                    Name = field.Name,
                    RawHub = hub,
                    Field = field,
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
                RawHub = hub,
            };
            draft.RawHubSatellites.Add(satellite);

            foreach (var satelliteField in satelliteFields)
            {
                var field = draft.FieldsById[satelliteField.Id];
                draft.RawHubSatelliteAttributes.Add(new MRDV.RawHubSatelliteAttribute
                {
                    Id = BuildRawHubSatelliteAttributeId(satellite.Id, field.Id),
                    Name = field.Name,
                    RawHubSatellite = satellite,
                    Field = field,
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
            var sourceTable = relationship.SourceTable.SchemaObject;
            var targetTable = relationship.TargetTable.SchemaObject;
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

            var link = new MRDV.RawLink
            {
                Id = BuildRawLinkId(relationship.Id),
                Name = rawLinkNamesByRelationshipId[relationship.Id],
                LinkKind = StandardLinkKind,
            };
            draft.RawLinks.Add(link);

            draft.RawLinkRoles.Add(new MRDV.RawLinkRole
            {
                Id = BuildRawLinkRoleId(link.Id, "source"),
                Name = BuildLinkRoleName(sourceTable, targetTable, isSource: true),
                RawLink = link,
                RawHub = draft.RawHubsById[sourceHubId!],
            });

            draft.RawLinkRoles.Add(new MRDV.RawLinkRole
            {
                Id = BuildRawLinkRoleId(link.Id, "target"),
                Name = BuildLinkRoleName(sourceTable, targetTable, isSource: false),
                RawLink = link,
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

    private static int GetKeyPriority(MS.Key key, SourceIndex sourceIndex)
    {
        if (sourceIndex.PrimaryKeyIds.Contains(key.Id))
        {
            return 0;
        }

        return sourceIndex.UniqueKeyIds.Contains(key.Id) ? 1 : 2;
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

    private static int ParseInt32(string? value, int defaultValue)
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

    private static string BuildRawLinkRoleId(string linkId, string role) => $"{linkId}:{role}";

    private static string BuildStructuralLinkName(MS.SchemaObject sourceTable, MS.SchemaObject targetTable)
    {
        return BuildLinkRoleName(sourceTable, targetTable, isSource: true) +
               BuildLinkRoleName(sourceTable, targetTable, isSource: false);
    }

    private static IReadOnlyDictionary<string, string> BuildRawLinkNames(SourceIndex sourceIndex)
    {
        var namesByRelationshipId = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var group in sourceIndex.IncludedRelationships
                     .GroupBy(
                         relationship => BuildStructuralLinkName(
                             relationship.SourceTable.SchemaObject,
                             relationship.TargetTable.SchemaObject),
                         StringComparer.Ordinal))
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
            .Select(field => sourceIndex.FieldById.TryGetValue(field.SourceField.Id, out var sourceField)
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

    private static string BuildLinkRoleName(MS.SchemaObject sourceTable, MS.SchemaObject targetTable, bool isSource)
    {
        if (!string.Equals(sourceTable.Name, targetTable.Name, StringComparison.OrdinalIgnoreCase))
        {
            return isSource ? sourceTable.Name : targetTable.Name;
        }

        return isSource ? "Source" + sourceTable.Name : "Target" + targetTable.Name;
    }
}
