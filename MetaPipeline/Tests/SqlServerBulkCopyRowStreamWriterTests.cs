namespace MetaPipeline.Tests;

public sealed class SqlServerBulkCopyRowStreamWriterTests
{
    [Fact]
    public async Task WriteBatch_UsesValuesWithoutConversionWorkspaceOrClrCoercion()
    {
        const string connectionString = "Server=.;Database=tempdb;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var table = "PipelineBulk_" + Guid.NewGuid().ToString("N");
        await using var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
        await connection.OpenAsync();
        await using (var create = connection.CreateCommand())
        {
            create.CommandText = $"CREATE TABLE dbo.[{table}] (Number int NOT NULL, TextValue nvarchar(25) NULL)";
            await create.ExecuteNonQueryAsync();
        }
        try
        {
            var shape = new PipelineRowStreamShape([new PipelineColumn("Number", 1), new PipelineColumn("TextValue", 2)]);
            await using var writer = new SqlServerBulkCopyRowStreamWriter(connectionString, $"dbo.{table}", shape);
            await writer.WriteBatchAsync(new PipelineDataBatch(shape, [new object?[] { 42, "hello" }, new object?[] { 7, null }]));
            await using var verify = connection.CreateCommand();
            verify.CommandText = $"SELECT COUNT(*) FROM dbo.[{table}] WHERE (Number = 42 AND TextValue = N'hello') OR (Number = 7 AND TextValue IS NULL)";
            Assert.Equal(2, (int)(await verify.ExecuteScalarAsync())!);
        }
        finally
        {
            await using var drop = connection.CreateCommand();
            drop.CommandText = $"DROP TABLE dbo.[{table}]";
            await drop.ExecuteNonQueryAsync();
        }
    }

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
