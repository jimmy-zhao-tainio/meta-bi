using Meta.Core.Domain;

namespace MetaTransform.Binding;

internal static class MetaDataTypeConversionWorkspaceProvider
{
    public static Workspace GetDefaultWorkspace() =>
        MetaDataTypeConversion.Core.MetaDataTypeConversionWorkspaceProvider.GetDefaultWorkspace();

    public static Workspace LoadOrDefault(string? workspacePath) =>
        MetaDataTypeConversion.Core.MetaDataTypeConversionWorkspaceProvider.LoadOrDefault(workspacePath);
}
