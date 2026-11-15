using Meta.Integration;
using Meta.Operations.Domain;

namespace MetaPipeline;

public static class MetaPipelineModels
{
    public static GenericModel LoadContract() => TypedWorkspaceModelMapper.LoadStateAsync(
        Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaPipeline"))
        .GetAwaiter().GetResult().Model;
}
