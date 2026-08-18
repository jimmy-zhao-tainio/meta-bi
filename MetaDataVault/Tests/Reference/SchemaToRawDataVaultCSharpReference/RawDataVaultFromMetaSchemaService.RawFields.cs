using MRDV = global::MetaRawDataVault;

namespace MetaConvert.SchemaToDataVault.Reference;

public sealed partial class RawDataVaultFromMetaSchemaService
{
    private static FromMetaSchemaDraft CreateRawDraft(SourceIndex sourceIndex)
    {
        var draft = new FromMetaSchemaDraft();

        foreach (var sourceField in sourceIndex.IncludedFields)
        {
            var field = new MRDV.Field
            {
                Id = sourceField.Id,
                Name = sourceField.Name,
                DataTypeId = sourceField.MetaDataTypeId,
            };
            draft.Fields.Add(field);
            draft.FieldsById[field.Id] = field;
        }

        foreach (var sourceDetail in sourceIndex.IncludedFieldDetails)
        {
            draft.FieldDetails.Add(new MRDV.FieldDataTypeDetail
            {
                Id = sourceDetail.Id,
                Name = sourceDetail.Name,
                Value = sourceDetail.Value,
                Field = draft.FieldsById[sourceDetail.Field.Id],
            });
        }

        return draft;
    }
}
