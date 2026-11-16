using Meta.Integration;
using Meta.Operations.Domain;

namespace MetaDataVault.Core;

public static class BusinessDataVaultModels
{
    public static GenericModel LoadContract() => TypedWorkspaceModelMapper.LoadStateAsync(
        Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaBusinessDataVault"))
        .GetAwaiter().GetResult().Model;
}
