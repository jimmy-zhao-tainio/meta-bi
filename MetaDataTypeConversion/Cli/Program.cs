using System.Linq;
using Meta.Core.Domain;
using Meta.Core.Presentation;
using Meta.Core.Services;
using MetaBi.Cli.Common;
using MetaDataTypeConversion.Core;

internal static class Program
{
    private static readonly ConsolePresenter Presenter = new();

    static async Task<int> Main(string[] args)
    {
        if (args.Length == 0 || IsHelpToken(args[0]))
        {
            PrintHelp();
            return 0;
        }

        if (string.Equals(args[0], "init", StringComparison.OrdinalIgnoreCase))
        {
            return await RunInitAsync(args).ConfigureAwait(false);
        }

        if (string.Equals(args[0], "check", StringComparison.OrdinalIgnoreCase))
        {
            return await RunCheckAsync(args).ConfigureAwait(false);
        }

        if (string.Equals(args[0], "resolve", StringComparison.OrdinalIgnoreCase))
        {
            return await RunResolveAsync(args).ConfigureAwait(false);
        }

        return Fail($"unknown command '{args[0]}'.", "meta-data-type-conversion help");
    }

    private static async Task<int> RunInitAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintInitHelp();
            return 0;
        }

        var parseResult = ParseNewWorkspaceOnly(args, startIndex: 1);
        if (!parseResult.Ok)
        {
            return Fail(parseResult.ErrorMessage, "meta-data-type-conversion init --help");
        }

        var targetValidation = CliNewWorkspaceTargetValidator.Validate(parseResult.NewWorkspacePath);
        if (!targetValidation.Ok)
        {
            return Fail(targetValidation.ErrorMessage, "choose a new folder or empty the target directory and retry.", 4, targetValidation.Details);
        }

        var workspacePath = targetValidation.FullPath;
        Directory.CreateDirectory(workspacePath);

        var workspace = MetaDataTypeConversionWorkspaces.CreateMetaDataTypeConversionWorkspace(workspacePath);
        var validation = new ValidationService().Validate(workspace);
        if (validation.HasErrors)
        {
            return Fail(
                "MetaDataTypeConversion workspace is invalid.",
                "fix the sanctioned model and retry init.",
                4,
                validation.Issues
                    .Where(item => item.Severity == IssueSeverity.Error)
                    .Select(item => $"  - {item.Code}: {item.Message}"));
        }

        await new WorkspaceService().SaveAsync(workspace).ConfigureAwait(false);

        Presenter.WriteOk(
            "MetaDataTypeConversion workspace created",
            ("Path", workspacePath),
            ("Model", workspace.Model.Name),
            ("ConversionImplementations", workspace.Instance.GetOrCreateEntityRecords("ConversionImplementation").Count.ToString()),
            ("DataTypeMappings", workspace.Instance.GetOrCreateEntityRecords("DataTypeMapping").Count.ToString()));
        return 0;
    }

    private static async Task<int> RunCheckAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintCheckHelp();
            return 0;
        }

        var parseResult = ParseWorkspaceOnly(args, startIndex: 1);
        if (!parseResult.Ok)
        {
            return Fail(parseResult.ErrorMessage, "meta-data-type-conversion check --help");
        }

        Workspace workspace;
        try
        {
            workspace = await new WorkspaceService().LoadAsync(Path.GetFullPath(parseResult.WorkspacePath), searchUpward: false).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is InvalidOperationException or IOException or UnauthorizedAccessException)
        {
            return Fail(ex.Message, "meta-data-type-conversion check --help", 4);
        }

        var result = new MetaDataTypeConversionService().Check(workspace);
        if (result.HasErrors)
        {
            return Fail("MetaDataTypeConversion check failed.", "fix the sanctioned mappings and rerun check.", 2, result.Errors.Select(error => $"  - {error}"));
        }

        Presenter.WriteOk(
            "MetaDataTypeConversion check",
            ("Workspace", Path.GetFullPath(parseResult.WorkspacePath)),
            ("ConversionImplementations", result.ImplementationCount.ToString()),
            ("DataTypeMappings", result.MappingCount.ToString()),
            ("Errors", "0"));
        return 0;
    }

    private static async Task<int> RunResolveAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintResolveHelp();
            return 0;
        }

        var parseResult = ParseResolveArgs(args, startIndex: 1);
        if (!parseResult.Ok)
        {
            return Fail(parseResult.ErrorMessage, "meta-data-type-conversion resolve --help");
        }

        Workspace workspace;
        try
        {
            workspace = await new WorkspaceService().LoadAsync(Path.GetFullPath(parseResult.WorkspacePath), searchUpward: false).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is InvalidOperationException or IOException or UnauthorizedAccessException)
        {
            return Fail(ex.Message, "meta-data-type-conversion resolve --help", 4);
        }

        try
        {
            var resolution = new MetaDataTypeConversionService().Resolve(workspace, parseResult.SourceDataTypeId);
            var details = new List<(string Key, string Value)>
            {
                ("Workspace", Path.GetFullPath(parseResult.WorkspacePath)),
                ("SourceDataTypeId", resolution.SourceDataTypeId),
                ("TargetDataTypeId", resolution.TargetDataTypeId),
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
            return Fail(ex.Message, "meta-data-type-conversion resolve --help", 4);
        }
    }

    private static (bool Ok, string NewWorkspacePath, string ErrorMessage) ParseNewWorkspaceOnly(string[] args, int startIndex)
    {
        var newWorkspacePath = string.Empty;
        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (!string.Equals(arg, "--new-workspace", StringComparison.OrdinalIgnoreCase))
            {
                return (false, newWorkspacePath, $"unknown option '{arg}'.");
            }

            if (i + 1 >= args.Length)
            {
                return (false, newWorkspacePath, "missing value for --new-workspace.");
            }

            if (!string.IsNullOrWhiteSpace(newWorkspacePath))
            {
                return (false, newWorkspacePath, "--new-workspace can only be provided once.");
            }

            newWorkspacePath = args[++i];
        }

        if (string.IsNullOrWhiteSpace(newWorkspacePath))
        {
            return (false, string.Empty, "missing required option --new-workspace <path>.");
        }

        return (true, newWorkspacePath, string.Empty);
    }

    private static (bool Ok, string WorkspacePath, string ErrorMessage) ParseWorkspaceOnly(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (!string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                return (false, workspacePath, $"unknown option '{arg}'.");
            }

            if (i + 1 >= args.Length)
            {
                return (false, workspacePath, "missing value for --workspace.");
            }

            if (!string.IsNullOrWhiteSpace(workspacePath))
            {
                return (false, workspacePath, "--workspace can only be provided once.");
            }

            workspacePath = args[++i];
        }

        if (string.IsNullOrWhiteSpace(workspacePath))
        {
            return (false, string.Empty, "missing required option --workspace <path>.");
        }

        return (true, workspacePath, string.Empty);
    }

    private static (bool Ok, string WorkspacePath, string SourceDataTypeId, string ErrorMessage) ParseResolveArgs(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        var sourceDataTypeId = string.Empty;
        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (i + 1 >= args.Length)
            {
                return (false, workspacePath, sourceDataTypeId, $"missing value for {arg}.");
            }

            switch (arg.ToLowerInvariant())
            {
                case "--workspace":
                    if (!string.IsNullOrWhiteSpace(workspacePath))
                    {
                        return (false, workspacePath, sourceDataTypeId, "--workspace can only be provided once.");
                    }
                    workspacePath = args[++i];
                    break;
                case "--source-data-type":
                    if (!string.IsNullOrWhiteSpace(sourceDataTypeId))
                    {
                        return (false, workspacePath, sourceDataTypeId, "--source-data-type can only be provided once.");
                    }
                    sourceDataTypeId = args[++i];
                    break;
                default:
                    return (false, workspacePath, sourceDataTypeId, $"unknown option '{arg}'.");
            }
        }

        if (string.IsNullOrWhiteSpace(workspacePath))
        {
            return (false, string.Empty, sourceDataTypeId, "missing required option --workspace <path>.");
        }

        if (string.IsNullOrWhiteSpace(sourceDataTypeId))
        {
            return (false, workspacePath, string.Empty, "missing required option --source-data-type <id>.");
        }

        return (true, workspacePath, sourceDataTypeId, string.Empty);
    }

    private static bool IsHelpToken(string value)
    {
        return string.Equals(value, "help", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "--help", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "-h", StringComparison.OrdinalIgnoreCase);
    }

    private static void PrintHelp()
    {
        Presenter.WriteUsage("meta-data-type-conversion <command> [options]");
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteCommandCatalog(
            "Commands:",
            new[]
            {
                ("help", "Show this help."),
                ("init", "Create a new MetaDataTypeConversion workspace."),
                ("check", "Validate sanctioned type mappings."),
                ("resolve", "Resolve one source data type id through the sanctioned mappings.")
            });
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteNext("meta-data-type-conversion init --help");
    }

    private static void PrintInitHelp()
    {
        Presenter.WriteInfo("Command: init");
        Presenter.WriteUsage("meta-data-type-conversion init --new-workspace <path>");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Creates a new workspace with the MetaDataTypeConversion model and validates it.");
    }

    private static void PrintCheckHelp()
    {
        Presenter.WriteInfo("Command: check");
        Presenter.WriteUsage("meta-data-type-conversion check --workspace <path>");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Validates that each source data type maps deterministically and that every mapping references a real ConversionImplementation.");
    }

    private static void PrintResolveHelp()
    {
        Presenter.WriteInfo("Command: resolve");
        Presenter.WriteUsage("meta-data-type-conversion resolve --workspace <path> --source-data-type <id>");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Resolves one source data type id to its target data type id and conversion implementation.");
    }

    private static int Fail(string message, string next, int exitCode = 1, IEnumerable<string>? details = null)
    {
        var renderedDetails = new List<string>();
        if (details != null)
        {
            renderedDetails.AddRange(details);
        }

        renderedDetails.Add($"Next: {next}");
        Presenter.WriteFailure(message, renderedDetails);
        return exitCode;
    }
}

