using Meta.Integration;
using Meta.Operations.Domain;

namespace MetaTransformPattern.Core;

public static class MetaTransformPatternModels
{
    public static GenericModel LoadContract() => TypedWorkspaceModelMapper.LoadStateAsync(
        Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaTransformPattern"))
        .GetAwaiter().GetResult().Model;
}
