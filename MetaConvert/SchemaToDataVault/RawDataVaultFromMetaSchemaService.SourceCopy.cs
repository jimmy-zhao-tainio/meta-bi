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
                SourceSystem = draft.SourceSystemsById[schema.System.Id],
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
                SourceSchema = draft.SourceSchemasById[table.Schema.Id],
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
                SourceTable = draft.SourceTablesById[field.Table.Id],
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
                SourceField = draft.SourceFieldsById[detail.Field.Id],
            });
        }

        foreach (var relationship in sourceIndex.IncludedRelationships)
        {
            var sourceRelationship = new MRDV.SourceTableRelationship
            {
                Id = relationship.Id,
                Name = relationship.Name,
                SourceTable = draft.SourceTablesById[relationship.SourceTable.Id],
                TargetTable = draft.SourceTablesById[relationship.TargetTable.Id],
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
                SourceTableRelationship = draft.SourceRelationshipsById[relationshipField.TableRelationship.Id],
                SourceField = draft.SourceFieldsById[relationshipField.SourceField.Id],
                TargetField = draft.SourceFieldsById[relationshipField.TargetField.Id],
            });
        }

        return draft;
    }
}
