using MetaTransformScript.Instance;
using MTS = global::MetaTransformScript;

namespace MetaTransformScript.Sql;

public sealed partial class MetaTransformScriptSqlService
{
    public async Task<UpdateTargetIdentifiersFromPatternResult> UpdateTargetIdentifiersFromPatternAsync(
        string workspacePath,
        string sourcePattern,
        string targetPattern,
        bool onlyMissing = false,
        bool dryRun = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourcePattern);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetPattern);
        cancellationToken.ThrowIfCancellationRequested();

        var compiledSourcePattern = TransformScriptTargetIdentifierSourcePattern.Compile(sourcePattern);
        TransformScriptTargetIdentifierPattern.ValidatePattern(targetPattern, "target-pattern");

        var workspaceFullPath = Path.GetFullPath(workspacePath);
        var model = await MetaTransformScriptInstance
            .LoadFromWorkspaceAsync(workspaceFullPath, searchUpward: false, cancellationToken)
            .ConfigureAwait(false);

        var updates = new List<TargetIdentifierPatternUpdate>();
        var matchedCount = 0;
        var skippedExistingCount = 0;
        var unchangedCount = 0;

        foreach (var script in model.TransformScriptList)
        {
            if (!compiledSourcePattern.TryMatch(script.Name, out var captures))
            {
                continue;
            }

            matchedCount++;
            var scriptObjectType = ResolveScriptObjectType(model, script);
            if (scriptObjectType != ScriptObjectType.View)
            {
                throw new InvalidOperationException(
                    $"Transform script '{script.Name}' matched the source pattern but is a {FormatScriptObjectType(scriptObjectType)}; target identifiers can only be set on view scripts.");
            }

            var previousTargetSqlIdentifier = TryGetScriptObjectView(model, script.Id)?.TargetSqlIdentifier;
            if (onlyMissing && !string.IsNullOrWhiteSpace(previousTargetSqlIdentifier))
            {
                skippedExistingCount++;
                continue;
            }

            var targetSqlIdentifier = NormalizeTargetSqlIdentifier(
                TransformScriptTargetIdentifierPattern.Render(targetPattern, captures));

            if (string.Equals(previousTargetSqlIdentifier, targetSqlIdentifier, StringComparison.Ordinal))
            {
                unchangedCount++;
                continue;
            }

            updates.Add(new TargetIdentifierPatternUpdate(
                script.Id,
                script.Name,
                string.IsNullOrWhiteSpace(previousTargetSqlIdentifier) ? null : previousTargetSqlIdentifier,
                targetSqlIdentifier));

            if (!dryRun)
            {
                EnsureScriptObjectView(model, script, targetSqlIdentifier);
            }
        }

        if (!dryRun && updates.Count != 0)
        {
            await MetaTransformScriptInstance
                .SaveToWorkspaceAsync(model, workspaceFullPath, cancellationToken)
                .ConfigureAwait(false);
        }

        return new UpdateTargetIdentifiersFromPatternResult(
            workspaceFullPath,
            model.TransformScriptList.Count,
            matchedCount,
            updates.Count,
            skippedExistingCount,
            unchangedCount,
            dryRun,
            updates);
    }

    private static string FormatScriptObjectType(ScriptObjectType scriptObjectType) =>
        scriptObjectType switch
        {
            ScriptObjectType.InlineTableValuedFunction => "table-valued function",
            ScriptObjectType.ScalarFunction => "scalar function",
            ScriptObjectType.StoredProcedure => "stored procedure",
            ScriptObjectType.RawStatement => "raw statement",
            _ => scriptObjectType.ToString()
        };
}

public sealed record UpdateTargetIdentifiersFromPatternResult(
    string WorkspacePath,
    int ScriptCount,
    int MatchedCount,
    int UpdatedCount,
    int SkippedExistingCount,
    int UnchangedCount,
    bool DryRun,
    IReadOnlyList<TargetIdentifierPatternUpdate> Updates);

public sealed record TargetIdentifierPatternUpdate(
    string TransformScriptId,
    string TransformScriptName,
    string? PreviousTargetSqlIdentifier,
    string TargetSqlIdentifier);

internal sealed class TransformScriptTargetIdentifierSourcePattern
{
    private readonly string pattern;
    private readonly TransformScriptTargetIdentifierPatternPart[] parts;

    private TransformScriptTargetIdentifierSourcePattern(
        string pattern,
        TransformScriptTargetIdentifierPatternPart[] parts)
    {
        this.pattern = pattern;
        this.parts = parts;
    }

    public static TransformScriptTargetIdentifierSourcePattern Compile(string pattern)
    {
        TransformScriptTargetIdentifierPattern.ValidatePattern(pattern, "source-pattern");
        var parts = pattern
            .Split('.', StringSplitOptions.None)
            .Select(TransformScriptTargetIdentifierPatternPart.Compile)
            .ToArray();

        if (parts.Length == 0 || parts.Any(static part => string.IsNullOrWhiteSpace(part.Pattern)))
        {
            throw new InvalidOperationException("source-pattern must contain non-empty identifier parts.");
        }

        return new TransformScriptTargetIdentifierSourcePattern(pattern, parts);
    }

    public bool TryMatch(
        string transformScriptName,
        out IReadOnlyDictionary<string, string> captures)
    {
        var values = transformScriptName.Split('.', StringSplitOptions.None);
        if (values.Length != parts.Length)
        {
            captures = new Dictionary<string, string>(StringComparer.Ordinal);
            return false;
        }

        var mutableCaptures = new Dictionary<string, string>(StringComparer.Ordinal);
        for (var i = 0; i < parts.Length; i++)
        {
            if (!parts[i].TryMatch(values[i], mutableCaptures))
            {
                captures = new Dictionary<string, string>(StringComparer.Ordinal);
                return false;
            }
        }

        captures = mutableCaptures;
        return true;
    }

    public override string ToString() => pattern;
}

internal sealed class TransformScriptTargetIdentifierPatternPart
{
    private TransformScriptTargetIdentifierPatternPart(
        string pattern,
        string? token,
        string prefix,
        string suffix)
    {
        Pattern = pattern;
        Token = token;
        Prefix = prefix;
        Suffix = suffix;
    }

    public string Pattern { get; }
    public string? Token { get; }
    public string Prefix { get; }
    public string Suffix { get; }

    public static TransformScriptTargetIdentifierPatternPart Compile(string pattern)
    {
        var tokens = TransformScriptTargetIdentifierPattern.ReadTokens(pattern).ToArray();
        if (tokens.Length > 1)
        {
            throw new InvalidOperationException(
                $"source-pattern identifier part '{pattern}' contains more than one token; use at most one token per identifier part.");
        }

        if (tokens.Length == 0)
        {
            return new TransformScriptTargetIdentifierPatternPart(pattern, null, pattern, string.Empty);
        }

        var tokenText = "{" + tokens[0] + "}";
        var tokenIndex = pattern.IndexOf(tokenText, StringComparison.Ordinal);
        return new TransformScriptTargetIdentifierPatternPart(
            pattern,
            tokens[0],
            pattern[..tokenIndex],
            pattern[(tokenIndex + tokenText.Length)..]);
    }

    public bool TryMatch(
        string value,
        IDictionary<string, string> captures)
    {
        if (Token is null)
        {
            return string.Equals(Pattern, value, StringComparison.OrdinalIgnoreCase);
        }

        if (!value.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase) ||
            !value.EndsWith(Suffix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var captureLength = value.Length - Prefix.Length - Suffix.Length;
        if (captureLength <= 0)
        {
            return false;
        }

        var captured = value.Substring(Prefix.Length, captureLength);
        if (captures.TryGetValue(Token, out var existing))
        {
            return string.Equals(existing, captured, StringComparison.OrdinalIgnoreCase);
        }

        captures.Add(Token, captured);
        return true;
    }
}

internal static class TransformScriptTargetIdentifierPattern
{
    public static void ValidatePattern(string pattern, string label)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pattern);
        _ = ReadTokens(pattern).ToArray();
        if (pattern.Split('.', StringSplitOptions.None).Any(static part => string.IsNullOrWhiteSpace(part)))
        {
            throw new InvalidOperationException($"{label} must contain non-empty identifier parts.");
        }
    }

    public static string Render(
        string pattern,
        IReadOnlyDictionary<string, string> captures)
    {
        ValidatePattern(pattern, "target-pattern");
        var rendered = pattern;
        foreach (var token in ReadTokens(pattern))
        {
            if (!captures.TryGetValue(token, out var value))
            {
                throw new InvalidOperationException(
                    $"target-pattern references token '{token}', but source-pattern did not capture it.");
            }

            rendered = rendered.Replace("{" + token + "}", value, StringComparison.Ordinal);
        }

        return rendered;
    }

    public static IEnumerable<string> ReadTokens(string pattern)
    {
        var index = 0;
        while (index < pattern.Length)
        {
            var open = pattern.IndexOf('{', index);
            var closeBeforeOpen = pattern.IndexOf('}', index);
            if (closeBeforeOpen >= 0 && (open < 0 || closeBeforeOpen < open))
            {
                throw new InvalidOperationException($"pattern '{pattern}' contains an unmatched closing brace.");
            }

            if (open < 0)
            {
                yield break;
            }

            var close = pattern.IndexOf('}', open + 1);
            if (close < 0)
            {
                throw new InvalidOperationException($"pattern '{pattern}' contains an unmatched opening brace.");
            }

            var token = pattern[(open + 1)..close];
            if (!IsValidTokenName(token))
            {
                throw new InvalidOperationException(
                    $"pattern token '{token}' is invalid; use letters, digits, and underscores, starting with a letter or underscore.");
            }

            yield return token;
            index = close + 1;
        }
    }

    private static bool IsValidTokenName(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        if (!(char.IsLetter(token[0]) || token[0] == '_'))
        {
            return false;
        }

        for (var i = 1; i < token.Length; i++)
        {
            if (!(char.IsLetterOrDigit(token[i]) || token[i] == '_'))
            {
                return false;
            }
        }

        return true;
    }
}
