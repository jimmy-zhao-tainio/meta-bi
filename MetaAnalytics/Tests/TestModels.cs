using Meta.Surfaces.Xml;
using MetaBi.Tests.Common;

namespace MetaAnalytics.Tests;

internal static class TestModels
{
    public static MetaAnalyticsModel LoadSampleCommerce()
    {
        var workspacePath = Path.Combine(
            CliTestRunner.FindRepositoryRoot(),
            "MetaAnalytics",
            "Workspaces",
            "SampleAnalyticsCommerce");
        return TypedWorkspaceXmlSerializer.Load<MetaAnalyticsModel>(workspacePath, searchUpward: false);
    }
}
