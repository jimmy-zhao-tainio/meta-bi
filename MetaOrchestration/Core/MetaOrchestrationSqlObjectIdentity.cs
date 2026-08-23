using MetaTransformScript;

namespace MetaOrchestration.Core;

internal static class MetaOrchestrationSqlObjectIdentity
{
    private const int MaximumPartCount = 4;

    public static string NormalizeKey(string sqlIdentifier)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sqlIdentifier);

        return string.Join(".", ParseParts(sqlIdentifier).Select(CanonicalizePart));
    }

    private static IReadOnlyList<string> ParseParts(string sqlIdentifier)
    {
        if (!TransformScriptSqlIdentifier.TryParseParts(sqlIdentifier, out var parts, out var failureReason))
        {
            throw InvalidIdentifier(sqlIdentifier, failureReason ?? "is invalid");
        }

        if (parts.Count > MaximumPartCount)
        {
            throw InvalidIdentifier(sqlIdentifier, $"uses {parts.Count} parts; expected between one and {MaximumPartCount}");
        }

        return parts;
    }

    private static string CanonicalizePart(string part)
    {
        var normalized = part.ToUpperInvariant();
        return IsOrdinaryIdentifier(normalized)
            ? normalized
            : "[" + normalized.Replace("]", "]]", StringComparison.Ordinal) + "]";
    }

    private static bool IsOrdinaryIdentifier(string part)
    {
        if (part.Length == 0 || !(char.IsLetter(part[0]) || part[0] is '_' or '@' or '#'))
        {
            return false;
        }

        return part.Skip(1).All(static character =>
            char.IsLetterOrDigit(character) || character is '_' or '@' or '#' or '$');
    }

    private static InvalidOperationException InvalidIdentifier(string sqlIdentifier, string reason) =>
        new($"SQL object identifier '{sqlIdentifier}' {reason}.");

}
