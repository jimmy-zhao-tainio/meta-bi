using Meta.Integration;
using Meta.Operations.Domain;

namespace MetaOrchestration.Core;

public static class MetaOrchestrationModels
{
    public static GenericModel LoadContract() => TypedWorkspaceModelMapper.LoadStateAsync(
        Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaOrchestration"))
        .GetAwaiter().GetResult().Model;
}
