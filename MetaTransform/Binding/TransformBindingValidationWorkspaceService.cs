using MetaSchema;
using MetaTransformBinding;

namespace MetaTransform.Binding;

public sealed class TransformBindingValidationWorkspaceService
{
    public ValidateWorkspaceResult ValidateWorkspace(
        string bindingWorkspacePath,
        string schemaWorkspacePath,
        string newWorkspacePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bindingWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(schemaWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(newWorkspacePath);

        var bindingWorkspaceFullPath = Path.GetFullPath(bindingWorkspacePath);
        var schemaWorkspaceFullPath = Path.GetFullPath(schemaWorkspacePath);
        var validatedWorkspaceFullPath = Path.GetFullPath(newWorkspacePath);

        var bindingModel = MetaTransformBindingModel.LoadFromXmlWorkspace(bindingWorkspaceFullPath, searchUpward: false);
        var schemaModel = MetaSchemaModel.LoadFromXmlWorkspace(schemaWorkspaceFullPath, searchUpward: false);

        var validated = new TransformBindingValidationService().ApplyValidation(bindingModel, schemaModel);
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
