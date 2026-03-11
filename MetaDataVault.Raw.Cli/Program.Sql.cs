using Meta.Core.Presentation;
using MetaDataVault.Core;

internal static partial class Program
{
    private static async Task<int> RunGenerateSqlAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintGenerateSqlHelp();
            return 0;
        }

        var parse = ParseGenerateSqlArgs(args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, "meta-datavault-raw generate-sql --help");
        }

        var outputPath = Path.GetFullPath(parse.OutputPath);
        if (Directory.Exists(outputPath) && Directory.EnumerateFileSystemEntries(outputPath).Any())
        {
            return Fail($"target directory '{outputPath}' must be empty.", "choose a new folder or empty the target directory and retry.", 4);
        }

        try
        {
            var result = await new RawDataVaultSqlGenerator().GenerateAsync(
                Path.GetFullPath(parse.WorkspacePath),
                Path.GetFullPath(parse.ImplementationWorkspacePath),
                Path.GetFullPath(parse.DataTypeConversionWorkspacePath),
                outputPath).ConfigureAwait(false);

            Presenter.WriteOk(
                "raw datavault sql generated",
                ("Workspace", Path.GetFullPath(parse.WorkspacePath)),
                ("Implementation Workspace", Path.GetFullPath(parse.ImplementationWorkspacePath)),
                ("DataTypeConversion Workspace", Path.GetFullPath(parse.DataTypeConversionWorkspacePath)),
                ("Path", result.OutputPath),
                ("Files", result.FileCount.ToString()),
                ("Tables", result.TableCount.ToString()),
                ("SourceSystems", result.SourceSystemCount.ToString()),
                ("SourceSchemas", result.SourceSchemaCount.ToString()),
                ("SourceTables", result.SourceTableCount.ToString()),
                ("SourceFields", result.SourceFieldCount.ToString()),
                ("RawHubs", result.RawHubCount.ToString()),
                ("RawLinks", result.RawLinkCount.ToString()),
                ("RawLinkHubs", result.RawLinkHubCount.ToString()),
                ("RawHubSatellites", result.RawHubSatelliteCount.ToString()),
                ("RawLinkSatellites", result.RawLinkSatelliteCount.ToString()));
            return 0;
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or IOException or UnauthorizedAccessException)
        {
            return Fail(ex.Message, "meta-datavault-raw generate-sql --help", 4);
        }
    }

    private static (bool Ok, string WorkspacePath, string ImplementationWorkspacePath, string DataTypeConversionWorkspacePath, string OutputPath, string ErrorMessage) ParseGenerateSqlArgs(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        var implementationWorkspacePath = string.Empty;
        var dataTypeConversionWorkspacePath = string.Empty;
        var outputPath = string.Empty;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "missing value for --workspace.");
                if (!string.IsNullOrWhiteSpace(workspacePath)) return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "--workspace can only be provided once.");
                workspacePath = args[++i];
                continue;
            }
            if (string.Equals(arg, "--implementation-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "missing value for --implementation-workspace.");
                if (!string.IsNullOrWhiteSpace(implementationWorkspacePath)) return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "--implementation-workspace can only be provided once.");
                implementationWorkspacePath = args[++i];
                continue;
            }
            if (string.Equals(arg, "--data-type-conversion-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "missing value for --data-type-conversion-workspace.");
                if (!string.IsNullOrWhiteSpace(dataTypeConversionWorkspacePath)) return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "--data-type-conversion-workspace can only be provided once.");
                dataTypeConversionWorkspacePath = args[++i];
                continue;
            }
            if (string.Equals(arg, "--out", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "missing value for --out.");
                if (!string.IsNullOrWhiteSpace(outputPath)) return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "--out can only be provided once.");
                outputPath = args[++i];
                continue;
            }
            return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(workspacePath)) return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "missing required option --workspace <path>.");
        if (string.IsNullOrWhiteSpace(implementationWorkspacePath)) return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "missing required option --implementation-workspace <path>.");
        if (string.IsNullOrWhiteSpace(dataTypeConversionWorkspacePath)) return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "missing required option --data-type-conversion-workspace <path>.");
        if (string.IsNullOrWhiteSpace(outputPath)) return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "missing required option --out <path>.");
        return (true, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, string.Empty);
    }

    private static void PrintGenerateSqlHelp()
    {
        Presenter.WriteInfo("Command: generate-sql");
        Presenter.WriteUsage("meta-datavault-raw generate-sql --workspace <path> --implementation-workspace <path> --data-type-conversion-workspace <path> --out <path>");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Loads a MetaRawDataVault workspace through generated typed tooling.");
        Presenter.WriteInfo("  Applies MetaDataVaultImplementation naming and technical column defaults.");
        Presenter.WriteInfo("  Emits SQL for raw hubs, raw links, raw hub satellites, and raw link satellites.");
    }
}
