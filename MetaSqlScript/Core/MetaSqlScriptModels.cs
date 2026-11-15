using Meta.Integration;
using Meta.Operations.Domain;

namespace MetaSqlScript;

public static class MetaSqlScriptModels
{
    public static GenericModel LoadContract() => TypedWorkspaceModelMapper.LoadStateAsync(
        Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaSqlScript"))
        .GetAwaiter().GetResult().Model;
}
