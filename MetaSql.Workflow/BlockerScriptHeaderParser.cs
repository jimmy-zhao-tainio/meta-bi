namespace MetaSql.Workflow;

public sealed class BlockerScriptHeaderParser
{
    public BlockerScriptHeader Parse(string sql)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sql);

        string? blockerId = null;
        string? objectName = null;
        string? blockerKind = null;
        foreach (var rawLine in sql.Split(["\r\n", "\n"], StringSplitOptions.None).Take(10))
        {
            var line = rawLine.Trim();
            if (!line.StartsWith("--", StringComparison.Ordinal))
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    break;
                }

                continue;
            }

            if (TryReadHeaderValue(line, "-- meta-sql blocker-id:", out var value))
            {
                blockerId = value;
                continue;
            }

            if (TryReadHeaderValue(line, "-- meta-sql blocker-kind:", out value))
            {
                blockerKind = value;
                continue;
            }

            if (TryReadHeaderValue(line, "-- meta-sql object:", out value))
            {
                objectName = value;
            }
        }

        if (string.IsNullOrWhiteSpace(blockerId) ||
            string.IsNullOrWhiteSpace(blockerKind) ||
            string.IsNullOrWhiteSpace(objectName))
        {
            throw new InvalidOperationException("required header lines are missing or incomplete.");
        }

        return new BlockerScriptHeader(
            blockerId,
            ParseKind(blockerKind),
            objectName);
    }

    private static bool TryReadHeaderValue(string line, string prefix, out string value)
    {
        if (line.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            value = line[prefix.Length..].Trim();
            return true;
        }

        value = string.Empty;
        return false;
    }

    private static BlockerKind ParseKind(string value)
    {
        return value.Trim().ToLowerInvariant() switch
        {
            "manual-required" => BlockerKind.ManualRequired,
            "blocked" => BlockerKind.Blocked,
            _ => throw new InvalidOperationException($"unsupported blocker kind '{value}'.")
        };
    }
}
