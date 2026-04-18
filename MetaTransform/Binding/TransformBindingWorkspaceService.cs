using MetaSchema;
using MetaTransformBinding;
using MetaTransformScript;

namespace MetaTransform.Binding;

public sealed class TransformBindingWorkspaceService
{
    public BindToWorkspaceResult BindValidatedToWorkspace(
        string transformWorkspacePath,
        string schemaWorkspacePath,
        string newWorkspacePath,
        string? transformScriptName = null,
        TransformBindingValidationOptions? validationOptions = null)
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
        var target = CreateTargetFromTransformScript(transformScript);

        var bound = new TransformBindingService().BindTransform(
            transformModel,
            transformScript);

        if (bound.HasErrors)
        {
            var firstError = bound.Issues.FirstOrDefault();
            var errorMessage = firstError is null
                ? "Binding produced one or more errors."
                : $"{firstError.Code}: {firstError.Message}";

            throw new TransformBindingValidationException("BindingFailed", errorMessage);
        }

        var bindingModel = TransformBindingModelBuilder.Create(bound, [target]);
        var validatedModel = new TransformBindingValidationService().ApplyValidation(
            bindingModel,
            schemaModel,
            validationOptions ?? TransformBindingValidationOptions.Default);

        validatedModel.SaveToXmlWorkspace(bindingWorkspaceFullPath);

        return new BindToWorkspaceResult(
            validatedModel,
            bindingWorkspaceFullPath,
            transformScript.Name,
            validatedModel.TransformBindingList.Count,
            validatedModel.RowsetList.Count(item =>
                string.Equals(item.DerivationKind, "Source", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(item.SqlIdentifier)),
            validatedModel.TransformBindingTargetList.Count,
            0,
            0,
            validatedModel.ValidationSourceRowsetLinkList.Count,
            validatedModel.ValidationTargetRowsetLinkList.Count,
            validatedModel.ValidationSourceColumnLinkList.Count,
            validatedModel.ValidationTargetColumnLinkList.Count);
    }

    public BindToWorkspaceResult BindToWorkspace(
        string transformWorkspacePath,
        string newWorkspacePath,
        string? transformScriptName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(transformWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(newWorkspacePath);

        var transformWorkspaceFullPath = Path.GetFullPath(transformWorkspacePath);
        var bindingWorkspaceFullPath = Path.GetFullPath(newWorkspacePath);

        var transformModel = MetaTransformScriptModel.LoadFromXmlWorkspace(transformWorkspaceFullPath, searchUpward: false);
        var transformScript = ResolveSingleScript(transformModel, transformScriptName);
        var target = CreateTargetFromTransformScript(transformScript);

        var bound = new TransformBindingService().BindTransform(
            transformModel,
            transformScript);
        var bindingModel = TransformBindingModelBuilder.Create(bound, [target]);

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

    private static TransformBindingTargetResolution CreateTargetFromTransformScript(TransformScript transformScript)
    {
        var trimmed = transformScript.TargetSqlIdentifier?.Trim() ?? string.Empty;
        var parts = trimmed
            .Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length is < 1 or > 3)
        {
            throw new InvalidOperationException(
                $"Transform script '{transformScript.Name}' target '{transformScript.TargetSqlIdentifier}' uses {parts.Length} identifier parts; binding supports table, schema.table, or database.schema.table targets only.");
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
    int ErrorCount,
    int SourceRowsetValidationCount = 0,
    int TargetRowsetValidationCount = 0,
    int SourceColumnValidationCount = 0,
    int TargetColumnValidationCount = 0);
