namespace MetaTransformScript;

public sealed partial class TransformScriptNavigator
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
            .FirstOrDefault(item => string.Equals(item.TableReferenceWithAliasAndColumns.Id, aliasAndColumnsBase.Id, StringComparison.Ordinal));
    }

    public ScalarExpression? TryGetXmlNodesTableReferenceTargetExpression(XmlNodesTableReference xmlNodesTableReference)
    {
        var link = model.XmlNodesTableReferenceTargetExpressionLinkList
            .FirstOrDefault(item => string.Equals(item.XmlNodesTableReference.Id, xmlNodesTableReference.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : scalarExpressionById.GetValueOrDefault(link.ScalarExpression.Id);
    }

    public FullTextTableReference? TryGetFullTextTableReference(TableReference tableReference)
    {
        if (!tableReferenceWithAliasByTableReferenceId.TryGetValue(tableReference.Id, out var aliasBase))
        {
            return null;
        }

        return model.FullTextTableReferenceList
            .FirstOrDefault(item => string.Equals(item.TableReferenceWithAlias.Id, aliasBase.Id, StringComparison.Ordinal));
    }

    public IReadOnlyList<string> GetFullTextTableReferenceTableNameParts(FullTextTableReference fullTextTableReference)
    {
        var link = model.FullTextTableReferenceTableNameLinkList
            .FirstOrDefault(item => string.Equals(item.FullTextTableReference.Id, fullTextTableReference.Id, StringComparison.Ordinal));
        if (link is null)
        {
            return [];
        }

        if (!schemaObjectNameById.TryGetValue(link.SchemaObjectName.Id, out var schemaObjectName))
        {
            return [];
        }

        return GetMultiPartIdentifierParts(schemaObjectName.MultiPartIdentifier.Id);
    }

    public IReadOnlyList<ColumnReferenceExpression> GetFullTextTableReferenceColumns(FullTextTableReference fullTextTableReference)
    {
        return model.FullTextTableReferenceColumnsItemList
            .Where(item => string.Equals(item.FullTextTableReference.Id, fullTextTableReference.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => model.ColumnReferenceExpressionList
                .FirstOrDefault(column => string.Equals(column.Id, item.ColumnReferenceExpression.Id, StringComparison.Ordinal)))
            .Where(item => item is not null)
            .Cast<ColumnReferenceExpression>()
            .ToArray();
    }

    public ScalarExpression? TryGetFullTextTableReferenceSearchCondition(FullTextTableReference fullTextTableReference)
    {
        var link = model.FullTextTableReferenceSearchConditionLinkList
            .FirstOrDefault(item => string.Equals(item.FullTextTableReference.Id, fullTextTableReference.Id, StringComparison.Ordinal));
        if (link is null)
        {
            return null;
        }

        return TryGetScalarExpressionFromValueExpressionId(link.ValueExpression.Id);
    }

    public PivotedTableReference? TryGetPivotedTableReference(TableReference tableReference)
    {
        if (!tableReferenceWithAliasByTableReferenceId.TryGetValue(tableReference.Id, out var aliasBase))
        {
            return null;
        }

        return model.PivotedTableReferenceList
            .FirstOrDefault(item => string.Equals(item.TableReferenceWithAlias.Id, aliasBase.Id, StringComparison.Ordinal));
    }

    public TableReference? TryGetPivotedTableReferenceSourceTableReference(PivotedTableReference pivotedTableReference)
    {
        var link = model.PivotedTableReferenceTableReferenceLinkList
            .FirstOrDefault(item => string.Equals(item.PivotedTableReference.Id, pivotedTableReference.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : tableReferenceById.GetValueOrDefault(link.TableReference.Id);
    }

    public IReadOnlyList<ColumnReferenceExpression> GetPivotedTableReferenceValueColumns(PivotedTableReference pivotedTableReference)
    {
        return model.PivotedTableReferenceValueColumnsItemList
            .Where(item => string.Equals(item.PivotedTableReference.Id, pivotedTableReference.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => model.ColumnReferenceExpressionList
                .FirstOrDefault(column => string.Equals(column.Id, item.ColumnReferenceExpression.Id, StringComparison.Ordinal)))
            .Where(item => item is not null)
            .Cast<ColumnReferenceExpression>()
            .ToArray();
    }

    public ColumnReferenceExpression? TryGetPivotedTableReferencePivotColumn(PivotedTableReference pivotedTableReference)
    {
        var link = model.PivotedTableReferencePivotColumnLinkList
            .FirstOrDefault(item => string.Equals(item.PivotedTableReference.Id, pivotedTableReference.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : model.ColumnReferenceExpressionList
                .FirstOrDefault(item => string.Equals(item.Id, link.ColumnReferenceExpression.Id, StringComparison.Ordinal));
    }

    public IReadOnlyList<string> GetPivotedTableReferenceInColumns(PivotedTableReference pivotedTableReference)
    {
        return model.PivotedTableReferenceInColumnsItemList
            .Where(item => string.Equals(item.PivotedTableReference.Id, pivotedTableReference.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => identifierById.GetValueOrDefault(item.Identifier.Id)?.Value)
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
            .FirstOrDefault(item => string.Equals(item.TableReferenceWithAlias.Id, aliasBase.Id, StringComparison.Ordinal));
    }

    public TableReference? TryGetUnpivotedTableReferenceSourceTableReference(UnpivotedTableReference unpivotedTableReference)
    {
        var link = model.UnpivotedTableReferenceTableReferenceLinkList
            .FirstOrDefault(item => string.Equals(item.UnpivotedTableReference.Id, unpivotedTableReference.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : tableReferenceById.GetValueOrDefault(link.TableReference.Id);
    }

    public IReadOnlyList<ColumnReferenceExpression> GetUnpivotedTableReferenceInColumns(UnpivotedTableReference unpivotedTableReference)
    {
        return model.UnpivotedTableReferenceInColumnsItemList
            .Where(item => string.Equals(item.UnpivotedTableReference.Id, unpivotedTableReference.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => model.ColumnReferenceExpressionList
                .FirstOrDefault(column => string.Equals(column.Id, item.ColumnReferenceExpression.Id, StringComparison.Ordinal)))
            .Where(item => item is not null)
            .Cast<ColumnReferenceExpression>()
            .ToArray();
    }

    public string? TryGetUnpivotedTableReferenceValueColumnName(UnpivotedTableReference unpivotedTableReference)
    {
        var link = model.UnpivotedTableReferenceValueColumnLinkList
            .FirstOrDefault(item => string.Equals(item.UnpivotedTableReference.Id, unpivotedTableReference.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : identifierById.GetValueOrDefault(link.Identifier.Id)?.Value;
    }

    public string? TryGetUnpivotedTableReferencePivotColumnName(UnpivotedTableReference unpivotedTableReference)
    {
        var link = model.UnpivotedTableReferencePivotColumnLinkList
            .FirstOrDefault(item => string.Equals(item.UnpivotedTableReference.Id, unpivotedTableReference.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : identifierById.GetValueOrDefault(link.Identifier.Id)?.Value;
    }
}
