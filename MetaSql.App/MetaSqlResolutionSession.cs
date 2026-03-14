using MetaSql.Workflow;

namespace MetaSql.App;

public sealed class MetaSqlResolutionSession
{
    public string BuildSummary(MetaSqlTargetInspection inspection)
    {
        ArgumentNullException.ThrowIfNull(inspection);

        var blockedCount = inspection.AllBlockers.Count(item => item.Kind == BlockerKind.Blocked);
        var attentionCount = inspection.ActionableBlockers.Count + inspection.StaleScripts.Count;
        var attentionWord = attentionCount == 1 ? "issue" : "issues";
        var blockedWord = blockedCount == 1 ? "item is" : "items are";
        var matchingWord = inspection.MatchingScripts.Count == 1 ? "has" : "have";
        var staleWord = inspection.StaleScripts.Count == 1 ? "has" : "have";
        var lines = new List<string>
        {
            $"Found {attentionCount} {attentionWord} requiring attention for target {inspection.Context.Name}.",
            $"{inspection.MatchingScripts.Count} already {matchingWord} matching scripts.",
            $"{inspection.StaleScripts.Count} {staleWord} stale scripts."
        };
        if (blockedCount > 0)
        {
            lines.Add($"{blockedCount} additional {blockedWord} currently blocked.");
        }

        return string.Join(Environment.NewLine, lines);
    }

    public string CreateBaselineStub(MetaSqlTargetInspection inspection, Blocker blocker)
    {
        ArgumentNullException.ThrowIfNull(inspection);
        ArgumentNullException.ThrowIfNull(blocker);

        Directory.CreateDirectory(inspection.Context.BaselinePath);
        return EmitSingleStub(inspection.Context.BaselinePath, blocker);
    }

    public string CreateTargetStub(MetaSqlTargetInspection inspection, Blocker blocker)
    {
        ArgumentNullException.ThrowIfNull(inspection);
        ArgumentNullException.ThrowIfNull(blocker);

        Directory.CreateDirectory(inspection.Context.TargetPath);
        return EmitSingleStub(inspection.Context.TargetPath, blocker);
    }

    public string ArchiveStaleScript(MetaSqlTargetInspection inspection, BlockerScriptFile staleScript)
    {
        ArgumentNullException.ThrowIfNull(inspection);
        ArgumentNullException.ThrowIfNull(staleScript);

        Directory.CreateDirectory(inspection.Context.ArchivePath);
        var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Path.GetFileName(staleScript.Path)}";
        var destinationPath = Path.Combine(inspection.Context.ArchivePath, fileName);
        File.Move(staleScript.Path, destinationPath);
        return destinationPath;
    }

    private static string EmitSingleStub(string directoryPath, Blocker blocker)
    {
        var emitter = new BlockerScriptStubEmitter();
        var emitted = emitter.EmitMissingStubs(directoryPath, [blocker]);
        return emitted.Single();
    }
}
