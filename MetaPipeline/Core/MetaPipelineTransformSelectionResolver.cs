using MetaTransformBinding;
using MetaTransformScript;

namespace MetaPipeline;

public sealed class MetaPipelineTransformSelectionResolver
{
    public MetaPipelineTransformSelection Resolve(
        string transformWorkspacePath,
        string bindingWorkspacePath,
        string script,
        string? binding = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(transformWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(bindingWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(script);

        var transformModel = Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(
            Path.GetFullPath(transformWorkspacePath),
            searchUpward: false);
        var transformScript = ResolveScript(transformModel, script.Trim());

        var bindingModel = Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Load<MetaTransformBindingModel>(
            Path.GetFullPath(bindingWorkspacePath),
            searchUpward: false);
        var transformBinding = string.IsNullOrWhiteSpace(binding)
            ? ResolveSingleBindingForScript(bindingModel, transformScript)
            : ResolveBindingForScript(bindingModel, transformScript, binding.Trim());

        return new MetaPipelineTransformSelection(
            transformScript.Id,
            transformScript.Name,
            transformBinding.Id);
    }

    private static TransformScript ResolveScript(
        MetaTransformScriptModel transformModel,
        string script)
    {
        var nameMatches = transformModel.TransformScriptList
            .Where(item => string.Equals(item.Name, script, StringComparison.Ordinal))
            .ToArray();
        if (nameMatches.Length == 1)
        {
            return nameMatches[0];
        }

        if (nameMatches.Length > 1)
        {
            throw new MetaPipelineConfigurationException(
                $"Transform script name '{script}' is ambiguous in the transform workspace. Use --script with the exact script id.");
        }

        var idMatches = transformModel.TransformScriptList
            .Where(item => string.Equals(item.Id, script, StringComparison.Ordinal))
            .ToArray();

        return idMatches.Length switch
        {
            0 => throw new MetaPipelineConfigurationException(
                $"Transform script '{script}' was not found by exact name or id in the transform workspace."),
            > 1 => throw new MetaPipelineConfigurationException(
                $"Transform script id '{script}' is ambiguous in the transform workspace."),
            _ => idMatches[0],
        };
    }

    private static TransformBinding ResolveSingleBindingForScript(
        MetaTransformBindingModel bindingModel,
        TransformScript transformScript)
    {
        var matches = bindingModel.TransformBindingList
            .Where(item => string.Equals(item.MetaTransformScriptTransformScriptId, transformScript.Id, StringComparison.Ordinal))
            .ToArray();

        return matches.Length switch
        {
            0 => throw new MetaPipelineConfigurationException(
                $"No transform binding references script '{transformScript.Name}' ({transformScript.Id}). Use meta-transform-binding to create one."),
            > 1 => throw new MetaPipelineConfigurationException(
                $"Transform script '{transformScript.Name}' ({transformScript.Id}) has multiple bindings. Use --binding <id> to choose one."),
            _ => matches[0],
        };
    }

    private static TransformBinding ResolveBindingForScript(
        MetaTransformBindingModel bindingModel,
        TransformScript transformScript,
        string binding)
    {
        var matches = bindingModel.TransformBindingList
            .Where(item => string.Equals(item.Id, binding, StringComparison.Ordinal))
            .ToArray();

        var transformBinding = matches.Length switch
        {
            0 => throw new MetaPipelineConfigurationException(
                $"Transform binding '{binding}' was not found by exact id in the binding workspace."),
            > 1 => throw new MetaPipelineConfigurationException(
                $"Transform binding id '{binding}' is ambiguous in the binding workspace."),
            _ => matches[0],
        };

        if (!string.Equals(transformBinding.MetaTransformScriptTransformScriptId, transformScript.Id, StringComparison.Ordinal))
        {
            throw new MetaPipelineConfigurationException(
                $"Transform binding '{transformBinding.Id}' references script id '{transformBinding.MetaTransformScriptTransformScriptId}', not selected script '{transformScript.Name}' ({transformScript.Id}).");
        }

        return transformBinding;
    }
}
