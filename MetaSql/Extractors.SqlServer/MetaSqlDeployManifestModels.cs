using Meta.Integration;
using Meta.Operations.Domain;

namespace MetaSqlDeployManifest;

public static class MetaSqlDeployManifestModels
{
    public static GenericModel LoadContract() =>
        TypedWorkspaceModelMapper.LoadStateAsync(Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaSqlDeployManifest"))
            .GetAwaiter().GetResult().Model;
}
