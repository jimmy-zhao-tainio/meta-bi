using System.Collections.Concurrent;
using System.Globalization;
using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformScriptNavigator
{
    private static readonly ConcurrentDictionary<Type, string?> OwnerPropertyByType = new();

    private readonly MetaTransformScriptModel model;
    private readonly IReadOnlyDictionary<string, TransformScriptStatementLink> scriptStatementLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, List<TransformScriptFunctionParametersItem>> scriptFunctionParametersByOwnerId;
    private readonly IReadOnlyDictionary<string, ScriptObjectView> scriptObjectViewByOwnerId;
    private readonly IReadOnlyDictionary<string, ScriptObjectTVF> scriptObjectTvfByOwnerId;
    private readonly IReadOnlyDictionary<string, ScriptObjectScalarFunction> scriptObjectScalarFunctionByOwnerId;
    private readonly IReadOnlyDictionary<string, ScriptObjectStoredProcedure> scriptObjectStoredProcedureByOwnerId;
    private readonly IReadOnlyDictionary<string, List<StoredProcedureContract>> storedProcedureContractsByOwnerId;
    private readonly IReadOnlyDictionary<string, List<StoredProcedureContractOperation>> storedProcedureOperationsByContractId;
    private readonly IReadOnlyDictionary<string, List<StoredProcedureResultRowsetItem>> storedProcedureResultRowsetsByOwnerId;
    private readonly IReadOnlyDictionary<string, List<StoredProcedureResultColumnItem>> storedProcedureResultColumnsByOwnerId;
    private readonly IReadOnlyDictionary<string, SelectStatement> selectStatementById;
    private readonly IReadOnlyDictionary<string, SelectStatement> selectStatementByStatementWithCtesId;
    private readonly IReadOnlyDictionary<string, SelectStatementQueryExpressionLink> selectStatementQueryExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, StatementWithCtesAndXmlNamespaces> statementWithCtesBySqlStatementId;
    private readonly IReadOnlyDictionary<string, InsertStatement> insertStatementByStatementWithCtesId;
    private readonly IReadOnlyDictionary<string, InsertStatementTargetLink> insertStatementTargetLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, InsertStatementSourceLink> insertStatementSourceLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, InsertQuerySource> insertQuerySourceByInsertSourceId;
    private readonly IReadOnlyDictionary<string, InsertQuerySourceQueryExpressionLink> insertQuerySourceQueryExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, UpdateStatement> updateStatementByStatementWithCtesId;
    private readonly IReadOnlyDictionary<string, UpdateStatementTargetLink> updateStatementTargetLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, UpdateStatementFromClauseLink> updateStatementFromClauseLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, DeleteStatement> deleteStatementByStatementWithCtesId;
    private readonly IReadOnlyDictionary<string, DeleteStatementTargetLink> deleteStatementTargetLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, DeleteStatementFromClauseLink> deleteStatementFromClauseLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, MergeStatement> mergeStatementByStatementWithCtesId;
    private readonly IReadOnlyDictionary<string, MergeStatementTargetLink> mergeStatementTargetLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, MergeStatementSourceLink> mergeStatementSourceLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, TruncateStatement> truncateStatementBySqlStatementId;
    private readonly IReadOnlyDictionary<string, TruncateStatementTargetLink> truncateStatementTargetLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLink> statementWithCtesLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, WithCtesAndXmlNamespaces> withCtesAndXmlNamespacesById;
    private readonly IReadOnlyDictionary<string, List<WithCtesAndXmlNamespacesCommonTableExpressionsItem>> commonTableExpressionsByWithClauseOwnerId;
    private readonly IReadOnlyDictionary<string, CommonTableExpression> commonTableExpressionById;
    private readonly IReadOnlyDictionary<string, CommonTableExpressionExpressionNameLink> commonTableExpressionNameLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, CommonTableExpressionQueryExpressionLink> commonTableExpressionQueryExpressionLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, List<CommonTableExpressionColumnsItem>> commonTableExpressionColumnsByOwnerId;
    private readonly IReadOnlyDictionary<string, QuerySpecification> querySpecificationByQueryExpressionId;
    private readonly IReadOnlyDictionary<string, BinaryQueryExpression> binaryQueryExpressionByQueryExpressionId;
    private readonly IReadOnlyDictionary<string, QueryParenthesisExpression> queryParenthesisExpressionByQueryExpressionId;
    private readonly IReadOnlyDictionary<string, QueryParenthesisExpressionQueryExpressionLink> queryParenthesisExpressionQueryExpressionLinkByOwnerId;
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
    private readonly IReadOnlyDictionary<string, GlobalFunctionTableReference> globalFunctionTableReferenceByBaseId;
    private readonly IReadOnlyDictionary<string, GlobalFunctionTableReferenceNameLink> globalFunctionTableReferenceNameLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, List<GlobalFunctionTableReferenceParametersItem>> globalFunctionTableReferenceParametersByOwnerId;
    private readonly IReadOnlyDictionary<string, SchemaObjectFunctionTableReference> schemaObjectFunctionTableReferenceByBaseId;
    private readonly IReadOnlyDictionary<string, SchemaObjectFunctionTableReferenceSchemaObjectLink> schemaObjectFunctionTableReferenceSchemaObjectLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, List<SchemaObjectFunctionTableReferenceParametersItem>> schemaObjectFunctionTableReferenceParametersByOwnerId;
    private readonly IReadOnlyDictionary<string, NamedTableReferenceSchemaObjectLink> namedTableReferenceSchemaObjectLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, NamedTableReferenceTableSampleClauseLink> namedTableReferenceTableSampleClauseLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, TableSampleClause> tableSampleClauseById;
    private readonly IReadOnlyDictionary<string, TableSampleClauseSampleNumberLink> tableSampleClauseSampleNumberLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, TableSampleClauseRepeatSeedLink> tableSampleClauseRepeatSeedLinkByOwnerId;
    private readonly IReadOnlyDictionary<string, SchemaObjectName> schemaObjectNameById;
    private readonly IReadOnlyDictionary<string, MultiPartIdentifier> multiPartIdentifierById;
    private readonly IReadOnlyDictionary<string, List<MultiPartIdentifierIdentifiersItem>> multiPartIdentifierItemsByOwnerId;
    private readonly IReadOnlyDictionary<string, Identifier> identifierById;
    private readonly IReadOnlyDictionary<string, JoinTableReference> joinTableReferenceByTableReferenceId;
    private readonly IReadOnlyDictionary<string, JoinParenthesisTableReference> joinParenthesisTableReferenceByTableReferenceId;
    private readonly IReadOnlyDictionary<string, JoinParenthesisTableReferenceJoinLink> joinParenthesisTableReferenceJoinLinkByOwnerId;
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
        scriptStatementLinkByOwnerId = model.TransformScriptStatementLinkList.ToDictionary(item => item.TransformScript.Id, StringComparer.Ordinal);
        scriptFunctionParametersByOwnerId = GroupByOwner(model.TransformScriptFunctionParametersItemList);
        scriptObjectViewByOwnerId = model.ScriptObjectViewList.ToDictionary(item => item.TransformScript.Id, StringComparer.Ordinal);
        scriptObjectTvfByOwnerId = model.ScriptObjectTVFList.ToDictionary(item => item.TransformScript.Id, StringComparer.Ordinal);
        scriptObjectScalarFunctionByOwnerId = model.ScriptObjectScalarFunctionList.ToDictionary(item => item.TransformScript.Id, StringComparer.Ordinal);
        scriptObjectStoredProcedureByOwnerId = model.ScriptObjectStoredProcedureList.ToDictionary(item => item.TransformScript.Id, StringComparer.Ordinal);
        storedProcedureContractsByOwnerId = GroupByOwner(model.StoredProcedureContractList);
        storedProcedureOperationsByContractId = GroupByOwner(model.StoredProcedureContractOperationList);
        storedProcedureResultRowsetsByOwnerId = GroupByOwner(model.StoredProcedureResultRowsetItemList);
        storedProcedureResultColumnsByOwnerId = GroupByOwner(model.StoredProcedureResultColumnItemList);
        selectStatementById = model.SelectStatementList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        selectStatementByStatementWithCtesId = model.SelectStatementList.ToDictionary(item => item.StatementWithCtesAndXmlNamespaces.Id, StringComparer.Ordinal);
        selectStatementQueryExpressionLinkByOwnerId = model.SelectStatementQueryExpressionLinkList.ToDictionary(item => item.SelectStatement.Id, StringComparer.Ordinal);
        statementWithCtesBySqlStatementId = model.StatementWithCtesAndXmlNamespacesList.ToDictionary(item => item.TSqlStatement.Id, StringComparer.Ordinal);
        insertStatementByStatementWithCtesId = model.InsertStatementList.ToDictionary(item => item.StatementWithCtesAndXmlNamespaces.Id, StringComparer.Ordinal);
        insertStatementTargetLinkByOwnerId = model.InsertStatementTargetLinkList.ToDictionary(item => item.InsertStatement.Id, StringComparer.Ordinal);
        insertStatementSourceLinkByOwnerId = model.InsertStatementSourceLinkList.ToDictionary(item => item.InsertStatement.Id, StringComparer.Ordinal);
        insertQuerySourceByInsertSourceId = model.InsertQuerySourceList.ToDictionary(item => item.InsertSource.Id, StringComparer.Ordinal);
        insertQuerySourceQueryExpressionLinkByOwnerId = model.InsertQuerySourceQueryExpressionLinkList.ToDictionary(item => item.InsertQuerySource.Id, StringComparer.Ordinal);
        updateStatementByStatementWithCtesId = model.UpdateStatementList.ToDictionary(item => item.StatementWithCtesAndXmlNamespaces.Id, StringComparer.Ordinal);
        updateStatementTargetLinkByOwnerId = model.UpdateStatementTargetLinkList.ToDictionary(item => item.UpdateStatement.Id, StringComparer.Ordinal);
        updateStatementFromClauseLinkByOwnerId = model.UpdateStatementFromClauseLinkList.ToDictionary(item => item.UpdateStatement.Id, StringComparer.Ordinal);
        deleteStatementByStatementWithCtesId = model.DeleteStatementList.ToDictionary(item => item.StatementWithCtesAndXmlNamespaces.Id, StringComparer.Ordinal);
        deleteStatementTargetLinkByOwnerId = model.DeleteStatementTargetLinkList.ToDictionary(item => item.DeleteStatement.Id, StringComparer.Ordinal);
        deleteStatementFromClauseLinkByOwnerId = model.DeleteStatementFromClauseLinkList.ToDictionary(item => item.DeleteStatement.Id, StringComparer.Ordinal);
        mergeStatementByStatementWithCtesId = model.MergeStatementList.ToDictionary(item => item.StatementWithCtesAndXmlNamespaces.Id, StringComparer.Ordinal);
        mergeStatementTargetLinkByOwnerId = model.MergeStatementTargetLinkList.ToDictionary(item => item.MergeStatement.Id, StringComparer.Ordinal);
        mergeStatementSourceLinkByOwnerId = model.MergeStatementSourceLinkList.ToDictionary(item => item.MergeStatement.Id, StringComparer.Ordinal);
        truncateStatementBySqlStatementId = model.TruncateStatementList.ToDictionary(item => item.TSqlStatement.Id, StringComparer.Ordinal);
        truncateStatementTargetLinkByOwnerId = model.TruncateStatementTargetLinkList.ToDictionary(item => item.TruncateStatement.Id, StringComparer.Ordinal);
        statementWithCtesLinkByOwnerId = model.StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkList.ToDictionary(item => item.StatementWithCtesAndXmlNamespaces.Id, StringComparer.Ordinal);
        withCtesAndXmlNamespacesById = model.WithCtesAndXmlNamespacesList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        commonTableExpressionsByWithClauseOwnerId = GroupByOwner(model.WithCtesAndXmlNamespacesCommonTableExpressionsItemList);
        commonTableExpressionById = model.CommonTableExpressionList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        commonTableExpressionNameLinkByOwnerId = model.CommonTableExpressionExpressionNameLinkList.ToDictionary(item => item.CommonTableExpression.Id, StringComparer.Ordinal);
        commonTableExpressionQueryExpressionLinkByOwnerId = model.CommonTableExpressionQueryExpressionLinkList.ToDictionary(item => item.CommonTableExpression.Id, StringComparer.Ordinal);
        commonTableExpressionColumnsByOwnerId = GroupByOwner(model.CommonTableExpressionColumnsItemList);
        querySpecificationByQueryExpressionId = model.QuerySpecificationList.ToDictionary(item => item.QueryExpression.Id, StringComparer.Ordinal);
        binaryQueryExpressionByQueryExpressionId = model.BinaryQueryExpressionList.ToDictionary(item => item.QueryExpression.Id, StringComparer.Ordinal);
        queryParenthesisExpressionByQueryExpressionId = model.QueryParenthesisExpressionList.ToDictionary(item => item.QueryExpression.Id, StringComparer.Ordinal);
        queryParenthesisExpressionQueryExpressionLinkByOwnerId = model.QueryParenthesisExpressionQueryExpressionLinkList.ToDictionary(item => item.QueryParenthesisExpression.Id, StringComparer.Ordinal);
        binaryQueryExpressionFirstQueryExpressionLinkByOwnerId = model.BinaryQueryExpressionFirstQueryExpressionLinkList.ToDictionary(item => item.BinaryQueryExpression.Id, StringComparer.Ordinal);
        binaryQueryExpressionSecondQueryExpressionLinkByOwnerId = model.BinaryQueryExpressionSecondQueryExpressionLinkList.ToDictionary(item => item.BinaryQueryExpression.Id, StringComparer.Ordinal);
        fromClauseLinkByOwnerId = model.QuerySpecificationFromClauseLinkList.ToDictionary(item => item.QuerySpecification.Id, StringComparer.Ordinal);
        fromClauseById = model.FromClauseList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        fromClauseTableReferencesByOwnerId = GroupByOwner(model.FromClauseTableReferencesItemList);
        groupByClauseLinkByOwnerId = model.QuerySpecificationGroupByClauseLinkList.ToDictionary(item => item.QuerySpecification.Id, StringComparer.Ordinal);
        groupByClauseById = model.GroupByClauseList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        groupingSpecificationsByGroupByClauseOwnerId = GroupByOwner(model.GroupByClauseGroupingSpecificationsItemList);
        groupingSpecificationById = model.GroupingSpecificationList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        expressionGroupingSpecificationByBaseId = model.ExpressionGroupingSpecificationList.ToDictionary(item => item.GroupingSpecification.Id, StringComparer.Ordinal);
        expressionGroupingSpecificationExpressionLinkByOwnerId = model.ExpressionGroupingSpecificationExpressionLinkList.ToDictionary(item => item.ExpressionGroupingSpecification.Id, StringComparer.Ordinal);
        whereClauseLinkByOwnerId = model.QuerySpecificationWhereClauseLinkList.ToDictionary(item => item.QuerySpecification.Id, StringComparer.Ordinal);
        whereClauseById = model.WhereClauseList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        whereClauseSearchConditionLinkByOwnerId = model.WhereClauseSearchConditionLinkList.ToDictionary(item => item.WhereClause.Id, StringComparer.Ordinal);
        havingClauseLinkByOwnerId = model.QuerySpecificationHavingClauseLinkList.ToDictionary(item => item.QuerySpecification.Id, StringComparer.Ordinal);
        havingClauseById = model.HavingClauseList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        havingClauseSearchConditionLinkByOwnerId = model.HavingClauseSearchConditionLinkList.ToDictionary(item => item.HavingClause.Id, StringComparer.Ordinal);
        tableReferenceById = model.TableReferenceList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        tableReferenceWithAliasByTableReferenceId = model.TableReferenceWithAliasList.ToDictionary(item => item.TableReference.Id, StringComparer.Ordinal);
        tableReferenceAliasLinkByOwnerId = model.TableReferenceWithAliasAliasLinkList.ToDictionary(item => item.TableReferenceWithAlias.Id, StringComparer.Ordinal);
        tableReferenceWithAliasAndColumnsByBaseId = model.TableReferenceWithAliasAndColumnsList.ToDictionary(item => item.TableReferenceWithAlias.Id, StringComparer.Ordinal);
        tableReferenceWithAliasAndColumnsColumnsByOwnerId = GroupByOwner(model.TableReferenceWithAliasAndColumnsColumnsItemList);
        namedTableReferenceByAliasBaseId = model.NamedTableReferenceList.ToDictionary(item => item.TableReferenceWithAlias.Id, StringComparer.Ordinal);
        queryDerivedTableByBaseId = model.QueryDerivedTableList.ToDictionary(item => item.TableReferenceWithAliasAndColumns.Id, StringComparer.Ordinal);
        queryDerivedTableQueryExpressionLinkByOwnerId = model.QueryDerivedTableQueryExpressionLinkList.ToDictionary(item => item.QueryDerivedTable.Id, StringComparer.Ordinal);
        inlineDerivedTableByBaseId = model.InlineDerivedTableList.ToDictionary(item => item.TableReferenceWithAliasAndColumns.Id, StringComparer.Ordinal);
        inlineDerivedTableRowValuesByOwnerId = GroupByOwner(model.InlineDerivedTableRowValuesItemList);
        rowValueById = model.RowValueList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        rowValueColumnValuesByOwnerId = GroupByOwner(model.RowValueColumnValuesItemList);
        globalFunctionTableReferenceByBaseId = model.GlobalFunctionTableReferenceList.ToDictionary(item => item.TableReferenceWithAlias.Id, StringComparer.Ordinal);
        globalFunctionTableReferenceNameLinkByOwnerId = model.GlobalFunctionTableReferenceNameLinkList.ToDictionary(item => item.GlobalFunctionTableReference.Id, StringComparer.Ordinal);
        globalFunctionTableReferenceParametersByOwnerId = GroupByOwner(model.GlobalFunctionTableReferenceParametersItemList);
        schemaObjectFunctionTableReferenceByBaseId = model.SchemaObjectFunctionTableReferenceList.ToDictionary(item => item.TableReferenceWithAliasAndColumns.Id, StringComparer.Ordinal);
        schemaObjectFunctionTableReferenceSchemaObjectLinkByOwnerId = model.SchemaObjectFunctionTableReferenceSchemaObjectLinkList.ToDictionary(item => item.SchemaObjectFunctionTableReference.Id, StringComparer.Ordinal);
        schemaObjectFunctionTableReferenceParametersByOwnerId = GroupByOwner(model.SchemaObjectFunctionTableReferenceParametersItemList);
        namedTableReferenceSchemaObjectLinkByOwnerId = model.NamedTableReferenceSchemaObjectLinkList.ToDictionary(item => item.NamedTableReference.Id, StringComparer.Ordinal);
        namedTableReferenceTableSampleClauseLinkByOwnerId = model.NamedTableReferenceTableSampleClauseLinkList.ToDictionary(item => item.NamedTableReference.Id, StringComparer.Ordinal);
        tableSampleClauseById = model.TableSampleClauseList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        tableSampleClauseSampleNumberLinkByOwnerId = model.TableSampleClauseSampleNumberLinkList.ToDictionary(item => item.TableSampleClause.Id, StringComparer.Ordinal);
        tableSampleClauseRepeatSeedLinkByOwnerId = model.TableSampleClauseRepeatSeedLinkList.ToDictionary(item => item.TableSampleClause.Id, StringComparer.Ordinal);
        schemaObjectNameById = model.SchemaObjectNameList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        multiPartIdentifierById = model.MultiPartIdentifierList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        multiPartIdentifierItemsByOwnerId = GroupByOwner(model.MultiPartIdentifierIdentifiersItemList);
        identifierById = model.IdentifierList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        joinTableReferenceByTableReferenceId = model.JoinTableReferenceList.ToDictionary(item => item.TableReference.Id, StringComparer.Ordinal);
        joinParenthesisTableReferenceByTableReferenceId = model.JoinParenthesisTableReferenceList.ToDictionary(item => item.TableReference.Id, StringComparer.Ordinal);
        joinParenthesisTableReferenceJoinLinkByOwnerId = model.JoinParenthesisTableReferenceJoinLinkList.ToDictionary(item => item.JoinParenthesisTableReference.Id, StringComparer.Ordinal);
        qualifiedJoinByBaseId = model.QualifiedJoinList.ToDictionary(item => item.JoinTableReference.Id, StringComparer.Ordinal);
        unqualifiedJoinByBaseId = model.UnqualifiedJoinList.ToDictionary(item => item.JoinTableReference.Id, StringComparer.Ordinal);
        joinFirstTableReferenceLinkByOwnerId = model.JoinTableReferenceFirstTableReferenceLinkList.ToDictionary(item => item.JoinTableReference.Id, StringComparer.Ordinal);
        joinSecondTableReferenceLinkByOwnerId = model.JoinTableReferenceSecondTableReferenceLinkList.ToDictionary(item => item.JoinTableReference.Id, StringComparer.Ordinal);
        selectElementsByOwnerId = GroupByOwner(model.QuerySpecificationSelectElementsItemList);
        selectElementById = model.SelectElementList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        selectScalarExpressionBySelectElementId = model.SelectScalarExpressionList.ToDictionary(item => item.SelectElement.Id, StringComparer.Ordinal);
        selectScalarExpressionLinkByOwnerId = model.SelectScalarExpressionExpressionLinkList.ToDictionary(item => item.SelectScalarExpression.Id, StringComparer.Ordinal);
        selectScalarExpressionColumnNameLinkByOwnerId = model.SelectScalarExpressionColumnNameLinkList.ToDictionary(item => item.SelectScalarExpression.Id, StringComparer.Ordinal);
        identifierOrValueExpressionById = model.IdentifierOrValueExpressionList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        identifierOrValueExpressionIdentifierLinkByOwnerId = model.IdentifierOrValueExpressionIdentifierLinkList.ToDictionary(item => item.IdentifierOrValueExpression.Id, StringComparer.Ordinal);
        selectStarExpressionBySelectElementId = model.SelectStarExpressionList.ToDictionary(item => item.SelectElement.Id, StringComparer.Ordinal);
        selectStarQualifierLinkByOwnerId = model.SelectStarExpressionQualifierLinkList.ToDictionary(item => item.SelectStarExpression.Id, StringComparer.Ordinal);
        scalarExpressionById = model.ScalarExpressionList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        binaryExpressionByBaseId = model.BinaryExpressionList.ToDictionary(item => item.ScalarExpression.Id, StringComparer.Ordinal);
        binaryExpressionFirstExpressionLinkByOwnerId = model.BinaryExpressionFirstExpressionLinkList.ToDictionary(item => item.BinaryExpression.Id, StringComparer.Ordinal);
        binaryExpressionSecondExpressionLinkByOwnerId = model.BinaryExpressionSecondExpressionLinkList.ToDictionary(item => item.BinaryExpression.Id, StringComparer.Ordinal);
        unaryExpressionByBaseId = model.UnaryExpressionList.ToDictionary(item => item.ScalarExpression.Id, StringComparer.Ordinal);
        unaryExpressionExpressionLinkByOwnerId = model.UnaryExpressionExpressionLinkList.ToDictionary(item => item.UnaryExpression.Id, StringComparer.Ordinal);
        primaryExpressionByScalarExpressionId = model.PrimaryExpressionList.ToDictionary(item => item.ScalarExpression.Id, StringComparer.Ordinal);
        columnReferenceExpressionByPrimaryExpressionId = model.ColumnReferenceExpressionList.ToDictionary(item => item.PrimaryExpression.Id, StringComparer.Ordinal);
        columnReferenceExpressionLinkByOwnerId = model.ColumnReferenceExpressionMultiPartIdentifierLinkList.ToDictionary(item => item.ColumnReferenceExpression.Id, StringComparer.Ordinal);
        parenthesisExpressionByPrimaryExpressionId = model.ParenthesisExpressionList.ToDictionary(item => item.PrimaryExpression.Id, StringComparer.Ordinal);
        parenthesisExpressionExpressionLinkByOwnerId = model.ParenthesisExpressionExpressionLinkList.ToDictionary(item => item.ParenthesisExpression.Id, StringComparer.Ordinal);
        scalarSubqueryByPrimaryExpressionId = model.ScalarSubqueryList.ToDictionary(item => item.PrimaryExpression.Id, StringComparer.Ordinal);
        scalarSubqueryQueryExpressionLinkByOwnerId = model.ScalarSubqueryQueryExpressionLinkList.ToDictionary(item => item.ScalarSubquery.Id, StringComparer.Ordinal);
        functionCallByPrimaryExpressionId = model.FunctionCallList.ToDictionary(item => item.PrimaryExpression.Id, StringComparer.Ordinal);
        functionCallFunctionNameLinkByOwnerId = model.FunctionCallFunctionNameLinkList.ToDictionary(item => item.FunctionCall.Id, StringComparer.Ordinal);
        functionCallOverClauseLinkByOwnerId = model.FunctionCallOverClauseLinkList.ToDictionary(item => item.FunctionCall.Id, StringComparer.Ordinal);
        functionCallParametersByOwnerId = GroupByOwner(model.FunctionCallParametersItemList);
        leftFunctionCallByPrimaryExpressionId = model.LeftFunctionCallList.ToDictionary(item => item.PrimaryExpression.Id, StringComparer.Ordinal);
        leftFunctionCallParametersByOwnerId = GroupByOwner(model.LeftFunctionCallParametersItemList);
        rightFunctionCallByPrimaryExpressionId = model.RightFunctionCallList.ToDictionary(item => item.PrimaryExpression.Id, StringComparer.Ordinal);
        rightFunctionCallParametersByOwnerId = GroupByOwner(model.RightFunctionCallParametersItemList);
        parameterlessCallByPrimaryExpressionId = model.ParameterlessCallList.ToDictionary(item => item.PrimaryExpression.Id, StringComparer.Ordinal);
        coalesceExpressionByPrimaryExpressionId = model.CoalesceExpressionList.ToDictionary(item => item.PrimaryExpression.Id, StringComparer.Ordinal);
        coalesceExpressionExpressionsByOwnerId = GroupByOwner(model.CoalesceExpressionExpressionsItemList);
        nullIfExpressionByPrimaryExpressionId = model.NullIfExpressionList.ToDictionary(item => item.PrimaryExpression.Id, StringComparer.Ordinal);
        nullIfExpressionFirstExpressionLinkByOwnerId = model.NullIfExpressionFirstExpressionLinkList.ToDictionary(item => item.NullIfExpression.Id, StringComparer.Ordinal);
        nullIfExpressionSecondExpressionLinkByOwnerId = model.NullIfExpressionSecondExpressionLinkList.ToDictionary(item => item.NullIfExpression.Id, StringComparer.Ordinal);
        iIfCallByPrimaryExpressionId = model.IIfCallList.ToDictionary(item => item.PrimaryExpression.Id, StringComparer.Ordinal);
        iIfCallPredicateLinkByOwnerId = model.IIfCallPredicateLinkList.ToDictionary(item => item.IIfCall.Id, StringComparer.Ordinal);
        iIfCallThenExpressionLinkByOwnerId = model.IIfCallThenExpressionLinkList.ToDictionary(item => item.IIfCall.Id, StringComparer.Ordinal);
        iIfCallElseExpressionLinkByOwnerId = model.IIfCallElseExpressionLinkList.ToDictionary(item => item.IIfCall.Id, StringComparer.Ordinal);
        caseExpressionByPrimaryExpressionId = model.CaseExpressionList.ToDictionary(item => item.PrimaryExpression.Id, StringComparer.Ordinal);
        searchedCaseExpressionByCaseExpressionId = model.SearchedCaseExpressionList.ToDictionary(item => item.CaseExpression.Id, StringComparer.Ordinal);
        searchedCaseExpressionWhenClausesByOwnerId = GroupByOwner(model.SearchedCaseExpressionWhenClausesItemList);
        searchedWhenClauseById = model.SearchedWhenClauseList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        searchedWhenClauseWhenExpressionLinkByOwnerId = model.SearchedWhenClauseWhenExpressionLinkList.ToDictionary(item => item.SearchedWhenClause.Id, StringComparer.Ordinal);
        caseExpressionElseExpressionLinkByOwnerId = model.CaseExpressionElseExpressionLinkList.ToDictionary(item => item.CaseExpression.Id, StringComparer.Ordinal);
        whenClauseThenExpressionLinkByOwnerId = model.WhenClauseThenExpressionLinkList.ToDictionary(item => item.WhenClause.Id, StringComparer.Ordinal);
        simpleCaseExpressionByCaseExpressionId = model.SimpleCaseExpressionList.ToDictionary(item => item.CaseExpression.Id, StringComparer.Ordinal);
        simpleCaseExpressionInputExpressionLinkByOwnerId = model.SimpleCaseExpressionInputExpressionLinkList.ToDictionary(item => item.SimpleCaseExpression.Id, StringComparer.Ordinal);
        simpleCaseExpressionWhenClausesByOwnerId = GroupByOwner(model.SimpleCaseExpressionWhenClausesItemList);
        simpleWhenClauseById = model.SimpleWhenClauseList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        simpleWhenClauseWhenExpressionLinkByOwnerId = model.SimpleWhenClauseWhenExpressionLinkList.ToDictionary(item => item.SimpleWhenClause.Id, StringComparer.Ordinal);
        castCallByPrimaryExpressionId = model.CastCallList.ToDictionary(item => item.PrimaryExpression.Id, StringComparer.Ordinal);
        castCallParameterLinkByOwnerId = model.CastCallParameterLinkList.ToDictionary(item => item.CastCall.Id, StringComparer.Ordinal);
        convertCallByPrimaryExpressionId = model.ConvertCallList.ToDictionary(item => item.PrimaryExpression.Id, StringComparer.Ordinal);
        convertCallParameterLinkByOwnerId = model.ConvertCallParameterLinkList.ToDictionary(item => item.ConvertCall.Id, StringComparer.Ordinal);
        convertCallStyleLinkByOwnerId = model.ConvertCallStyleLinkList.ToDictionary(item => item.ConvertCall.Id, StringComparer.Ordinal);
        tryCastCallByPrimaryExpressionId = model.TryCastCallList.ToDictionary(item => item.PrimaryExpression.Id, StringComparer.Ordinal);
        tryCastCallParameterLinkByOwnerId = model.TryCastCallParameterLinkList.ToDictionary(item => item.TryCastCall.Id, StringComparer.Ordinal);
        tryConvertCallByPrimaryExpressionId = model.TryConvertCallList.ToDictionary(item => item.PrimaryExpression.Id, StringComparer.Ordinal);
        tryConvertCallParameterLinkByOwnerId = model.TryConvertCallParameterLinkList.ToDictionary(item => item.TryConvertCall.Id, StringComparer.Ordinal);
        tryConvertCallStyleLinkByOwnerId = model.TryConvertCallStyleLinkList.ToDictionary(item => item.TryConvertCall.Id, StringComparer.Ordinal);
        parseCallByPrimaryExpressionId = model.ParseCallList.ToDictionary(item => item.PrimaryExpression.Id, StringComparer.Ordinal);
        parseCallStringValueLinkByOwnerId = model.ParseCallStringValueLinkList.ToDictionary(item => item.ParseCall.Id, StringComparer.Ordinal);
        parseCallCultureLinkByOwnerId = model.ParseCallCultureLinkList.ToDictionary(item => item.ParseCall.Id, StringComparer.Ordinal);
        tryParseCallByPrimaryExpressionId = model.TryParseCallList.ToDictionary(item => item.PrimaryExpression.Id, StringComparer.Ordinal);
        tryParseCallStringValueLinkByOwnerId = model.TryParseCallStringValueLinkList.ToDictionary(item => item.TryParseCall.Id, StringComparer.Ordinal);
        tryParseCallCultureLinkByOwnerId = model.TryParseCallCultureLinkList.ToDictionary(item => item.TryParseCall.Id, StringComparer.Ordinal);
        atTimeZoneCallByPrimaryExpressionId = model.AtTimeZoneCallList.ToDictionary(item => item.PrimaryExpression.Id, StringComparer.Ordinal);
        atTimeZoneCallDateValueLinkByOwnerId = model.AtTimeZoneCallDateValueLinkList.ToDictionary(item => item.AtTimeZoneCall.Id, StringComparer.Ordinal);
        atTimeZoneCallTimeZoneLinkByOwnerId = model.AtTimeZoneCallTimeZoneLinkList.ToDictionary(item => item.AtTimeZoneCall.Id, StringComparer.Ordinal);
        booleanBinaryExpressionByBaseId = model.BooleanBinaryExpressionList.ToDictionary(item => item.BooleanExpression.Id, StringComparer.Ordinal);
        booleanBinaryExpressionFirstExpressionLinkByOwnerId = model.BooleanBinaryExpressionFirstExpressionLinkList.ToDictionary(item => item.BooleanBinaryExpression.Id, StringComparer.Ordinal);
        booleanBinaryExpressionSecondExpressionLinkByOwnerId = model.BooleanBinaryExpressionSecondExpressionLinkList.ToDictionary(item => item.BooleanBinaryExpression.Id, StringComparer.Ordinal);
        booleanComparisonExpressionByBaseId = model.BooleanComparisonExpressionList.ToDictionary(item => item.BooleanExpression.Id, StringComparer.Ordinal);
        booleanComparisonExpressionFirstExpressionLinkByOwnerId = model.BooleanComparisonExpressionFirstExpressionLinkList.ToDictionary(item => item.BooleanComparisonExpression.Id, StringComparer.Ordinal);
        booleanComparisonExpressionSecondExpressionLinkByOwnerId = model.BooleanComparisonExpressionSecondExpressionLinkList.ToDictionary(item => item.BooleanComparisonExpression.Id, StringComparer.Ordinal);
        booleanNotExpressionByBaseId = model.BooleanNotExpressionList.ToDictionary(item => item.BooleanExpression.Id, StringComparer.Ordinal);
        booleanNotExpressionExpressionLinkByOwnerId = model.BooleanNotExpressionExpressionLinkList.ToDictionary(item => item.BooleanNotExpression.Id, StringComparer.Ordinal);
        booleanParenthesisExpressionByBaseId = model.BooleanParenthesisExpressionList.ToDictionary(item => item.BooleanExpression.Id, StringComparer.Ordinal);
        booleanParenthesisExpressionExpressionLinkByOwnerId = model.BooleanParenthesisExpressionExpressionLinkList.ToDictionary(item => item.BooleanParenthesisExpression.Id, StringComparer.Ordinal);
        existsPredicateByBaseId = model.ExistsPredicateList.ToDictionary(item => item.BooleanExpression.Id, StringComparer.Ordinal);
        existsPredicateSubqueryLinkByOwnerId = model.ExistsPredicateSubqueryLinkList.ToDictionary(item => item.ExistsPredicate.Id, StringComparer.Ordinal);
        inPredicateByBaseId = model.InPredicateList.ToDictionary(item => item.BooleanExpression.Id, StringComparer.Ordinal);
        inPredicateExpressionLinkByOwnerId = model.InPredicateExpressionLinkList.ToDictionary(item => item.InPredicate.Id, StringComparer.Ordinal);
        inPredicateSubqueryLinkByOwnerId = model.InPredicateSubqueryLinkList.ToDictionary(item => item.InPredicate.Id, StringComparer.Ordinal);
        subqueryComparisonPredicateByBaseId = model.SubqueryComparisonPredicateList.ToDictionary(item => item.BooleanExpression.Id, StringComparer.Ordinal);
        subqueryComparisonPredicateExpressionLinkByOwnerId = model.SubqueryComparisonPredicateExpressionLinkList.ToDictionary(item => item.SubqueryComparisonPredicate.Id, StringComparer.Ordinal);
        subqueryComparisonPredicateSubqueryLinkByOwnerId = model.SubqueryComparisonPredicateSubqueryLinkList.ToDictionary(item => item.SubqueryComparisonPredicate.Id, StringComparer.Ordinal);
    }

    public SelectStatement? TryGetSelectStatement(TransformScript script)
    {
        if (!scriptStatementLinkByOwnerId.TryGetValue(script.Id, out var link))
        {
            return null;
        }

        if (!statementWithCtesBySqlStatementId.TryGetValue(link.TSqlStatement.Id, out var statementWithCtes))
        {
            return null;
        }

        return selectStatementByStatementWithCtesId.GetValueOrDefault(statementWithCtes.Id);
    }

    public BoundStatementKind GetTransformScriptStatementKind(TransformScript script)
    {
        if (!scriptStatementLinkByOwnerId.TryGetValue(script.Id, out var link))
        {
            if (scriptObjectScalarFunctionByOwnerId.ContainsKey(script.Id))
            {
                return BoundStatementKind.ScalarFunction;
            }

            if (scriptObjectStoredProcedureByOwnerId.ContainsKey(script.Id))
            {
                return BoundStatementKind.StoredProcedure;
            }

            return BoundStatementKind.Unsupported;
        }

        if (truncateStatementBySqlStatementId.ContainsKey(link.TSqlStatement.Id))
        {
            return BoundStatementKind.Truncate;
        }

        if (!statementWithCtesBySqlStatementId.TryGetValue(link.TSqlStatement.Id, out var statementWithCtes))
        {
            return BoundStatementKind.Unsupported;
        }

        if (selectStatementByStatementWithCtesId.ContainsKey(statementWithCtes.Id)) return BoundStatementKind.Select;
        if (insertStatementByStatementWithCtesId.ContainsKey(statementWithCtes.Id)) return BoundStatementKind.Insert;
        if (updateStatementByStatementWithCtesId.ContainsKey(statementWithCtes.Id)) return BoundStatementKind.Update;
        if (deleteStatementByStatementWithCtesId.ContainsKey(statementWithCtes.Id)) return BoundStatementKind.Delete;
        if (mergeStatementByStatementWithCtesId.ContainsKey(statementWithCtes.Id)) return BoundStatementKind.Merge;

        return BoundStatementKind.Unsupported;
    }

    public string? TryGetMutationTargetSqlIdentifier(TransformScript script)
    {
        if (!scriptStatementLinkByOwnerId.TryGetValue(script.Id, out var link))
        {
            return null;
        }

        if (truncateStatementBySqlStatementId.TryGetValue(link.TSqlStatement.Id, out var truncateStatement) &&
            truncateStatementTargetLinkByOwnerId.TryGetValue(truncateStatement.Id, out var truncateTarget))
        {
            return RenderSchemaObjectName(truncateTarget.SchemaObjectName);
        }

        var statementWithCtes = TryGetStatementWithCtes(script);
        if (statementWithCtes is null)
        {
            return null;
        }

        if (insertStatementByStatementWithCtesId.TryGetValue(statementWithCtes.Id, out var insertStatement) &&
            insertStatementTargetLinkByOwnerId.TryGetValue(insertStatement.Id, out var insertTarget))
        {
            return RenderSchemaObjectName(insertTarget.SchemaObjectName);
        }

        if (updateStatementByStatementWithCtesId.TryGetValue(statementWithCtes.Id, out var updateStatement) &&
            updateStatementTargetLinkByOwnerId.TryGetValue(updateStatement.Id, out var updateTarget))
        {
            return ResolveMutationTargetAgainstFromClause(
                RenderSchemaObjectName(updateTarget.SchemaObjectName),
                TryGetUpdateStatementFromClause(script));
        }

        if (deleteStatementByStatementWithCtesId.TryGetValue(statementWithCtes.Id, out var deleteStatement) &&
            deleteStatementTargetLinkByOwnerId.TryGetValue(deleteStatement.Id, out var deleteTarget))
        {
            return ResolveMutationTargetAgainstFromClause(
                RenderSchemaObjectName(deleteTarget.SchemaObjectName),
                TryGetDeleteStatementFromClause(script));
        }

        if (mergeStatementByStatementWithCtesId.TryGetValue(statementWithCtes.Id, out var mergeStatement) &&
            mergeStatementTargetLinkByOwnerId.TryGetValue(mergeStatement.Id, out var mergeTarget))
        {
            return RenderSchemaObjectName(mergeTarget.SchemaObjectName);
        }

        return null;
    }

    private string? ResolveMutationTargetAgainstFromClause(
        string? targetSqlIdentifier,
        FromClause? fromClause)
    {
        if (string.IsNullOrWhiteSpace(targetSqlIdentifier) ||
            fromClause is null ||
            targetSqlIdentifier.Contains('.', StringComparison.Ordinal))
        {
            return targetSqlIdentifier;
        }

        return TryResolveOnePartTableReferenceTarget(fromClause, targetSqlIdentifier, out var resolved)
            ? resolved
            : targetSqlIdentifier;
    }

    private bool TryResolveOnePartTableReferenceTarget(
        FromClause fromClause,
        string targetNameOrAlias,
        out string resolvedSqlIdentifier)
    {
        resolvedSqlIdentifier = string.Empty;
        foreach (var tableReference in GetTableReferences(fromClause))
        {
            if (TryResolveOnePartTableReferenceTarget(tableReference, targetNameOrAlias, out resolvedSqlIdentifier))
            {
                return true;
            }
        }

        return false;
    }

    private bool TryResolveOnePartTableReferenceTarget(
        TableReference tableReference,
        string targetNameOrAlias,
        out string resolvedSqlIdentifier)
    {
        resolvedSqlIdentifier = string.Empty;
        if (TryGetJoinParenthesisInnerJoinReference(tableReference) is { } innerJoinReference)
        {
            return TryResolveOnePartTableReferenceTarget(innerJoinReference, targetNameOrAlias, out resolvedSqlIdentifier);
        }

        if (TryGetJoinChildren(tableReference) is { } children)
        {
            if (children.First is not null &&
                TryResolveOnePartTableReferenceTarget(children.First, targetNameOrAlias, out resolvedSqlIdentifier))
            {
                return true;
            }

            if (children.Second is not null &&
                TryResolveOnePartTableReferenceTarget(children.Second, targetNameOrAlias, out resolvedSqlIdentifier))
            {
                return true;
            }
        }

        var namedTableReference = TryGetNamedTableReference(tableReference);
        if (namedTableReference is null)
        {
            return false;
        }

        var parts = GetNamedTableReferenceParts(namedTableReference);
        if (parts.Count == 0)
        {
            return false;
        }

        var tableAlias = TryGetTableAlias(tableReference);
        var leafName = parts[^1];
        if (!string.Equals(tableAlias, targetNameOrAlias, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(leafName, targetNameOrAlias, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        resolvedSqlIdentifier = string.Join(".", parts);
        return true;
    }

    public string? TryGetInsertStatementQueryExpressionId(TransformScript script)
    {
        var statementWithCtes = TryGetStatementWithCtes(script);
        if (statementWithCtes is null ||
            !insertStatementByStatementWithCtesId.TryGetValue(statementWithCtes.Id, out var insertStatement) ||
            !insertStatementSourceLinkByOwnerId.TryGetValue(insertStatement.Id, out var sourceLink) ||
            !insertQuerySourceByInsertSourceId.TryGetValue(sourceLink.InsertSource.Id, out var querySource) ||
            !insertQuerySourceQueryExpressionLinkByOwnerId.TryGetValue(querySource.Id, out var queryLink))
        {
            return null;
        }

        return queryLink.QueryExpression.Id;
    }

    public FromClause? TryGetUpdateStatementFromClause(TransformScript script)
    {
        var statementWithCtes = TryGetStatementWithCtes(script);
        return statementWithCtes is not null &&
               updateStatementByStatementWithCtesId.TryGetValue(statementWithCtes.Id, out var updateStatement) &&
               updateStatementFromClauseLinkByOwnerId.TryGetValue(updateStatement.Id, out var fromLink)
            ? fromClauseById.GetValueOrDefault(fromLink.FromClause.Id)
            : null;
    }

    public FromClause? TryGetDeleteStatementFromClause(TransformScript script)
    {
        var statementWithCtes = TryGetStatementWithCtes(script);
        return statementWithCtes is not null &&
               deleteStatementByStatementWithCtesId.TryGetValue(statementWithCtes.Id, out var deleteStatement) &&
               deleteStatementFromClauseLinkByOwnerId.TryGetValue(deleteStatement.Id, out var fromLink)
            ? fromClauseById.GetValueOrDefault(fromLink.FromClause.Id)
            : null;
    }

    public TableReference? TryGetMergeStatementSourceTableReference(TransformScript script)
    {
        var statementWithCtes = TryGetStatementWithCtes(script);
        return statementWithCtes is not null &&
               mergeStatementByStatementWithCtesId.TryGetValue(statementWithCtes.Id, out var mergeStatement) &&
               mergeStatementSourceLinkByOwnerId.TryGetValue(mergeStatement.Id, out var sourceLink)
            ? tableReferenceById.GetValueOrDefault(sourceLink.TableReference.Id)
            : null;
    }

    private StatementWithCtesAndXmlNamespaces? TryGetStatementWithCtes(TransformScript script)
    {
        return scriptStatementLinkByOwnerId.TryGetValue(script.Id, out var link) &&
               statementWithCtesBySqlStatementId.TryGetValue(link.TSqlStatement.Id, out var statementWithCtes)
            ? statementWithCtes
            : null;
    }

    private string? RenderSchemaObjectName(SchemaObjectName schemaObjectName)
    {
        return schemaObjectNameById.TryGetValue(schemaObjectName.Id, out var resolved)
            ? string.Join(".", GetMultiPartIdentifierParts(resolved.MultiPartIdentifier.Id))
            : null;
    }

    public string GetTransformScriptObjectKind(TransformScript script)
    {
        var hasView = scriptObjectViewByOwnerId.ContainsKey(script.Id);
        var hasTvf = scriptObjectTvfByOwnerId.ContainsKey(script.Id);
        var hasScalarFunction = scriptObjectScalarFunctionByOwnerId.ContainsKey(script.Id);
        var hasStoredProcedure = scriptObjectStoredProcedureByOwnerId.ContainsKey(script.Id);
        if ((hasView ? 1 : 0) + (hasTvf ? 1 : 0) + (hasScalarFunction ? 1 : 0) + (hasStoredProcedure ? 1 : 0) > 1)
        {
            throw new InvalidOperationException(
                $"Transform script '{script.Name}' has more than one script object row. Exactly one script object type is allowed.");
        }

        if (hasTvf)
        {
            return "InlineTableValuedFunction";
        }

        if (hasScalarFunction)
        {
            return "ScalarFunction";
        }

        if (hasStoredProcedure)
        {
            return "StoredProcedure";
        }

        return "View";
    }

    public string? TryGetTransformScriptTargetSqlIdentifier(TransformScript script)
    {
        return scriptObjectViewByOwnerId.TryGetValue(script.Id, out var scriptObjectView)
            ? scriptObjectView.TargetSqlIdentifier
            : null;
    }

    public ScriptObjectStoredProcedure? TryGetScriptObjectStoredProcedure(TransformScript script)
    {
        return scriptObjectStoredProcedureByOwnerId.GetValueOrDefault(script.Id);
    }

    public IReadOnlyList<StoredProcedureContract> GetStoredProcedureContracts(TransformScript script)
    {
        return TryGetScriptObjectStoredProcedure(script) is { } storedProcedure &&
               storedProcedureContractsByOwnerId.TryGetValue(storedProcedure.Id, out var items)
            ? items
            : [];
    }

    public IReadOnlyList<StoredProcedureContractOperation> GetStoredProcedureOperations(TransformScript script)
    {
        return TryGetSingleStoredProcedureContract(script) is { } contract &&
               storedProcedureOperationsByContractId.TryGetValue(contract.Id, out var items)
            ? items.OrderBy(item => ParseOrdinal(item.Ordinal)).ToArray()
            : [];
    }

    public IReadOnlyList<StoredProcedureResultRowsetItem> GetStoredProcedureResultRowsets(TransformScript script)
    {
        return TryGetSingleStoredProcedureContract(script) is { } contract &&
               storedProcedureResultRowsetsByOwnerId.TryGetValue(contract.Id, out var items)
            ? items.OrderBy(item => ParseOrdinal(item.Ordinal)).ToArray()
            : [];
    }

    private StoredProcedureContract? TryGetSingleStoredProcedureContract(TransformScript script)
    {
        var contracts = GetStoredProcedureContracts(script);
        return contracts.Count == 1 ? contracts[0] : null;
    }

    public IReadOnlyList<StoredProcedureResultColumnItem> GetStoredProcedureResultColumns(StoredProcedureResultRowsetItem rowset)
    {
        return storedProcedureResultColumnsByOwnerId.TryGetValue(rowset.Id, out var items)
            ? items.OrderBy(item => ParseOrdinal(item.Ordinal)).ToArray()
            : [];
    }

    public TransformScript? TryResolveScalarFunctionTransformScript(FunctionCall functionCall)
    {
        var functionName = TryGetFunctionCallName(functionCall);
        if (string.IsNullOrWhiteSpace(functionName))
        {
            return null;
        }

        var qualifiedNameParts = GetFunctionCallCallTargetParts(functionCall)
            .Concat([functionName.Trim()])
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .ToArray();
        var qualifiedName = string.Join(".", qualifiedNameParts);
        if (string.IsNullOrWhiteSpace(qualifiedName))
        {
            return null;
        }

        return model.TransformScriptList.SingleOrDefault(item =>
            scriptObjectScalarFunctionByOwnerId.ContainsKey(item.Id) &&
            string.Equals(item.Name, qualifiedName, StringComparison.OrdinalIgnoreCase));
    }

    public ScalarExpression? TryGetScalarFunctionReturnExpression(TransformScript script)
    {
        return scriptObjectScalarFunctionByOwnerId.TryGetValue(script.Id, out var scalarFunction)
            ? scalarExpressionById.GetValueOrDefault(scalarFunction.ScalarExpression.Id)
            : null;
    }

    public IReadOnlyList<string> GetTransformScriptFunctionParameterNames(TransformScript script)
    {
        if (!scriptFunctionParametersByOwnerId.TryGetValue(script.Id, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => identifierById.GetValueOrDefault(item.Identifier.Id)?.Value)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Cast<string>()
            .ToArray();
    }

    public string? TryGetSelectStatementQueryExpressionId(SelectStatement selectStatement)
    {
        return selectStatementQueryExpressionLinkByOwnerId.TryGetValue(selectStatement.Id, out var link)
            ? link.QueryExpression.Id
            : null;
    }

    public QuerySpecification? TryGetQuerySpecification(SelectStatement selectStatement)
    {
        if (!selectStatementQueryExpressionLinkByOwnerId.TryGetValue(selectStatement.Id, out var link))
        {
            return null;
        }

        return querySpecificationByQueryExpressionId.GetValueOrDefault(link.QueryExpression.Id);
    }

    public IReadOnlyList<CommonTableExpression> GetCommonTableExpressions(SelectStatement selectStatement)
    {
        return GetCommonTableExpressions(selectStatement.StatementWithCtesAndXmlNamespaces);
    }

    public IReadOnlyList<CommonTableExpression> GetCommonTableExpressions(TransformScript script)
    {
        var statementWithCtes = TryGetStatementWithCtes(script);
        return statementWithCtes is null
            ? []
            : GetCommonTableExpressions(statementWithCtes);
    }

    private IReadOnlyList<CommonTableExpression> GetCommonTableExpressions(StatementWithCtesAndXmlNamespaces statementWithCtes)
    {
        if (!statementWithCtesLinkByOwnerId.TryGetValue(statementWithCtes.Id, out var withCtesLink))
        {
            return [];
        }

        if (!withCtesAndXmlNamespacesById.TryGetValue(withCtesLink.WithCtesAndXmlNamespaces.Id, out var withCtes))
        {
            return [];
        }

        if (!commonTableExpressionsByWithClauseOwnerId.TryGetValue(withCtes.Id, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => commonTableExpressionById.GetValueOrDefault(item.CommonTableExpression.Id))
            .Where(item => item is not null)
            .Cast<CommonTableExpression>()
            .ToArray();
    }

    public string? TryGetCommonTableExpressionName(CommonTableExpression commonTableExpression)
    {
        return commonTableExpressionNameLinkByOwnerId.TryGetValue(commonTableExpression.Id, out var link)
            ? identifierById.GetValueOrDefault(link.Identifier.Id)?.Value
            : null;
    }

    public string? TryGetCommonTableExpressionQueryExpressionId(CommonTableExpression commonTableExpression)
    {
        return commonTableExpressionQueryExpressionLinkByOwnerId.TryGetValue(commonTableExpression.Id, out var link)
            ? link.QueryExpression.Id
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
            .Select(item => identifierById.GetValueOrDefault(item.Identifier.Id)?.Value)
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

        return (firstLink.QueryExpression.Id, secondLink.QueryExpression.Id);
    }

    public FromClause? TryGetFromClause(QuerySpecification querySpecification)
    {
        if (!fromClauseLinkByOwnerId.TryGetValue(querySpecification.Id, out var link))
        {
            return null;
        }

        return fromClauseById.GetValueOrDefault(link.FromClause.Id);
    }

    public IReadOnlyList<TableReference> GetTableReferences(FromClause fromClause)
    {
        if (!fromClauseTableReferencesByOwnerId.TryGetValue(fromClause.Id, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => tableReferenceById.GetValueOrDefault(item.TableReference.Id))
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

    public GlobalFunctionTableReference? TryGetGlobalFunctionTableReference(TableReference tableReference)
    {
        if (!tableReferenceWithAliasByTableReferenceId.TryGetValue(tableReference.Id, out var aliasBase))
        {
            return null;
        }

        return globalFunctionTableReferenceByBaseId.GetValueOrDefault(aliasBase.Id);
    }

    public string? TryGetGlobalFunctionTableReferenceName(GlobalFunctionTableReference functionTableReference)
    {
        return globalFunctionTableReferenceNameLinkByOwnerId.TryGetValue(functionTableReference.Id, out var link)
            ? identifierById.GetValueOrDefault(link.Identifier.Id)?.Value
            : null;
    }

    public IReadOnlyList<ScalarExpression> GetGlobalFunctionTableReferenceParameters(GlobalFunctionTableReference functionTableReference)
    {
        if (!globalFunctionTableReferenceParametersByOwnerId.TryGetValue(functionTableReference.Id, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => scalarExpressionById.GetValueOrDefault(item.ScalarExpression.Id))
            .Where(item => item is not null)
            .Cast<ScalarExpression>()
            .ToArray();
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
            .Select(item => rowValueById.GetValueOrDefault(item.RowValue.Id))
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
            .Select(item => scalarExpressionById.GetValueOrDefault(item.ScalarExpression.Id))
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

        if (!schemaObjectNameById.TryGetValue(link.SchemaObjectName.Id, out var schemaObjectName))
        {
            return [];
        }

        return GetMultiPartIdentifierParts(schemaObjectName.MultiPartIdentifier.Id);
    }

    public IReadOnlyList<ScalarExpression> GetSchemaObjectFunctionTableReferenceParameters(SchemaObjectFunctionTableReference functionTableReference)
    {
        if (!schemaObjectFunctionTableReferenceParametersByOwnerId.TryGetValue(functionTableReference.Id, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => scalarExpressionById.GetValueOrDefault(item.ScalarExpression.Id))
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
            tableReferenceById.GetValueOrDefault(firstLink.TableReference.Id),
            tableReferenceById.GetValueOrDefault(secondLink.TableReference.Id));
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

        return identifierById.GetValueOrDefault(link.Identifier.Id)?.Value;
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
            .Select(item => identifierById.GetValueOrDefault(item.Identifier.Id)?.Value)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Cast<string>()
            .ToArray();
    }

    public string? TryGetQueryDerivedTableQueryExpressionId(QueryDerivedTable queryDerivedTable)
    {
        return queryDerivedTableQueryExpressionLinkByOwnerId.TryGetValue(queryDerivedTable.Id, out var link)
            ? link.QueryExpression.Id
            : null;
    }

    public IReadOnlyList<string> GetNamedTableReferenceParts(NamedTableReference namedTableReference)
    {
        if (!namedTableReferenceSchemaObjectLinkByOwnerId.TryGetValue(namedTableReference.Id, out var link))
        {
            return [];
        }

        if (!schemaObjectNameById.TryGetValue(link.SchemaObjectName.Id, out var schemaObjectName))
        {
            return [];
        }

        return GetMultiPartIdentifierParts(schemaObjectName.MultiPartIdentifier.Id);
    }

    public ScalarExpression? TryGetNamedTableReferenceTableSampleNumber(NamedTableReference namedTableReference)
    {
        var tableSampleClause = TryGetNamedTableReferenceTableSampleClause(namedTableReference);
        if (tableSampleClause is null)
        {
            return null;
        }

        return tableSampleClauseSampleNumberLinkByOwnerId.TryGetValue(tableSampleClause.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpression.Id)
            : null;
    }

    public ScalarExpression? TryGetNamedTableReferenceTableSampleRepeatSeed(NamedTableReference namedTableReference)
    {
        var tableSampleClause = TryGetNamedTableReferenceTableSampleClause(namedTableReference);
        if (tableSampleClause is null)
        {
            return null;
        }

        return tableSampleClauseRepeatSeedLinkByOwnerId.TryGetValue(tableSampleClause.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpression.Id)
            : null;
    }

    private TableSampleClause? TryGetNamedTableReferenceTableSampleClause(NamedTableReference namedTableReference)
    {
        if (!namedTableReferenceTableSampleClauseLinkByOwnerId.TryGetValue(namedTableReference.Id, out var link))
        {
            return null;
        }

        return tableSampleClauseById.GetValueOrDefault(link.TableSampleClause.Id);
    }

    public IReadOnlyList<SelectElement> GetSelectElements(QuerySpecification querySpecification)
    {
        if (!selectElementsByOwnerId.TryGetValue(querySpecification.Id, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => selectElementById.GetValueOrDefault(item.SelectElement.Id))
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

        return scalarExpressionById.GetValueOrDefault(link.ScalarExpression.Id);
    }

    public string? TryGetSelectScalarExpressionAlias(SelectScalarExpression selectScalarExpression)
    {
        if (!selectScalarExpressionColumnNameLinkByOwnerId.TryGetValue(selectScalarExpression.Id, out var link))
        {
            return null;
        }

        if (!identifierOrValueExpressionById.TryGetValue(link.IdentifierOrValueExpression.Id, out var aliasValue))
        {
            return null;
        }

        if (identifierOrValueExpressionIdentifierLinkByOwnerId.TryGetValue(aliasValue.Id, out var identifierLink))
        {
            return identifierById.GetValueOrDefault(identifierLink.Identifier.Id)?.Value;
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

        return GetMultiPartIdentifierParts(link.MultiPartIdentifier.Id);
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

        return GetMultiPartIdentifierParts(link.MultiPartIdentifier.Id);
    }

    private IReadOnlyList<string> GetMultiPartIdentifierParts(string multiPartIdentifierId)
    {
        if (!multiPartIdentifierItemsByOwnerId.TryGetValue(multiPartIdentifierId, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => identifierById.GetValueOrDefault(item.Identifier.Id)?.Value)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Cast<string>()
            .ToArray();
    }

    private static IReadOnlyDictionary<string, List<T>> GroupByOwner<T>(IEnumerable<T> rows)
    {
        var ownerPropertyName = ResolveOwnerProperty(typeof(T));
        if (string.IsNullOrWhiteSpace(ownerPropertyName))
        {
            return new Dictionary<string, List<T>>(StringComparer.Ordinal);
        }

        return rows
            .GroupBy(item => GetRelatedId(item!, ownerPropertyName), StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.ToList(), StringComparer.Ordinal);
    }

    private static string GetRelatedId(object target, string propertyName)
    {
        var related = target.GetType().GetProperty(propertyName)?.GetValue(target);
        return related?.GetType().GetProperty("Id")?.GetValue(related) as string ?? string.Empty;
    }

    private static int ParseOrdinal(string? ordinal)
    {
        return int.TryParse(ordinal, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : int.MaxValue;
    }

    private static string? ResolveOwnerProperty(Type type)
    {
        return OwnerPropertyByType.GetOrAdd(type, static key =>
        {
            var referenceProperties = key.GetProperties()
                .Select(property => property.Name)
                .Where(name => IsReferenceProperty(key, name))
                .ToArray();

            if (referenceProperties.Length == 0)
            {
                return null;
            }

            if (referenceProperties.Length == 1)
            {
                return referenceProperties[0];
            }

            foreach (var candidate in referenceProperties.OrderByDescending(static value => value.Length))
            {
                if (key.Name.StartsWith(candidate, StringComparison.Ordinal))
                {
                    return candidate;
                }
            }

            return referenceProperties[0];
        });
    }

    private static bool IsReferenceProperty(Type ownerType, string propertyName)
    {
        var property = ownerType.GetProperty(propertyName);
        return property is not null &&
               property.PropertyType != typeof(string) &&
               property.PropertyType.GetProperty("Id") is not null;
    }
}
