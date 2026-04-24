namespace MetaPipeline.Tests;

public sealed class SqlServerMultipartIdentifierTests
{
    [Theory]
    [InlineData("dbo.Customer", "[dbo].[Customer]")]
    [InlineData("[warehouse].[Customer Load]", "[warehouse].[Customer Load]")]
    [InlineData("Warehouse.dbo.Customer", "[Warehouse].[dbo].[Customer]")]
    [InlineData("[Reporting.Db].[sales].[Order.Detail]", "[Reporting.Db].[sales].[Order.Detail]")]
    public void Parse_RendersBracketQuotedIdentifier(string value, string expected)
    {
        var result = SqlServerMultipartIdentifier.Parse(value);

        Assert.Equal(expected, result.RenderBracketQuoted());
    }
}
