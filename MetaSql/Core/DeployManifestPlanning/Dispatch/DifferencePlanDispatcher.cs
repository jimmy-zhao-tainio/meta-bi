namespace MetaSql;

/// <summary>
/// Dispatches difference planning across per-family planners and returns a manifest delta.
/// </summary>
internal sealed class DifferencePlanDispatcher
{
    private readonly TableDifferencePlanner tableDifferencePlanner;
    private readonly TableColumnDifferencePlanner tableColumnDifferencePlanner;
    private readonly PrimaryKeyDifferencePlanner primaryKeyDifferencePlanner;
    private readonly ForeignKeyDifferencePlanner foreignKeyDifferencePlanner;
    private readonly IndexDifferencePlanner indexDifferencePlanner;
    public DifferencePlanDispatcher(
        TableDifferencePlanner tableDifferencePlanner,
        TableColumnDifferencePlanner tableColumnDifferencePlanner,
        PrimaryKeyDifferencePlanner primaryKeyDifferencePlanner,
        ForeignKeyDifferencePlanner foreignKeyDifferencePlanner,
        IndexDifferencePlanner indexDifferencePlanner)
    {
        this.tableDifferencePlanner = tableDifferencePlanner;
        this.tableColumnDifferencePlanner = tableColumnDifferencePlanner;
        this.primaryKeyDifferencePlanner = primaryKeyDifferencePlanner;
        this.foreignKeyDifferencePlanner = foreignKeyDifferencePlanner;
        this.indexDifferencePlanner = indexDifferencePlanner;
    }

    public ManifestPlanDelta Dispatch(
        ManifestPlanningContext context,
        ManifestPlanningLookupContext lookup,
        ManifestPlanDelta delta)
    {
        foreach (var difference in context.Differences
                     .OrderBy(row => row.ObjectKind)
                     .ThenBy(row => row.DifferenceKind)
                     .ThenBy(row => row.ScopeDisplayName, StringComparer.Ordinal)
                     .ThenBy(row => row.DisplayName, StringComparer.Ordinal))
        {
            switch (difference.ObjectKind)
            {
                case MetaSqlObjectKind.Table:
                    tableDifferencePlanner.Plan(lookup, delta, difference);
                    break;
                case MetaSqlObjectKind.TableColumn:
                    tableColumnDifferencePlanner.Plan(lookup, delta, difference);
                    break;
                case MetaSqlObjectKind.PrimaryKey:
                    primaryKeyDifferencePlanner.Plan(lookup, delta, difference);
                    break;
                case MetaSqlObjectKind.ForeignKey:
                    foreignKeyDifferencePlanner.Plan(lookup, delta, difference);
                    break;
                case MetaSqlObjectKind.Index:
                    indexDifferencePlanner.Plan(lookup, delta, difference);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported object kind '{difference.ObjectKind}'.");
            }
        }

        return delta;
    }
}
