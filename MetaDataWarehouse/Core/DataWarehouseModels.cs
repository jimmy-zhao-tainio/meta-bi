using Meta.Integration;
using Meta.Operations.Domain;

namespace MetaDataWarehouse.Core;

public static class DataWarehouseModels
{
    public static GenericModel LoadContract() => TypedWorkspaceModelMapper.LoadStateAsync(
        Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaDataWarehouse"))
        .GetAwaiter().GetResult().Model;
}
