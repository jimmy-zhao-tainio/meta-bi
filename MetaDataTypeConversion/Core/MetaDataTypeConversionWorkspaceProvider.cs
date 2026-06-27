using MetaDataTypeConversion;
using MetaDataTypeConversion.Instance;

namespace MetaDataTypeConversion.Core;

public static class MetaDataTypeConversionWorkspaceProvider
{
    private static readonly Lazy<MetaDataTypeConversionModel> DefaultWorkspace = new(
        () => MetaDataTypeConversionInstance.Default,
        LazyThreadSafetyMode.ExecutionAndPublication);

    public static MetaDataTypeConversionModel LoadOrDefault(string? workspacePath)
    {
        if (string.IsNullOrWhiteSpace(workspacePath))
        {
            return DefaultWorkspace.Value;
        }

        return MetaDataTypeConversionModel.LoadFromXmlWorkspace(Path.GetFullPath(workspacePath), searchUpward: false);
    }

    public static MetaDataTypeConversionModel GetDefaultWorkspace() => DefaultWorkspace.Value;
}
