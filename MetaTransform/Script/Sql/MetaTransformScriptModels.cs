using Meta.Integration;
using Meta.Operations.Domain;

namespace MetaTransformScript.Sql;

public static class MetaTransformScriptModels
{
    public static GenericModel LoadContract() =>
        TypedWorkspaceModelMapper.LoadStateAsync(
                Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaTransformScript"))
            .GetAwaiter().GetResult().Model;
}
