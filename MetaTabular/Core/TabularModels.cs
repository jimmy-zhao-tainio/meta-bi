using Meta.Integration;
using Meta.Operations.Domain;

namespace MetaTabular.Core;

public static class TabularModels
{
    public static GenericModel LoadContract() => TypedWorkspaceModelMapper.LoadStateAsync(
        Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaTabular"))
        .GetAwaiter().GetResult().Model;
}
