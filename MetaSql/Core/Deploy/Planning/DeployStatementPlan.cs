namespace MetaSql;

internal interface IDeployStatementAction;

internal sealed record DropForeignKeyAction(ForeignKey ForeignKey) : IDeployStatementAction;
internal sealed record DropIndexAction(Index Index) : IDeployStatementAction;
internal sealed record DropPrimaryKeyAction(PrimaryKey PrimaryKey) : IDeployStatementAction;
internal sealed record DropTableColumnAction(TableColumn Column) : IDeployStatementAction;
internal sealed record DropTableAction(Table Table) : IDeployStatementAction;
internal sealed record TruncateTableColumnDataAction(
    TableColumn SourceColumn,
    TableColumn LiveColumn,
    IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> SourceDetailsByColumnId) : IDeployStatementAction;
internal sealed record AlterTableColumnAction(
    TableColumn SourceColumn,
    TableColumn LiveColumn,
    IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> SourceDetailsByColumnId,
    IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> LiveDetailsByColumnId) : IDeployStatementAction;
internal sealed record AddTableAction(
    Table Table,
    IReadOnlyList<TableColumn> Columns,
    IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> SourceDetailsByColumnId) : IDeployStatementAction;
internal sealed record AddTableColumnAction(
    TableColumn Column,
    IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> SourceDetailsByColumnId) : IDeployStatementAction;
internal sealed record AddPrimaryKeyAction(PrimaryKey PrimaryKey, IReadOnlyList<PrimaryKeyColumn> Members) : IDeployStatementAction;
internal sealed record AddForeignKeyAction(ForeignKey ForeignKey, IReadOnlyList<ForeignKeyColumn> Members) : IDeployStatementAction;
internal sealed record AddIndexAction(Index Index, IReadOnlyList<IndexColumn> Members) : IDeployStatementAction;

/// <summary>
/// Abstract deploy plan generated from manifest input before SQL rendering/execution.
/// </summary>
internal sealed record DeployStatementPlan
{
    public required MetaSqlDeployManifest.MetaSqlDeployManifestModel ManifestModel { get; init; }
    public required MetaSqlModel SourceModel { get; init; }
    public required MetaSqlModel LiveModel { get; init; }
    public required int AppliedAddCount { get; init; }
    public required int AppliedDropCount { get; init; }
    public required int AppliedAlterCount { get; init; }
    public required int AppliedTruncateCount { get; init; }
    public required int AppliedReplaceCount { get; init; }
    public required IReadOnlyList<IDeployStatementAction> Actions { get; init; }
}
