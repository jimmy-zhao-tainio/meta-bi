using System.Globalization;
using MetaTransformScript.Instance;
using MTS = global::MetaTransformScript;

namespace MetaTransformScript.Sql;

public sealed partial class MetaTransformScriptSqlService
{
    public async Task<StoredProcedureContractInspectionResult> InspectStoredProcedureContractsAsync(
        string workspacePath,
        string? transformScriptName = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        cancellationToken.ThrowIfCancellationRequested();

        var workspaceFullPath = Path.GetFullPath(workspacePath);
        var model = await MetaTransformScriptInstance
            .LoadFromWorkspaceAsync(workspaceFullPath, searchUpward: false, cancellationToken)
            .ConfigureAwait(false);

        var items = BuildStoredProcedureContractInspectionItems(model, transformScriptName);
        return new StoredProcedureContractInspectionResult(
            workspaceFullPath,
            items.Count,
            items.Count(static item => item.ContractState == StoredProcedureContractState.Present),
            items.Count(static item => item.ContractState == StoredProcedureContractState.Missing),
            items.Count(static item => item.ContractState == StoredProcedureContractState.Invalid),
            items);
    }

    public async Task<StoredProcedureContractDeclarationResult> AddStoredProcedureContractAsync(
        string workspacePath,
        string transformScriptName,
        StoredProcedureContractDeclaration declaration,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(transformScriptName);
        ArgumentNullException.ThrowIfNull(declaration);
        cancellationToken.ThrowIfCancellationRequested();
        ValidateStoredProcedureContractDeclaration(declaration);

        var workspaceFullPath = Path.GetFullPath(workspacePath);
        var model = await MetaTransformScriptInstance
            .LoadFromWorkspaceAsync(workspaceFullPath, searchUpward: false, cancellationToken)
            .ConfigureAwait(false);

        var (script, storedProcedure) = ResolveStoredProcedure(model, transformScriptName);
        var storedProcedureId = storedProcedure.Id;
        var removedContractIds = model.StoredProcedureContractList
            .Where(item => string.Equals(item.ScriptObjectStoredProcedure.Id, storedProcedureId, StringComparison.Ordinal))
            .Select(static item => item.Id)
            .ToHashSet(StringComparer.Ordinal);

        model.StoredProcedureContractList.RemoveAll(item =>
            string.Equals(item.ScriptObjectStoredProcedure.Id, storedProcedureId, StringComparison.Ordinal));
        RemoveStoredProcedureContractRows(model, removedContractIds);

        var contract = new MTS.StoredProcedureContract
        {
            Id = Guid.NewGuid().ToString("N"),
            ScriptObjectStoredProcedure = storedProcedure,
            Notes = string.IsNullOrWhiteSpace(declaration.Notes) ? null : declaration.Notes
        };
        model.StoredProcedureContractList.Add(contract);
        AddOperations(model, contract, declaration.Operations);
        AddResultRowsets(model, contract, declaration.ResultRowsets);

        await MetaTransformScriptInstance
            .SaveToWorkspaceAsync(model, workspaceFullPath, cancellationToken)
            .ConfigureAwait(false);

        var item = BuildStoredProcedureContractInspectionItem(model, script, storedProcedure);
        return new StoredProcedureContractDeclarationResult(workspaceFullPath, item);
    }

    public async Task<StoredProcedureContractRemovalResult> RemoveStoredProcedureContractAsync(
        string workspacePath,
        string transformScriptName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(transformScriptName);
        cancellationToken.ThrowIfCancellationRequested();

        var workspaceFullPath = Path.GetFullPath(workspacePath);
        var model = await MetaTransformScriptInstance
            .LoadFromWorkspaceAsync(workspaceFullPath, searchUpward: false, cancellationToken)
            .ConfigureAwait(false);

        var (script, storedProcedure) = ResolveStoredProcedure(model, transformScriptName);
        var storedProcedureId = storedProcedure.Id;
        var removedContractIds = model.StoredProcedureContractList
            .Where(item => string.Equals(item.ScriptObjectStoredProcedure.Id, storedProcedureId, StringComparison.Ordinal))
            .Select(static item => item.Id)
            .ToHashSet(StringComparer.Ordinal);
        var resultRowsetIds = model.StoredProcedureResultRowsetItemList
            .Where(item => removedContractIds.Contains(item.StoredProcedureContract.Id))
            .Select(static item => item.Id)
            .ToHashSet(StringComparer.Ordinal);

        var contractCount = model.StoredProcedureContractList.RemoveAll(item =>
            string.Equals(item.ScriptObjectStoredProcedure.Id, storedProcedureId, StringComparison.Ordinal));
        var operationCount = model.StoredProcedureContractOperationList.RemoveAll(item =>
            removedContractIds.Contains(item.StoredProcedureContract.Id));
        var resultColumnCount = model.StoredProcedureResultColumnItemList.RemoveAll(item =>
            resultRowsetIds.Contains(item.StoredProcedureResultRowsetItem.Id));
        var resultRowsetCount = model.StoredProcedureResultRowsetItemList.RemoveAll(item =>
            removedContractIds.Contains(item.StoredProcedureContract.Id));

        await MetaTransformScriptInstance
            .SaveToWorkspaceAsync(model, workspaceFullPath, cancellationToken)
            .ConfigureAwait(false);

        return new StoredProcedureContractRemovalResult(
            workspaceFullPath,
            script.Id,
            script.Name,
            storedProcedureId,
            contractCount,
            operationCount,
            resultRowsetCount,
            resultColumnCount);
    }

    private static IReadOnlyList<StoredProcedureContractInspectionItem> BuildStoredProcedureContractInspectionItems(
        MTS.MetaTransformScriptModel model,
        string? transformScriptName)
    {
        var scriptsById = model.TransformScriptList.ToDictionary(static item => item.Id, StringComparer.Ordinal);
        var items = new List<StoredProcedureContractInspectionItem>();
        foreach (var storedProcedure in model.ScriptObjectStoredProcedureList)
        {
            if (!scriptsById.TryGetValue(storedProcedure.TransformScript.Id, out var script))
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(transformScriptName) &&
                !string.Equals(script.Name, transformScriptName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            items.Add(BuildStoredProcedureContractInspectionItem(model, script, storedProcedure));
        }

        if (!string.IsNullOrWhiteSpace(transformScriptName) && items.Count == 0)
        {
            throw new InvalidOperationException($"MetaTransformScript workspace does not contain a stored procedure transform named '{transformScriptName}'.");
        }

        return items
            .OrderBy(static item => item.TransformScriptName, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static StoredProcedureContractInspectionItem BuildStoredProcedureContractInspectionItem(
        MTS.MetaTransformScriptModel model,
        MTS.TransformScript script,
        MTS.ScriptObjectStoredProcedure storedProcedure)
    {
        var contracts = model.StoredProcedureContractList
            .Where(item => string.Equals(item.ScriptObjectStoredProcedure.Id, storedProcedure.Id, StringComparison.Ordinal))
            .ToArray();
        var contractIds = contracts
            .Select(static item => item.Id)
            .ToHashSet(StringComparer.Ordinal);
        var resultRowsets = model.StoredProcedureResultRowsetItemList
            .Where(item => contractIds.Contains(item.StoredProcedureContract.Id))
            .ToArray();
        var state = contracts.Length == 0
            ? StoredProcedureContractState.Missing
            : contracts.Length == 1 && resultRowsets.Length <= 1
                ? StoredProcedureContractState.Present
                : StoredProcedureContractState.Invalid;
        var resultRowsetIds = resultRowsets
            .Select(static item => item.Id)
            .ToHashSet(StringComparer.Ordinal);
        var operations = model.StoredProcedureContractOperationList
            .Where(item => contractIds.Contains(item.StoredProcedureContract.Id))
            .ToArray();

        return new StoredProcedureContractInspectionItem(
            script.Id,
            script.Name,
            storedProcedure.Id,
            contracts.Length,
            state,
            operations.Length,
            operations.Count(static item => IsOperationKind(item.OperationKind, "Read")),
            operations.Count(static item => IsWriteOperationKind(item.OperationKind)),
            operations.Count(static item => IsOperationKind(item.OperationKind, "Call")),
            resultRowsets.Length,
            model.StoredProcedureResultColumnItemList.Count(item => resultRowsetIds.Contains(item.StoredProcedureResultRowsetItem.Id)));
    }

    private static (MTS.TransformScript Script, MTS.ScriptObjectStoredProcedure StoredProcedure) ResolveStoredProcedure(
        MTS.MetaTransformScriptModel model,
        string transformScriptName)
    {
        var scriptsById = model.TransformScriptList.ToDictionary(static item => item.Id, StringComparer.Ordinal);
        var matches = model.ScriptObjectStoredProcedureList
            .Select(storedProcedure => (
                StoredProcedure: storedProcedure,
                Script: scriptsById.GetValueOrDefault(storedProcedure.TransformScript.Id)))
            .Where(item => item.Script is not null &&
                           string.Equals(item.Script.Name, transformScriptName, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (matches.Length == 0)
        {
            throw new InvalidOperationException($"MetaTransformScript workspace does not contain a stored procedure transform named '{transformScriptName}'.");
        }

        if (matches.Length > 1)
        {
            throw new InvalidOperationException($"MetaTransformScript workspace contains multiple stored procedure transforms named '{transformScriptName}'.");
        }

        return (matches[0].Script!, matches[0].StoredProcedure);
    }

    private static void AddOperations(
        MTS.MetaTransformScriptModel model,
        MTS.StoredProcedureContract contract,
        IReadOnlyList<StoredProcedureContractOperationDeclaration> declarations)
    {
        var duplicateOrdinals = declarations
            .GroupBy(static item => item.Ordinal)
            .Where(static group => group.Count() > 1)
            .Select(static group => group.Key)
            .OrderBy(static item => item)
            .ToArray();
        if (duplicateOrdinals.Length > 0)
        {
            throw new InvalidOperationException($"Stored procedure contract operation ordinals must be unique. Duplicates: {string.Join(", ", duplicateOrdinals)}.");
        }

        foreach (var declaration in declarations.OrderBy(static item => item.Ordinal))
        {
            var operationKind = NormalizeStoredProcedureOperationKind(declaration.OperationKind)
                ?? throw new InvalidOperationException($"Unsupported stored procedure operation kind '{declaration.OperationKind}'. Supported values: Read, Append, Replace, Reset, Mutation, Call.");
            if (string.IsNullOrWhiteSpace(declaration.SqlIdentifier))
            {
                throw new InvalidOperationException("Stored procedure contract operations require a SQL identifier.");
            }

            model.StoredProcedureContractOperationList.Add(new MTS.StoredProcedureContractOperation
            {
                Id = Guid.NewGuid().ToString("N"),
                StoredProcedureContract = contract,
                Ordinal = declaration.Ordinal.ToString(CultureInfo.InvariantCulture),
                OperationKind = operationKind,
                SqlIdentifier = declaration.SqlIdentifier,
                AccessRole = string.IsNullOrWhiteSpace(declaration.AccessRole) ? null : declaration.AccessRole,
                Notes = string.IsNullOrWhiteSpace(declaration.Notes) ? null : declaration.Notes
            });
        }
    }

    private static void AddResultRowsets(
        MTS.MetaTransformScriptModel model,
        MTS.StoredProcedureContract contract,
        IReadOnlyList<StoredProcedureResultRowsetDeclaration> declarations)
    {
        if (declarations.Count > 1)
        {
            throw new InvalidOperationException("Stored procedure contracts support at most one result rowset.");
        }

        for (var i = 0; i < declarations.Count; i++)
        {
            var declaration = declarations[i];
            var rowset = new MTS.StoredProcedureResultRowsetItem
            {
                Id = Guid.NewGuid().ToString("N"),
                StoredProcedureContract = contract,
                Ordinal = i.ToString(CultureInfo.InvariantCulture),
                Name = declaration.Name
            };
            model.StoredProcedureResultRowsetItemList.Add(rowset);

            for (var columnIndex = 0; columnIndex < declaration.Columns.Count; columnIndex++)
            {
                var column = declaration.Columns[columnIndex];
                model.StoredProcedureResultColumnItemList.Add(new MTS.StoredProcedureResultColumnItem
                {
                    Id = Guid.NewGuid().ToString("N"),
                    StoredProcedureResultRowsetItem = rowset,
                    Ordinal = columnIndex.ToString(CultureInfo.InvariantCulture),
                    Name = column.Name,
                    MetaDataTypeId = column.MetaDataTypeId,
                    IsNullable = column.IsNullable.HasValue ? ToContractFlag(column.IsNullable.Value) : null
                });
            }
        }
    }

    private static void ValidateStoredProcedureContractDeclaration(
        StoredProcedureContractDeclaration declaration)
    {
        if (declaration.ResultRowsets.Count > 1)
        {
            throw new InvalidOperationException("Stored procedure contracts support at most one result rowset.");
        }
    }

    private static void RemoveStoredProcedureContractRows(
        MTS.MetaTransformScriptModel model,
        IReadOnlySet<string> contractIds)
    {
        var resultRowsetIds = model.StoredProcedureResultRowsetItemList
            .Where(item => contractIds.Contains(item.StoredProcedureContract.Id))
            .Select(static item => item.Id)
            .ToHashSet(StringComparer.Ordinal);

        model.StoredProcedureContractOperationList.RemoveAll(item =>
            contractIds.Contains(item.StoredProcedureContract.Id));
        model.StoredProcedureResultColumnItemList.RemoveAll(item =>
            resultRowsetIds.Contains(item.StoredProcedureResultRowsetItem.Id));
        model.StoredProcedureResultRowsetItemList.RemoveAll(item =>
            contractIds.Contains(item.StoredProcedureContract.Id));
    }

    private static string ToContractFlag(bool value) => value ? "true" : "false";

    private static bool IsOperationKind(string? operationKind, string expected) =>
        string.Equals(NormalizeStoredProcedureOperationKind(operationKind), expected, StringComparison.Ordinal);

    private static bool IsWriteOperationKind(string? operationKind) =>
        NormalizeStoredProcedureOperationKind(operationKind) is "Append" or "Replace" or "Reset" or "Mutation";

    public static string? NormalizeStoredProcedureOperationKind(string? operationKind)
    {
        if (string.IsNullOrWhiteSpace(operationKind))
        {
            return null;
        }

        return operationKind.Trim().ToLowerInvariant() switch
        {
            "read" => "Read",
            "append" => "Append",
            "replace" => "Replace",
            "reset" => "Reset",
            "mutation" => "Mutation",
            "call" => "Call",
            _ => null
        };
    }
}

public sealed record StoredProcedureContractDeclaration(
    IReadOnlyList<StoredProcedureContractOperationDeclaration> Operations,
    IReadOnlyList<StoredProcedureResultRowsetDeclaration> ResultRowsets,
    string? Notes = null);

public sealed record StoredProcedureContractOperationDeclaration(
    int Ordinal,
    string OperationKind,
    string SqlIdentifier,
    string? AccessRole = null,
    string? Notes = null);

public sealed record StoredProcedureResultRowsetDeclaration(
    string? Name,
    IReadOnlyList<StoredProcedureResultColumnDeclaration> Columns);

public sealed record StoredProcedureResultColumnDeclaration(
    string Name,
    string? MetaDataTypeId,
    bool? IsNullable);

public sealed record StoredProcedureContractInspectionResult(
    string WorkspacePath,
    int StoredProcedureCount,
    int ContractedCount,
    int MissingContractCount,
    int InvalidContractCount,
    IReadOnlyList<StoredProcedureContractInspectionItem> Items);

public enum StoredProcedureContractState
{
    Missing,
    Present,
    Invalid
}

public sealed record StoredProcedureContractInspectionItem(
    string TransformScriptId,
    string TransformScriptName,
    string ScriptObjectStoredProcedureId,
    int ContractRowCount,
    StoredProcedureContractState ContractState,
    int OperationCount,
    int ReadOperationCount,
    int WriteOperationCount,
    int CallOperationCount,
    int ResultRowsetCount,
    int ResultColumnCount);

public sealed record StoredProcedureContractDeclarationResult(
    string WorkspacePath,
    StoredProcedureContractInspectionItem Item);

public sealed record StoredProcedureContractRemovalResult(
    string WorkspacePath,
    string TransformScriptId,
    string TransformScriptName,
    string ScriptObjectStoredProcedureId,
    int ContractCount,
    int OperationCount,
    int ResultRowsetCount,
    int ResultColumnCount);
