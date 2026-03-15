using System.Text;

namespace MetaSql.Workflow;

public sealed class BlockerScriptLoader
{
    public BlockerScriptDocument Load(string path, bool requireSqlBody)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var fullPath = Path.GetFullPath(path);
        var sql = DecodeUtf8(fullPath);
        ValidateCharacters(sql, fullPath);

        BlockerScriptHeader header;
        try
        {
            header = new BlockerScriptHeaderParser().Parse(sql);
        }
        catch (InvalidOperationException exception)
        {
            throw new InvalidOperationException($"sql script '{fullPath}' is invalid: {exception.Message}", exception);
        }
        var sqlBody = ExtractSqlBody(sql);
        if (requireSqlBody && string.IsNullOrWhiteSpace(sqlBody))
        {
            throw new InvalidOperationException($"sql script '{fullPath}' is invalid: it has no SQL body after the required header.");
        }

        if (sql.Contains("Fill in the required SQL below.", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"sql script '{fullPath}' is invalid: it still contains the default stub placeholder text.");
        }

        return new BlockerScriptDocument(fullPath, header, sqlBody);
    }

    private static string DecodeUtf8(string fullPath)
    {
        try
        {
            var text = new UTF8Encoding(false, true).GetString(File.ReadAllBytes(fullPath));
            return text.Length > 0 && text[0] == '\uFEFF'
                ? text[1..]
                : text;
        }
        catch (DecoderFallbackException)
        {
            throw new InvalidOperationException($"sql script '{fullPath}' is invalid: file encoding must be UTF-8 or ASCII.");
        }
    }

    private static void ValidateCharacters(string sql, string fullPath)
    {
        for (var i = 0; i < sql.Length; i++)
        {
            var character = sql[i];
            if (character == '\r' || character == '\n' || character == '\t')
            {
                continue;
            }

            if (character == '\uFEFF' || character == '\0' || char.IsControl(character))
            {
                throw new InvalidOperationException(
                    $"sql script '{fullPath}' is invalid: unsupported hidden or control character at position {i + 1}.");
            }
        }
    }

    private static string ExtractSqlBody(string sql)
    {
        var lines = sql.Split(["\r\n", "\n"], StringSplitOptions.None);
        return string.Join(
                Environment.NewLine,
                lines.Where(line =>
                {
                    var trimmed = line.Trim();
                    return !string.IsNullOrWhiteSpace(trimmed) && !trimmed.StartsWith("--", StringComparison.Ordinal);
                }))
            .Trim();
    }
}
