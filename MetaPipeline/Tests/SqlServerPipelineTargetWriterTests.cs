namespace MetaPipeline.Tests;

public sealed class SqlServerPipelineTargetWriterTests
{
    [Fact]
    public void Constructor_WhenColumnsContainDuplicateNames_FailsBeforeOpeningConnection()
    {
        var exception = Assert.Throws<MetaPipelineConfigurationException>(() =>
            new SqlServerPipelineTargetWriter(
                "Server=.;Database=DoesNotOpen;Integrated Security=true;TrustServerCertificate=true;Encrypt=false",
                "dbo.Target",
                [
                    new PipelineColumn("CustomerId", 1),
                    new PipelineColumn("customerid", 2),
                ]));

        Assert.Contains("duplicate column names", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_WhenColumnNameIsBlank_FailsBeforeOpeningConnection()
    {
        var exception = Assert.Throws<MetaPipelineConfigurationException>(() =>
            new SqlServerPipelineTargetWriter(
                "Server=.;Database=DoesNotOpen;Integrated Security=true;TrustServerCertificate=true;Encrypt=false",
                "dbo.Target",
                [
                    new PipelineColumn(" ", 1),
                ]));

        Assert.Contains("blank name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
