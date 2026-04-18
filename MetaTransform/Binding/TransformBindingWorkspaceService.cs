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
        var transformScripts = ResolveScripts(transformModel);
        var packages = BindTransformScripts(transformModel, transformScripts);
        EnsureBindingSucceeded(packages);
        var bindingModel = BuildCombinedBindingModel(packages);
        var validatedModel = new TransformBindingValidationService().ApplyValidation(
            bindingModel,
            schemaModel,
            validationOptions ?? TransformBindingValidationOptions.Default);

        validatedModel.SaveToXmlWorkspace(bindingWorkspaceFullPath);

        return new BindToWorkspaceResult(
            validatedModel,
            bindingWorkspaceFullPath,
            packages.Count,
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
        string newWorkspacePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(transformWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(newWorkspacePath);

        var transformWorkspaceFullPath = Path.GetFullPath(transformWorkspacePath);
        var bindingWorkspaceFullPath = Path.GetFullPath(newWorkspacePath);

        var transformModel = MetaTransformScriptModel.LoadFromXmlWorkspace(transformWorkspaceFullPath, searchUpward: false);
        var transformScripts = ResolveScripts(transformModel);
        var packages = BindTransformScripts(transformModel, transformScripts);
        var bindingModel = BuildCombinedBindingModel(packages);

        bindingModel.SaveToXmlWorkspace(bindingWorkspaceFullPath);

        var issueCount = packages.Sum(item => item.Bound.Issues.Count);
        var errorCount = issueCount;

        return new BindToWorkspaceResult(
            bindingModel,
            bindingWorkspaceFullPath,
            packages.Count,
            bindingModel.TransformBindingList.Count,
            bindingModel.RowsetList.Count(item =>
                string.Equals(item.DerivationKind, "Source", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(item.SqlIdentifier)),
            bindingModel.TransformBindingTargetList.Count,
            issueCount,
            errorCount);
    }

    private static TransformScript[] ResolveScripts(MetaTransformScriptModel model)
    {
        var scripts = model.TransformScriptList.ToArray();
        if (scripts.Length == 0)
        {
            throw new InvalidOperationException("MetaTransformScript workspace does not contain any TransformScript rows.");
        }

        return scripts;
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

    private static List<ScriptBindingPackage> BindTransformScripts(
        MetaTransformScriptModel transformModel,
        IReadOnlyList<TransformScript> transformScripts)
    {
        var bindingService = new TransformBindingService();
        var packages = new List<ScriptBindingPackage>(transformScripts.Count);

        foreach (var transformScript in transformScripts)
        {
            var target = CreateTargetFromTransformScript(transformScript);
            var bound = bindingService.BindTransform(transformModel, transformScript);
            packages.Add(new ScriptBindingPackage(transformScript, bound, target));
        }

        return packages;
    }

    private static void EnsureBindingSucceeded(IReadOnlyList<ScriptBindingPackage> packages)
    {
        foreach (var package in packages)
        {
            if (!package.Bound.HasErrors)
            {
                continue;
            }

            var firstError = package.Bound.Issues.FirstOrDefault();
            var errorMessage = firstError is null
                ? $"Transform script '{package.TransformScript.Name}' produced one or more binding errors."
                : $"Transform script '{package.TransformScript.Name}' failed binding with {firstError.Code}: {firstError.Message}";

            throw new TransformBindingValidationException("BindingFailed", errorMessage);
        }
    }

    private static MetaTransformBindingModel BuildCombinedBindingModel(IReadOnlyList<ScriptBindingPackage> packages)
    {
        var model = MetaTransformBindingModel.CreateEmpty();

        foreach (var package in packages)
        {
            var partial = TransformBindingModelBuilder.Create(package.Bound, [package.Target]);
            MergeById(model.TransformBindingList, partial.TransformBindingList, static item => item.Id, "TransformBinding", package.TransformScript.Name);
            MergeById(model.TransformBindingTargetList, partial.TransformBindingTargetList, static item => item.Id, "TransformBindingTarget", package.TransformScript.Name);
            MergeById(model.RowsetList, partial.RowsetList, static item => item.Id, "Rowset", package.TransformScript.Name);
            MergeById(model.SourceTargetList, partial.SourceTargetList, static item => item.Id, "SourceTarget", package.TransformScript.Name);
            MergeById(model.ColumnList, partial.ColumnList, static item => item.Id, "Column", package.TransformScript.Name);
            MergeById(model.ColumnReferenceList, partial.ColumnReferenceList, static item => item.Id, "ColumnReference", package.TransformScript.Name);
            MergeById(model.TableSourceList, partial.TableSourceList, static item => item.Id, "TableSource", package.TransformScript.Name);
            MergeById(model.OutputRowsetList, partial.OutputRowsetList, static item => item.Id, "OutputRowset", package.TransformScript.Name);
        }

        return model;
    }

    private static void MergeById<T>(
        List<T> destination,
        IReadOnlyList<T> source,
        Func<T, string> idSelector,
        string entityName,
        string transformScriptName)
    {
        var seen = destination
            .Select(idSelector)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var item in source)
        {
            var id = idSelector(item);
            if (!seen.Add(id))
            {
                throw new InvalidOperationException(
                    $"Binding merge produced duplicate {entityName} Id '{id}' while processing transform script '{transformScriptName}'.");
            }

            destination.Add(item);
        }
    }

    private sealed record ScriptBindingPackage(
        TransformScript TransformScript,
        TransformBindingResult Bound,
        TransformBindingTargetResolution Target);

}

public sealed record BindToWorkspaceResult(
    MetaTransformBindingModel Model,
    string WorkspacePath,
    int TransformScriptCount,
    int TransformBindingCount,
    int SourceCount,
    int TargetCount,
    int IssueCount,
    int ErrorCount,
    int SourceRowsetValidationCount = 0,
    int TargetRowsetValidationCount = 0,
    int SourceColumnValidationCount = 0,
    int TargetColumnValidationCount = 0);
