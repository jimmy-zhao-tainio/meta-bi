using System.Globalization;
using MetaTransformBinding;

namespace MetaTransform.Binding;

internal static class TransformBindingModelBuilder
{
    public static MetaTransformBindingModel Create(
        TransformBindingResult bound,
        IReadOnlyList<TransformBindingTargetResolution>? targets = null)
    {
        ArgumentNullException.ThrowIfNull(bound);

        var model = MetaTransformBindingModel.CreateEmpty();
        var bindingId = $"{bound.TransformScriptId}:binding";
        var bindingRow = new TransformBinding
        {
            Id = bindingId,
            MetaTransformScriptTransformScriptId = bound.TransformScriptId,
            TransformScriptName = bound.TransformScriptName
        };

        model.TransformBindingList.Add(bindingRow);

        foreach (var target in (targets ?? [])
                     .GroupBy(item => item.SqlIdentifier, StringComparer.OrdinalIgnoreCase)
                     .Select(static group => group.First()))
        {
            model.TransformBindingTargetList.Add(new TransformBindingTarget
            {
                Id = $"{bindingId}:target:{model.TransformBindingTargetList.Count + 1}",
                TransformBinding = bindingRow,
                SqlIdentifier = target.SqlIdentifier
            });
        }

        foreach (var rowset in bound.Rowsets)
        {
            AddRowset(model, bindingRow, rowset);
        }

        var rowsetsById = model.RowsetList.ToDictionary(static item => item.Id, StringComparer.Ordinal);
        var columnsById = model.ColumnList.ToDictionary(static item => item.Id, StringComparer.Ordinal);

        foreach (var operationBinding in bound.StoredProcedureOperationBindings)
        {
            model.StoredProcedureOperationBindingList.Add(new StoredProcedureOperationBinding
            {
                Id = $"{bindingId}:stored-procedure-operation:{model.StoredProcedureOperationBindingList.Count + 1}",
                TransformBinding = bindingRow,
                Rowset = rowsetsById[operationBinding.Rowset.Id],
                MetaTransformScriptStoredProcedureContractOperationId = operationBinding.MetaTransformScriptStoredProcedureContractOperationId
            });
        }

        foreach (var rowset in bound.Rowsets)
        {
            foreach (var input in rowset.Inputs)
            {
                model.SourceTargetList.Add(new SourceTarget
                {
                    Id = $"{rowset.Id}:input:{input.Ordinal + 1}",
                    Target = rowsetsById[rowset.Id],
                    Source = rowsetsById[input.Rowset.Id],
                    Ordinal = input.Ordinal.ToString(CultureInfo.InvariantCulture),
                    InputRole = input.InputRole ?? string.Empty
                });
            }
        }

        var boundTableSourcesByMetaTransformScriptTableReferenceId = new Dictionary<string, TableSource>(StringComparer.Ordinal);
        foreach (var source in bound.TableSources.Select((item, ordinal) => (Item: item, Ordinal: ordinal)))
        {
            var tableSourceId = $"{bindingId}:table-source:{source.Ordinal + 1}";
            var tableSource = new TableSource
            {
                Id = tableSourceId,
                TransformBinding = bindingRow,
                Rowset = rowsetsById[source.Item.Rowset.Id],
                ExposedName = source.Item.ExposedName,
                MetaTransformScriptTableReferenceId = source.Item.SyntaxTableReferenceId
            };

            model.TableSourceList.Add(tableSource);
            boundTableSourcesByMetaTransformScriptTableReferenceId[source.Item.SyntaxTableReferenceId] = tableSource;
        }

        foreach (var columnReference in bound.ColumnReferences.Select((item, ordinal) => (Item: item, Ordinal: ordinal)))
        {
            if (!boundTableSourcesByMetaTransformScriptTableReferenceId.TryGetValue(
                columnReference.Item.ResolvedTableSource.SyntaxTableReferenceId,
                out var resolvedTableSource))
            {
                continue;
            }

            model.ColumnReferenceList.Add(new ColumnReference
            {
                Id = $"{bindingId}:column-reference:{columnReference.Ordinal + 1}",
                TransformBinding = bindingRow,
                Column = columnsById[columnReference.Item.ResolvedColumn.Id],
                TableSource = resolvedTableSource,
                MetaTransformScriptColumnReferenceId = columnReference.Item.SyntaxColumnReferenceId
            });
        }

        if (bound.TopLevelRowset is not null)
        {
            model.OutputRowsetList.Add(new OutputRowset
            {
                Id = $"{bindingId}:final-rowset",
                TransformBinding = bindingRow,
                Rowset = rowsetsById[bound.TopLevelRowset.Id]
            });
        }

        return model;
    }

    private static void AddRowset(
        MetaTransformBindingModel model,
        TransformBinding binding,
        RuntimeRowset rowset)
    {
        if (model.RowsetList.Any(item => string.Equals(item.Id, rowset.Id, StringComparison.Ordinal)))
        {
            return;
        }

        var rowsetRow = new Rowset
        {
            Id = rowset.Id,
            TransformBinding = binding,
            Name = rowset.Name,
            DerivationKind = rowset.DerivationKind,
            SqlIdentifier = rowset.SqlIdentifier ?? string.Empty
        };

        model.RowsetList.Add(rowsetRow);

        foreach (var column in rowset.Columns)
        {
            model.ColumnList.Add(new Column
            {
                Id = column.Id,
                Rowset = rowsetRow,
                Name = column.Name,
                Ordinal = column.Ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }
    }
}
