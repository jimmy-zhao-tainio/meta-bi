namespace MetaSql;

/// <summary>
/// Renders SQL Server ALTER COLUMN statements.
/// </summary>
internal sealed class SqlServerAlterColumnSqlRenderer
{
    private static readonly HashSet<string> ExecutableColumnAspects = new(StringComparer.Ordinal)
    {
        "MetaDataTypeId",
        "MetaDataTypeDetail",
        "IsNullable",
    };

    public string BuildAlterColumnSql(
        TableColumn sourceColumn,
        TableColumn liveColumn,
        IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> sourceDetailsByColumnId,
        IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> liveDetailsByColumnId)
    {
        if (sourceColumn.TableId != liveColumn.TableId)
        {
            throw new InvalidOperationException(
                $"Cannot alter column because source table '{sourceColumn.TableId}' and live table '{liveColumn.TableId}' differ.");
        }

        if (sourceColumn.Name != liveColumn.Name)
        {
            throw new InvalidOperationException(
                $"Cannot alter column '{sourceColumn.Id}' because source name '{sourceColumn.Name}' and live name '{liveColumn.Name}' differ.");
        }

        if (!string.IsNullOrWhiteSpace(sourceColumn.ExpressionSql) ||
            !string.IsNullOrWhiteSpace(liveColumn.ExpressionSql))
        {
            throw new InvalidOperationException(
                $"Cannot alter computed column '{sourceColumn.Id}' in this deploy slice.");
        }

        if (SqlServerRenderingSupport.IsTrue(sourceColumn.IsIdentity) || SqlServerRenderingSupport.IsTrue(liveColumn.IsIdentity))
        {
            throw new InvalidOperationException(
                $"Cannot alter identity column '{sourceColumn.Id}' in this deploy slice.");
        }

        var changedAspects = GetChangedColumnAspects(sourceColumn, liveColumn, sourceDetailsByColumnId, liveDetailsByColumnId);
        if (changedAspects.Count == 0)
        {
            throw new InvalidOperationException(
                $"Cannot alter column '{sourceColumn.Id}' because no supported changed aspects were detected.");
        }

        var unsupportedAspects = changedAspects
            .Where(row => !ExecutableColumnAspects.Contains(row))
            .OrderBy(row => row, StringComparer.Ordinal)
            .ToList();
        if (unsupportedAspects.Count > 0)
        {
            throw new InvalidOperationException(
                $"Cannot alter column '{sourceColumn.Id}' because unsupported aspect changes are present: {string.Join(", ", unsupportedAspects)}.");
        }

        var typeShapeChanged = changedAspects.Contains("MetaDataTypeId", StringComparer.Ordinal) ||
                               changedAspects.Contains("MetaDataTypeDetail", StringComparer.Ordinal);
        if (typeShapeChanged)
        {
            if (!SqlServerRenderingSupport.IsSqlServerTypeId(sourceColumn.MetaDataTypeId) ||
                !SqlServerRenderingSupport.IsSqlServerTypeId(liveColumn.MetaDataTypeId))
            {
                throw new InvalidOperationException(
                    $"Cannot alter column '{sourceColumn.Id}' because only MetaDataTypeId values owned by DataTypeSystem 'SqlServer' are supported.");
            }

            var sourceTypeName = SqlServerRenderingSupport.GetSqlServerTypeName(sourceColumn.MetaDataTypeId);
            var liveTypeName = SqlServerRenderingSupport.GetSqlServerTypeName(liveColumn.MetaDataTypeId);
            if (!string.Equals(sourceTypeName, liveTypeName, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Cannot alter column '{sourceColumn.Id}' because type-family transitions are blocked in this deploy slice ({liveTypeName} -> {sourceTypeName}).");
            }

            if (!SqlServerRenderingSupport.LengthBasedSqlServerTypeNames.Contains(sourceTypeName))
            {
                throw new InvalidOperationException(
                    $"Cannot alter column '{sourceColumn.Id}' because only length-based SqlServer type-shape changes are executable in this deploy slice.");
            }

            var sourceDetailMap = GetDetailMap(sourceDetailsByColumnId, sourceColumn.Id);
            var liveDetailMap = GetDetailMap(liveDetailsByColumnId, liveColumn.Id);
            if (!HasOnlyLengthDetailChange(sourceDetailMap, liveDetailMap))
            {
                throw new InvalidOperationException(
                    $"Cannot alter column '{sourceColumn.Id}' because only Length detail changes are executable for type-shape changes in this deploy slice.");
            }
        }

        var detailValues = GetGroup(sourceDetailsByColumnId, sourceColumn.Id)
            .ToDictionary(row => row.Name, row => row.Value, StringComparer.OrdinalIgnoreCase);
        var typeSql = SqlServerRenderingSupport.BuildSqlServerTypeSql(sourceColumn.MetaDataTypeId, detailValues);
        var nullableSql = SqlServerRenderingSupport.IsTrue(sourceColumn.IsNullable) ? "NULL" : "NOT NULL";
        return $"ALTER TABLE {SqlServerRenderingSupport.FormatTableName(liveColumn.Table)} ALTER COLUMN {SqlServerRenderingSupport.EscapeSqlIdentifier(liveColumn.Name)} {typeSql} {nullableSql};";
    }

    private static List<string> GetChangedColumnAspects(
        TableColumn sourceColumn,
        TableColumn liveColumn,
        IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> sourceDetailsByColumnId,
        IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> liveDetailsByColumnId)
    {
        var changedAspects = new List<string>();
        AddIfDifferent(changedAspects, "Name", sourceColumn.Name, liveColumn.Name);
        AddIfDifferent(changedAspects, "Ordinal", sourceColumn.Ordinal, liveColumn.Ordinal);
        AddIfDifferent(changedAspects, "MetaDataTypeId", sourceColumn.MetaDataTypeId, liveColumn.MetaDataTypeId);
        AddIfDifferent(changedAspects, "IsNullable", sourceColumn.IsNullable, liveColumn.IsNullable);
        AddIfDifferent(changedAspects, "IsIdentity", sourceColumn.IsIdentity, liveColumn.IsIdentity);
        AddIfDifferent(changedAspects, "IdentitySeed", sourceColumn.IdentitySeed, liveColumn.IdentitySeed);
        AddIfDifferent(changedAspects, "IdentityIncrement", sourceColumn.IdentityIncrement, liveColumn.IdentityIncrement);
        AddIfDifferent(changedAspects, "ExpressionSql", sourceColumn.ExpressionSql, liveColumn.ExpressionSql);

        var sourceDetails = GetDetailPairs(sourceDetailsByColumnId, sourceColumn.Id);
        var liveDetails = GetDetailPairs(liveDetailsByColumnId, liveColumn.Id);
        if (!sourceDetails.SequenceEqual(liveDetails))
        {
            changedAspects.Add("MetaDataTypeDetail");
        }

        return changedAspects;
    }

    private static void AddIfDifferent(List<string> changedAspects, string aspectName, string left, string right)
    {
        if (!string.Equals(left ?? string.Empty, right ?? string.Empty, StringComparison.Ordinal))
        {
            changedAspects.Add(aspectName);
        }
    }

    private static IReadOnlyList<T> GetGroup<T>(IReadOnlyDictionary<string, List<T>> groups, string key)
    {
        return groups.TryGetValue(key, out var bucket)
            ? bucket
            : Array.Empty<T>();
    }

    private static List<string> GetDetailPairs(
        IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> detailsByColumnId,
        string columnId)
    {
        return GetGroup(detailsByColumnId, columnId)
            .Select(row => $"{row.Name}={row.Value}")
            .OrderBy(row => row, StringComparer.Ordinal)
            .ToList();
    }

    private static Dictionary<string, string> GetDetailMap(
        IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> detailsByColumnId,
        string columnId)
    {
        return GetGroup(detailsByColumnId, columnId)
            .ToDictionary(
                row => row.Name,
                row => row.Value,
                StringComparer.OrdinalIgnoreCase);
    }

    private static bool HasOnlyLengthDetailChange(
        IReadOnlyDictionary<string, string> sourceDetailMap,
        IReadOnlyDictionary<string, string> liveDetailMap)
    {
        var detailNames = sourceDetailMap.Keys
            .Concat(liveDetailMap.Keys)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(row => row, StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var detailName in detailNames)
        {
            var sourceValue = sourceDetailMap.TryGetValue(detailName, out var sourceFoundValue) ? sourceFoundValue : string.Empty;
            var liveValue = liveDetailMap.TryGetValue(detailName, out var liveFoundValue) ? liveFoundValue : string.Empty;
            if (string.Equals(sourceValue, liveValue, StringComparison.Ordinal))
            {
                continue;
            }

            if (!string.Equals(detailName, "Length", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }
}
