using MetaSql.Workflow;

namespace MetaSql.App;

public sealed class MetaSqlResolutionSession
{
    public string BuildSummary(MetaSqlTargetInspection inspection)
    {
        ArgumentNullException.ThrowIfNull(inspection);

        var attentionCount = inspection.ActionableIssues.Count;
        var attentionWord = attentionCount == 1 ? "issue" : "issues";
        var attentionVerb = attentionCount == 1 ? "needs" : "need";
        var matchingScriptWord = inspection.MatchingScripts.Count == 1 ? "script already exists" : "scripts already exist";
        var lines = new List<string> { $"{attentionCount} {attentionWord} {attentionVerb} attention for target {inspection.Context.Name}." };
        if (inspection.MatchingScripts.Count > 0)
        {
            lines.Add($"{inspection.MatchingScripts.Count} matching {matchingScriptWord}.");
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
