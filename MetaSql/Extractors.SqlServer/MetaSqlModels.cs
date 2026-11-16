using Meta.Integration;
using Meta.Operations.Domain;

namespace MetaSql;

public static class MetaSqlModels
{
    public static GenericModel LoadContract() =>
        TypedWorkspaceModelMapper.LoadStateAsync(Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaSql"))
            .GetAwaiter().GetResult().Model;
}
