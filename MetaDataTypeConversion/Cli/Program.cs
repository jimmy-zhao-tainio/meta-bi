using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaDataTypeConversion;
using MetaDataTypeConversion.Core;
using MetaDataTypeConversion.Instance;

internal static class Program
{
    private const string AppName = "meta-data-type-conversion";
    private const string ApplicationId = "app-meta-data-type-conversion";
    private const string CommandWorkspaceDirectoryName = "meta-data-type-conversion.MetaCli";
    private static readonly ConsolePresenter Presenter = new();

    static int Main(string[] args)
    {
        if (CliVersion.TryWriteVersion(Presenter, AppName, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        Environment.ExitCode = 0;
        var runtime = new MetaCliRuntime<MetaDataTypeConversionModel>(CommandWorkspacePath, ApplicationId)
            .UseDefaultHelp()
            .Bind("exec-new-workspace", invocation => Complete(() => RunNewWorkspace(invocation)))
            .Bind("exec-check", (invocation, model) => Complete(() => RunCheck(model)))
            .Bind("exec-resolve", (invocation, model) => Complete(() => RunResolve(invocation, model)));

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

    private static int RunNewWorkspace(MetaCliInvocation invocation)
    {
        var targetValidation = CliNewWorkspaceTargetValidator.Validate(invocation.Required("path"));
        if (!targetValidation.Ok)
        {
            return Fail(
                targetValidation.ErrorMessage,
                "choose a new folder or empty the target directory and retry.",
                4,
                targetValidation.Details);
        }

        var workspacePath = targetValidation.FullPath;
        Directory.CreateDirectory(workspacePath);

        var model = MetaDataTypeConversionInstance.Default;
        model.SaveToXmlWorkspace(workspacePath);

        Presenter.WriteOk(
            "MetaDataTypeConversion workspace created",
            ("Path", workspacePath),
            ("Model", "MetaDataTypeConversion"),
            ("ConversionImplementations", model.ConversionImplementationList.Count.ToString()),
            ("DataTypeMappings", model.DataTypeMappingList.Count.ToString()));
        return 0;
    }

    private static int RunCheck(MetaDataTypeConversionModel model)
    {
        var result = new MetaDataTypeConversionService().Check(model);
        if (result.HasErrors)
        {
            return Fail(
                "Cannot check data-type conversions.",
                "fix the sanctioned mappings and rerun check.",
                2,
                result.Errors.Select(error => $"  - {error}"));
        }

        Presenter.WriteOk(
            "MetaDataTypeConversion check",
            ("ConversionImplementations", result.ImplementationCount.ToString()),
            ("DataTypeMappings", result.MappingCount.ToString()),
            ("Errors", "0"));
        return 0;
    }

    private static int RunResolve(MetaCliInvocation invocation, MetaDataTypeConversionModel model)
    {
        var sourceDataTypeId = invocation.Required("source-data-type");
        var targetDataTypeSystemName = invocation.Optional("target-data-type-system");

        try
        {
            var resolution = string.IsNullOrWhiteSpace(targetDataTypeSystemName)
                ? new MetaDataTypeConversionService().Resolve(model, sourceDataTypeId)
                : new MetaDataTypeConversionService().Resolve(model, sourceDataTypeId, targetDataTypeSystemName);
            var details = new List<(string Key, string Value)>
            {
                ("SourceDataTypeId", resolution.SourceDataTypeId),
                ("TargetDataTypeId", resolution.TargetDataTypeId),
                ("TargetDataTypeSystem", resolution.TargetDataTypeSystemName),
                ("ConversionImplementation", resolution.ConversionImplementationName)
            };

            if (!string.IsNullOrWhiteSpace(resolution.Notes))
            {
                details.Add(("Notes", resolution.Notes));
            }

            Presenter.WriteOk("MetaDataTypeConversion resolve", details.ToArray());
            return 0;
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            return Fail(
                "Cannot resolve data-type conversion.",
                HelpCommand("resolve"),
                4,
                [$"  {ex.Message}"]);
        }
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
