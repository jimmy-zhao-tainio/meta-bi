using Meta.Core.Domain;
using Microsoft.Data.SqlClient;

namespace MetaSql;

public sealed record MetaSqlDifferenceBlocker
{
    public required MetaSqlDifference Difference { get; init; }
    public required string Reason { get; init; }
}

public sealed class MetaSqlDifferenceFeasibilityService
{
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
        if (changedColumns.Count == 0)
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
                        Reason = $"{difference.DisplayName}: source requires NOT NULL but live contains NULL values.",
                    });
                }
            }

            var sourceTypeId = GetValue(sourceColumn, "MetaDataTypeId");
            var liveTypeId = GetValue(liveColumn, "MetaDataTypeId");
            var sourceLength = GetLength(sourceDetailsByColumnId, sourceColumn.Id);
            var liveLength = GetLength(liveDetailsByColumnId, liveColumn.Id);
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
                            Reason = $"{difference.DisplayName}: source length {sourceLength.Value} is smaller than live data currently stored.",
                        });
                    }
                }
            }
        }

        return blockers;
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
        return string.Equals(typeId, "sqlserver:type:varchar", StringComparison.Ordinal) ||
               string.Equals(typeId, "sqlserver:type:char", StringComparison.Ordinal) ||
               string.Equals(typeId, "sqlserver:type:nvarchar", StringComparison.Ordinal) ||
               string.Equals(typeId, "sqlserver:type:nchar", StringComparison.Ordinal) ||
               string.Equals(typeId, "sqlserver:type:varbinary", StringComparison.Ordinal) ||
               string.Equals(typeId, "sqlserver:type:binary", StringComparison.Ordinal);
    }

    private static int GetMaxBytes(string sourceTypeId, int sourceLength)
    {
        if (string.Equals(sourceTypeId, "sqlserver:type:nvarchar", StringComparison.Ordinal) ||
            string.Equals(sourceTypeId, "sqlserver:type:nchar", StringComparison.Ordinal))
        {
            return sourceLength * 2;
        }

        return sourceLength;
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
