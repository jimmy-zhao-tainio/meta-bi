using MetaOrchestration.Core;

namespace MetaOrchestration.Tests;

public sealed class MetaOrchestrationSqlObjectIdentityTests
{
    [Fact]
    public void NormalizeKey_PreservesOrdinaryObjectKeyShape()
    {
        Assert.Equal("WAREHOUSEDB.DBO.CUSTOMER", MetaOrchestrationSqlObjectIdentity.NormalizeKey("WarehouseDb.dbo.Customer"));
    }

    [Fact]
    public void NormalizeKey_MakesBracketedAndDoubleQuotedDottedPartsEquivalent()
    {
        var bracketed = MetaOrchestrationSqlObjectIdentity.NormalizeKey("[Reporting.Db].[sales].[Order.Detail]");
        var doubleQuoted = MetaOrchestrationSqlObjectIdentity.NormalizeKey("\"Reporting.Db\".\"sales\".\"Order.Detail\"");

        Assert.Equal("[REPORTING.DB].SALES.[ORDER.DETAIL]", bracketed);
        Assert.Equal(bracketed, doubleQuoted);
    }

    [Fact]
    public void NormalizeKey_PreservesPartBoundariesAcrossThreeAndFourPartNames()
    {
        var threePart = MetaOrchestrationSqlObjectIdentity.NormalizeKey("[Server.Db].sales.[Order.Detail]");
        var fourPart = MetaOrchestrationSqlObjectIdentity.NormalizeKey("Server.Db.sales.[Order.Detail]");

        Assert.Equal("[SERVER.DB].SALES.[ORDER.DETAIL]", threePart);
        Assert.Equal("SERVER.DB.SALES.[ORDER.DETAIL]", fourPart);
        Assert.NotEqual(threePart, fourPart);
    }

    [Fact]
    public void NormalizeKey_DecodesEscapedQuoteDelimiters()
    {
        var bracketed = MetaOrchestrationSqlObjectIdentity.NormalizeKey("[A]]B].[C.D]");
        var doubleQuoted = MetaOrchestrationSqlObjectIdentity.NormalizeKey("\"A]B\".\"C.D\"");

        Assert.Equal("[A]]B].[C.D]", bracketed);
        Assert.Equal(bracketed, doubleQuoted);
    }

    [Fact]
    public void NormalizeKey_PreservesWhitespaceInsideQuotedParts()
    {
        Assert.Equal("[ SALES ].ORDER", MetaOrchestrationSqlObjectIdentity.NormalizeKey("[ sales ].[Order]"));
    }

    [Theory]
    [InlineData(".")]
    [InlineData("dbo..Customer")]
    [InlineData("[dbo].[Customer")]
    [InlineData("\"dbo\".\"Customer")]
    [InlineData("[dbo]suffix.Customer")]
    [InlineData("Server.Database.Schema.Owner.Customer")]
    public void NormalizeKey_RejectsMalformedOrUnsupportedObjectNames(string sqlIdentifier)
    {
        var error = Assert.Throws<InvalidOperationException>(() => MetaOrchestrationSqlObjectIdentity.NormalizeKey(sqlIdentifier));

        Assert.Contains("SQL object identifier", error.Message, StringComparison.Ordinal);
    }
}
