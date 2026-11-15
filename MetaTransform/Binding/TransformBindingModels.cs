using Meta.Integration;
using Meta.Operations.Domain;

namespace MetaTransform.Binding;

public static class TransformBindingModels
{
    public static GenericModel LoadContract() =>
        TypedWorkspaceModelMapper.LoadStateAsync(
                Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaTransformBinding"))
            .GetAwaiter().GetResult().Model;
}
