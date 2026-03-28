using Meta.Core.Domain;

namespace MetaSql;

/// <summary>
/// Encapsulates table-column alter executability and truncation-approval assessment.
/// </summary>
internal sealed class TableColumnAlterAssessmentService
{
    private static readonly HashSet<string> ExecutableColumnAspects = new(StringComparer.Ordinal)
    {
        "MetaDataTypeId",
        "MetaDataTypeDetail",
        "IsNullable",
    };

    private static readonly HashSet<string> LengthBasedSqlServerTypeNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "varchar",
        "char",
        "nvarchar",
        "nchar",
        "varbinary",
        "binary",
    };

    public (bool Executable, bool RequiresDataTruncation, string Reason) Assess(
        MetaSqlDifference difference,
        ManifestPlanningLookupContext lookup)
    {
        var sourceId = difference.SourceId ?? string.Empty;
        var liveId = difference.LiveId ?? string.Empty;
        if (string.IsNullOrWhiteSpace(sourceId) || string.IsNullOrWhiteSpace(liveId))
        {
            return (false, false, $"{difference.DisplayName}: missing SourceId/LiveId for changed column.");
        }

        if (!lookup.SourceColumnsById.TryGetValue(sourceId, out var sourceColumn) ||
            !lookup.LiveColumnsById.TryGetValue(liveId, out var liveColumn))
        {
            return (false, false, $"{difference.DisplayName}: changed column row is missing in source or live workspace.");
        }

        var changedAspects = GetChangedColumnAspects(
            sourceColumn,
            liveColumn,
            lookup.SourceColumnDetailsByColumnId,
            lookup.LiveColumnDetailsByColumnId);
        if (changedAspects.Count == 0)
        {
            return (false, false, $"{difference.DisplayName}: no executable changed aspects were detected.");
        }

        var unsupportedAspects = changedAspects
            .Where(row => !ExecutableColumnAspects.Contains(row))
            .OrderBy(row => row, StringComparer.Ordinal)
            .ToList();
        if (unsupportedAspects.Count > 0)
        {
            return (false, false, $"{difference.DisplayName}: unsupported column aspect change(s): {string.Join(", ", unsupportedAspects)}.");
        }

        var sourceTypeId = GetValue(sourceColumn, "MetaDataTypeId");
        var liveTypeId = GetValue(liveColumn, "MetaDataTypeId");
        if (!IsSupportedSqlServerType(sourceTypeId) || !IsSupportedSqlServerType(liveTypeId))
        {
            return (false, false, $"{difference.DisplayName}: AlterTableColumn supports only MetaDataTypeId values owned by DataTypeSystem 'SqlServer'.");
        }

        var typeShapeChanged = changedAspects.Contains("MetaDataTypeId", StringComparer.Ordinal) ||
                               changedAspects.Contains("MetaDataTypeDetail", StringComparer.Ordinal);
        if (typeShapeChanged)
        {
            var sourceTypeName = GetSqlServerTypeName(sourceTypeId);
            var liveTypeName = GetSqlServerTypeName(liveTypeId);
            if (!string.Equals(sourceTypeName, liveTypeName, StringComparison.OrdinalIgnoreCase))
            {
                return (false, false, $"{difference.DisplayName}: type family changes are not auto-executable in this deploy slice ({liveTypeName} -> {sourceTypeName}).");
            }

            if (!LengthBasedSqlServerTypeNames.Contains(sourceTypeName))
            {
                return (false, false, $"{difference.DisplayName}: type-shape changes are only auto-executable for length-based SqlServer types (char, nchar, varchar, nvarchar, binary, varbinary).");
            }

            var sourceDetailMap = GetDetailMap(lookup.SourceColumnDetailsByColumnId, sourceColumn.Id);
            var liveDetailMap = GetDetailMap(lookup.LiveColumnDetailsByColumnId, liveColumn.Id);
            if (!HasOnlyLengthDetailChange(sourceDetailMap, liveDetailMap))
            {
                return (false, false, $"{difference.DisplayName}: type-shape changes are only auto-executable when Length is the only changed detail.");
            }
        }

        var blockerKey = BuildColumnBlockerKey(sourceId, liveId);
        if (!lookup.BlockerByColumnPairKey.TryGetValue(blockerKey, out var blockers) || blockers.Count == 0)
        {
            return (true, false, string.Empty);
        }

        var sourceColumnScope = TryGetColumnScope(sourceColumn, lookup.SourceTablesById, lookup.SourceSchemasById, out var sourceSchemaName, out var sourceTableName, out var sourceColumnName);
        var liveColumnScope = TryGetColumnScope(liveColumn, lookup.LiveTablesById, lookup.LiveSchemasById, out var liveSchemaName, out var liveTableName, out var liveColumnName);
        if (!sourceColumnScope || !liveColumnScope)
        {
            return (false, false, $"{difference.DisplayName}: changed column scope is missing in source or live workspace.");
        }

        if (!string.Equals(sourceSchemaName, liveSchemaName, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(sourceTableName, liveTableName, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(sourceColumnName, liveColumnName, StringComparison.OrdinalIgnoreCase))
        {
            return (false, false, $"{difference.DisplayName}: changed column scope differs between source and live.");
        }

        var requiresDataTruncation = false;
        foreach (var blocker in blockers)
        {
            if (blocker.Code == MetaSqlDifferenceBlockerCode.DataTruncationRequired)
            {
                if (!lookup.ApprovalSet.HasDataTruncationColumn(sourceSchemaName, sourceTableName, sourceColumnName))
                {
                    var missingApproval = MetaSqlDestructiveApprovalSet.BuildColumnKey(sourceSchemaName, sourceTableName, sourceColumnName);
                    return (false, false, $"{blocker.Reason} Missing approval DataTruncationColumn({missingApproval}).");
                }

                requiresDataTruncation = true;
                continue;
            }

            return (false, false, blocker.Reason);
        }

        return (true, requiresDataTruncation, string.Empty);
    }

    public Dictionary<string, List<MetaSqlDifferenceBlocker>> BuildColumnBlockerLookup(
        IReadOnlyList<MetaSqlDifferenceBlocker>? blockers)
    {
        var result = new Dictionary<string, List<MetaSqlDifferenceBlocker>>(StringComparer.Ordinal);
        if (blockers is null)
        {
            return result;
        }

        foreach (var blocker in blockers)
        {
            if (blocker.Difference.ObjectKind != MetaSqlObjectKind.TableColumn)
            {
                continue;
            }

            var sourceId = blocker.Difference.SourceId ?? string.Empty;
            var liveId = blocker.Difference.LiveId ?? string.Empty;
            if (string.IsNullOrWhiteSpace(sourceId) || string.IsNullOrWhiteSpace(liveId))
            {
                continue;
            }

            var key = BuildColumnBlockerKey(sourceId, liveId);
            if (!result.TryGetValue(key, out var bucket))
            {
                bucket = new List<MetaSqlDifferenceBlocker>();
                result[key] = bucket;
            }

            bucket.Add(blocker);
        }

        return result;
    }

    public Dictionary<string, List<MetaSqlDifferenceBlocker>> BuildLiveColumnBlockerLookup(
        IReadOnlyList<MetaSqlDifferenceBlocker>? blockers)
    {
        var result = new Dictionary<string, List<MetaSqlDifferenceBlocker>>(StringComparer.Ordinal);
        if (blockers is null)
        {
            return result;
        }

        foreach (var blocker in blockers)
        {
            if (blocker.Difference.ObjectKind != MetaSqlObjectKind.TableColumn ||
                blocker.Difference.DifferenceKind != MetaSqlDifferenceKind.ExtraInLive)
            {
                continue;
            }

            var liveId = blocker.Difference.LiveId ?? string.Empty;
            if (string.IsNullOrWhiteSpace(liveId))
            {
                continue;
            }

            if (!result.TryGetValue(liveId, out var bucket))
            {
                bucket = new List<MetaSqlDifferenceBlocker>();
                result[liveId] = bucket;
            }

            bucket.Add(blocker);
        }

        return result;
    }

    private static List<string> GetChangedColumnAspects(
        GenericRecord sourceColumn,
        GenericRecord liveColumn,
        IReadOnlyDictionary<string, List<GenericRecord>> sourceColumnDetailsByColumnId,
        IReadOnlyDictionary<string, List<GenericRecord>> liveColumnDetailsByColumnId)
    {
        var changedAspects = new List<string>();
        AddIfDifferent(changedAspects, "Name", GetValue(sourceColumn, "Name"), GetValue(liveColumn, "Name"));
        AddIfDifferent(changedAspects, "Ordinal", GetValue(sourceColumn, "Ordinal"), GetValue(liveColumn, "Ordinal"));
        AddIfDifferent(changedAspects, "MetaDataTypeId", GetValue(sourceColumn, "MetaDataTypeId"), GetValue(liveColumn, "MetaDataTypeId"));
        AddIfDifferent(changedAspects, "IsNullable", GetValue(sourceColumn, "IsNullable"), GetValue(liveColumn, "IsNullable"));
        AddIfDifferent(changedAspects, "IsIdentity", GetValue(sourceColumn, "IsIdentity"), GetValue(liveColumn, "IsIdentity"));
        AddIfDifferent(changedAspects, "IdentitySeed", GetValue(sourceColumn, "IdentitySeed"), GetValue(liveColumn, "IdentitySeed"));
        AddIfDifferent(changedAspects, "IdentityIncrement", GetValue(sourceColumn, "IdentityIncrement"), GetValue(liveColumn, "IdentityIncrement"));
        AddIfDifferent(changedAspects, "ExpressionSql", GetValue(sourceColumn, "ExpressionSql"), GetValue(liveColumn, "ExpressionSql"));

        var sourceDetails = GetDetailPairs(sourceColumnDetailsByColumnId, sourceColumn.Id);
        var liveDetails = GetDetailPairs(liveColumnDetailsByColumnId, liveColumn.Id);
        if (!sourceDetails.SequenceEqual(liveDetails))
        {
            changedAspects.Add("MetaDataTypeDetail");
        }

        return changedAspects;
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

    private static Dictionary<string, string> GetDetailMap(
        IReadOnlyDictionary<string, List<GenericRecord>> detailsByColumnId,
        string columnId)
    {
        if (!detailsByColumnId.TryGetValue(columnId, out var rows))
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        return rows.ToDictionary(
            row => GetValue(row, "Name"),
            row => GetValue(row, "Value"),
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

    private static bool TryGetTableScope(
        GenericRecord tableRecord,
        IReadOnlyDictionary<string, GenericRecord> schemasById,
        out string schemaName,
        out string tableName)
    {
        schemaName = string.Empty;
        tableName = string.Empty;

        if (!tableRecord.RelationshipIds.TryGetValue("SchemaId", out var schemaId) ||
            string.IsNullOrWhiteSpace(schemaId) ||
            !schemasById.TryGetValue(schemaId, out var schemaRecord))
        {
            return false;
        }

        schemaName = GetValue(schemaRecord, "Name");
        tableName = GetValue(tableRecord, "Name");
        return !string.IsNullOrWhiteSpace(schemaName) && !string.IsNullOrWhiteSpace(tableName);
    }

    private static bool TryGetColumnScope(
        GenericRecord columnRecord,
        IReadOnlyDictionary<string, GenericRecord> tablesById,
        IReadOnlyDictionary<string, GenericRecord> schemasById,
        out string schemaName,
        out string tableName,
        out string columnName)
    {
        schemaName = string.Empty;
        tableName = string.Empty;
        columnName = string.Empty;

        if (!columnRecord.RelationshipIds.TryGetValue("TableId", out var tableId) ||
            string.IsNullOrWhiteSpace(tableId) ||
            !tablesById.TryGetValue(tableId, out var tableRecord))
        {
            return false;
        }

        if (!TryGetTableScope(tableRecord, schemasById, out schemaName, out tableName))
        {
            return false;
        }

        columnName = GetValue(columnRecord, "Name");
        return !string.IsNullOrWhiteSpace(columnName);
    }

    private static bool IsSupportedSqlServerType(string metaDataTypeId)
    {
        return SqlServerRenderingSupport.IsSqlServerTypeId(metaDataTypeId);
    }

    private static string GetSqlServerTypeName(string metaDataTypeId)
    {
        return SqlServerRenderingSupport.GetSqlServerTypeName(metaDataTypeId);
    }

    private static string BuildColumnBlockerKey(string sourceId, string liveId)
    {
        return sourceId + "|" + liveId;
    }

    private static void AddIfDifferent(List<string> changedAspects, string aspectName, string left, string right)
    {
        if (!string.Equals(left ?? string.Empty, right ?? string.Empty, StringComparison.Ordinal))
        {
            changedAspects.Add(aspectName);
        }
    }

    private static string GetValue(GenericRecord record, string propertyName)
    {
        return record.Values.TryGetValue(propertyName, out var value) ? value : string.Empty;
    }
}
