using System.Globalization;
using MetaSql;

namespace MetaConvert.DataVaultToSql;

internal static partial class BusinessDataVaultToSqlCSharpReference
{
    private const int SqlServerIdentifierMaxLength = 128;

    private static T RequireSingleImplementation<T>(IReadOnlyList<T> rows, string logicalName)
        where T : class
    {
        if (rows.Count != 1)
        {
            throw new InvalidOperationException($"{logicalName} must contain exactly one row for this projection path.");
        }

        return rows[0];
    }

    private static Table AddTable(ConversionContext context, string schemaName, string name)
    {
        if (!context.SchemasByName.TryGetValue(schemaName, out var schema))
        {
            throw new InvalidOperationException($"Projected schema '{schemaName}' is not present in conversion context.");
        }

        var actualName = RequireSqlServerIdentifier(name, "table");
        var id = $"{schema.Id}.{actualName}";
        EnsureUniqueId(context.MetaSql.TableList.Select(row => row.Id), id, "table");

        var table = new Table
        {
            Id = id,
            Name = actualName,
            Schema = schema,
        };

        context.MetaSql.TableList.Add(table);
        return table;
    }

    private static TableColumn AddImplementationColumn(
        ConversionContext context,
        Table table,
        string name,
        string metaDataTypeId,
        string isNullable,
        HashSet<string> reservedColumnNames,
        params (string Name, string? Value)[] details)
    {
        return AddImplementationColumn(
            context,
            table,
            name,
            metaDataTypeId,
            isNullable,
            reservedColumnNames,
            defaultExpressionSql: null,
            details);
    }

    private static TableColumn AddImplementationColumn(
        ConversionContext context,
        Table table,
        string name,
        string metaDataTypeId,
        string isNullable,
        HashSet<string> reservedColumnNames,
        string? defaultExpressionSql,
        params (string Name, string? Value)[] details)
    {
        var column = AddColumn(
            context,
            table,
            name,
            ResolveProjectedMetaDataTypeId(context, metaDataTypeId),
            isNullable,
            reservedColumnNames);
        column.DefaultExpressionSql = string.IsNullOrWhiteSpace(defaultExpressionSql)
            ? null
            : defaultExpressionSql.Trim();

        foreach (var detail in details)
        {
            AddDetail(context, column, detail.Name, detail.Value);
        }

        return column;
    }

    private static string ResolveProjectedMetaDataTypeId(ConversionContext context, string metaDataTypeId)
    {
        return context.BusinessTypeLowering is null
            ? metaDataTypeId
            : context.BusinessTypeLowering.LowerRequired(metaDataTypeId);
    }

    private static TableColumn AddColumn(
        ConversionContext context,
        Table table,
        string requestedName,
        string metaDataTypeId,
        string isNullable,
        HashSet<string> reservedColumnNames)
    {
        var actualName = ReserveColumnName(
            reservedColumnNames,
            RequireSqlServerIdentifier(requestedName, "column"));
        var id = $"{table.Id}.{actualName}";
        EnsureUniqueId(context.MetaSql.TableColumnList.Select(row => row.Id), id, "table column");
        var ordinal = (context.MetaSql.TableColumnList.Count(row => ReferenceEquals(row.Table, table)) + 1).ToString(CultureInfo.InvariantCulture);

        var column = new TableColumn
        {
            Id = id,
            Name = actualName,
            Ordinal = ordinal,
            MetaDataTypeId = metaDataTypeId,
            IsNullable = isNullable,
            Table = table,
        };

        context.MetaSql.TableColumnList.Add(column);
        return column;
    }

    private static void AddDetail(ConversionContext context, TableColumn tableColumn, string name, string? value)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        context.MetaSql.TableColumnDataTypeDetailList.Add(new TableColumnDataTypeDetail
        {
            Id = $"{tableColumn.Id}.detail.{name}",
            Name = name,
            Value = value,
            TableColumn = tableColumn,
        });
    }

    private static void AddPrimaryKey(
        ConversionContext context,
        Table table,
        string name,
        params TableColumn[] tableColumns)
    {
        if (tableColumns.Length == 0)
        {
            throw new InvalidOperationException("A primary key must contain at least one column.");
        }

        var actualName = RequireSqlServerIdentifier(name, "primary-key");
        var id = $"{table.Id}.pk.{actualName}";
        EnsureUniqueId(context.MetaSql.PrimaryKeyList.Select(row => row.Id), id, "primary key");

        var primaryKey = new PrimaryKey
        {
            Id = id,
            Name = actualName,
            Table = table,
        };
        context.MetaSql.PrimaryKeyList.Add(primaryKey);
        for (var index = 0; index < tableColumns.Length; index++)
        {
            context.MetaSql.PrimaryKeyColumnList.Add(new PrimaryKeyColumn
            {
                Id = $"{id}.column.{index + 1}",
                PrimaryKey = primaryKey,
                TableColumn = tableColumns[index],
                Ordinal = (index + 1).ToString(CultureInfo.InvariantCulture),
            });
        }
    }

    private static void AddForeignKey(
        ConversionContext context,
        Table sourceTable,
        string name,
        Table targetTable,
        IEnumerable<(TableColumn SourceColumn, TableColumn TargetColumn)> columnPairs)
    {
        var actualName = RequireSqlServerIdentifier(name, "foreign-key");
        var id = $"{sourceTable.Id}.fk.{actualName}";
        EnsureUniqueId(context.MetaSql.ForeignKeyList.Select(row => row.Id), id, "foreign key");

        var foreignKey = new ForeignKey
        {
            Id = id,
            Name = actualName,
            SourceTable = sourceTable,
            TargetTable = targetTable,
        };
        context.MetaSql.ForeignKeyList.Add(foreignKey);

        var ordinal = 1;
        foreach (var (sourceColumn, targetColumn) in columnPairs)
        {
            context.MetaSql.ForeignKeyColumnList.Add(new ForeignKeyColumn
            {
                Id = $"{id}.column.{ordinal}",
                ForeignKey = foreignKey,
                SourceColumn = sourceColumn,
                TargetColumn = targetColumn,
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture),
            });
            ordinal++;
        }
    }

    private static string ReserveColumnName(HashSet<string> reservedColumnNames, string requestedName)
    {
        var actualName = requestedName;
        while (reservedColumnNames.Contains(actualName))
        {
            actualName = RequireSqlServerIdentifier("_" + actualName, "column");
        }

        reservedColumnNames.Add(actualName);
        return actualName;
    }

    private static string RequireSqlServerIdentifier(string value, string logicalName)
    {
        if (value.Length > SqlServerIdentifierMaxLength)
        {
            throw new InvalidOperationException(
                $"SQL Server {logicalName} identifier '{value}' contains {value.Length} characters; maximum is {SqlServerIdentifierMaxLength}.");
        }

        return value;
    }

    private static void EnsureUniqueId(IEnumerable<string> existingIds, string id, string logicalName)
    {
        if (existingIds.Contains(id, StringComparer.Ordinal))
        {
            throw new InvalidOperationException($"Projected {logicalName} id '{id}' is duplicated. The physical naming contract is not unique.");
        }
    }

    private static string ApplyPattern(string pattern, params (string Token, string Value)[] replacements)
    {
        var output = pattern;
        foreach (var (token, value) in replacements)
        {
            output = output.Replace("{" + token + "}", value, StringComparison.Ordinal);
        }

        return output;
    }

    private static int ParseOrdinal(string ordinal)
    {
        return int.TryParse(ordinal, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : int.MaxValue;
    }
}
