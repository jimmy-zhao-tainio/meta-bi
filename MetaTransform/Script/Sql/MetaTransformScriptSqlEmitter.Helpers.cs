using System.Globalization;
using MetaTransformScript;

namespace MetaTransformScript.Sql;

internal sealed partial class MetaTransformScriptSqlEmitter
{
    private T GetById<T>(IEnumerable<T> rows, Func<T, string?> selectId, string id, string label) where T : class
    {
        var match = index.FindById(rows, selectId, id);
        return match ?? throw new InvalidOperationException($"Required MetaTransformScript lookup '{label}' with id '{id}' was not found.");
    }

    private T? FindByBaseId<T>(
        IEnumerable<T> rows,
        Func<T, string?> selectBaseId,
        string relationshipName,
        string baseId) where T : class =>
        index.FindByRelatedId(rows, selectBaseId, relationshipName, baseId);

    private TLink GetOwnerLink<TLink>(
        IEnumerable<TLink> links,
        Func<TLink, string?> selectOwnerId,
        string relationshipName,
        string ownerId,
        string label) where TLink : class
    {
        var match = FindOwnerLink(links, selectOwnerId, relationshipName, ownerId);
        return match ?? throw new InvalidOperationException($"Required MetaTransformScript link '{label}' with owner id '{ownerId}' was not found.");
    }

    private TLink? FindOwnerLink<TLink>(
        IEnumerable<TLink> links,
        Func<TLink, string?> selectOwnerId,
        string relationshipName,
        string ownerId) where TLink : class =>
        index.FindByRelatedId(links, selectOwnerId, relationshipName, ownerId);

    private IEnumerable<TItem> GetOrderedItems<TItem>(
        IEnumerable<TItem> items,
        Func<TItem, string?> selectOwnerId,
        string relationshipName,
        Func<TItem, string?> selectOrdinal,
        string ownerId) where TItem : class =>
        index.GetOrderedByRelatedId(items, selectOwnerId, relationshipName, selectOrdinal, ownerId);

    private sealed class MetaTransformScriptSqlModelIndex
    {
        private readonly Dictionary<object, object> rowsById = new(ReferenceEqualityComparer.Instance);
        private readonly Dictionary<object, Dictionary<string, object>> rowsByRelatedId = new(ReferenceEqualityComparer.Instance);
        private readonly Dictionary<object, Dictionary<string, object>> orderedRowsByRelatedId = new(ReferenceEqualityComparer.Instance);

        public T? FindById<T>(IEnumerable<T> rows, Func<T, string?> selectId, string id) where T : class
        {
            if (!rowsById.TryGetValue(rows, out var boxedIndex))
            {
                var built = new Dictionary<string, T>(StringComparer.Ordinal);
                foreach (var row in rows)
                {
                    built.TryAdd(selectId(row) ?? string.Empty, row);
                }

                boxedIndex = built;
                rowsById.Add(rows, boxedIndex);
            }

            return ((IReadOnlyDictionary<string, T>)boxedIndex).GetValueOrDefault(id);
        }

        public T? FindByRelatedId<T>(
            IEnumerable<T> rows,
            Func<T, string?> selectRelatedId,
            string relationshipName,
            string relatedId) where T : class =>
            GetOrCreateRelatedIndex(rows, selectRelatedId, relationshipName).GetValueOrDefault(relatedId);

        public IReadOnlyList<T> GetOrderedByRelatedId<T>(
            IEnumerable<T> rows,
            Func<T, string?> selectRelatedId,
            string relationshipName,
            Func<T, string?> selectOrdinal,
            string relatedId) where T : class
        {
            if (!orderedRowsByRelatedId.TryGetValue(rows, out var indexesByRelationship))
            {
                indexesByRelationship = new Dictionary<string, object>(StringComparer.Ordinal);
                orderedRowsByRelatedId.Add(rows, indexesByRelationship);
            }

            if (!indexesByRelationship.TryGetValue(relationshipName, out var boxedIndex))
            {
                var built = rows
                    .GroupBy(row => selectRelatedId(row) ?? string.Empty, StringComparer.Ordinal)
                    .ToDictionary(
                        static group => group.Key,
                        group => (IReadOnlyList<T>)group
                            .OrderBy(row => ParseOrdinal(selectOrdinal(row)))
                            .ToArray(),
                        StringComparer.Ordinal);
                boxedIndex = built;
                indexesByRelationship.Add(relationshipName, boxedIndex);
            }

            return ((IReadOnlyDictionary<string, IReadOnlyList<T>>)boxedIndex).GetValueOrDefault(relatedId) ?? [];
        }

        private IReadOnlyDictionary<string, T> GetOrCreateRelatedIndex<T>(
            IEnumerable<T> rows,
            Func<T, string?> selectRelatedId,
            string relationshipName) where T : class
        {
            if (!rowsByRelatedId.TryGetValue(rows, out var indexesByRelationship))
            {
                indexesByRelationship = new Dictionary<string, object>(StringComparer.Ordinal);
                rowsByRelatedId.Add(rows, indexesByRelationship);
            }

            // The compiler-checked relationship name partitions caches; access uses the typed selector.
            if (!indexesByRelationship.TryGetValue(relationshipName, out var boxedIndex))
            {
                var built = new Dictionary<string, T>(StringComparer.Ordinal);
                foreach (var row in rows)
                {
                    built.TryAdd(selectRelatedId(row) ?? string.Empty, row);
                }

                boxedIndex = built;
                indexesByRelationship.Add(relationshipName, boxedIndex);
            }

            return (IReadOnlyDictionary<string, T>)boxedIndex;
        }
    }

    private static int ParseOrdinal(string? ordinal) =>
        int.TryParse(ordinal, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) ? value : 0;

    private static bool IsTrue(string? value) =>
        string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);

    private static string RenderIdentifier(Identifier identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier.Value))
        {
            return "[]";
        }

        if (string.Equals(identifier.QuoteType, "SquareBracket", StringComparison.Ordinal))
        {
            return "[" + identifier.Value.Replace("]", "]]", StringComparison.Ordinal) + "]";
        }

        if (string.Equals(identifier.QuoteType, "Backtick", StringComparison.Ordinal))
        {
            return "[" + identifier.Value.Replace("]", "]]", StringComparison.Ordinal) + "]";
        }

        if (string.Equals(identifier.QuoteType, "MergeActionPseudoColumn", StringComparison.Ordinal))
        {
            return string.Equals(identifier.Value, "$action", StringComparison.OrdinalIgnoreCase)
                ? "$action"
                : throw new InvalidOperationException($"Unsupported MetaTransformScript MERGE OUTPUT pseudo-column '{identifier.Value}'.");
        }

        if (!string.IsNullOrWhiteSpace(identifier.QuoteType) &&
            !string.Equals(identifier.QuoteType, "NotQuoted", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Unsupported MetaTransformScript Identifier.QuoteType '{identifier.QuoteType}'.");
        }

        if (IsPlainIdentifier(identifier.Value))
        {
            return identifier.Value;
        }

        return "[" + identifier.Value.Replace("]", "]]", StringComparison.Ordinal) + "]";
    }

    private static bool IsPlainIdentifier(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (!(char.IsLetter(value[0]) || value[0] == '_' || value[0] == '@' || value[0] == '#'))
        {
            return false;
        }

        for (var i = 1; i < value.Length; i++)
        {
            var ch = value[i];
            if (!(char.IsLetterOrDigit(ch) || ch == '_' || ch == '@' || ch == '#' || ch == '$'))
            {
                return false;
            }
        }

        return true;
    }
}
