using Meta.Core.Domain;
using MRDV = MetaRawDataVault;

namespace MetaDataVault.Core;

public sealed partial class MetaSchemaToRawDataVaultConverter
{
    private static Workspace CreateWorkspace(SchemaBootstrapDraft draft, string newWorkspacePath)
    {
        var workspace = MetaDataVaultWorkspaces.CreateEmptyMetaRawDataVaultWorkspace(newWorkspacePath);

        AddRecords(workspace, "SourceSystem", draft.SourceSystems, row =>
        {
            row.Values["Name"] = row.Entity.Name;
            AddOptionalValue(row.Values, "Description", row.Entity.Description);
        });
        AddRecords(workspace, "SourceSchema", draft.SourceSchemas, row =>
        {
            row.Values["Name"] = row.Entity.Name;
            row.RelationshipIds["SourceSystemId"] = row.Entity.SourceSystemId;
        });
        AddRecords(workspace, "SourceTable", draft.SourceTables, row =>
        {
            row.Values["Name"] = row.Entity.Name;
            row.RelationshipIds["SourceSchemaId"] = row.Entity.SourceSchemaId;
        });
        AddRecords(workspace, "SourceField", draft.SourceFields, row =>
        {
            row.Values["Name"] = row.Entity.Name;
            row.Values["DataTypeId"] = row.Entity.DataTypeId;
            AddOptionalValue(row.Values, "Ordinal", row.Entity.Ordinal);
            AddOptionalValue(row.Values, "IsNullable", row.Entity.IsNullable);
            row.RelationshipIds["SourceTableId"] = row.Entity.SourceTableId;
        });
        AddRecords(workspace, "SourceFieldDataTypeDetail", draft.SourceFieldDetails, row =>
        {
            row.Values["Name"] = row.Entity.Name;
            row.Values["Value"] = row.Entity.Value;
            row.RelationshipIds["SourceFieldId"] = row.Entity.SourceFieldId;
        });
        AddRecords(workspace, "SourceTableRelationship", draft.SourceRelationships, row =>
        {
            row.Values["Name"] = row.Entity.Name;
            row.RelationshipIds["SourceTableId"] = row.Entity.SourceTableId;
            row.RelationshipIds["TargetTableId"] = row.Entity.TargetTableId;
        });
        AddRecords(workspace, "SourceTableRelationshipField", draft.SourceRelationshipFields, row =>
        {
            row.Values["Ordinal"] = row.Entity.Ordinal;
            row.RelationshipIds["SourceTableRelationshipId"] = row.Entity.SourceTableRelationshipId;
            row.RelationshipIds["SourceFieldId"] = row.Entity.SourceFieldId;
            row.RelationshipIds["TargetFieldId"] = row.Entity.TargetFieldId;
        });
        AddRecords(workspace, "RawHub", draft.RawHubs, row =>
        {
            row.Values["Name"] = row.Entity.Name;
            row.RelationshipIds["SourceTableId"] = row.Entity.SourceTableId;
        });
        AddRecords(workspace, "RawHubKeyPart", draft.RawHubKeyParts, row =>
        {
            row.Values["Name"] = row.Entity.Name;
            row.Values["Ordinal"] = row.Entity.Ordinal;
            row.RelationshipIds["RawHubId"] = row.Entity.RawHubId;
            row.RelationshipIds["SourceFieldId"] = row.Entity.SourceFieldId;
        });
        AddRecords(workspace, "RawHubSatellite", draft.RawHubSatellites, row =>
        {
            row.Values["Name"] = row.Entity.Name;
            row.Values["SatelliteKind"] = row.Entity.SatelliteKind;
            row.RelationshipIds["RawHubId"] = row.Entity.RawHubId;
            row.RelationshipIds["SourceTableId"] = row.Entity.SourceTableId;
        });
        AddRecords(workspace, "RawHubSatelliteAttribute", draft.RawHubSatelliteAttributes, row =>
        {
            row.Values["Name"] = row.Entity.Name;
            row.Values["Ordinal"] = row.Entity.Ordinal;
            row.RelationshipIds["RawHubSatelliteId"] = row.Entity.RawHubSatelliteId;
            row.RelationshipIds["SourceFieldId"] = row.Entity.SourceFieldId;
        });
        AddRecords(workspace, "RawLink", draft.RawLinks, row =>
        {
            row.Values["Name"] = row.Entity.Name;
            row.Values["LinkKind"] = row.Entity.LinkKind;
            row.RelationshipIds["SourceTableRelationshipId"] = row.Entity.SourceTableRelationshipId;
        });
        AddRecords(workspace, "RawLinkHub", draft.RawLinkHubs, row =>
        {
            row.Values["Ordinal"] = row.Entity.Ordinal;
            AddOptionalValue(row.Values, "RoleName", row.Entity.RoleName);
            row.RelationshipIds["RawLinkId"] = row.Entity.RawLinkId;
            row.RelationshipIds["RawHubId"] = row.Entity.RawHubId;
        });
        AddRecords(workspace, "RawLinkSatellite", draft.RawLinkSatellites, row =>
        {
            row.Values["Name"] = row.Entity.Name;
            row.Values["SatelliteKind"] = row.Entity.SatelliteKind;
            row.RelationshipIds["RawLinkId"] = row.Entity.RawLinkId;
            row.RelationshipIds["SourceTableId"] = row.Entity.SourceTableId;
        });
        AddRecords(workspace, "RawLinkSatelliteAttribute", draft.RawLinkSatelliteAttributes, row =>
        {
            row.Values["Name"] = row.Entity.Name;
            row.Values["Ordinal"] = row.Entity.Ordinal;
            row.RelationshipIds["RawLinkSatelliteId"] = row.Entity.RawLinkSatelliteId;
            row.RelationshipIds["SourceFieldId"] = row.Entity.SourceFieldId;
        });

        return workspace;
    }

    private static void AddRecords<T>(
        Workspace workspace,
        string entityName,
        IEnumerable<T> entities,
        Action<SerializationRow<T>> populate)
        where T : class
    {
        var records = workspace.Instance.GetOrCreateEntityRecords(entityName);
        foreach (var entity in entities)
        {
            var record = new GenericRecord
            {
                Id = GetId(entity),
                SourceShardFileName = entityName + ".xml",
            };
            populate(new SerializationRow<T>(entity, record.Values, record.RelationshipIds));
            records.Add(record);
        }
    }

    private static string GetId<T>(T entity)
        where T : class
    {
        return entity switch
        {
            MRDV.SourceSystem row => row.Id,
            MRDV.SourceSchema row => row.Id,
            MRDV.SourceTable row => row.Id,
            MRDV.SourceField row => row.Id,
            MRDV.SourceFieldDataTypeDetail row => row.Id,
            MRDV.SourceTableRelationship row => row.Id,
            MRDV.SourceTableRelationshipField row => row.Id,
            MRDV.RawHub row => row.Id,
            MRDV.RawHubKeyPart row => row.Id,
            MRDV.RawHubSatellite row => row.Id,
            MRDV.RawHubSatelliteAttribute row => row.Id,
            MRDV.RawLink row => row.Id,
            MRDV.RawLinkHub row => row.Id,
            MRDV.RawLinkSatellite row => row.Id,
            MRDV.RawLinkSatelliteAttribute row => row.Id,
            _ => throw new InvalidOperationException($"Unsupported raw datavault entity type '{typeof(T).Name}'."),
        };
    }

    private static void AddOptionalValue(Dictionary<string, string> values, string key, string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            values[key] = value;
        }
    }

    private readonly record struct SerializationRow<T>(
        T Entity,
        Dictionary<string, string> Values,
        Dictionary<string, string> RelationshipIds)
        where T : class;
}
