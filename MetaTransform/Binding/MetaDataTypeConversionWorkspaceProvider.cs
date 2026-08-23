using MetaDataTypeConversion;

namespace MetaTransform.Binding;

internal static class MetaDataTypeConversionWorkspaceProvider
{
    public static MetaDataTypeConversionModel GetDefaultWorkspace() =>
        MetaDataTypeConversion.Core.MetaDataTypeConversionWorkspaceProvider.GetDefaultWorkspace();

    public static MetaDataTypeConversionModel LoadOrDefault(string? workspacePath) =>
        MetaDataTypeConversion.Core.MetaDataTypeConversionWorkspaceProvider.LoadOrDefault(workspacePath);
}
