using MetaTransformBinding;
using MTS = MetaTransformScript;
using MetaTransformScript;
using MetaTransformScript.Sql;

namespace MetaPipeline;

public sealed class MetaPipelineExecutionWorkspaceResolver
{
    private readonly MetaTransformScriptSqlService sqlService = new();

    public MetaPipelineExecutionDefinition Resolve(
        string transformWorkspacePath,
        string bindingWorkspacePath,
        string transformScriptName,
        string? targetSqlIdentifier = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(transformWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(bindingWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(transformScriptName);

        var bindingModel = MetaTransformBindingModel.LoadFromXmlWorkspace(
            Path.GetFullPath(bindingWorkspacePath),
            searchUpward: false);
        var transformModel = MetaTransformScriptModel.LoadFromXmlWorkspace(
            Path.GetFullPath(transformWorkspacePath),
            searchUpward: false);

        var binding = ResolveBinding(bindingModel, transformScriptName);
        var transformScript = ResolveTransformScript(transformModel, binding);
        EnsureTransformScriptIsSupported(transformModel, transformScript);

        var target = ResolveTarget(bindingModel, binding, targetSqlIdentifier);
        var outputRowset = ResolveSingleOutputRowset(bindingModel, binding);
        var columns = ResolveOrderedColumns(bindingModel, outputRowset);
        var sourceSql = sqlService.ExportToSqlCode(transformModel, transformScript.Name);

        return new MetaPipelineExecutionDefinition(
            transformScript.Id,
            transformScript.Name,
            sourceSql,
            target.SqlIdentifier,
            new PipelineRowStreamShape(columns));
    }

    private static TransformBinding ResolveBinding(
        MetaTransformBindingModel bindingModel,
        string transformScriptName)
    {
        var bindings = bindingModel.TransformBindingList.ToArray();
        if (bindings.Length == 0)
        {
            throw new MetaPipelineConfigurationException(
                "Binding workspace does not contain any TransformBinding rows.");
        }

        var matches = bindings
            .Where(item => string.Equals(item.TransformScriptName, transformScriptName, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return matches.Length switch
        {
            0 => throw new MetaPipelineConfigurationException(
                $"Transform script '{transformScriptName}' was not found in the binding workspace."),
            > 1 => throw new MetaPipelineConfigurationException(
                $"Transform script name '{transformScriptName}' is ambiguous in the binding workspace."),
            _ => matches[0],
        };
    }

    private static MTS.TransformScript ResolveTransformScript(
        MTS.MetaTransformScriptModel transformModel,
        TransformBinding binding)
    {
        var transformScripts = transformModel.TransformScriptList.ToArray();
        var matchesById = transformScripts
            .Where(item => string.Equals(item.Id, binding.MetaTransformScriptTransformScriptId, StringComparison.Ordinal))
            .ToArray();

        if (matchesById.Length == 1)
        {
            return matchesById[0];
        }

        if (matchesById.Length > 1)
        {
            throw new MetaPipelineConfigurationException(
                $"Transform script id '{binding.MetaTransformScriptTransformScriptId}' is ambiguous in the transform workspace.");
        }

        throw new MetaPipelineConfigurationException(
            $"Transform script '{binding.TransformScriptName}' referenced by the binding workspace was not found in the transform workspace.");
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
                $"Transform binding '{binding.TransformScriptName}' does not contain a target.");
        }

        foreach (var target in targets)
        {
            if (string.IsNullOrWhiteSpace(target.SqlIdentifier))
            {
                throw new MetaPipelineConfigurationException(
                    $"Transform binding '{binding.TransformScriptName}' contains a blank target SQL identifier.");
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
                    $"Target '{targetSqlIdentifier}' was not found for transform binding '{binding.TransformScriptName}'."),
                > 1 => throw new MetaPipelineConfigurationException(
                    $"Target '{targetSqlIdentifier}' is ambiguous for transform binding '{binding.TransformScriptName}'."),
                _ => matches[0],
            };
        }

        return targets.Length switch
        {
            1 => targets[0],
            _ => throw new MetaPipelineConfigurationException(
                $"Transform binding '{binding.TransformScriptName}' contains multiple targets. Use --target <sql-identifier> to select one."),
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
                $"Transform binding '{binding.TransformScriptName}' does not contain an output rowset."),
            > 1 => throw new MetaPipelineConfigurationException(
                $"Transform binding '{binding.TransformScriptName}' contains multiple output rowsets. Stage 1 execute supports exactly one output rowset."),
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
