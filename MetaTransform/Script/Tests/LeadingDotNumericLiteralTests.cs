using Meta.Integration;
using Meta.Operations.Domain;
using MetaTransformScript.Sql;
using MetaTransformScript.Sql.Parsing;

public sealed class LeadingDotNumericLiteralTests
{
    [Fact]
    public void ParserAndEmitter_RoundTripLeadingDotNumericLiteral_WithExactStructure()
    {
        const string sql = "SELECT .5 AS Value";

        var parser = new MetaTransformScriptSqlParser();
        var service = new MetaTransformScriptSqlService();
        var first = parser.ParseSqlCode(sql, bareSelectName: "dbo.v_leading_dot");
        var emitted = service.ExportToSqlCode(first);
        var second = parser.ParseSqlCode(emitted, bareSelectName: "dbo.v_leading_dot");

        Assert.Equal(".5", Assert.Single(first.LiteralList).Value);
        Assert.Single(first.NumericLiteralList);
        Assert.Contains(".5 AS Value", emitted, StringComparison.Ordinal);
        Assert.Null(InMemoryWorkspaceComparer.FindDifference(
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(first),
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(second)));
        MetaTransformScriptTestHelper.AssertMetaSqlProjectionEqual(first, second);
    }

    [Theory]
    [InlineData(".5E2", "Real")]
    [InlineData(".5e-2", "Real")]
    [InlineData("-.5", "Numeric")]
    [InlineData("+.5", "Numeric")]
    public void ParserAndEmitter_RoundTripSupportedLeadingDotForms(
        string literalExpression,
        string expectedLiteralType)
    {
        var sql = $"SELECT {literalExpression} AS Value";
        var parser = new MetaTransformScriptSqlParser();
        var service = new MetaTransformScriptSqlService();

        var first = parser.ParseSqlCode(sql, bareSelectName: "dbo.v_leading_dot_forms");
        var emitted = service.ExportToSqlCode(first);
        var second = parser.ParseSqlCode(emitted, bareSelectName: "dbo.v_leading_dot_forms");

        var literal = Assert.Single(first.LiteralList);
        Assert.Equal(literalExpression.TrimStart('-', '+'), literal.Value);
        Assert.Equal(expectedLiteralType, literal.LiteralType);
        Assert.Contains(literalExpression + " AS Value", emitted, StringComparison.Ordinal);
        Assert.Null(InMemoryWorkspaceComparer.FindDifference(
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(first),
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(second)));
    }

    [Fact]
    public void ParserAndEmitter_KeepDotAsMultipartIdentifierPunctuation()
    {
        const string sql = "SELECT schema.table AS Value FROM database.schema.table AS source";
        var parser = new MetaTransformScriptSqlParser();
        var service = new MetaTransformScriptSqlService();

        var first = parser.ParseSqlCode(sql, bareSelectName: "dbo.v_multipart_identifiers");
        var emitted = service.ExportToSqlCode(first);
        var second = parser.ParseSqlCode(emitted, bareSelectName: "dbo.v_multipart_identifiers");

        Assert.Contains("schema.table AS Value", emitted, StringComparison.Ordinal);
        Assert.Contains("database.schema.table AS source", emitted, StringComparison.Ordinal);
        Assert.Empty(first.LiteralList);
        Assert.Null(InMemoryWorkspaceComparer.FindDifference(
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(first),
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(second)));
    }

    [Theory]
    [InlineData("SELECT .", "Expected a scalar expression")]
    [InlineData("SELECT .E2", "Expected a scalar expression")]
    [InlineData("SELECT ..5", "Expected a scalar expression")]
    [InlineData("SELECT .5E", "Expected decimal digits after the exponent")]
    [InlineData("SELECT .5E+", "Expected decimal digits after the exponent")]
    public void Parser_RejectsLoneDotAndMalformedLeadingDotFormsClearly(
        string sql,
        string expectedMessage)
    {
        var exception = Assert.Throws<MetaTransformScriptSqlParserException>(
            () => new MetaTransformScriptSqlParser().ParseSqlCode(
                sql,
                bareSelectName: "dbo.v_invalid_leading_dot"));

        Assert.Equal(MetaTransformScriptSqlParserFailureKind.ParseError, exception.FailureKind);
        Assert.Contains(expectedMessage, exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
