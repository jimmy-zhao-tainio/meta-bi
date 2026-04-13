using System.Collections.Concurrent;
using System.Globalization;
using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformScriptNavigator
{
    private static readonly ConcurrentDictionary<Type, string?> OwnerIdPropertyByType = new();

    private readonly MetaTransformScriptModel model;
    private readonly IReadOnlyDictionary<string, TransformScriptSelectStatementLink> scriptSelectStatementLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, SelectStatement> selectStatementById;
    private readonly IReadOnlyDictionary<string, SelectStatementQueryExpressionLink> selectStatementQueryExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLink> statementWithCtesLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, WithCtesAndXmlNamespaces> withCtesAndXmlNamespacesById;
    private readonly IReadOnlyDictionary<string, List<WithCtesAndXmlNamespacesCommonTableExpressionsItem>> commonTableExpressionsByWithClauseOwnerId;
    private readonly IReadOnlyDictionary<string, CommonTableExpression> commonTableExpressionById;
    private readonly IReadOnlyDictionary<string, CommonTableExpressionExpressionNameLink> commonTableExpressionNameLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, CommonTableExpressionQueryExpressionLink> commonTableExpressionQueryExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, List<CommonTableExpressionColumnsItem>> commonTableExpressionColumnsByOwnerId;
    private readonly IReadOnlyDictionary<string, QuerySpecification> querySpecificationByQueryExpressionId;
    private readonly IReadOnlyDictionary<string, BinaryQueryExpression> binaryQueryExpressionByQueryExpressionId;
    private readonly IReadOnlyDictionary<string, BinaryQueryExpressionFirstQueryExpressionLink> binaryQueryExpressionFirstQueryExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, BinaryQueryExpressionSecondQueryExpressionLink> binaryQueryExpressionSecondQueryExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, QuerySpecificationFromClauseLink> fromClauseLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, FromClause> fromClauseById;
    private readonly IReadOnlyDictionary<string, List<FromClauseTableReferencesItem>> fromClauseTableReferencesByOwnerId;
    private readonly IReadOnlyDictionary<string, QuerySpecificationGroupByClauseLink> groupByClauseLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, GroupByClause> groupByClauseById;
    private readonly IReadOnlyDictionary<string, List<GroupByClauseGroupingSpecificationsItem>> groupingSpecificationsByGroupByClauseOwnerId;
    private readonly IReadOnlyDictionary<string, GroupingSpecification> groupingSpecificationById;
    private readonly IReadOnlyDictionary<string, ExpressionGroupingSpecification> expressionGroupingSpecificationByBaseId;
    private readonly IReadOnlyDictionary<string, ExpressionGroupingSpecificationExpressionLink> expressionGroupingSpecificationExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, QuerySpecificationWhereClauseLink> whereClauseLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, WhereClause> whereClauseById;
    private readonly IReadOnlyDictionary<string, WhereClauseSearchConditionLink> whereClauseSearchConditionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, QuerySpecificationHavingClauseLink> havingClauseLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, HavingClause> havingClauseById;
    private readonly IReadOnlyDictionary<string, HavingClauseSearchConditionLink> havingClauseSearchConditionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, TableReference> tableReferenceById;
    private readonly IReadOnlyDictionary<string, TableReferenceWithAlias> tableReferenceWithAliasByTableReferenceId;
    private readonly IReadOnlyDictionary<string, TableReferenceWithAliasAliasLink> tableReferenceAliasLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, TableReferenceWithAliasAndColumns> tableReferenceWithAliasAndColumnsByBaseId;
    private readonly IReadOnlyDictionary<string, List<TableReferenceWithAliasAndColumnsColumnsItem>> tableReferenceWithAliasAndColumnsColumnsByOwnerId;
    private readonly IReadOnlyDictionary<string, NamedTableReference> namedTableReferenceByAliasBaseId;
    private readonly IReadOnlyDictionary<string, QueryDerivedTable> queryDerivedTableByBaseId;
    private readonly IReadOnlyDictionary<string, QueryDerivedTableQueryExpressionLink> queryDerivedTableQueryExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, InlineDerivedTable> inlineDerivedTableByBaseId;
    private readonly IReadOnlyDictionary<string, List<InlineDerivedTableRowValuesItem>> inlineDerivedTableRowValuesByOwnerId;
    private readonly IReadOnlyDictionary<string, RowValue> rowValueById;
    private readonly IReadOnlyDictionary<string, List<RowValueColumnValuesItem>> rowValueColumnValuesByOwnerId;
    private readonly IReadOnlyDictionary<string, SchemaObjectFunctionTableReference> schemaObjectFunctionTableReferenceByBaseId;
    private readonly IReadOnlyDictionary<string, SchemaObjectFunctionTableReferenceSchemaObjectLink> schemaObjectFunctionTableReferenceSchemaObjectLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, List<SchemaObjectFunctionTableReferenceParametersItem>> schemaObjectFunctionTableReferenceParametersByOwnerId;
    private readonly IReadOnlyDictionary<string, NamedTableReferenceSchemaObjectLink> namedTableReferenceSchemaObjectLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, SchemaObjectName> schemaObjectNameById;
    private readonly IReadOnlyDictionary<string, MultiPartIdentifier> multiPartIdentifierById;
    private readonly IReadOnlyDictionary<string, List<MultiPartIdentifierIdentifiersItem>> multiPartIdentifierItemsByOwnerId;
    private readonly IReadOnlyDictionary<string, Identifier> identifierById;
    private readonly IReadOnlyDictionary<string, JoinTableReference> joinTableReferenceByTableReferenceId;
    private readonly IReadOnlyDictionary<string, QualifiedJoin> qualifiedJoinByBaseId;
    private readonly IReadOnlyDictionary<string, UnqualifiedJoin> unqualifiedJoinByBaseId;
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
    private readonly IReadOnlyDictionary<string, BinaryExpression> binaryExpressionByBaseId;
    private readonly IReadOnlyDictionary<string, BinaryExpressionFirstExpressionLink> binaryExpressionFirstExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, BinaryExpressionSecondExpressionLink> binaryExpressionSecondExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, UnaryExpression> unaryExpressionByBaseId;
    private readonly IReadOnlyDictionary<string, UnaryExpressionExpressionLink> unaryExpressionExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, PrimaryExpression> primaryExpressionByScalarExpressionId;
    private readonly IReadOnlyDictionary<string, ColumnReferenceExpression> columnReferenceExpressionByPrimaryExpressionId;
    private readonly IReadOnlyDictionary<string, ColumnReferenceExpressionMultiPartIdentifierLink> columnReferenceExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, ParenthesisExpression> parenthesisExpressionByPrimaryExpressionId;
    private readonly IReadOnlyDictionary<string, ParenthesisExpressionExpressionLink> parenthesisExpressionExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, ScalarSubquery> scalarSubqueryByPrimaryExpressionId;
    private readonly IReadOnlyDictionary<string, ScalarSubqueryQueryExpressionLink> scalarSubqueryQueryExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, FunctionCall> functionCallByPrimaryExpressionId;
    private readonly IReadOnlyDictionary<string, FunctionCallFunctionNameLink> functionCallFunctionNameLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, FunctionCallOverClauseLink> functionCallOverClauseLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, List<FunctionCallParametersItem>> functionCallParametersByOwnerId;
    private readonly IReadOnlyDictionary<string, LeftFunctionCall> leftFunctionCallByPrimaryExpressionId;
    private readonly IReadOnlyDictionary<string, List<LeftFunctionCallParametersItem>> leftFunctionCallParametersByOwnerId;
    private readonly IReadOnlyDictionary<string, RightFunctionCall> rightFunctionCallByPrimaryExpressionId;
    private readonly IReadOnlyDictionary<string, List<RightFunctionCallParametersItem>> rightFunctionCallParametersByOwnerId;
    private readonly IReadOnlyDictionary<string, ParameterlessCall> parameterlessCallByPrimaryExpressionId;
    private readonly IReadOnlyDictionary<string, CoalesceExpression> coalesceExpressionByPrimaryExpressionId;
    private readonly IReadOnlyDictionary<string, List<CoalesceExpressionExpressionsItem>> coalesceExpressionExpressionsByOwnerId;
    private readonly IReadOnlyDictionary<string, NullIfExpression> nullIfExpressionByPrimaryExpressionId;
    private readonly IReadOnlyDictionary<string, NullIfExpressionFirstExpressionLink> nullIfExpressionFirstExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, NullIfExpressionSecondExpressionLink> nullIfExpressionSecondExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, IIfCall> iIfCallByPrimaryExpressionId;
    private readonly IReadOnlyDictionary<string, IIfCallPredicateLink> iIfCallPredicateLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, IIfCallThenExpressionLink> iIfCallThenExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, IIfCallElseExpressionLink> iIfCallElseExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, CaseExpression> caseExpressionByPrimaryExpressionId;
    private readonly IReadOnlyDictionary<string, SearchedCaseExpression> searchedCaseExpressionByCaseExpressionId;
    private readonly IReadOnlyDictionary<string, List<SearchedCaseExpressionWhenClausesItem>> searchedCaseExpressionWhenClausesByOwnerId;
    private readonly IReadOnlyDictionary<string, SearchedWhenClause> searchedWhenClauseById;
    private readonly IReadOnlyDictionary<string, SearchedWhenClauseWhenExpressionLink> searchedWhenClauseWhenExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, CaseExpressionElseExpressionLink> caseExpressionElseExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, WhenClauseThenExpressionLink> whenClauseThenExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, SimpleCaseExpression> simpleCaseExpressionByCaseExpressionId;
    private readonly IReadOnlyDictionary<string, SimpleCaseExpressionInputExpressionLink> simpleCaseExpressionInputExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, List<SimpleCaseExpressionWhenClausesItem>> simpleCaseExpressionWhenClausesByOwnerId;
    private readonly IReadOnlyDictionary<string, SimpleWhenClause> simpleWhenClauseById;
    private readonly IReadOnlyDictionary<string, SimpleWhenClauseWhenExpressionLink> simpleWhenClauseWhenExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, CastCall> castCallByPrimaryExpressionId;
    private readonly IReadOnlyDictionary<string, CastCallParameterLink> castCallParameterLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, ConvertCall> convertCallByPrimaryExpressionId;
    private readonly IReadOnlyDictionary<string, ConvertCallParameterLink> convertCallParameterLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, ConvertCallStyleLink> convertCallStyleLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, TryCastCall> tryCastCallByPrimaryExpressionId;
    private readonly IReadOnlyDictionary<string, TryCastCallParameterLink> tryCastCallParameterLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, TryConvertCall> tryConvertCallByPrimaryExpressionId;
    private readonly IReadOnlyDictionary<string, TryConvertCallParameterLink> tryConvertCallParameterLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, TryConvertCallStyleLink> tryConvertCallStyleLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, ParseCall> parseCallByPrimaryExpressionId;
    private readonly IReadOnlyDictionary<string, ParseCallStringValueLink> parseCallStringValueLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, ParseCallCultureLink> parseCallCultureLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, TryParseCall> tryParseCallByPrimaryExpressionId;
    private readonly IReadOnlyDictionary<string, TryParseCallStringValueLink> tryParseCallStringValueLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, TryParseCallCultureLink> tryParseCallCultureLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, AtTimeZoneCall> atTimeZoneCallByPrimaryExpressionId;
    private readonly IReadOnlyDictionary<string, AtTimeZoneCallDateValueLink> atTimeZoneCallDateValueLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, AtTimeZoneCallTimeZoneLink> atTimeZoneCallTimeZoneLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, BooleanBinaryExpression> booleanBinaryExpressionByBaseId;
    private readonly IReadOnlyDictionary<string, BooleanBinaryExpressionFirstExpressionLink> booleanBinaryExpressionFirstExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, BooleanBinaryExpressionSecondExpressionLink> booleanBinaryExpressionSecondExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, BooleanComparisonExpression> booleanComparisonExpressionByBaseId;
    private readonly IReadOnlyDictionary<string, BooleanComparisonExpressionFirstExpressionLink> booleanComparisonExpressionFirstExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, BooleanComparisonExpressionSecondExpressionLink> booleanComparisonExpressionSecondExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, BooleanNotExpression> booleanNotExpressionByBaseId;
    private readonly IReadOnlyDictionary<string, BooleanNotExpressionExpressionLink> booleanNotExpressionExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, BooleanParenthesisExpression> booleanParenthesisExpressionByBaseId;
    private readonly IReadOnlyDictionary<string, BooleanParenthesisExpressionExpressionLink> booleanParenthesisExpressionExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, ExistsPredicate> existsPredicateByBaseId;
    private readonly IReadOnlyDictionary<string, ExistsPredicateSubqueryLink> existsPredicateSubqueryLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, InPredicate> inPredicateByBaseId;
    private readonly IReadOnlyDictionary<string, InPredicateExpressionLink> inPredicateExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, InPredicateSubqueryLink> inPredicateSubqueryLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, SubqueryComparisonPredicate> subqueryComparisonPredicateByBaseId;
    private readonly IReadOnlyDictionary<string, SubqueryComparisonPredicateExpressionLink> subqueryComparisonPredicateExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, SubqueryComparisonPredicateSubqueryLink> subqueryComparisonPredicateSubqueryLinkByOwnerId;

    public TransformScriptNavigator(MetaTransformScriptModel model)
    {
        this.model = model;
        scriptSelectStatementLinkByOwnerId = model.TransformScriptSelectStatementLinkList.ToDictionary(item => item.TransformScriptId, StringComparer.Ordinal);
        selectStatementById = model.SelectStatementList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        selectStatementQueryExpressionLinkByOwnerId = model.SelectStatementQueryExpressionLinkList.ToDictionary(item => item.SelectStatementId, StringComparer.Ordinal);
        statementWithCtesLinkByOwnerId = model.StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkList.ToDictionary(item => item.StatementWithCtesAndXmlNamespacesId, StringComparer.Ordinal);
        withCtesAndXmlNamespacesById = model.WithCtesAndXmlNamespacesList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        commonTableExpressionsByWithClauseOwnerId = GroupByOwner(model.WithCtesAndXmlNamespacesCommonTableExpressionsItemList);
        commonTableExpressionById = model.CommonTableExpressionList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        commonTableExpressionNameLinkByOwnerId = model.CommonTableExpressionExpressionNameLinkList.ToDictionary(item => item.CommonTableExpressionId, StringComparer.Ordinal);
        commonTableExpressionQueryExpressionLinkByOwnerId = model.CommonTableExpressionQueryExpressionLinkList.ToDictionary(item => item.CommonTableExpressionId, StringComparer.Ordinal);
        commonTableExpressionColumnsByOwnerId = GroupByOwner(model.CommonTableExpressionColumnsItemList);
        querySpecificationByQueryExpressionId = model.QuerySpecificationList.ToDictionary(item => item.QueryExpressionId, StringComparer.Ordinal);
        binaryQueryExpressionByQueryExpressionId = model.BinaryQueryExpressionList.ToDictionary(item => item.QueryExpressionId, StringComparer.Ordinal);
        binaryQueryExpressionFirstQueryExpressionLinkByOwnerId = model.BinaryQueryExpressionFirstQueryExpressionLinkList.ToDictionary(item => item.BinaryQueryExpressionId, StringComparer.Ordinal);
        binaryQueryExpressionSecondQueryExpressionLinkByOwnerId = model.BinaryQueryExpressionSecondQueryExpressionLinkList.ToDictionary(item => item.BinaryQueryExpressionId, StringComparer.Ordinal);
        fromClauseLinkByOwnerId = model.QuerySpecificationFromClauseLinkList.ToDictionary(item => item.QuerySpecificationId, StringComparer.Ordinal);
        fromClauseById = model.FromClauseList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        fromClauseTableReferencesByOwnerId = GroupByOwner(model.FromClauseTableReferencesItemList);
        groupByClauseLinkByOwnerId = model.QuerySpecificationGroupByClauseLinkList.ToDictionary(item => item.QuerySpecificationId, StringComparer.Ordinal);
        groupByClauseById = model.GroupByClauseList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        groupingSpecificationsByGroupByClauseOwnerId = GroupByOwner(model.GroupByClauseGroupingSpecificationsItemList);
        groupingSpecificationById = model.GroupingSpecificationList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        expressionGroupingSpecificationByBaseId = model.ExpressionGroupingSpecificationList.ToDictionary(item => item.GroupingSpecificationId, StringComparer.Ordinal);
        expressionGroupingSpecificationExpressionLinkByOwnerId = model.ExpressionGroupingSpecificationExpressionLinkList.ToDictionary(item => item.ExpressionGroupingSpecificationId, StringComparer.Ordinal);
        whereClauseLinkByOwnerId = model.QuerySpecificationWhereClauseLinkList.ToDictionary(item => item.QuerySpecificationId, StringComparer.Ordinal);
        whereClauseById = model.WhereClauseList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        whereClauseSearchConditionLinkByOwnerId = model.WhereClauseSearchConditionLinkList.ToDictionary(item => item.WhereClauseId, StringComparer.Ordinal);
        havingClauseLinkByOwnerId = model.QuerySpecificationHavingClauseLinkList.ToDictionary(item => item.QuerySpecificationId, StringComparer.Ordinal);
        havingClauseById = model.HavingClauseList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        havingClauseSearchConditionLinkByOwnerId = model.HavingClauseSearchConditionLinkList.ToDictionary(item => item.HavingClauseId, StringComparer.Ordinal);
        tableReferenceById = model.TableReferenceList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        tableReferenceWithAliasByTableReferenceId = model.TableReferenceWithAliasList.ToDictionary(item => item.TableReferenceId, StringComparer.Ordinal);
        tableReferenceAliasLinkByOwnerId = model.TableReferenceWithAliasAliasLinkList.ToDictionary(item => item.TableReferenceWithAliasId, StringComparer.Ordinal);
        tableReferenceWithAliasAndColumnsByBaseId = model.TableReferenceWithAliasAndColumnsList.ToDictionary(item => item.TableReferenceWithAliasId, StringComparer.Ordinal);
        tableReferenceWithAliasAndColumnsColumnsByOwnerId = GroupByOwner(model.TableReferenceWithAliasAndColumnsColumnsItemList);
        namedTableReferenceByAliasBaseId = model.NamedTableReferenceList.ToDictionary(item => item.TableReferenceWithAliasId, StringComparer.Ordinal);
        queryDerivedTableByBaseId = model.QueryDerivedTableList.ToDictionary(item => item.TableReferenceWithAliasAndColumnsId, StringComparer.Ordinal);
        queryDerivedTableQueryExpressionLinkByOwnerId = model.QueryDerivedTableQueryExpressionLinkList.ToDictionary(item => item.QueryDerivedTableId, StringComparer.Ordinal);
        inlineDerivedTableByBaseId = model.InlineDerivedTableList.ToDictionary(item => item.TableReferenceWithAliasAndColumnsId, StringComparer.Ordinal);
        inlineDerivedTableRowValuesByOwnerId = GroupByOwner(model.InlineDerivedTableRowValuesItemList);
        rowValueById = model.RowValueList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        rowValueColumnValuesByOwnerId = GroupByOwner(model.RowValueColumnValuesItemList);
        schemaObjectFunctionTableReferenceByBaseId = model.SchemaObjectFunctionTableReferenceList.ToDictionary(item => item.TableReferenceWithAliasAndColumnsId, StringComparer.Ordinal);
        schemaObjectFunctionTableReferenceSchemaObjectLinkByOwnerId = model.SchemaObjectFunctionTableReferenceSchemaObjectLinkList.ToDictionary(item => item.SchemaObjectFunctionTableReferenceId, StringComparer.Ordinal);
        schemaObjectFunctionTableReferenceParametersByOwnerId = GroupByOwner(model.SchemaObjectFunctionTableReferenceParametersItemList);
        namedTableReferenceSchemaObjectLinkByOwnerId = model.NamedTableReferenceSchemaObjectLinkList.ToDictionary(item => item.NamedTableReferenceId, StringComparer.Ordinal);
        schemaObjectNameById = model.SchemaObjectNameList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        multiPartIdentifierById = model.MultiPartIdentifierList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        multiPartIdentifierItemsByOwnerId = GroupByOwner(model.MultiPartIdentifierIdentifiersItemList);
        identifierById = model.IdentifierList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        joinTableReferenceByTableReferenceId = model.JoinTableReferenceList.ToDictionary(item => item.TableReferenceId, StringComparer.Ordinal);
        qualifiedJoinByBaseId = model.QualifiedJoinList.ToDictionary(item => item.JoinTableReferenceId, StringComparer.Ordinal);
        unqualifiedJoinByBaseId = model.UnqualifiedJoinList.ToDictionary(item => item.JoinTableReferenceId, StringComparer.Ordinal);
        joinFirstTableReferenceLinkByOwnerId = model.JoinTableReferenceFirstTableReferenceLinkList.ToDictionary(item => item.JoinTableReferenceId, StringComparer.Ordinal);
        joinSecondTableReferenceLinkByOwnerId = model.JoinTableReferenceSecondTableReferenceLinkList.ToDictionary(item => item.JoinTableReferenceId, StringComparer.Ordinal);
        selectElementsByOwnerId = GroupByOwner(model.QuerySpecificationSelectElementsItemList);
        selectElementById = model.SelectElementList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        selectScalarExpressionBySelectElementId = model.SelectScalarExpressionList.ToDictionary(item => item.SelectElementId, StringComparer.Ordinal);
        selectScalarExpressionLinkByOwnerId = model.SelectScalarExpressionExpressionLinkList.ToDictionary(item => item.SelectScalarExpressionId, StringComparer.Ordinal);
        selectScalarExpressionColumnNameLinkByOwnerId = model.SelectScalarExpressionColumnNameLinkList.ToDictionary(item => item.SelectScalarExpressionId, StringComparer.Ordinal);
        identifierOrValueExpressionById = model.IdentifierOrValueExpressionList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        identifierOrValueExpressionIdentifierLinkByOwnerId = model.IdentifierOrValueExpressionIdentifierLinkList.ToDictionary(item => item.IdentifierOrValueExpressionId, StringComparer.Ordinal);
        selectStarExpressionBySelectElementId = model.SelectStarExpressionList.ToDictionary(item => item.SelectElementId, StringComparer.Ordinal);
        selectStarQualifierLinkByOwnerId = model.SelectStarExpressionQualifierLinkList.ToDictionary(item => item.SelectStarExpressionId, StringComparer.Ordinal);
        scalarExpressionById = model.ScalarExpressionList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        binaryExpressionByBaseId = model.BinaryExpressionList.ToDictionary(item => item.ScalarExpressionId, StringComparer.Ordinal);
        binaryExpressionFirstExpressionLinkByOwnerId = model.BinaryExpressionFirstExpressionLinkList.ToDictionary(item => item.BinaryExpressionId, StringComparer.Ordinal);
        binaryExpressionSecondExpressionLinkByOwnerId = model.BinaryExpressionSecondExpressionLinkList.ToDictionary(item => item.BinaryExpressionId, StringComparer.Ordinal);
        unaryExpressionByBaseId = model.UnaryExpressionList.ToDictionary(item => item.ScalarExpressionId, StringComparer.Ordinal);
        unaryExpressionExpressionLinkByOwnerId = model.UnaryExpressionExpressionLinkList.ToDictionary(item => item.UnaryExpressionId, StringComparer.Ordinal);
        primaryExpressionByScalarExpressionId = model.PrimaryExpressionList.ToDictionary(item => item.ScalarExpressionId, StringComparer.Ordinal);
        columnReferenceExpressionByPrimaryExpressionId = model.ColumnReferenceExpressionList.ToDictionary(item => item.PrimaryExpressionId, StringComparer.Ordinal);
        columnReferenceExpressionLinkByOwnerId = model.ColumnReferenceExpressionMultiPartIdentifierLinkList.ToDictionary(item => item.ColumnReferenceExpressionId, StringComparer.Ordinal);
        parenthesisExpressionByPrimaryExpressionId = model.ParenthesisExpressionList.ToDictionary(item => item.PrimaryExpressionId, StringComparer.Ordinal);
        parenthesisExpressionExpressionLinkByOwnerId = model.ParenthesisExpressionExpressionLinkList.ToDictionary(item => item.ParenthesisExpressionId, StringComparer.Ordinal);
        scalarSubqueryByPrimaryExpressionId = model.ScalarSubqueryList.ToDictionary(item => item.PrimaryExpressionId, StringComparer.Ordinal);
        scalarSubqueryQueryExpressionLinkByOwnerId = model.ScalarSubqueryQueryExpressionLinkList.ToDictionary(item => item.ScalarSubqueryId, StringComparer.Ordinal);
        functionCallByPrimaryExpressionId = model.FunctionCallList.ToDictionary(item => item.PrimaryExpressionId, StringComparer.Ordinal);
        functionCallFunctionNameLinkByOwnerId = model.FunctionCallFunctionNameLinkList.ToDictionary(item => item.FunctionCallId, StringComparer.Ordinal);
        functionCallOverClauseLinkByOwnerId = model.FunctionCallOverClauseLinkList.ToDictionary(item => item.FunctionCallId, StringComparer.Ordinal);
        functionCallParametersByOwnerId = GroupByOwner(model.FunctionCallParametersItemList);
        leftFunctionCallByPrimaryExpressionId = model.LeftFunctionCallList.ToDictionary(item => item.PrimaryExpressionId, StringComparer.Ordinal);
        leftFunctionCallParametersByOwnerId = GroupByOwner(model.LeftFunctionCallParametersItemList);
        rightFunctionCallByPrimaryExpressionId = model.RightFunctionCallList.ToDictionary(item => item.PrimaryExpressionId, StringComparer.Ordinal);
        rightFunctionCallParametersByOwnerId = GroupByOwner(model.RightFunctionCallParametersItemList);
        parameterlessCallByPrimaryExpressionId = model.ParameterlessCallList.ToDictionary(item => item.PrimaryExpressionId, StringComparer.Ordinal);
        coalesceExpressionByPrimaryExpressionId = model.CoalesceExpressionList.ToDictionary(item => item.PrimaryExpressionId, StringComparer.Ordinal);
        coalesceExpressionExpressionsByOwnerId = GroupByOwner(model.CoalesceExpressionExpressionsItemList);
        nullIfExpressionByPrimaryExpressionId = model.NullIfExpressionList.ToDictionary(item => item.PrimaryExpressionId, StringComparer.Ordinal);
        nullIfExpressionFirstExpressionLinkByOwnerId = model.NullIfExpressionFirstExpressionLinkList.ToDictionary(item => item.NullIfExpressionId, StringComparer.Ordinal);
        nullIfExpressionSecondExpressionLinkByOwnerId = model.NullIfExpressionSecondExpressionLinkList.ToDictionary(item => item.NullIfExpressionId, StringComparer.Ordinal);
        iIfCallByPrimaryExpressionId = model.IIfCallList.ToDictionary(item => item.PrimaryExpressionId, StringComparer.Ordinal);
        iIfCallPredicateLinkByOwnerId = model.IIfCallPredicateLinkList.ToDictionary(item => item.IIfCallId, StringComparer.Ordinal);
        iIfCallThenExpressionLinkByOwnerId = model.IIfCallThenExpressionLinkList.ToDictionary(item => item.IIfCallId, StringComparer.Ordinal);
        iIfCallElseExpressionLinkByOwnerId = model.IIfCallElseExpressionLinkList.ToDictionary(item => item.IIfCallId, StringComparer.Ordinal);
        caseExpressionByPrimaryExpressionId = model.CaseExpressionList.ToDictionary(item => item.PrimaryExpressionId, StringComparer.Ordinal);
        searchedCaseExpressionByCaseExpressionId = model.SearchedCaseExpressionList.ToDictionary(item => item.CaseExpressionId, StringComparer.Ordinal);
        searchedCaseExpressionWhenClausesByOwnerId = GroupByOwner(model.SearchedCaseExpressionWhenClausesItemList);
        searchedWhenClauseById = model.SearchedWhenClauseList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        searchedWhenClauseWhenExpressionLinkByOwnerId = model.SearchedWhenClauseWhenExpressionLinkList.ToDictionary(item => item.SearchedWhenClauseId, StringComparer.Ordinal);
        caseExpressionElseExpressionLinkByOwnerId = model.CaseExpressionElseExpressionLinkList.ToDictionary(item => item.CaseExpressionId, StringComparer.Ordinal);
        whenClauseThenExpressionLinkByOwnerId = model.WhenClauseThenExpressionLinkList.ToDictionary(item => item.WhenClauseId, StringComparer.Ordinal);
        simpleCaseExpressionByCaseExpressionId = model.SimpleCaseExpressionList.ToDictionary(item => item.CaseExpressionId, StringComparer.Ordinal);
        simpleCaseExpressionInputExpressionLinkByOwnerId = model.SimpleCaseExpressionInputExpressionLinkList.ToDictionary(item => item.SimpleCaseExpressionId, StringComparer.Ordinal);
        simpleCaseExpressionWhenClausesByOwnerId = GroupByOwner(model.SimpleCaseExpressionWhenClausesItemList);
        simpleWhenClauseById = model.SimpleWhenClauseList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        simpleWhenClauseWhenExpressionLinkByOwnerId = model.SimpleWhenClauseWhenExpressionLinkList.ToDictionary(item => item.SimpleWhenClauseId, StringComparer.Ordinal);
        castCallByPrimaryExpressionId = model.CastCallList.ToDictionary(item => item.PrimaryExpressionId, StringComparer.Ordinal);
        castCallParameterLinkByOwnerId = model.CastCallParameterLinkList.ToDictionary(item => item.CastCallId, StringComparer.Ordinal);
        convertCallByPrimaryExpressionId = model.ConvertCallList.ToDictionary(item => item.PrimaryExpressionId, StringComparer.Ordinal);
        convertCallParameterLinkByOwnerId = model.ConvertCallParameterLinkList.ToDictionary(item => item.ConvertCallId, StringComparer.Ordinal);
        convertCallStyleLinkByOwnerId = model.ConvertCallStyleLinkList.ToDictionary(item => item.ConvertCallId, StringComparer.Ordinal);
        tryCastCallByPrimaryExpressionId = model.TryCastCallList.ToDictionary(item => item.PrimaryExpressionId, StringComparer.Ordinal);
        tryCastCallParameterLinkByOwnerId = model.TryCastCallParameterLinkList.ToDictionary(item => item.TryCastCallId, StringComparer.Ordinal);
        tryConvertCallByPrimaryExpressionId = model.TryConvertCallList.ToDictionary(item => item.PrimaryExpressionId, StringComparer.Ordinal);
        tryConvertCallParameterLinkByOwnerId = model.TryConvertCallParameterLinkList.ToDictionary(item => item.TryConvertCallId, StringComparer.Ordinal);
        tryConvertCallStyleLinkByOwnerId = model.TryConvertCallStyleLinkList.ToDictionary(item => item.TryConvertCallId, StringComparer.Ordinal);
        parseCallByPrimaryExpressionId = model.ParseCallList.ToDictionary(item => item.PrimaryExpressionId, StringComparer.Ordinal);
        parseCallStringValueLinkByOwnerId = model.ParseCallStringValueLinkList.ToDictionary(item => item.ParseCallId, StringComparer.Ordinal);
        parseCallCultureLinkByOwnerId = model.ParseCallCultureLinkList.ToDictionary(item => item.ParseCallId, StringComparer.Ordinal);
        tryParseCallByPrimaryExpressionId = model.TryParseCallList.ToDictionary(item => item.PrimaryExpressionId, StringComparer.Ordinal);
        tryParseCallStringValueLinkByOwnerId = model.TryParseCallStringValueLinkList.ToDictionary(item => item.TryParseCallId, StringComparer.Ordinal);
        tryParseCallCultureLinkByOwnerId = model.TryParseCallCultureLinkList.ToDictionary(item => item.TryParseCallId, StringComparer.Ordinal);
        atTimeZoneCallByPrimaryExpressionId = model.AtTimeZoneCallList.ToDictionary(item => item.PrimaryExpressionId, StringComparer.Ordinal);
        atTimeZoneCallDateValueLinkByOwnerId = model.AtTimeZoneCallDateValueLinkList.ToDictionary(item => item.AtTimeZoneCallId, StringComparer.Ordinal);
        atTimeZoneCallTimeZoneLinkByOwnerId = model.AtTimeZoneCallTimeZoneLinkList.ToDictionary(item => item.AtTimeZoneCallId, StringComparer.Ordinal);
        booleanBinaryExpressionByBaseId = model.BooleanBinaryExpressionList.ToDictionary(item => item.BooleanExpressionId, StringComparer.Ordinal);
        booleanBinaryExpressionFirstExpressionLinkByOwnerId = model.BooleanBinaryExpressionFirstExpressionLinkList.ToDictionary(item => item.BooleanBinaryExpressionId, StringComparer.Ordinal);
        booleanBinaryExpressionSecondExpressionLinkByOwnerId = model.BooleanBinaryExpressionSecondExpressionLinkList.ToDictionary(item => item.BooleanBinaryExpressionId, StringComparer.Ordinal);
        booleanComparisonExpressionByBaseId = model.BooleanComparisonExpressionList.ToDictionary(item => item.BooleanExpressionId, StringComparer.Ordinal);
        booleanComparisonExpressionFirstExpressionLinkByOwnerId = model.BooleanComparisonExpressionFirstExpressionLinkList.ToDictionary(item => item.BooleanComparisonExpressionId, StringComparer.Ordinal);
        booleanComparisonExpressionSecondExpressionLinkByOwnerId = model.BooleanComparisonExpressionSecondExpressionLinkList.ToDictionary(item => item.BooleanComparisonExpressionId, StringComparer.Ordinal);
        booleanNotExpressionByBaseId = model.BooleanNotExpressionList.ToDictionary(item => item.BooleanExpressionId, StringComparer.Ordinal);
        booleanNotExpressionExpressionLinkByOwnerId = model.BooleanNotExpressionExpressionLinkList.ToDictionary(item => item.BooleanNotExpressionId, StringComparer.Ordinal);
        booleanParenthesisExpressionByBaseId = model.BooleanParenthesisExpressionList.ToDictionary(item => item.BooleanExpressionId, StringComparer.Ordinal);
        booleanParenthesisExpressionExpressionLinkByOwnerId = model.BooleanParenthesisExpressionExpressionLinkList.ToDictionary(item => item.BooleanParenthesisExpressionId, StringComparer.Ordinal);
        existsPredicateByBaseId = model.ExistsPredicateList.ToDictionary(item => item.BooleanExpressionId, StringComparer.Ordinal);
        existsPredicateSubqueryLinkByOwnerId = model.ExistsPredicateSubqueryLinkList.ToDictionary(item => item.ExistsPredicateId, StringComparer.Ordinal);
        inPredicateByBaseId = model.InPredicateList.ToDictionary(item => item.BooleanExpressionId, StringComparer.Ordinal);
        inPredicateExpressionLinkByOwnerId = model.InPredicateExpressionLinkList.ToDictionary(item => item.InPredicateId, StringComparer.Ordinal);
        inPredicateSubqueryLinkByOwnerId = model.InPredicateSubqueryLinkList.ToDictionary(item => item.InPredicateId, StringComparer.Ordinal);
        subqueryComparisonPredicateByBaseId = model.SubqueryComparisonPredicateList.ToDictionary(item => item.BooleanExpressionId, StringComparer.Ordinal);
        subqueryComparisonPredicateExpressionLinkByOwnerId = model.SubqueryComparisonPredicateExpressionLinkList.ToDictionary(item => item.SubqueryComparisonPredicateId, StringComparer.Ordinal);
        subqueryComparisonPredicateSubqueryLinkByOwnerId = model.SubqueryComparisonPredicateSubqueryLinkList.ToDictionary(item => item.SubqueryComparisonPredicateId, StringComparer.Ordinal);
    }

    public SelectStatement? TryGetSelectStatement(TransformScript script)
    {
        if (!scriptSelectStatementLinkByOwnerId.TryGetValue(script.Id, out var link))
        {
            return null;
        }

        return selectStatementById.GetValueOrDefault(link.SelectStatementId);
    }

    public string? TryGetSelectStatementQueryExpressionId(SelectStatement selectStatement)
    {
        return selectStatementQueryExpressionLinkByOwnerId.TryGetValue(selectStatement.Id, out var link)
            ? link.QueryExpressionId
            : null;
    }

    public QuerySpecification? TryGetQuerySpecification(SelectStatement selectStatement)
    {
        if (!selectStatementQueryExpressionLinkByOwnerId.TryGetValue(selectStatement.Id, out var link))
        {
            return null;
        }

        return querySpecificationByQueryExpressionId.GetValueOrDefault(link.QueryExpressionId);
    }

    public IReadOnlyList<CommonTableExpression> GetCommonTableExpressions(SelectStatement selectStatement)
    {
        if (!statementWithCtesLinkByOwnerId.TryGetValue(selectStatement.StatementWithCtesAndXmlNamespacesId, out var withCtesLink))
        {
            return [];
        }

        if (!withCtesAndXmlNamespacesById.TryGetValue(withCtesLink.WithCtesAndXmlNamespacesId, out var withCtes))
        {
            return [];
        }

        if (!commonTableExpressionsByWithClauseOwnerId.TryGetValue(withCtes.Id, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => commonTableExpressionById.GetValueOrDefault(item.CommonTableExpressionId))
            .Where(item => item is not null)
            .Cast<CommonTableExpression>()
            .ToArray();
    }

    public string? TryGetCommonTableExpressionName(CommonTableExpression commonTableExpression)
    {
        return commonTableExpressionNameLinkByOwnerId.TryGetValue(commonTableExpression.Id, out var link)
            ? identifierById.GetValueOrDefault(link.IdentifierId)?.Value
            : null;
    }

    public string? TryGetCommonTableExpressionQueryExpressionId(CommonTableExpression commonTableExpression)
    {
        return commonTableExpressionQueryExpressionLinkByOwnerId.TryGetValue(commonTableExpression.Id, out var link)
            ? link.QueryExpressionId
            : null;
    }

    public IReadOnlyList<string> GetCommonTableExpressionColumnAliases(CommonTableExpression commonTableExpression)
    {
        if (!commonTableExpressionColumnsByOwnerId.TryGetValue(commonTableExpression.Id, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => identifierById.GetValueOrDefault(item.IdentifierId)?.Value)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Cast<string>()
            .ToArray();
    }

    public QuerySpecification? TryGetQuerySpecification(string queryExpressionId) =>
        querySpecificationByQueryExpressionId.GetValueOrDefault(queryExpressionId);

    public BinaryQueryExpression? TryGetBinaryQueryExpression(string queryExpressionId) =>
        binaryQueryExpressionByQueryExpressionId.GetValueOrDefault(queryExpressionId);

    public (string FirstQueryExpressionId, string SecondQueryExpressionId)? TryGetBinaryQueryExpressionChildren(BinaryQueryExpression binaryQueryExpression)
    {
        if (!binaryQueryExpressionFirstQueryExpressionLinkByOwnerId.TryGetValue(binaryQueryExpression.Id, out var firstLink) ||
            !binaryQueryExpressionSecondQueryExpressionLinkByOwnerId.TryGetValue(binaryQueryExpression.Id, out var secondLink))
        {
            return null;
        }

        return (firstLink.QueryExpressionId, secondLink.QueryExpressionId);
    }

    public FromClause? TryGetFromClause(QuerySpecification querySpecification)
    {
        if (!fromClauseLinkByOwnerId.TryGetValue(querySpecification.Id, out var link))
        {
            return null;
        }

        return fromClauseById.GetValueOrDefault(link.FromClauseId);
    }

    public IReadOnlyList<TableReference> GetTableReferences(FromClause fromClause)
    {
        if (!fromClauseTableReferencesByOwnerId.TryGetValue(fromClause.Id, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => tableReferenceById.GetValueOrDefault(item.TableReferenceId))
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

    public QueryDerivedTable? TryGetQueryDerivedTable(TableReference tableReference)
    {
        if (!tableReferenceWithAliasByTableReferenceId.TryGetValue(tableReference.Id, out var aliasBase))
        {
            return null;
        }

        if (!tableReferenceWithAliasAndColumnsByBaseId.TryGetValue(aliasBase.Id, out var aliasAndColumnsBase))
        {
            return null;
        }

        return queryDerivedTableByBaseId.GetValueOrDefault(aliasAndColumnsBase.Id);
    }

    public SchemaObjectFunctionTableReference? TryGetSchemaObjectFunctionTableReference(TableReference tableReference)
    {
        if (!tableReferenceWithAliasByTableReferenceId.TryGetValue(tableReference.Id, out var aliasBase))
        {
            return null;
        }

        if (!tableReferenceWithAliasAndColumnsByBaseId.TryGetValue(aliasBase.Id, out var aliasAndColumnsBase))
        {
            return null;
        }

        return schemaObjectFunctionTableReferenceByBaseId.GetValueOrDefault(aliasAndColumnsBase.Id);
    }

    public InlineDerivedTable? TryGetInlineDerivedTable(TableReference tableReference)
    {
        if (!tableReferenceWithAliasByTableReferenceId.TryGetValue(tableReference.Id, out var aliasBase))
        {
            return null;
        }

        if (!tableReferenceWithAliasAndColumnsByBaseId.TryGetValue(aliasBase.Id, out var aliasAndColumnsBase))
        {
            return null;
        }

        return inlineDerivedTableByBaseId.GetValueOrDefault(aliasAndColumnsBase.Id);
    }

    public IReadOnlyList<RowValue> GetInlineDerivedTableRowValues(InlineDerivedTable inlineDerivedTable)
    {
        if (!inlineDerivedTableRowValuesByOwnerId.TryGetValue(inlineDerivedTable.Id, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => rowValueById.GetValueOrDefault(item.RowValueId))
            .Where(item => item is not null)
            .Cast<RowValue>()
            .ToArray();
    }

    public IReadOnlyList<ScalarExpression> GetRowValueColumnValues(RowValue rowValue)
    {
        if (!rowValueColumnValuesByOwnerId.TryGetValue(rowValue.Id, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => scalarExpressionById.GetValueOrDefault(item.ScalarExpressionId))
            .Where(item => item is not null)
            .Cast<ScalarExpression>()
            .ToArray();
    }

    public IReadOnlyList<string> GetSchemaObjectFunctionTableReferenceNameParts(SchemaObjectFunctionTableReference functionTableReference)
    {
        if (!schemaObjectFunctionTableReferenceSchemaObjectLinkByOwnerId.TryGetValue(functionTableReference.Id, out var link))
        {
            return [];
        }

        if (!schemaObjectNameById.TryGetValue(link.SchemaObjectNameId, out var schemaObjectName))
        {
            return [];
        }

        return GetMultiPartIdentifierParts(schemaObjectName.MultiPartIdentifierId);
    }

    public IReadOnlyList<ScalarExpression> GetSchemaObjectFunctionTableReferenceParameters(SchemaObjectFunctionTableReference functionTableReference)
    {
        if (!schemaObjectFunctionTableReferenceParametersByOwnerId.TryGetValue(functionTableReference.Id, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => scalarExpressionById.GetValueOrDefault(item.ScalarExpressionId))
            .Where(item => item is not null)
            .Cast<ScalarExpression>()
            .ToArray();
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
            tableReferenceById.GetValueOrDefault(firstLink.TableReferenceId),
            tableReferenceById.GetValueOrDefault(secondLink.TableReferenceId));
    }

    public string? TryGetJoinOperator(TableReference tableReference)
    {
        if (!joinTableReferenceByTableReferenceId.TryGetValue(tableReference.Id, out var joinBase))
        {
            return null;
        }

        if (unqualifiedJoinByBaseId.TryGetValue(joinBase.Id, out var unqualifiedJoin))
        {
            return string.IsNullOrWhiteSpace(unqualifiedJoin.UnqualifiedJoinType)
                ? null
                : unqualifiedJoin.UnqualifiedJoinType;
        }

        if (qualifiedJoinByBaseId.TryGetValue(joinBase.Id, out var qualifiedJoin))
        {
            return string.IsNullOrWhiteSpace(qualifiedJoin.QualifiedJoinType)
                ? null
                : qualifiedJoin.QualifiedJoinType;
        }

        return null;
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

        return identifierById.GetValueOrDefault(link.IdentifierId)?.Value;
    }

    public IReadOnlyList<string> GetTableReferenceColumnAliases(TableReference tableReference)
    {
        if (!tableReferenceWithAliasByTableReferenceId.TryGetValue(tableReference.Id, out var aliasBase))
        {
            return [];
        }

        if (!tableReferenceWithAliasAndColumnsByBaseId.TryGetValue(aliasBase.Id, out var aliasAndColumnsBase))
        {
            return [];
        }

        if (!tableReferenceWithAliasAndColumnsColumnsByOwnerId.TryGetValue(aliasAndColumnsBase.Id, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => identifierById.GetValueOrDefault(item.IdentifierId)?.Value)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Cast<string>()
            .ToArray();
    }

    public string? TryGetQueryDerivedTableQueryExpressionId(QueryDerivedTable queryDerivedTable)
    {
        return queryDerivedTableQueryExpressionLinkByOwnerId.TryGetValue(queryDerivedTable.Id, out var link)
            ? link.QueryExpressionId
            : null;
    }

    public IReadOnlyList<string> GetNamedTableReferenceParts(NamedTableReference namedTableReference)
    {
        if (!namedTableReferenceSchemaObjectLinkByOwnerId.TryGetValue(namedTableReference.Id, out var link))
        {
            return [];
        }

        if (!schemaObjectNameById.TryGetValue(link.SchemaObjectNameId, out var schemaObjectName))
        {
            return [];
        }

        return GetMultiPartIdentifierParts(schemaObjectName.MultiPartIdentifierId);
    }

    public IReadOnlyList<SelectElement> GetSelectElements(QuerySpecification querySpecification)
    {
        if (!selectElementsByOwnerId.TryGetValue(querySpecification.Id, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => selectElementById.GetValueOrDefault(item.SelectElementId))
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

        return scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId);
    }

    public string? TryGetSelectScalarExpressionAlias(SelectScalarExpression selectScalarExpression)
    {
        if (!selectScalarExpressionColumnNameLinkByOwnerId.TryGetValue(selectScalarExpression.Id, out var link))
        {
            return null;
        }

        if (!identifierOrValueExpressionById.TryGetValue(link.IdentifierOrValueExpressionId, out var aliasValue))
        {
            return null;
        }

        if (identifierOrValueExpressionIdentifierLinkByOwnerId.TryGetValue(aliasValue.Id, out var identifierLink))
        {
            return identifierById.GetValueOrDefault(identifierLink.IdentifierId)?.Value;
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

        return GetMultiPartIdentifierParts(link.MultiPartIdentifierId);
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

        return GetMultiPartIdentifierParts(link.MultiPartIdentifierId);
    }

    private IReadOnlyList<string> GetMultiPartIdentifierParts(string multiPartIdentifierId)
    {
        if (!multiPartIdentifierItemsByOwnerId.TryGetValue(multiPartIdentifierId, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => identifierById.GetValueOrDefault(item.IdentifierId)?.Value)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Cast<string>()
            .ToArray();
    }

    private static IReadOnlyDictionary<string, List<T>> GroupByOwner<T>(IEnumerable<T> rows)
    {
        var ownerPropertyName = ResolveOwnerIdProperty(typeof(T));
        if (string.IsNullOrWhiteSpace(ownerPropertyName))
        {
            return new Dictionary<string, List<T>>(StringComparer.Ordinal);
        }

        return rows
            .GroupBy(item => (string?)item!.GetType().GetProperty(ownerPropertyName)?.GetValue(item) ?? string.Empty, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.ToList(), StringComparer.Ordinal);
    }

    private static int ParseOrdinal(string ordinal)
    {
        return int.TryParse(ordinal, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : int.MaxValue;
    }

    private static string? ResolveOwnerIdProperty(Type type)
    {
        return OwnerIdPropertyByType.GetOrAdd(type, static key =>
        {
            var idProperties = key.GetProperties()
                .Select(property => property.Name)
                .Where(static name => name.EndsWith("Id", StringComparison.Ordinal) && !string.Equals(name, "Id", StringComparison.Ordinal))
                .ToArray();

            if (idProperties.Length == 0)
            {
                return null;
            }

            if (idProperties.Length == 1)
            {
                return idProperties[0];
            }

            foreach (var candidate in idProperties.OrderByDescending(static value => value.Length))
            {
                var candidateTypeName = candidate[..^2];
                if (key.Name.StartsWith(candidateTypeName, StringComparison.Ordinal))
                {
                    return candidate;
                }
            }

            return idProperties[0];
        });
    }
}
