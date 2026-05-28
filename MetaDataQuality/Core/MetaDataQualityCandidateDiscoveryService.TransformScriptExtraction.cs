using MetaDataQuality;
using MetaTransformScript;

namespace MetaDataQuality.Core;

public sealed partial class MetaDataQualityCandidateDiscoveryService
{
    private static IReadOnlyList<ExtractedScriptEvidence> ExtractWorkspaceEvidence(
        MetaTransformScriptModel transformModel)
    {
        var extractor = new TransformScriptEvidenceExtractor(transformModel);
        return transformModel.TransformScriptList
            .OrderBy(static item => item.Name, StringComparer.OrdinalIgnoreCase)
            .Select(script => new ExtractedScriptEvidence
            {
                TransformScript = script,
                Scan = extractor.ScanScript(script),
            })
            .ToArray();
    }

    private sealed class TransformScriptEvidenceExtractor
    {
        private readonly Dictionary<string, TransformScriptStatementLink> scriptStatementByScriptId;
        private readonly Dictionary<string, SelectStatementQueryExpressionLink> selectQueryBySelectStatementId;
        private readonly Dictionary<string, SelectStatement> selectStatementById;
        private readonly Dictionary<string, SelectStatement> selectStatementByStatementWithCtesId;
        private readonly Dictionary<string, StatementWithCtesAndXmlNamespaces> statementWithCtesBySqlStatementId;
        private readonly Dictionary<string, InsertStatement> insertStatementByStatementWithCtesId;
        private readonly Dictionary<string, InsertStatementSourceLink> insertSourceByInsertStatementId;
        private readonly Dictionary<string, InsertQuerySource> insertQuerySourceByInsertSourceId;
        private readonly Dictionary<string, InsertQuerySourceQueryExpressionLink> insertQueryByInsertQuerySourceId;
        private readonly Dictionary<string, UpdateStatement> updateStatementByStatementWithCtesId;
        private readonly Dictionary<string, UpdateStatementFromClauseLink> updateFromClauseByUpdateStatementId;
        private readonly Dictionary<string, DeleteStatement> deleteStatementByStatementWithCtesId;
        private readonly Dictionary<string, DeleteStatementFromClauseLink> deleteFromClauseByDeleteStatementId;
        private readonly Dictionary<string, MergeStatement> mergeStatementByStatementWithCtesId;
        private readonly Dictionary<string, MergeStatementSourceLink> mergeSourceByMergeStatementId;
        private readonly Dictionary<string, StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLink> withCtesLinkByStatementWithCtesId;
        private readonly Dictionary<string, List<WithCtesAndXmlNamespacesCommonTableExpressionsItem>> cteItemsByWithCtesId;
        private readonly Dictionary<string, CommonTableExpressionQueryExpressionLink> cteQueryByCteId;
        private readonly Dictionary<string, CommonTableExpressionExpressionNameLink> cteNameByCteId;
        private readonly Dictionary<string, Identifier> identifierById;
        private readonly Dictionary<string, ScriptObjectScalarFunction> scalarFunctionByQualifiedName;
        private readonly Dictionary<string, ScriptObjectScalarFunction> scalarFunctionByUnqualifiedName;

        private readonly Dictionary<string, QuerySpecification> querySpecificationByQueryExpressionId;
        private readonly Dictionary<string, BinaryQueryExpression> binaryQueryByQueryExpressionId;
        private readonly Dictionary<string, QueryParenthesisExpression> queryParenthesisByQueryExpressionId;
        private readonly Dictionary<string, QueryParenthesisExpressionQueryExpressionLink> queryParenthesisLinkByOwnerId;
        private readonly Dictionary<string, BinaryQueryExpressionFirstQueryExpressionLink> binaryFirstQueryByOwnerId;
        private readonly Dictionary<string, BinaryQueryExpressionSecondQueryExpressionLink> binarySecondQueryByOwnerId;

        private readonly Dictionary<string, QuerySpecificationFromClauseLink> fromClauseLinkByQuerySpecificationId;
        private readonly Dictionary<string, QuerySpecificationWhereClauseLink> whereClauseLinkByQuerySpecificationId;
        private readonly Dictionary<string, List<FromClauseTableReferencesItem>> fromClauseItemsByFromClauseId;
        private readonly Dictionary<string, WhereClauseSearchConditionLink> whereClauseSearchConditionByWhereClauseId;
        private readonly Dictionary<string, QuerySpecificationGroupByClauseLink> groupByLinkByQuerySpecificationId;
        private readonly Dictionary<string, List<QuerySpecificationSelectElementsItem>> selectElementItemsByQuerySpecificationId;
        private readonly Dictionary<string, SelectScalarExpression> selectScalarBySelectElementId;
        private readonly Dictionary<string, SelectScalarExpressionExpressionLink> selectScalarExpressionLinkBySelectScalarId;

        private readonly Dictionary<string, JoinTableReference> joinByTableReferenceId;
        private readonly Dictionary<string, JoinTableReferenceFirstTableReferenceLink> joinFirstByJoinId;
        private readonly Dictionary<string, JoinTableReferenceSecondTableReferenceLink> joinSecondByJoinId;
        private readonly Dictionary<string, QualifiedJoin> qualifiedJoinByJoinId;
        private readonly Dictionary<string, UnqualifiedJoin> unqualifiedJoinByJoinId;
        private readonly Dictionary<string, QualifiedJoinSearchConditionLink> qualifiedJoinPredicateByJoinId;

        private readonly Dictionary<string, JoinParenthesisTableReference> joinParenthesisByTableReferenceId;
        private readonly Dictionary<string, JoinParenthesisTableReferenceJoinLink> joinParenthesisLinkByOwnerId;

        private readonly Dictionary<string, TableReferenceWithAlias> tableReferenceWithAliasByTableReferenceId;
        private readonly Dictionary<string, TableReferenceWithAliasAndColumns> tableReferenceWithAliasAndColumnsByAliasId;
        private readonly Dictionary<string, TableReferenceWithAliasAliasLink> tableReferenceAliasByAliasId;
        private readonly Dictionary<string, QueryDerivedTable> queryDerivedByAliasAndColumnsId;
        private readonly Dictionary<string, QueryDerivedTableQueryExpressionLink> queryDerivedQueryByOwnerId;
        private readonly Dictionary<string, NamedTableReference> namedTableByAliasId;
        private readonly Dictionary<string, NamedTableReferenceSchemaObjectLink> namedTableSchemaObjectByNamedTableId;
        private readonly Dictionary<string, SchemaObjectName> schemaObjectById;
        private readonly Dictionary<string, MultiPartIdentifier> multiPartIdentifierById;
        private readonly Dictionary<string, List<MultiPartIdentifierIdentifiersItem>> multiPartIdentifierItemsByMultiPartId;
        private readonly Dictionary<string, PrimaryExpression> primaryExpressionByScalarExpressionId;
        private readonly Dictionary<string, ColumnReferenceExpression> columnReferenceByPrimaryExpressionId;
        private readonly Dictionary<string, ColumnReferenceExpressionMultiPartIdentifierLink> columnReferenceMultiPartByColumnReferenceId;
        private readonly Dictionary<string, ParenthesisExpression> parenthesisByPrimaryExpressionId;
        private readonly Dictionary<string, ParenthesisExpressionExpressionLink> parenthesisInnerByParenthesisExpressionId;
        private readonly Dictionary<string, UnaryExpression> unaryByScalarExpressionId;
        private readonly Dictionary<string, UnaryExpressionExpressionLink> unaryInnerByUnaryExpressionId;
        private readonly Dictionary<string, BinaryExpression> binaryByScalarExpressionId;
        private readonly Dictionary<string, BinaryExpressionFirstExpressionLink> binaryFirstByBinaryExpressionId;
        private readonly Dictionary<string, BinaryExpressionSecondExpressionLink> binarySecondByBinaryExpressionId;
        private readonly Dictionary<string, IdentifierOrValueExpression> identifierOrValueByPrimaryExpressionId;
        private readonly Dictionary<string, IdentifierOrValueExpressionIdentifierLink> identifierOrValueIdentifierByOwnerId;
        private readonly Dictionary<string, CoalesceExpression> coalesceExpressionByPrimaryExpressionId;
        private readonly Dictionary<string, List<CoalesceExpressionExpressionsItem>> coalesceExpressionItemsByCoalesceId;
        private readonly Dictionary<string, CaseExpression> caseExpressionByPrimaryExpressionId;
        private readonly Dictionary<string, SearchedCaseExpression> searchedCaseByCaseExpressionId;
        private readonly Dictionary<string, List<SearchedCaseExpressionWhenClausesItem>> searchedCaseItemsBySearchedCaseId;
        private readonly Dictionary<string, SearchedWhenClauseWhenExpressionLink> searchedWhenExpressionBySearchedWhenClauseId;
        private readonly Dictionary<string, SimpleCaseExpression> simpleCaseByCaseExpressionId;
        private readonly Dictionary<string, SimpleCaseExpressionInputExpressionLink> simpleCaseInputExpressionBySimpleCaseId;
        private readonly Dictionary<string, List<SimpleCaseExpressionWhenClausesItem>> simpleCaseItemsBySimpleCaseId;
        private readonly Dictionary<string, SimpleWhenClauseWhenExpressionLink> simpleWhenExpressionBySimpleWhenClauseId;
        private readonly Dictionary<string, WhenClauseThenExpressionLink> whenThenExpressionByWhenClauseId;
        private readonly Dictionary<string, CaseExpressionElseExpressionLink> caseElseExpressionByCaseExpressionId;
        private readonly Dictionary<string, FunctionCall> functionCallByPrimaryExpressionId;
        private readonly Dictionary<string, FunctionCallFunctionNameLink> functionCallNameByFunctionCallId;
        private readonly Dictionary<string, FunctionCallCallTargetLink> functionCallTargetByFunctionCallId;
        private readonly Dictionary<string, MultiPartIdentifierCallTarget> multiPartCallTargetByCallTargetId;
        private readonly Dictionary<string, MultiPartIdentifierCallTargetMultiPartIdentifierLink> multiPartCallTargetIdentifierByOwnerId;
        private readonly Dictionary<string, List<FunctionCallParametersItem>> functionCallParametersByFunctionCallId;
        private readonly Dictionary<string, ScalarSubquery> scalarSubqueryByPrimaryExpressionId;
        private readonly Dictionary<string, ScalarSubqueryQueryExpressionLink> scalarSubqueryQueryByScalarSubqueryId;

        private readonly Dictionary<string, BooleanComparisonExpression> booleanComparisonByBooleanExpressionId;
        private readonly Dictionary<string, BooleanComparisonExpressionFirstExpressionLink> booleanComparisonFirstByOwnerId;
        private readonly Dictionary<string, BooleanComparisonExpressionSecondExpressionLink> booleanComparisonSecondByOwnerId;
        private readonly Dictionary<string, BooleanIsNullExpression> booleanIsNullByBooleanExpressionId;
        private readonly Dictionary<string, BooleanIsNullExpressionExpressionLink> booleanIsNullExpressionLinkByOwnerId;
        private readonly Dictionary<string, BooleanBinaryExpression> booleanBinaryByBooleanExpressionId;
        private readonly Dictionary<string, BooleanBinaryExpressionFirstExpressionLink> booleanBinaryFirstByOwnerId;
        private readonly Dictionary<string, BooleanBinaryExpressionSecondExpressionLink> booleanBinarySecondByOwnerId;
        private readonly Dictionary<string, BooleanParenthesisExpression> booleanParenthesisByBooleanExpressionId;
        private readonly Dictionary<string, BooleanParenthesisExpressionExpressionLink> booleanParenthesisLinkByOwnerId;
        private readonly Dictionary<string, BooleanNotExpression> booleanNotByBooleanExpressionId;
        private readonly Dictionary<string, BooleanNotExpressionExpressionLink> booleanNotLinkByOwnerId;

        public TransformScriptEvidenceExtractor(MetaTransformScriptModel model)
        {
            scriptStatementByScriptId = ToFirstMap(model.TransformScriptStatementLinkList, static item => item.TransformScript.Id);
            selectQueryBySelectStatementId = ToFirstMap(model.SelectStatementQueryExpressionLinkList, static item => item.SelectStatement.Id);
            selectStatementById = ToFirstMap(model.SelectStatementList, static item => item.Id);
            selectStatementByStatementWithCtesId = ToFirstMap(model.SelectStatementList, static item => item.StatementWithCtesAndXmlNamespaces.Id);
            statementWithCtesBySqlStatementId = ToFirstMap(model.StatementWithCtesAndXmlNamespacesList, static item => item.TSqlStatement.Id);
            insertStatementByStatementWithCtesId = ToFirstMap(model.InsertStatementList, static item => item.StatementWithCtesAndXmlNamespaces.Id);
            insertSourceByInsertStatementId = ToFirstMap(model.InsertStatementSourceLinkList, static item => item.InsertStatement.Id);
            insertQuerySourceByInsertSourceId = ToFirstMap(model.InsertQuerySourceList, static item => item.InsertSource.Id);
            insertQueryByInsertQuerySourceId = ToFirstMap(model.InsertQuerySourceQueryExpressionLinkList, static item => item.InsertQuerySource.Id);
            updateStatementByStatementWithCtesId = ToFirstMap(model.UpdateStatementList, static item => item.StatementWithCtesAndXmlNamespaces.Id);
            updateFromClauseByUpdateStatementId = ToFirstMap(model.UpdateStatementFromClauseLinkList, static item => item.UpdateStatement.Id);
            deleteStatementByStatementWithCtesId = ToFirstMap(model.DeleteStatementList, static item => item.StatementWithCtesAndXmlNamespaces.Id);
            deleteFromClauseByDeleteStatementId = ToFirstMap(model.DeleteStatementFromClauseLinkList, static item => item.DeleteStatement.Id);
            mergeStatementByStatementWithCtesId = ToFirstMap(model.MergeStatementList, static item => item.StatementWithCtesAndXmlNamespaces.Id);
            mergeSourceByMergeStatementId = ToFirstMap(model.MergeStatementSourceLinkList, static item => item.MergeStatement.Id);
            withCtesLinkByStatementWithCtesId = ToFirstMap(
                model.StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkList,
                static item => item.StatementWithCtesAndXmlNamespaces.Id);
            cteItemsByWithCtesId = GroupByKey(model.WithCtesAndXmlNamespacesCommonTableExpressionsItemList, static item => item.WithCtesAndXmlNamespaces.Id);
            cteQueryByCteId = ToFirstMap(model.CommonTableExpressionQueryExpressionLinkList, static item => item.CommonTableExpression.Id);
            cteNameByCteId = ToFirstMap(model.CommonTableExpressionExpressionNameLinkList, static item => item.CommonTableExpression.Id);
            identifierById = ToFirstMap(model.IdentifierList, static item => item.Id);
            scalarFunctionByQualifiedName = model.ScriptObjectScalarFunctionList
                .Where(static item => !string.IsNullOrWhiteSpace(item.TransformScript.Name))
                .GroupBy(static item => NormalizeSqlIdentifierName(item.TransformScript.Name), StringComparer.OrdinalIgnoreCase)
                .Where(static group => !string.IsNullOrWhiteSpace(group.Key) && group.Count() == 1)
                .ToDictionary(static group => group.Key, static group => group.First(), StringComparer.OrdinalIgnoreCase);
            scalarFunctionByUnqualifiedName = model.ScriptObjectScalarFunctionList
                .Where(static item => !string.IsNullOrWhiteSpace(item.TransformScript.Name))
                .GroupBy(static item => NormalizeSqlIdentifierLeafName(item.TransformScript.Name), StringComparer.OrdinalIgnoreCase)
                .Where(static group => !string.IsNullOrWhiteSpace(group.Key) && group.Count() == 1)
                .ToDictionary(static group => group.Key, static group => group.First(), StringComparer.OrdinalIgnoreCase);

            querySpecificationByQueryExpressionId = ToFirstMap(model.QuerySpecificationList, static item => item.QueryExpression.Id);
            binaryQueryByQueryExpressionId = ToFirstMap(model.BinaryQueryExpressionList, static item => item.QueryExpression.Id);
            queryParenthesisByQueryExpressionId = ToFirstMap(model.QueryParenthesisExpressionList, static item => item.QueryExpression.Id);
            queryParenthesisLinkByOwnerId = ToFirstMap(model.QueryParenthesisExpressionQueryExpressionLinkList, static item => item.QueryParenthesisExpression.Id);
            binaryFirstQueryByOwnerId = ToFirstMap(model.BinaryQueryExpressionFirstQueryExpressionLinkList, static item => item.BinaryQueryExpression.Id);
            binarySecondQueryByOwnerId = ToFirstMap(model.BinaryQueryExpressionSecondQueryExpressionLinkList, static item => item.BinaryQueryExpression.Id);

            fromClauseLinkByQuerySpecificationId = ToFirstMap(model.QuerySpecificationFromClauseLinkList, static item => item.QuerySpecification.Id);
            whereClauseLinkByQuerySpecificationId = ToFirstMap(model.QuerySpecificationWhereClauseLinkList, static item => item.QuerySpecification.Id);
            fromClauseItemsByFromClauseId = GroupByKey(model.FromClauseTableReferencesItemList, static item => item.FromClause.Id);
            whereClauseSearchConditionByWhereClauseId = ToFirstMap(model.WhereClauseSearchConditionLinkList, static item => item.WhereClause.Id);
            groupByLinkByQuerySpecificationId = ToFirstMap(model.QuerySpecificationGroupByClauseLinkList, static item => item.QuerySpecification.Id);
            selectElementItemsByQuerySpecificationId = GroupByKey(model.QuerySpecificationSelectElementsItemList, static item => item.QuerySpecification.Id);
            selectScalarBySelectElementId = ToFirstMap(model.SelectScalarExpressionList, static item => item.SelectElement.Id);
            selectScalarExpressionLinkBySelectScalarId = ToFirstMap(model.SelectScalarExpressionExpressionLinkList, static item => item.SelectScalarExpression.Id);

            joinByTableReferenceId = ToFirstMap(model.JoinTableReferenceList, static item => item.TableReference.Id);
            joinFirstByJoinId = ToFirstMap(model.JoinTableReferenceFirstTableReferenceLinkList, static item => item.JoinTableReference.Id);
            joinSecondByJoinId = ToFirstMap(model.JoinTableReferenceSecondTableReferenceLinkList, static item => item.JoinTableReference.Id);
            qualifiedJoinByJoinId = ToFirstMap(model.QualifiedJoinList, static item => item.JoinTableReference.Id);
            unqualifiedJoinByJoinId = ToFirstMap(model.UnqualifiedJoinList, static item => item.JoinTableReference.Id);
            qualifiedJoinPredicateByJoinId = ToFirstMap(model.QualifiedJoinSearchConditionLinkList, static item => item.QualifiedJoin.Id);

            joinParenthesisByTableReferenceId = ToFirstMap(model.JoinParenthesisTableReferenceList, static item => item.TableReference.Id);
            joinParenthesisLinkByOwnerId = ToFirstMap(model.JoinParenthesisTableReferenceJoinLinkList, static item => item.JoinParenthesisTableReference.Id);

            tableReferenceWithAliasByTableReferenceId = ToFirstMap(model.TableReferenceWithAliasList, static item => item.TableReference.Id);
            tableReferenceWithAliasAndColumnsByAliasId = ToFirstMap(model.TableReferenceWithAliasAndColumnsList, static item => item.TableReferenceWithAlias.Id);
            tableReferenceAliasByAliasId = ToFirstMap(model.TableReferenceWithAliasAliasLinkList, static item => item.TableReferenceWithAlias.Id);
            queryDerivedByAliasAndColumnsId = ToFirstMap(model.QueryDerivedTableList, static item => item.TableReferenceWithAliasAndColumns.Id);
            queryDerivedQueryByOwnerId = ToFirstMap(model.QueryDerivedTableQueryExpressionLinkList, static item => item.QueryDerivedTable.Id);
            namedTableByAliasId = ToFirstMap(model.NamedTableReferenceList, static item => item.TableReferenceWithAlias.Id);
            namedTableSchemaObjectByNamedTableId = ToFirstMap(model.NamedTableReferenceSchemaObjectLinkList, static item => item.NamedTableReference.Id);
            schemaObjectById = ToFirstMap(model.SchemaObjectNameList, static item => item.Id);
            multiPartIdentifierById = ToFirstMap(model.MultiPartIdentifierList, static item => item.Id);
            multiPartIdentifierItemsByMultiPartId = GroupByKey(model.MultiPartIdentifierIdentifiersItemList, static item => item.MultiPartIdentifier.Id);
            primaryExpressionByScalarExpressionId = ToFirstMap(model.PrimaryExpressionList, static item => item.ScalarExpression.Id);
            columnReferenceByPrimaryExpressionId = ToFirstMap(model.ColumnReferenceExpressionList, static item => item.PrimaryExpression.Id);
            columnReferenceMultiPartByColumnReferenceId = ToFirstMap(model.ColumnReferenceExpressionMultiPartIdentifierLinkList, static item => item.ColumnReferenceExpression.Id);
            parenthesisByPrimaryExpressionId = ToFirstMap(model.ParenthesisExpressionList, static item => item.PrimaryExpression.Id);
            parenthesisInnerByParenthesisExpressionId = ToFirstMap(model.ParenthesisExpressionExpressionLinkList, static item => item.ParenthesisExpression.Id);
            unaryByScalarExpressionId = ToFirstMap(model.UnaryExpressionList, static item => item.ScalarExpression.Id);
            unaryInnerByUnaryExpressionId = ToFirstMap(model.UnaryExpressionExpressionLinkList, static item => item.UnaryExpression.Id);
            binaryByScalarExpressionId = ToFirstMap(model.BinaryExpressionList, static item => item.ScalarExpression.Id);
            binaryFirstByBinaryExpressionId = ToFirstMap(model.BinaryExpressionFirstExpressionLinkList, static item => item.BinaryExpression.Id);
            binarySecondByBinaryExpressionId = ToFirstMap(model.BinaryExpressionSecondExpressionLinkList, static item => item.BinaryExpression.Id);
            identifierOrValueByPrimaryExpressionId = ToFirstMap(model.IdentifierOrValueExpressionList, static item => item.Id);
            identifierOrValueIdentifierByOwnerId = ToFirstMap(model.IdentifierOrValueExpressionIdentifierLinkList, static item => item.IdentifierOrValueExpression.Id);
            coalesceExpressionByPrimaryExpressionId = ToFirstMap(model.CoalesceExpressionList, static item => item.PrimaryExpression.Id);
            coalesceExpressionItemsByCoalesceId = GroupByKey(model.CoalesceExpressionExpressionsItemList, static item => item.CoalesceExpression.Id);
            caseExpressionByPrimaryExpressionId = ToFirstMap(model.CaseExpressionList, static item => item.PrimaryExpression.Id);
            searchedCaseByCaseExpressionId = ToFirstMap(model.SearchedCaseExpressionList, static item => item.CaseExpression.Id);
            searchedCaseItemsBySearchedCaseId = GroupByKey(model.SearchedCaseExpressionWhenClausesItemList, static item => item.SearchedCaseExpression.Id);
            searchedWhenExpressionBySearchedWhenClauseId = ToFirstMap(model.SearchedWhenClauseWhenExpressionLinkList, static item => item.SearchedWhenClause.Id);
            simpleCaseByCaseExpressionId = ToFirstMap(model.SimpleCaseExpressionList, static item => item.CaseExpression.Id);
            simpleCaseInputExpressionBySimpleCaseId = ToFirstMap(model.SimpleCaseExpressionInputExpressionLinkList, static item => item.SimpleCaseExpression.Id);
            simpleCaseItemsBySimpleCaseId = GroupByKey(model.SimpleCaseExpressionWhenClausesItemList, static item => item.SimpleCaseExpression.Id);
            simpleWhenExpressionBySimpleWhenClauseId = ToFirstMap(model.SimpleWhenClauseWhenExpressionLinkList, static item => item.SimpleWhenClause.Id);
            whenThenExpressionByWhenClauseId = ToFirstMap(model.WhenClauseThenExpressionLinkList, static item => item.WhenClause.Id);
            caseElseExpressionByCaseExpressionId = ToFirstMap(model.CaseExpressionElseExpressionLinkList, static item => item.CaseExpression.Id);
            functionCallByPrimaryExpressionId = ToFirstMap(model.FunctionCallList, static item => item.PrimaryExpression.Id);
            functionCallNameByFunctionCallId = ToFirstMap(model.FunctionCallFunctionNameLinkList, static item => item.FunctionCall.Id);
            functionCallTargetByFunctionCallId = ToFirstMap(model.FunctionCallCallTargetLinkList, static item => item.FunctionCall.Id);
            multiPartCallTargetByCallTargetId = ToFirstMap(model.MultiPartIdentifierCallTargetList, static item => item.CallTarget.Id);
            multiPartCallTargetIdentifierByOwnerId = ToFirstMap(model.MultiPartIdentifierCallTargetMultiPartIdentifierLinkList, static item => item.MultiPartIdentifierCallTarget.Id);
            functionCallParametersByFunctionCallId = GroupByKey(model.FunctionCallParametersItemList, static item => item.FunctionCall.Id);
            scalarSubqueryByPrimaryExpressionId = ToFirstMap(model.ScalarSubqueryList, static item => item.PrimaryExpression.Id);
            scalarSubqueryQueryByScalarSubqueryId = ToFirstMap(model.ScalarSubqueryQueryExpressionLinkList, static item => item.ScalarSubquery.Id);

            booleanComparisonByBooleanExpressionId = ToFirstMap(model.BooleanComparisonExpressionList, static item => item.BooleanExpression.Id);
            booleanComparisonFirstByOwnerId = ToFirstMap(model.BooleanComparisonExpressionFirstExpressionLinkList, static item => item.BooleanComparisonExpression.Id);
            booleanComparisonSecondByOwnerId = ToFirstMap(model.BooleanComparisonExpressionSecondExpressionLinkList, static item => item.BooleanComparisonExpression.Id);
            booleanIsNullByBooleanExpressionId = ToFirstMap(model.BooleanIsNullExpressionList, static item => item.BooleanExpression.Id);
            booleanIsNullExpressionLinkByOwnerId = ToFirstMap(model.BooleanIsNullExpressionExpressionLinkList, static item => item.BooleanIsNullExpression.Id);
            booleanBinaryByBooleanExpressionId = ToFirstMap(model.BooleanBinaryExpressionList, static item => item.BooleanExpression.Id);
            booleanBinaryFirstByOwnerId = ToFirstMap(model.BooleanBinaryExpressionFirstExpressionLinkList, static item => item.BooleanBinaryExpression.Id);
            booleanBinarySecondByOwnerId = ToFirstMap(model.BooleanBinaryExpressionSecondExpressionLinkList, static item => item.BooleanBinaryExpression.Id);
            booleanParenthesisByBooleanExpressionId = ToFirstMap(model.BooleanParenthesisExpressionList, static item => item.BooleanExpression.Id);
            booleanParenthesisLinkByOwnerId = ToFirstMap(model.BooleanParenthesisExpressionExpressionLinkList, static item => item.BooleanParenthesisExpression.Id);
            booleanNotByBooleanExpressionId = ToFirstMap(model.BooleanNotExpressionList, static item => item.BooleanExpression.Id);
            booleanNotLinkByOwnerId = ToFirstMap(model.BooleanNotExpressionExpressionLinkList, static item => item.BooleanNotExpression.Id);
        }

        public ScriptScanResult ScanScript(TransformScript script)
        {
            var result = new ScriptScanResult();
            if (!scriptStatementByScriptId.TryGetValue(script.Id, out var scriptStatementLink) ||
                !statementWithCtesBySqlStatementId.TryGetValue(scriptStatementLink.TSqlStatement.Id, out var statementWithCtes))
            {
                return result;
            }

            var cteDefinitionsByName = BuildCteDefinitionsByStatementWithCtesId(statementWithCtes.Id);

            if (selectStatementByStatementWithCtesId.TryGetValue(statementWithCtes.Id, out var selectStatement) &&
                selectQueryBySelectStatementId.TryGetValue(selectStatement.Id, out var selectQueryLink))
            {
                ScanQueryExpression(
                    selectQueryLink.QueryExpression.Id,
                    QueryScope.Main(),
                    cteDefinitionsByName,
                    result,
                    new HashSet<string>(StringComparer.Ordinal),
                    new HashSet<string>(StringComparer.Ordinal),
                    new HashSet<string>(StringComparer.Ordinal));
            }
            else if (TryGetInsertQueryExpressionId(statementWithCtes.Id, out var insertQueryExpressionId))
            {
                ScanQueryExpression(
                    insertQueryExpressionId,
                    QueryScope.Main().WithPathSegment("InsertSource"),
                    cteDefinitionsByName,
                    result,
                    new HashSet<string>(StringComparer.Ordinal),
                    new HashSet<string>(StringComparer.Ordinal),
                    new HashSet<string>(StringComparer.Ordinal));
            }
            else if (TryGetUpdateFromClauseId(statementWithCtes.Id, out var updateFromClauseId))
            {
                ScanFromClause(
                    $"{statementWithCtes.Id}.UpdateFrom",
                    $"{statementWithCtes.Id}.UpdateFrom",
                    updateFromClauseId,
                    QueryScope.Main().WithPathSegment("UpdateFrom"),
                    cteDefinitionsByName,
                    result);
            }
            else if (TryGetDeleteFromClauseId(statementWithCtes.Id, out var deleteFromClauseId))
            {
                ScanFromClause(
                    $"{statementWithCtes.Id}.DeleteFrom",
                    $"{statementWithCtes.Id}.DeleteFrom",
                    deleteFromClauseId,
                    QueryScope.Main().WithPathSegment("DeleteFrom"),
                    cteDefinitionsByName,
                    result);
            }
            else if (TryGetMergeSourceTableReferenceId(statementWithCtes.Id, out var mergeSourceTableReferenceId))
            {
                ScanTableReference(
                    $"{statementWithCtes.Id}.MergeSource",
                    $"{statementWithCtes.Id}.MergeSource",
                    mergeSourceTableReferenceId,
                    QueryScope.Main().WithPathSegment("MergeSource"),
                    cteDefinitionsByName,
                    result,
                    new HashSet<string>(StringComparer.Ordinal),
                    new HashSet<string>(StringComparer.Ordinal),
                    new HashSet<string>(StringComparer.Ordinal));
            }

            foreach (var cteDefinition in cteDefinitionsByName.Values
                         .OrderBy(static item => item.Name, StringComparer.OrdinalIgnoreCase))
            {
                ScanQueryExpression(
                    cteDefinition.QueryExpressionId,
                    QueryScope.ForCte(cteDefinition.CteId, cteDefinition.Name),
                    cteDefinitionsByName,
                    result,
                    new HashSet<string>(StringComparer.Ordinal),
                    new HashSet<string>(StringComparer.Ordinal),
                    new HashSet<string>(StringComparer.Ordinal));
            }

            return result;
        }

        private IReadOnlyDictionary<string, CteDefinition> BuildCteDefinitionsByName(string selectStatementId)
        {
            if (!selectStatementById.TryGetValue(selectStatementId, out var selectStatement))
            {
                return new Dictionary<string, CteDefinition>(StringComparer.OrdinalIgnoreCase);
            }

            return BuildCteDefinitionsByStatementWithCtesId(selectStatement.StatementWithCtesAndXmlNamespaces.Id);
        }

        private IReadOnlyDictionary<string, CteDefinition> BuildCteDefinitionsByStatementWithCtesId(string statementWithCtesId)
        {
            var map = new Dictionary<string, CteDefinition>(StringComparer.OrdinalIgnoreCase);
            if (!withCtesLinkByStatementWithCtesId.TryGetValue(statementWithCtesId, out var withCtesLink))
            {
                return map;
            }

            if (!cteItemsByWithCtesId.TryGetValue(withCtesLink.WithCtesAndXmlNamespaces.Id, out var cteItems))
            {
                return map;
            }

            foreach (var cteItem in cteItems.OrderBy(static item => ParseOrdinalOrMax(item.Ordinal)))
            {
                if (!cteQueryByCteId.TryGetValue(cteItem.CommonTableExpression.Id, out var cteQuery))
                {
                    continue;
                }

                var cteName = ResolveCteName(cteItem.CommonTableExpression.Id);
                if (string.IsNullOrWhiteSpace(cteName))
                {
                    continue;
                }

                if (!map.ContainsKey(cteName))
                {
                    map.Add(
                        cteName,
                        new CteDefinition
                        {
                            CteId = cteItem.CommonTableExpression.Id,
                            Name = cteName,
                            QueryExpressionId = cteQuery.QueryExpression.Id,
                        });
                }
            }

            return map;
        }

        private bool TryGetInsertQueryExpressionId(string statementWithCtesId, out string queryExpressionId)
        {
            queryExpressionId = string.Empty;
            if (!insertStatementByStatementWithCtesId.TryGetValue(statementWithCtesId, out var insertStatement) ||
                !insertSourceByInsertStatementId.TryGetValue(insertStatement.Id, out var sourceLink) ||
                !insertQuerySourceByInsertSourceId.TryGetValue(sourceLink.InsertSource.Id, out var querySource) ||
                !insertQueryByInsertQuerySourceId.TryGetValue(querySource.Id, out var queryLink))
            {
                return false;
            }

            queryExpressionId = queryLink.QueryExpression.Id;
            return !string.IsNullOrWhiteSpace(queryExpressionId);
        }

        private bool TryGetUpdateFromClauseId(string statementWithCtesId, out string fromClauseId)
        {
            fromClauseId = string.Empty;
            if (!updateStatementByStatementWithCtesId.TryGetValue(statementWithCtesId, out var updateStatement) ||
                !updateFromClauseByUpdateStatementId.TryGetValue(updateStatement.Id, out var fromClauseLink))
            {
                return false;
            }

            fromClauseId = fromClauseLink.FromClause.Id;
            return !string.IsNullOrWhiteSpace(fromClauseId);
        }

        private bool TryGetDeleteFromClauseId(string statementWithCtesId, out string fromClauseId)
        {
            fromClauseId = string.Empty;
            if (!deleteStatementByStatementWithCtesId.TryGetValue(statementWithCtesId, out var deleteStatement) ||
                !deleteFromClauseByDeleteStatementId.TryGetValue(deleteStatement.Id, out var fromClauseLink))
            {
                return false;
            }

            fromClauseId = fromClauseLink.FromClause.Id;
            return !string.IsNullOrWhiteSpace(fromClauseId);
        }

        private bool TryGetMergeSourceTableReferenceId(string statementWithCtesId, out string tableReferenceId)
        {
            tableReferenceId = string.Empty;
            if (!mergeStatementByStatementWithCtesId.TryGetValue(statementWithCtesId, out var mergeStatement) ||
                !mergeSourceByMergeStatementId.TryGetValue(mergeStatement.Id, out var sourceLink))
            {
                return false;
            }

            tableReferenceId = sourceLink.TableReference.Id;
            return !string.IsNullOrWhiteSpace(tableReferenceId);
        }

        private string ResolveCteName(string cteId)
        {
            if (!cteNameByCteId.TryGetValue(cteId, out var cteNameLink))
            {
                return string.Empty;
            }

            return identifierById.TryGetValue(cteNameLink.Identifier.Id, out var identifier)
                ? identifier.Value ?? string.Empty
                : string.Empty;
        }

        private IReadOnlyList<string> GetOrderedFromTableReferenceIds(string querySpecificationId)
        {
            if (!fromClauseLinkByQuerySpecificationId.TryGetValue(querySpecificationId, out var fromClauseLink))
            {
                return [];
            }

            return GetOrderedFromClauseTableReferenceIds(fromClauseLink.FromClause.Id);
        }

        private IReadOnlyList<string> GetOrderedFromClauseTableReferenceIds(string fromClauseId)
        {
            if (!fromClauseItemsByFromClauseId.TryGetValue(fromClauseId, out var tableReferenceItems))
            {
                return [];
            }

            return tableReferenceItems
                .OrderBy(static item => ParseOrdinalOrMax(item.Ordinal))
                .Select(static item => item.TableReference.Id)
                .Where(static item => !string.IsNullOrWhiteSpace(item))
                .ToArray();
        }

        private bool TryGetJoinParenthesizedTableReferenceId(
            string tableReferenceId,
            out string innerTableReferenceId)
        {
            innerTableReferenceId = string.Empty;
            if (!joinParenthesisByTableReferenceId.TryGetValue(tableReferenceId, out var joinParenthesis))
            {
                return false;
            }

            if (!joinParenthesisLinkByOwnerId.TryGetValue(joinParenthesis.Id, out var joinParenthesisLink))
            {
                return false;
            }

            innerTableReferenceId = joinParenthesisLink.TableReference.Id;
            return !string.IsNullOrWhiteSpace(innerTableReferenceId);
        }

        private void TryGetJoinChildTableReferenceIds(
            string joinTableReferenceId,
            out string firstTableReferenceId,
            out string secondTableReferenceId)
        {
            firstTableReferenceId = joinFirstByJoinId.TryGetValue(joinTableReferenceId, out var first)
                ? first.TableReference.Id
                : string.Empty;
            secondTableReferenceId = joinSecondByJoinId.TryGetValue(joinTableReferenceId, out var second)
                ? second.TableReference.Id
                : string.Empty;
        }

        private static QueryScope WithDerivedScope(QueryScope scope, string aliasName)
        {
            return scope.WithPathSegment(
                string.IsNullOrWhiteSpace(aliasName)
                    ? "DerivedQuery"
                    : $"DerivedQuery:{aliasName}");
        }

        private void ScanQueryExpression(
            string queryExpressionId,
            QueryScope scope,
            IReadOnlyDictionary<string, CteDefinition> cteDefinitionsByName,
            ScriptScanResult result,
            HashSet<string> visitedQueryExpressionIds,
            HashSet<string> visitedQuerySpecificationIds,
            HashSet<string> visitedTableReferenceIds)
        {
            if (string.IsNullOrWhiteSpace(queryExpressionId) || !visitedQueryExpressionIds.Add(queryExpressionId))
            {
                return;
            }

            if (querySpecificationByQueryExpressionId.TryGetValue(queryExpressionId, out var querySpecification))
            {
                ScanQuerySpecification(
                    queryExpressionId,
                    querySpecification,
                    scope,
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds);
            }

            ScanQueryExpressionChildren(
                queryExpressionId,
                scope,
                cteDefinitionsByName,
                result,
                visitedQueryExpressionIds,
                visitedQuerySpecificationIds,
                visitedTableReferenceIds);
        }

        private void ScanQueryExpressionChildren(
            string queryExpressionId,
            QueryScope scope,
            IReadOnlyDictionary<string, CteDefinition> cteDefinitionsByName,
            ScriptScanResult result,
            HashSet<string> visitedQueryExpressionIds,
            HashSet<string> visitedQuerySpecificationIds,
            HashSet<string> visitedTableReferenceIds)
        {
            if (binaryQueryByQueryExpressionId.TryGetValue(queryExpressionId, out var binaryQuery))
            {
                if (binaryFirstQueryByOwnerId.TryGetValue(binaryQuery.Id, out var first))
                {
                    ScanQueryExpression(
                        first.QueryExpression.Id,
                        scope,
                        cteDefinitionsByName,
                        result,
                        visitedQueryExpressionIds,
                        visitedQuerySpecificationIds,
                        visitedTableReferenceIds);
                }

                if (binarySecondQueryByOwnerId.TryGetValue(binaryQuery.Id, out var second))
                {
                    ScanQueryExpression(
                        second.QueryExpression.Id,
                        scope,
                        cteDefinitionsByName,
                        result,
                        visitedQueryExpressionIds,
                        visitedQuerySpecificationIds,
                        visitedTableReferenceIds);
                }
            }

            if (queryParenthesisByQueryExpressionId.TryGetValue(queryExpressionId, out var queryParenthesis)
                && queryParenthesisLinkByOwnerId.TryGetValue(queryParenthesis.Id, out var queryParenthesisLink))
            {
                ScanQueryExpression(
                    queryParenthesisLink.QueryExpression.Id,
                    scope.WithPathSegment("QueryParenthesis"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds);
            }
        }

        private void ScanQuerySpecification(
            string queryExpressionId,
            QuerySpecification querySpecification,
            QueryScope scope,
            IReadOnlyDictionary<string, CteDefinition> cteDefinitionsByName,
            ScriptScanResult result,
            HashSet<string> visitedQueryExpressionIds,
            HashSet<string> visitedQuerySpecificationIds,
            HashSet<string> visitedTableReferenceIds)
        {
            if (!visitedQuerySpecificationIds.Add(querySpecification.Id))
            {
                return;
            }

            if (string.Equals(querySpecification.UniqueRowFilter, "Distinct", StringComparison.OrdinalIgnoreCase))
            {
                result.HasDistinct = true;
            }

            if (groupByLinkByQuerySpecificationId.ContainsKey(querySpecification.Id))
            {
                result.HasGroupBy = true;
            }

            var tableReferenceIds = GetOrderedFromTableReferenceIds(querySpecification.Id);
            foreach (var tableReferenceId in tableReferenceIds)
            {
                ScanTableReference(
                    queryExpressionId,
                    querySpecification.Id,
                    tableReferenceId,
                    scope.WithPathSegment("From"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds);
            }

            ScanSelectScalarExpressions(
                queryExpressionId,
                querySpecification.Id,
                scope.WithPathSegment("Select"),
                cteDefinitionsByName,
                result,
                visitedQueryExpressionIds,
                visitedQuerySpecificationIds,
                visitedTableReferenceIds);

            ScanWhereBooleanExpression(
                queryExpressionId,
                querySpecification.Id,
                scope.WithPathSegment("Where"),
                cteDefinitionsByName,
                result,
                visitedQueryExpressionIds,
                visitedQuerySpecificationIds,
                visitedTableReferenceIds);
        }

        private void ScanSelectScalarExpressions(
            string queryExpressionId,
            string querySpecificationId,
            QueryScope scope,
            IReadOnlyDictionary<string, CteDefinition> cteDefinitionsByName,
            ScriptScanResult result,
            HashSet<string> visitedQueryExpressionIds,
            HashSet<string> visitedQuerySpecificationIds,
            HashSet<string> visitedTableReferenceIds)
        {
            if (!selectElementItemsByQuerySpecificationId.TryGetValue(querySpecificationId, out var selectElementItems))
            {
                return;
            }

            foreach (var selectElementItem in selectElementItems.OrderBy(static item => ParseOrdinalOrMax(item.Ordinal)))
            {
                if (!selectScalarBySelectElementId.TryGetValue(selectElementItem.SelectElement.Id, out var selectScalar) ||
                    !selectScalarExpressionLinkBySelectScalarId.TryGetValue(selectScalar.Id, out var expressionLink))
                {
                    continue;
                }

                ScanScalarExpression(
                    expressionLink.ScalarExpression.Id,
                    scope,
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds,
                    new HashSet<string>(StringComparer.Ordinal),
                    new HashSet<string>(StringComparer.Ordinal));
            }
        }

        private void ScanScalarExpression(
            string scalarExpressionId,
            QueryScope scope,
            IReadOnlyDictionary<string, CteDefinition> cteDefinitionsByName,
            ScriptScanResult result,
            HashSet<string> visitedQueryExpressionIds,
            HashSet<string> visitedQuerySpecificationIds,
            HashSet<string> visitedTableReferenceIds,
            HashSet<string> visitedScalarExpressionIds,
            HashSet<string> visitedScalarFunctionScriptIds)
        {
            if (string.IsNullOrWhiteSpace(scalarExpressionId) || !visitedScalarExpressionIds.Add(scalarExpressionId))
            {
                return;
            }

            if (TryGetUnaryInnerScalarExpression(scalarExpressionId, out _, out var unaryInnerScalarExpressionId))
            {
                ScanScalarExpression(
                    unaryInnerScalarExpressionId,
                    scope.WithPathSegment("UnaryExpression"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds,
                    visitedScalarExpressionIds,
                    visitedScalarFunctionScriptIds);
            }

            if (TryGetBinaryScalarExpressionChildren(
                    scalarExpressionId,
                    out _,
                    out var binaryFirstScalarExpressionId,
                    out var binarySecondScalarExpressionId))
            {
                ScanScalarExpression(
                    binaryFirstScalarExpressionId,
                    scope.WithPathSegment("BinaryLeft"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds,
                    visitedScalarExpressionIds,
                    visitedScalarFunctionScriptIds);
                ScanScalarExpression(
                    binarySecondScalarExpressionId,
                    scope.WithPathSegment("BinaryRight"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds,
                    visitedScalarExpressionIds,
                    visitedScalarFunctionScriptIds);
            }

            if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpressionId, out var primaryExpression))
            {
                return;
            }

            if (TryGetParenthesizedScalarExpression(primaryExpression.Id, out var parenthesizedScalarExpressionId))
            {
                ScanScalarExpression(
                    parenthesizedScalarExpressionId,
                    scope.WithPathSegment("ParenthesizedExpression"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds,
                    visitedScalarExpressionIds,
                    visitedScalarFunctionScriptIds);
            }

            if (scalarSubqueryByPrimaryExpressionId.TryGetValue(primaryExpression.Id, out var scalarSubquery) &&
                scalarSubqueryQueryByScalarSubqueryId.TryGetValue(scalarSubquery.Id, out var scalarSubqueryLink))
            {
                ScanQueryExpression(
                    scalarSubqueryLink.QueryExpression.Id,
                    scope.WithPathSegment("ScalarSubquery"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds);
            }

            if (coalesceExpressionByPrimaryExpressionId.TryGetValue(primaryExpression.Id, out var coalesceExpression))
            {
                ScanCoalesceExpression(
                    coalesceExpression,
                    scope.WithPathSegment("CoalesceExpression"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds,
                    visitedScalarExpressionIds,
                    visitedScalarFunctionScriptIds);
            }

            if (caseExpressionByPrimaryExpressionId.TryGetValue(primaryExpression.Id, out var caseExpression))
            {
                ScanCaseExpression(
                    caseExpression,
                    scope.WithPathSegment("CaseExpression"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds,
                    visitedScalarExpressionIds,
                    visitedScalarFunctionScriptIds);
            }

            if (!functionCallByPrimaryExpressionId.TryGetValue(primaryExpression.Id, out var functionCall))
            {
                return;
            }

            if (functionCallParametersByFunctionCallId.TryGetValue(functionCall.Id, out var parameters))
            {
                foreach (var parameter in parameters.OrderBy(static item => ParseOrdinalOrMax(item.Ordinal)))
                {
                    ScanScalarExpression(
                        parameter.ScalarExpression.Id,
                        scope.WithPathSegment("FunctionArgument"),
                        cteDefinitionsByName,
                        result,
                        visitedQueryExpressionIds,
                        visitedQuerySpecificationIds,
                        visitedTableReferenceIds,
                        visitedScalarExpressionIds,
                        visitedScalarFunctionScriptIds);
                }
            }

            if (TryResolveKnownScalarFunction(functionCall, out var scalarFunction) &&
                visitedScalarFunctionScriptIds.Add(scalarFunction.TransformScript.Id))
            {
                ScanScalarExpression(
                    scalarFunction.ScalarExpression.Id,
                    scope.WithPathSegment($"ScalarFunction:{scalarFunction.TransformScript.Name}"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds,
                    visitedScalarExpressionIds,
                    visitedScalarFunctionScriptIds);
            }
        }

        private void ScanCoalesceExpression(
            CoalesceExpression coalesceExpression,
            QueryScope scope,
            IReadOnlyDictionary<string, CteDefinition> cteDefinitionsByName,
            ScriptScanResult result,
            HashSet<string> visitedQueryExpressionIds,
            HashSet<string> visitedQuerySpecificationIds,
            HashSet<string> visitedTableReferenceIds,
            HashSet<string> visitedScalarExpressionIds,
            HashSet<string> visitedScalarFunctionScriptIds)
        {
            if (!coalesceExpressionItemsByCoalesceId.TryGetValue(coalesceExpression.Id, out var items))
            {
                return;
            }

            foreach (var item in items.OrderBy(static row => ParseOrdinalOrMax(row.Ordinal)))
            {
                ScanScalarExpression(
                    item.ScalarExpression.Id,
                    scope,
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds,
                    visitedScalarExpressionIds,
                    visitedScalarFunctionScriptIds);
            }
        }

        private void ScanCaseExpression(
            CaseExpression caseExpression,
            QueryScope scope,
            IReadOnlyDictionary<string, CteDefinition> cteDefinitionsByName,
            ScriptScanResult result,
            HashSet<string> visitedQueryExpressionIds,
            HashSet<string> visitedQuerySpecificationIds,
            HashSet<string> visitedTableReferenceIds,
            HashSet<string> visitedScalarExpressionIds,
            HashSet<string> visitedScalarFunctionScriptIds)
        {
            if (searchedCaseByCaseExpressionId.TryGetValue(caseExpression.Id, out var searchedCaseExpression))
            {
                ScanSearchedCaseExpression(
                    searchedCaseExpression,
                    scope.WithPathSegment("SearchedCase"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds,
                    visitedScalarExpressionIds,
                    visitedScalarFunctionScriptIds);
            }

            if (simpleCaseByCaseExpressionId.TryGetValue(caseExpression.Id, out var simpleCaseExpression))
            {
                ScanSimpleCaseExpression(
                    simpleCaseExpression,
                    scope.WithPathSegment("SimpleCase"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds,
                    visitedScalarExpressionIds,
                    visitedScalarFunctionScriptIds);
            }

            if (caseElseExpressionByCaseExpressionId.TryGetValue(caseExpression.Id, out var elseExpression))
            {
                ScanScalarExpression(
                    elseExpression.ScalarExpression.Id,
                    scope.WithPathSegment("Else"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds,
                    visitedScalarExpressionIds,
                    visitedScalarFunctionScriptIds);
            }
        }

        private void ScanSearchedCaseExpression(
            SearchedCaseExpression searchedCaseExpression,
            QueryScope scope,
            IReadOnlyDictionary<string, CteDefinition> cteDefinitionsByName,
            ScriptScanResult result,
            HashSet<string> visitedQueryExpressionIds,
            HashSet<string> visitedQuerySpecificationIds,
            HashSet<string> visitedTableReferenceIds,
            HashSet<string> visitedScalarExpressionIds,
            HashSet<string> visitedScalarFunctionScriptIds)
        {
            if (!searchedCaseItemsBySearchedCaseId.TryGetValue(searchedCaseExpression.Id, out var items))
            {
                return;
            }

            foreach (var item in items.OrderBy(static row => ParseOrdinalOrMax(row.Ordinal)))
            {
                if (searchedWhenExpressionBySearchedWhenClauseId.TryGetValue(item.SearchedWhenClause.Id, out var whenExpression))
                {
                    ScanBooleanExpression(
                        whenExpression.BooleanExpression.Id,
                        scope.WithPathSegment("When"),
                        cteDefinitionsByName,
                        result,
                        visitedQueryExpressionIds,
                        visitedQuerySpecificationIds,
                        visitedTableReferenceIds,
                        new HashSet<string>(StringComparer.Ordinal),
                        visitedScalarExpressionIds,
                        visitedScalarFunctionScriptIds);
                }

                ScanCaseWhenThenExpression(
                    item.SearchedWhenClause.WhenClause.Id,
                    scope.WithPathSegment("Then"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds,
                    visitedScalarExpressionIds,
                    visitedScalarFunctionScriptIds);
            }
        }

        private void ScanSimpleCaseExpression(
            SimpleCaseExpression simpleCaseExpression,
            QueryScope scope,
            IReadOnlyDictionary<string, CteDefinition> cteDefinitionsByName,
            ScriptScanResult result,
            HashSet<string> visitedQueryExpressionIds,
            HashSet<string> visitedQuerySpecificationIds,
            HashSet<string> visitedTableReferenceIds,
            HashSet<string> visitedScalarExpressionIds,
            HashSet<string> visitedScalarFunctionScriptIds)
        {
            if (simpleCaseInputExpressionBySimpleCaseId.TryGetValue(simpleCaseExpression.Id, out var inputExpression))
            {
                ScanScalarExpression(
                    inputExpression.ScalarExpression.Id,
                    scope.WithPathSegment("Input"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds,
                    visitedScalarExpressionIds,
                    visitedScalarFunctionScriptIds);
            }

            if (!simpleCaseItemsBySimpleCaseId.TryGetValue(simpleCaseExpression.Id, out var items))
            {
                return;
            }

            foreach (var item in items.OrderBy(static row => ParseOrdinalOrMax(row.Ordinal)))
            {
                if (simpleWhenExpressionBySimpleWhenClauseId.TryGetValue(item.SimpleWhenClause.Id, out var whenExpression))
                {
                    ScanScalarExpression(
                        whenExpression.ScalarExpression.Id,
                        scope.WithPathSegment("When"),
                        cteDefinitionsByName,
                        result,
                        visitedQueryExpressionIds,
                        visitedQuerySpecificationIds,
                        visitedTableReferenceIds,
                        visitedScalarExpressionIds,
                        visitedScalarFunctionScriptIds);
                }

                ScanCaseWhenThenExpression(
                    item.SimpleWhenClause.WhenClause.Id,
                    scope.WithPathSegment("Then"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds,
                    visitedScalarExpressionIds,
                    visitedScalarFunctionScriptIds);
            }
        }

        private void ScanCaseWhenThenExpression(
            string whenClauseId,
            QueryScope scope,
            IReadOnlyDictionary<string, CteDefinition> cteDefinitionsByName,
            ScriptScanResult result,
            HashSet<string> visitedQueryExpressionIds,
            HashSet<string> visitedQuerySpecificationIds,
            HashSet<string> visitedTableReferenceIds,
            HashSet<string> visitedScalarExpressionIds,
            HashSet<string> visitedScalarFunctionScriptIds)
        {
            if (!whenThenExpressionByWhenClauseId.TryGetValue(whenClauseId, out var thenExpression))
            {
                return;
            }

            ScanScalarExpression(
                thenExpression.ScalarExpression.Id,
                scope,
                cteDefinitionsByName,
                result,
                visitedQueryExpressionIds,
                visitedQuerySpecificationIds,
                visitedTableReferenceIds,
                visitedScalarExpressionIds,
                visitedScalarFunctionScriptIds);
        }

        private void ScanWhereBooleanExpression(
            string queryExpressionId,
            string querySpecificationId,
            QueryScope scope,
            IReadOnlyDictionary<string, CteDefinition> cteDefinitionsByName,
            ScriptScanResult result,
            HashSet<string> visitedQueryExpressionIds,
            HashSet<string> visitedQuerySpecificationIds,
            HashSet<string> visitedTableReferenceIds)
        {
            if (!TryResolveWhereBooleanExpressionId(querySpecificationId, out var whereBooleanExpressionId))
            {
                return;
            }

            ScanBooleanExpression(
                whereBooleanExpressionId,
                scope,
                cteDefinitionsByName,
                result,
                visitedQueryExpressionIds,
                visitedQuerySpecificationIds,
                visitedTableReferenceIds,
                new HashSet<string>(StringComparer.Ordinal),
                new HashSet<string>(StringComparer.Ordinal),
                new HashSet<string>(StringComparer.Ordinal));
        }

        private void ScanBooleanExpression(
            string booleanExpressionId,
            QueryScope scope,
            IReadOnlyDictionary<string, CteDefinition> cteDefinitionsByName,
            ScriptScanResult result,
            HashSet<string> visitedQueryExpressionIds,
            HashSet<string> visitedQuerySpecificationIds,
            HashSet<string> visitedTableReferenceIds,
            HashSet<string> visitedBooleanExpressionIds,
            HashSet<string> visitedScalarExpressionIds,
            HashSet<string> visitedScalarFunctionScriptIds)
        {
            if (string.IsNullOrWhiteSpace(booleanExpressionId) || !visitedBooleanExpressionIds.Add(booleanExpressionId))
            {
                return;
            }

            if (booleanComparisonByBooleanExpressionId.TryGetValue(booleanExpressionId, out var comparison))
            {
                if (booleanComparisonFirstByOwnerId.TryGetValue(comparison.Id, out var first))
                {
                    ScanScalarExpression(
                        first.ScalarExpression.Id,
                        scope.WithPathSegment("ComparisonLeft"),
                        cteDefinitionsByName,
                        result,
                        visitedQueryExpressionIds,
                        visitedQuerySpecificationIds,
                        visitedTableReferenceIds,
                        visitedScalarExpressionIds,
                        visitedScalarFunctionScriptIds);
                }

                if (booleanComparisonSecondByOwnerId.TryGetValue(comparison.Id, out var second))
                {
                    ScanScalarExpression(
                        second.ScalarExpression.Id,
                        scope.WithPathSegment("ComparisonRight"),
                        cteDefinitionsByName,
                        result,
                        visitedQueryExpressionIds,
                        visitedQuerySpecificationIds,
                        visitedTableReferenceIds,
                        visitedScalarExpressionIds,
                        visitedScalarFunctionScriptIds);
                }

                return;
            }

            if (booleanIsNullByBooleanExpressionId.TryGetValue(booleanExpressionId, out var isNullExpression))
            {
                if (booleanIsNullExpressionLinkByOwnerId.TryGetValue(isNullExpression.Id, out var isNullLink))
                {
                    ScanScalarExpression(
                        isNullLink.ScalarExpression.Id,
                        scope.WithPathSegment("IsNullExpression"),
                        cteDefinitionsByName,
                        result,
                        visitedQueryExpressionIds,
                        visitedQuerySpecificationIds,
                        visitedTableReferenceIds,
                        visitedScalarExpressionIds,
                        visitedScalarFunctionScriptIds);
                }

                return;
            }

            if (TryGetBooleanBinaryChildren(booleanExpressionId, out var firstBooleanExpressionId, out var secondBooleanExpressionId))
            {
                ScanBooleanExpression(
                    firstBooleanExpressionId,
                    scope.WithPathSegment("BooleanLeft"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds,
                    visitedBooleanExpressionIds,
                    visitedScalarExpressionIds,
                    visitedScalarFunctionScriptIds);
                ScanBooleanExpression(
                    secondBooleanExpressionId,
                    scope.WithPathSegment("BooleanRight"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds,
                    visitedBooleanExpressionIds,
                    visitedScalarExpressionIds,
                    visitedScalarFunctionScriptIds);
                return;
            }

            if (TryGetParenthesizedBooleanExpressionId(booleanExpressionId, out var parenthesizedBooleanExpressionId))
            {
                ScanBooleanExpression(
                    parenthesizedBooleanExpressionId,
                    scope.WithPathSegment("BooleanParenthesis"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds,
                    visitedBooleanExpressionIds,
                    visitedScalarExpressionIds,
                    visitedScalarFunctionScriptIds);
                return;
            }

            if (TryGetNotBooleanExpressionId(booleanExpressionId, out var notBooleanExpressionId))
            {
                ScanBooleanExpression(
                    notBooleanExpressionId,
                    scope.WithPathSegment("BooleanNot"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds,
                    visitedBooleanExpressionIds,
                    visitedScalarExpressionIds,
                    visitedScalarFunctionScriptIds);
            }
        }

        private void ScanFromClause(
            string queryExpressionId,
            string querySpecificationId,
            string fromClauseId,
            QueryScope scope,
            IReadOnlyDictionary<string, CteDefinition> cteDefinitionsByName,
            ScriptScanResult result)
        {
            foreach (var tableReferenceId in GetOrderedFromClauseTableReferenceIds(fromClauseId))
            {
                ScanTableReference(
                    queryExpressionId,
                    querySpecificationId,
                    tableReferenceId,
                    scope,
                    cteDefinitionsByName,
                    result,
                    new HashSet<string>(StringComparer.Ordinal),
                    new HashSet<string>(StringComparer.Ordinal),
                    new HashSet<string>(StringComparer.Ordinal));
            }
        }

        private void ScanTableReference(
            string queryExpressionId,
            string querySpecificationId,
            string tableReferenceId,
            QueryScope scope,
            IReadOnlyDictionary<string, CteDefinition> cteDefinitionsByName,
            ScriptScanResult result,
            HashSet<string> visitedQueryExpressionIds,
            HashSet<string> visitedQuerySpecificationIds,
            HashSet<string> visitedTableReferenceIds)
        {
            if (string.IsNullOrWhiteSpace(tableReferenceId) || !visitedTableReferenceIds.Add(tableReferenceId))
            {
                return;
            }

            if (TryGetJoinParenthesizedTableReferenceId(tableReferenceId, out var joinParenthesizedTableReferenceId))
            {
                ScanTableReference(
                    queryExpressionId,
                    querySpecificationId,
                    joinParenthesizedTableReferenceId,
                    scope.WithPathSegment("JoinParenthesis"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds);
                return;
            }

            if (joinByTableReferenceId.TryGetValue(tableReferenceId, out var joinBase))
            {
                TryGetJoinChildTableReferenceIds(
                    joinBase.Id,
                    out var firstTableReferenceId,
                    out var secondTableReferenceId);

                if (qualifiedJoinByJoinId.TryGetValue(joinBase.Id, out var qualifiedJoin))
                {
                    var searchConditionBooleanExpressionId = qualifiedJoinPredicateByJoinId.TryGetValue(qualifiedJoin.Id, out var predicateLink)
                        ? predicateLink.BooleanExpression.Id
                        : string.Empty;
                    if (!string.IsNullOrWhiteSpace(searchConditionBooleanExpressionId))
                    {
                        ScanBooleanExpression(
                            searchConditionBooleanExpressionId,
                            scope.WithPathSegment("JoinPredicate"),
                            cteDefinitionsByName,
                            result,
                            visitedQueryExpressionIds,
                            visitedQuerySpecificationIds,
                            visitedTableReferenceIds,
                            new HashSet<string>(StringComparer.Ordinal),
                            new HashSet<string>(StringComparer.Ordinal),
                            new HashSet<string>(StringComparer.Ordinal));
                    }

                    var filterPredicates = CollectQueryFilterPredicates(querySpecificationId);

                    var equalityPredicates = CollectEqualityPredicates(searchConditionBooleanExpressionId);
                    var projectsRightDetailColumn = ProjectsNonKeyColumnFromJoinSide(
                        querySpecificationId,
                        secondTableReferenceId,
                        equalityPredicates);

                    var baseTables = new List<BaseTableEvidence>();
                    ResolveJoinInputBaseTables(
                        firstTableReferenceId,
                        scope.WithPathSegment("JoinFirstInput"),
                        cteDefinitionsByName,
                        baseTables);
                    ResolveJoinInputBaseTables(
                        secondTableReferenceId,
                        scope.WithPathSegment("JoinSecondInput"),
                        cteDefinitionsByName,
                        baseTables);

                    result.AddJoinLocation(new JoinLocationEvidence
                    {
                        QueryExpressionId = queryExpressionId,
                        QuerySpecificationId = querySpecificationId,
                        JoinTableReferenceId = joinBase.Id,
                        QualifiedJoinId = qualifiedJoin.Id,
                        QualifiedJoinType = qualifiedJoin.QualifiedJoinType,
                        SearchConditionBooleanExpressionId = searchConditionBooleanExpressionId,
                        EqualityPredicateCount = equalityPredicates.Count,
                        EqualityPredicates = equalityPredicates,
                        FirstTableReferenceId = firstTableReferenceId,
                        SecondTableReferenceId = secondTableReferenceId,
                        ContainsEqualityPredicate = equalityPredicates.Count > 0,
                        ProjectsRightDetailColumn = projectsRightDetailColumn,
                        ScopePath = scope.Path,
                        CteId = scope.CteId,
                        CteName = scope.CteName,
                        BaseTables = baseTables,
                        FilterPredicates = filterPredicates,
                    });
                }

                if (unqualifiedJoinByJoinId.TryGetValue(joinBase.Id, out _))
                {
                    // Recognized but not promoted in stage 1.
                }

                ScanJoinChildTableReference(
                    queryExpressionId,
                    querySpecificationId,
                    firstTableReferenceId,
                    scope.WithPathSegment("JoinFirst"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds);
                ScanJoinChildTableReference(
                    queryExpressionId,
                    querySpecificationId,
                    secondTableReferenceId,
                    scope.WithPathSegment("JoinSecond"),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds);

                return;
            }

            if (TryGetDerivedQueryExpression(tableReferenceId, out var derivedQueryExpressionId, out var aliasName))
            {
                ScanQueryExpression(
                    derivedQueryExpressionId,
                    WithDerivedScope(scope, aliasName),
                    cteDefinitionsByName,
                    result,
                    visitedQueryExpressionIds,
                    visitedQuerySpecificationIds,
                    visitedTableReferenceIds);
            }
        }

        private bool ProjectsNonKeyColumnFromJoinSide(
            string querySpecificationId,
            string joinSideTableReferenceId,
            IReadOnlyList<EqualityPredicateEvidence> equalityPredicates)
        {
            var sideAliases = ResolveTableReferenceAliases(joinSideTableReferenceId);
            if (sideAliases.Count == 0)
            {
                return false;
            }

            var keyColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var predicate in equalityPredicates)
            {
                AddSideKeyColumn(predicate.FirstExpressionDisplay, sideAliases, keyColumns);
                AddSideKeyColumn(predicate.SecondExpressionDisplay, sideAliases, keyColumns);
            }

            foreach (var projectedExpression in ResolveProjectedScalarExpressionDisplays(querySpecificationId))
            {
                if (!TryParseQualifiedColumnDisplay(projectedExpression, out var alias, out var column))
                {
                    continue;
                }

                if (sideAliases.Contains(alias) && !keyColumns.Contains(column))
                {
                    return true;
                }
            }

            return false;
        }

        private static void AddSideKeyColumn(
            string expression,
            ISet<string> sideAliases,
            ISet<string> keyColumns)
        {
            if (TryParseQualifiedColumnDisplay(expression, out var alias, out var column)
                && sideAliases.Contains(alias)
                && !string.IsNullOrWhiteSpace(column))
            {
                keyColumns.Add(column);
            }
        }

        private static bool TryParseQualifiedColumnDisplay(
            string expression,
            out string alias,
            out string column)
        {
            alias = string.Empty;
            column = string.Empty;
            if (string.IsNullOrWhiteSpace(expression))
            {
                return false;
            }

            var parts = expression
                .Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(static part => UnquoteIdentifier(part))
                .Where(static part => !string.IsNullOrWhiteSpace(part))
                .ToArray();
            if (parts.Length < 2)
            {
                return false;
            }

            alias = parts[^2];
            column = parts[^1];
            return !string.IsNullOrWhiteSpace(alias) && !string.IsNullOrWhiteSpace(column);
        }

        private static string UnquoteIdentifier(string value)
        {
            var trimmed = value.Trim();
            if (trimmed.Length >= 2 && trimmed[0] == '[' && trimmed[^1] == ']')
            {
                return trimmed[1..^1].Replace("]]", "]", StringComparison.Ordinal);
            }

            return trimmed;
        }

        private IReadOnlyList<string> ResolveProjectedScalarExpressionDisplays(string querySpecificationId)
        {
            if (!selectElementItemsByQuerySpecificationId.TryGetValue(querySpecificationId, out var selectElementItems))
            {
                return [];
            }

            return selectElementItems
                .OrderBy(static item => ParseOrdinalOrMax(item.Ordinal))
                .Select(item => selectScalarBySelectElementId.TryGetValue(item.SelectElement.Id, out var selectScalar)
                    && selectScalarExpressionLinkBySelectScalarId.TryGetValue(selectScalar.Id, out var expressionLink)
                        ? ResolveScalarExpressionDisplay(expressionLink.ScalarExpression.Id)
                        : string.Empty)
                .Where(static value => !string.IsNullOrWhiteSpace(value))
                .ToArray();
        }

        private HashSet<string> ResolveTableReferenceAliases(string tableReferenceId)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            CollectTableReferenceAliases(tableReferenceId, result, new HashSet<string>(StringComparer.Ordinal));
            return result;
        }

        private void CollectTableReferenceAliases(
            string tableReferenceId,
            ISet<string> aliases,
            ISet<string> visitedTableReferenceIds)
        {
            if (string.IsNullOrWhiteSpace(tableReferenceId) || !visitedTableReferenceIds.Add(tableReferenceId))
            {
                return;
            }

            if (TryGetJoinParenthesizedTableReferenceId(tableReferenceId, out var joinParenthesizedTableReferenceId))
            {
                CollectTableReferenceAliases(joinParenthesizedTableReferenceId, aliases, visitedTableReferenceIds);
                return;
            }

            if (joinByTableReferenceId.TryGetValue(tableReferenceId, out var joinBase))
            {
                TryGetJoinChildTableReferenceIds(
                    joinBase.Id,
                    out var firstTableReferenceId,
                    out var secondTableReferenceId);
                CollectTableReferenceAliases(firstTableReferenceId, aliases, visitedTableReferenceIds);
                CollectTableReferenceAliases(secondTableReferenceId, aliases, visitedTableReferenceIds);
                return;
            }

            if (TryGetTableReferenceAlias(tableReferenceId, out var aliasName))
            {
                aliases.Add(aliasName);
            }

            if (TryResolveNamedTableReference(
                    tableReferenceId,
                    out _,
                    out _,
                    out _,
                    out var objectBaseName,
                    out _)
                && !string.IsNullOrWhiteSpace(objectBaseName))
            {
                aliases.Add(objectBaseName);
            }
        }

        private bool TryGetTableReferenceAlias(string tableReferenceId, out string aliasName)
        {
            aliasName = string.Empty;
            if (!tableReferenceWithAliasByTableReferenceId.TryGetValue(tableReferenceId, out var aliasBase))
            {
                return false;
            }

            if (!tableReferenceAliasByAliasId.TryGetValue(aliasBase.Id, out var aliasLink))
            {
                return false;
            }

            if (!identifierById.TryGetValue(aliasLink.Identifier.Id, out var identifier)
                || string.IsNullOrWhiteSpace(identifier.Value))
            {
                return false;
            }

            aliasName = identifier.Value;
            return true;
        }

        private void ResolveJoinInputBaseTables(
            string joinInputTableReferenceId,
            QueryScope scope,
            IReadOnlyDictionary<string, CteDefinition> cteDefinitionsByName,
            List<BaseTableEvidence> baseTables)
        {
            if (string.IsNullOrWhiteSpace(joinInputTableReferenceId))
            {
                return;
            }

            ResolveBaseTables(
                joinInputTableReferenceId,
                joinInputTableReferenceId,
                scope,
                cteDefinitionsByName,
                baseTables,
                new HashSet<string>(StringComparer.Ordinal),
                new HashSet<string>(StringComparer.Ordinal),
                new HashSet<string>(StringComparer.Ordinal),
                depth: 0);
        }

        private void ScanJoinChildTableReference(
            string queryExpressionId,
            string querySpecificationId,
            string childTableReferenceId,
            QueryScope scope,
            IReadOnlyDictionary<string, CteDefinition> cteDefinitionsByName,
            ScriptScanResult result,
            HashSet<string> visitedQueryExpressionIds,
            HashSet<string> visitedQuerySpecificationIds,
            HashSet<string> visitedTableReferenceIds)
        {
            if (string.IsNullOrWhiteSpace(childTableReferenceId))
            {
                return;
            }

            ScanTableReference(
                queryExpressionId,
                querySpecificationId,
                childTableReferenceId,
                scope,
                cteDefinitionsByName,
                result,
                visitedQueryExpressionIds,
                visitedQuerySpecificationIds,
                visitedTableReferenceIds);
        }

        private void ResolveJoinChildBaseTables(
            string childTableReferenceId,
            string joinInputTableReferenceId,
            QueryScope scope,
            IReadOnlyDictionary<string, CteDefinition> cteDefinitionsByName,
            List<BaseTableEvidence> results,
            HashSet<string> visitedTableReferenceIds,
            HashSet<string> visitedQueryExpressionIds,
            HashSet<string> visitedCteIds,
            int depth)
        {
            if (string.IsNullOrWhiteSpace(childTableReferenceId))
            {
                return;
            }

            ResolveBaseTables(
                childTableReferenceId,
                joinInputTableReferenceId,
                scope,
                cteDefinitionsByName,
                results,
                visitedTableReferenceIds,
                visitedQueryExpressionIds,
                visitedCteIds,
                depth);
        }

        private void ResolveBaseTables(
            string tableReferenceId,
            string joinInputTableReferenceId,
            QueryScope scope,
            IReadOnlyDictionary<string, CteDefinition> cteDefinitionsByName,
            List<BaseTableEvidence> results,
            HashSet<string> visitedTableReferenceIds,
            HashSet<string> visitedQueryExpressionIds,
            HashSet<string> visitedCteIds,
            int depth)
        {
            if (string.IsNullOrWhiteSpace(tableReferenceId) || !visitedTableReferenceIds.Add(tableReferenceId))
            {
                return;
            }

            if (TryGetJoinParenthesizedTableReferenceId(tableReferenceId, out var joinParenthesizedTableReferenceId))
            {
                ResolveBaseTables(
                    joinParenthesizedTableReferenceId,
                    joinInputTableReferenceId,
                    scope.WithPathSegment("JoinParenthesis"),
                    cteDefinitionsByName,
                    results,
                    visitedTableReferenceIds,
                    visitedQueryExpressionIds,
                    visitedCteIds,
                    depth + 1);
                return;
            }

            if (joinByTableReferenceId.TryGetValue(tableReferenceId, out var joinBase))
            {
                TryGetJoinChildTableReferenceIds(
                    joinBase.Id,
                    out var firstTableReferenceId,
                    out var secondTableReferenceId);

                ResolveJoinChildBaseTables(
                    firstTableReferenceId,
                    joinInputTableReferenceId,
                    scope.WithPathSegment("JoinFirst"),
                    cteDefinitionsByName,
                    results,
                    visitedTableReferenceIds,
                    visitedQueryExpressionIds,
                    visitedCteIds,
                    depth + 1);

                ResolveJoinChildBaseTables(
                    secondTableReferenceId,
                    joinInputTableReferenceId,
                    scope.WithPathSegment("JoinSecond"),
                    cteDefinitionsByName,
                    results,
                    visitedTableReferenceIds,
                    visitedQueryExpressionIds,
                    visitedCteIds,
                    depth + 1);

                return;
            }

            if (TryResolveNamedTableReference(
                    tableReferenceId,
                    out var namedTableReferenceId,
                    out var schemaObjectNameId,
                    out var objectName,
                    out var objectBaseName,
                    out var objectPartCount))
            {
                if (objectPartCount == 1
                    && cteDefinitionsByName.TryGetValue(objectBaseName, out var cteDefinition)
                    && !string.IsNullOrWhiteSpace(cteDefinition.QueryExpressionId)
                    && visitedCteIds.Add(cteDefinition.CteId))
                {
                    ResolveBaseTablesFromQueryExpression(
                        cteDefinition.QueryExpressionId,
                        joinInputTableReferenceId,
                        scope.WithPathSegment($"RefCTE:{cteDefinition.Name}").WithCte(cteDefinition.CteId, cteDefinition.Name),
                        cteDefinitionsByName,
                        results,
                        visitedTableReferenceIds,
                        visitedQueryExpressionIds,
                        visitedCteIds,
                        depth + 1);
                    return;
                }

                results.Add(new BaseTableEvidence
                {
                    JoinInputTableReferenceId = joinInputTableReferenceId,
                    BaseTableReferenceId = tableReferenceId,
                    BaseNamedTableReferenceId = namedTableReferenceId,
                    BaseSchemaObjectNameId = schemaObjectNameId,
                    BaseObjectName = objectName,
                    ResolutionDepth = depth,
                    ResolutionPath = scope.Path,
                    ResolvedInCteId = scope.CteId,
                    ResolvedInCteName = scope.CteName,
                });
                return;
            }

            if (TryGetDerivedQueryExpression(tableReferenceId, out var derivedQueryExpressionId, out var aliasName))
            {
                ResolveBaseTablesFromQueryExpression(
                    derivedQueryExpressionId,
                    joinInputTableReferenceId,
                    WithDerivedScope(scope, aliasName),
                    cteDefinitionsByName,
                    results,
                    visitedTableReferenceIds,
                    visitedQueryExpressionIds,
                    visitedCteIds,
                    depth + 1);
            }
        }

        private void ResolveBaseTablesFromQueryExpression(
            string queryExpressionId,
            string joinInputTableReferenceId,
            QueryScope scope,
            IReadOnlyDictionary<string, CteDefinition> cteDefinitionsByName,
            List<BaseTableEvidence> results,
            HashSet<string> visitedTableReferenceIds,
            HashSet<string> visitedQueryExpressionIds,
            HashSet<string> visitedCteIds,
            int depth)
        {
            if (string.IsNullOrWhiteSpace(queryExpressionId) || !visitedQueryExpressionIds.Add(queryExpressionId))
            {
                return;
            }

            if (querySpecificationByQueryExpressionId.TryGetValue(queryExpressionId, out var querySpecification))
            {
                foreach (var tableReferenceId in GetOrderedFromTableReferenceIds(querySpecification.Id))
                {
                    ResolveBaseTables(
                        tableReferenceId,
                        joinInputTableReferenceId,
                        scope.WithPathSegment("From"),
                        cteDefinitionsByName,
                        results,
                        visitedTableReferenceIds,
                        visitedQueryExpressionIds,
                        visitedCteIds,
                        depth + 1);
                }
            }

            ResolveBaseTablesFromQueryExpressionChildren(
                queryExpressionId,
                joinInputTableReferenceId,
                scope,
                cteDefinitionsByName,
                results,
                visitedTableReferenceIds,
                visitedQueryExpressionIds,
                visitedCteIds,
                depth);
        }

        private void ResolveBaseTablesFromQueryExpressionChildren(
            string queryExpressionId,
            string joinInputTableReferenceId,
            QueryScope scope,
            IReadOnlyDictionary<string, CteDefinition> cteDefinitionsByName,
            List<BaseTableEvidence> results,
            HashSet<string> visitedTableReferenceIds,
            HashSet<string> visitedQueryExpressionIds,
            HashSet<string> visitedCteIds,
            int depth)
        {
            if (binaryQueryByQueryExpressionId.TryGetValue(queryExpressionId, out var binaryQuery))
            {
                if (binaryFirstQueryByOwnerId.TryGetValue(binaryQuery.Id, out var first))
                {
                    ResolveBaseTablesFromQueryExpression(
                        first.QueryExpression.Id,
                        joinInputTableReferenceId,
                        scope.WithPathSegment("BinaryFirst"),
                        cteDefinitionsByName,
                        results,
                        visitedTableReferenceIds,
                        visitedQueryExpressionIds,
                        visitedCteIds,
                        depth + 1);
                }

                if (binarySecondQueryByOwnerId.TryGetValue(binaryQuery.Id, out var second))
                {
                    ResolveBaseTablesFromQueryExpression(
                        second.QueryExpression.Id,
                        joinInputTableReferenceId,
                        scope.WithPathSegment("BinarySecond"),
                        cteDefinitionsByName,
                        results,
                        visitedTableReferenceIds,
                        visitedQueryExpressionIds,
                        visitedCteIds,
                        depth + 1);
                }
            }

            if (queryParenthesisByQueryExpressionId.TryGetValue(queryExpressionId, out var queryParenthesis)
                && queryParenthesisLinkByOwnerId.TryGetValue(queryParenthesis.Id, out var queryParenthesisLink))
            {
                ResolveBaseTablesFromQueryExpression(
                    queryParenthesisLink.QueryExpression.Id,
                    joinInputTableReferenceId,
                    scope.WithPathSegment("QueryParenthesis"),
                    cteDefinitionsByName,
                    results,
                    visitedTableReferenceIds,
                    visitedQueryExpressionIds,
                    visitedCteIds,
                    depth + 1);
            }
        }

        private bool TryResolveNamedTableReference(
            string tableReferenceId,
            out string namedTableReferenceId,
            out string schemaObjectNameId,
            out string objectName,
            out string objectBaseName,
            out int objectPartCount)
        {
            namedTableReferenceId = string.Empty;
            schemaObjectNameId = string.Empty;
            objectName = string.Empty;
            objectBaseName = string.Empty;
            objectPartCount = 0;

            if (!tableReferenceWithAliasByTableReferenceId.TryGetValue(tableReferenceId, out var aliasBase))
            {
                return false;
            }

            if (!namedTableByAliasId.TryGetValue(aliasBase.Id, out var namedTable))
            {
                return false;
            }

            if (!namedTableSchemaObjectByNamedTableId.TryGetValue(namedTable.Id, out var schemaObjectLink))
            {
                return false;
            }

            if (!ResolveSchemaObjectName(
                    schemaObjectLink.SchemaObjectName.Id,
                    out objectName,
                    out objectBaseName,
                    out objectPartCount))
            {
                return false;
            }

            namedTableReferenceId = namedTable.Id;
            schemaObjectNameId = schemaObjectLink.SchemaObjectName.Id;
            return true;
        }

        private bool ResolveSchemaObjectName(
            string schemaObjectNameId,
            out string fullName,
            out string baseName,
            out int partCount)
        {
            fullName = string.Empty;
            baseName = string.Empty;
            partCount = 0;

            if (!schemaObjectById.TryGetValue(schemaObjectNameId, out var schemaObject))
            {
                return false;
            }

            if (!multiPartIdentifierById.TryGetValue(schemaObject.MultiPartIdentifier.Id, out var multiPartIdentifier))
            {
                return false;
            }

            if (!TryGetMultiPartIdentifierValues(multiPartIdentifier.Id, out var parts))
            {
                return false;
            }
            if (parts.Length == 0)
            {
                return false;
            }

            fullName = string.Join(".", parts);
            baseName = parts[^1];
            partCount = parts.Length;
            return true;
        }

        private string ResolveScalarExpressionDisplay(string scalarExpressionId)
        {
            if (string.IsNullOrWhiteSpace(scalarExpressionId))
            {
                return string.Empty;
            }

            return ResolveScalarExpressionDisplayRecursive(
                scalarExpressionId,
                new HashSet<string>(StringComparer.Ordinal));
        }

        private string ResolveScalarExpressionDisplayRecursive(
            string scalarExpressionId,
            HashSet<string> visitedScalarExpressionIds)
        {
            if (string.IsNullOrWhiteSpace(scalarExpressionId) || !visitedScalarExpressionIds.Add(scalarExpressionId))
            {
                return scalarExpressionId;
            }

            if (TryGetUnaryInnerScalarExpression(scalarExpressionId, out var unaryExpressionType, out var unaryInnerScalarExpressionId))
            {
                var inner = ResolveScalarExpressionDisplayRecursive(unaryInnerScalarExpressionId, visitedScalarExpressionIds);
                if (!string.IsNullOrWhiteSpace(inner))
                {
                    return string.IsNullOrWhiteSpace(unaryExpressionType)
                        ? inner
                        : $"{unaryExpressionType} {inner}";
                }
            }

            if (TryGetBinaryScalarExpressionChildren(
                    scalarExpressionId,
                    out var binaryExpressionType,
                    out var binaryFirstScalarExpressionId,
                    out var binarySecondScalarExpressionId))
            {
                var left = ResolveScalarExpressionDisplayRecursive(binaryFirstScalarExpressionId, visitedScalarExpressionIds);
                var right = ResolveScalarExpressionDisplayRecursive(binarySecondScalarExpressionId, visitedScalarExpressionIds);
                if (!string.IsNullOrWhiteSpace(left) && !string.IsNullOrWhiteSpace(right))
                {
                    return $"{left} {binaryExpressionType} {right}".Trim();
                }
            }

            if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpressionId, out var primaryExpression))
            {
                return scalarExpressionId;
            }

            if (TryResolvePrimaryExpressionDisplay(primaryExpression.Id, out var primaryDisplay))
            {
                return primaryDisplay;
            }

            if (TryGetParenthesizedScalarExpression(primaryExpression.Id, out var parenthesizedScalarExpressionId))
            {
                var inner = ResolveScalarExpressionDisplayRecursive(parenthesizedScalarExpressionId, visitedScalarExpressionIds);
                if (!string.IsNullOrWhiteSpace(inner))
                {
                    return $"({inner})";
                }
            }

            return scalarExpressionId;
        }

        private bool TryGetUnaryInnerScalarExpression(
            string scalarExpressionId,
            out string unaryExpressionType,
            out string innerScalarExpressionId)
        {
            unaryExpressionType = string.Empty;
            innerScalarExpressionId = string.Empty;
            if (!unaryByScalarExpressionId.TryGetValue(scalarExpressionId, out var unary))
            {
                return false;
            }

            if (!unaryInnerByUnaryExpressionId.TryGetValue(unary.Id, out var unaryInner))
            {
                return false;
            }

            unaryExpressionType = unary.UnaryExpressionType;
            innerScalarExpressionId = unaryInner.ScalarExpression.Id;
            return !string.IsNullOrWhiteSpace(innerScalarExpressionId);
        }

        private bool TryGetBinaryScalarExpressionChildren(
            string scalarExpressionId,
            out string binaryExpressionType,
            out string firstScalarExpressionId,
            out string secondScalarExpressionId)
        {
            binaryExpressionType = string.Empty;
            firstScalarExpressionId = string.Empty;
            secondScalarExpressionId = string.Empty;
            if (!binaryByScalarExpressionId.TryGetValue(scalarExpressionId, out var binary))
            {
                return false;
            }

            if (!binaryFirstByBinaryExpressionId.TryGetValue(binary.Id, out var first)
                || !binarySecondByBinaryExpressionId.TryGetValue(binary.Id, out var second))
            {
                return false;
            }

            binaryExpressionType = binary.BinaryExpressionType;
            firstScalarExpressionId = first.ScalarExpression.Id;
            secondScalarExpressionId = second.ScalarExpression.Id;
            return !string.IsNullOrWhiteSpace(firstScalarExpressionId)
                   && !string.IsNullOrWhiteSpace(secondScalarExpressionId);
        }

        private bool TryResolvePrimaryExpressionDisplay(
            string primaryExpressionId,
            out string display)
        {
            display = string.Empty;
            if (columnReferenceByPrimaryExpressionId.TryGetValue(primaryExpressionId, out var columnReference)
                && columnReferenceMultiPartByColumnReferenceId.TryGetValue(columnReference.Id, out var multiPartLink))
            {
                var columnName = ResolveMultiPartIdentifierDisplay(multiPartLink.MultiPartIdentifier.Id);
                if (!string.IsNullOrWhiteSpace(columnName))
                {
                    display = columnName;
                    return true;
                }
            }

            if (identifierOrValueByPrimaryExpressionId.TryGetValue(primaryExpressionId, out var identifierOrValue))
            {
                if (identifierOrValueIdentifierByOwnerId.TryGetValue(identifierOrValue.Id, out var identifierLink)
                    && identifierById.TryGetValue(identifierLink.Identifier.Id, out var identifier)
                    && !string.IsNullOrWhiteSpace(identifier.Value))
                {
                    display = identifier.Value;
                    return true;
                }

                if (!string.IsNullOrWhiteSpace(identifierOrValue.Value))
                {
                    display = identifierOrValue.Value;
                    return true;
                }
            }

            return false;
        }

        private bool TryGetParenthesizedScalarExpression(
            string primaryExpressionId,
            out string innerScalarExpressionId)
        {
            innerScalarExpressionId = string.Empty;
            if (!parenthesisByPrimaryExpressionId.TryGetValue(primaryExpressionId, out var parenthesis))
            {
                return false;
            }

            if (!parenthesisInnerByParenthesisExpressionId.TryGetValue(parenthesis.Id, out var parenthesisInner))
            {
                return false;
            }

            innerScalarExpressionId = parenthesisInner.ScalarExpression.Id;
            return !string.IsNullOrWhiteSpace(innerScalarExpressionId);
        }

        private string ResolveMultiPartIdentifierDisplay(string multiPartIdentifierId)
        {
            if (string.IsNullOrWhiteSpace(multiPartIdentifierId))
            {
                return string.Empty;
            }

            if (!TryGetMultiPartIdentifierValues(multiPartIdentifierId, out var parts))
            {
                return string.Empty;
            }
            return parts.Length == 0
                ? string.Empty
                : string.Join(".", parts);
        }

        private bool TryGetMultiPartIdentifierValues(
            string multiPartIdentifierId,
            out string[] parts)
        {
            parts = [];
            if (!multiPartIdentifierItemsByMultiPartId.TryGetValue(multiPartIdentifierId, out var items))
            {
                return false;
            }

            parts = items
                .OrderBy(static item => ParseOrdinalOrMax(item.Ordinal))
                .Select(item => identifierById.TryGetValue(item.Identifier.Id, out var identifier) ? identifier.Value ?? string.Empty : string.Empty)
                .Where(static value => !string.IsNullOrWhiteSpace(value))
                .ToArray();
            return true;
        }

        private bool TryResolveKnownScalarFunction(
            FunctionCall functionCall,
            out ScriptObjectScalarFunction scalarFunction)
        {
            scalarFunction = null!;
            if (!functionCallNameByFunctionCallId.TryGetValue(functionCall.Id, out var nameLink) ||
                !identifierById.TryGetValue(nameLink.Identifier.Id, out var nameIdentifier) ||
                string.IsNullOrWhiteSpace(nameIdentifier.Value))
            {
                return false;
            }

            var nameParts = GetFunctionCallCallTargetParts(functionCall)
                .Concat([nameIdentifier.Value])
                .ToArray();
            var qualifiedName = NormalizeSqlIdentifierPath(nameParts);
            if (!string.IsNullOrWhiteSpace(qualifiedName) &&
                scalarFunctionByQualifiedName.TryGetValue(qualifiedName, out scalarFunction))
            {
                return true;
            }

            var unqualifiedName = NormalizeSqlIdentifierPart(nameIdentifier.Value);
            return !string.IsNullOrWhiteSpace(unqualifiedName) &&
                   scalarFunctionByUnqualifiedName.TryGetValue(unqualifiedName, out scalarFunction);
        }

        private IReadOnlyList<string> GetFunctionCallCallTargetParts(FunctionCall functionCall)
        {
            if (!functionCallTargetByFunctionCallId.TryGetValue(functionCall.Id, out var callTargetLink) ||
                !multiPartCallTargetByCallTargetId.TryGetValue(callTargetLink.CallTarget.Id, out var multiPartCallTarget) ||
                !multiPartCallTargetIdentifierByOwnerId.TryGetValue(multiPartCallTarget.Id, out var multiPartIdentifierLink) ||
                !TryGetMultiPartIdentifierValues(multiPartIdentifierLink.MultiPartIdentifier.Id, out var parts))
            {
                return [];
            }

            return parts;
        }

        private static string NormalizeSqlIdentifierName(string name)
        {
            return NormalizeSqlIdentifierPath(SplitSqlIdentifierName(name));
        }

        private static string NormalizeSqlIdentifierLeafName(string name)
        {
            var parts = SplitSqlIdentifierName(name);
            return parts.Length == 0
                ? string.Empty
                : NormalizeSqlIdentifierPart(parts[^1]);
        }

        private static string[] SplitSqlIdentifierName(string name)
        {
            return name
                .Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(NormalizeSqlIdentifierPart)
                .Where(static part => !string.IsNullOrWhiteSpace(part))
                .ToArray();
        }

        private static string NormalizeSqlIdentifierPath(IEnumerable<string> parts)
        {
            return string.Join(
                ".",
                parts
                    .Select(NormalizeSqlIdentifierPart)
                    .Where(static part => !string.IsNullOrWhiteSpace(part)));
        }

        private static string NormalizeSqlIdentifierPart(string part)
        {
            var trimmed = part.Trim();
            if (trimmed.Length >= 2)
            {
                if ((trimmed[0] == '[' && trimmed[^1] == ']') ||
                    (trimmed[0] == '"' && trimmed[^1] == '"'))
                {
                    trimmed = trimmed[1..^1];
                }
            }

            return trimmed;
        }

        private bool TryGetDerivedQueryExpression(
            string tableReferenceId,
            out string queryExpressionId,
            out string aliasName)
        {
            queryExpressionId = string.Empty;
            aliasName = string.Empty;

            if (!tableReferenceWithAliasByTableReferenceId.TryGetValue(tableReferenceId, out var aliasBase))
            {
                return false;
            }

            if (tableReferenceAliasByAliasId.TryGetValue(aliasBase.Id, out var aliasLink)
                && identifierById.TryGetValue(aliasLink.Identifier.Id, out var aliasIdentifier))
            {
                aliasName = aliasIdentifier.Value;
            }

            if (!tableReferenceWithAliasAndColumnsByAliasId.TryGetValue(aliasBase.Id, out var aliasAndColumns))
            {
                return false;
            }

            if (!queryDerivedByAliasAndColumnsId.TryGetValue(aliasAndColumns.Id, out var queryDerived))
            {
                return false;
            }

            if (!queryDerivedQueryByOwnerId.TryGetValue(queryDerived.Id, out var queryDerivedLink))
            {
                return false;
            }

            queryExpressionId = queryDerivedLink.QueryExpression.Id;
            return !string.IsNullOrWhiteSpace(queryExpressionId);
        }

        private List<FilterPredicateEvidence> CollectQueryFilterPredicates(string querySpecificationId)
        {
            if (!TryResolveWhereBooleanExpressionId(querySpecificationId, out var whereBooleanExpressionId))
            {
                return [];
            }

            var aliasToBaseObjectName = BuildAliasToBaseObjectNameMap(querySpecificationId);
            if (aliasToBaseObjectName.Count == 0)
            {
                return [];
            }

            var result = new List<FilterPredicateEvidence>();
            CollectFilterPredicatesRecursive(
                whereBooleanExpressionId,
                aliasToBaseObjectName,
                new HashSet<string>(StringComparer.Ordinal),
                result);

            return result
                .Where(static row => !string.IsNullOrWhiteSpace(row.BaseObjectName)
                                     && !string.IsNullOrWhiteSpace(row.PredicateSignature)
                                     && !string.IsNullOrWhiteSpace(row.PredicateDisplay))
                .GroupBy(
                    static row => $"{row.BaseObjectName}|{row.PredicateSignature}",
                    StringComparer.Ordinal)
                .Select(static group => group.First())
                .OrderBy(static row => row.BaseObjectName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(static row => row.PredicateSignature, StringComparer.Ordinal)
                .ToList();
        }

        private bool TryResolveWhereBooleanExpressionId(string querySpecificationId, out string whereBooleanExpressionId)
        {
            whereBooleanExpressionId = string.Empty;
            if (!whereClauseLinkByQuerySpecificationId.TryGetValue(querySpecificationId, out var whereClauseLink))
            {
                return false;
            }

            if (!whereClauseSearchConditionByWhereClauseId.TryGetValue(whereClauseLink.WhereClause.Id, out var whereSearchCondition))
            {
                return false;
            }

            whereBooleanExpressionId = whereSearchCondition.BooleanExpression.Id;
            return !string.IsNullOrWhiteSpace(whereBooleanExpressionId);
        }

        private Dictionary<string, string> BuildAliasToBaseObjectNameMap(string querySpecificationId)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var ambiguousAliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var tableReferenceId in GetOrderedFromTableReferenceIds(querySpecificationId))
            {
                CollectAliasToBaseObjectNames(
                    tableReferenceId,
                    map,
                    ambiguousAliases,
                    new HashSet<string>(StringComparer.Ordinal));
            }

            return map;
        }

        private void CollectAliasToBaseObjectNames(
            string tableReferenceId,
            IDictionary<string, string> map,
            ISet<string> ambiguousAliases,
            ISet<string> visitedTableReferenceIds)
        {
            if (string.IsNullOrWhiteSpace(tableReferenceId) || !visitedTableReferenceIds.Add(tableReferenceId))
            {
                return;
            }

            if (TryGetJoinParenthesizedTableReferenceId(tableReferenceId, out var joinParenthesizedTableReferenceId))
            {
                CollectAliasToBaseObjectNames(
                    joinParenthesizedTableReferenceId,
                    map,
                    ambiguousAliases,
                    visitedTableReferenceIds);
                return;
            }

            if (joinByTableReferenceId.TryGetValue(tableReferenceId, out var joinBase))
            {
                TryGetJoinChildTableReferenceIds(
                    joinBase.Id,
                    out var firstTableReferenceId,
                    out var secondTableReferenceId);
                CollectAliasToBaseObjectNames(firstTableReferenceId, map, ambiguousAliases, visitedTableReferenceIds);
                CollectAliasToBaseObjectNames(secondTableReferenceId, map, ambiguousAliases, visitedTableReferenceIds);
                return;
            }

            if (TryResolveNamedTableReference(
                    tableReferenceId,
                    out _,
                    out _,
                    out var objectName,
                    out var objectBaseName,
                    out _)
                && !string.IsNullOrWhiteSpace(objectName))
            {
                if (TryGetTableReferenceAlias(tableReferenceId, out var aliasName))
                {
                    AddAliasToBaseObjectMap(map, ambiguousAliases, aliasName, objectName);
                }

                if (!string.IsNullOrWhiteSpace(objectBaseName))
                {
                    AddAliasToBaseObjectMap(map, ambiguousAliases, objectBaseName, objectName);
                }
            }
        }

        private static void AddAliasToBaseObjectMap(
            IDictionary<string, string> map,
            ISet<string> ambiguousAliases,
            string alias,
            string baseObjectName)
        {
            if (string.IsNullOrWhiteSpace(alias)
                || string.IsNullOrWhiteSpace(baseObjectName)
                || ambiguousAliases.Contains(alias))
            {
                return;
            }

            if (!map.TryGetValue(alias, out var existing))
            {
                map[alias] = baseObjectName;
                return;
            }

            if (!string.Equals(existing, baseObjectName, StringComparison.OrdinalIgnoreCase))
            {
                map.Remove(alias);
                ambiguousAliases.Add(alias);
            }
        }

        private void CollectFilterPredicatesRecursive(
            string booleanExpressionId,
            IReadOnlyDictionary<string, string> aliasToBaseObjectName,
            ISet<string> visitedBooleanExpressionIds,
            List<FilterPredicateEvidence> result)
        {
            if (string.IsNullOrWhiteSpace(booleanExpressionId) || !visitedBooleanExpressionIds.Add(booleanExpressionId))
            {
                return;
            }

            if (booleanComparisonByBooleanExpressionId.TryGetValue(booleanExpressionId, out var comparison))
            {
                if (booleanComparisonFirstByOwnerId.TryGetValue(comparison.Id, out var first)
                    && booleanComparisonSecondByOwnerId.TryGetValue(comparison.Id, out var second)
                    && TryCreateFilterPredicateEvidenceFromComparison(
                        comparison,
                        first.ScalarExpression.Id,
                        second.ScalarExpression.Id,
                        aliasToBaseObjectName,
                        out var filterPredicate))
                {
                    result.Add(filterPredicate);
                }

                return;
            }

            if (booleanIsNullByBooleanExpressionId.TryGetValue(booleanExpressionId, out var isNullExpression))
            {
                if (booleanIsNullExpressionLinkByOwnerId.TryGetValue(isNullExpression.Id, out var isNullLink)
                    && TryCreateFilterPredicateEvidenceFromIsNull(
                        isNullExpression,
                        isNullLink.ScalarExpression.Id,
                        aliasToBaseObjectName,
                        out var filterPredicate))
                {
                    result.Add(filterPredicate);
                }

                return;
            }

            if (TryGetBooleanBinaryChildren(booleanExpressionId, out var firstBooleanExpressionId, out var secondBooleanExpressionId))
            {
                if (!string.IsNullOrWhiteSpace(firstBooleanExpressionId))
                {
                    CollectFilterPredicatesRecursive(firstBooleanExpressionId, aliasToBaseObjectName, visitedBooleanExpressionIds, result);
                }

                if (!string.IsNullOrWhiteSpace(secondBooleanExpressionId))
                {
                    CollectFilterPredicatesRecursive(secondBooleanExpressionId, aliasToBaseObjectName, visitedBooleanExpressionIds, result);
                }

                return;
            }

            if (TryGetParenthesizedBooleanExpressionId(booleanExpressionId, out var parenthesizedBooleanExpressionId))
            {
                CollectFilterPredicatesRecursive(parenthesizedBooleanExpressionId, aliasToBaseObjectName, visitedBooleanExpressionIds, result);
                return;
            }

            if (TryGetNotBooleanExpressionId(booleanExpressionId, out var notBooleanExpressionId))
            {
                CollectFilterPredicatesRecursive(notBooleanExpressionId, aliasToBaseObjectName, visitedBooleanExpressionIds, result);
            }
        }

        private bool TryCreateFilterPredicateEvidenceFromComparison(
            BooleanComparisonExpression comparison,
            string firstScalarExpressionId,
            string secondScalarExpressionId,
            IReadOnlyDictionary<string, string> aliasToBaseObjectName,
            out FilterPredicateEvidence filterPredicate)
        {
            filterPredicate = default!;
            var firstDisplay = ResolveScalarExpressionDisplay(firstScalarExpressionId);
            var secondDisplay = ResolveScalarExpressionDisplay(secondScalarExpressionId);
            if (string.IsNullOrWhiteSpace(firstDisplay) || string.IsNullOrWhiteSpace(secondDisplay))
            {
                return false;
            }

            var firstIsColumn = TryResolveBaseObjectAndColumn(
                firstDisplay,
                aliasToBaseObjectName,
                out var firstBaseObjectName,
                out var firstColumnName);
            var secondIsColumn = TryResolveBaseObjectAndColumn(
                secondDisplay,
                aliasToBaseObjectName,
                out var secondBaseObjectName,
                out var secondColumnName);
            if (firstIsColumn == secondIsColumn)
            {
                return false;
            }

            var comparisonType = string.IsNullOrWhiteSpace(comparison.ComparisonType)
                ? "comparison"
                : CorpusInferenceNormalization.NormalizeSignaturePart(comparison.ComparisonType);
            if (firstIsColumn)
            {
                var rightPart = NormalizeFilterOperand(secondDisplay);
                if (string.IsNullOrWhiteSpace(rightPart))
                {
                    return false;
                }

                filterPredicate = new FilterPredicateEvidence
                {
                    BaseObjectName = firstBaseObjectName,
                    PredicateSignature = $"{CorpusInferenceNormalization.NormalizeSignaturePart(firstColumnName)} {comparisonType} {rightPart}",
                    PredicateDisplay = $"{firstDisplay} {comparison.ComparisonType} {secondDisplay}".Trim(),
                };
                return true;
            }

            var leftPart = NormalizeFilterOperand(firstDisplay);
            if (string.IsNullOrWhiteSpace(leftPart))
            {
                return false;
            }

            filterPredicate = new FilterPredicateEvidence
            {
                BaseObjectName = secondBaseObjectName,
                PredicateSignature = $"{CorpusInferenceNormalization.NormalizeSignaturePart(secondColumnName)} {comparisonType} {leftPart}",
                PredicateDisplay = $"{firstDisplay} {comparison.ComparisonType} {secondDisplay}".Trim(),
            };
            return true;
        }

        private bool TryCreateFilterPredicateEvidenceFromIsNull(
            BooleanIsNullExpression isNullExpression,
            string scalarExpressionId,
            IReadOnlyDictionary<string, string> aliasToBaseObjectName,
            out FilterPredicateEvidence filterPredicate)
        {
            filterPredicate = default!;
            var expressionDisplay = ResolveScalarExpressionDisplay(scalarExpressionId);
            if (!TryResolveBaseObjectAndColumn(
                    expressionDisplay,
                    aliasToBaseObjectName,
                    out var baseObjectName,
                    out var columnName))
            {
                return false;
            }

            var isNot = string.Equals(isNullExpression.IsNot, "true", StringComparison.OrdinalIgnoreCase);
            var nullToken = isNot ? "is not null" : "is null";
            filterPredicate = new FilterPredicateEvidence
            {
                BaseObjectName = baseObjectName,
                PredicateSignature = $"{CorpusInferenceNormalization.NormalizeSignaturePart(columnName)} {nullToken}",
                PredicateDisplay = $"{expressionDisplay} {(isNot ? "IS NOT NULL" : "IS NULL")}",
            };
            return true;
        }

        private bool TryResolveBaseObjectAndColumn(
            string expressionDisplay,
            IReadOnlyDictionary<string, string> aliasToBaseObjectName,
            out string baseObjectName,
            out string columnName)
        {
            baseObjectName = string.Empty;
            columnName = string.Empty;
            if (!TryParseQualifiedColumnDisplay(expressionDisplay, out var alias, out var column))
            {
                return false;
            }

            if (!aliasToBaseObjectName.TryGetValue(alias, out var resolvedBaseObjectName)
                || string.IsNullOrWhiteSpace(resolvedBaseObjectName))
            {
                return false;
            }

            baseObjectName = resolvedBaseObjectName;
            columnName = column;
            return !string.IsNullOrWhiteSpace(baseObjectName) && !string.IsNullOrWhiteSpace(columnName);
        }

        private static string NormalizeFilterOperand(string display)
        {
            if (string.IsNullOrWhiteSpace(display))
            {
                return string.Empty;
            }

            return CorpusInferenceNormalization.NormalizeSignaturePart(display);
        }

        private List<EqualityPredicateEvidence> CollectEqualityPredicates(string booleanExpressionId)
        {
            var result = new List<EqualityPredicateEvidence>();
            CollectEqualityPredicatesRecursive(
                booleanExpressionId,
                new HashSet<string>(StringComparer.Ordinal),
                result);
            return result;
        }

        private void CollectEqualityPredicatesRecursive(
            string booleanExpressionId,
            HashSet<string> visitedBooleanExpressionIds,
            List<EqualityPredicateEvidence> result)
        {
            if (string.IsNullOrWhiteSpace(booleanExpressionId) || !visitedBooleanExpressionIds.Add(booleanExpressionId))
            {
                return;
            }

            if (booleanComparisonByBooleanExpressionId.TryGetValue(booleanExpressionId, out var comparison))
            {
                if (string.Equals(comparison.ComparisonType, "Equals", StringComparison.OrdinalIgnoreCase)
                    && booleanComparisonFirstByOwnerId.TryGetValue(comparison.Id, out var first)
                    && booleanComparisonSecondByOwnerId.TryGetValue(comparison.Id, out var second))
                {
                    result.Add(new EqualityPredicateEvidence
                    {
                        BooleanComparisonExpressionId = comparison.Id,
                        FirstExpressionId = first.ScalarExpression.Id,
                        SecondExpressionId = second.ScalarExpression.Id,
                        FirstExpressionDisplay = ResolveScalarExpressionDisplay(first.ScalarExpression.Id),
                        SecondExpressionDisplay = ResolveScalarExpressionDisplay(second.ScalarExpression.Id),
                    });
                }

                return;
            }

            if (TryGetBooleanBinaryChildren(booleanExpressionId, out var firstBooleanExpressionId, out var secondBooleanExpressionId))
            {
                if (!string.IsNullOrWhiteSpace(firstBooleanExpressionId))
                {
                    CollectEqualityPredicatesRecursive(firstBooleanExpressionId, visitedBooleanExpressionIds, result);
                }

                if (!string.IsNullOrWhiteSpace(secondBooleanExpressionId))
                {
                    CollectEqualityPredicatesRecursive(secondBooleanExpressionId, visitedBooleanExpressionIds, result);
                }

                return;
            }

            if (TryGetParenthesizedBooleanExpressionId(booleanExpressionId, out var parenthesizedBooleanExpressionId))
            {
                CollectEqualityPredicatesRecursive(parenthesizedBooleanExpressionId, visitedBooleanExpressionIds, result);
                return;
            }

            if (TryGetNotBooleanExpressionId(booleanExpressionId, out var notBooleanExpressionId))
            {
                CollectEqualityPredicatesRecursive(notBooleanExpressionId, visitedBooleanExpressionIds, result);
            }
        }

        private bool TryGetBooleanBinaryChildren(
            string booleanExpressionId,
            out string firstBooleanExpressionId,
            out string secondBooleanExpressionId)
        {
            firstBooleanExpressionId = string.Empty;
            secondBooleanExpressionId = string.Empty;
            if (!booleanBinaryByBooleanExpressionId.TryGetValue(booleanExpressionId, out var binary))
            {
                return false;
            }

            firstBooleanExpressionId = booleanBinaryFirstByOwnerId.TryGetValue(binary.Id, out var first)
                ? first.BooleanExpression.Id
                : string.Empty;
            secondBooleanExpressionId = booleanBinarySecondByOwnerId.TryGetValue(binary.Id, out var second)
                ? second.BooleanExpression.Id
                : string.Empty;
            return !string.IsNullOrWhiteSpace(firstBooleanExpressionId)
                   || !string.IsNullOrWhiteSpace(secondBooleanExpressionId);
        }

        private bool TryGetParenthesizedBooleanExpressionId(
            string booleanExpressionId,
            out string innerBooleanExpressionId)
        {
            innerBooleanExpressionId = string.Empty;
            if (!booleanParenthesisByBooleanExpressionId.TryGetValue(booleanExpressionId, out var parenthesis))
            {
                return false;
            }

            if (!booleanParenthesisLinkByOwnerId.TryGetValue(parenthesis.Id, out var inner))
            {
                return false;
            }

            innerBooleanExpressionId = inner.BooleanExpression.Id;
            return !string.IsNullOrWhiteSpace(innerBooleanExpressionId);
        }

        private bool TryGetNotBooleanExpressionId(
            string booleanExpressionId,
            out string innerBooleanExpressionId)
        {
            innerBooleanExpressionId = string.Empty;
            if (!booleanNotByBooleanExpressionId.TryGetValue(booleanExpressionId, out var not))
            {
                return false;
            }

            if (!booleanNotLinkByOwnerId.TryGetValue(not.Id, out var inner))
            {
                return false;
            }

            innerBooleanExpressionId = inner.BooleanExpression.Id;
            return !string.IsNullOrWhiteSpace(innerBooleanExpressionId);
        }
    }

    private sealed class ScriptScanResult
    {
        private readonly Dictionary<string, JoinLocationEvidence> joinEvidenceByQualifiedJoinId =
            new(StringComparer.Ordinal);

        public int QualifiedJoinCount { get; private set; }

        public int QualifiedJoinWithEqualityCount { get; private set; }

        public int OuterJoinCount { get; private set; }

        public bool HasOuterJoin => OuterJoinCount > 0;

        public bool HasGroupBy { get; set; }

        public bool HasDistinct { get; set; }

        public IReadOnlyList<JoinLocationEvidence> JoinLocations => joinEvidenceByQualifiedJoinId.Values.ToArray();

        public void AddJoinLocation(JoinLocationEvidence evidence)
        {
            if (string.IsNullOrWhiteSpace(evidence.QualifiedJoinId))
            {
                return;
            }

            if (joinEvidenceByQualifiedJoinId.ContainsKey(evidence.QualifiedJoinId))
            {
                return;
            }

            joinEvidenceByQualifiedJoinId.Add(evidence.QualifiedJoinId, evidence);
            QualifiedJoinCount++;
            if (evidence.ContainsEqualityPredicate)
            {
                QualifiedJoinWithEqualityCount++;
            }

            if (IsOuterJoin(evidence.QualifiedJoinType))
            {
                OuterJoinCount++;
            }
        }
    }

    private sealed class ExtractedScriptEvidence
    {
        public TransformScript TransformScript { get; init; } = null!;

        public ScriptScanResult Scan { get; init; } = null!;
    }

    private sealed class JoinLocationEvidence
    {
        public string QueryExpressionId { get; init; } = string.Empty;

        public string QuerySpecificationId { get; init; } = string.Empty;

        public string JoinTableReferenceId { get; init; } = string.Empty;

        public string QualifiedJoinId { get; init; } = string.Empty;

        public string QualifiedJoinType { get; init; } = string.Empty;

        public string SearchConditionBooleanExpressionId { get; init; } = string.Empty;

        public int EqualityPredicateCount { get; init; }

        public IReadOnlyList<EqualityPredicateEvidence> EqualityPredicates { get; init; } = [];

        public string FirstTableReferenceId { get; init; } = string.Empty;

        public string SecondTableReferenceId { get; init; } = string.Empty;

        public bool ContainsEqualityPredicate { get; init; }

        public bool ProjectsRightDetailColumn { get; init; }

        public string ScopePath { get; init; } = string.Empty;

        public string CteId { get; init; } = string.Empty;

        public string CteName { get; init; } = string.Empty;

        public IReadOnlyList<BaseTableEvidence> BaseTables { get; init; } = [];

        public IReadOnlyList<FilterPredicateEvidence> FilterPredicates { get; init; } = [];
    }

    private sealed class BaseTableEvidence
    {
        public string JoinInputTableReferenceId { get; init; } = string.Empty;

        public string BaseTableReferenceId { get; init; } = string.Empty;

        public string BaseNamedTableReferenceId { get; init; } = string.Empty;

        public string BaseSchemaObjectNameId { get; init; } = string.Empty;

        public string BaseObjectName { get; init; } = string.Empty;

        public int ResolutionDepth { get; init; }

        public string ResolutionPath { get; init; } = string.Empty;

        public string ResolvedInCteId { get; init; } = string.Empty;

        public string ResolvedInCteName { get; init; } = string.Empty;
    }

    private sealed class EqualityPredicateEvidence
    {
        public string BooleanComparisonExpressionId { get; init; } = string.Empty;

        public string FirstExpressionId { get; init; } = string.Empty;

        public string SecondExpressionId { get; init; } = string.Empty;

        public string FirstExpressionDisplay { get; init; } = string.Empty;

        public string SecondExpressionDisplay { get; init; } = string.Empty;
    }

    private sealed class FilterPredicateEvidence
    {
        public string BaseObjectName { get; init; } = string.Empty;

        public string PredicateSignature { get; init; } = string.Empty;

        public string PredicateDisplay { get; init; } = string.Empty;
    }

    private sealed class CteDefinition
    {
        public string CteId { get; init; } = string.Empty;

        public string Name { get; init; } = string.Empty;

        public string QueryExpressionId { get; init; } = string.Empty;
    }

    private readonly record struct QueryScope(
        string Path,
        string CteId,
        string CteName)
    {
        public static QueryScope Main() => new("MainQuery", string.Empty, string.Empty);

        public static QueryScope ForCte(string cteId, string cteName)
        {
            var label = string.IsNullOrWhiteSpace(cteName)
                ? $"CTE:{cteId}"
                : $"CTE:{cteName}";
            return new QueryScope(label, cteId, cteName);
        }

        public QueryScope WithPathSegment(string segment)
        {
            var trimmed = segment?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                return this;
            }

            return string.IsNullOrWhiteSpace(Path)
                ? this with { Path = trimmed }
                : this with { Path = $"{Path} > {trimmed}" };
        }

        public QueryScope WithCte(string cteId, string cteName) =>
            this with
            {
                CteId = cteId ?? string.Empty,
                CteName = cteName ?? string.Empty,
            };
    }

    private static Dictionary<string, T> ToFirstMap<T>(
        IEnumerable<T> rows,
        Func<T, string> keySelector)
    {
        return rows
            .Select(row => (Key: keySelector(row), Row: row))
            .Where(static item => !string.IsNullOrWhiteSpace(item.Key))
            .GroupBy(item => item.Key, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.First().Row, StringComparer.Ordinal);
    }

    private static Dictionary<string, List<T>> GroupByKey<T>(
        IEnumerable<T> rows,
        Func<T, string> keySelector)
    {
        return rows
            .Select(row => (Key: keySelector(row), Row: row))
            .Where(static item => !string.IsNullOrWhiteSpace(item.Key))
            .GroupBy(item => item.Key, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Select(static item => item.Row).ToList(), StringComparer.Ordinal);
    }

    private static int ParseOrdinalOrMax(string? ordinal)
    {
        return int.TryParse(ordinal, out var parsed)
            ? parsed
            : int.MaxValue;
    }
}
