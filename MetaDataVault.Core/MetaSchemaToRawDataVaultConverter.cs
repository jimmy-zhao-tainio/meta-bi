using Meta.Core.Domain;

namespace MetaDataVault.Core;

public sealed class MetaSchemaToRawDataVaultConverter
{
    private const string MetaSchemaModelName = "MetaSchema";
    private const string MetaBusinessModelName = "MetaBusiness";
    private const string MetaDataVaultImplementationModelName = "MetaDataVaultImplementation";
    private const string StandardSatelliteKind = "standard";
    private const string StandardLinkKind = "standard";
    private const string DefaultSatelliteName = "Attributes";

    public Workspace Convert(
        Workspace metaSchemaWorkspace,
        string newWorkspacePath,
        Workspace? businessWorkspace = null,
        Workspace? implementationWorkspace = null,
        IEnumerable<string>? ignoredFieldNames = null,
        IEnumerable<string>? ignoredFieldSuffixes = null)
    {
        ArgumentNullException.ThrowIfNull(metaSchemaWorkspace);
        ArgumentException.ThrowIfNullOrWhiteSpace(newWorkspacePath);

        EnsureModel(metaSchemaWorkspace, MetaSchemaModelName, nameof(metaSchemaWorkspace));
        if (businessWorkspace != null)
        {
            EnsureModel(businessWorkspace, MetaBusinessModelName, nameof(businessWorkspace));
        }
        if (implementationWorkspace != null)
        {
            EnsureModel(implementationWorkspace, MetaDataVaultImplementationModelName, nameof(implementationWorkspace));
        }

        return ConvertSchemaBootstrap(
            metaSchemaWorkspace,
            newWorkspacePath,
            ignoredFieldNames,
            ignoredFieldSuffixes);
    }

    public Workspace Convert(
        Workspace metaSchemaWorkspace,
        Workspace businessWorkspace,
        Workspace implementationWorkspace,
        string newWorkspacePath,
        IEnumerable<string>? ignoredFieldNames = null,
        IEnumerable<string>? ignoredFieldSuffixes = null)
    {
        return Convert(
            metaSchemaWorkspace,
            newWorkspacePath,
            businessWorkspace,
            implementationWorkspace,
            ignoredFieldNames,
            ignoredFieldSuffixes);
    }

    private static void EnsureModel(Workspace workspace, string expectedModelName, string parameterName)
    {
        if (!string.Equals(workspace.Model.Name, expectedModelName, StringComparison.Ordinal))
        {
            throw new ArgumentException(
                $"Expected sanctioned model '{expectedModelName}' but found '{workspace.Model.Name}'.",
                parameterName);
        }
    }

    private static Workspace ConvertSchemaBootstrap(
        Workspace metaSchemaWorkspace,
        string newWorkspacePath,
        IEnumerable<string>? ignoredFieldNames,
        IEnumerable<string>? ignoredFieldSuffixes)
    {
        var rawWorkspace = MetaDataVaultWorkspaces.CreateEmptyMetaRawDataVaultWorkspace(newWorkspacePath);
        var ignoredFieldNameSet = ignoredFieldNames?
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToHashSet(StringComparer.OrdinalIgnoreCase)
            ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var ignoredFieldSuffixSet = ignoredFieldSuffixes?
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToHashSet(StringComparer.OrdinalIgnoreCase)
            ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var sourceSystems = metaSchemaWorkspace.Instance.GetOrCreateEntityRecords("System");
        var sourceSchemas = metaSchemaWorkspace.Instance.GetOrCreateEntityRecords("Schema");
        var sourceTables = metaSchemaWorkspace.Instance.GetOrCreateEntityRecords("Table");
        var sourceFields = metaSchemaWorkspace.Instance.GetOrCreateEntityRecords("Field");
        var sourceFieldDetails = metaSchemaWorkspace.Instance.GetOrCreateEntityRecords("FieldDataTypeDetail");
        var tableKeys = metaSchemaWorkspace.Instance.GetOrCreateEntityRecords("TableKey");
        var tableKeyFields = metaSchemaWorkspace.Instance.GetOrCreateEntityRecords("TableKeyField");
        var tableRelationships = metaSchemaWorkspace.Instance.GetOrCreateEntityRecords("TableRelationship");
        var tableRelationshipFields = metaSchemaWorkspace.Instance.GetOrCreateEntityRecords("TableRelationshipField");

        var includedTables = sourceTables
            .Where(IsIncludedTable)
            .OrderBy(record => record.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(record => record.Id, StringComparer.Ordinal)
            .ToList();
        var includedTableIds = new HashSet<string>(includedTables.Select(record => record.Id), StringComparer.Ordinal);

        var includedSchemas = sourceSchemas
            .Where(record => includedTables.Any(table => GetRelationshipId(table, "SchemaId") == record.Id))
            .OrderBy(record => record.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(record => record.Id, StringComparer.Ordinal)
            .ToList();
        var includedSchemaIds = new HashSet<string>(includedSchemas.Select(record => record.Id), StringComparer.Ordinal);

        var includedSystems = sourceSystems
            .Where(record => includedSchemas.Any(schema => GetRelationshipId(schema, "SystemId") == record.Id))
            .OrderBy(record => record.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(record => record.Id, StringComparer.Ordinal)
            .ToList();

        var includedFields = sourceFields
            .Where(record => includedTableIds.Contains(GetRelationshipId(record, "TableId")))
            .OrderBy(record => record.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(record => record.Id, StringComparer.Ordinal)
            .ToList();
        var includedFieldIds = new HashSet<string>(includedFields.Select(record => record.Id), StringComparer.Ordinal);

        var includedFieldDetails = sourceFieldDetails
            .Where(record => includedFieldIds.Contains(GetRelationshipId(record, "FieldId")))
            .OrderBy(record => record.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(record => record.Id, StringComparer.Ordinal)
            .ToList();

        var includedRelationships = tableRelationships
            .Where(record => includedTableIds.Contains(GetRelationshipId(record, "SourceTableId")) &&
                             includedTableIds.Contains(GetRelationshipId(record, "TargetTableId")))
            .OrderBy(record => record.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(record => record.Id, StringComparer.Ordinal)
            .ToList();
        var includedRelationshipIds = new HashSet<string>(includedRelationships.Select(record => record.Id), StringComparer.Ordinal);

        var includedRelationshipFields = tableRelationshipFields
            .Where(record => includedRelationshipIds.Contains(GetRelationshipId(record, "TableRelationshipId")) &&
                             includedFieldIds.Contains(GetRelationshipId(record, "SourceFieldId")) &&
                             includedFieldIds.Contains(GetRelationshipId(record, "TargetFieldId")))
            .OrderBy(record => ParseInt32(GetValue(record, "Ordinal"), int.MaxValue))
            .ThenBy(record => record.Id, StringComparer.OrdinalIgnoreCase)
            .ThenBy(record => record.Id, StringComparer.Ordinal)
            .ToList();

        var tableById = includedTables.ToDictionary(record => record.Id, StringComparer.Ordinal);
        var fieldById = includedFields.ToDictionary(record => record.Id, StringComparer.Ordinal);
        var fieldsByTableId = includedFields
            .GroupBy(record => GetRelationshipId(record, "TableId"), StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.OrderBy(record => ParseInt32(GetValue(record, "Ordinal"), int.MaxValue))
                .ThenBy(record => GetValue(record, "Name"), StringComparer.OrdinalIgnoreCase)
                .ThenBy(record => record.Id, StringComparer.OrdinalIgnoreCase)
                .ThenBy(record => record.Id, StringComparer.Ordinal)
                .ToList(), StringComparer.Ordinal);
        var relationshipFieldsByRelationshipId = includedRelationshipFields
            .GroupBy(record => GetRelationshipId(record, "TableRelationshipId"), StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.OrderBy(record => ParseInt32(GetValue(record, "Ordinal"), int.MaxValue))
                .ThenBy(record => record.Id, StringComparer.OrdinalIgnoreCase)
                .ThenBy(record => record.Id, StringComparer.Ordinal)
                .ToList(), StringComparer.Ordinal);

        foreach (var system in includedSystems)
        {
            AddRecord(rawWorkspace, "SourceSystem", system.Id, values =>
            {
                values["Name"] = GetValue(system, "Name");
                AddOptionalValue(values, "Description", GetValue(system, "Description"));
            });
        }

        foreach (var schema in includedSchemas)
        {
            AddRecord(rawWorkspace, "SourceSchema", schema.Id, values =>
            {
                values["Name"] = GetValue(schema, "Name");
            }, relationships =>
            {
                relationships["SourceSystemId"] = GetRelationshipId(schema, "SystemId");
            });
        }

        foreach (var table in includedTables)
        {
            AddRecord(rawWorkspace, "SourceTable", table.Id, values =>
            {
                values["Name"] = GetValue(table, "Name");
            }, relationships =>
            {
                relationships["SourceSchemaId"] = GetRelationshipId(table, "SchemaId");
            });
        }

        foreach (var field in includedFields)
        {
            AddRecord(rawWorkspace, "SourceField", field.Id, values =>
            {
                values["Name"] = GetValue(field, "Name");
                values["DataTypeId"] = GetValue(field, "DataTypeId");
                AddOptionalValue(values, "Ordinal", GetValue(field, "Ordinal"));
                AddOptionalValue(values, "IsNullable", GetValue(field, "IsNullable"));
            }, relationships =>
            {
                relationships["SourceTableId"] = GetRelationshipId(field, "TableId");
            });
        }

        foreach (var detail in includedFieldDetails)
        {
            AddRecord(rawWorkspace, "SourceFieldDataTypeDetail", detail.Id, values =>
            {
                values["Name"] = GetValue(detail, "Name");
                values["Value"] = GetValue(detail, "Value");
            }, relationships =>
            {
                relationships["SourceFieldId"] = GetRelationshipId(detail, "FieldId");
            });
        }

        foreach (var relationship in includedRelationships)
        {
            AddRecord(rawWorkspace, "SourceTableRelationship", relationship.Id, values =>
            {
                values["Name"] = GetValue(relationship, "Name");
            }, relationships =>
            {
                relationships["SourceTableId"] = GetRelationshipId(relationship, "SourceTableId");
                relationships["TargetTableId"] = GetRelationshipId(relationship, "TargetTableId");
            });
        }

        foreach (var relationshipField in includedRelationshipFields)
        {
            AddRecord(rawWorkspace, "SourceTableRelationshipField", relationshipField.Id, values =>
            {
                values["Ordinal"] = GetValue(relationshipField, "Ordinal");
            }, relationships =>
            {
                relationships["SourceTableRelationshipId"] = GetRelationshipId(relationshipField, "TableRelationshipId");
                relationships["SourceFieldId"] = GetRelationshipId(relationshipField, "SourceFieldId");
                relationships["TargetFieldId"] = GetRelationshipId(relationshipField, "TargetFieldId");
            });
        }

        var candidateKeysByTableId = SelectCandidateKeys(
            tableKeys,
            tableKeyFields,
            includedTableIds,
            includedFieldIds,
            fieldById,
            ignoredFieldNameSet,
            ignoredFieldSuffixSet);
        var hubIdsByTableId = new Dictionary<string, string>(StringComparer.Ordinal);
        var relationshipSourceFieldIdsByTableId = includedRelationshipFields
            .GroupBy(record => GetRelationshipId(record, "SourceFieldId"), StringComparer.Ordinal)
            .Select(group => group.Key)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var table in includedTables)
        {
            if (!candidateKeysByTableId.TryGetValue(table.Id, out var keySelection))
            {
                continue;
            }

            var hubId = BuildRawHubId(table.Id);
            var hubName = GetValue(table, "Name");
            hubIdsByTableId[table.Id] = hubId;

            AddRecord(rawWorkspace, "RawHub", hubId, values =>
            {
                values["Name"] = hubName;
            }, relationships =>
            {
                relationships["SourceTableId"] = table.Id;
            });

            var orderedKeyFields = keySelection.OrderedKeyFields
                .Select(keyField => fieldById[GetRelationshipId(keyField, "FieldId")])
                .ToList();

            if (orderedKeyFields.Count == 0)
            {
                continue;
            }

            var keyOrdinal = 1;
            foreach (var field in orderedKeyFields)
            {
                AddRecord(rawWorkspace, "RawHubKeyPart", BuildRawHubKeyPartId(hubId, field.Id), values =>
                {
                    values["Name"] = GetValue(field, "Name");
                    values["Ordinal"] = keyOrdinal.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }, relationships =>
                {
                    relationships["RawHubId"] = hubId;
                    relationships["SourceFieldId"] = field.Id;
                });
                keyOrdinal++;
            }

            var keyFieldIds = orderedKeyFields.Select(field => field.Id).ToHashSet(StringComparer.Ordinal);
            var satelliteFields = fieldsByTableId.TryGetValue(table.Id, out var tableFields)
                ? tableFields.Where(field =>
                        !keyFieldIds.Contains(field.Id) &&
                        !relationshipSourceFieldIdsByTableId.Contains(field.Id) &&
                        !ShouldIgnoreField(GetValue(field, "Name"), ignoredFieldNameSet, ignoredFieldSuffixSet))
                    .ToList()
                : new List<GenericRecord>();

            if (satelliteFields.Count == 0)
            {
                continue;
            }

            var satelliteId = BuildRawHubSatelliteId(hubId);
            AddRecord(rawWorkspace, "RawHubSatellite", satelliteId, values =>
            {
                values["Name"] = DefaultSatelliteName;
                values["SatelliteKind"] = StandardSatelliteKind;
            }, relationships =>
            {
                relationships["RawHubId"] = hubId;
                relationships["SourceTableId"] = table.Id;
            });

            for (var i = 0; i < satelliteFields.Count; i++)
            {
                var field = satelliteFields[i];
                AddRecord(rawWorkspace, "RawHubSatelliteAttribute", BuildRawHubSatelliteAttributeId(satelliteId, field.Id), values =>
                {
                    values["Name"] = GetValue(field, "Name");
                    values["Ordinal"] = (i + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);
                }, relationships =>
                {
                    relationships["RawHubSatelliteId"] = satelliteId;
                    relationships["SourceFieldId"] = field.Id;
                });
            }
        }

        foreach (var relationship in includedRelationships)
        {
            var sourceTableId = GetRelationshipId(relationship, "SourceTableId");
            var targetTableId = GetRelationshipId(relationship, "TargetTableId");
            if (!hubIdsByTableId.TryGetValue(sourceTableId, out var sourceHubId) ||
                !hubIdsByTableId.TryGetValue(targetTableId, out var targetHubId))
            {
                continue;
            }

            var linkId = BuildRawLinkId(relationship.Id);
            var sourceTable = tableById[sourceTableId];
            var targetTable = tableById[targetTableId];

            AddRecord(rawWorkspace, "RawLink", linkId, values =>
            {
                values["Name"] = BuildRawLinkName(relationship, sourceTable, targetTable);
                values["LinkKind"] = StandardLinkKind;
            }, relationships =>
            {
                relationships["SourceTableRelationshipId"] = relationship.Id;
            });

            AddRecord(rawWorkspace, "RawLinkHub", BuildRawLinkHubId(linkId, "source"), values =>
            {
                values["Ordinal"] = "1";
                values["RoleName"] = BuildRoleName(sourceTable, targetTable, isSource: true);
            }, relationships =>
            {
                relationships["RawLinkId"] = linkId;
                relationships["RawHubId"] = sourceHubId;
            });

            AddRecord(rawWorkspace, "RawLinkHub", BuildRawLinkHubId(linkId, "target"), values =>
            {
                values["Ordinal"] = "2";
                values["RoleName"] = BuildRoleName(sourceTable, targetTable, isSource: false);
            }, relationships =>
            {
                relationships["RawLinkId"] = linkId;
                relationships["RawHubId"] = targetHubId;
            });
        }

        return rawWorkspace;
    }

    private static Dictionary<string, CandidateKeySelection> SelectCandidateKeys(
        IReadOnlyList<GenericRecord> tableKeys,
        IReadOnlyList<GenericRecord> tableKeyFields,
        HashSet<string> includedTableIds,
        HashSet<string> includedFieldIds,
        IReadOnlyDictionary<string, GenericRecord> fieldById,
        ISet<string> ignoredFieldNames,
        ISet<string> ignoredFieldSuffixes)
    {
        var keyFieldsByKeyId = tableKeyFields
            .Where(record => includedFieldIds.Contains(GetRelationshipId(record, "FieldId")))
            .GroupBy(record => GetRelationshipId(record, "TableKeyId"), StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group
                .OrderBy(record => ParseInt32(GetValue(record, "Ordinal"), int.MaxValue))
                .ThenBy(record => record.Id, StringComparer.OrdinalIgnoreCase)
                .ThenBy(record => record.Id, StringComparer.Ordinal)
                .ToList(), StringComparer.Ordinal);

        return tableKeys
            .Where(record => includedTableIds.Contains(GetRelationshipId(record, "TableId")))
            .Select(record => new CandidateKeySelection(
                record,
                keyFieldsByKeyId.TryGetValue(record.Id, out var orderedKeyFields)
                    ? orderedKeyFields
                        .Where(keyField =>
                        {
                            var fieldId = GetRelationshipId(keyField, "FieldId");
                            return fieldById.TryGetValue(fieldId, out var field) &&
                                   !ShouldIgnoreField(GetValue(field, "Name"), ignoredFieldNames, ignoredFieldSuffixes) &&
                                   !IsRecognizedTechnicalFieldName(GetValue(field, "Name"));
                        })
                        .ToList()
                    : new List<GenericRecord>()))
            .Where(selection => selection.OrderedKeyFields.Count > 0)
            .GroupBy(selection => GetRelationshipId(selection.TableKey, "TableId"), StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderBy(selection => GetKeyPriority(GetValue(selection.TableKey, "KeyType")))
                    .ThenBy(selection => GetValue(selection.TableKey, "Name"), StringComparer.OrdinalIgnoreCase)
                    .ThenBy(selection => selection.TableKey.Id, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(selection => selection.TableKey.Id, StringComparer.Ordinal)
                    .First(),
                StringComparer.Ordinal);
    }

    private static bool IsIncludedTable(GenericRecord table)
    {
        var objectType = GetValue(table, "ObjectType");
        return !string.Equals(objectType, "View", StringComparison.OrdinalIgnoreCase);
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

    private static bool IsRecognizedTechnicalFieldName(string fieldName)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            return false;
        }

        return string.Equals(fieldName, "AuditId", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(fieldName, "RecordSource", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(fieldName, "LoadTimestamp", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(fieldName, "SnapshotTimestamp", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(fieldName, "HashDiff", StringComparison.OrdinalIgnoreCase) ||
               fieldName.EndsWith("HashKey", StringComparison.OrdinalIgnoreCase) ||
               fieldName.EndsWith("LoadTimestamp", StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildRawHubId(string tableId) => "rawhub:" + tableId;

    private static string BuildRawHubKeyPartId(string hubId, string fieldId) => $"{hubId}:key:{fieldId}";

    private static string BuildRawHubSatelliteId(string hubId) => $"{hubId}:sat";

    private static string BuildRawHubSatelliteAttributeId(string satelliteId, string fieldId) => $"{satelliteId}:attr:{fieldId}";

    private static string BuildRawLinkId(string relationshipId) => "rawlink:" + relationshipId;

    private static string BuildRawLinkHubId(string linkId, string role) => $"{linkId}:{role}";

    private static string BuildRawLinkName(GenericRecord relationship, GenericRecord sourceTable, GenericRecord targetTable)
    {
        var relationshipName = GetValue(relationship, "Name");
        if (!string.IsNullOrWhiteSpace(relationshipName))
        {
            return relationshipName;
        }

        return GetValue(sourceTable, "Name") + GetValue(targetTable, "Name");
    }

    private static string BuildRoleName(GenericRecord sourceTable, GenericRecord targetTable, bool isSource)
    {
        var sourceName = GetValue(sourceTable, "Name");
        var targetName = GetValue(targetTable, "Name");
        if (!string.Equals(sourceName, targetName, StringComparison.OrdinalIgnoreCase))
        {
            return isSource ? sourceName : targetName;
        }

        return isSource ? "Source" + sourceName : "Target" + targetName;
    }

    private static void AddRecord(
        Workspace workspace,
        string entityName,
        string id,
        Action<Dictionary<string, string>>? populateValues = null,
        Action<Dictionary<string, string>>? populateRelationships = null)
    {
        var record = new GenericRecord
        {
            Id = id,
            SourceShardFileName = entityName + ".xml",
        };
        populateValues?.Invoke(record.Values);
        populateRelationships?.Invoke(record.RelationshipIds);
        workspace.Instance.GetOrCreateEntityRecords(entityName).Add(record);
    }

    private static void AddOptionalValue(Dictionary<string, string> values, string key, string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            values[key] = value;
        }
    }

    private static string GetValue(GenericRecord record, string key)
    {
        return record.Values.TryGetValue(key, out var value)
            ? value ?? string.Empty
            : string.Empty;
    }

    private static string GetRelationshipId(GenericRecord record, string key)
    {
        return record.RelationshipIds.TryGetValue(key, out var value)
            ? value ?? string.Empty
            : string.Empty;
    }

    private static int ParseInt32(string value, int defaultValue)
    {
        return int.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : defaultValue;
    }

    private sealed record CandidateKeySelection(
        GenericRecord TableKey,
        IReadOnlyList<GenericRecord> OrderedKeyFields);
}
