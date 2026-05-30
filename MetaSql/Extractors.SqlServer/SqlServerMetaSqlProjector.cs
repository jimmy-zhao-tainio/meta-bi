using System.Globalization;
using Meta.Core.Domain;
using Meta.Core.Services;
using MetaDataType.Instance;

namespace MetaSql.Extractors.SqlServer;

internal static class SqlServerMetaSqlProjector
{
    internal static Workspace Project(
        string newWorkspacePath,
        string databaseName,
        IReadOnlyList<TableRow> tableRows,
        IReadOnlyDictionary<string, List<ColumnRow>> columnsByTableKey,
        IReadOnlyDictionary<string, List<PrimaryKeyRow>> primaryKeysByTableKey,
        IReadOnlyDictionary<string, List<PrimaryKeyColumnRow>> primaryKeyColumnsByTableKey,
        IReadOnlyDictionary<string, List<ForeignKeyRow>> foreignKeysByTableKey,
        IReadOnlyDictionary<string, List<ForeignKeyColumnRow>> foreignKeyColumnsByTableKey,
        IReadOnlyDictionary<string, List<IndexRow>> indexesByTableKey,
        IReadOnlyDictionary<string, List<IndexColumnRow>> indexColumnsByTableKey,
        IReadOnlyList<ViewRow>? viewRows = null,
        IReadOnlyList<FunctionRow>? functionRows = null,
        IReadOnlyList<StoredProcedureRow>? storedProcedureRows = null)
    {
        var model = MetaSqlModel.CreateEmpty();
        var database = new Database
        {
            Id = databaseName,
            Name = databaseName,
        };
        model.DatabaseList.Add(database);

        var schemasByName = new Dictionary<string, Schema>(StringComparer.OrdinalIgnoreCase);
        var tablesByScopedKey = new Dictionary<string, Table>(StringComparer.OrdinalIgnoreCase);
        var columnsByScopedTableAndName = new Dictionary<string, TableColumn>(StringComparer.OrdinalIgnoreCase);
        var normalizedViewRows = viewRows ?? Array.Empty<ViewRow>();
        var normalizedFunctionRows = functionRows ?? Array.Empty<FunctionRow>();
        var normalizedStoredProcedureRows = storedProcedureRows ?? Array.Empty<StoredProcedureRow>();

        foreach (var schemaName in normalizedViewRows
                     .Select(row => row.SchemaName)
                     .Concat(normalizedFunctionRows.Select(row => row.SchemaName))
                     .Concat(normalizedStoredProcedureRows.Select(row => row.SchemaName))
                     .OrderBy(row => row, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(row => row, StringComparer.Ordinal))
        {
            GetOrAddSchema(model, database, schemasByName, schemaName);
        }

        foreach (var tableRow in tableRows)
        {
            var schema = GetOrAddSchema(model, database, schemasByName, tableRow.SchemaName);
            var table = AddTable(model, schema, tableRow.TableName);
            var scopedTableKey = BuildScopedObjectKey(tableRow.SchemaName, tableRow.TableName);
            tablesByScopedKey[scopedTableKey] = table;

            foreach (var columnRow in GetGroup(columnsByTableKey, scopedTableKey)
                         .OrderBy(row => row.OrdinalPosition)
                         .ThenBy(row => row.ColumnName, StringComparer.OrdinalIgnoreCase)
                         .ThenBy(row => row.ColumnName, StringComparer.Ordinal))
            {
                var column = AddColumn(model, table, columnRow);
                columnsByScopedTableAndName[BuildScopedObjectKey(scopedTableKey, columnRow.ColumnName)] = column;
            }
        }

        foreach (var tableRow in tableRows)
        {
            var scopedTableKey = BuildScopedObjectKey(tableRow.SchemaName, tableRow.TableName);
            var table = tablesByScopedKey[scopedTableKey];
            var primaryKeysByName = new Dictionary<string, PrimaryKey>(StringComparer.Ordinal);
            foreach (var primaryKeyRow in GetGroup(primaryKeysByTableKey, scopedTableKey)
                         .OrderBy(row => row.Name, StringComparer.Ordinal))
            {
                var primaryKey = AddPrimaryKey(model, table, primaryKeyRow);
                primaryKeysByName[primaryKeyRow.Name] = primaryKey;
            }

            foreach (var primaryKeyColumnRow in GetGroup(primaryKeyColumnsByTableKey, scopedTableKey)
                         .OrderBy(row => row.KeyName, StringComparer.Ordinal)
                         .ThenBy(row => row.Ordinal))
            {
                if (!primaryKeysByName.TryGetValue(primaryKeyColumnRow.KeyName, out var primaryKey))
                {
                    continue;
                }

                if (!columnsByScopedTableAndName.TryGetValue(BuildScopedObjectKey(scopedTableKey, primaryKeyColumnRow.ColumnName), out var column))
                {
                    throw new InvalidOperationException(
                        $"Primary key '{table.Name}.{primaryKey.Name}' referenced missing column '{primaryKeyColumnRow.ColumnName}'.");
                }

                model.PrimaryKeyColumnList.Add(new PrimaryKeyColumn
                {
                    Id = $"{primaryKey.Id}.column.{primaryKeyColumnRow.Ordinal.ToString(CultureInfo.InvariantCulture)}",
                    PrimaryKey = primaryKey,
                    TableColumn = column,
                    Ordinal = primaryKeyColumnRow.Ordinal.ToString(CultureInfo.InvariantCulture),
                    IsDescending = primaryKeyColumnRow.IsDescending ? "true" : string.Empty,
                });
            }
        }

        foreach (var tableRow in tableRows)
        {
            var scopedTableKey = BuildScopedObjectKey(tableRow.SchemaName, tableRow.TableName);
            var table = tablesByScopedKey[scopedTableKey];
            var foreignKeysByName = new Dictionary<string, ForeignKey>(StringComparer.Ordinal);
            foreach (var foreignKeyRow in GetGroup(foreignKeysByTableKey, scopedTableKey)
                         .OrderBy(row => row.Name, StringComparer.Ordinal))
            {
                var targetScopedKey = BuildScopedObjectKey(foreignKeyRow.TargetSchemaName, foreignKeyRow.TargetTableName);
                if (!tablesByScopedKey.TryGetValue(targetScopedKey, out var targetTable))
                {
                    continue;
                }

                var foreignKey = AddForeignKey(model, table, targetTable, foreignKeyRow.Name);
                foreignKeysByName[foreignKeyRow.Name] = foreignKey;
            }

            foreach (var foreignKeyColumnRow in GetGroup(foreignKeyColumnsByTableKey, scopedTableKey)
                         .OrderBy(row => row.ForeignKeyName, StringComparer.Ordinal)
                         .ThenBy(row => row.Ordinal))
            {
                if (!foreignKeysByName.TryGetValue(foreignKeyColumnRow.ForeignKeyName, out var foreignKey))
                {
                    continue;
                }

                var targetScopedKey = BuildScopedObjectKey(foreignKey.TargetTable!.Schema!.Name, foreignKey.TargetTable.Name);
                if (!columnsByScopedTableAndName.TryGetValue(BuildScopedObjectKey(scopedTableKey, foreignKeyColumnRow.SourceColumnName), out var sourceColumn))
                {
                    throw new InvalidOperationException(
                        $"Foreign key '{table.Name}.{foreignKey.Name}' referenced missing source column '{foreignKeyColumnRow.SourceColumnName}'.");
                }

                if (!columnsByScopedTableAndName.TryGetValue(BuildScopedObjectKey(targetScopedKey, foreignKeyColumnRow.TargetColumnName), out var targetColumn))
                {
                    throw new InvalidOperationException(
                        $"Foreign key '{table.Name}.{foreignKey.Name}' referenced missing target column '{foreignKeyColumnRow.TargetColumnName}'.");
                }

                model.ForeignKeyColumnList.Add(new ForeignKeyColumn
                {
                    Id = $"{foreignKey.Id}.column.{foreignKeyColumnRow.Ordinal.ToString(CultureInfo.InvariantCulture)}",
                    ForeignKey = foreignKey,
                    SourceColumn = sourceColumn,
                    TargetColumn = targetColumn,
                    Ordinal = foreignKeyColumnRow.Ordinal.ToString(CultureInfo.InvariantCulture),
                });
            }
        }

        foreach (var tableRow in tableRows)
        {
            var scopedTableKey = BuildScopedObjectKey(tableRow.SchemaName, tableRow.TableName);
            var table = tablesByScopedKey[scopedTableKey];
            var indexesByName = new Dictionary<string, Index>(StringComparer.Ordinal);
            foreach (var indexRow in GetGroup(indexesByTableKey, scopedTableKey)
                         .OrderBy(row => row.Name, StringComparer.Ordinal))
            {
                var index = AddIndex(model, table, indexRow);
                indexesByName[indexRow.Name] = index;
            }

            foreach (var indexColumnRow in GetGroup(indexColumnsByTableKey, scopedTableKey)
                         .OrderBy(row => row.IndexName, StringComparer.Ordinal)
                         .ThenBy(row => row.Ordinal))
            {
                if (!indexesByName.TryGetValue(indexColumnRow.IndexName, out var index))
                {
                    continue;
                }

                if (!columnsByScopedTableAndName.TryGetValue(BuildScopedObjectKey(scopedTableKey, indexColumnRow.ColumnName), out var column))
                {
                    throw new InvalidOperationException(
                        $"Index '{table.Name}.{index.Name}' referenced missing column '{indexColumnRow.ColumnName}'.");
                }

                model.IndexColumnList.Add(new IndexColumn
                {
                    Id = $"{index.Id}.column.{indexColumnRow.Ordinal.ToString(CultureInfo.InvariantCulture)}",
                    Index = index,
                    TableColumn = column,
                    Ordinal = indexColumnRow.Ordinal.ToString(CultureInfo.InvariantCulture),
                    IsDescending = indexColumnRow.IsDescending ? "true" : string.Empty,
                    IsIncluded = indexColumnRow.IsIncluded ? "true" : string.Empty,
                });
            }
        }

        foreach (var viewRow in normalizedViewRows
                     .OrderBy(row => row.DeployOrdinal ?? int.MaxValue)
                     .ThenBy(row => row.SchemaName, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(row => row.ViewName, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(row => row.ViewName, StringComparer.Ordinal))
        {
            var schema = GetOrAddSchema(model, database, schemasByName, viewRow.SchemaName);
            AddView(model, schema, viewRow);
        }

        foreach (var functionRow in normalizedFunctionRows
                     .OrderBy(row => row.DeployOrdinal ?? int.MaxValue)
                     .ThenBy(row => row.SchemaName, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(row => row.FunctionName, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(row => row.FunctionName, StringComparer.Ordinal))
        {
            var schema = GetOrAddSchema(model, database, schemasByName, functionRow.SchemaName);
            AddFunction(model, schema, functionRow);
        }

        foreach (var storedProcedureRow in normalizedStoredProcedureRows
                     .OrderBy(row => row.DeployOrdinal ?? int.MaxValue)
                     .ThenBy(row => row.SchemaName, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(row => row.ProcedureName, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(row => row.ProcedureName, StringComparer.Ordinal))
        {
            var schema = GetOrAddSchema(model, database, schemasByName, storedProcedureRow.SchemaName);
            AddStoredProcedure(model, schema, storedProcedureRow);
        }

        model.SaveToXmlWorkspace(newWorkspacePath);
        return new WorkspaceService().LoadAsync(newWorkspacePath, searchUpward: false).GetAwaiter().GetResult();
    }

    internal static string BuildScopedObjectKey(string part1, string part2) => part1 + "." + part2;

    private static Schema GetOrAddSchema(MetaSqlModel model, Database database, IDictionary<string, Schema> schemasByName, string schemaName)
    {
        if (schemasByName.TryGetValue(schemaName, out var existing))
        {
            return existing;
        }

        var schema = new Schema
        {
            Id = $"{database.Id}.{schemaName}",
            Name = schemaName,
            Database = database,
        };
        model.SchemaList.Add(schema);
        schemasByName[schemaName] = schema;
        return schema;
    }

    private static Table AddTable(MetaSqlModel model, Schema schema, string tableName)
    {
        var table = new Table
        {
            Id = $"{schema.Id}.{tableName}",
            Name = tableName,
            Schema = schema,
        };
        model.TableList.Add(table);
        return table;
    }

    private static TableColumn AddColumn(MetaSqlModel model, Table table, ColumnRow row)
    {
        var column = new TableColumn
        {
            Id = $"{table.Id}.{row.ColumnName}",
            Name = row.ColumnName,
            Ordinal = row.OrdinalPosition.ToString(CultureInfo.InvariantCulture),
            MetaDataTypeId = MetaDataTypeInstance.BuildDataTypeId("SqlServer", row.DataTypeName),
            IsNullable = row.IsNullable ? "true" : "false",
            IsIdentity = row.IsIdentity ? "true" : string.Empty,
            IdentitySeed = string.IsNullOrWhiteSpace(row.IdentitySeed) ? string.Empty : row.IdentitySeed,
            IdentityIncrement = string.IsNullOrWhiteSpace(row.IdentityIncrement) ? string.Empty : row.IdentityIncrement,
            ExpressionSql = string.IsNullOrWhiteSpace(row.ExpressionSql) ? string.Empty : row.ExpressionSql,
            DefaultExpressionSql = NormalizeDefaultExpressionSql(row.DefaultExpressionSql),
            Table = table,
        };
        model.TableColumnList.Add(column);
        AddTypeDetails(model, column, row);
        return column;
    }

    private static void AddTypeDetails(MetaSqlModel model, TableColumn column, ColumnRow row)
    {
        switch (row.DataTypeName.ToLowerInvariant())
        {
            case "char":
            case "nchar":
            case "varchar":
            case "nvarchar":
            case "binary":
            case "varbinary":
                AddDetail(model, column, "Length", row.Length);
                break;

            case "decimal":
            case "numeric":
                AddDetail(model, column, "Precision", row.Precision);
                AddDetail(model, column, "Scale", row.Scale);
                break;

            case "time":
            case "datetime2":
            case "datetimeoffset":
                AddDetail(model, column, "Precision", row.Precision);
                break;
        }
    }

    private static void AddDetail(MetaSqlModel model, TableColumn column, string name, int? value)
    {
        if (!value.HasValue)
        {
            return;
        }

        model.TableColumnDataTypeDetailList.Add(new TableColumnDataTypeDetail
        {
            Id = $"{column.Id}.detail.{name}",
            Name = name,
            Value = value.Value.ToString(CultureInfo.InvariantCulture),
            TableColumn = column,
        });
    }

    private static string NormalizeDefaultExpressionSql(string expressionSql)
    {
        if (string.IsNullOrWhiteSpace(expressionSql))
        {
            return string.Empty;
        }

        var current = expressionSql.Trim();
        while (HasSingleOuterParenthesisPair(current))
        {
            current = current[1..^1].Trim();
        }

        return current;
    }

    private static bool HasSingleOuterParenthesisPair(string value)
    {
        if (value.Length < 2 || value[0] != '(' || value[^1] != ')')
        {
            return false;
        }

        var depth = 0;
        for (var i = 0; i < value.Length; i++)
        {
            var ch = value[i];
            if (ch == '(')
            {
                depth++;
            }
            else if (ch == ')')
            {
                depth--;
                if (depth == 0 && i < value.Length - 1)
                {
                    return false;
                }
            }

            if (depth < 0)
            {
                return false;
            }
        }

        return depth == 0;
    }

    private static PrimaryKey AddPrimaryKey(MetaSqlModel model, Table table, PrimaryKeyRow row)
    {
        var primaryKey = new PrimaryKey
        {
            Id = $"{table.Id}.pk.{row.Name}",
            Name = row.Name,
            IsClustered = row.IsClustered ? "true" : string.Empty,
            Table = table,
        };
        model.PrimaryKeyList.Add(primaryKey);
        return primaryKey;
    }

    private static ForeignKey AddForeignKey(MetaSqlModel model, Table sourceTable, Table targetTable, string name)
    {
        var foreignKey = new ForeignKey
        {
            Id = $"{sourceTable.Id}.fk.{name}",
            Name = name,
            SourceTable = sourceTable,
            TargetTable = targetTable,
        };
        model.ForeignKeyList.Add(foreignKey);
        return foreignKey;
    }

    private static Index AddIndex(MetaSqlModel model, Table table, IndexRow row)
    {
        var index = new Index
        {
            Id = $"{table.Id}.index.{row.Name}",
            Name = row.Name,
            IsUnique = row.IsUnique ? "true" : string.Empty,
            IsClustered = row.IsClustered ? "true" : string.Empty,
            FilterSql = string.IsNullOrWhiteSpace(row.FilterSql) ? string.Empty : row.FilterSql,
            Table = table,
        };
        model.IndexList.Add(index);
        return index;
    }

    private static View AddView(MetaSqlModel model, Schema schema, ViewRow row)
    {
        var view = new View
        {
            Id = $"{schema.Id}.view.{row.ViewName}",
            Name = row.ViewName,
            DefinitionSql = row.DefinitionSql,
            DeployOrdinal = row.DeployOrdinal?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            Schema = schema,
        };
        model.ViewList.Add(view);
        return view;
    }

    private static Function AddFunction(MetaSqlModel model, Schema schema, FunctionRow row)
    {
        var function = new Function
        {
            Id = $"{schema.Id}.function.{row.FunctionName}",
            Name = row.FunctionName,
            FunctionKind = row.FunctionKind,
            DefinitionSql = row.DefinitionSql,
            DeployOrdinal = row.DeployOrdinal?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            Schema = schema,
        };
        model.FunctionList.Add(function);
        return function;
    }

    private static StoredProcedure AddStoredProcedure(MetaSqlModel model, Schema schema, StoredProcedureRow row)
    {
        var storedProcedure = new StoredProcedure
        {
            Id = $"{schema.Id}.procedure.{row.ProcedureName}",
            Name = row.ProcedureName,
            DefinitionSql = row.DefinitionSql,
            DeployOrdinal = row.DeployOrdinal?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            Schema = schema,
        };
        model.StoredProcedureList.Add(storedProcedure);
        return storedProcedure;
    }

    private static IReadOnlyList<T> GetGroup<T>(IReadOnlyDictionary<string, List<T>> groups, string key)
    {
        return groups.TryGetValue(key, out var bucket)
            ? bucket
            : Array.Empty<T>();
    }

    internal readonly record struct TableRow(string SchemaName, string TableName);
    internal readonly record struct ColumnRow(
        string SchemaName,
        string TableName,
        string ColumnName,
        int OrdinalPosition,
        bool IsNullable,
        string DataTypeName,
        int? Length,
        int? Precision,
        int? Scale,
        bool IsIdentity = false,
        string IdentitySeed = "",
        string IdentityIncrement = "",
        string ExpressionSql = "",
        string DefaultExpressionSql = "");
    internal readonly record struct PrimaryKeyRow(string Name, bool IsClustered);
    internal readonly record struct PrimaryKeyColumnRow(string KeyName, int Ordinal, string ColumnName, bool IsDescending);
    internal readonly record struct ForeignKeyRow(string Name, string TargetSchemaName, string TargetTableName);
    internal readonly record struct ForeignKeyColumnRow(string ForeignKeyName, int Ordinal, string SourceColumnName, string TargetColumnName);
    internal readonly record struct IndexRow(string Name, bool IsUnique, bool IsClustered, string FilterSql = "");
    internal readonly record struct IndexColumnRow(string IndexName, int Ordinal, string ColumnName, bool IsDescending, bool IsIncluded);
    internal readonly record struct ViewRow(string SchemaName, string ViewName, string DefinitionSql, int? DeployOrdinal = null);
    internal readonly record struct FunctionRow(string SchemaName, string FunctionName, string FunctionKind, string DefinitionSql, int? DeployOrdinal = null);
    internal readonly record struct StoredProcedureRow(string SchemaName, string ProcedureName, string DefinitionSql, int? DeployOrdinal = null);
}
