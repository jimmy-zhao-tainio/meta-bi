using System.Diagnostics;
using System.Text;

namespace MetaOrchestration.Core;

internal sealed class OrchestrationRunJournal
{
    private const int MaxDetailLength = 16_000;
    private readonly object gate = new();
    private readonly string eventLogPath;

    private OrchestrationRunJournal(Guid runId, string runDirectoryPath, string eventLogPath)
    {
        RunId = runId;
        RunDirectoryPath = runDirectoryPath;
        this.eventLogPath = eventLogPath;
    }

    public Guid RunId { get; }

    public string RunDirectoryPath { get; }

    public static OrchestrationRunJournal Start(
        Guid runId,
        OrchestrationRuntimeRequest request,
        string workspacePath)
    {
        var root = OrchestrationWorkspaceExecutionLease.ResolveOperationalRoot(request.RunArtifactsRootPath);
        var runDirectoryName = DateTimeOffset.UtcNow.ToString("yyyyMMdd-HHmmss", System.Globalization.CultureInfo.InvariantCulture)
                               + "_"
                               + runId.ToString("N");
        var runDirectoryPath = Path.Combine(root, "runs", runDirectoryName);
        Directory.CreateDirectory(runDirectoryPath);

        var eventLogPath = Path.Combine(runDirectoryPath, "events.tsv");
        File.WriteAllText(
            eventLogPath,
            "OccurredAtUtc\tEventKind\tSubject\tDetail" + Environment.NewLine,
            Encoding.UTF8);

        var journal = new OrchestrationRunJournal(runId, runDirectoryPath, eventLogPath);
        journal.WriteEvent("RunStarted", "orchestration", workspacePath);
        journal.WriteEvent("Process", Process.GetCurrentProcess().ProcessName, Environment.ProcessId.ToString(System.Globalization.CultureInfo.InvariantCulture));
        journal.WriteEvent("PipelineExecutable", request.PipelineExecutableName, string.Empty);
        journal.WriteEvent("PipelineWorkspace", Path.GetFullPath(request.PipelineWorkspacePath), string.Empty);
        journal.WriteEvent("TransformWorkspace", Path.GetFullPath(request.TransformWorkspacePath), string.Empty);
        journal.WriteEvent("BindingWorkspace", Path.GetFullPath(request.BindingWorkspacePath), string.Empty);
        if (!string.IsNullOrWhiteSpace(request.DataTypeConversionWorkspacePath))
        {
            journal.WriteEvent("DataTypeConversionWorkspace", Path.GetFullPath(request.DataTypeConversionWorkspacePath), string.Empty);
        }

        if (!string.IsNullOrWhiteSpace(request.PipelineDbConnectionEnvironmentVariableName))
        {
            journal.WriteEvent("PipelineDbConnectionEnv", request.PipelineDbConnectionEnvironmentVariableName, "value not persisted");
        }

        return journal;
    }

    public void WriteEvent(string eventKind, string subject, string detail)
    {
        var line = string.Join(
            "\t",
            DateTimeOffset.UtcNow.ToString("O"),
            Escape(eventKind),
            Escape(subject),
            Escape(TrimDetail(detail)));
        lock (gate)
        {
            File.AppendAllText(eventLogPath, line + Environment.NewLine, Encoding.UTF8);
        }
    }

    public void WriteException(string eventKind, Exception exception)
    {
        WriteEvent(eventKind, exception.GetType().FullName ?? exception.GetType().Name, exception.Message);
        WriteEvent(eventKind + "Detail", exception.GetType().Name, exception.ToString());
    }

    private static string Escape(string? value) =>
        (value ?? string.Empty)
        .Replace("\t", " ", StringComparison.Ordinal)
        .ReplaceLineEndings(" ");

    private static string TrimDetail(string? value)
    {
        if (string.IsNullOrEmpty(value) ||
            value.Length <= MaxDetailLength)
        {
            return value ?? string.Empty;
        }

        return value[..MaxDetailLength] + "... <truncated>";
    }
}
