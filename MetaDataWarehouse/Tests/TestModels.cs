using Meta.Surfaces.Xml;
using MetaBi.Tests.Common;

namespace MetaDataWarehouse.Tests;

internal static class TestModels
{
    public static MetaDataWarehouseModel LoadSampleSales()
    {
        var workspacePath = Path.Combine(
            CliTestRunner.FindRepositoryRoot(),
            "MetaDataWarehouse",
            "Workspaces",
            "SampleDataWarehouseCommerce");
        return TypedWorkspaceXmlSerializer.Load<MetaDataWarehouseModel>(workspacePath, searchUpward: false);
    }
}
