using Meta.Integration;
using Meta.Operations.Domain;

namespace MetaDataWarehouse.Core;

public static class DataWarehouseImplementationModels
{
    public static GenericModel LoadContract() => TypedWorkspaceModelMapper.LoadStateAsync(
        Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaDataWarehouseImplementation"))
        .GetAwaiter().GetResult().Model;
}
