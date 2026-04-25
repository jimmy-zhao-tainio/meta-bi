namespace MetaPipeline.Tests;

public sealed class PipelineRowStreamShapeTests
{
    [Fact]
    public void Constructor_OrdersColumnsByOrdinal()
    {
        var shape = new PipelineRowStreamShape(
            [
                new PipelineColumn("CustomerName", 2),
                new PipelineColumn("CustomerId", 1),
            ]);

        Assert.Collection(
            shape.Columns,
            column => Assert.Equal("CustomerId", column.Name),
            column => Assert.Equal("CustomerName", column.Name));
    }

    [Fact]
    public void Constructor_WhenColumnNameIsBlank_Fails()
    {
        var exception = Assert.Throws<MetaPipelineConfigurationException>(() =>
            new PipelineRowStreamShape([new PipelineColumn(" ", 1)]));

        Assert.Contains("blank name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_WhenColumnNamesAreDuplicated_Fails()
    {
        var exception = Assert.Throws<MetaPipelineConfigurationException>(() =>
            new PipelineRowStreamShape(
                [
                    new PipelineColumn("CustomerId", 1),
                    new PipelineColumn("customerid", 2),
                ]));

        Assert.Contains("duplicate column names", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_WhenColumnOrdinalIsNegative_Fails()
    {
        var exception = Assert.Throws<MetaPipelineConfigurationException>(() =>
            new PipelineRowStreamShape([new PipelineColumn("CustomerId", -1)]));

        Assert.Contains("invalid column ordinal", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_WhenColumnOrdinalsAreDuplicated_Fails()
    {
        var exception = Assert.Throws<MetaPipelineConfigurationException>(() =>
            new PipelineRowStreamShape(
                [
                    new PipelineColumn("CustomerId", 1),
                    new PipelineColumn("CustomerName", 1),
                ]));

        Assert.Contains("duplicate column ordinals", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void EnsureCompatibleWith_WhenColumnNamesDiffer_Fails()
    {
        var expected = new PipelineRowStreamShape(
            [
                new PipelineColumn("CustomerId", 1),
                new PipelineColumn("CustomerName", 2),
            ]);
        var actual = new PipelineRowStreamShape(
            [
                new PipelineColumn("CustomerId", 1),
                new PipelineColumn("OtherName", 2),
            ]);

        var exception = Assert.Throws<MetaPipelineConfigurationException>(() =>
            expected.EnsureCompatibleWith(actual, "actual shape"));

        Assert.Contains("OtherName", exception.Message, StringComparison.Ordinal);
        Assert.Contains("CustomerName", exception.Message, StringComparison.Ordinal);
    }
}
