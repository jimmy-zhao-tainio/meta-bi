namespace MetaPipeline.Tests;

public sealed class SqlServerMultipartIdentifierTests
{
    [Theory]
    [InlineData("dbo.Customer", "[dbo].[Customer]")]
    [InlineData("[warehouse].[Customer Load]", "[warehouse].[Customer Load]")]
    [InlineData("Warehouse.dbo.Customer", "[Warehouse].[dbo].[Customer]")]
    [InlineData("[Reporting.Db].[sales].[Order.Detail]", "[Reporting.Db].[sales].[Order.Detail]")]
    [InlineData("\"Reporting.Db\".\"sales\".\"Order.Detail\"", "[Reporting.Db].[sales].[Order.Detail]")]
    [InlineData("[A]]B].[C.D]", "[A]]B].[C.D]")]
    public void Parse_RendersBracketQuotedIdentifier(string value, string expected)
    {
        var result = SqlServerMultipartIdentifier.Parse(value);

        Assert.Equal(expected, result.RenderBracketQuoted());
    }

    [Theory]
    [InlineData(".")]
    [InlineData("dbo..Customer")]
    [InlineData("[dbo].[Customer")]
    [InlineData("[dbo]suffix.Customer")]
    [InlineData("Server.Database.Schema.Customer")]
    public void Parse_RejectsMalformedOrUnsupportedIdentifier(string value)
    {
        var exception = Assert.Throws<MetaPipelineConfigurationException>(() =>
            SqlServerMultipartIdentifier.Parse(value));

        Assert.Contains("SQL identifier", exception.Message, StringComparison.Ordinal);
    }
}
