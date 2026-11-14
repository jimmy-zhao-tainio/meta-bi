using Meta.Integration;
using Meta.Operations.Domain;

namespace MetaDataTypeConversion.Core;

public static class MetaDataTypeConversionWorkspaceProvider
{
    public static MetaDataTypeConversionModel LoadOrDefault(string? workspacePath)
    {
        var path = string.IsNullOrWhiteSpace(workspacePath) ? DefaultWorkspacePath : Path.GetFullPath(workspacePath);
        return TypedWorkspaceModelMapper.Load(path, new MetaDataTypeConversionWorkspaceBinding());
    }

    public static MetaDataTypeConversionModel GetDefaultWorkspace() => LoadOrDefault(null);

    public static Task<InMemoryWorkspace> LoadDefaultStateAsync(CancellationToken cancellationToken = default) =>
        TypedWorkspaceModelMapper.LoadStateAsync(DefaultWorkspacePath, cancellationToken);

    private static string DefaultWorkspacePath =>
        Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaDataTypeConversion");
}
