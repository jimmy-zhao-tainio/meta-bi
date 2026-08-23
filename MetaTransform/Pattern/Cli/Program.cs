using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaTransformPattern.Core;
using MetaTransformPatternInstance.Core;
using MTP = global::MetaTransformPattern;
using MTPI = global::MetaTransformPatternInstance;

internal static class Program
{
    private const string AppName = "meta-transform-pattern";
    private const string ApplicationId = "app-meta-transform-pattern";
    private const string CommandWorkspaceDirectoryName = "meta-transform-pattern.MetaCli";
    private static readonly ConsolePresenter Presenter = new();
    private static readonly TransformPatternAuthoringService PatternService = new();
    private static readonly TransformPatternInstanceAuthoringService InstanceService = new();

    private static int Main(string[] args)
    {
        if (CliVersion.TryWriteVersion(Presenter, AppName, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        return IsInstanceCommand(args) ? RunInstanceCommand(args) : RunPatternCommand(args);
    }

    private static int RunPatternCommand(string[] args)
    {
        var exitCode = 0;
        var runtime = new MetaCliRuntime<MTP.MetaTransformPatternModel>(
                CommandWorkspacePath,
                ApplicationId,
                setExitCode: code => exitCode = code)
            .UseDefaultHelp(options: new MetaCliHelpOptions("meta-transform-pattern show"))
            .Bind(
                "exec-create",
                [MetaCliWorkspace.Create("output", "xml", "csharp", "sql")],
                RunCreatePatternWorkspace)
            .Bind("exec-add-pattern", RunAddPattern)
            .Bind("exec-update-pattern", RunUpdatePattern)
            .BindReadOnly("exec-emit-pattern", RunEmitPattern)
            .BindReadOnly("exec-show", RunShowPatterns);

        runtime.Run(args);
        return exitCode;
    }

    private static int RunInstanceCommand(string[] args)
    {
        var exitCode = 0;
        var patternWorkspace = new[] { MetaCliWorkspace.Open("pattern-workspace") };
        var runtime = new MetaCliRuntime<MTPI.MetaTransformPatternInstanceModel>(
                CommandWorkspacePath,
                ApplicationId,
                setExitCode: code => exitCode = code)
            .UseDefaultHelp(options: new MetaCliHelpOptions("meta-transform-pattern show-instances"))
            .Bind(
                "exec-create-instance-workspace",
                [MetaCliWorkspace.Create("output", "xml", "csharp", "sql")],
                RunCreateInstanceWorkspace)
            .Bind("exec-add-instance", patternWorkspace, RunAddInstance)
            .Bind("exec-set-placeholder", patternWorkspace, RunSetPlaceholder)
            .Bind("exec-show-instances", patternWorkspace, RunShowInstances);

        runtime.Run(args);
        return exitCode;
    }

    private static bool IsInstanceCommand(IReadOnlyList<string> args) =>
        args.Count > 0 && args[0] is
            "create-instance-workspace" or
            "add-instance" or
            "set-placeholder" or
            "show-instances";

    private static string CommandWorkspacePath =>
        Path.Combine(AppContext.BaseDirectory, CommandWorkspaceDirectoryName);

    private static async Task RunCreatePatternWorkspace(
        MetaCliInvocation invocation,
        MetaCliWorkspaces workspaces)
    {
        await workspaces.CreateAsync("output", PatternService.CreateWorkspace()).ConfigureAwait(false);
        Presenter.WriteKeyValueBlock(
            "MetaTransformPattern workspace created",
            [
                ("Path", MetaCliWorkspace.OutputLocation(invocation)),
                ("Model", "MetaTransformPattern"),
                ("Rows", "0"),
            ]);
    }

    private static async Task RunCreateInstanceWorkspace(
        MetaCliInvocation invocation,
        MetaCliWorkspaces workspaces)
    {
        await workspaces.CreateAsync("output", InstanceService.CreateWorkspace()).ConfigureAwait(false);
        Presenter.WriteKeyValueBlock(
            "MetaTransformPatternInstance workspace created",
            [
                ("Path", MetaCliWorkspace.OutputLocation(invocation)),
                ("Model", "MetaTransformPatternInstance"),
                ("Rows", "0"),
            ]);
    }

    private static void RunAddPattern(
        MetaCliInvocation invocation,
        MTP.MetaTransformPatternModel model)
    {
        var pattern = PatternService.AddPattern(
            model,
            invocation.Required("id"),
            invocation.Required("name"),
            invocation.Optional("description"),
            MetaCliStandardInput.ReadToEnd());
        Presenter.WriteOk($"Transform pattern '{pattern.Id}' added");
    }

    private static void RunUpdatePattern(
        MetaCliInvocation invocation,
        MTP.MetaTransformPatternModel model)
    {
        var pattern = PatternService.UpdatePattern(
            model,
            invocation.Required("id"),
            MetaCliStandardInput.ReadToEnd());
        Presenter.WriteOk($"Transform pattern '{pattern.Id}' updated");
    }

    private static void RunEmitPattern(
        MetaCliInvocation invocation,
        MTP.MetaTransformPatternModel model) =>
        Presenter.WriteInfo(PatternService.EmitPattern(model, invocation.Required("id")));

    private static async Task RunAddInstance(
        MetaCliInvocation invocation,
        MTPI.MetaTransformPatternInstanceModel model,
        MetaCliWorkspaces workspaces)
    {
        var patterns = await RequirePatterns(workspaces).ConfigureAwait(false);
        var instance = InstanceService.AddInstance(
            model,
            patterns,
            invocation.Required("id"),
            invocation.Required("name"),
            invocation.Required("pattern"));
        Presenter.WriteOk($"Transform-pattern instance '{instance.Id}' added");
    }

    private static async Task RunSetPlaceholder(
        MetaCliInvocation invocation,
        MTPI.MetaTransformPatternInstanceModel model,
        MetaCliWorkspaces workspaces)
    {
        var patterns = await RequirePatterns(workspaces).ConfigureAwait(false);
        var holder = InstanceService.SetPlaceholderValue(
            model,
            patterns,
            invocation.Required("instance"),
            invocation.Required("placeholder"),
            MetaCliStandardInput.ReadToEnd());
        Presenter.WriteOk($"Placeholder '{holder.TransformPatternPlaceholderId}' set");
    }

    private static void RunShowPatterns(
        MetaCliInvocation invocation,
        MTP.MetaTransformPatternModel model)
    {
        Presenter.WriteInfo($"Patterns: {model.TransformPatternList.Count}");
        foreach (var pattern in model.TransformPatternList.OrderBy(
                     static pattern => pattern.Id,
                     StringComparer.OrdinalIgnoreCase))
        {
            var itemCount = model.TransformPatternItemList.Count(item =>
                ReferenceEquals(item.TransformPattern, pattern));
            var placeholderCount = model.TransformPatternPlaceholderList.Count(placeholder =>
                ReferenceEquals(placeholder.TransformPattern, pattern));
            Presenter.WriteInfo(
                $"  {pattern.Id}: {pattern.Name} ({itemCount} items, {placeholderCount} placeholders)");
        }
    }

    private static async Task RunShowInstances(
        MetaCliInvocation invocation,
        MTPI.MetaTransformPatternInstanceModel model,
        MetaCliWorkspaces workspaces)
    {
        var patterns = await RequirePatterns(workspaces).ConfigureAwait(false);
        Presenter.WriteInfo($"Instances: {model.TransformPatternInstanceList.Count}");
        foreach (var instance in model.TransformPatternInstanceList.OrderBy(
                     static instance => instance.Id,
                     StringComparer.OrdinalIgnoreCase))
        {
            var pattern = PatternService.RequirePattern(patterns, instance.TransformPatternId);
            var expected = patterns.TransformPatternPlaceholderList.Count(placeholder =>
                ReferenceEquals(placeholder.TransformPattern, pattern));
            var holders = model.TransformPatternInstancePlaceholderList.Where(holder =>
                ReferenceEquals(holder.TransformPatternInstance, instance)).ToArray();
            Presenter.WriteInfo(
                $"  {instance.Id}: {instance.Name} <- {pattern.Id} ({holders.Length}/{expected} placeholders)");
        }
    }

    private static Task<MTP.MetaTransformPatternModel> RequirePatterns(MetaCliWorkspaces workspaces) =>
        workspaces.RequiredAsync<MTP.MetaTransformPatternModel>("pattern-workspace");
}
