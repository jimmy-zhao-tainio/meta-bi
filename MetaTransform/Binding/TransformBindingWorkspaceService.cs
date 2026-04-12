using MetaTransformBinding;
using MetaTransformScript;

namespace MetaTransform.Binding;

public sealed class TransformBindingWorkspaceService
{
    public BindToWorkspaceResult BindToWorkspace(
        string transformWorkspacePath,
        string newWorkspacePath,
        IReadOnlyList<string> targetSqlIdentifiers,
        string? transformScriptName = null,
        string? activeLanguageProfileIdOverride = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(transformWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(newWorkspacePath);
        ArgumentNullException.ThrowIfNull(targetSqlIdentifiers);

        if (targetSqlIdentifiers.Count == 0)
        {
            throw new InvalidOperationException("At least one --target SQL identifier is required.");
        }

        var transformWorkspaceFullPath = Path.GetFullPath(transformWorkspacePath);
        var bindingWorkspaceFullPath = Path.GetFullPath(newWorkspacePath);

        var transformModel = MetaTransformScriptModel.LoadFromXmlWorkspace(transformWorkspaceFullPath, searchUpward: false);
        var transformScript = ResolveSingleScript(transformModel, transformScriptName);
        var targets = targetSqlIdentifiers
            .Select(CreateUnresolvedTarget)
            .ToArray();

        var bound = new TransformBindingService().BindTransform(
            transformModel,
            transformScript,
            activeLanguageProfileIdOverride);
        var bindingModel = TransformBindingModelBuilder.Create(bound, targets);

        bindingModel.SaveToXmlWorkspace(bindingWorkspaceFullPath);

        var issueCount = bound.Issues.Count;
        var errorCount = issueCount;

        return new BindToWorkspaceResult(
            bindingModel,
            bindingWorkspaceFullPath,
            transformScript.Name,
            bindingModel.TransformBindingList.Count,
            bindingModel.RowsetList.Count(item =>
                string.Equals(item.DerivationKind, "Source", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(item.SqlIdentifier)),
            bindingModel.TransformBindingTargetList.Count,
            issueCount,
            errorCount);
    }

    private static TransformScript ResolveSingleScript(MetaTransformScriptModel model, string? transformScriptName)
    {
        var scripts = model.TransformScriptList.ToArray();
        if (scripts.Length == 0)
        {
            throw new InvalidOperationException("MetaTransformScript workspace does not contain any TransformScript rows.");
        }

        if (!string.IsNullOrWhiteSpace(transformScriptName))
        {
            var matches = scripts
                .Where(script => string.Equals(script.Name, transformScriptName, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            return matches.Length switch
            {
                0 => throw new InvalidOperationException($"Transform script '{transformScriptName}' was not found."),
                > 1 => throw new InvalidOperationException($"Transform script name '{transformScriptName}' is ambiguous."),
                _ => matches[0]
            };
        }

        if (scripts.Length != 1)
        {
            throw new InvalidOperationException(
                $"Workspace contains {scripts.Length} transform scripts. Use --name to select which one to bind.");
        }

        return scripts[0];
    }

    private static TransformBindingTargetResolution CreateUnresolvedTarget(string targetSqlIdentifier)
    {
        var trimmed = targetSqlIdentifier?.Trim() ?? string.Empty;
        var parts = trimmed
            .Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length is < 1 or > 3)
        {
            throw new InvalidOperationException(
                $"Target '{targetSqlIdentifier}' uses {parts.Length} identifier parts; binding supports table, schema.table, or database.schema.table targets only.");
        }

        return new TransformBindingTargetResolution(trimmed, null);
    }

}

public sealed record BindToWorkspaceResult(
    MetaTransformBindingModel Model,
    string WorkspacePath,
    string TransformScriptName,
    int TransformBindingCount,
    int SourceCount,
    int TargetCount,
    int IssueCount,
    int ErrorCount);
