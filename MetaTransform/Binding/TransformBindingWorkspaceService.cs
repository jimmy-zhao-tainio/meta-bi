using MetaSchema;
using MetaTransformBinding;
using MetaTransformScript;

namespace MetaTransform.Binding;

public sealed class TransformBindingWorkspaceService
{
    public BindToWorkspaceResult BindToWorkspace(
        string transformWorkspacePath,
        string schemaWorkspacePath,
        string newWorkspacePath,
        string? transformScriptName = null,
        string? activeLanguageProfileIdOverride = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(transformWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(schemaWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(newWorkspacePath);

        var transformWorkspaceFullPath = Path.GetFullPath(transformWorkspacePath);
        var schemaWorkspaceFullPath = Path.GetFullPath(schemaWorkspacePath);
        var bindingWorkspaceFullPath = Path.GetFullPath(newWorkspacePath);

        var transformModel = MetaTransformScriptModel.LoadFromXmlWorkspace(transformWorkspaceFullPath, searchUpward: false);
        var schemaModel = MetaSchemaModel.LoadFromXmlWorkspace(schemaWorkspaceFullPath, searchUpward: false);
        var transformScript = ResolveSingleScript(transformModel, transformScriptName);

        var bindingModel = new TransformBindingService().BindTransformModel(
            transformModel,
            transformScript,
            schemaModel,
            activeLanguageProfileIdOverride);

        bindingModel.SaveToXmlWorkspace(bindingWorkspaceFullPath);

        var issueCount = bindingModel.BoundIssueList.Count;
        var errorCount = bindingModel.BoundIssueList.Count(item => string.Equals(item.Severity, "Error", StringComparison.OrdinalIgnoreCase));

        return new BindToWorkspaceResult(
            bindingModel,
            bindingWorkspaceFullPath,
            transformScript.Name,
            bindingModel.TransformBindingList.Count,
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
}

public sealed record BindToWorkspaceResult(
    MetaTransformBindingModel Model,
    string WorkspacePath,
    string TransformScriptName,
    int TransformBindingCount,
    int IssueCount,
    int ErrorCount);
