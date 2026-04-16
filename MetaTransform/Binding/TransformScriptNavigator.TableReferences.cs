using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformScriptNavigator
{
    public XmlNodesTableReference? TryGetXmlNodesTableReference(TableReference tableReference)
    {
        if (!tableReferenceWithAliasByTableReferenceId.TryGetValue(tableReference.Id, out var aliasBase))
        {
            return null;
        }

        if (!tableReferenceWithAliasAndColumnsByBaseId.TryGetValue(aliasBase.Id, out var aliasAndColumnsBase))
        {
            return null;
        }

        return model.XmlNodesTableReferenceList
            .FirstOrDefault(item => string.Equals(item.TableReferenceWithAliasAndColumnsId, aliasAndColumnsBase.Id, StringComparison.Ordinal));
    }

    public ScalarExpression? TryGetXmlNodesTableReferenceTargetExpression(XmlNodesTableReference xmlNodesTableReference)
    {
        var link = model.XmlNodesTableReferenceTargetExpressionLinkList
            .FirstOrDefault(item => string.Equals(item.XmlNodesTableReferenceId, xmlNodesTableReference.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId);
    }

    public FullTextTableReference? TryGetFullTextTableReference(TableReference tableReference)
    {
        if (!tableReferenceWithAliasByTableReferenceId.TryGetValue(tableReference.Id, out var aliasBase))
        {
            return null;
        }

        return model.FullTextTableReferenceList
            .FirstOrDefault(item => string.Equals(item.TableReferenceWithAliasId, aliasBase.Id, StringComparison.Ordinal));
    }

    public IReadOnlyList<string> GetFullTextTableReferenceTableNameParts(FullTextTableReference fullTextTableReference)
    {
        var link = model.FullTextTableReferenceTableNameLinkList
            .FirstOrDefault(item => string.Equals(item.FullTextTableReferenceId, fullTextTableReference.Id, StringComparison.Ordinal));
        if (link is null)
        {
            return [];
        }

        if (!schemaObjectNameById.TryGetValue(link.SchemaObjectNameId, out var schemaObjectName))
        {
            return [];
        }

        return GetMultiPartIdentifierParts(schemaObjectName.MultiPartIdentifierId);
    }

    public IReadOnlyList<ColumnReferenceExpression> GetFullTextTableReferenceColumns(FullTextTableReference fullTextTableReference)
    {
        return model.FullTextTableReferenceColumnsItemList
            .Where(item => string.Equals(item.FullTextTableReferenceId, fullTextTableReference.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => model.ColumnReferenceExpressionList
                .FirstOrDefault(column => string.Equals(column.Id, item.ColumnReferenceExpressionId, StringComparison.Ordinal)))
            .Where(item => item is not null)
            .Cast<ColumnReferenceExpression>()
            .ToArray();
    }

    public ScalarExpression? TryGetFullTextTableReferenceSearchCondition(FullTextTableReference fullTextTableReference)
    {
        var link = model.FullTextTableReferenceSearchConditionLinkList
            .FirstOrDefault(item => string.Equals(item.FullTextTableReferenceId, fullTextTableReference.Id, StringComparison.Ordinal));
        if (link is null)
        {
            return null;
        }

        return TryGetScalarExpressionFromValueExpressionId(link.ValueExpressionId);
    }

    public PivotedTableReference? TryGetPivotedTableReference(TableReference tableReference)
    {
        if (!tableReferenceWithAliasByTableReferenceId.TryGetValue(tableReference.Id, out var aliasBase))
        {
            return null;
        }

        return model.PivotedTableReferenceList
            .FirstOrDefault(item => string.Equals(item.TableReferenceWithAliasId, aliasBase.Id, StringComparison.Ordinal));
    }

    public TableReference? TryGetPivotedTableReferenceSourceTableReference(PivotedTableReference pivotedTableReference)
    {
        var link = model.PivotedTableReferenceTableReferenceLinkList
            .FirstOrDefault(item => string.Equals(item.PivotedTableReferenceId, pivotedTableReference.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : tableReferenceById.GetValueOrDefault(link.TableReferenceId);
    }

    public IReadOnlyList<ColumnReferenceExpression> GetPivotedTableReferenceValueColumns(PivotedTableReference pivotedTableReference)
    {
        return model.PivotedTableReferenceValueColumnsItemList
            .Where(item => string.Equals(item.PivotedTableReferenceId, pivotedTableReference.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => model.ColumnReferenceExpressionList
                .FirstOrDefault(column => string.Equals(column.Id, item.ColumnReferenceExpressionId, StringComparison.Ordinal)))
            .Where(item => item is not null)
            .Cast<ColumnReferenceExpression>()
            .ToArray();
    }

    public ColumnReferenceExpression? TryGetPivotedTableReferencePivotColumn(PivotedTableReference pivotedTableReference)
    {
        var link = model.PivotedTableReferencePivotColumnLinkList
            .FirstOrDefault(item => string.Equals(item.PivotedTableReferenceId, pivotedTableReference.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : model.ColumnReferenceExpressionList
                .FirstOrDefault(item => string.Equals(item.Id, link.ColumnReferenceExpressionId, StringComparison.Ordinal));
    }

    public IReadOnlyList<string> GetPivotedTableReferenceInColumns(PivotedTableReference pivotedTableReference)
    {
        return model.PivotedTableReferenceInColumnsItemList
            .Where(item => string.Equals(item.PivotedTableReferenceId, pivotedTableReference.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => identifierById.GetValueOrDefault(item.IdentifierId)?.Value)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Cast<string>()
            .ToArray();
    }

    public UnpivotedTableReference? TryGetUnpivotedTableReference(TableReference tableReference)
    {
        if (!tableReferenceWithAliasByTableReferenceId.TryGetValue(tableReference.Id, out var aliasBase))
        {
            return null;
        }

        return model.UnpivotedTableReferenceList
            .FirstOrDefault(item => string.Equals(item.TableReferenceWithAliasId, aliasBase.Id, StringComparison.Ordinal));
    }

    public TableReference? TryGetUnpivotedTableReferenceSourceTableReference(UnpivotedTableReference unpivotedTableReference)
    {
        var link = model.UnpivotedTableReferenceTableReferenceLinkList
            .FirstOrDefault(item => string.Equals(item.UnpivotedTableReferenceId, unpivotedTableReference.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : tableReferenceById.GetValueOrDefault(link.TableReferenceId);
    }

    public IReadOnlyList<ColumnReferenceExpression> GetUnpivotedTableReferenceInColumns(UnpivotedTableReference unpivotedTableReference)
    {
        return model.UnpivotedTableReferenceInColumnsItemList
            .Where(item => string.Equals(item.UnpivotedTableReferenceId, unpivotedTableReference.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => model.ColumnReferenceExpressionList
                .FirstOrDefault(column => string.Equals(column.Id, item.ColumnReferenceExpressionId, StringComparison.Ordinal)))
            .Where(item => item is not null)
            .Cast<ColumnReferenceExpression>()
            .ToArray();
    }

    public string? TryGetUnpivotedTableReferenceValueColumnName(UnpivotedTableReference unpivotedTableReference)
    {
        var link = model.UnpivotedTableReferenceValueColumnLinkList
            .FirstOrDefault(item => string.Equals(item.UnpivotedTableReferenceId, unpivotedTableReference.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : identifierById.GetValueOrDefault(link.IdentifierId)?.Value;
    }

    public string? TryGetUnpivotedTableReferencePivotColumnName(UnpivotedTableReference unpivotedTableReference)
    {
        var link = model.UnpivotedTableReferencePivotColumnLinkList
            .FirstOrDefault(item => string.Equals(item.UnpivotedTableReferenceId, unpivotedTableReference.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : identifierById.GetValueOrDefault(link.IdentifierId)?.Value;
    }
}
