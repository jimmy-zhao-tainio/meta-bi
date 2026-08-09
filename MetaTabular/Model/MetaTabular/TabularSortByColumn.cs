#nullable enable

namespace MetaTabular
{
    public sealed class TabularSortByColumn
    {
        public string Id { get; set; } = string.Empty;

        public TabularColumn SortColumn { get; set; } = null!;

        public TabularColumn SourceColumn { get; set; } = null!;

    }
}
