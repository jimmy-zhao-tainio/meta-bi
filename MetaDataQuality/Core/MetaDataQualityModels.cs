using Meta.Integration;
using Meta.Operations.Domain;

namespace MetaDataQuality.Core;

public static class MetaDataQualityModels
{
    public static GenericModel LoadContract() => TypedWorkspaceModelMapper.LoadStateAsync(
        Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaDataQuality"))
        .GetAwaiter().GetResult().Model;
}
