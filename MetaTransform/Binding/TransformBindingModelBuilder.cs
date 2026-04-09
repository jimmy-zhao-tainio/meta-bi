using System.Globalization;
using MetaTransformBinding;

namespace MetaTransform.Binding;

internal static class TransformBindingModelBuilder
{
    public static MetaTransformBindingModel Create(BoundTransform bound)
    {
        ArgumentNullException.ThrowIfNull(bound);

        var model = MetaTransformBindingModel.CreateEmpty();
        var bindingId = $"{bound.TransformScriptId}:binding";
        var bindingRow = new TransformBinding
        {
            Id = bindingId,
            TransformScriptId = bound.TransformScriptId,
            TransformScriptName = bound.TransformScriptName,
            ActiveLanguageProfileId = bound.ActiveLanguageProfileId
        };

        model.TransformBindingList.Add(bindingRow);

        foreach (var issue in bound.Issues.Select((item, ordinal) => (Item: item, Ordinal: ordinal)))
        {
            model.BoundIssueList.Add(new BoundIssue
            {
                Id = $"{bindingId}:issue:{issue.Ordinal + 1}",
                OwnerId = bindingId,
                Code = issue.Item.Code,
                Message = issue.Item.Message,
                Severity = "Error",
                SyntaxId = issue.Item.SyntaxId ?? string.Empty
            });
        }

        foreach (var rowset in bound.BoundRowsets)
        {
            AddRowset(model, bindingId, rowset);
        }

        foreach (var rowset in bound.BoundRowsets)
        {
            foreach (var input in rowset.Inputs)
            {
                model.BoundRowsetInputList.Add(new BoundRowsetInput
                {
                    Id = $"{rowset.Id}:input:{input.Ordinal + 1}",
                    OwnerId = rowset.Id,
                    ValueId = input.Rowset.Id,
                    Ordinal = input.Ordinal.ToString(CultureInfo.InvariantCulture),
                    InputRole = input.InputRole ?? string.Empty
                });
            }
        }

        var boundTableSourceIdsBySyntaxTableReferenceId = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var source in bound.BoundTableSources.Select((item, ordinal) => (Item: item, Ordinal: ordinal)))
        {
            var tableSourceId = $"{bindingId}:table-source:{source.Ordinal + 1}";
            model.BoundTableSourceList.Add(new BoundTableSource
            {
                Id = tableSourceId,
                OwnerId = bindingId,
                ValueId = source.Item.Rowset.Id,
                ExposedName = source.Item.ExposedName,
                SyntaxTableReferenceId = source.Item.SyntaxTableReferenceId
            });

            boundTableSourceIdsBySyntaxTableReferenceId[source.Item.SyntaxTableReferenceId] = tableSourceId;
        }

        foreach (var columnReference in bound.BoundColumnReferences.Select((item, ordinal) => (Item: item, Ordinal: ordinal)))
        {
            if (!boundTableSourceIdsBySyntaxTableReferenceId.TryGetValue(
                columnReference.Item.ResolvedTableSource.SyntaxTableReferenceId,
                out var resolvedTableSourceId))
            {
                continue;
            }

            model.BoundColumnReferenceList.Add(new BoundColumnReference
            {
                Id = $"{bindingId}:column-reference:{columnReference.Ordinal + 1}",
                OwnerId = bindingId,
                ValueId = columnReference.Item.ResolvedColumn.Id,
                ResolvedTableSourceId = resolvedTableSourceId,
                SyntaxColumnReferenceId = columnReference.Item.SyntaxColumnReferenceId
            });
        }

        if (bound.TopLevelRowset is not null)
        {
            model.TransformBindingFinalRowsetLinkList.Add(new TransformBindingFinalRowsetLink
            {
                Id = $"{bindingId}:final-rowset",
                OwnerId = bindingId,
                ValueId = bound.TopLevelRowset.Id
            });
        }

        return model;
    }

    private static void AddRowset(
        MetaTransformBindingModel model,
        string bindingId,
        RuntimeBoundRowset rowset)
    {
        if (model.BoundRowsetList.Any(item => string.Equals(item.Id, rowset.Id, StringComparison.Ordinal)))
        {
            return;
        }

        model.BoundRowsetList.Add(new BoundRowset
        {
            Id = rowset.Id,
            OwnerId = bindingId,
            Name = rowset.Name,
            DerivationKind = rowset.DerivationKind,
            RowsetRole = rowset.RowsetRole ?? string.Empty,
            SyntaxId = rowset.SyntaxId ?? string.Empty,
            SourceTableId = rowset.SourceTableId ?? string.Empty
        });

        foreach (var column in rowset.Columns)
        {
            model.BoundColumnList.Add(new BoundColumn
            {
                Id = column.Id,
                OwnerId = rowset.Id,
                Name = column.Name,
                Ordinal = column.Ordinal.ToString(CultureInfo.InvariantCulture),
                SourceFieldId = column.SourceFieldId ?? string.Empty,
                SourceTableId = column.SourceTableId ?? string.Empty
            });
        }
    }
}
