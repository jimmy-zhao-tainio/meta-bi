namespace MetaTransform.Binding;

internal enum SourceSqlIdentifierExpansionFailureKind
{
    None,
    MissingIdentifier,
    MissingExecuteSystem,
    MissingDefaultSchemaName,
    UnsupportedIdentifierShape
}

internal readonly record struct SourceSqlIdentifierExpansionResult(
    bool IsExpanded,
    string ExpandedSqlIdentifier,
    IReadOnlyList<string> ExpandedIdentifierParts,
    SourceSqlIdentifierExpansionFailureKind FailureKind)
{
    public bool IsSuccess => FailureKind == SourceSqlIdentifierExpansionFailureKind.None;
}

internal static class SourceSqlIdentifierExpansion
{
    public static SourceSqlIdentifierExpansionResult Expand(
        string sqlIdentifier,
        string? executeSystemName,
        string? executeSystemDefaultSchemaName)
    {
        var parse = Parse(sqlIdentifier);
        if (!parse.IsSuccess)
        {
            return new SourceSqlIdentifierExpansionResult(
                IsExpanded: false,
                ExpandedSqlIdentifier: string.Empty,
                ExpandedIdentifierParts: [],
                FailureKind: parse.FailureKind);
        }

        var normalizedExecuteSystemName = NormalizeIdentifierPart(executeSystemName);
        var normalizedDefaultSchemaName = NormalizeIdentifierPart(executeSystemDefaultSchemaName);
        var parts = parse.IdentifierParts;

        switch (parts.Count)
        {
            case 1:
                if (string.IsNullOrWhiteSpace(normalizedExecuteSystemName))
                {
                    return new SourceSqlIdentifierExpansionResult(
                        IsExpanded: false,
                        ExpandedSqlIdentifier: string.Empty,
                        ExpandedIdentifierParts: [],
                        FailureKind: SourceSqlIdentifierExpansionFailureKind.MissingExecuteSystem);
                }

                if (string.IsNullOrWhiteSpace(normalizedDefaultSchemaName))
                {
                    return new SourceSqlIdentifierExpansionResult(
                        IsExpanded: false,
                        ExpandedSqlIdentifier: string.Empty,
                        ExpandedIdentifierParts: [],
                        FailureKind: SourceSqlIdentifierExpansionFailureKind.MissingDefaultSchemaName);
                }

                return Success([normalizedExecuteSystemName, normalizedDefaultSchemaName, parts[0]]);

            case 2:
                if (string.IsNullOrWhiteSpace(normalizedExecuteSystemName))
                {
                    return new SourceSqlIdentifierExpansionResult(
                        IsExpanded: false,
                        ExpandedSqlIdentifier: string.Empty,
                        ExpandedIdentifierParts: [],
                        FailureKind: SourceSqlIdentifierExpansionFailureKind.MissingExecuteSystem);
                }

                return Success([normalizedExecuteSystemName, parts[0], parts[1]]);

            case 3:
                return Success([parts[0], parts[1], parts[2]]);

            default:
                return new SourceSqlIdentifierExpansionResult(
                    IsExpanded: false,
                    ExpandedSqlIdentifier: string.Empty,
                    ExpandedIdentifierParts: [],
                    FailureKind: SourceSqlIdentifierExpansionFailureKind.UnsupportedIdentifierShape);
        }
    }

    public static bool TryGetPartCount(string sqlIdentifier, out int partCount)
    {
        var parse = Parse(sqlIdentifier);
        if (!parse.IsSuccess)
        {
            partCount = 0;
            return false;
        }

        partCount = parse.IdentifierParts.Count;
        return true;
    }

    private static SourceSqlIdentifierParseResult Parse(string sqlIdentifier)
    {
        if (string.IsNullOrWhiteSpace(sqlIdentifier))
        {
            return SourceSqlIdentifierParseResult.Failure(SourceSqlIdentifierExpansionFailureKind.MissingIdentifier);
        }

        var rawParts = sqlIdentifier
            .Split('.', StringSplitOptions.TrimEntries);
        if (rawParts.Length == 0)
        {
            return SourceSqlIdentifierParseResult.Failure(SourceSqlIdentifierExpansionFailureKind.MissingIdentifier);
        }

        var parts = new List<string>(rawParts.Length);
        foreach (var rawPart in rawParts)
        {
            var normalizedPart = NormalizeIdentifierPart(rawPart);
            if (string.IsNullOrWhiteSpace(normalizedPart))
            {
                return SourceSqlIdentifierParseResult.Failure(SourceSqlIdentifierExpansionFailureKind.MissingIdentifier);
            }

            parts.Add(normalizedPart);
        }

        if (parts.Count is < 1 or > 3)
        {
            return SourceSqlIdentifierParseResult.Failure(SourceSqlIdentifierExpansionFailureKind.UnsupportedIdentifierShape);
        }

        return SourceSqlIdentifierParseResult.Success(parts);
    }

    private static SourceSqlIdentifierExpansionResult Success(IReadOnlyList<string> parts)
    {
        return new SourceSqlIdentifierExpansionResult(
            IsExpanded: true,
            ExpandedSqlIdentifier: string.Join(".", parts),
            ExpandedIdentifierParts: parts,
            FailureKind: SourceSqlIdentifierExpansionFailureKind.None);
    }

    private static string NormalizeIdentifierPart(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var trimmed = value.Trim();
        return trimmed.Length >= 2 && trimmed[0] == '[' && trimmed[^1] == ']'
            ? trimmed[1..^1].Trim()
            : trimmed;
    }

    private readonly record struct SourceSqlIdentifierParseResult(
        bool IsSuccess,
        IReadOnlyList<string> IdentifierParts,
        SourceSqlIdentifierExpansionFailureKind FailureKind)
    {
        public static SourceSqlIdentifierParseResult Success(IReadOnlyList<string> identifierParts) =>
            new(true, identifierParts, SourceSqlIdentifierExpansionFailureKind.None);

        public static SourceSqlIdentifierParseResult Failure(SourceSqlIdentifierExpansionFailureKind failureKind) =>
            new(false, [], failureKind);
    }
}
