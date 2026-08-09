#nullable enable

namespace MetaTransformScript
{
    public sealed class PivotedTableReferenceAggregateFunctionIdentifierLink
    {
        public string Id { get; set; } = string.Empty;

        public MultiPartIdentifier MultiPartIdentifier { get; set; } = null!;

        public PivotedTableReference PivotedTableReference { get; set; } = null!;

    }
}
