using MRDV = global::MetaRawDataVault;

namespace MetaConvert.SchemaToDataVault;

public sealed partial class RawDataVaultFromMetaSchemaService
{
    private static FromMetaSchemaDraft CopySourceStructure(SourceIndex sourceIndex)
    {
        var draft = new FromMetaSchemaDraft();

        foreach (var system in sourceIndex.IncludedSystems)
        {
            var sourceSystem = new MRDV.SourceSystem
            {
                Id = system.Id,
                Name = system.Name,
                Description = system.Description,
            };
            draft.SourceSystems.Add(sourceSystem);
            draft.SourceSystemsById[sourceSystem.Id] = sourceSystem;
        }

        foreach (var schema in sourceIndex.IncludedSchemas)
        {
            var sourceSchema = new MRDV.SourceSchema
            {
                Id = schema.Id,
                Name = schema.Name,
                SourceSystemId = schema.SystemId,
                SourceSystem = draft.SourceSystemsById[schema.SystemId],
            };
            draft.SourceSchemas.Add(sourceSchema);
            draft.SourceSchemasById[sourceSchema.Id] = sourceSchema;
        }

        foreach (var table in sourceIndex.IncludedTables)
        {
            var sourceTable = new MRDV.SourceTable
            {
                Id = table.Id,
                Name = table.Name,
                SourceSchemaId = table.SchemaId,
                SourceSchema = draft.SourceSchemasById[table.SchemaId],
            };
            draft.SourceTables.Add(sourceTable);
            draft.SourceTablesById[sourceTable.Id] = sourceTable;
        }

        foreach (var field in sourceIndex.IncludedFields)
        {
            var sourceField = new MRDV.SourceField
            {
                Id = field.Id,
                Name = field.Name,
                DataTypeId = field.MetaDataTypeId,
                Ordinal = field.Ordinal,
                IsNullable = field.IsNullable,
                SourceTableId = field.TableId,
                SourceTable = draft.SourceTablesById[field.TableId],
            };
            draft.SourceFields.Add(sourceField);
            draft.SourceFieldsById[sourceField.Id] = sourceField;
        }

        foreach (var detail in sourceIndex.IncludedFieldDetails)
        {
            draft.SourceFieldDetails.Add(new MRDV.SourceFieldDataTypeDetail
            {
                Id = detail.Id,
                Name = detail.Name,
                Value = detail.Value,
                SourceFieldId = detail.FieldId,
                SourceField = draft.SourceFieldsById[detail.FieldId],
            });
        }

        foreach (var relationship in sourceIndex.IncludedRelationships)
        {
            var sourceRelationship = new MRDV.SourceTableRelationship
            {
                Id = relationship.Id,
                Name = relationship.Name,
                SourceTableId = relationship.SourceTableId,
                SourceTable = draft.SourceTablesById[relationship.SourceTableId],
                TargetTableId = relationship.TargetTableId,
                TargetTable = draft.SourceTablesById[relationship.TargetTableId],
            };
            draft.SourceRelationships.Add(sourceRelationship);
            draft.SourceRelationshipsById[sourceRelationship.Id] = sourceRelationship;
        }

        foreach (var relationshipField in sourceIndex.IncludedRelationshipFields)
        {
            draft.SourceRelationshipFields.Add(new MRDV.SourceTableRelationshipField
            {
                Id = relationshipField.Id,
                Ordinal = relationshipField.Ordinal,
                SourceTableRelationshipId = relationshipField.TableRelationshipId,
                SourceTableRelationship = draft.SourceRelationshipsById[relationshipField.TableRelationshipId],
                SourceFieldId = relationshipField.SourceFieldId,
                SourceField = draft.SourceFieldsById[relationshipField.SourceFieldId],
                TargetFieldId = relationshipField.TargetFieldId,
                TargetField = draft.SourceFieldsById[relationshipField.TargetFieldId],
            });
        }

        return draft;
    }
}
