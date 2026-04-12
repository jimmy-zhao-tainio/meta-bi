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
            TransformScriptName = bound.TransformScriptName,
            ActiveLanguageProfileId = bound.ActiveLanguageProfileId
        };

        model.TransformBindingList.Add(bindingRow);

        foreach (var target in (targets ?? [])
                     .GroupBy(item => item.SqlIdentifier, StringComparer.OrdinalIgnoreCase)
                     .Select(static group => group.First()))
        {
            model.TransformBindingTargetList.Add(new TransformBindingTarget
            {
                Id = $"{bindingId}:target:{model.TransformBindingTargetList.Count + 1}",
                TransformBindingId = bindingId,
                SqlIdentifier = target.SqlIdentifier
            });
        }

        foreach (var rowset in bound.Rowsets)
        {
            AddRowset(model, bindingId, rowset);
        }

        foreach (var rowset in bound.Rowsets)
        {
            foreach (var input in rowset.Inputs)
            {
                model.SourceTargetList.Add(new SourceTarget
                {
                    Id = $"{rowset.Id}:input:{input.Ordinal + 1}",
                    TargetId = rowset.Id,
                    SourceId = input.Rowset.Id,
                    Ordinal = input.Ordinal.ToString(CultureInfo.InvariantCulture),
                    InputRole = input.InputRole ?? string.Empty
                });
            }
        }

        var boundTableSourceIdsByMetaTransformScriptTableReferenceId = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var source in bound.TableSources.Select((item, ordinal) => (Item: item, Ordinal: ordinal)))
        {
            var tableSourceId = $"{bindingId}:table-source:{source.Ordinal + 1}";
            model.TableSourceList.Add(new TableSource
            {
                Id = tableSourceId,
                TransformBindingId = bindingId,
                RowsetId = source.Item.Rowset.Id,
                ExposedName = source.Item.ExposedName,
                MetaTransformScriptTableReferenceId = source.Item.SyntaxTableReferenceId
            });

            boundTableSourceIdsByMetaTransformScriptTableReferenceId[source.Item.SyntaxTableReferenceId] = tableSourceId;
        }

        foreach (var columnReference in bound.ColumnReferences.Select((item, ordinal) => (Item: item, Ordinal: ordinal)))
        {
            if (!boundTableSourceIdsByMetaTransformScriptTableReferenceId.TryGetValue(
                columnReference.Item.ResolvedTableSource.SyntaxTableReferenceId,
                out var resolvedTableSourceId))
            {
                continue;
            }

            model.ColumnReferenceList.Add(new ColumnReference
            {
                Id = $"{bindingId}:column-reference:{columnReference.Ordinal + 1}",
                TransformBindingId = bindingId,
                ColumnId = columnReference.Item.ResolvedColumn.Id,
                TableSourceId = resolvedTableSourceId,
                MetaTransformScriptColumnReferenceId = columnReference.Item.SyntaxColumnReferenceId
            });
        }

        if (bound.TopLevelRowset is not null)
        {
            model.OutputRowsetList.Add(new OutputRowset
            {
                Id = $"{bindingId}:final-rowset",
                TransformBindingId = bindingId,
                RowsetId = bound.TopLevelRowset.Id
            });
        }

        return model;
    }

    private static void AddRowset(
        MetaTransformBindingModel model,
        string bindingId,
        RuntimeRowset rowset)
    {
        if (model.RowsetList.Any(item => string.Equals(item.Id, rowset.Id, StringComparison.Ordinal)))
        {
            return;
        }

        model.RowsetList.Add(new Rowset
        {
            Id = rowset.Id,
            TransformBindingId = bindingId,
            Name = rowset.Name,
            DerivationKind = rowset.DerivationKind,
            SqlIdentifier = rowset.SqlIdentifier ?? string.Empty
        });

        foreach (var column in rowset.Columns)
        {
            model.ColumnList.Add(new Column
            {
                Id = column.Id,
                RowsetId = rowset.Id,
                Name = column.Name,
                Ordinal = column.Ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }
    }
}
