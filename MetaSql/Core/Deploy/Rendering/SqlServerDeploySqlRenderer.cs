namespace MetaSql;

/// <summary>
/// Renders SQL Server deploy SQL from ordered abstract deploy plans.
/// </summary>
internal sealed class SqlServerDeploySqlRenderer
{
    private readonly SqlServerSchemaSqlRenderer schemaSqlRenderer = new();
    private readonly SqlServerAlterColumnSqlRenderer alterColumnSqlRenderer = new();
    private readonly SqlServerTruncateColumnDataSqlRenderer truncateColumnDataSqlRenderer = new();
    private readonly SqlServerConstraintSqlRenderer constraintSqlRenderer = new();
    private readonly SqlServerTableSqlRenderer tableSqlRenderer = new();

    public IReadOnlyList<string> Render(DeployStatementPlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);

        var statements = new List<string>(plan.Actions.Count);
        foreach (var action in plan.Actions)
        {
            statements.Add(RenderAction(action));
        }

        return statements;
    }

    private string RenderAction(IDeployStatementAction action)
    {
        return action switch
        {
            DropForeignKeyAction row => constraintSqlRenderer.BuildDropForeignKeySql(row.ForeignKey),
            DropIndexAction row => constraintSqlRenderer.BuildDropIndexSql(row.Index),
            DropPrimaryKeyAction row => constraintSqlRenderer.BuildDropPrimaryKeySql(row.PrimaryKey),
            DropTableColumnAction row => tableSqlRenderer.BuildDropColumnSql(row.Column),
            DropTableAction row => tableSqlRenderer.BuildDropTableSql(row.Table),
            AddSchemaAction row => schemaSqlRenderer.BuildAddSchemaSql(row.Schema),
            TruncateTableColumnDataAction row => truncateColumnDataSqlRenderer.BuildTruncateColumnDataSql(
                row.SourceColumn,
                row.LiveColumn,
                row.SourceDetailsByColumnId),
            AlterTableColumnAction row => alterColumnSqlRenderer.BuildAlterColumnSql(
                row.SourceColumn,
                row.LiveColumn,
                row.SourceDetailsByColumnId,
                row.LiveDetailsByColumnId),
            AddTableAction row => tableSqlRenderer.BuildCreateTableSql(
                row.Table,
                row.Columns,
                row.SourceDetailsByColumnId),
            AddTableColumnAction row => tableSqlRenderer.BuildAddColumnSql(
                row.Column,
                row.SourceDetailsByColumnId),
            AddPrimaryKeyAction row => constraintSqlRenderer.BuildAddPrimaryKeySql(row.PrimaryKey, row.Members),
            AddForeignKeyAction row => constraintSqlRenderer.BuildAddForeignKeySql(row.ForeignKey, row.Members),
            AddIndexAction row => constraintSqlRenderer.BuildAddIndexSql(row.Index, row.Members),
            _ => throw new InvalidOperationException($"Unsupported deploy action type '{action.GetType().Name}'.")
        };
    }
}
