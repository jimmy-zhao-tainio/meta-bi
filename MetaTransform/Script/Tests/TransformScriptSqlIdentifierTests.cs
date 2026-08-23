using MetaTransformScript;

public sealed class TransformScriptSqlIdentifierTests
{
    [Theory]
    [InlineData("dbo.Customer", "dbo", "Customer")]
    [InlineData("[Reporting.Db].[sales].[Order.Detail]", "Reporting.Db", "sales", "Order.Detail")]
    [InlineData("\"Reporting.Db\".\"sales\".\"Order.Detail\"", "Reporting.Db", "sales", "Order.Detail")]
    [InlineData("[A]]B].[C.D]", "A]B", "C.D")]
    [InlineData("[ sales ].[Order]", " sales ", "Order")]
    public void TryParseParts_RecognizesMultipartSqlIdentifierSyntax(
        string sqlIdentifier,
        params string[] expectedParts)
    {
        var parsed = TransformScriptSqlIdentifier.TryParseParts(
            sqlIdentifier,
            out var parts,
            out var failureReason);

        Assert.True(parsed, failureReason);
        Assert.Equal(expectedParts, parts);
        Assert.Null(failureReason);
    }

    [Theory]
    [InlineData(".")]
    [InlineData("dbo..Customer")]
    [InlineData("[dbo].[Customer")]
    [InlineData("\"dbo\".\"Customer")]
    [InlineData("[dbo]suffix.Customer")]
    [InlineData("dbo[bad].Customer")]
    public void TryParseParts_RejectsMalformedMultipartSqlIdentifiers(string sqlIdentifier)
    {
        var parsed = TransformScriptSqlIdentifier.TryParseParts(
            sqlIdentifier,
            out var parts,
            out var failureReason);

        Assert.False(parsed);
        Assert.Empty(parts);
        Assert.False(string.IsNullOrWhiteSpace(failureReason));
    }
}
