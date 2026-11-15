using Meta.Integration;
using Meta.Operations.Domain;

namespace MetaAnalytics.Core;

public static class AnalyticsModels
{
    public static GenericModel LoadContract() => TypedWorkspaceModelMapper.LoadStateAsync(
        Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaAnalytics"))
        .GetAwaiter().GetResult().Model;
}
