using Meta.Integration;
using Meta.Operations.Domain;

namespace MetaDataVault.Core;

public static class RawDataVaultModels
{
    public static GenericModel LoadContract() => TypedWorkspaceModelMapper.LoadStateAsync(
        Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaRawDataVault"))
        .GetAwaiter().GetResult().Model;
}
