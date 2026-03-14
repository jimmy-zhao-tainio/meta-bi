using System.Text;

namespace MetaSql.Workflow;

public sealed class BlockerScriptStubEmitter
{
    public IReadOnlyList<string> EmitMissingStubs(string directoryPath, IReadOnlyList<Blocker> blockers)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);
        ArgumentNullException.ThrowIfNull(blockers);

        var fullPath = Path.GetFullPath(directoryPath);
        Directory.CreateDirectory(fullPath);

        var existingHeaders = new BlockerScriptCatalog().Load(fullPath);
        var existingIds = existingHeaders.Headers
            .Select(item => item.Header.BlockerId)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var emittedPaths = new List<string>();
        foreach (var blocker in blockers)
        {
            if (existingIds.Contains(blocker.Id))
            {
                continue;
            }

            var fileName = BuildFileName(blocker);
            var path = Path.Combine(fullPath, fileName);
            File.WriteAllText(path, RenderStub(blocker), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            emittedPaths.Add(path);
        }

        return emittedPaths;
    }

    private static string BuildFileName(Blocker blocker)
    {
        var safeObjectName = blocker.ObjectName
            .Replace('.', '_')
            .Replace('[', '_')
            .Replace(']', '_');
        return $"{safeObjectName}.{blocker.Id}.sql";
    }

    private static string RenderStub(Blocker blocker)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"-- meta-sql blocker-id: {blocker.Id}");
        builder.AppendLine($"-- meta-sql blocker-kind: {RenderKind(blocker.Kind)}");
        builder.AppendLine($"-- meta-sql object: {blocker.ObjectName}");
        builder.AppendLine("--");
        foreach (var reason in blocker.Reasons)
        {
            builder.AppendLine($"-- {reason}");
        }

        builder.AppendLine();
        builder.AppendLine("-- Fill in the required SQL below.");
        builder.AppendLine();
        return builder.ToString();
    }

    private static string RenderKind(BlockerKind kind)
    {
        return kind switch
        {
            BlockerKind.ManualRequired => "manual-required",
            BlockerKind.Blocked => "blocked",
            _ => throw new InvalidOperationException($"unsupported blocker kind '{kind}'.")
        };
    }
}
