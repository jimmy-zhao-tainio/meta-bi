using System.Collections.Concurrent;
using System.Globalization;
using MetaTransformScript;

namespace MetaTransformScript.Sql;

internal sealed partial class MetaTransformScriptSqlEmitter
{
    private static readonly ConcurrentDictionary<Type, string?> OwnerIdPropertyByType = new();
    private static readonly ConcurrentDictionary<Type, string?> BaseIdPropertyByType = new();

    private static T GetById<T>(IEnumerable<T> rows, string id, string label) where T : class
    {
        var match = rows.FirstOrDefault(row => string.Equals(GetString(row, "Id"), id, StringComparison.Ordinal));
        return match ?? throw new InvalidOperationException($"Required MetaTransformScript lookup '{label}' with id '{id}' was not found.");
    }

    private static T GetByBaseId<T>(IEnumerable<T> rows, string baseId, string label) where T : class
    {
        var match = FindByBaseId(rows, baseId);
        return match ?? throw new InvalidOperationException($"Required MetaTransformScript base lookup '{label}' with base id '{baseId}' was not found.");
    }

    private static T? FindByBaseId<T>(IEnumerable<T> rows, string baseId) where T : class
    {
        var propertyName = ResolveBaseIdProperty(typeof(T));
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return null;
        }

        return rows.FirstOrDefault(row => string.Equals(GetString(row, propertyName), baseId, StringComparison.Ordinal));
    }

    private static TLink GetOwnerLink<TLink>(IEnumerable<TLink> links, string ownerId, string label) where TLink : class
    {
        var match = FindOwnerLink(links, ownerId);
        return match ?? throw new InvalidOperationException($"Required MetaTransformScript link '{label}' with owner id '{ownerId}' was not found.");
    }

    private static TLink? FindOwnerLink<TLink>(IEnumerable<TLink> links, string ownerId) where TLink : class
    {
        var propertyName = ResolveOwnerIdProperty(typeof(TLink));
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return null;
        }

        return links.FirstOrDefault(row => string.Equals(GetString(row, propertyName), ownerId, StringComparison.Ordinal));
    }

    private static IEnumerable<TItem> GetOrderedItems<TItem>(IEnumerable<TItem> items, string ownerId) where TItem : class
    {
        var propertyName = ResolveOwnerIdProperty(typeof(TItem));
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return [];
        }

        return items.Where(row => string.Equals(GetString(row, propertyName), ownerId, StringComparison.Ordinal))
            .OrderBy(row => ParseOrdinal(GetString(row, "Ordinal")));
    }

    private static string GetString(object target, string propertyName) =>
        (string?)target.GetType().GetProperty(propertyName)?.GetValue(target) ?? string.Empty;

    private static int ParseOrdinal(string ordinal) =>
        int.TryParse(ordinal, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) ? value : 0;

    private static string? ResolveOwnerIdProperty(Type type)
    {
        return OwnerIdPropertyByType.GetOrAdd(type, static key =>
        {
            var idProperties = key.GetProperties()
                .Select(property => property.Name)
                .Where(static name => name.EndsWith("Id", StringComparison.Ordinal) && !string.Equals(name, "Id", StringComparison.Ordinal))
                .ToArray();

            if (idProperties.Length == 0)
            {
                return null;
            }

            if (idProperties.Length == 1)
            {
                return idProperties[0];
            }

            foreach (var candidate in idProperties.OrderByDescending(static value => value.Length))
            {
                var candidateTypeName = candidate[..^2];
                if (key.Name.StartsWith(candidateTypeName, StringComparison.Ordinal))
                {
                    return candidate;
                }
            }

            return idProperties[0];
        });
    }

    private static string? ResolveBaseIdProperty(Type type)
    {
        return BaseIdPropertyByType.GetOrAdd(type, static key =>
        {
            return key.GetProperties()
                .Select(property => property.Name)
                .FirstOrDefault(static name => name.EndsWith("Id", StringComparison.Ordinal) && !string.Equals(name, "Id", StringComparison.Ordinal));
        });
    }

    private static bool IsTrue(string value) =>
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
