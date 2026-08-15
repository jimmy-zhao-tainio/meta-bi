using MetaDataTypeConversion;
namespace MetaDataTypeConversion.Core;

public static class MetaDataTypeConversionWorkspaceProvider
{
    private static readonly Lazy<MetaDataTypeConversionModel> DefaultWorkspace = new(
        () => MetaDataTypeConversionInstance.BuiltIn,
        LazyThreadSafetyMode.ExecutionAndPublication);

    public static MetaDataTypeConversionModel LoadOrDefault(string? workspacePath)
    {
        if (string.IsNullOrWhiteSpace(workspacePath))
        {
            return DefaultWorkspace.Value;
        }

        return Meta.Integration.TypedWorkspaceModelMapper.Load<MetaDataTypeConversionModel>(Path.GetFullPath(workspacePath), searchUpward: false);
    }

    public static MetaDataTypeConversionModel GetDefaultWorkspace() => DefaultWorkspace.Value;
}
