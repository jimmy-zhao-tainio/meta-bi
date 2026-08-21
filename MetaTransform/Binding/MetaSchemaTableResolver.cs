using System.Globalization;
using MetaSchema;
using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed class MetaSchemaTableResolver
{
    private readonly IReadOnlyDictionary<string, ResolvedSchemaTable> tablesById;
    private readonly IReadOnlyList<ResolvedSchemaTable> tables;

    public MetaSchemaTableResolver(MetaSchemaModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var systemNamesById = model.SystemList.ToDictionary(item => item.Id, item => item.Name, StringComparer.Ordinal);
        var schemaRowsById = model.SchemaList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        var dataTypeDetailsByFieldId = model.FieldDataTypeDetailList
            .GroupBy(item => item.Field.Id, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                ResolveDataTypeDetail,
                StringComparer.Ordinal);
        var tableIds = model.TableList
            .Select(item => item.SchemaObject.Id)
            .ToHashSet(StringComparer.Ordinal);
        var viewIds = model.ViewList
            .Select(item => item.SchemaObject.Id)
            .ToHashSet(StringComparer.Ordinal);
        var fieldRowsBySchemaObjectId = model.FieldList
            .GroupBy(item => item.SchemaObject.Id, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderBy(item => ParseOrdinal(item.Ordinal))
                    .ThenBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                    .Select(item =>
                    {
                        var dataTypeDetail = dataTypeDetailsByFieldId.GetValueOrDefault(item.Id);
                        return new ResolvedSchemaField(
                            item.Id,
                            item.Name,
                            ParseOrdinal(item.Ordinal),
                            item.MetaDataTypeId,
                            IsTrue(item.IsIdentity),
                            viewIds.Contains(item.SchemaObject.Id) ? true : ParseOptionalBool(item.IsNullable),
                            dataTypeDetail.Length,
                            dataTypeDetail.Precision,
                            dataTypeDetail.Scale);
                    })
                    .ToArray(),
                StringComparer.Ordinal);

        tables = model.SchemaObjectList
            .Select(item =>
            {
                var isTable = tableIds.Contains(item.Id);
                var isView = viewIds.Contains(item.Id);
                if (isTable == isView)
                {
                    throw new InvalidOperationException(
                        $"Schema object '{item.Id}' must have exactly one Table or View specialization.");
                }

                schemaRowsById.TryGetValue(item.Schema.Id, out var schemaRow);
                var systemId = schemaRow?.System.Id ?? string.Empty;
                var systemName = systemNamesById.GetValueOrDefault(systemId) ?? string.Empty;
                var schemaName = schemaRow?.Name ?? string.Empty;
                var canonicalSqlIdentifier = string.IsNullOrWhiteSpace(systemName)
                    ? TransformScriptSqlIdentifier.FormatParts([schemaName, item.Name])
                    : TransformScriptSqlIdentifier.FormatParts([systemName, schemaName, item.Name]);

                return new ResolvedSchemaTable(
                    item.Id,
                    systemName,
                    schemaName,
                    item.Name,
                    isTable,
                    canonicalSqlIdentifier,
                    fieldRowsBySchemaObjectId.GetValueOrDefault(item.Id) ?? []);
            })
            .ToArray();

        tablesById = tables.ToDictionary(item => item.TableId, StringComparer.Ordinal);
    }

    public bool TryGetTableById(string tableId, out ResolvedSchemaTable? table)
    {
        if (string.IsNullOrWhiteSpace(tableId))
        {
            table = null;
            return false;
        }

        return tablesById.TryGetValue(tableId, out table);
    }

    public SchemaTableResolutionResult ResolveIdentifierParts(IReadOnlyList<string> identifierParts)
    {
        ArgumentNullException.ThrowIfNull(identifierParts);

        var normalizedParts = identifierParts
            .Select(NormalizeIdentifierPart)
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .ToArray();

        return ResolveCore(normalizedParts);
    }

    public SchemaTableResolutionResult ResolveSqlIdentifier(string? sqlIdentifier)
    {
        if (string.IsNullOrWhiteSpace(sqlIdentifier))
        {
            return new SchemaTableResolutionResult(
                [],
                string.Empty,
                null,
                SchemaTableResolutionFailureKind.MissingIdentifier);
        }

        if (!TransformScriptSqlIdentifier.TryParseParts(sqlIdentifier, out var parsedParts))
        {
            return new SchemaTableResolutionResult(
                [],
                sqlIdentifier.Trim(),
                null,
                SchemaTableResolutionFailureKind.UnsupportedIdentifierShape);
        }

        var normalizedParts = parsedParts
            .Select(NormalizeIdentifierPart)
            .ToArray();

        if (normalizedParts.Any(string.IsNullOrWhiteSpace))
        {
            return new SchemaTableResolutionResult(
                normalizedParts,
                sqlIdentifier.Trim(),
                null,
                SchemaTableResolutionFailureKind.MissingIdentifier);
        }

        return ResolveCore(normalizedParts);
    }

    private SchemaTableResolutionResult ResolveCore(IReadOnlyList<string> identifierParts)
    {
        if (identifierParts.Count == 0)
        {
            return new SchemaTableResolutionResult(
                identifierParts,
                string.Empty,
                null,
                SchemaTableResolutionFailureKind.MissingIdentifier);
        }

        var displayIdentifier = string.Join(".", identifierParts);
        ResolvedSchemaTable[] matches;

        switch (identifierParts.Count)
        {
            case 1:
                matches = tables
                    .Where(item => string.Equals(item.TableName, identifierParts[0], StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                break;

            case 2:
                matches = tables
                    .Where(item =>
                        string.Equals(item.SchemaName, identifierParts[0], StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(item.TableName, identifierParts[1], StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                break;

            case 3:
                matches = tables
                    .Where(item =>
                        string.Equals(item.SystemName, identifierParts[0], StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(item.SchemaName, identifierParts[1], StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(item.TableName, identifierParts[2], StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                break;

            default:
                return new SchemaTableResolutionResult(
                    identifierParts,
                    displayIdentifier,
                    null,
                    SchemaTableResolutionFailureKind.UnsupportedIdentifierShape);
        }

        return matches.Length switch
        {
            0 => new SchemaTableResolutionResult(
                identifierParts,
                displayIdentifier,
                null,
                SchemaTableResolutionFailureKind.NotFound),
            > 1 => new SchemaTableResolutionResult(
                identifierParts,
                displayIdentifier,
                null,
                SchemaTableResolutionFailureKind.Ambiguous),
            _ => new SchemaTableResolutionResult(
                identifierParts,
                displayIdentifier,
                matches[0],
                SchemaTableResolutionFailureKind.None)
        };
    }

    private static string NormalizeIdentifierPart(string value)
    {
        var trimmed = value.Trim();
        return trimmed.Length >= 2 && trimmed[0] == '[' && trimmed[^1] == ']'
            ? trimmed[1..^1].Trim()
            : trimmed;
    }

    private static int ParseOrdinal(string? ordinal)
    {
        return int.TryParse(ordinal, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : int.MaxValue;
    }

    private static bool IsTrue(string? value)
    {
        return string.Equals(value?.Trim(), "true", StringComparison.OrdinalIgnoreCase);
    }

    private static bool? ParseOptionalBool(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (bool.TryParse(value.Trim(), out var parsed))
        {
            return parsed;
        }

        return null;
    }

    private static (int? Length, int? Precision, int? Scale) ResolveDataTypeDetail(
        IGrouping<string, FieldDataTypeDetail> details)
    {
        int? length = null;
        int? precision = null;
        int? scale = null;

        foreach (var detail in details)
        {
            if (string.Equals(detail.Name, "Length", StringComparison.OrdinalIgnoreCase))
            {
                length = ParseOptionalInt(detail.Value);
            }
            else if (string.Equals(detail.Name, "Precision", StringComparison.OrdinalIgnoreCase))
            {
                precision = ParseOptionalInt(detail.Value);
            }
            else if (string.Equals(detail.Name, "Scale", StringComparison.OrdinalIgnoreCase))
            {
                scale = ParseOptionalInt(detail.Value);
            }
        }

        return (length, precision, scale);
    }

    private static int? ParseOptionalInt(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (int.TryParse(value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
        {
            return parsed;
        }

        return null;
    }
}

internal sealed record ResolvedSchemaField(
    string FieldId,
    string FieldName,
    int Ordinal,
    string MetaDataTypeId,
    bool IsIdentity,
    bool? IsNullable,
    int? Length,
    int? Precision,
    int? Scale);

internal sealed record ResolvedSchemaTable(
    string TableId,
    string SystemName,
    string SchemaName,
    string TableName,
    bool IsWritableTable,
    string CanonicalSqlIdentifier,
    IReadOnlyList<ResolvedSchemaField> Fields);

internal sealed record SchemaTableResolutionResult(
    IReadOnlyList<string> IdentifierParts,
    string DisplayIdentifier,
    ResolvedSchemaTable? Table,
    SchemaTableResolutionFailureKind FailureKind)
{
    public bool IsResolved => Table is not null;
}

internal enum SchemaTableResolutionFailureKind
{
    None,
    MissingIdentifier,
    UnsupportedIdentifierShape,
    NotFound,
    Ambiguous
}
