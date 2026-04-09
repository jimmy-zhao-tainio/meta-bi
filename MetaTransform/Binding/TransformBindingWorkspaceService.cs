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
        IReadOnlyList<string> targetSqlIdentifiers,
        string? transformScriptName = null,
        string? activeLanguageProfileIdOverride = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(transformWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(schemaWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(newWorkspacePath);
        ArgumentNullException.ThrowIfNull(targetSqlIdentifiers);

        if (targetSqlIdentifiers.Count == 0)
        {
            throw new InvalidOperationException("At least one --target SQL identifier is required.");
        }

        var transformWorkspaceFullPath = Path.GetFullPath(transformWorkspacePath);
        var schemaWorkspaceFullPath = Path.GetFullPath(schemaWorkspacePath);
        var bindingWorkspaceFullPath = Path.GetFullPath(newWorkspacePath);

        var transformModel = MetaTransformScriptModel.LoadFromXmlWorkspace(transformWorkspaceFullPath, searchUpward: false);
        var schemaModel = MetaSchemaModel.LoadFromXmlWorkspace(schemaWorkspaceFullPath, searchUpward: false);
        var transformScript = ResolveSingleScript(transformModel, transformScriptName);
        var resolvedTargets = ResolveTargets(schemaModel, targetSqlIdentifiers);

        var bound = new TransformBindingService().BindTransform(
            transformModel,
            transformScript,
            schemaModel,
            activeLanguageProfileIdOverride);
        var bindingModel = TransformBindingModelBuilder.Create(bound, resolvedTargets);

        bindingModel.SaveToXmlWorkspace(bindingWorkspaceFullPath);

        var issueCount = bindingModel.BoundIssueList.Count;
        var errorCount = bindingModel.BoundIssueList.Count(item => string.Equals(item.Severity, "Error", StringComparison.OrdinalIgnoreCase));

        return new BindToWorkspaceResult(
            bindingModel,
            bindingWorkspaceFullPath,
            transformScript.Name,
            bindingModel.TransformBindingList.Count,
            bindingModel.TransformBindingSourceList.Count,
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

    private static IReadOnlyList<TransformBindingTargetResolution> ResolveTargets(
        MetaSchemaModel schemaModel,
        IReadOnlyList<string> targetSqlIdentifiers)
    {
        var resolver = new MetaSchemaTableResolver(schemaModel);
        var targets = new List<TransformBindingTargetResolution>();

        foreach (var targetSqlIdentifier in targetSqlIdentifiers)
        {
            var resolution = resolver.ResolveSqlIdentifier(targetSqlIdentifier);
            if (!resolution.IsResolved)
            {
                throw new InvalidOperationException(CreateTargetResolutionErrorMessage(targetSqlIdentifier, resolution));
            }

            targets.Add(new TransformBindingTargetResolution(
                targetSqlIdentifier.Trim(),
                resolution.Table!.TableId));
        }

        return targets;
    }

    private static string CreateTargetResolutionErrorMessage(
        string targetSqlIdentifier,
        SchemaTableResolutionResult resolution)
    {
        return resolution.FailureKind switch
        {
            SchemaTableResolutionFailureKind.MissingIdentifier =>
                $"Target '{targetSqlIdentifier}' does not expose a supported SQL identifier.",
            SchemaTableResolutionFailureKind.UnsupportedIdentifierShape =>
                $"Target '{targetSqlIdentifier}' uses {resolution.IdentifierParts.Count} identifier parts; binding supports table, schema.table, or database.schema.table targets only.",
            SchemaTableResolutionFailureKind.NotFound =>
                $"Target '{targetSqlIdentifier}' was not found in the sanctioned schema workspace.",
            SchemaTableResolutionFailureKind.Ambiguous =>
                $"Target '{targetSqlIdentifier}' resolves ambiguously in the sanctioned schema workspace.",
            _ =>
                $"Target '{targetSqlIdentifier}' could not be resolved in the sanctioned schema workspace."
        };
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
