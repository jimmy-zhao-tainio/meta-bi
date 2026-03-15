namespace MetaSql.Workflow;

public sealed class BlockerScriptCatalog
{
    public BlockerScriptCatalogResult Load(string directoryPath, IReadOnlyList<Blocker>? currentBlockers = null)
    {
        return Load([directoryPath], currentBlockers);
    }

    public BlockerScriptCatalogResult Load(IEnumerable<string> directoryPaths, IReadOnlyList<Blocker>? currentBlockers = null)
    {
        ArgumentNullException.ThrowIfNull(directoryPaths);

        var fullPaths = directoryPaths
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Select(Path.GetFullPath)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (fullPaths.Length == 0)
        {
            return new BlockerScriptCatalogResult([], [], []);
        }

        var loader = new BlockerScriptLoader();
        var headers = fullPaths
            .Where(Directory.Exists)
            .SelectMany(path => Directory.EnumerateFiles(path, "*.sql", SearchOption.TopDirectoryOnly))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ThenBy(path => path, StringComparer.Ordinal)
            .Select(path => ToBlockerScriptFile(loader.Load(path, requireSqlBody: true)))
            .ToArray();

        if (currentBlockers == null)
        {
            return new BlockerScriptCatalogResult(headers, [], []);
        }

        var blockersById = currentBlockers.ToDictionary(item => item.Id, StringComparer.OrdinalIgnoreCase);
        var currentIds = blockersById.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var matched = headers.Where(item => currentIds.Contains(item.Header.BlockerId)).ToArray();
        var stale = headers.Where(item => !currentIds.Contains(item.Header.BlockerId)).ToArray();
        foreach (var script in matched)
        {
            var blocker = blockersById[script.Header.BlockerId];
            if (script.Header.Kind != blocker.Kind ||
                !string.Equals(script.Header.ObjectName, blocker.ObjectName, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"sql script '{script.Path}' is invalid: header does not match the current blocker for '{blocker.ObjectName}'.");
            }
        }

        return new BlockerScriptCatalogResult(headers, matched, stale);
    }

    private static BlockerScriptFile ToBlockerScriptFile(BlockerScriptDocument document)
    {
        return new BlockerScriptFile(document.Path, document.Header);
    }
}

public sealed record BlockerScriptFile(string Path, BlockerScriptHeader Header);

public sealed record BlockerScriptCatalogResult(
    IReadOnlyList<BlockerScriptFile> Headers,
    IReadOnlyList<BlockerScriptFile> Matched,
    IReadOnlyList<BlockerScriptFile> Stale);
