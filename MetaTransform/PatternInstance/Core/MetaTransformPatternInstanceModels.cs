using Meta.Integration;
using Meta.Operations.Domain;

namespace MetaTransformPatternInstance.Core;

public static class MetaTransformPatternInstanceModels
{
    public static GenericModel LoadContract() => TypedWorkspaceModelMapper.LoadStateAsync(
        Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaTransformPatternInstance"))
        .GetAwaiter().GetResult().Model;
}
