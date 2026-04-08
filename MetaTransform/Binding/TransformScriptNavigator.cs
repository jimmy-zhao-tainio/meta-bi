using System.Globalization;
using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed class TransformScriptNavigator
{
    private readonly IReadOnlyDictionary<string, TransformScriptSelectStatementLink> scriptSelectStatementLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, SelectStatement> selectStatementById;
    private readonly IReadOnlyDictionary<string, SelectStatementQueryExpressionLink> selectStatementQueryExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, QuerySpecification> querySpecificationByQueryExpressionId;
    private readonly IReadOnlyDictionary<string, QuerySpecificationFromClauseLink> fromClauseLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, FromClause> fromClauseById;
    private readonly IReadOnlyDictionary<string, List<FromClauseTableReferencesItem>> fromClauseTableReferencesByOwnerId;
    private readonly IReadOnlyDictionary<string, TableReference> tableReferenceById;
    private readonly IReadOnlyDictionary<string, TableReferenceWithAlias> tableReferenceWithAliasByTableReferenceId;
    private readonly IReadOnlyDictionary<string, TableReferenceWithAliasAliasLink> tableReferenceAliasLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, NamedTableReference> namedTableReferenceByAliasBaseId;
    private readonly IReadOnlyDictionary<string, NamedTableReferenceSchemaObjectLink> namedTableReferenceSchemaObjectLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, SchemaObjectName> schemaObjectNameById;
    private readonly IReadOnlyDictionary<string, MultiPartIdentifier> multiPartIdentifierById;
    private readonly IReadOnlyDictionary<string, List<MultiPartIdentifierIdentifiersItem>> multiPartIdentifierItemsByOwnerId;
    private readonly IReadOnlyDictionary<string, Identifier> identifierById;
    private readonly IReadOnlyDictionary<string, JoinTableReference> joinTableReferenceByTableReferenceId;
    private readonly IReadOnlyDictionary<string, JoinTableReferenceFirstTableReferenceLink> joinFirstTableReferenceLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, JoinTableReferenceSecondTableReferenceLink> joinSecondTableReferenceLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, List<QuerySpecificationSelectElementsItem>> selectElementsByOwnerId;
    private readonly IReadOnlyDictionary<string, SelectElement> selectElementById;
    private readonly IReadOnlyDictionary<string, SelectScalarExpression> selectScalarExpressionBySelectElementId;
    private readonly IReadOnlyDictionary<string, SelectScalarExpressionExpressionLink> selectScalarExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, SelectScalarExpressionColumnNameLink> selectScalarExpressionColumnNameLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, IdentifierOrValueExpression> identifierOrValueExpressionById;
    private readonly IReadOnlyDictionary<string, IdentifierOrValueExpressionIdentifierLink> identifierOrValueExpressionIdentifierLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, SelectStarExpression> selectStarExpressionBySelectElementId;
    private readonly IReadOnlyDictionary<string, SelectStarExpressionQualifierLink> selectStarQualifierLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, ScalarExpression> scalarExpressionById;
    private readonly IReadOnlyDictionary<string, PrimaryExpression> primaryExpressionByScalarExpressionId;
    private readonly IReadOnlyDictionary<string, ColumnReferenceExpression> columnReferenceExpressionByPrimaryExpressionId;
    private readonly IReadOnlyDictionary<string, ColumnReferenceExpressionMultiPartIdentifierLink> columnReferenceExpressionLinkByOwnerId;

    public TransformScriptNavigator(MetaTransformScriptModel model)
    {
        scriptSelectStatementLinkByOwnerId = model.TransformScriptSelectStatementLinkList.ToDictionary(item => item.OwnerId, StringComparer.Ordinal);
        selectStatementById = model.SelectStatementList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        selectStatementQueryExpressionLinkByOwnerId = model.SelectStatementQueryExpressionLinkList.ToDictionary(item => item.OwnerId, StringComparer.Ordinal);
        querySpecificationByQueryExpressionId = model.QuerySpecificationList.ToDictionary(item => item.BaseId, StringComparer.Ordinal);
        fromClauseLinkByOwnerId = model.QuerySpecificationFromClauseLinkList.ToDictionary(item => item.OwnerId, StringComparer.Ordinal);
        fromClauseById = model.FromClauseList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        fromClauseTableReferencesByOwnerId = GroupByOwner(model.FromClauseTableReferencesItemList);
        tableReferenceById = model.TableReferenceList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        tableReferenceWithAliasByTableReferenceId = model.TableReferenceWithAliasList.ToDictionary(item => item.BaseId, StringComparer.Ordinal);
        tableReferenceAliasLinkByOwnerId = model.TableReferenceWithAliasAliasLinkList.ToDictionary(item => item.OwnerId, StringComparer.Ordinal);
        namedTableReferenceByAliasBaseId = model.NamedTableReferenceList.ToDictionary(item => item.BaseId, StringComparer.Ordinal);
        namedTableReferenceSchemaObjectLinkByOwnerId = model.NamedTableReferenceSchemaObjectLinkList.ToDictionary(item => item.OwnerId, StringComparer.Ordinal);
        schemaObjectNameById = model.SchemaObjectNameList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        multiPartIdentifierById = model.MultiPartIdentifierList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        multiPartIdentifierItemsByOwnerId = GroupByOwner(model.MultiPartIdentifierIdentifiersItemList);
        identifierById = model.IdentifierList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        joinTableReferenceByTableReferenceId = model.JoinTableReferenceList.ToDictionary(item => item.BaseId, StringComparer.Ordinal);
        joinFirstTableReferenceLinkByOwnerId = model.JoinTableReferenceFirstTableReferenceLinkList.ToDictionary(item => item.OwnerId, StringComparer.Ordinal);
        joinSecondTableReferenceLinkByOwnerId = model.JoinTableReferenceSecondTableReferenceLinkList.ToDictionary(item => item.OwnerId, StringComparer.Ordinal);
        selectElementsByOwnerId = GroupByOwner(model.QuerySpecificationSelectElementsItemList);
        selectElementById = model.SelectElementList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        selectScalarExpressionBySelectElementId = model.SelectScalarExpressionList.ToDictionary(item => item.BaseId, StringComparer.Ordinal);
        selectScalarExpressionLinkByOwnerId = model.SelectScalarExpressionExpressionLinkList.ToDictionary(item => item.OwnerId, StringComparer.Ordinal);
        selectScalarExpressionColumnNameLinkByOwnerId = model.SelectScalarExpressionColumnNameLinkList.ToDictionary(item => item.OwnerId, StringComparer.Ordinal);
        identifierOrValueExpressionById = model.IdentifierOrValueExpressionList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        identifierOrValueExpressionIdentifierLinkByOwnerId = model.IdentifierOrValueExpressionIdentifierLinkList.ToDictionary(item => item.OwnerId, StringComparer.Ordinal);
        selectStarExpressionBySelectElementId = model.SelectStarExpressionList.ToDictionary(item => item.BaseId, StringComparer.Ordinal);
        selectStarQualifierLinkByOwnerId = model.SelectStarExpressionQualifierLinkList.ToDictionary(item => item.OwnerId, StringComparer.Ordinal);
        scalarExpressionById = model.ScalarExpressionList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        primaryExpressionByScalarExpressionId = model.PrimaryExpressionList.ToDictionary(item => item.BaseId, StringComparer.Ordinal);
        columnReferenceExpressionByPrimaryExpressionId = model.ColumnReferenceExpressionList.ToDictionary(item => item.BaseId, StringComparer.Ordinal);
        columnReferenceExpressionLinkByOwnerId = model.ColumnReferenceExpressionMultiPartIdentifierLinkList.ToDictionary(item => item.OwnerId, StringComparer.Ordinal);
    }

    public SelectStatement? TryGetSelectStatement(TransformScript script)
    {
        if (!scriptSelectStatementLinkByOwnerId.TryGetValue(script.Id, out var link))
        {
            return null;
        }

        return selectStatementById.GetValueOrDefault(link.ValueId);
    }

    public QuerySpecification? TryGetQuerySpecification(SelectStatement selectStatement)
    {
        if (!selectStatementQueryExpressionLinkByOwnerId.TryGetValue(selectStatement.Id, out var link))
        {
            return null;
        }

        return querySpecificationByQueryExpressionId.GetValueOrDefault(link.ValueId);
    }

    public FromClause? TryGetFromClause(QuerySpecification querySpecification)
    {
        if (!fromClauseLinkByOwnerId.TryGetValue(querySpecification.Id, out var link))
        {
            return null;
        }

        return fromClauseById.GetValueOrDefault(link.ValueId);
    }

    public IReadOnlyList<TableReference> GetTableReferences(FromClause fromClause)
    {
        if (!fromClauseTableReferencesByOwnerId.TryGetValue(fromClause.Id, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => tableReferenceById.GetValueOrDefault(item.ValueId))
            .Where(item => item is not null)
            .Cast<TableReference>()
            .ToArray();
    }

    public NamedTableReference? TryGetNamedTableReference(TableReference tableReference)
    {
        if (!tableReferenceWithAliasByTableReferenceId.TryGetValue(tableReference.Id, out var aliasBase))
        {
            return null;
        }

        return namedTableReferenceByAliasBaseId.GetValueOrDefault(aliasBase.Id);
    }

    public (TableReference? First, TableReference? Second)? TryGetJoinChildren(TableReference tableReference)
    {
        if (!joinTableReferenceByTableReferenceId.TryGetValue(tableReference.Id, out var joinBase))
        {
            return null;
        }

        if (!joinFirstTableReferenceLinkByOwnerId.TryGetValue(joinBase.Id, out var firstLink) ||
            !joinSecondTableReferenceLinkByOwnerId.TryGetValue(joinBase.Id, out var secondLink))
        {
            return null;
        }

        return (
            tableReferenceById.GetValueOrDefault(firstLink.ValueId),
            tableReferenceById.GetValueOrDefault(secondLink.ValueId));
    }

    public string? TryGetTableAlias(TableReference tableReference)
    {
        if (!tableReferenceWithAliasByTableReferenceId.TryGetValue(tableReference.Id, out var aliasBase))
        {
            return null;
        }

        if (!tableReferenceAliasLinkByOwnerId.TryGetValue(aliasBase.Id, out var link))
        {
            return null;
        }

        return identifierById.GetValueOrDefault(link.ValueId)?.Value;
    }

    public IReadOnlyList<string> GetNamedTableReferenceParts(NamedTableReference namedTableReference)
    {
        if (!namedTableReferenceSchemaObjectLinkByOwnerId.TryGetValue(namedTableReference.Id, out var link))
        {
            return [];
        }

        if (!schemaObjectNameById.TryGetValue(link.ValueId, out var schemaObjectName))
        {
            return [];
        }

        return GetMultiPartIdentifierParts(schemaObjectName.BaseId);
    }

    public IReadOnlyList<SelectElement> GetSelectElements(QuerySpecification querySpecification)
    {
        if (!selectElementsByOwnerId.TryGetValue(querySpecification.Id, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => selectElementById.GetValueOrDefault(item.ValueId))
            .Where(item => item is not null)
            .Cast<SelectElement>()
            .ToArray();
    }

    public SelectScalarExpression? TryGetSelectScalarExpression(SelectElement selectElement) =>
        selectScalarExpressionBySelectElementId.GetValueOrDefault(selectElement.Id);

    public ScalarExpression? TryGetSelectScalarExpressionBody(SelectScalarExpression selectScalarExpression)
    {
        if (!selectScalarExpressionLinkByOwnerId.TryGetValue(selectScalarExpression.Id, out var link))
        {
            return null;
        }

        return scalarExpressionById.GetValueOrDefault(link.ValueId);
    }

    public string? TryGetSelectScalarExpressionAlias(SelectScalarExpression selectScalarExpression)
    {
        if (!selectScalarExpressionColumnNameLinkByOwnerId.TryGetValue(selectScalarExpression.Id, out var link))
        {
            return null;
        }

        if (!identifierOrValueExpressionById.TryGetValue(link.ValueId, out var aliasValue))
        {
            return null;
        }

        if (identifierOrValueExpressionIdentifierLinkByOwnerId.TryGetValue(aliasValue.Id, out var identifierLink))
        {
            return identifierById.GetValueOrDefault(identifierLink.ValueId)?.Value;
        }

        return string.IsNullOrWhiteSpace(aliasValue.Value) ? null : aliasValue.Value;
    }

    public SelectStarExpression? TryGetSelectStarExpression(SelectElement selectElement) =>
        selectStarExpressionBySelectElementId.GetValueOrDefault(selectElement.Id);

    public IReadOnlyList<string> GetSelectStarQualifierParts(SelectStarExpression selectStarExpression)
    {
        if (!selectStarQualifierLinkByOwnerId.TryGetValue(selectStarExpression.Id, out var link))
        {
            return [];
        }

        return GetMultiPartIdentifierParts(link.ValueId);
    }

    public ColumnReferenceExpression? TryGetDirectColumnReference(ScalarExpression scalarExpression)
    {
        if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpression.Id, out var primaryExpression))
        {
            return null;
        }

        return columnReferenceExpressionByPrimaryExpressionId.GetValueOrDefault(primaryExpression.Id);
    }

    public IReadOnlyList<string> GetColumnReferenceParts(ColumnReferenceExpression columnReferenceExpression)
    {
        if (!columnReferenceExpressionLinkByOwnerId.TryGetValue(columnReferenceExpression.Id, out var link))
        {
            return [];
        }

        return GetMultiPartIdentifierParts(link.ValueId);
    }

    private IReadOnlyList<string> GetMultiPartIdentifierParts(string multiPartIdentifierId)
    {
        if (!multiPartIdentifierItemsByOwnerId.TryGetValue(multiPartIdentifierId, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => identifierById.GetValueOrDefault(item.ValueId)?.Value)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Cast<string>()
            .ToArray();
    }

    private static IReadOnlyDictionary<string, List<T>> GroupByOwner<T>(IEnumerable<T> rows)
    {
        return rows
            .GroupBy(item => (string?)item!.GetType().GetProperty("OwnerId")?.GetValue(item) ?? string.Empty, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.ToList(), StringComparer.Ordinal);
    }

    private static int ParseOrdinal(string ordinal)
    {
        return int.TryParse(ordinal, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : int.MaxValue;
    }
}
