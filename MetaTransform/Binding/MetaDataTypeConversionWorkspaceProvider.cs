using Meta.Core.Domain;
using MetaDataTypeConversion.Core;
using MetaDataTypeConversion.Instance;

namespace MetaTransform.Binding;

internal static class MetaDataTypeConversionWorkspaceProvider
{
    private static readonly Lazy<Workspace> DefaultWorkspace = new(
        CreateDefaultWorkspace,
        LazyThreadSafetyMode.ExecutionAndPublication);

    public static Workspace GetDefaultWorkspace() => DefaultWorkspace.Value;

    private static Workspace CreateDefaultWorkspace()
    {
        var workspace = MetaDataTypeConversionWorkspaceFactory.CreateEmptyWorkspace(
            Path.Combine(
                Path.GetTempPath(),
                "meta-bi",
                "MetaDataTypeConversion",
                "Default",
                "Workspace"),
            MetaDataTypeConversionModels.CreateMetaDataTypeConversionModel());

        foreach (var implementation in MetaDataTypeConversionInstance.Default.ConversionImplementationList)
        {
            workspace.Instance.GetOrCreateEntityRecords("ConversionImplementation").Add(new GenericRecord
            {
                Id = implementation.Id,
                SourceShardFileName = "ConversionImplementation.xml",
                Values =
                {
                    ["Name"] = implementation.Name,
                    ["Description"] = implementation.Description
                }
            });
        }

        foreach (var mapping in MetaDataTypeConversionInstance.Default.DataTypeMappingList)
        {
            workspace.Instance.GetOrCreateEntityRecords("DataTypeMapping").Add(new GenericRecord
            {
                Id = mapping.Id,
                SourceShardFileName = "DataTypeMapping.xml",
                Values =
                {
                    ["SourceDataTypeId"] = mapping.SourceDataTypeId,
                    ["TargetDataTypeId"] = mapping.TargetDataTypeId,
                    ["Notes"] = mapping.Notes
                },
                RelationshipIds =
                {
                    ["ConversionImplementationId"] = mapping.ConversionImplementationId
                }
            });
        }

        return workspace;
    }
}
