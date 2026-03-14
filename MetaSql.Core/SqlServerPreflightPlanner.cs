namespace MetaSql.Core;

public sealed class SqlServerPreflightPlanner
{
    public SqlServerPreflightPlan Plan(
        DesiredSqlModel desiredModel,
        LiveDatabaseSnapshot liveSnapshot,
        IReadOnlyDictionary<string, SqlObjectTraits> traitsByObject)
    {
        var plan = new SqlServerPreflightPlanBuilder
        {
            DesiredTableCount = desiredModel.Tables.Count,
            LiveTableCount = liveSnapshot.Tables.Count
        };

        var tableStates = new Dictionary<string, TablePlanState>(StringComparer.OrdinalIgnoreCase);
        foreach (var desiredTable in desiredModel.Tables)
        {
            var objectKey = desiredTable.ObjectKey;
            var traits = GetTraits(objectKey, traitsByObject);
            if (!liveSnapshot.Tables.TryGetValue(objectKey, out var liveTable))
            {
                plan.AddTable(desiredTable.CreateTableSql);
                tableStates[objectKey] = TablePlanState.Create;
                continue;
            }

            var reasons = new List<string>();
            var addColumnSql = new List<string>();
            foreach (var desiredColumn in desiredTable.Columns)
            {
                if (!liveTable.Columns.TryGetValue(desiredColumn.Name, out var liveColumn))
                {
                    addColumnSql.Add(
                        $"ALTER TABLE [{desiredTable.SchemaName}].[{desiredTable.TableName}] ADD [{desiredColumn.Name}] {desiredColumn.DefinitionSql};");
                    continue;
                }

                if (!string.Equals(desiredColumn.TypeSql, liveColumn.TypeSql, StringComparison.OrdinalIgnoreCase) ||
                    desiredColumn.IsNullable != liveColumn.IsNullable)
                {
                    reasons.Add(
                        $"Column [{desiredColumn.Name}] differs. Desired: {desiredColumn.DefinitionSql}. Live: {RenderLiveColumn(liveColumn)}.");
                }
            }

            foreach (var liveColumn in liveTable.Columns.Values.OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase))
            {
                if (!desiredTable.Columns.Any(item => string.Equals(item.Name, liveColumn.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    reasons.Add($"Live column [{liveColumn.Name}] exists but is not present in desired SQL.");
                }
            }

            if (reasons.Count == 0)
            {
                foreach (var sql in addColumnSql)
                {
                    plan.AddColumn(sql);
                }

                tableStates[objectKey] = TablePlanState.Compatible;
                continue;
            }

            if (CanReplace(traits, liveTable.RowCount))
            {
                plan.DropTable($"DROP TABLE [{desiredTable.SchemaName}].[{desiredTable.TableName}];");
                plan.AddTable(desiredTable.CreateTableSql);
                tableStates[objectKey] = TablePlanState.Replace;
                continue;
            }

            if (traits.AutoPolicy == SqlObjectAutoPolicy.AdditivePlusEmptyDrop && liveTable.RowCount > 0)
            {
                reasons.Insert(
                    0,
                    $"Refused automatic DROP/CREATE because [{desiredTable.SchemaName}].[{desiredTable.TableName}] contains {liveTable.RowCount} row(s).");
            }

            plan.ManualRequired(objectKey, reasons.ToArray());
            tableStates[objectKey] = TablePlanState.ManualRequired;
        }

        foreach (var liveTable in liveSnapshot.Tables.Values
                     .OrderBy(item => item.SchemaName, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(item => item.SchemaName, StringComparer.Ordinal)
                     .ThenBy(item => item.TableName, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(item => item.TableName, StringComparer.Ordinal))
        {
            if (tableStates.ContainsKey(liveTable.ObjectKey))
            {
                continue;
            }

            var traits = GetTraits(liveTable.ObjectKey, traitsByObject);
            if (CanReplace(traits, liveTable.RowCount))
            {
                plan.DropTable($"DROP TABLE [{liveTable.SchemaName}].[{liveTable.TableName}];");
                tableStates[liveTable.ObjectKey] = TablePlanState.Replace;
                continue;
            }

            if (traits.AutoPolicy == SqlObjectAutoPolicy.AdditivePlusEmptyDrop && liveTable.RowCount > 0)
            {
                plan.ManualRequired(
                    liveTable.ObjectKey,
                    $"Refused DROP TABLE because [{liveTable.SchemaName}].[{liveTable.TableName}] contains {liveTable.RowCount} row(s).");
            }
            else
            {
                plan.ManualRequired(
                    liveTable.ObjectKey,
                    $"Live table [{liveTable.SchemaName}].[{liveTable.TableName}] exists but is not present in desired SQL.");
            }

            tableStates[liveTable.ObjectKey] = TablePlanState.ManualRequired;
        }

        foreach (var desiredTable in desiredModel.Tables)
        {
            var state = tableStates[desiredTable.ObjectKey];
            var liveTable = liveSnapshot.Tables.TryGetValue(desiredTable.ObjectKey, out var foundTable) ? foundTable : null;
            foreach (var desiredIndex in desiredTable.Indexes)
            {
                if (state == TablePlanState.ManualRequired)
                {
                    plan.Blocked(desiredTable.ObjectKey, $"Waiting on table-level manual work before CREATE INDEX [{desiredIndex.Name}].");
                    continue;
                }

                if (state is TablePlanState.Create or TablePlanState.Replace || liveTable == null || !liveTable.IndexNames.Contains(desiredIndex.Name))
                {
                    plan.AddIndex(desiredIndex.Sql);
                }
            }

            foreach (var desiredConstraint in desiredTable.InlineConstraints.Concat(desiredTable.AlterConstraints))
            {
                if (state == TablePlanState.ManualRequired)
                {
                    plan.Blocked(desiredTable.ObjectKey, $"Waiting on table-level manual work before ADD CONSTRAINT [{desiredConstraint.Name}].");
                    continue;
                }

                if (string.Equals(desiredConstraint.ConstraintKind, "FOREIGN KEY", StringComparison.OrdinalIgnoreCase))
                {
                    var targetKey = desiredConstraint.ReferencedSchemaName != null && desiredConstraint.ReferencedTableName != null
                        ? SqlObjectName.Format(desiredConstraint.ReferencedSchemaName, desiredConstraint.ReferencedTableName)
                        : null;
                    if (targetKey != null &&
                        tableStates.TryGetValue(targetKey, out var targetState) &&
                        targetState == TablePlanState.ManualRequired)
                    {
                        plan.Blocked(desiredTable.ObjectKey, $"Waiting on {targetKey} before ADD CONSTRAINT [{desiredConstraint.Name}].");
                        continue;
                    }

                    if (targetKey != null &&
                        !liveSnapshot.Tables.ContainsKey(targetKey) &&
                        !tableStates.TryGetValue(targetKey, out _))
                    {
                        plan.Blocked(desiredTable.ObjectKey, $"Referenced table {targetKey} is missing.");
                        continue;
                    }
                }

                if (state is TablePlanState.Create or TablePlanState.Replace &&
                    desiredTable.InlineConstraints.Contains(desiredConstraint))
                {
                    continue;
                }

                if (liveTable == null || !liveTable.ConstraintNames.Contains(desiredConstraint.Name) || state is TablePlanState.Create or TablePlanState.Replace)
                {
                    plan.AddConstraint(desiredConstraint.Sql);
                }
            }
        }

        return plan.Build();
    }

    private static SqlObjectTraits GetTraits(string objectKey, IReadOnlyDictionary<string, SqlObjectTraits> traitsByObject)
    {
        return traitsByObject.TryGetValue(objectKey, out var traits) ? traits : SqlObjectTraits.Default;
    }

    private static bool CanReplace(SqlObjectTraits traits, long rowCount)
    {
        return traits.StateClass == SqlObjectStateClass.Replaceable &&
               traits.AutoPolicy == SqlObjectAutoPolicy.AdditivePlusEmptyDrop &&
               rowCount == 0;
    }

    private static string RenderLiveColumn(LiveColumn column)
    {
        return $"{column.TypeSql} {(column.IsNullable ? "NULL" : "NOT NULL")}";
    }

    private enum TablePlanState
    {
        Compatible,
        Create,
        Replace,
        ManualRequired
    }
}
