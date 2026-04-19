using MetaSchema;
using MetaTransformBinding;

namespace MetaTransform.Binding;

public sealed class TransformBindingValidationWorkspaceService
{
    public ValidateWorkspaceResult ValidateWorkspace(
        string bindingWorkspacePath,
        string sourceSchemaWorkspacePath,
        string targetSchemaWorkspacePath,
        string newWorkspacePath,
        TransformBindingValidationOptions? options = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bindingWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceSchemaWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetSchemaWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(newWorkspacePath);

        var bindingWorkspaceFullPath = Path.GetFullPath(bindingWorkspacePath);
        var sourceSchemaWorkspaceFullPath = Path.GetFullPath(sourceSchemaWorkspacePath);
        var targetSchemaWorkspaceFullPath = Path.GetFullPath(targetSchemaWorkspacePath);
        var validatedWorkspaceFullPath = Path.GetFullPath(newWorkspacePath);

        var bindingModel = MetaTransformBindingModel.LoadFromXmlWorkspace(bindingWorkspaceFullPath, searchUpward: false);
        var sourceSchemaModel = MetaSchemaModel.LoadFromXmlWorkspace(sourceSchemaWorkspaceFullPath, searchUpward: false);
        var targetSchemaModel = MetaSchemaModel.LoadFromXmlWorkspace(targetSchemaWorkspaceFullPath, searchUpward: false);

        var validated = new TransformBindingValidationService().ApplyValidation(
            bindingModel,
            sourceSchemaModel,
            targetSchemaModel,
            options ?? TransformBindingValidationOptions.Default);
        validated.SaveToXmlWorkspace(validatedWorkspaceFullPath);

        return new ValidateWorkspaceResult(
            validated,
            validatedWorkspaceFullPath,
            validated.TransformBindingList.Count,
            validated.ValidationSourceRowsetLinkList.Count,
            validated.ValidationTargetRowsetLinkList.Count,
            validated.ValidationSourceColumnLinkList.Count,
            validated.ValidationTargetColumnLinkList.Count);
    }
}

public sealed record ValidateWorkspaceResult(
    MetaTransformBindingModel Model,
    string WorkspacePath,
    int TransformBindingCount,
    int SourceRowsetValidationCount,
    int TargetRowsetValidationCount,
    int SourceColumnValidationCount,
    int TargetColumnValidationCount);
