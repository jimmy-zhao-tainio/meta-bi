using Meta.Operations.Domain;
using Meta.Operations;
using Meta.Integration;
using Meta.Surfaces.CSharp;
using Meta.Surfaces;

namespace MetaAnalytics.Tests;

public sealed class CSharpWorkspaceContractTests
{
    [Fact]
    public void MetaAnalyticsModel_IsProducedAndConsumedByTheCSharpSurfaceContract()
    {
        var sourceState = TypedWorkspaceModelMapper.ToInMemoryWorkspace(
            TestModels.LoadSampleCommerce());

        var csharp = MetaCSharpWriter.Write(sourceState);
        var source = Assert.Single(csharp.Sources.Values);
        Assert.Contains("public sealed partial class MetaAnalyticsModel", source);
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
