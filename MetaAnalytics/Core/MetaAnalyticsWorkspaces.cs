using Meta.Core.Domain;

namespace MetaAnalytics.Core;

public static class MetaAnalyticsWorkspaces
{
    public static Workspace CreateEmptyMetaAnalyticsWorkspace(string workspaceRootPath)
    {
        return MetaAnalyticsWorkspaceFactory.CreateEmptyWorkspace(
            workspaceRootPath,
            MetaAnalyticsModels.CreateMetaAnalyticsModel());
    }
}
