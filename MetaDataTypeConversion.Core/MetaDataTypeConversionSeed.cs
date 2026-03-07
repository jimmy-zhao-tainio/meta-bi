using Meta.Core.Domain;

namespace MetaDataTypeConversion.Core;

internal static class MetaDataTypeConversionSeed
{
    public static void Populate(GenericInstance instance)
    {
        ArgumentNullException.ThrowIfNull(instance);

        foreach (var implementation in MetaDataTypeConversionSeedData.ConversionImplementations)
        {
            var record = new GenericRecord
            {
                Id = implementation.Id
            };
            record.Values["Name"] = implementation.Name;
            if (!string.IsNullOrWhiteSpace(implementation.Description))
            {
                record.Values["Description"] = implementation.Description;
            }

            instance.GetOrCreateEntityRecords("ConversionImplementation").Add(record);
        }

        foreach (var mapping in MetaDataTypeConversionSeedData.TypeMappings)
        {
            var record = new GenericRecord
            {
                Id = mapping.Id,
                RelationshipIds =
                {
                    ["ConversionImplementationId"] = mapping.ConversionImplementationId
                }
            };
            record.Values["SourceTypeId"] = mapping.SourceTypeId;
            record.Values["TargetTypeId"] = mapping.TargetTypeId;
            if (!string.IsNullOrWhiteSpace(mapping.Notes))
            {
                record.Values["Notes"] = mapping.Notes;
            }

            instance.GetOrCreateEntityRecords("TypeMapping").Add(record);
        }
    }
}
