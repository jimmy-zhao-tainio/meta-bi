using Meta.Core.Domain;
using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using Meta.Core.Services;
using MetaTabular.Core;
using MetaTabular.Core.Deploy;

internal static class Program
{
    private const string AppName = "meta-tabular";

    private static readonly ConsolePresenter Presenter = new();
    private static readonly IReadOnlyDictionary<string, ModelDrivenAddCommandSpec> AddCommands =
        ModelDrivenAddCommandCatalog.Build(MetaTabularModels.CreateMetaTabularModel());
    private static readonly Lazy<IReadOnlyList<CliCommandRoute>> CommandRoutesLazy = new(BuildCommandRoutes);
    private static readonly Lazy<IReadOnlyDictionary<string, CliCommandRoute>> CommandRoutesByNameLazy = new(
        () => CommandRoutes.ToDictionary(route => route.Definition.Name, StringComparer.OrdinalIgnoreCase));
    private static readonly Lazy<CliAppDefinition> CliLazy = new(
        () => new CliAppDefinition(
            AppName,
            new[]
            {
                "meta-tabular [--new-workspace <path> | <command> [options]]"
            },
            CommandRoutes.Select(route => route.Definition).ToArray(),
            Next: "meta-tabular add-tabular-model --help"));

    private static IReadOnlyList<CliCommandRoute> CommandRoutes => CommandRoutesLazy.Value;

    private static IReadOnlyDictionary<string, CliCommandRoute> CommandRoutesByName => CommandRoutesByNameLazy.Value;

    private static CliAppDefinition Cli => CliLazy.Value;

    private static IReadOnlyList<CliCommandRoute> BuildCommandRoutes()
    {
        var routes = new List<CliCommandRoute>
        {
            new(
                new CliCommandDefinition(
                    "help",
                    "Show this help.",
                    new[] { "meta-tabular help" }),
                _ =>
                {
                    PrintHelp();
                    return Task.FromResult(0);
                }),
            new(
                new CliCommandDefinition(
                    "--new-workspace",
                    "Create an empty MetaTabular workspace.",
                    new[] { "meta-tabular --new-workspace <path>" },
                    new[]
                    {
                        new CliOptionDefinition("--new-workspace <path>", "Required. Directory where the empty MetaTabular workspace will be created.")
                    }),
                RunNewWorkspaceAsync),
            new(CreateDeployCommandDefinition(), RunDeployAsync),
            new(CreateRestoreCommandDefinition(), RunRestoreAsync),
            new(CreateDropCommandDefinition(), RunDropAsync)
        };

        routes.AddRange(BuildAddCommandRoutes());
        return routes;
    }

    private static CliCommandDefinition CreateDeployCommandDefinition() =>
        new(
            "deploy",
            "Create modeled objects on an Analysis Services tabular instance.",
            new[] { "meta-tabular deploy [--workspace <path>] --server <server> [--database-name <name>] [--drop-existing] [--no-process]" },
            new[]
            {
                new CliOptionDefinition("--workspace <path>", "MetaTabular workspace to deploy. Defaults to the current directory."),
                new CliOptionDefinition("--server <server>", "Required. Analysis Services tabular server."),
                new CliOptionDefinition("--database-name <name>", "Optional target database name. Defaults to the modeled database name."),
                new CliOptionDefinition("--drop-existing", "Drop an existing target database before create/deploy."),
                new CliOptionDefinition("--no-process", "Deploy metadata only and skip full processing.")
            },
            new[]
            {
                "Creates tabular database objects on an Analysis Services tabular instance.",
                "By default, the command runs full processing after deploy and fails if processing fails.",
                "Without --drop-existing, the command fails if the database already exists.",
                "With --drop-existing, the command uses the safe drop, create, full-process sequence.",
                "With --no-process, the command deploys metadata only.",
                "This deploys modeled data sources, tables, columns, partitions, measures, relationships, calculation groups, and role filters."
            });

    private static CliCommandDefinition CreateRestoreCommandDefinition() =>
        new(
            "restore",
            "Promote a processed tabular database through backup and restore.",
            new[] { "meta-tabular restore --source-server <server> --source-database-name <name> --target-server <server> --target-database-name <name> --backup-file <path> [--drop-existing] [--overwrite-backup-file]" },
            new[]
            {
                new CliOptionDefinition("--source-server <server>", "Required. Source Analysis Services server containing the processed database."),
                new CliOptionDefinition("--source-database-name <name>", "Required. Source processed database name."),
                new CliOptionDefinition("--target-server <server>", "Required. Target Analysis Services server."),
                new CliOptionDefinition("--target-database-name <name>", "Required. Target database name to restore."),
                new CliOptionDefinition("--backup-file <path>", "Required. Backup file path accessible to the Analysis Services service accounts."),
                new CliOptionDefinition("--drop-existing", "Drop an existing target database before restore."),
                new CliOptionDefinition("--overwrite-backup-file", "Overwrite an existing backup file.")
            },
            new[]
            {
                "Backs up a processed source tabular database and restores it as the target database.",
                "Use this for pre-prod-to-prod promotion after pre-prod deploy and processing succeeds.",
                "If the target database exists, --drop-existing is required before restore.",
                "Restore does not process. Partial or object-level processing belongs in a separate command.",
                "The backup file path must be accessible to the Analysis Services service accounts on both source and target servers."
            });

    private static CliCommandDefinition CreateDropCommandDefinition() =>
        new(
            "drop",
            "Drop a tabular database from an Analysis Services tabular instance.",
            new[] { "meta-tabular drop --server <server> --database-name <name>" },
            new[]
            {
                new CliOptionDefinition("--server <server>", "Required. Analysis Services tabular server."),
                new CliOptionDefinition("--database-name <name>", "Required. Database name to drop.")
            },
            new[]
            {
                "Drops a tabular database from an Analysis Services tabular instance.",
                "This command has no confirmation prompt; use it only with an explicit database name.",
                "The command fails if the database does not exist."
            });

    static async Task<int> Main(string[] args)
    {
        if (Meta.Core.Presentation.Cli.CliVersion.TryWriteVersion(Presenter, Cli.Name, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        if (args.Length == 0 || IsHelpToken(args[0]))
        {
            PrintHelp();
            return 0;
        }

        if (CommandRoutesByName.TryGetValue(args[0], out var route))
        {
            return await route.ExecuteAsync(args).ConfigureAwait(false);
        }

        return Fail($"unknown command '{args[0]}'.", $"{AppName} help");
    }

    private static async Task<int> RunNewWorkspaceAsync(string[] args)
    {
        if (args.Length > 1 && IsHelpToken(args[1]))
        {
            PrintCommandHelp("--new-workspace");
            return 0;
        }

        var parse = ParseNewWorkspaceOnly(args);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("--new-workspace"));
        }

        var targetValidation = CliNewWorkspaceTargetValidator.Validate(parse.NewWorkspacePath);
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
        var workspace = MetaTabularWorkspaces.CreateEmptyMetaTabularWorkspace(workspacePath);
        var validation = new ValidationService().Validate(workspace);
        if (validation.HasErrors)
        {
            return Fail(
                "metatabular workspace is invalid.",
                "fix the sanctioned model and retry workspace creation.",
                4,
                validation.Issues
                    .Where(item => item.Severity == IssueSeverity.Error)
                    .Select(item => $"  - {item.Code}: {item.Message}"));
        }

        await new WorkspaceService().SaveAsync(workspace).ConfigureAwait(false);
        Presenter.WriteOk($"Created {Path.GetFileName(workspacePath)}");
        return 0;
    }

    private static async Task<int> RunDeployAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintDeployHelp();
            return 0;
        }

        var parse = ParseDeployCommand(args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("deploy"));
        }

        try
        {
            var result = await new MetaTabularDeployService()
                .DeployAsync(new MetaTabularDeployRequest
                {
                    WorkspacePath = Path.GetFullPath(parse.WorkspacePath),
                    Server = parse.Server,
                    DatabaseName = parse.DatabaseName,
                    DropExisting = parse.DropExisting,
                    Process = parse.Process,
                })
                .ConfigureAwait(false);

            Presenter.WriteOk($"Deployed {result.DatabaseName} to {result.Server}");
            Presenter.WriteInfo(result.DropExisting ? "Mode: drop, create" : "Mode: create");
            Presenter.WriteInfo(result.Processed ? "Process: full" : "Process: skipped");
            Presenter.WriteInfo($"Tables: {result.TableCount}");
            Presenter.WriteInfo($"Columns: {result.ColumnCount}");
            Presenter.WriteInfo($"Measures: {result.MeasureCount}");
            Presenter.WriteInfo($"Relationships: {result.RelationshipCount}");
            return 0;
        }
        catch (Exception ex) when (IsExpectedDeployException(ex))
        {
            return Fail(
                "Cannot deploy tabular database.",
                HelpCommand("deploy"),
                4,
                [$"  {ex.Message}"]);
        }
    }

    private static async Task<int> RunRestoreAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintRestoreHelp();
            return 0;
        }

        var parse = ParseRestoreCommand(args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("restore"));
        }

        try
        {
            var result = await new MetaTabularRestoreService()
                .RestoreAsync(new MetaTabularRestoreRequest
                {
                    SourceServer = parse.SourceServer,
                    SourceDatabaseName = parse.SourceDatabaseName,
                    TargetServer = parse.TargetServer,
                    TargetDatabaseName = parse.TargetDatabaseName,
                    BackupFile = parse.BackupFile,
                    DropExisting = parse.DropExisting,
                    OverwriteBackupFile = parse.OverwriteBackupFile,
                })
                .ConfigureAwait(false);

            Presenter.WriteOk($"Restored {result.TargetDatabaseName} to {result.TargetServer}");
            Presenter.WriteInfo($"Source: {result.SourceServer}/{result.SourceDatabaseName}");
            Presenter.WriteInfo($"Backup file: {result.BackupFile}");
            Presenter.WriteInfo(result.DroppedExisting ? "Mode: drop target, restore" : "Mode: restore new target");
            Presenter.WriteInfo(result.OverwriteBackupFile ? "Backup file mode: overwrite" : "Backup file mode: create");
            return 0;
        }
        catch (Exception ex) when (IsExpectedDeployException(ex))
        {
            return Fail(
                "Cannot restore tabular database.",
                HelpCommand("restore"),
                4,
                [$"  {ex.Message}"]);
        }
    }

    private static async Task<int> RunDropAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintDropHelp();
            return 0;
        }

        var parse = ParseDropCommand(args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("drop"));
        }

        try
        {
            var result = await new MetaTabularDropService()
                .DropAsync(new MetaTabularDropRequest
                {
                    Server = parse.Server,
                    DatabaseName = parse.DatabaseName,
                })
                .ConfigureAwait(false);

            Presenter.WriteOk($"Dropped {result.DatabaseName} from {result.Server}");
            return 0;
        }
        catch (Exception ex) when (IsExpectedDeployException(ex))
        {
            return Fail(
                "Cannot drop tabular database.",
                HelpCommand("drop"),
                4,
                [$"  {ex.Message}"]);
        }
    }

    private static IReadOnlyList<CliCommandRoute> BuildAddCommandRoutes()
    {
        return AddCommands.Values
            .OrderBy(spec => spec.CommandName, StringComparer.Ordinal)
            .Select(spec => new CliCommandRoute(CreateAddCommandDefinition(spec), args => RunAddCommandAsync(spec, args)))
            .ToArray();
    }

    private static CliCommandDefinition CreateAddCommandDefinition(ModelDrivenAddCommandSpec spec)
    {
        var usageParts = new List<string> { $"{AppName} {spec.CommandName}", "[--workspace <path>]", "--id <id>" };
        usageParts.AddRange(spec.PropertyOptions.Select(item => item.Required ? $"{item.OptionName} {item.ValueLabel}" : $"[{item.OptionName} {item.ValueLabel}]"));
        usageParts.AddRange(spec.RelationshipOptions.Select(item => item.Required ? $"{item.OptionName} {item.ValueLabel}" : $"[{item.OptionName} {item.ValueLabel}]"));

        var options = new List<CliOptionDefinition>
        {
            new("--workspace <path>", "Optional. Workspace path. Default: current working directory."),
            new("--id <id>", $"Required. {spec.EntityName} row id."),
        };
        options.AddRange(spec.PropertyOptions.Select(item =>
            new CliOptionDefinition(
                $"{item.OptionName} {item.ValueLabel}",
                $"{(item.Required ? "Required" : "Optional")}. {item.PropertyName}.")));
        options.AddRange(spec.RelationshipOptions.Select(item =>
            new CliOptionDefinition(
                $"{item.OptionName} {item.ValueLabel}",
                $"{(item.Required ? "Required" : "Optional")}. {item.TargetEntityName} id for {item.ColumnName}.")));

        return new CliCommandDefinition(
            spec.CommandName,
            spec.Description,
            new[] { string.Join(" ", usageParts) },
            options,
            new[]
            {
                $"Adds one {spec.EntityName} row to a MetaTabular workspace.",
                "Defaults to the current working directory when --workspace is omitted."
            });
    }

    private static async Task<int> RunAddCommandAsync(ModelDrivenAddCommandSpec spec, string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintAddCommandHelp(spec);
            return 0;
        }

        var parse = ParseAddCommand(spec, args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand(spec.CommandName));
        }

        try
        {
            var request = new TabularAuthoringRequest
            {
                WorkspacePath = Path.GetFullPath(parse.WorkspacePath),
                EntityName = spec.EntityName,
                RecordId = parse.RecordId,
            };
            foreach (var value in parse.Values) request.Values[value.Key] = value.Value;
            request.Relationships.AddRange(parse.Relationships);
            await new TabularAuthoringService().AddRecordAsync(request).ConfigureAwait(false);

            Presenter.WriteOk($"Added {parse.RecordId} to {spec.EntityName}");
            return 0;
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or IOException or UnauthorizedAccessException)
        {
            return Fail(
                "Cannot update tabular workspace.",
                HelpCommand(spec.CommandName),
                4,
                [$"  {ex.Message}"]);
        }
    }

    private static (bool Ok, string NewWorkspacePath, string ErrorMessage) ParseNewWorkspaceOnly(string[] args)
    {
        var newWorkspacePath = string.Empty;
        for (var i = 0; i < args.Length; i++)
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

    private static ParsedAddCommand ParseAddCommand(ModelDrivenAddCommandSpec spec, string[] args, int startIndex)
    {
        var workspacePath = ".";
        var recordId = string.Empty;
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var relationships = new List<TabularRelationshipAssignment>();
        var propertyOptions = spec.PropertyOptions.ToDictionary(item => item.OptionName, StringComparer.OrdinalIgnoreCase);
        var relationshipOptions = spec.RelationshipOptions.ToDictionary(item => item.OptionName, StringComparer.OrdinalIgnoreCase);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (i + 1 >= args.Length)
            {
                return new ParsedAddCommand(false, workspacePath, recordId, values, relationships, $"missing value for {arg}.");
            }

            var value = args[++i];
            if (!seen.Add(arg))
            {
                return new ParsedAddCommand(false, workspacePath, recordId, values, relationships, $"{arg} can only be provided once.");
            }

            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                workspacePath = value;
                continue;
            }

            if (string.Equals(arg, "--id", StringComparison.OrdinalIgnoreCase))
            {
                recordId = value;
                continue;
            }

            if (propertyOptions.TryGetValue(arg, out var prop))
            {
                values[prop.PropertyName] = value;
                continue;
            }

            if (relationshipOptions.TryGetValue(arg, out var rel))
            {
                relationships.Add(new TabularRelationshipAssignment(rel.ColumnName, rel.TargetEntityName, value));
                continue;
            }

            return new ParsedAddCommand(false, workspacePath, recordId, values, relationships, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(recordId))
        {
            return new ParsedAddCommand(false, workspacePath, recordId, values, relationships, "missing required option --id <id>.");
        }

        foreach (var prop in spec.PropertyOptions.Where(item => item.Required))
        {
            if (!values.ContainsKey(prop.PropertyName) || string.IsNullOrWhiteSpace(values[prop.PropertyName]))
            {
                return new ParsedAddCommand(false, workspacePath, recordId, values, relationships, $"missing required option {prop.OptionName} {prop.ValueLabel}.");
            }
        }

        foreach (var rel in spec.RelationshipOptions.Where(item => item.Required))
        {
            if (!relationships.Any(item => string.Equals(item.ColumnName, rel.ColumnName, StringComparison.OrdinalIgnoreCase)))
            {
                return new ParsedAddCommand(false, workspacePath, recordId, values, relationships, $"missing required option {rel.OptionName} {rel.ValueLabel}.");
            }
        }

        return new ParsedAddCommand(true, workspacePath, recordId, values, relationships, string.Empty);
    }

    private static ParsedDeployCommand ParseDeployCommand(string[] args, int startIndex)
    {
        var workspacePath = ".";
        var server = string.Empty;
        string? databaseName = null;
        var dropExisting = false;
        var process = true;
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--drop-existing", StringComparison.OrdinalIgnoreCase))
            {
                if (!seen.Add(arg))
                {
                    return new ParsedDeployCommand(false, workspacePath, server, databaseName, dropExisting, process, "--drop-existing can only be provided once.");
                }

                dropExisting = true;
                continue;
            }

            if (string.Equals(arg, "--no-process", StringComparison.OrdinalIgnoreCase))
            {
                if (!seen.Add(arg))
                {
                    return new ParsedDeployCommand(false, workspacePath, server, databaseName, dropExisting, process, "--no-process can only be provided once.");
                }

                process = false;
                continue;
            }

            if (i + 1 >= args.Length)
            {
                return new ParsedDeployCommand(false, workspacePath, server, databaseName, dropExisting, process, $"missing value for {arg}.");
            }

            var value = args[++i];
            if (!seen.Add(arg))
            {
                return new ParsedDeployCommand(false, workspacePath, server, databaseName, dropExisting, process, $"{arg} can only be provided once.");
            }

            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                workspacePath = value;
                continue;
            }

            if (string.Equals(arg, "--server", StringComparison.OrdinalIgnoreCase))
            {
                server = value;
                continue;
            }

            if (string.Equals(arg, "--database-name", StringComparison.OrdinalIgnoreCase))
            {
                databaseName = value;
                continue;
            }

            return new ParsedDeployCommand(false, workspacePath, server, databaseName, dropExisting, process, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(server))
        {
            return new ParsedDeployCommand(false, workspacePath, server, databaseName, dropExisting, process, "missing required option --server <server>.");
        }

        return new ParsedDeployCommand(true, workspacePath, server, databaseName, dropExisting, process, string.Empty);
    }

    private static ParsedRestoreCommand ParseRestoreCommand(string[] args, int startIndex)
    {
        var sourceServer = string.Empty;
        var sourceDatabaseName = string.Empty;
        var targetServer = string.Empty;
        var targetDatabaseName = string.Empty;
        var backupFile = string.Empty;
        var dropExisting = false;
        var overwriteBackupFile = false;
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--drop-existing", StringComparison.OrdinalIgnoreCase))
            {
                if (!seen.Add(arg))
                {
                    return new ParsedRestoreCommand(false, sourceServer, sourceDatabaseName, targetServer, targetDatabaseName, backupFile, dropExisting, overwriteBackupFile, "--drop-existing can only be provided once.");
                }

                dropExisting = true;
                continue;
            }

            if (string.Equals(arg, "--overwrite-backup-file", StringComparison.OrdinalIgnoreCase))
            {
                if (!seen.Add(arg))
                {
                    return new ParsedRestoreCommand(false, sourceServer, sourceDatabaseName, targetServer, targetDatabaseName, backupFile, dropExisting, overwriteBackupFile, "--overwrite-backup-file can only be provided once.");
                }

                overwriteBackupFile = true;
                continue;
            }

            if (i + 1 >= args.Length)
            {
                return new ParsedRestoreCommand(false, sourceServer, sourceDatabaseName, targetServer, targetDatabaseName, backupFile, dropExisting, overwriteBackupFile, $"missing value for {arg}.");
            }

            var value = args[++i];
            if (!seen.Add(arg))
            {
                return new ParsedRestoreCommand(false, sourceServer, sourceDatabaseName, targetServer, targetDatabaseName, backupFile, dropExisting, overwriteBackupFile, $"{arg} can only be provided once.");
            }

            if (string.Equals(arg, "--source-server", StringComparison.OrdinalIgnoreCase))
            {
                sourceServer = value;
                continue;
            }

            if (string.Equals(arg, "--source-database-name", StringComparison.OrdinalIgnoreCase))
            {
                sourceDatabaseName = value;
                continue;
            }

            if (string.Equals(arg, "--target-server", StringComparison.OrdinalIgnoreCase))
            {
                targetServer = value;
                continue;
            }

            if (string.Equals(arg, "--target-database-name", StringComparison.OrdinalIgnoreCase))
            {
                targetDatabaseName = value;
                continue;
            }

            if (string.Equals(arg, "--backup-file", StringComparison.OrdinalIgnoreCase))
            {
                backupFile = value;
                continue;
            }

            return new ParsedRestoreCommand(false, sourceServer, sourceDatabaseName, targetServer, targetDatabaseName, backupFile, dropExisting, overwriteBackupFile, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(sourceServer))
        {
            return new ParsedRestoreCommand(false, sourceServer, sourceDatabaseName, targetServer, targetDatabaseName, backupFile, dropExisting, overwriteBackupFile, "missing required option --source-server <server>.");
        }

        if (string.IsNullOrWhiteSpace(sourceDatabaseName))
        {
            return new ParsedRestoreCommand(false, sourceServer, sourceDatabaseName, targetServer, targetDatabaseName, backupFile, dropExisting, overwriteBackupFile, "missing required option --source-database-name <name>.");
        }

        if (string.IsNullOrWhiteSpace(targetServer))
        {
            return new ParsedRestoreCommand(false, sourceServer, sourceDatabaseName, targetServer, targetDatabaseName, backupFile, dropExisting, overwriteBackupFile, "missing required option --target-server <server>.");
        }

        if (string.IsNullOrWhiteSpace(targetDatabaseName))
        {
            return new ParsedRestoreCommand(false, sourceServer, sourceDatabaseName, targetServer, targetDatabaseName, backupFile, dropExisting, overwriteBackupFile, "missing required option --target-database-name <name>.");
        }

        if (string.IsNullOrWhiteSpace(backupFile))
        {
            return new ParsedRestoreCommand(false, sourceServer, sourceDatabaseName, targetServer, targetDatabaseName, backupFile, dropExisting, overwriteBackupFile, "missing required option --backup-file <path>.");
        }

        return new ParsedRestoreCommand(true, sourceServer, sourceDatabaseName, targetServer, targetDatabaseName, backupFile, dropExisting, overwriteBackupFile, string.Empty);
    }

    private static ParsedDropCommand ParseDropCommand(string[] args, int startIndex)
    {
        var server = string.Empty;
        var databaseName = string.Empty;
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (i + 1 >= args.Length)
            {
                return new ParsedDropCommand(false, server, databaseName, $"missing value for {arg}.");
            }

            var value = args[++i];
            if (!seen.Add(arg))
            {
                return new ParsedDropCommand(false, server, databaseName, $"{arg} can only be provided once.");
            }

            if (string.Equals(arg, "--server", StringComparison.OrdinalIgnoreCase))
            {
                server = value;
                continue;
            }

            if (string.Equals(arg, "--database-name", StringComparison.OrdinalIgnoreCase))
            {
                databaseName = value;
                continue;
            }

            return new ParsedDropCommand(false, server, databaseName, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(server))
        {
            return new ParsedDropCommand(false, server, databaseName, "missing required option --server <server>.");
        }

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            return new ParsedDropCommand(false, server, databaseName, "missing required option --database-name <name>.");
        }

        return new ParsedDropCommand(true, server, databaseName, string.Empty);
    }

    private static void PrintAddCommandHelp(ModelDrivenAddCommandSpec spec)
    {
        PrintCommandHelp(spec.CommandName);
    }

    private static void PrintDeployHelp()
    {
        PrintCommandHelp("deploy");
    }

    private static void PrintRestoreHelp()
    {
        PrintCommandHelp("restore");
    }

    private static void PrintDropHelp()
    {
        PrintCommandHelp("drop");
    }

    private static bool IsHelpToken(string value)
    {
        return string.Equals(value, "help", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "--help", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "-h", StringComparison.OrdinalIgnoreCase);
    }

    private static void PrintHelp()
    {
        CliHelpRenderer.WriteAppHelp(Presenter, Cli);
    }

    private static void PrintCommandHelp(string commandName)
    {
        CliHelpRenderer.WriteCommandHelp(Presenter, Cli, Cli.GetCommand(commandName));
    }

    private static string HelpCommand(string commandName) => Cli.GetCommand(commandName).HelpCommand(Cli.Name);

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

    private static bool IsExpectedDeployException(Exception ex)
    {
        var fullName = ex.GetType().FullName ?? string.Empty;
        return ex is ArgumentException or InvalidOperationException or IOException or UnauthorizedAccessException ||
               fullName.Contains("Adomd", StringComparison.OrdinalIgnoreCase) ||
               fullName.Contains("AnalysisServices", StringComparison.OrdinalIgnoreCase);
    }

    private sealed record ParsedAddCommand(
        bool Ok,
        string WorkspacePath,
        string RecordId,
        Dictionary<string, string> Values,
        List<TabularRelationshipAssignment> Relationships,
        string ErrorMessage);

    private sealed record ParsedDeployCommand(
        bool Ok,
        string WorkspacePath,
        string Server,
        string? DatabaseName,
        bool DropExisting,
        bool Process,
        string ErrorMessage);

    private sealed record ParsedDropCommand(
        bool Ok,
        string Server,
        string DatabaseName,
        string ErrorMessage);

    private sealed record ParsedRestoreCommand(
        bool Ok,
        string SourceServer,
        string SourceDatabaseName,
        string TargetServer,
        string TargetDatabaseName,
        string BackupFile,
        bool DropExisting,
        bool OverwriteBackupFile,
        string ErrorMessage);
}
