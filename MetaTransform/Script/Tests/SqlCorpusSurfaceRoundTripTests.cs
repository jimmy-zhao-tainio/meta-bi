public sealed class SqlCorpusSurfaceRoundTripTests
{
    [Theory]
    [InlineData("020_xml_namespaces_and_methods.sql")]
    [InlineData("023_table_sample.sql")]
    [InlineData("036_sequence_and_globals.sql")]
    [InlineData("040_view_column_list.sql")]
    [InlineData("041_xml_namespaces_default.sql")]
    [InlineData("049_data_type_variants.sql")]
    [InlineData("050_remaining_sanctioned_sqlserver_types.sql")]
    [InlineData("051_cross_database_names.sql")]
    [InlineData("055_xml_nodes.sql")]
    [InlineData("028_fulltext_table.sql")]
    [InlineData("062_freetext_table.sql")]
    [InlineData("064_remaining_data_types.sql")]
    [InlineData("066_inline_tvf.sql")]
    public void ParserAndEmitter_RoundTripSurfaceCorpus(string fileName)
    {
        MetaTransformScriptTestHelper.AssertParserRoundTripsCorpusFile(fileName);
    }
}
