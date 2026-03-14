namespace MetaSql.Core;

public sealed record SqlServerPreflightPlan(
    int DesiredTableCount,
    int LiveTableCount,
    IReadOnlyList<string> AddTables,
    IReadOnlyList<string> AddColumns,
    IReadOnlyList<string> AddIndexes,
    IReadOnlyList<string> AddConstraints,
    IReadOnlyList<string> DropTables,
    IReadOnlyList<PlanNote> ManualRequiredItems,
    IReadOnlyList<PlanNote> BlockedItems);

internal sealed class SqlServerPreflightPlanBuilder
{
    private readonly List<string> _addTables = [];
    private readonly List<string> _addColumns = [];
    private readonly List<string> _addIndexes = [];
    private readonly List<string> _addConstraints = [];
    private readonly List<string> _dropTables = [];
    private readonly List<PlanNote> _manualRequired = [];
    private readonly List<PlanNote> _blocked = [];

    public int DesiredTableCount { get; set; }
    public int LiveTableCount { get; set; }

    public void AddTable(string sql) => _addTables.Add(sql);
    public void AddColumn(string sql) => _addColumns.Add(sql);
    public void AddIndex(string sql) => _addIndexes.Add(sql);
    public void AddConstraint(string sql) => _addConstraints.Add(sql);
    public void DropTable(string sql) => _dropTables.Add(sql);
    public void ManualRequired(string objectName, params string[] reasons) => _manualRequired.Add(new PlanNote(objectName, reasons));
    public void Blocked(string objectName, params string[] reasons) => _blocked.Add(new PlanNote(objectName, reasons));

    public SqlServerPreflightPlan Build()
    {
        return new SqlServerPreflightPlan(
            DesiredTableCount,
            LiveTableCount,
            _addTables.ToArray(),
            _addColumns.ToArray(),
            _addIndexes.ToArray(),
            _addConstraints.ToArray(),
            _dropTables.ToArray(),
            _manualRequired.ToArray(),
            _blocked.ToArray());
    }
}

public sealed record PlanNote(string ObjectName, IReadOnlyList<string> Reasons);
