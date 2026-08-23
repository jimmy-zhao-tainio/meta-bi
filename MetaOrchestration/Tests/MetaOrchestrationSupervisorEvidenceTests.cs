using MetaOrchestration.Core;

namespace MetaOrchestration.Tests;

public sealed class MetaOrchestrationSupervisorEvidenceTests
{
    [Fact]
    public void RunJournalRecordsSupervisorExceptionDetailAndState()
    {
        var root = CreateTempDirectory();
        try
        {
            var request = new OrchestrationRuntimeRequest(
                WorkspacePath: Path.Combine(root, "OrchestrationWS"),
                PipelineWorkspacePath: Path.Combine(root, "PipelineWS"),
                TransformWorkspacePath: Path.Combine(root, "TransformWS"),
                BindingWorkspacePath: Path.Combine(root, "BindingWS"),
                DataTypeConversionWorkspacePath: string.Empty,
                PipelineDbConnectionEnvironmentVariableName: string.Empty,
                MaxDegreeOfParallelism: 1,
                RunArtifactsRootPath: root);
            var runId = Guid.NewGuid();
            var journal = OrchestrationRunJournal.Start(runId, request, request.WorkspacePath);
            var state = new OrchestrationSupervisorRunState(runId, request.WorkspacePath);
            state.SetPhase("Executing");
            state.SetRunPlan("DefaultRunPlan", 7);
            state.SetRuntimeCounts(pending: 3, ready: 2, running: 1, retryScheduled: 1, completed: 4, blocked: 1);
            state.SetLiveWorkers(["Extract", "Warehouse"]);
            state.NoteEvent("TaskStarted", "Warehouse.load-fact");

            journal.WriteException("SupervisorException", new InvalidOperationException("state went sideways"));
            journal.WriteEvent("SupervisorState", "exception", state.Describe());

            var eventsPath = Directory.GetFiles(Path.Combine(root, "runs"), "events.tsv", SearchOption.AllDirectories)
                .Single();
            var events = File.ReadAllText(eventsPath);

            Assert.Contains("SupervisorException", events, StringComparison.Ordinal);
            Assert.Contains("SupervisorExceptionDetail", events, StringComparison.Ordinal);
            Assert.Contains("state went sideways", events, StringComparison.Ordinal);
            Assert.Contains("Phase=Executing", events, StringComparison.Ordinal);
            Assert.Contains("Pending=3", events, StringComparison.Ordinal);
            Assert.Contains("Ready=2", events, StringComparison.Ordinal);
            Assert.Contains("Running=1", events, StringComparison.Ordinal);
            Assert.Contains("RetryScheduled=1", events, StringComparison.Ordinal);
            Assert.Contains("LiveWorkers=Extract,Warehouse", events, StringComparison.Ordinal);
            Assert.Contains("LastEvent=TaskStarted:Warehouse.load-fact", events, StringComparison.Ordinal);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static string CreateTempDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), "MetaOrchestration.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }
}
