using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaTransform.Binding;
using MetaTransformBinding;

internal static class Program
{
    private const string AppName = "meta-transform-binding";
    private const string ApplicationId = "app-meta-transform-binding";
    private const string CommandWorkspaceDirectoryName = "meta-transform-binding.MetaCli";

    private static readonly ConsolePresenter Presenter = new();

    static int Main(string[] args)
    {
        if (CliVersion.TryWriteVersion(Presenter, AppName, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        var handlers = new MetaTransformBindingCommandHandlers(
            Presenter,
            new TransformBindingWorkspaceService(),
            new TransformBindingPartialReportService(),
            AppName);

        Environment.ExitCode = 0;
        var runtime = new MetaCliRuntime<MetaTransformBindingModel>(CommandWorkspacePath, ApplicationId)
            .UseDefaultHelp()
            .Bind(
                "exec-bind",
                [MetaCliWorkspace.Create("output", "output-xml", "output-csharp", "output-sql", "output-connection-env")],
                handlers.RunBindAsync);

        runtime.Run(args);
        return Environment.ExitCode;
    }

    private static string CommandWorkspacePath =>
        Path.Combine(AppContext.BaseDirectory, CommandWorkspaceDirectoryName);
}
