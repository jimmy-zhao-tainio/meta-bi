using Meta.Integration;
using Meta.Operations.Domain;

namespace MetaDataType.Core;

public static class MetaDataTypeWorkspaceProvider
{
    public static MetaDataTypeModel LoadOrDefault(string? workspacePath)
    {
        var path = string.IsNullOrWhiteSpace(workspacePath)
            ? DefaultWorkspacePath
            : Path.GetFullPath(workspacePath);
        return TypedWorkspaceModelMapper.Load(path, new MetaDataTypeWorkspaceBinding());
    }

    public static MetaDataTypeModel GetDefaultWorkspace() => LoadOrDefault(null);

    public static GenericModel LoadContract() =>
        LoadDefaultStateAsync().GetAwaiter().GetResult().Model;

    public static Task<InMemoryWorkspace> LoadDefaultStateAsync(CancellationToken cancellationToken = default) =>
        TypedWorkspaceModelMapper.LoadStateAsync(DefaultWorkspacePath, cancellationToken);

    private static string DefaultWorkspacePath =>
        Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaDataType");
}
