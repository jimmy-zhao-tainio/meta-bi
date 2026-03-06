using Meta.Core.Domain;

namespace MetaDataVault.Core;

public sealed class MetaSchemaToRawDataVaultConverter
{
    public Workspace Convert(Workspace metaSchemaWorkspace, string newWorkspacePath)
    {
        ArgumentNullException.ThrowIfNull(metaSchemaWorkspace);
        ArgumentException.ThrowIfNullOrWhiteSpace(newWorkspacePath);

        if (!string.Equals(metaSchemaWorkspace.Model.Name, "MetaSchema", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Source workspace model must be 'MetaSchema', got '{metaSchemaWorkspace.Model.Name}'.");
        }

        var sourceSystems = metaSchemaWorkspace.Instance.GetOrCreateEntityRecords("System")
            .OrderBy(record => record.Id, StringComparer.Ordinal)
            .ToList();
        var sourceSchemas = metaSchemaWorkspace.Instance.GetOrCreateEntityRecords("Schema")
            .OrderBy(record => record.Id, StringComparer.Ordinal)
            .ToList();
        var sourceTables = metaSchemaWorkspace.Instance.GetOrCreateEntityRecords("Table")
            .OrderBy(record => record.Id, StringComparer.Ordinal)
            .ToList();
        var sourceFields = metaSchemaWorkspace.Instance.GetOrCreateEntityRecords("Field")
            .OrderBy(record => record.Id, StringComparer.Ordinal)
            .ToList();
        var sourceTableRelationships = metaSchemaWorkspace.Instance.GetOrCreateEntityRecords("TableRelationship")
            .OrderBy(record => record.Id, StringComparer.Ordinal)
            .ToList();
        var sourceTableRelationshipFields = metaSchemaWorkspace.Instance.GetOrCreateEntityRecords("TableRelationshipField")
            .OrderBy(record => record.Id, StringComparer.Ordinal)
            .ToList();

        var result = MetaDataVaultWorkspaces.CreateEmptyMetaRawDataVaultWorkspace(newWorkspacePath);
        var instance = result.Instance;

        CopyRecords(sourceSystems, instance.GetOrCreateEntityRecords("SourceSystem"), "SourceSystem.xml", null);
        CopyRecords(sourceSchemas, instance.GetOrCreateEntityRecords("SourceSchema"), "SourceSchema.xml", null, ("SystemId", "SourceSystemId"));
        CopyRecords(sourceTables, instance.GetOrCreateEntityRecords("SourceTable"), "SourceTable.xml", new[] { "Name" }, ("SchemaId", "SourceSchemaId"));
        CopyRecords(sourceFields, instance.GetOrCreateEntityRecords("SourceField"), "SourceField.xml", null, ("TableId", "SourceTableId"));
        CopyRecords(sourceTableRelationships, instance.GetOrCreateEntityRecords("SourceTableRelationship"), "SourceTableRelationship.xml", null, ("SourceTableId", "SourceTableId"));
        CopyRecords(sourceTableRelationshipFields, instance.GetOrCreateEntityRecords("SourceTableRelationshipField"), "SourceTableRelationshipField.xml", null, ("TableRelationshipId", "SourceTableRelationshipId"), ("SourceFieldId", "SourceFieldId"));

        var fieldsByTableId = sourceFields
            .GroupBy(field => RequireRelationshipId(field, "TableId"), StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.ToList(), StringComparer.Ordinal);
        var sourceSchemaNameById = sourceSchemas.ToDictionary(
            item => item.Id,
            item => RequireValue(item, "Name"),
            StringComparer.Ordinal);
        var sourceTableIdBySchemaAndName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var sourceTable in sourceTables)
        {
            var sourceSchemaId = RequireRelationshipId(sourceTable, "SchemaId");
            if (!sourceSchemaNameById.TryGetValue(sourceSchemaId, out var sourceSchemaName))
            {
                throw new InvalidOperationException(
                    $"Source table '{sourceTable.Id}' references missing schema '{sourceSchemaId}'.");
            }

            var sourceTableName = RequireValue(sourceTable, "Name");
            var key = BuildSchemaTableKey(sourceSchemaName, sourceTableName);
            if (!sourceTableIdBySchemaAndName.ContainsKey(key))
            {
                sourceTableIdBySchemaAndName[key] = sourceTable.Id;
            }
        }

        var rawHubs = instance.GetOrCreateEntityRecords("RawHub");
        var rawHubKeyParts = instance.GetOrCreateEntityRecords("RawHubKeyPart");
        var rawHubSatellites = instance.GetOrCreateEntityRecords("RawHubSatellite");
        var rawHubSatelliteAttributes = instance.GetOrCreateEntityRecords("RawHubSatelliteAttribute");
        var rawLinks = instance.GetOrCreateEntityRecords("RawLink");
        var rawLinkEnds = instance.GetOrCreateEntityRecords("RawLinkEnd");
        var rawLinkSatellites = instance.GetOrCreateEntityRecords("RawLinkSatellite");
        var rawLinkSatelliteAttributes = instance.GetOrCreateEntityRecords("RawLinkSatelliteAttribute");
        var rawHubIdBySourceTableId = new Dictionary<string, string>(StringComparer.Ordinal);
        var rawHubById = new Dictionary<string, GenericRecord>(StringComparer.Ordinal);

        foreach (var sourceTable in sourceTables)
        {
            var sourceTableId = sourceTable.Id;
            var tableName = RequireValue(sourceTable, "Name");
            fieldsByTableId.TryGetValue(sourceTableId, out var tableFields);
            tableFields ??= new List<GenericRecord>();

            var selectedBusinessKeyField = SelectBusinessKeyField(tableName, tableFields);
            var hubId = "hub:" + sourceTableId;
            var hubKeyColumnName = BuildHashKeyColumnName(tableName);
            var satelliteId = "hubsat:" + sourceTableId;

            var hub = new GenericRecord
            {
                Id = hubId,
                SourceShardFileName = "RawHub.xml",
            };
            hub.Values["Name"] = tableName;
            hub.Values["HubKeyColumnName"] = hubKeyColumnName;
            hub.Values["LoadTimestampColumnName"] = "LoadTimestamp";
            hub.Values["RecordSourceColumnName"] = "RecordSource";
            hub.RelationshipIds["SourceTableId"] = sourceTableId;
            rawHubs.Add(hub);
            rawHubIdBySourceTableId[sourceTableId] = hubId;
            rawHubById[hubId] = hub;

            var hubKeyPart = new GenericRecord
            {
                Id = "hubkey:" + sourceTableId + ":1",
                SourceShardFileName = "RawHubKeyPart.xml",
            };
            hubKeyPart.Values["BusinessKeyColumnName"] = RequireValue(selectedBusinessKeyField, "Name");
            hubKeyPart.Values["Ordinal"] = "1";
            hubKeyPart.RelationshipIds["RawHubId"] = hubId;
            hubKeyPart.RelationshipIds["SourceFieldId"] = selectedBusinessKeyField.Id;
            rawHubKeyParts.Add(hubKeyPart);

            var hubSatellite = new GenericRecord
            {
                Id = satelliteId,
                SourceShardFileName = "RawHubSatellite.xml",
            };
            hubSatellite.Values["Name"] = tableName + "Sat";
            hubSatellite.Values["SatelliteKind"] = "standard";
            hubSatellite.Values["ParentHubKeyColumnName"] = hubKeyColumnName;
            hubSatellite.Values["LoadTimestampColumnName"] = "LoadTimestamp";
            hubSatellite.Values["RecordSourceColumnName"] = "RecordSource";
            hubSatellite.Values["HashDiffColumnName"] = tableName + "HashDiff";
            hubSatellite.RelationshipIds["RawHubId"] = hubId;
            hubSatellite.RelationshipIds["SourceTableId"] = sourceTableId;
            rawHubSatellites.Add(hubSatellite);

            var satelliteFields = tableFields
                .Where(field => !string.Equals(field.Id, selectedBusinessKeyField.Id, StringComparison.Ordinal))
                .OrderBy(GetFieldOrdinal)
                .ThenBy(field => field.Values.TryGetValue("Name", out var name) ? name : string.Empty, StringComparer.Ordinal)
                .ThenBy(field => field.Id, StringComparer.Ordinal)
                .ToList();

            for (var i = 0; i < satelliteFields.Count; i++)
            {
                var field = satelliteFields[i];
                var satelliteAttribute = new GenericRecord
                {
                    Id = "hubsatattr:" + sourceTableId + ":" + (i + 1).ToString(),
                    SourceShardFileName = "RawHubSatelliteAttribute.xml",
                };
                satelliteAttribute.Values["ColumnName"] = RequireValue(field, "Name");
                satelliteAttribute.Values["Ordinal"] = (i + 1).ToString();
                satelliteAttribute.RelationshipIds["RawHubSatelliteId"] = satelliteId;
                satelliteAttribute.RelationshipIds["SourceFieldId"] = field.Id;
                rawHubSatelliteAttributes.Add(satelliteAttribute);
            }
        }

        foreach (var sourceTableRelationship in sourceTableRelationships)
        {
            var sourceTableId = RequireRelationshipId(sourceTableRelationship, "SourceTableId");
            if (!rawHubIdBySourceTableId.TryGetValue(sourceTableId, out var sourceHubId))
            {
                continue;
            }

            var targetSchemaName = RequireValue(sourceTableRelationship, "TargetSchemaName");
            var targetTableName = RequireValue(sourceTableRelationship, "TargetTableName");
            var targetKey = BuildSchemaTableKey(targetSchemaName, targetTableName);
            if (!sourceTableIdBySchemaAndName.TryGetValue(targetKey, out var targetSourceTableId))
            {
                continue;
            }

            if (!rawHubIdBySourceTableId.TryGetValue(targetSourceTableId, out var targetHubId))
            {
                continue;
            }

            var linkName = RequireValue(sourceTableRelationship, "Name");
            var linkId = "link:" + sourceTableRelationship.Id;
            var link = new GenericRecord
            {
                Id = linkId,
                SourceShardFileName = "RawLink.xml",
            };
            link.Values["Name"] = linkName;
            link.Values["LinkKind"] = "standard";
            link.Values["LinkKeyColumnName"] = BuildHashKeyColumnName(linkName);
            link.Values["LoadTimestampColumnName"] = "LoadTimestamp";
            link.Values["RecordSourceColumnName"] = "RecordSource";
            link.RelationshipIds["SourceTableRelationshipId"] = sourceTableRelationship.Id;
            rawLinks.Add(link);

            var sourceEnd = new GenericRecord
            {
                Id = linkId + ":end:1",
                SourceShardFileName = "RawLinkEnd.xml",
            };
            sourceEnd.Values["Ordinal"] = "1";
            sourceEnd.Values["RoleName"] = "Source";
            sourceEnd.Values["LinkReferenceColumnName"] = BuildLinkReferenceColumnName(rawHubById[sourceHubId]);
            sourceEnd.RelationshipIds["RawLinkId"] = linkId;
            sourceEnd.RelationshipIds["RawHubId"] = sourceHubId;
            rawLinkEnds.Add(sourceEnd);

            var targetEnd = new GenericRecord
            {
                Id = linkId + ":end:2",
                SourceShardFileName = "RawLinkEnd.xml",
            };
            targetEnd.Values["Ordinal"] = "2";
            targetEnd.Values["RoleName"] = "Target";
            targetEnd.Values["LinkReferenceColumnName"] = BuildLinkReferenceColumnName(rawHubById[targetHubId]);
            targetEnd.RelationshipIds["RawLinkId"] = linkId;
            targetEnd.RelationshipIds["RawHubId"] = targetHubId;
            rawLinkEnds.Add(targetEnd);
        }

        _ = rawLinkSatellites;
        _ = rawLinkSatelliteAttributes;

        result.IsDirty = true;
        return result;
    }

    private static void CopyRecords(
        IEnumerable<GenericRecord> source,
        ICollection<GenericRecord> destination,
        string shardFileName,
        IReadOnlyCollection<string>? valuePropertyNames,
        params (string SourceName, string TargetName)[] relationshipNameMap)
    {
        var relationshipMap = relationshipNameMap.ToDictionary(item => item.SourceName, item => item.TargetName, StringComparer.Ordinal);
        var valueNameFilter = valuePropertyNames == null
            ? null
            : new HashSet<string>(valuePropertyNames, StringComparer.Ordinal);

        foreach (var record in source)
        {
            var copy = new GenericRecord
            {
                Id = record.Id,
                SourceShardFileName = shardFileName,
            };
            foreach (var value in record.Values.OrderBy(item => item.Key, StringComparer.Ordinal))
            {
                if (valueNameFilter != null && !valueNameFilter.Contains(value.Key))
                {
                    continue;
                }

                copy.Values[value.Key] = value.Value;
            }

            foreach (var rel in record.RelationshipIds.OrderBy(item => item.Key, StringComparer.Ordinal))
            {
                var relationshipName = relationshipMap.TryGetValue(rel.Key, out var mappedName)
                    ? mappedName
                    : rel.Key;
                copy.RelationshipIds[relationshipName] = rel.Value;
            }

            destination.Add(copy);
        }
    }

    private static GenericRecord SelectBusinessKeyField(string tableName, IReadOnlyList<GenericRecord> fields)
    {
        if (fields.Count == 0)
        {
            throw new InvalidOperationException(
                $"Cannot build raw hub for table '{tableName}' because the table has no fields.");
        }

        var targetName = tableName + "Id";
        var match = fields.FirstOrDefault(field =>
            field.Values.TryGetValue("Name", out var name) &&
            string.Equals(name, targetName, StringComparison.OrdinalIgnoreCase));
        if (match != null)
        {
            return match;
        }

        match = fields.FirstOrDefault(field =>
            field.Values.TryGetValue("Name", out var name) &&
            string.Equals(name, "Id", StringComparison.OrdinalIgnoreCase));
        if (match != null)
        {
            return match;
        }

        return fields
            .OrderBy(GetFieldOrdinal)
            .ThenBy(field => field.Values.TryGetValue("Name", out var name) ? name : string.Empty, StringComparer.Ordinal)
            .ThenBy(field => field.Id, StringComparer.Ordinal)
            .First();
    }

    private static int GetFieldOrdinal(GenericRecord field)
    {
        if (!field.Values.TryGetValue("Ordinal", out var ordinalValue))
        {
            return int.MaxValue;
        }

        if (int.TryParse(ordinalValue, out var ordinal))
        {
            return ordinal;
        }

        return int.MaxValue;
    }

    private static string BuildHashKeyColumnName(string name)
    {
        return name + "Hk";
    }

    private static string BuildLinkReferenceColumnName(GenericRecord rawHub)
    {
        return RequireValue(rawHub, "HubKeyColumnName");
    }

    private static string BuildSchemaTableKey(string schemaName, string tableName)
    {
        return schemaName + "\u001F" + tableName;
    }

    private static string RequireValue(GenericRecord record, string propertyName)
    {
        if (!record.Values.TryGetValue(propertyName, out var value) || string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Record '{record.Id}' is missing required property '{propertyName}'.");
        }

        return value;
    }

    private static string RequireRelationshipId(GenericRecord record, string relationshipName)
    {
        if (!record.RelationshipIds.TryGetValue(relationshipName, out var value) || string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Record '{record.Id}' is missing required relationship '{relationshipName}'.");
        }

        return value;
    }
}

