using Meta.Core.Domain;
using Meta.Core.Services;
using MetaDataTypeConversion.Instance;

namespace MetaDataTypeConversion.Core;

public static class MetaDataTypeConversionWorkspaceProvider
{
    private static readonly Lazy<Workspace> DefaultWorkspace = new(
        CreateDefaultWorkspace,
        LazyThreadSafetyMode.ExecutionAndPublication);

    public static Workspace LoadOrDefault(string? workspacePath)
    {
        if (string.IsNullOrWhiteSpace(workspacePath))
        {
            return DefaultWorkspace.Value;
        }

        return new WorkspaceService()
            .LoadAsync(Path.GetFullPath(workspacePath), searchUpward: false)
            .GetAwaiter()
            .GetResult();
    }

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
                    ["Description"] = implementation.Description ?? string.Empty
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
                    ["Notes"] = mapping.Notes ?? string.Empty
                },
                RelationshipIds =
                {
                    ["ConversionImplementationId"] = mapping.ConversionImplementation.Id
                }
            });
        }

        return workspace;
    }
}
