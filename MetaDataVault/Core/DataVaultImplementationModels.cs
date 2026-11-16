using Meta.Integration;
using Meta.Operations.Domain;

namespace MetaDataVault.Core;

public static class DataVaultImplementationModels
{
    public static GenericModel LoadContract() => TypedWorkspaceModelMapper.LoadStateAsync(
        Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaDataVaultImplementation"))
        .GetAwaiter().GetResult().Model;
}
