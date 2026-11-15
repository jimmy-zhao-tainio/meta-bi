using Meta.Integration;
using Meta.Operations.Domain;

namespace MetaMultiDimensional.Core;

public static class MultiDimensionalModels
{
    public static GenericModel LoadContract() => TypedWorkspaceModelMapper.LoadStateAsync(
        Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaMultiDimensional"))
        .GetAwaiter().GetResult().Model;
}
