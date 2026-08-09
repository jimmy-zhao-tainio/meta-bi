using Meta.Core.Domain;
using Meta.Core.Operations;
using Meta.Core.Serialization;
using Meta.Surfaces;
using MetaAnalytics.Instance;

namespace MetaAnalytics.Tests;

public sealed class CSharpWorkspaceContractTests
{
    [Fact]
    public void MetaAnalyticsModel_IsProducedAndConsumedByTheCSharpSurfaceContract()
    {
        var sourceState = TypedWorkspaceModelMapper.ToInMemoryWorkspace(
            MetaAnalyticsInstance.SampleCommerce);

        var csharp = MetaCSharpWriter.Write(sourceState);
        var source = Assert.Single(csharp.Sources.Values);
        Assert.Contains("public sealed class MetaAnalyticsModel", source);
        Assert.Contains("CreateEmpty()", source);
        Assert.Contains("List<Table> TableList", source);
        Assert.Contains("MetaAnalyticsInstance", source);
        Assert.Contains("CreateBuiltIn()", source);
        Assert.Contains("BuiltIn", source);

        var readState = MetaCSharpReader.Read(csharp);
        var consumed = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            readState,
            static () => new MetaAnalyticsModel());

        Assert.Null(InMemoryWorkspaceComparer.FindDifference(
            sourceState,
            readState));
        Assert.Contains(
            consumed.TableList,
            table => table.Name == "Sales");
        Assert.Contains(
            consumed.MeasureList,
            measure => measure.Name == "Sales Amount");
    }
}
