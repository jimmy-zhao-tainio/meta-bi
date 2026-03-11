using System.Linq;
using Meta.Core.Domain;
using Meta.Core.Presentation;
using Meta.Core.Services;
using MetaDataVault.Core;

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

        if (string.Equals(args[0], "--new-workspace", StringComparison.OrdinalIgnoreCase))
        {
            return await RunNewWorkspaceAsync(args).ConfigureAwait(false);
        }

        if (string.Equals(args[0], "check-business-materialization", StringComparison.OrdinalIgnoreCase))
        {
            return await RunCheckBusinessMaterializationAsync(args).ConfigureAwait(false);
        }

        if (string.Equals(args[0], "materialize-business", StringComparison.OrdinalIgnoreCase))
        {
            return await RunMaterializeBusinessAsync(args).ConfigureAwait(false);
        }

        if (string.Equals(args[0], "generate-sql", StringComparison.OrdinalIgnoreCase))
        {
            return await RunGenerateSqlAsync(args).ConfigureAwait(false);
        }

        return Fail($"unknown command '{args[0]}'.", "meta-datavault-business help");
    }



    private static async Task<int> RunNewWorkspaceAsync(string[] args)
    {
        var parseResult = ParseNewWorkspaceOnly(args, 0);
        if (!parseResult.Ok)
        {
            return Fail(parseResult.ErrorMessage, "meta-datavault-business --new-workspace <path>");
        }

        var workspacePath = Path.GetFullPath(parseResult.NewWorkspacePath);
        if (Directory.Exists(workspacePath) && Directory.EnumerateFileSystemEntries(workspacePath).Any())
        {
            return Fail($"target directory '{workspacePath}' must be empty.", "choose a new folder or empty the target directory and retry.", 4);
        }

        Directory.CreateDirectory(workspacePath);
        var workspace = MetaDataVaultWorkspaces.CreateEmptyMetaBusinessDataVaultWorkspace(workspacePath);
        var validation = new ValidationService().Validate(workspace);
        if (validation.HasErrors)
        {
            return Fail(
                "metabusinessdatavault workspace is invalid.",
                "fix the sanctioned model and retry workspace creation.",
                4,
                validation.Issues.Where(item => item.Severity == IssueSeverity.Error).Select(item => $"  - {item.Code}: {item.Message}"));
        }

        await new WorkspaceService().SaveAsync(workspace).ConfigureAwait(false);
        Presenter.WriteOk(
            "metabusinessdatavault workspace created",
            ("Path", workspacePath),
            ("Model", workspace.Model.Name));
        return 0;
    }

    private static async Task<int> RunCheckBusinessMaterializationAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintCheckBusinessMaterializationHelp();
            return 0;
        }

        var parse = ParseCheckBusinessMaterializationArgs(args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, "meta-datavault-business check-business-materialization --help");
        }

        var businessWorkspacePath = Path.GetFullPath(parse.BusinessWorkspacePath);
        var businessDataVaultWorkspacePath = Path.GetFullPath(parse.BusinessDataVaultWorkspacePath);
        var implementationWorkspacePath = Path.GetFullPath(parse.ImplementationWorkspacePath);
        var weaveWorkspacePaths = parse.WeaveWorkspacePaths.Select(Path.GetFullPath).ToArray();
        var fabricWorkspacePaths = parse.FabricWorkspacePaths.Select(Path.GetFullPath).ToArray();

        Workspace businessWorkspace;
        Workspace businessDataVaultWorkspace;
        Workspace implementationWorkspace;
        var weaveWorkspaces = new List<Workspace>();
        var fabricWorkspaces = new List<Workspace>();

        try
        {
            var workspaceService = new WorkspaceService();
            businessWorkspace = await workspaceService.LoadAsync(businessWorkspacePath, searchUpward: false).ConfigureAwait(false);
            businessDataVaultWorkspace = await workspaceService.LoadAsync(businessDataVaultWorkspacePath, searchUpward: false).ConfigureAwait(false);
            implementationWorkspace = await workspaceService.LoadAsync(implementationWorkspacePath, searchUpward: false).ConfigureAwait(false);
            foreach (var path in weaveWorkspacePaths)
            {
                weaveWorkspaces.Add(await workspaceService.LoadAsync(path, searchUpward: false).ConfigureAwait(false));
            }

            foreach (var path in fabricWorkspacePaths)
            {
                fabricWorkspaces.Add(await workspaceService.LoadAsync(path, searchUpward: false).ConfigureAwait(false));
            }
        }
        catch (Exception ex)
        {
            return Fail(
                "could not load sanctioned business datavault materialization inputs.",
                "check the MetaBusiness, MetaBusinessDataVault, MetaDataVaultImplementation, MetaWeave, and MetaFabric workspace paths and retry.",
                4,
                new[] { $"  - {ex.Message}" });
        }

        BusinessDataVaultMaterializationContractResult contract;
        try
        {
            contract = await new BusinessDataVaultMaterializationContractService().CheckAsync(
                businessWorkspace,
                businessDataVaultWorkspace,
                implementationWorkspace,
                weaveWorkspaces,
                fabricWorkspaces).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return Fail(
                "could not evaluate business datavault materialization contract.",
                "check the sanctioned input models and retry.",
                4,
                new[] { $"  - {ex.Message}" });
        }

        if (contract.HasErrors)
        {
            return Fail(
                "business datavault materialization contract failed.",
                "fix the sanctioned inputs and retry.",
                4,
                contract.Errors.Select(item => $"  - {item}"));
        }

        Presenter.WriteOk(
            "business datavault materialization contract",
            ("Business Workspace", businessWorkspacePath),
            ("BusinessDataVault Workspace", businessDataVaultWorkspacePath),
            ("Implementation Workspace", implementationWorkspacePath),
            ("Weaves", contract.WeaveCount.ToString()),
            ("Fabrics", contract.FabricCount.ToString()),
            ("FlatAnchors", $"{contract.FlatAnchorsSatisfied}/{contract.FlatAnchorsRequired}"),
            ("ScopedAnchors", $"{contract.ScopedAnchorsSatisfied}/{contract.ScopedAnchorsRequired}"));
        return 0;
    }

    private static async Task<int> RunMaterializeBusinessAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintMaterializeBusinessHelp();
            return 0;
        }

        var parse = ParseMaterializeBusinessArgs(args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, "meta-datavault-business materialize-business --help");
        }

        var businessWorkspacePath = Path.GetFullPath(parse.BusinessWorkspacePath);
        var businessDataVaultWorkspacePath = Path.GetFullPath(parse.BusinessDataVaultWorkspacePath);
        var implementationWorkspacePath = Path.GetFullPath(parse.ImplementationWorkspacePath);
        var weaveWorkspacePaths = parse.WeaveWorkspacePaths.Select(Path.GetFullPath).ToArray();
        var fabricWorkspacePaths = parse.FabricWorkspacePaths.Select(Path.GetFullPath).ToArray();
        var newWorkspacePath = Path.GetFullPath(parse.NewWorkspacePath);
        if (Directory.Exists(newWorkspacePath) && Directory.EnumerateFileSystemEntries(newWorkspacePath).Any())
        {
            return Fail($"target directory '{newWorkspacePath}' must be empty.", "choose a new folder or empty the target directory and retry.", 4);
        }

        Workspace businessWorkspace;
        Workspace businessDataVaultWorkspace;
        Workspace implementationWorkspace;
        var weaveWorkspaces = new List<Workspace>();
        var fabricWorkspaces = new List<Workspace>();

        try
        {
            var workspaceService = new WorkspaceService();
            businessWorkspace = await workspaceService.LoadAsync(businessWorkspacePath, searchUpward: false).ConfigureAwait(false);
            businessDataVaultWorkspace = await workspaceService.LoadAsync(businessDataVaultWorkspacePath, searchUpward: false).ConfigureAwait(false);
            implementationWorkspace = await workspaceService.LoadAsync(implementationWorkspacePath, searchUpward: false).ConfigureAwait(false);
            foreach (var path in weaveWorkspacePaths)
            {
                weaveWorkspaces.Add(await workspaceService.LoadAsync(path, searchUpward: false).ConfigureAwait(false));
            }

            foreach (var path in fabricWorkspacePaths)
            {
                fabricWorkspaces.Add(await workspaceService.LoadAsync(path, searchUpward: false).ConfigureAwait(false));
            }
        }
        catch (Exception ex)
        {
            return Fail(
                "could not load sanctioned business datavault materialization inputs.",
                "check the MetaBusiness, MetaBusinessDataVault, MetaDataVaultImplementation, MetaWeave, and MetaFabric workspace paths and retry.",
                4,
                new[] { $"  - {ex.Message}" });
        }

        BusinessDataVaultMaterializationResult materialization;
        try
        {
            materialization = await new BusinessDataVaultMaterializer().MaterializeAsync(
                businessWorkspace,
                businessDataVaultWorkspace,
                implementationWorkspace,
                weaveWorkspaces,
                fabricWorkspaces,
                newWorkspacePath).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return Fail(
                "could not materialize business datavault from sanctioned inputs.",
                "check the sanctioned inputs, implementation defaults, and scope bindings, then retry.",
                4,
                new[] { $"  - {ex.Message}" });
        }

        var validation = new ValidationService().Validate(materialization.Workspace);
        if (validation.HasErrors)
        {
            return Fail(
                "materialized business datavault workspace is invalid.",
                "inspect the materialized workspace and retry.",
                4,
                validation.Issues
                    .Where(item => item.Severity == IssueSeverity.Error)
                    .Select(item => $"  - {item.Code}: {item.Message}"));
        }

        Directory.CreateDirectory(newWorkspacePath);
        await new WorkspaceService().SaveAsync(materialization.Workspace).ConfigureAwait(false);

        Presenter.WriteOk(
            "business datavault materialized",
            ("Business Workspace", businessWorkspacePath),
            ("BusinessDataVault Workspace", businessDataVaultWorkspacePath),
            ("Implementation Workspace", implementationWorkspacePath),
            ("Path", newWorkspacePath),
            ("Model", materialization.Workspace.Model.Name),
            ("MaterializedTables", materialization.MaterializedTableCount.ToString()),
            ("BusinessHubs", materialization.BusinessHubCount.ToString()),
            ("BusinessLinks", materialization.BusinessLinkCount.ToString()),
            ("BusinessHubSatellites", materialization.BusinessHubSatelliteCount.ToString()),
            ("BusinessLinkSatellites", materialization.BusinessLinkSatelliteCount.ToString()),
            ("BusinessPointInTimes", materialization.BusinessPointInTimeCount.ToString()),
            ("BusinessBridges", materialization.BusinessBridgeCount.ToString()));
        return 0;
    }

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
            return Fail(parse.ErrorMessage, "meta-datavault-business generate-sql --help");
        }

        var workspacePath = Path.GetFullPath(parse.WorkspacePath);
        var implementationWorkspacePath = Path.GetFullPath(parse.ImplementationWorkspacePath);
        var dataTypeConversionWorkspacePath = Path.GetFullPath(parse.DataTypeConversionWorkspacePath);
        var outputPath = Path.GetFullPath(parse.OutputPath);
        if (Directory.Exists(outputPath) && Directory.EnumerateFileSystemEntries(outputPath).Any())
        {
            return Fail($"target directory '{outputPath}' must be empty.", "choose a new folder or empty the target directory and retry.", 4);
        }

        BusinessDataVaultSqlGenerationResult result;
        try
        {
            result = await new BusinessDataVaultSqlGenerator().GenerateAsync(
                workspacePath,
                implementationWorkspacePath,
                dataTypeConversionWorkspacePath,
                outputPath).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return Fail(
                "could not generate business datavault SQL.",
                "check the materialized MetaBusinessDataVault, MetaDataVaultImplementation, and MetaDataTypeConversion workspaces and retry.",
                4,
                new[] { $"  - {ex.Message}" });
        }

        Presenter.WriteOk(
            "business datavault sql generated",
            ("Workspace", workspacePath),
            ("Implementation Workspace", implementationWorkspacePath),
            ("DataTypeConversion Workspace", dataTypeConversionWorkspacePath),
            ("Path", result.OutputPath),
            ("Files", result.FileCount.ToString()),
            ("Tables", result.TableCount.ToString()),
            ("BusinessHubs", result.BusinessHubCount.ToString()),
            ("BusinessLinks", result.BusinessLinkCount.ToString()),
            ("BusinessSameAsLinks", result.BusinessSameAsLinkCount.ToString()),
            ("BusinessHierarchicalLinks", result.BusinessHierarchicalLinkCount.ToString()),
            ("BusinessHubSatellites", result.BusinessHubSatelliteCount.ToString()),
            ("BusinessLinkSatellites", result.BusinessLinkSatelliteCount.ToString()),
            ("BusinessSameAsLinkSatellites", result.BusinessSameAsLinkSatelliteCount.ToString()),
            ("BusinessHierarchicalLinkSatellites", result.BusinessHierarchicalLinkSatelliteCount.ToString()),
            ("BusinessReferences", result.BusinessReferenceCount.ToString()),
            ("BusinessReferenceSatellites", result.BusinessReferenceSatelliteCount.ToString()),
            ("BusinessPointInTimes", result.BusinessPointInTimeCount.ToString()),
            ("BusinessBridges", result.BusinessBridgeCount.ToString()));
        return 0;
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
    private static (bool Ok, string BusinessWorkspacePath, string BusinessDataVaultWorkspacePath, string ImplementationWorkspacePath, List<string> WeaveWorkspacePaths, List<string> FabricWorkspacePaths, string ErrorMessage) ParseCheckBusinessMaterializationArgs(string[] args, int startIndex)
    {
        var businessWorkspacePath = string.Empty;
        var businessDataVaultWorkspacePath = string.Empty;
        var implementationWorkspacePath = string.Empty;
        var weaveWorkspacePaths = new List<string>();
        var fabricWorkspacePaths = new List<string>();

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--business-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, "missing value for --business-workspace.");
                }

                if (!string.IsNullOrWhiteSpace(businessWorkspacePath))
                {
                    return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, "--business-workspace can only be provided once.");
                }

                businessWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--bdv-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, "missing value for --bdv-workspace.");
                }

                if (!string.IsNullOrWhiteSpace(businessDataVaultWorkspacePath))
                {
                    return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, "--bdv-workspace can only be provided once.");
                }

                businessDataVaultWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--implementation-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, "missing value for --implementation-workspace.");
                }

                if (!string.IsNullOrWhiteSpace(implementationWorkspacePath))
                {
                    return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, "--implementation-workspace can only be provided once.");
                }

                implementationWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--weave-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, "missing value for --weave-workspace.");
                }

                weaveWorkspacePaths.Add(args[++i]);
                continue;
            }

            if (string.Equals(arg, "--fabric-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, "missing value for --fabric-workspace.");
                }

                fabricWorkspacePaths.Add(args[++i]);
                continue;
            }

            return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(businessWorkspacePath))
        {
            return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, "missing required option --business-workspace <path>.");
        }

        if (string.IsNullOrWhiteSpace(businessDataVaultWorkspacePath))
        {
            return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, "missing required option --bdv-workspace <path>.");
        }

        if (string.IsNullOrWhiteSpace(implementationWorkspacePath))
        {
            return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, "missing required option --implementation-workspace <path>.");
        }

        if (weaveWorkspacePaths.Count == 0)
        {
            return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, "missing required option --weave-workspace <path>.");
        }

        if (fabricWorkspacePaths.Count == 0)
        {
            return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, "missing required option --fabric-workspace <path>.");
        }

        return (true, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, string.Empty);
    }

    private static (bool Ok, string BusinessWorkspacePath, string BusinessDataVaultWorkspacePath, string ImplementationWorkspacePath, List<string> WeaveWorkspacePaths, List<string> FabricWorkspacePaths, string NewWorkspacePath, string ErrorMessage) ParseMaterializeBusinessArgs(string[] args, int startIndex)
    {
        var businessWorkspacePath = string.Empty;
        var businessDataVaultWorkspacePath = string.Empty;
        var implementationWorkspacePath = string.Empty;
        var weaveWorkspacePaths = new List<string>();
        var fabricWorkspacePaths = new List<string>();
        var newWorkspacePath = string.Empty;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--business-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, newWorkspacePath, "missing value for --business-workspace.");
                }

                if (!string.IsNullOrWhiteSpace(businessWorkspacePath))
                {
                    return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, newWorkspacePath, "--business-workspace can only be provided once.");
                }

                businessWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--bdv-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, newWorkspacePath, "missing value for --bdv-workspace.");
                }

                if (!string.IsNullOrWhiteSpace(businessDataVaultWorkspacePath))
                {
                    return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, newWorkspacePath, "--bdv-workspace can only be provided once.");
                }

                businessDataVaultWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--implementation-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, newWorkspacePath, "missing value for --implementation-workspace.");
                }

                if (!string.IsNullOrWhiteSpace(implementationWorkspacePath))
                {
                    return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, newWorkspacePath, "--implementation-workspace can only be provided once.");
                }

                implementationWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--weave-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, newWorkspacePath, "missing value for --weave-workspace.");
                }

                weaveWorkspacePaths.Add(args[++i]);
                continue;
            }

            if (string.Equals(arg, "--fabric-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, newWorkspacePath, "missing value for --fabric-workspace.");
                }

                fabricWorkspacePaths.Add(args[++i]);
                continue;
            }

            if (string.Equals(arg, "--new-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, newWorkspacePath, "missing value for --new-workspace.");
                }

                if (!string.IsNullOrWhiteSpace(newWorkspacePath))
                {
                    return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, newWorkspacePath, "--new-workspace can only be provided once.");
                }

                newWorkspacePath = args[++i];
                continue;
            }

            return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, newWorkspacePath, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(businessWorkspacePath))
        {
            return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, newWorkspacePath, "missing required option --business-workspace <path>.");
        }

        if (string.IsNullOrWhiteSpace(businessDataVaultWorkspacePath))
        {
            return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, newWorkspacePath, "missing required option --bdv-workspace <path>.");
        }

        if (string.IsNullOrWhiteSpace(implementationWorkspacePath))
        {
            return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, newWorkspacePath, "missing required option --implementation-workspace <path>.");
        }

        if (weaveWorkspacePaths.Count == 0)
        {
            return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, newWorkspacePath, "missing required option --weave-workspace <path>.");
        }

        if (fabricWorkspacePaths.Count == 0)
        {
            return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, newWorkspacePath, "missing required option --fabric-workspace <path>.");
        }

        if (string.IsNullOrWhiteSpace(newWorkspacePath))
        {
            return (false, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, newWorkspacePath, "missing required option --new-workspace <path>.");
        }

        return (true, businessWorkspacePath, businessDataVaultWorkspacePath, implementationWorkspacePath, weaveWorkspacePaths, fabricWorkspacePaths, newWorkspacePath, string.Empty);
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
                if (i + 1 >= args.Length)
                {
                    return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "missing value for --workspace.");
                }

                if (!string.IsNullOrWhiteSpace(workspacePath))
                {
                    return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "--workspace can only be provided once.");
                }

                workspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--implementation-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "missing value for --implementation-workspace.");
                }

                if (!string.IsNullOrWhiteSpace(implementationWorkspacePath))
                {
                    return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "--implementation-workspace can only be provided once.");
                }

                implementationWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--data-type-conversion-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "missing value for --data-type-conversion-workspace.");
                }

                if (!string.IsNullOrWhiteSpace(dataTypeConversionWorkspacePath))
                {
                    return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "--data-type-conversion-workspace can only be provided once.");
                }

                dataTypeConversionWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--out", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "missing value for --out.");
                }

                if (!string.IsNullOrWhiteSpace(outputPath))
                {
                    return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "--out can only be provided once.");
                }

                outputPath = args[++i];
                continue;
            }

            return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(workspacePath))
        {
            return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "missing required option --workspace <path>.");
        }

        if (string.IsNullOrWhiteSpace(implementationWorkspacePath))
        {
            return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "missing required option --implementation-workspace <path>.");
        }

        if (string.IsNullOrWhiteSpace(dataTypeConversionWorkspacePath))
        {
            return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "missing required option --data-type-conversion-workspace <path>.");
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            return (false, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, "missing required option --out <path>.");
        }

        return (true, workspacePath, implementationWorkspacePath, dataTypeConversionWorkspacePath, outputPath, string.Empty);
    }


    private static bool IsHelpToken(string value)
    {
        return string.Equals(value, "help", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "--help", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "-h", StringComparison.OrdinalIgnoreCase);
    }

    private static void PrintHelp()
    {
        Presenter.WriteUsage("meta-datavault-business [--new-workspace <path> | <command> [options]]");
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteCommandCatalog(
            "Commands:",
            new[]
            {
                ("help", "Show this help."),
                ("check-business-materialization", "Check the sanctioned input contract for Business Data Vault materialization."),
                ("materialize-business", "Materialize a Business Data Vault workspace from sanctioned inputs."),
                ("generate-sql", "Generate SQL scripts from a materialized Business Data Vault workspace.")
            });
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteNext("meta-datavault-business generate-sql --help");
    }


    private static void PrintCheckBusinessMaterializationHelp()
    {
        Presenter.WriteInfo("Command: check-business-materialization");
        Presenter.WriteUsage("meta-datavault-business check-business-materialization --business-workspace <path> --bdv-workspace <path> --implementation-workspace <path> --weave-workspace <path> [--weave-workspace <path> ...] --fabric-workspace <path> [--fabric-workspace <path> ...]");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Loads sanctioned MetaBusiness, MetaBusinessDataVault, MetaDataVaultImplementation, MetaWeave, and MetaFabric workspaces.");
        Presenter.WriteInfo("  Verifies the current Business -> BusinessDataVault anchor contract before materialization.");
    }

    private static void PrintMaterializeBusinessHelp()
    {
        Presenter.WriteInfo("Command: materialize-business");
        Presenter.WriteUsage("meta-datavault-business materialize-business --business-workspace <path> --bdv-workspace <path> --implementation-workspace <path> --weave-workspace <path> [--weave-workspace <path> ...] --fabric-workspace <path> [--fabric-workspace <path> ...] --new-workspace <path>");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Loads sanctioned MetaBusiness, MetaBusinessDataVault, MetaDataVaultImplementation, MetaWeave, and MetaFabric workspaces.");
        Presenter.WriteInfo("  Applies sanctioned Business Data Vault table name patterns into a new output workspace while keeping semantic row identities stable.");
    }

    private static void PrintGenerateSqlHelp()
    {
        Presenter.WriteInfo("Command: generate-sql");
        Presenter.WriteUsage("meta-datavault-business generate-sql --workspace <path> --implementation-workspace <path> --data-type-conversion-workspace <path> --out <path>");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Loads a materialized MetaBusinessDataVault workspace and emits SQL scripts for hubs, links, same-as links, hierarchical links, references, satellites, point-in-time tables, and bridges.");
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


