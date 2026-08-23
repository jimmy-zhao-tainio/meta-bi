using System.Collections.Concurrent;
using System.Globalization;
using MetaTransformScript;

namespace MetaTransformScript.Sql;

internal sealed partial class MetaTransformScriptSqlEmitter
{
    private static readonly ConcurrentDictionary<Type, string?> OwnerPropertyByType = new();
    private static readonly ConcurrentDictionary<Type, string?> BasePropertyByType = new();

    private T GetById<T>(IEnumerable<T> rows, string id, string label) where T : class
    {
        var match = index.FindById(rows, id);
        return match ?? throw new InvalidOperationException($"Required MetaTransformScript lookup '{label}' with id '{id}' was not found.");
    }

    private T GetByBaseId<T>(IEnumerable<T> rows, string baseId, string label) where T : class
    {
        var match = FindByBaseId(rows, baseId);
        return match ?? throw new InvalidOperationException($"Required MetaTransformScript base lookup '{label}' with base id '{baseId}' was not found.");
    }

    private T? FindByBaseId<T>(IEnumerable<T> rows, string baseId) where T : class
    {
        var propertyName = ResolveBaseProperty(typeof(T));
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return null;
        }

        return index.FindByRelatedId(rows, propertyName, baseId);
    }

    private TLink GetOwnerLink<TLink>(IEnumerable<TLink> links, string ownerId, string label) where TLink : class
    {
        var match = FindOwnerLink(links, ownerId);
        return match ?? throw new InvalidOperationException($"Required MetaTransformScript link '{label}' with owner id '{ownerId}' was not found.");
    }

    private TLink? FindOwnerLink<TLink>(IEnumerable<TLink> links, string ownerId) where TLink : class
    {
        var propertyName = ResolveOwnerProperty(typeof(TLink));
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return null;
        }

        return index.FindByRelatedId(links, propertyName, ownerId);
    }

    private IEnumerable<TItem> GetOrderedItems<TItem>(IEnumerable<TItem> items, string ownerId) where TItem : class
    {
        var propertyName = ResolveOwnerProperty(typeof(TItem));
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return [];
        }

        return index.GetOrderedByRelatedId(items, propertyName, ownerId);
    }

    private sealed class MetaTransformScriptSqlModelIndex
    {
        private readonly Dictionary<object, object> rowsById = new(ReferenceEqualityComparer.Instance);
        private readonly Dictionary<object, Dictionary<string, object>> rowsByRelatedId = new(ReferenceEqualityComparer.Instance);
        private readonly Dictionary<object, Dictionary<string, object>> orderedRowsByRelatedId = new(ReferenceEqualityComparer.Instance);

        public T? FindById<T>(IEnumerable<T> rows, string id) where T : class
        {
            if (!rowsById.TryGetValue(rows, out var boxedIndex))
            {
                var built = new Dictionary<string, T>(StringComparer.Ordinal);
                foreach (var row in rows)
                {
                    built.TryAdd(GetString(row, "Id"), row);
                }

                boxedIndex = built;
                rowsById.Add(rows, boxedIndex);
            }

            var typedIndex = (IReadOnlyDictionary<string, T>)boxedIndex;
            return typedIndex.GetValueOrDefault(id);
        }

        public T? FindByRelatedId<T>(
            IEnumerable<T> rows,
            string propertyName,
            string relatedId)
            where T : class
        {
            var typedIndex = GetOrCreateRelatedIndex(rows, propertyName);
            return typedIndex.GetValueOrDefault(relatedId);
        }

        public IReadOnlyList<T> GetOrderedByRelatedId<T>(
            IEnumerable<T> rows,
            string propertyName,
            string relatedId)
            where T : class
        {
            if (!orderedRowsByRelatedId.TryGetValue(rows, out var indexesByProperty))
            {
                indexesByProperty = new Dictionary<string, object>(StringComparer.Ordinal);
                orderedRowsByRelatedId.Add(rows, indexesByProperty);
            }

            if (!indexesByProperty.TryGetValue(propertyName, out var boxedIndex))
            {
                var built = rows
                    .GroupBy(row => GetRelatedId(row, propertyName), StringComparer.Ordinal)
                    .ToDictionary(
                        static group => group.Key,
                        static group => (IReadOnlyList<T>)group
                            .OrderBy(row => ParseOrdinal(GetString(row, "Ordinal")))
                            .ToArray(),
                        StringComparer.Ordinal);
                boxedIndex = built;
                indexesByProperty.Add(propertyName, boxedIndex);
            }

            var typedIndex = (IReadOnlyDictionary<string, IReadOnlyList<T>>)boxedIndex;
            return typedIndex.GetValueOrDefault(relatedId) ?? [];
        }

        private IReadOnlyDictionary<string, T> GetOrCreateRelatedIndex<T>(
            IEnumerable<T> rows,
            string propertyName)
            where T : class
        {
            if (!rowsByRelatedId.TryGetValue(rows, out var indexesByProperty))
            {
                indexesByProperty = new Dictionary<string, object>(StringComparer.Ordinal);
                rowsByRelatedId.Add(rows, indexesByProperty);
            }

            if (!indexesByProperty.TryGetValue(propertyName, out var boxedIndex))
            {
                var built = new Dictionary<string, T>(StringComparer.Ordinal);
                foreach (var row in rows)
                {
                    built.TryAdd(GetRelatedId(row, propertyName), row);
                }

                boxedIndex = built;
                indexesByProperty.Add(propertyName, boxedIndex);
            }

            return (IReadOnlyDictionary<string, T>)boxedIndex;
        }
    }

    private static string GetString(object target, string propertyName) =>
        (string?)target.GetType().GetProperty(propertyName)?.GetValue(target) ?? string.Empty;

    private static string GetRelatedId(object target, string propertyName)
    {
        var related = target.GetType().GetProperty(propertyName)?.GetValue(target);
        return related is null ? string.Empty : GetString(related, "Id");
    }

    private static int ParseOrdinal(string? ordinal) =>
        int.TryParse(ordinal, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) ? value : 0;

    private static string? ResolveOwnerProperty(Type type)
    {
        return OwnerPropertyByType.GetOrAdd(type, static key =>
        {
            var referenceProperties = key.GetProperties()
                .Select(property => property.Name)
                .Where(name => IsReferenceProperty(key, name))
                .ToArray();

            if (referenceProperties.Length == 0)
            {
                return null;
            }

            if (referenceProperties.Length == 1)
            {
                return referenceProperties[0];
            }

            foreach (var candidate in referenceProperties.OrderByDescending(static value => value.Length))
            {
                if (key.Name.StartsWith(candidate, StringComparison.Ordinal))
                {
                    return candidate;
                }
            }

            return referenceProperties[0];
        });
    }

    private static string? ResolveBaseProperty(Type type)
    {
        return BasePropertyByType.GetOrAdd(type, static key =>
        {
            return key.GetProperties()
                .Select(property => property.Name)
                .FirstOrDefault(name => IsReferenceProperty(key, name));
        });
    }

    private static bool IsReferenceProperty(Type ownerType, string propertyName)
    {
        var property = ownerType.GetProperty(propertyName);
        return property is not null &&
               property.PropertyType != typeof(string) &&
               property.PropertyType.GetProperty("Id") is not null;
    }

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
