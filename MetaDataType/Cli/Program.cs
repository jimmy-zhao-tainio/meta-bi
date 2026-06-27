using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using Meta.Core.Services;
using MetaCli.Core;
using MetaDataType.Instance;
using MetaDataTypeModel = MetaDataType.MetaDataTypeModel;

internal static class Program
{
    private const string AppName = "meta-data-type";
    private const string ApplicationId = "app-meta-data-type";
    private const string CommandWorkspaceDirectoryName = "meta-data-type.MetaCli";
    private static readonly ConsolePresenter Presenter = new();

    static int Main(string[] args)
    {
        if (CliVersion.TryWriteVersion(Presenter, AppName, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        Environment.ExitCode = 0;
        var runtime = new MetaCliRuntime<MetaDataTypeModel>(CommandWorkspacePath, ApplicationId)
            .UseDefaultHelp()
            .Bind("exec-new-workspace", RunNewWorkspace);

        runtime.Run(args);
        return Environment.ExitCode;
    }

    private static string CommandWorkspacePath =>
        Path.Combine(AppContext.BaseDirectory, CommandWorkspaceDirectoryName);

    private static void RunNewWorkspace(MetaCliInvocation invocation)
    {
        var targetValidation = CliNewWorkspaceTargetValidator.Validate(invocation.Required("path"));
        if (!targetValidation.Ok)
        {
            throw new InvalidOperationException(targetValidation.ErrorMessage);
        }

        var workspacePath = targetValidation.FullPath;
        Directory.CreateDirectory(workspacePath);

        var model = MetaDataTypeInstance.Default;
        model.SaveToXmlWorkspace(workspacePath);

        Presenter.WriteKeyValueBlock(
            "MetaDataType workspace created",
            new[]
            {
                ("Path", workspacePath),
                ("Model", "MetaDataType"),
                ("DataTypeSystems", model.DataTypeSystemList.Count.ToString()),
                ("DataTypes", model.DataTypeList.Count.ToString())
            });
    }
}
