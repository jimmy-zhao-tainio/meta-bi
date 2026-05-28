namespace MetaPipeline.Tests;

public sealed class SqlServerBulkCopyRowStreamWriterTests
{
    [Fact]
    public async Task Constructor_UsesProvidedShapeWithoutOpeningConnection()
    {
        var shape = new PipelineRowStreamShape(
            [
                new PipelineColumn("CustomerId", 1),
                new PipelineColumn("CustomerName", 2),
            ]);

        var writer = new SqlServerBulkCopyRowStreamWriter(
            "Server=.;Database=DoesNotOpen;Integrated Security=true;TrustServerCertificate=true;Encrypt=false",
            "dbo.Target",
            shape);

        await using (writer)
        {
            Assert.Same(shape, writer.Shape);
        }
    }

    [Fact]
    public void Constructor_WhenTargetIdentifierIsBlank_FailsBeforeOpeningConnection()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new SqlServerBulkCopyRowStreamWriter(
                "Server=.;Database=DoesNotOpen;Integrated Security=true;TrustServerCertificate=true;Encrypt=false",
                " ",
                new PipelineRowStreamShape([new PipelineColumn("CustomerId", 1)])));

        Assert.Contains("targetSqlIdentifier", exception.Message, StringComparison.Ordinal);
    }
}
