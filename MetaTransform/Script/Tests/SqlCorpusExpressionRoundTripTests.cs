public sealed class SqlCorpusExpressionRoundTripTests
{
    [Theory]
    [InlineData("007_where_predicates.sql")]
    [InlineData("014_value_expressions.sql")]
    [InlineData("015_window_functions.sql")]
    [InlineData("016_named_window.sql")]
    [InlineData("025_distinct_predicate.sql")]
    [InlineData("027_fulltext.sql")]
    [InlineData("061_freetext.sql")]
    [InlineData("029_literals_and_special_calls.sql")]
    [InlineData("030_time_zone_extract.sql")]
    [InlineData("044_window_frame_offsets.sql")]
    [InlineData("046_aggregate_distinct.sql")]
    [InlineData("047_parenthesized_scalar_expressions.sql")]
    [InlineData("052_arithmetic_operators.sql")]
    [InlineData("053_negated_predicates.sql")]
    [InlineData("054_like_escape.sql")]
    [InlineData("056_analytic_window_functions.sql")]
    [InlineData("057_percentile_within_group.sql")]
    [InlineData("059_range_window_frames.sql")]
    [InlineData("060_remaining_analytic_functions.sql")]
    [InlineData("067_backtick_identifiers.sql")]
    public void ParserAndEmitter_RoundTripExpressionCorpus(string fileName)
    {
        MetaTransformScriptTestHelper.AssertParserRoundTripsCorpusFile(fileName);
    }
}
