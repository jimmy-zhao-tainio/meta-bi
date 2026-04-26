using MetaTransformBinding;
using MTS = MetaTransformScript;
using MetaTransformScript;
using MetaTransformScript.Sql;

namespace MetaPipeline;

public sealed class MetaPipelineExecutionWorkspaceResolver
{
    private readonly MetaTransformScriptSqlService sqlService = new();

    public MetaPipelineExecutionDefinition ResolveByIds(
        string transformWorkspacePath,
        string bindingWorkspacePath,
        string transformScriptId,
        string transformBindingId,
        string? targetSqlIdentifier = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(transformWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(bindingWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(transformScriptId);
        ArgumentException.ThrowIfNullOrWhiteSpace(transformBindingId);

        var bindingModel = MetaTransformBindingModel.LoadFromXmlWorkspace(
            Path.GetFullPath(bindingWorkspacePath),
            searchUpward: false);
        var transformModel = MetaTransformScriptModel.LoadFromXmlWorkspace(
            Path.GetFullPath(transformWorkspacePath),
            searchUpward: false);

        var transformScript = ResolveTransformScriptById(transformModel, transformScriptId);
        var binding = ResolveBindingById(bindingModel, transformBindingId);
        if (!string.Equals(binding.MetaTransformScriptTransformScriptId, transformScript.Id, StringComparison.Ordinal))
        {
            throw new MetaPipelineConfigurationException(
                $"Transform binding id '{binding.Id}' does not reference transform script id '{transformScript.Id}'.");
        }

        EnsureTransformScriptIsSupported(transformModel, transformScript);

        return CreateDefinition(
            transformModel,
            bindingModel,
            transformScript,
            binding,
            targetSqlIdentifier);
    }

    private MetaPipelineExecutionDefinition CreateDefinition(
        MTS.MetaTransformScriptModel transformModel,
        MetaTransformBindingModel bindingModel,
        MTS.TransformScript transformScript,
        TransformBinding binding,
        string? targetSqlIdentifier)
    {
        var target = ResolveTarget(bindingModel, binding, targetSqlIdentifier);
        var outputRowset = ResolveSingleOutputRowset(bindingModel, binding);
        var columns = ResolveOrderedColumns(bindingModel, outputRowset);
        var sourceSql = sqlService.ExportToSqlCode(transformModel, transformScript.Name);

        return new MetaPipelineExecutionDefinition(
            transformScript.Id,
            transformScript.Name,
            binding.Id,
            sourceSql,
            target.SqlIdentifier,
            new PipelineRowStreamShape(columns));
    }

    private static TransformBinding ResolveBindingById(
        MetaTransformBindingModel bindingModel,
        string transformBindingId)
    {
        var matches = bindingModel.TransformBindingList
            .Where(item => string.Equals(item.Id, transformBindingId, StringComparison.Ordinal))
            .ToArray();

        return matches.Length switch
        {
            0 => throw new MetaPipelineConfigurationException(
                $"Transform binding id '{transformBindingId}' was not found in the binding workspace."),
            > 1 => throw new MetaPipelineConfigurationException(
                $"Transform binding id '{transformBindingId}' is ambiguous in the binding workspace."),
            _ => matches[0],
        };
    }

    private static MTS.TransformScript ResolveTransformScriptById(
        MTS.MetaTransformScriptModel transformModel,
        string transformScriptId)
    {
        var transformScripts = transformModel.TransformScriptList.ToArray();
        var matchesById = transformScripts
            .Where(item => string.Equals(item.Id, transformScriptId, StringComparison.Ordinal))
            .ToArray();

        if (matchesById.Length == 1)
        {
            return matchesById[0];
        }

        if (matchesById.Length > 1)
        {
            throw new MetaPipelineConfigurationException(
                $"Transform script id '{transformScriptId}' is ambiguous in the transform workspace.");
        }

        throw new MetaPipelineConfigurationException(
            $"Transform script id '{transformScriptId}' was not found in the transform workspace.");
    }

    private static void EnsureTransformScriptIsSupported(
        MTS.MetaTransformScriptModel transformModel,
        MTS.TransformScript transformScript)
    {
        var duplicateNameCount = transformModel.TransformScriptList.Count(item =>
            string.Equals(item.Name, transformScript.Name, StringComparison.OrdinalIgnoreCase));
        if (duplicateNameCount > 1)
        {
            throw new MetaPipelineConfigurationException(
                $"Transform script name '{transformScript.Name}' is ambiguous in the transform workspace.");
        }

        var parameterCount = transformModel.TransformScriptFunctionParametersItemList.Count(item =>
            string.Equals(item.TransformScriptId, transformScript.Id, StringComparison.Ordinal));
        if (parameterCount > 0)
        {
            throw new MetaPipelineConfigurationException(
                $"Transform script '{transformScript.Name}' has function parameters. Stage 1 execute supports parameterless transform scripts only.");
        }
    }

    private static TransformBindingTarget ResolveTarget(
        MetaTransformBindingModel bindingModel,
        TransformBinding binding,
        string? targetSqlIdentifier)
    {
        var targets = bindingModel.TransformBindingTargetList
            .Where(item => string.Equals(item.TransformBindingId, binding.Id, StringComparison.Ordinal))
            .ToArray();

        if (targets.Length == 0)
        {
            throw new MetaPipelineConfigurationException(
                $"Transform binding id '{binding.Id}' does not contain a target.");
        }

        foreach (var target in targets)
        {
            if (string.IsNullOrWhiteSpace(target.SqlIdentifier))
            {
                throw new MetaPipelineConfigurationException(
                    $"Transform binding id '{binding.Id}' contains a blank target SQL identifier.");
            }
        }

        if (!string.IsNullOrWhiteSpace(targetSqlIdentifier))
        {
            var matches = targets
                .Where(item => string.Equals(item.SqlIdentifier, targetSqlIdentifier.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToArray();

            return matches.Length switch
            {
                0 => throw new MetaPipelineConfigurationException(
                    $"Target '{targetSqlIdentifier}' was not found for transform binding id '{binding.Id}'."),
                > 1 => throw new MetaPipelineConfigurationException(
                    $"Target '{targetSqlIdentifier}' is ambiguous for transform binding id '{binding.Id}'."),
                _ => matches[0],
            };
        }

        return targets.Length switch
        {
            1 => targets[0],
            _ => throw new MetaPipelineConfigurationException(
                $"Transform binding id '{binding.Id}' contains multiple targets. Use --target <sql-identifier> to select one."),
        };
    }

    private static OutputRowset ResolveSingleOutputRowset(
        MetaTransformBindingModel bindingModel,
        TransformBinding binding)
    {
        var outputRowsets = bindingModel.OutputRowsetList
            .Where(item => string.Equals(item.TransformBindingId, binding.Id, StringComparison.Ordinal))
            .ToArray();

        return outputRowsets.Length switch
        {
            0 => throw new MetaPipelineConfigurationException(
                $"Transform binding id '{binding.Id}' does not contain an output rowset."),
            > 1 => throw new MetaPipelineConfigurationException(
                $"Transform binding id '{binding.Id}' contains multiple output rowsets. Stage 1 execute supports exactly one output rowset."),
            _ => outputRowsets[0],
        };
    }

    private static IReadOnlyList<PipelineColumn> ResolveOrderedColumns(
        MetaTransformBindingModel bindingModel,
        OutputRowset outputRowset)
    {
        var rowset = bindingModel.RowsetList.SingleOrDefault(item =>
            string.Equals(item.Id, outputRowset.RowsetId, StringComparison.Ordinal));
        if (rowset is null)
        {
            throw new MetaPipelineConfigurationException(
                $"Binding output rowset '{outputRowset.RowsetId}' points to a missing Rowset.");
        }

        var columns = bindingModel.ColumnList
            .Where(item => string.Equals(item.RowsetId, rowset.Id, StringComparison.Ordinal))
            .Select(item => new
            {
                Column = item,
                Ordinal = ParseOrdinal(item),
            })
            .OrderBy(item => item.Ordinal)
            .ToArray();

        if (columns.Length == 0)
        {
            throw new MetaPipelineConfigurationException(
                $"Binding output rowset '{rowset.Name}' does not contain any columns.");
        }

        for (var index = 0; index < columns.Length; index++)
        {
            if (index > 0 && columns[index - 1].Ordinal == columns[index].Ordinal)
            {
                throw new MetaPipelineConfigurationException(
                    $"Binding output rowset '{rowset.Name}' contains duplicate column ordinal '{columns[index].Ordinal}'.");
            }
        }

        return columns
            .Select(item => new PipelineColumn(item.Column.Name, item.Ordinal))
            .ToArray();
    }

    private static int ParseOrdinal(Column column)
    {
        if (!int.TryParse(column.Ordinal, out var ordinal) || ordinal < 0)
        {
            throw new MetaPipelineConfigurationException(
                $"Binding column '{column.Name}' contains invalid ordinal '{column.Ordinal}'.");
        }

        return ordinal;
    }
}
