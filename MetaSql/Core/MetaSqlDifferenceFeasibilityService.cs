using Meta.Core.Domain;
using Microsoft.Data.SqlClient;

namespace MetaSql;

public enum MetaSqlDifferenceBlockerCode
{
    Unspecified = 0,
    DataTruncationRequired,
}

public sealed record MetaSqlDifferenceBlocker
{
    public required MetaSqlDifference Difference { get; init; }
    public required MetaSqlDifferenceBlockerCode Code { get; init; }
    public required string Reason { get; init; }
}

public sealed class MetaSqlDifferenceFeasibilityService
{
    private static readonly HashSet<string> SupportedAlterAspects = new(StringComparer.Ordinal)
    {
        "MetaDataTypeId",
        "MetaDataTypeDetail",
        "IsNullable",
    };

    public async Task<IReadOnlyList<MetaSqlDifferenceBlocker>> BuildBlockersAsync(
        IReadOnlyList<MetaSqlDifference> differences,
        Workspace sourceWorkspace,
        Workspace liveWorkspace,
        string connectionString,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(differences);
        ArgumentNullException.ThrowIfNull(sourceWorkspace);
        ArgumentNullException.ThrowIfNull(liveWorkspace);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        MetaSqlDiffService.EnsureMetaSqlWorkspace(sourceWorkspace, nameof(sourceWorkspace));
        MetaSqlDiffService.EnsureMetaSqlWorkspace(liveWorkspace, nameof(liveWorkspace));

        var blockers = new List<MetaSqlDifferenceBlocker>();
        var changedColumns = differences
            .Where(row => row.ObjectKind == MetaSqlObjectKind.TableColumn && row.DifferenceKind == MetaSqlDifferenceKind.Different)
            .ToList();
        var droppedColumns = differences
            .Where(row => row.ObjectKind == MetaSqlObjectKind.TableColumn && row.DifferenceKind == MetaSqlDifferenceKind.ExtraInLive)
            .ToList();
        if (changedColumns.Count == 0 && droppedColumns.Count == 0)
        {
            return blockers;
        }

        var sourceColumnsById = GetRecordIndex(sourceWorkspace, "TableColumn");
        var liveColumnsById = GetRecordIndex(liveWorkspace, "TableColumn");
        var liveTablesById = GetRecordIndex(liveWorkspace, "Table");
        var liveSchemasById = GetRecordIndex(liveWorkspace, "Schema");
        var sourceDetailsByColumnId = GetGroupedRecords(sourceWorkspace, "TableColumnDataTypeDetail", "TableColumnId");
        var liveDetailsByColumnId = GetGroupedRecords(liveWorkspace, "TableColumnDataTypeDetail", "TableColumnId");

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        foreach (var difference in droppedColumns)
        {
            if (string.IsNullOrWhiteSpace(difference.LiveId) ||
                !liveColumnsById.TryGetValue(difference.LiveId, out var liveColumn))
            {
                continue;
            }

            if (!liveTablesById.TryGetValue(liveColumn.RelationshipIds["TableId"], out var liveTable) ||
                !liveSchemasById.TryGetValue(liveTable.RelationshipIds["SchemaId"], out var liveSchema))
            {
                continue;
            }

            var schemaName = liveSchema.Values["Name"];
            var tableName = liveTable.Values["Name"];
            var columnName = liveColumn.Values["Name"];
            var dependencyKinds = await GetConstraintDependencyKindsAsync(
                    connection,
                    schemaName,
                    tableName,
                    columnName,
                    cancellationToken)
                .ConfigureAwait(false);
            if (dependencyKinds.Count == 0)
            {
                continue;
            }

            blockers.Add(new MetaSqlDifferenceBlocker
            {
                Difference = difference,
                Code = MetaSqlDifferenceBlockerCode.Unspecified,
                Reason = $"{difference.DisplayName}: DROP COLUMN is blocked by live {string.Join("/", dependencyKinds)} constraint dependency.",
            });
        }

        foreach (var difference in changedColumns)
        {
            if (string.IsNullOrWhiteSpace(difference.SourceId) ||
                string.IsNullOrWhiteSpace(difference.LiveId) ||
                !sourceColumnsById.TryGetValue(difference.SourceId, out var sourceColumn) ||
                !liveColumnsById.TryGetValue(difference.LiveId, out var liveColumn))
            {
                continue;
            }

            if (!liveTablesById.TryGetValue(liveColumn.RelationshipIds["TableId"], out var liveTable) ||
                !liveSchemasById.TryGetValue(liveTable.RelationshipIds["SchemaId"], out var liveSchema))
            {
                continue;
            }

            var schemaName = liveSchema.Values["Name"];
            var tableName = liveTable.Values["Name"];
            var columnName = liveColumn.Values["Name"];

            var changedAspects = GetChangedColumnAspects(
                sourceColumn,
                liveColumn,
                sourceDetailsByColumnId,
                liveDetailsByColumnId);
            if (changedAspects.Count == 0)
            {
                continue;
            }

            var hasSupportedAlterAspect = changedAspects.Any(aspect => SupportedAlterAspects.Contains(aspect));
            if (!hasSupportedAlterAspect)
            {
                continue;
            }

            var sourceNullable = IsTrue(GetValue(sourceColumn, "IsNullable"));
            var liveNullable = IsTrue(GetValue(liveColumn, "IsNullable"));
            if (liveNullable && !sourceNullable)
            {
                var hasNulls = await ExistsAsync(
                    connection,
                    $"SELECT TOP (1) 1 FROM {QuoteIdentifier(schemaName)}.{QuoteIdentifier(tableName)} WHERE {QuoteIdentifier(columnName)} IS NULL;",
                    null,
                    cancellationToken).ConfigureAwait(false);
                if (hasNulls)
                {
                    blockers.Add(new MetaSqlDifferenceBlocker
                    {
                        Difference = difference,
                        Code = MetaSqlDifferenceBlockerCode.Unspecified,
                        Reason = $"{difference.DisplayName}: source requires NOT NULL but live contains NULL values.",
                    });
                }
            }

            var sourceTypeId = GetValue(sourceColumn, "MetaDataTypeId");
            var liveTypeId = GetValue(liveColumn, "MetaDataTypeId");
            var sourceLength = GetLength(sourceDetailsByColumnId, sourceColumn.Id);
            var liveLength = GetLength(liveDetailsByColumnId, liveColumn.Id);
            var typeShapeChanged = changedAspects.Contains("MetaDataTypeId", StringComparer.Ordinal) ||
                                   changedAspects.Contains("MetaDataTypeDetail", StringComparer.Ordinal);
            if (typeShapeChanged)
            {
                var isPartitioned = await ExistsAsync(
                    connection,
                    """
                    SELECT TOP (1) 1
                    FROM sys.tables AS t
                    INNER JOIN sys.schemas AS s ON s.schema_id = t.schema_id
                    INNER JOIN sys.indexes AS i ON i.object_id = t.object_id
                    INNER JOIN sys.partition_schemes AS ps ON ps.data_space_id = i.data_space_id
                    WHERE s.name = @SchemaName
                      AND t.name = @TableName;
                    """,
                    command =>
                    {
                        command.Parameters.Add(new SqlParameter("@SchemaName", schemaName));
                        command.Parameters.Add(new SqlParameter("@TableName", tableName));
                    },
                    cancellationToken).ConfigureAwait(false);
                if (isPartitioned)
                {
                    blockers.Add(new MetaSqlDifferenceBlocker
                    {
                        Difference = difference,
                        Code = MetaSqlDifferenceBlockerCode.Unspecified,
                        Reason = $"{difference.DisplayName}: SQL Server blocks ALTER COLUMN data-type changes on partitioned tables.",
                    });
                }
            }

            var dependencyKinds = await GetConstraintDependencyKindsAsync(
                    connection,
                    schemaName,
                    tableName,
                    columnName,
                    cancellationToken)
                .ConfigureAwait(false);
            if (dependencyKinds.Count > 0)
            {
                blockers.Add(new MetaSqlDifferenceBlocker
                {
                    Difference = difference,
                    Code = MetaSqlDifferenceBlockerCode.Unspecified,
                    Reason = $"{difference.DisplayName}: ALTER COLUMN is blocked by live {string.Join("/", dependencyKinds)} constraint dependency.",
                });
            }

            if (ShouldCheckLengthNarrowing(sourceTypeId, liveTypeId, sourceLength, liveLength))
            {
                var maxBytes = GetMaxBytes(sourceTypeId, sourceLength!.Value);
                if (maxBytes > 0)
                {
                    var sql = $"SELECT TOP (1) 1 FROM {QuoteIdentifier(schemaName)}.{QuoteIdentifier(tableName)} WHERE {QuoteIdentifier(columnName)} IS NOT NULL AND DATALENGTH({QuoteIdentifier(columnName)}) > @maxBytes;";
                    var hasTooLargeValues = await ExistsAsync(
                        connection,
                        sql,
                        command => command.Parameters.Add(new SqlParameter("@maxBytes", maxBytes)),
                        cancellationToken).ConfigureAwait(false);
                    if (hasTooLargeValues)
                    {
                        blockers.Add(new MetaSqlDifferenceBlocker
                        {
                            Difference = difference,
                            Code = MetaSqlDifferenceBlockerCode.DataTruncationRequired,
                            Reason = $"{difference.DisplayName}: source length {sourceLength.Value} is smaller than live data currently stored.",
                        });
                    }
                }
            }
        }

        return blockers;
    }

    private static List<string> GetChangedColumnAspects(
        GenericRecord sourceColumn,
        GenericRecord liveColumn,
        IReadOnlyDictionary<string, List<GenericRecord>> sourceDetailsByColumnId,
        IReadOnlyDictionary<string, List<GenericRecord>> liveDetailsByColumnId)
    {
        var changedAspects = new List<string>();
        AddIfDifferent(changedAspects, "Ordinal", GetValue(sourceColumn, "Ordinal"), GetValue(liveColumn, "Ordinal"));
        AddIfDifferent(changedAspects, "MetaDataTypeId", GetValue(sourceColumn, "MetaDataTypeId"), GetValue(liveColumn, "MetaDataTypeId"));
        AddIfDifferent(changedAspects, "IsNullable", GetValue(sourceColumn, "IsNullable"), GetValue(liveColumn, "IsNullable"));
        AddIfDifferent(changedAspects, "IsIdentity", GetValue(sourceColumn, "IsIdentity"), GetValue(liveColumn, "IsIdentity"));
        AddIfDifferent(changedAspects, "IdentitySeed", GetValue(sourceColumn, "IdentitySeed"), GetValue(liveColumn, "IdentitySeed"));
        AddIfDifferent(changedAspects, "IdentityIncrement", GetValue(sourceColumn, "IdentityIncrement"), GetValue(liveColumn, "IdentityIncrement"));
        AddIfDifferent(changedAspects, "ExpressionSql", GetValue(sourceColumn, "ExpressionSql"), GetValue(liveColumn, "ExpressionSql"));

        var sourceDetails = GetDetailPairs(sourceDetailsByColumnId, sourceColumn.Id);
        var liveDetails = GetDetailPairs(liveDetailsByColumnId, liveColumn.Id);
        if (!sourceDetails.SequenceEqual(liveDetails))
        {
            changedAspects.Add("MetaDataTypeDetail");
        }

        return changedAspects;
    }

    private static async Task<IReadOnlyList<string>> GetConstraintDependencyKindsAsync(
        SqlConnection connection,
        string schemaName,
        string tableName,
        string columnName,
        CancellationToken cancellationToken)
    {
        var kinds = new List<string>();

        var hasDefaultDependency = await ExistsAsync(
            connection,
            """
            SELECT TOP (1) 1
            FROM sys.default_constraints AS dc
            INNER JOIN sys.tables AS t ON t.object_id = dc.parent_object_id
            INNER JOIN sys.schemas AS s ON s.schema_id = t.schema_id
            INNER JOIN sys.columns AS c ON c.object_id = t.object_id AND c.column_id = dc.parent_column_id
            WHERE s.name = @SchemaName
              AND t.name = @TableName
              AND c.name = @ColumnName;
            """,
            command =>
            {
                command.Parameters.Add(new SqlParameter("@SchemaName", schemaName));
                command.Parameters.Add(new SqlParameter("@TableName", tableName));
                command.Parameters.Add(new SqlParameter("@ColumnName", columnName));
            },
            cancellationToken).ConfigureAwait(false);
        if (hasDefaultDependency)
        {
            kinds.Add("DEFAULT");
        }

        var hasCheckDependency = await ExistsAsync(
            connection,
            """
            SELECT TOP (1) 1
            FROM sys.check_constraints AS cc
            INNER JOIN sys.tables AS t ON t.object_id = cc.parent_object_id
            INNER JOIN sys.schemas AS s ON s.schema_id = t.schema_id
            INNER JOIN sys.columns AS c ON c.object_id = t.object_id
            WHERE s.name = @SchemaName
              AND t.name = @TableName
              AND c.name = @ColumnName
              AND (
                    cc.parent_column_id = c.column_id
                    OR EXISTS (
                        SELECT 1
                        FROM sys.sql_expression_dependencies AS d
                        WHERE d.referencing_id = cc.object_id
                          AND d.referenced_id = t.object_id
                          AND d.referenced_minor_id = c.column_id
                    )
                  );
            """,
            command =>
            {
                command.Parameters.Add(new SqlParameter("@SchemaName", schemaName));
                command.Parameters.Add(new SqlParameter("@TableName", tableName));
                command.Parameters.Add(new SqlParameter("@ColumnName", columnName));
            },
            cancellationToken).ConfigureAwait(false);
        if (hasCheckDependency)
        {
            kinds.Add("CHECK");
        }

        var hasUniqueDependency = await ExistsAsync(
            connection,
            """
            SELECT TOP (1) 1
            FROM sys.key_constraints AS kc
            INNER JOIN sys.tables AS t ON t.object_id = kc.parent_object_id
            INNER JOIN sys.schemas AS s ON s.schema_id = t.schema_id
            INNER JOIN sys.index_columns AS ic ON ic.object_id = kc.parent_object_id AND ic.index_id = kc.unique_index_id
            INNER JOIN sys.columns AS c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
            WHERE kc.type = 'UQ'
              AND s.name = @SchemaName
              AND t.name = @TableName
              AND c.name = @ColumnName;
            """,
            command =>
            {
                command.Parameters.Add(new SqlParameter("@SchemaName", schemaName));
                command.Parameters.Add(new SqlParameter("@TableName", tableName));
                command.Parameters.Add(new SqlParameter("@ColumnName", columnName));
            },
            cancellationToken).ConfigureAwait(false);
        if (hasUniqueDependency)
        {
            kinds.Add("UNIQUE");
        }

        return kinds;
    }

    private static bool ShouldCheckLengthNarrowing(
        string sourceTypeId,
        string liveTypeId,
        int? sourceLength,
        int? liveLength)
    {
        if (!sourceLength.HasValue || sourceLength.Value <= 0)
        {
            return false;
        }

        if (!IsSqlServerLengthType(sourceTypeId) || !IsSqlServerLengthType(liveTypeId))
        {
            return false;
        }

        if (!liveLength.HasValue)
        {
            return true;
        }

        if (liveLength.Value <= 0)
        {
            return true;
        }

        return sourceLength.Value < liveLength.Value;
    }

    private static bool IsSqlServerLengthType(string typeId)
    {
        var typeName = SqlServerRenderingSupport.GetSqlServerTypeName(typeId);
        return string.Equals(typeName, "varchar", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(typeName, "char", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(typeName, "nvarchar", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(typeName, "nchar", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(typeName, "varbinary", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(typeName, "binary", StringComparison.OrdinalIgnoreCase);
    }

    private static int GetMaxBytes(string sourceTypeId, int sourceLength)
    {
        var typeName = SqlServerRenderingSupport.GetSqlServerTypeName(sourceTypeId);
        if (string.Equals(typeName, "nvarchar", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(typeName, "nchar", StringComparison.OrdinalIgnoreCase))
        {
            return sourceLength * 2;
        }

        return sourceLength;
    }

    private static List<string> GetDetailPairs(
        IReadOnlyDictionary<string, List<GenericRecord>> detailsByColumnId,
        string columnId)
    {
        if (!detailsByColumnId.TryGetValue(columnId, out var rows))
        {
            return [];
        }

        return rows
            .Select(row => $"{GetValue(row, "Name")}={GetValue(row, "Value")}")
            .OrderBy(row => row, StringComparer.Ordinal)
            .ToList();
    }

    private static void AddIfDifferent(List<string> changedAspects, string aspectName, string left, string right)
    {
        if (!string.Equals(left ?? string.Empty, right ?? string.Empty, StringComparison.Ordinal))
        {
            changedAspects.Add(aspectName);
        }
    }

    private static int? GetLength(IReadOnlyDictionary<string, List<GenericRecord>> detailsByColumnId, string columnId)
    {
        if (!detailsByColumnId.TryGetValue(columnId, out var details))
        {
            return null;
        }

        var lengthRecord = details.FirstOrDefault(row =>
            string.Equals(GetValue(row, "Name"), "Length", StringComparison.Ordinal));
        if (lengthRecord is null)
        {
            return null;
        }

        return int.TryParse(GetValue(lengthRecord, "Value"), out var parsed) ? parsed : null;
    }

    private static string QuoteIdentifier(string name)
    {
        return "[" + name.Replace("]", "]]", StringComparison.Ordinal) + "]";
    }

    private static async Task<bool> ExistsAsync(
        SqlConnection connection,
        string sql,
        Action<SqlCommand>? addParameters,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        addParameters?.Invoke(command);
        var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return result is not null && result is not DBNull;
    }

    private static bool IsTrue(string value)
    {
        return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "1", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetValue(GenericRecord record, string propertyName)
    {
        return record.Values.TryGetValue(propertyName, out var value) ? value : string.Empty;
    }

    private static Dictionary<string, GenericRecord> GetRecordIndex(Workspace workspace, string entityName)
    {
        return workspace.Instance.GetOrCreateEntityRecords(entityName).ToDictionary(row => row.Id, StringComparer.Ordinal);
    }

    private static Dictionary<string, List<GenericRecord>> GetGroupedRecords(Workspace workspace, string entityName, string relationshipName)
    {
        return workspace.Instance.GetOrCreateEntityRecords(entityName)
            .GroupBy(row => row.RelationshipIds[relationshipName], StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(row => row.Id, StringComparer.Ordinal).ToList(),
                StringComparer.Ordinal);
    }
}
