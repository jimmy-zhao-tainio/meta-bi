using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;

internal static partial class Program
{
    private const string AppName = "meta-pipeline";
    private const string ApplicationId = "app-meta-pipeline";
    private const string CommandWorkspaceDirectoryName = "meta-pipeline.MetaCli";
    private static readonly ConsolePresenter Presenter = new();

    static int Main(string[] args)
    {
        if (CliVersion.TryWriteVersion(Presenter, AppName, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        Environment.ExitCode = 0;
        var runtime = new MetaCliRuntime<MetaPipeline.MetaPipelineModel>(CommandWorkspacePath, ApplicationId)
            .UseDefaultHelp()
            .Bind("exec-new-workspace", invocation => Complete(() => RunNewWorkspace(invocation)))
            .Bind("exec-add-pipeline", (invocation, model) => Complete(() => RunAddPipeline(invocation, model)))
            .Bind("exec-inspect", (invocation, model) => Complete(() => RunInspect(invocation, model)))
            .Bind("exec-add-step", (invocation, model) => Complete(() => RunAddStep(invocation, model)))
            .Bind("exec-add-executable-step", (invocation, model) => Complete(() => RunAddExecutableStep(invocation, model)))
            .Bind("exec-execute", (invocation, model) => CompleteAsync(() => RunExecuteAsync(invocation, model)))
            .Bind("exec-execute-step", (invocation, model) => CompleteAsync(() => RunExecuteStepAsync(invocation, model)))
            .Bind("exec-execute-worker", (invocation, model) => CompleteAsync(() => RunExecuteWorkerAsync(invocation, model)))
            .Bind("exec-execute-sqlserver", invocation => CompleteAsync(() => RunExecuteSqlServerAsync(invocation)))
            .Bind("exec-create-pipeline-db", invocation => CompleteAsync(() => RunCreatePipelineDbAsync(invocation)))
            .Bind("exec-prune-pipeline-db", invocation => CompleteAsync(() => RunPrunePipelineDbAsync(invocation)));

        runtime.Run(args);
        return Environment.ExitCode;
    }

    private static string CommandWorkspacePath =>
        Path.Combine(AppContext.BaseDirectory, CommandWorkspaceDirectoryName);

    private static void Complete(Func<int> action)
    {
        var exitCode = action();
        if (exitCode != 0)
        {
            throw new MetaCliExitException(exitCode);
        }
    }

    private static void CompleteAsync(Func<Task<int>> action)
    {
        Complete(() => action().GetAwaiter().GetResult());
    }

    private static string HelpCommand(string commandName) => $"{AppName} help {commandName}";

    private static int Fail(string message, string next, int exitCode = 1, IEnumerable<string>? details = null)
    {
        var renderedDetails = new List<string>();
        if (details != null)
        {
            renderedDetails.AddRange(details.Where(static detail => !string.IsNullOrWhiteSpace(detail)));
        }

        renderedDetails.Add($"Next: {next}");
        Presenter.WriteFailure(message, renderedDetails);
        return exitCode;
    }
}
