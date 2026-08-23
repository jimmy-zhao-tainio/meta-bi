namespace MetaPipeline;

public sealed class PipelineRowStreamShape
{
    public PipelineRowStreamShape(IReadOnlyList<PipelineColumn> columns)
    {
        ArgumentNullException.ThrowIfNull(columns);

        if (columns.Count == 0)
        {
            throw new MetaPipelineConfigurationException(
                "Pipeline row stream shape requires at least one column.");
        }

        var invalidOrdinalColumn = columns.FirstOrDefault(static item => item.Ordinal < 0);
        if (invalidOrdinalColumn is not null)
        {
            throw new MetaPipelineConfigurationException(
                $"Pipeline row stream shape contains invalid column ordinal '{invalidOrdinalColumn.Ordinal}'.");
        }

        var orderedColumns = columns.OrderBy(static item => item.Ordinal).ToArray();

        var blankColumn = orderedColumns.FirstOrDefault(static item => string.IsNullOrWhiteSpace(item.Name));
        if (blankColumn is not null)
        {
            throw new MetaPipelineConfigurationException(
                "Pipeline row stream shape contains a column with a blank name.");
        }

        var duplicateColumnNames = orderedColumns
            .GroupBy(static item => item.Name, StringComparer.OrdinalIgnoreCase)
            .Where(static group => group.Count() > 1)
            .Select(static group => group.Key)
            .OrderBy(static item => item, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (duplicateColumnNames.Length > 0)
        {
            throw new MetaPipelineConfigurationException(
                $"Pipeline row stream shape contains duplicate column names: {string.Join(", ", duplicateColumnNames)}.");
        }

        var duplicateOrdinals = orderedColumns
            .GroupBy(static item => item.Ordinal)
            .Where(static group => group.Count() > 1)
            .Select(static group => group.Key)
            .ToArray();
        if (duplicateOrdinals.Length > 0)
        {
            throw new MetaPipelineConfigurationException(
                $"Pipeline row stream shape contains duplicate column ordinals: {string.Join(", ", duplicateOrdinals)}.");
        }

        Columns = orderedColumns;
    }

    public IReadOnlyList<PipelineColumn> Columns { get; }

    public int ColumnCount => Columns.Count;

    public void EnsureCompatibleWith(PipelineRowStreamShape other, string otherDescription)
    {
        ArgumentNullException.ThrowIfNull(other);
        ArgumentException.ThrowIfNullOrWhiteSpace(otherDescription);

        if (ColumnCount != other.ColumnCount)
        {
            throw new MetaPipelineConfigurationException(
                $"Pipeline row stream shape has {ColumnCount} columns but {otherDescription} has {other.ColumnCount}.");
        }

        for (var ordinal = 0; ordinal < Columns.Count; ordinal++)
        {
            var expected = Columns[ordinal].Name;
            var actual = other.Columns[ordinal].Name;
            if (!string.Equals(expected, actual, StringComparison.OrdinalIgnoreCase))
            {
                throw new MetaPipelineConfigurationException(
                    $"Pipeline row stream column {ordinal + 1} is '{actual}' in {otherDescription} but expected '{expected}'.");
            }
        }
    }
}
