using Meta.Core.Presentation.Cli;
using MetaTransformScript.Sql;

internal static partial class Program
{
    private static CliCommandDefinition CreateStoredProcedureViewContractCommand() =>
        new(
            "stored-procedure view-contract",
            "View stored procedure contracts.",
            new[] { "meta-transform-script stored-procedure view-contract [--workspace <path>] [--name <transform-script-name>]" },
            new[]
            {
                new CliOptionDefinition("--workspace <path>", "MetaTransformScript workspace to inspect. Defaults to the current directory."),
                new CliOptionDefinition("--name <transform-script-name>", "Inspect one stored procedure transform script by name.")
            },
            new[]
            {
                "Reports whether each stored procedure has exactly one StoredProcedureContract row.",
                "A present contract is authoritative: omitted operation/result rows mean none are declared."
            },
            ShowInCommandCatalog: false);

    private static CliCommandDefinition CreateStoredProcedureAddContractCommand() =>
        new(
            "stored-procedure add-contract",
            "Add or replace stored procedure contract metadata.",
            new[] { "meta-transform-script stored-procedure add-contract [--workspace <path>] --name <transform-script-name> [--operation <ordinal>:<kind>:<sql-id>[=<role>]] [--result-rowset <name>] [--result-column <rowset>=<column>]" },
            new[]
            {
                new CliOptionDefinition("--workspace <path>", "MetaTransformScript workspace to update. Defaults to the current directory."),
                new CliOptionDefinition("--name <transform-script-name>", "Required. Stored procedure transform script name."),
                new CliOptionDefinition("--operation <ordinal>:<kind>:<sql-id>[=<role>]", "Declare an ordered operation. Kinds: read, append, replace, reset, mutation, call. May be repeated."),
                new CliOptionDefinition("--result-rowset <name>", "Declare the optional result rowset."),
                new CliOptionDefinition("--result-column <rowset>=<column>", "Declare a result column for a named result rowset. May be repeated."),
                new CliOptionDefinition("--notes <text>", "Optional human note stored on the contract.")
            },
            new[]
            {
                "This command replaces the entire contract for one stored procedure.",
                "Omitting --operation or --result-* declares that part empty.",
                "Operations are globally ordered inside the procedure. Use separate reset and append operations when order matters."
            },
            ShowInCommandCatalog: false);

    private static CliCommandDefinition CreateStoredProcedureRemoveContractCommand() =>
        new(
            "stored-procedure remove-contract",
            "Remove a stored procedure contract and its declared rows.",
            new[] { "meta-transform-script stored-procedure remove-contract [--workspace <path>] --name <transform-script-name>" },
            new[]
            {
                new CliOptionDefinition("--workspace <path>", "MetaTransformScript workspace to update. Defaults to the current directory."),
                new CliOptionDefinition("--name <transform-script-name>", "Required. Stored procedure transform script name.")
            },
            new[]
            {
                "Removes the contract row plus operation, result rowset, and result column declaration rows for the stored procedure."
            },
            ShowInCommandCatalog: false);

    private static async Task<int> RunStoredProcedureAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintCommandHelp("stored-procedure");
            return 0;
        }

        if (string.Equals(args[1], "view-contract", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length >= 3 && IsHelpToken(args[2]))
            {
                PrintCommandHelp("stored-procedure view-contract");
                return 0;
            }

            var parse = ParseStoredProcedureViewContractArgs(args, 2);
            if (!parse.Ok)
            {
                return Fail(parse.ErrorMessage, HelpCommand("stored-procedure view-contract"));
            }

            var workspacePath = Path.GetFullPath(parse.WorkspacePath);
            try
            {
                var result = await new MetaTransformScriptSqlService()
                    .InspectStoredProcedureContractsAsync(workspacePath, parse.Name)
                    .ConfigureAwait(false);

                Presenter.WriteInfo($"Stored procedures: {result.StoredProcedureCount}");
                Presenter.WriteKeyValueBlock("Contracts", new[]
                {
                    ("Present", result.ContractedCount.ToString()),
                    ("Missing", result.MissingContractCount.ToString()),
                    ("Invalid", result.InvalidContractCount.ToString()),
                    ("Workspace", result.WorkspacePath)
                });

                foreach (var item in result.Items)
                {
                    WriteStoredProcedureContractInspectionItem(item);
                }

                return 0;
            }
            catch (Exception ex)
            {
                return Fail(
                    "Cannot view stored procedure contracts.",
                    "check the workspace path and optional --name value, then retry.",
                    4,
                    new[]
                    {
                        $"  Workspace: {workspacePath}",
                        $"  {ex.Message}"
                    });
            }
        }

        if (string.Equals(args[1], "add-contract", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length >= 3 && IsHelpToken(args[2]))
            {
                PrintCommandHelp("stored-procedure add-contract");
                return 0;
            }

            var parse = ParseStoredProcedureAddContractArgs(args, 2);
            if (!parse.Ok)
            {
                return Fail(parse.ErrorMessage, HelpCommand("stored-procedure add-contract"));
            }

            var workspacePath = Path.GetFullPath(parse.WorkspacePath);
            try
            {
                var result = await new MetaTransformScriptSqlService()
                    .AddStoredProcedureContractAsync(
                        workspacePath,
                        parse.Name,
                        parse.ToDeclaration())
                    .ConfigureAwait(false);

                Presenter.WriteInfo($"Stored procedure contract written: {result.Item.TransformScriptName}");
                Presenter.WriteKeyValueBlock("Declared", new[]
                {
                    ("Operations", result.Item.OperationCount.ToString()),
                    ("Reads", result.Item.ReadOperationCount.ToString()),
                    ("Writes", result.Item.WriteOperationCount.ToString()),
                    ("Calls", result.Item.CallOperationCount.ToString()),
                    ("ResultRowsets", result.Item.ResultRowsetCount.ToString()),
                    ("ResultColumns", result.Item.ResultColumnCount.ToString()),
                    ("Workspace", result.WorkspacePath)
                });

                return 0;
            }
            catch (Exception ex)
            {
                return Fail(
                    "Cannot add stored procedure contract.",
                    "check the workspace, transform script name, and declaration options, then retry.",
                    4,
                    new[]
                    {
                        $"  Workspace: {workspacePath}",
                        $"  Name: {parse.Name}",
                        $"  {ex.Message}"
                    });
            }
        }

        if (string.Equals(args[1], "remove-contract", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length >= 3 && IsHelpToken(args[2]))
            {
                PrintCommandHelp("stored-procedure remove-contract");
                return 0;
            }

            var parse = ParseStoredProcedureRemoveContractArgs(args, 2);
            if (!parse.Ok)
            {
                return Fail(parse.ErrorMessage, HelpCommand("stored-procedure remove-contract"));
            }

            var workspacePath = Path.GetFullPath(parse.WorkspacePath);
            try
            {
                var result = await new MetaTransformScriptSqlService()
                    .RemoveStoredProcedureContractAsync(workspacePath, parse.Name)
                    .ConfigureAwait(false);

                Presenter.WriteInfo($"Stored procedure contract removed: {result.TransformScriptName}");
                Presenter.WriteKeyValueBlock("Removed", new[]
                {
                    ("Contracts", result.ContractCount.ToString()),
                    ("Operations", result.OperationCount.ToString()),
                    ("ResultRowsets", result.ResultRowsetCount.ToString()),
                    ("ResultColumns", result.ResultColumnCount.ToString()),
                    ("Workspace", result.WorkspacePath)
                });

                return 0;
            }
            catch (Exception ex)
            {
                return Fail(
                    "Cannot remove stored procedure contract.",
                    "check the workspace and transform script name, then retry.",
                    4,
                    new[]
                    {
                        $"  Workspace: {workspacePath}",
                        $"  Name: {parse.Name}",
                        $"  {ex.Message}"
                    });
            }
        }

        return Fail($"unknown stored-procedure operation '{args[1]}'.", HelpCommand("stored-procedure"));
    }

    private static void WriteStoredProcedureContractInspectionItem(
        StoredProcedureContractInspectionItem item)
    {
        Console.Out.WriteLine(item.TransformScriptName);
        Console.Out.WriteLine($"  Contract: {RenderStoredProcedureContractState(item.ContractState, item.ContractRowCount)}");

        Console.Out.WriteLine(
            $"  Declared: operations {item.OperationCount}, reads {item.ReadOperationCount}, writes {item.WriteOperationCount}, calls {item.CallOperationCount}, result rowsets {item.ResultRowsetCount}, result columns {item.ResultColumnCount}");
    }

    private static string RenderStoredProcedureContractState(
        StoredProcedureContractState state,
        int contractRowCount) =>
        state switch
        {
            StoredProcedureContractState.Present => "Present",
            StoredProcedureContractState.Missing => "Missing",
            _ => $"Invalid ({contractRowCount} rows)"
        };

    private sealed record StoredProcedureViewContractArgs(
        bool Ok,
        string WorkspacePath,
        string? Name,
        string ErrorMessage);

    private static StoredProcedureViewContractArgs ParseStoredProcedureViewContractArgs(
        string[] args,
        int startIndex)
    {
        var workspacePath = ".";
        var workspaceSpecified = false;
        string? name = null;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return new(false, workspacePath, name, "missing value for --workspace.");
                if (workspaceSpecified) return new(false, workspacePath, name, "--workspace can only be provided once.");
                workspacePath = args[++i];
                workspaceSpecified = true;
                continue;
            }

            if (string.Equals(arg, "--name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return new(false, workspacePath, name, "missing value for --name.");
                if (!string.IsNullOrWhiteSpace(name)) return new(false, workspacePath, name, "--name can only be provided once.");
                name = args[++i];
                continue;
            }

            return new(false, workspacePath, name, $"unknown option '{arg}'.");
        }

        return new(true, workspacePath, name, string.Empty);
    }

    private sealed record StoredProcedureRemoveContractArgs(
        bool Ok,
        string WorkspacePath,
        string Name,
        string ErrorMessage);

    private static StoredProcedureRemoveContractArgs ParseStoredProcedureRemoveContractArgs(
        string[] args,
        int startIndex)
    {
        var workspacePath = ".";
        var workspaceSpecified = false;
        var name = string.Empty;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return new(false, workspacePath, name, "missing value for --workspace.");
                if (workspaceSpecified) return new(false, workspacePath, name, "--workspace can only be provided once.");
                workspacePath = args[++i];
                workspaceSpecified = true;
                continue;
            }

            if (string.Equals(arg, "--name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return new(false, workspacePath, name, "missing value for --name.");
                if (!string.IsNullOrWhiteSpace(name)) return new(false, workspacePath, name, "--name can only be provided once.");
                name = args[++i];
                continue;
            }

            return new(false, workspacePath, name, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return new(false, workspacePath, name, "missing required option --name <transform-script-name>.");
        }

        return new(true, workspacePath, name, string.Empty);
    }

    private sealed record StoredProcedureAddContractArgs(
        bool Ok,
        string WorkspacePath,
        string Name,
        IReadOnlyList<StoredProcedureContractOperationDeclaration> Operations,
        IReadOnlyList<StoredProcedureResultRowsetDeclaration> Results,
        string? Notes,
        string ErrorMessage)
    {
        public StoredProcedureContractDeclaration ToDeclaration()
        {
            return new StoredProcedureContractDeclaration(
                Operations: Operations,
                ResultRowsets: Results,
                Notes: Notes);
        }
    }

    private static StoredProcedureAddContractArgs ParseStoredProcedureAddContractArgs(
        string[] args,
        int startIndex)
    {
        var workspacePath = ".";
        var workspaceSpecified = false;
        var name = string.Empty;
        var operations = new List<StoredProcedureContractOperationDeclaration>();
        var resultRowsets = new List<ResultRowsetBuilder>();
        string? notes = null;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailAddContract(workspacePath, name, operations, resultRowsets, notes, "missing value for --workspace.");
                if (workspaceSpecified) return FailAddContract(workspacePath, name, operations, resultRowsets, notes, "--workspace can only be provided once.");
                workspacePath = args[++i];
                workspaceSpecified = true;
                continue;
            }

            if (string.Equals(arg, "--name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailAddContract(workspacePath, name, operations, resultRowsets, notes, "missing value for --name.");
                if (!string.IsNullOrWhiteSpace(name)) return FailAddContract(workspacePath, name, operations, resultRowsets, notes, "--name can only be provided once.");
                name = args[++i];
                continue;
            }

            if (string.Equals(arg, "--operation", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailAddContract(workspacePath, name, operations, resultRowsets, notes, "missing value for --operation.");
                var operation = ParseStoredProcedureOperation(args[++i], out var operationError);
                if (operation is null)
                {
                    return FailAddContract(workspacePath, name, operations, resultRowsets, notes, operationError);
                }

                operations.Add(operation);
                continue;
            }

            if (string.Equals(arg, "--result-rowset", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailAddContract(workspacePath, name, operations, resultRowsets, notes, "missing value for --result-rowset.");
                var value = args[++i].Trim();
                if (string.IsNullOrWhiteSpace(value)) return FailAddContract(workspacePath, name, operations, resultRowsets, notes, "--result-rowset requires a name.");
                GetOrAddResultRowset(resultRowsets, value);
                continue;
            }

            if (string.Equals(arg, "--result-column", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailAddContract(workspacePath, name, operations, resultRowsets, notes, "missing value for --result-column.");
                var (rowsetName, columnName) = SplitRequiredAssignment(args[++i]);
                if (string.IsNullOrWhiteSpace(rowsetName) || string.IsNullOrWhiteSpace(columnName))
                {
                    return FailAddContract(workspacePath, name, operations, resultRowsets, notes, "--result-column requires <rowset>=<column>.");
                }

                GetOrAddResultRowset(resultRowsets, rowsetName)
                    .Columns
                    .Add(new StoredProcedureResultColumnDeclaration(columnName, MetaDataTypeId: null, IsNullable: null));
                continue;
            }

            if (string.Equals(arg, "--notes", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailAddContract(workspacePath, name, operations, resultRowsets, notes, "missing value for --notes.");
                if (!string.IsNullOrWhiteSpace(notes)) return FailAddContract(workspacePath, name, operations, resultRowsets, notes, "--notes can only be provided once.");
                notes = args[++i];
                continue;
            }

            return FailAddContract(workspacePath, name, operations, resultRowsets, notes, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return FailAddContract(workspacePath, name, operations, resultRowsets, notes, "missing required option --name <transform-script-name>.");
        }

        if (resultRowsets.Count > 1)
        {
            return FailAddContract(workspacePath, name, operations, resultRowsets, notes, "stored procedure contracts support at most one result rowset.");
        }

        return new(
            true,
            workspacePath,
            name,
            operations,
            resultRowsets.Select(static item => item.ToDeclaration()).ToArray(),
            notes,
            string.Empty);
    }

    private static StoredProcedureAddContractArgs FailAddContract(
        string workspacePath,
        string name,
        IReadOnlyList<StoredProcedureContractOperationDeclaration> operations,
        IReadOnlyList<ResultRowsetBuilder> resultRowsets,
        string? notes,
        string errorMessage) =>
        new(
            false,
            workspacePath,
            name,
            operations,
            resultRowsets.Select(static item => item.ToDeclaration()).ToArray(),
            notes,
            errorMessage);

    private static StoredProcedureContractOperationDeclaration? ParseStoredProcedureOperation(
        string value,
        out string errorMessage)
    {
        errorMessage = string.Empty;
        var firstSeparator = value.IndexOf(':');
        var secondSeparator = firstSeparator < 0 ? -1 : value.IndexOf(':', firstSeparator + 1);
        if (firstSeparator <= 0 || secondSeparator <= firstSeparator + 1)
        {
            errorMessage = "--operation requires <ordinal>:<kind>:<sql-id>[=<role>].";
            return null;
        }

        var ordinalText = value[..firstSeparator].Trim();
        if (!int.TryParse(ordinalText, out var ordinal) || ordinal < 0)
        {
            errorMessage = "--operation ordinal must be a non-negative integer.";
            return null;
        }

        var operationKind = value[(firstSeparator + 1)..secondSeparator].Trim();
        var normalizedKind = MetaTransformScriptSqlService.NormalizeStoredProcedureOperationKind(operationKind);
        if (normalizedKind is null)
        {
            errorMessage = $"unsupported --operation kind '{operationKind}'. Supported values: read, append, replace, reset, mutation, call.";
            return null;
        }

        var (sqlIdentifier, accessRole) = SplitOptionalAssignment(value[(secondSeparator + 1)..]);
        if (string.IsNullOrWhiteSpace(sqlIdentifier))
        {
            errorMessage = "--operation requires a SQL identifier.";
            return null;
        }

        return new StoredProcedureContractOperationDeclaration(ordinal, normalizedKind, sqlIdentifier, accessRole);
    }

    private static (string Left, string? Right) SplitOptionalAssignment(string value)
    {
        var index = value.IndexOf('=');
        if (index < 0)
        {
            return (value.Trim(), null);
        }

        var left = value[..index].Trim();
        var right = value[(index + 1)..].Trim();
        return (left, string.IsNullOrWhiteSpace(right) ? null : right);
    }

    private static (string Left, string Right) SplitRequiredAssignment(string value)
    {
        var index = value.IndexOf('=');
        return index < 0
            ? (string.Empty, string.Empty)
            : (value[..index].Trim(), value[(index + 1)..].Trim());
    }

    private static ResultRowsetBuilder GetOrAddResultRowset(
        List<ResultRowsetBuilder> rowsets,
        string name)
    {
        var existing = rowsets.FirstOrDefault(item =>
            string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
        {
            return existing;
        }

        var created = new ResultRowsetBuilder(name);
        rowsets.Add(created);
        return created;
    }

    private sealed record ResultRowsetBuilder(string Name)
    {
        public List<StoredProcedureResultColumnDeclaration> Columns { get; } = [];

        public StoredProcedureResultRowsetDeclaration ToDeclaration() =>
            new(Name, Columns);
    }
}
