#nullable enable
using System;
using System.Collections.Generic;

namespace MetaTransformScript;
public sealed partial class AtTimeZoneCall
{
    public string Id { get; set; } = null !;
    public PrimaryExpression PrimaryExpression { get; set; } = null !;
}

public sealed partial class AtTimeZoneCallDateValueLink
{
    public string Id { get; set; } = null !;
    public AtTimeZoneCall AtTimeZoneCall { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class AtTimeZoneCallTimeZoneLink
{
    public string Id { get; set; } = null !;
    public AtTimeZoneCall AtTimeZoneCall { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class BinaryExpression
{
    public string Id { get; set; } = null !;
    public string? BinaryExpressionType { get; set; }
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class BinaryExpressionFirstExpressionLink
{
    public string Id { get; set; } = null !;
    public BinaryExpression BinaryExpression { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class BinaryExpressionSecondExpressionLink
{
    public string Id { get; set; } = null !;
    public BinaryExpression BinaryExpression { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class BinaryLiteral
{
    public string Id { get; set; } = null !;
    public string? IsLargeObject { get; set; }
    public string? LiteralType { get; set; }
    public Literal Literal { get; set; } = null !;
}

public sealed partial class BinaryQueryExpression
{
    public string Id { get; set; } = null !;
    public string? All { get; set; }
    public string? BinaryQueryExpressionType { get; set; }
    public QueryExpression QueryExpression { get; set; } = null !;
}

public sealed partial class BinaryQueryExpressionFirstQueryExpressionLink
{
    public string Id { get; set; } = null !;
    public BinaryQueryExpression BinaryQueryExpression { get; set; } = null !;
    public QueryExpression QueryExpression { get; set; } = null !;
}

public sealed partial class BinaryQueryExpressionSecondQueryExpressionLink
{
    public string Id { get; set; } = null !;
    public BinaryQueryExpression BinaryQueryExpression { get; set; } = null !;
    public QueryExpression QueryExpression { get; set; } = null !;
}

public sealed partial class BooleanBinaryExpression
{
    public string Id { get; set; } = null !;
    public string? BinaryExpressionType { get; set; }
    public BooleanExpression BooleanExpression { get; set; } = null !;
}

public sealed partial class BooleanBinaryExpressionFirstExpressionLink
{
    public string Id { get; set; } = null !;
    public BooleanBinaryExpression BooleanBinaryExpression { get; set; } = null !;
    public BooleanExpression BooleanExpression { get; set; } = null !;
}

public sealed partial class BooleanBinaryExpressionSecondExpressionLink
{
    public string Id { get; set; } = null !;
    public BooleanBinaryExpression BooleanBinaryExpression { get; set; } = null !;
    public BooleanExpression BooleanExpression { get; set; } = null !;
}

public sealed partial class BooleanComparisonExpression
{
    public string Id { get; set; } = null !;
    public string? ComparisonType { get; set; }
    public BooleanExpression BooleanExpression { get; set; } = null !;
}

public sealed partial class BooleanComparisonExpressionFirstExpressionLink
{
    public string Id { get; set; } = null !;
    public BooleanComparisonExpression BooleanComparisonExpression { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class BooleanComparisonExpressionSecondExpressionLink
{
    public string Id { get; set; } = null !;
    public BooleanComparisonExpression BooleanComparisonExpression { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class BooleanExpression
{
    public string Id { get; set; } = null !;
}

public sealed partial class BooleanIsNullExpression
{
    public string Id { get; set; } = null !;
    public string? IsNot { get; set; }
    public BooleanExpression BooleanExpression { get; set; } = null !;
}

public sealed partial class BooleanIsNullExpressionExpressionLink
{
    public string Id { get; set; } = null !;
    public BooleanIsNullExpression BooleanIsNullExpression { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class BooleanNotExpression
{
    public string Id { get; set; } = null !;
    public BooleanExpression BooleanExpression { get; set; } = null !;
}

public sealed partial class BooleanNotExpressionExpressionLink
{
    public string Id { get; set; } = null !;
    public BooleanExpression BooleanExpression { get; set; } = null !;
    public BooleanNotExpression BooleanNotExpression { get; set; } = null !;
}

public sealed partial class BooleanParenthesisExpression
{
    public string Id { get; set; } = null !;
    public BooleanExpression BooleanExpression { get; set; } = null !;
}

public sealed partial class BooleanParenthesisExpressionExpressionLink
{
    public string Id { get; set; } = null !;
    public BooleanExpression BooleanExpression { get; set; } = null !;
    public BooleanParenthesisExpression BooleanParenthesisExpression { get; set; } = null !;
}

public sealed partial class BooleanTernaryExpression
{
    public string Id { get; set; } = null !;
    public string? TernaryExpressionType { get; set; }
    public BooleanExpression BooleanExpression { get; set; } = null !;
}

public sealed partial class BooleanTernaryExpressionFirstExpressionLink
{
    public string Id { get; set; } = null !;
    public BooleanTernaryExpression BooleanTernaryExpression { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class BooleanTernaryExpressionSecondExpressionLink
{
    public string Id { get; set; } = null !;
    public BooleanTernaryExpression BooleanTernaryExpression { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class BooleanTernaryExpressionThirdExpressionLink
{
    public string Id { get; set; } = null !;
    public BooleanTernaryExpression BooleanTernaryExpression { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class CallTarget
{
    public string Id { get; set; } = null !;
}

public sealed partial class CaseExpression
{
    public string Id { get; set; } = null !;
    public PrimaryExpression PrimaryExpression { get; set; } = null !;
}

public sealed partial class CaseExpressionElseExpressionLink
{
    public string Id { get; set; } = null !;
    public CaseExpression CaseExpression { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class CastCall
{
    public string Id { get; set; } = null !;
    public PrimaryExpression PrimaryExpression { get; set; } = null !;
}

public sealed partial class CastCallDataTypeLink
{
    public string Id { get; set; } = null !;
    public CastCall CastCall { get; set; } = null !;
    public DataTypeReference DataTypeReference { get; set; } = null !;
}

public sealed partial class CastCallParameterLink
{
    public string Id { get; set; } = null !;
    public CastCall CastCall { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class CoalesceExpression
{
    public string Id { get; set; } = null !;
    public PrimaryExpression PrimaryExpression { get; set; } = null !;
}

public sealed partial class CoalesceExpressionExpressionsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public CoalesceExpression CoalesceExpression { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class ColumnReferenceExpression
{
    public string Id { get; set; } = null !;
    public string? ColumnType { get; set; }
    public PrimaryExpression PrimaryExpression { get; set; } = null !;
}

public sealed partial class ColumnReferenceExpressionMultiPartIdentifierLink
{
    public string Id { get; set; } = null !;
    public ColumnReferenceExpression ColumnReferenceExpression { get; set; } = null !;
    public MultiPartIdentifier MultiPartIdentifier { get; set; } = null !;
}

public sealed partial class CommonTableExpression
{
    public string Id { get; set; } = null !;
}

public sealed partial class CommonTableExpressionColumnsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public CommonTableExpression CommonTableExpression { get; set; } = null !;
    public Identifier Identifier { get; set; } = null !;
}

public sealed partial class CommonTableExpressionExpressionNameLink
{
    public string Id { get; set; } = null !;
    public CommonTableExpression CommonTableExpression { get; set; } = null !;
    public Identifier Identifier { get; set; } = null !;
}

public sealed partial class CommonTableExpressionQueryExpressionLink
{
    public string Id { get; set; } = null !;
    public CommonTableExpression CommonTableExpression { get; set; } = null !;
    public QueryExpression QueryExpression { get; set; } = null !;
}

public sealed partial class CompositeGroupingSpecification
{
    public string Id { get; set; } = null !;
    public GroupingSpecification GroupingSpecification { get; set; } = null !;
}

public sealed partial class CompositeGroupingSpecificationItemsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public CompositeGroupingSpecification CompositeGroupingSpecification { get; set; } = null !;
    public GroupingSpecification GroupingSpecification { get; set; } = null !;
}

public sealed partial class ConvertCall
{
    public string Id { get; set; } = null !;
    public PrimaryExpression PrimaryExpression { get; set; } = null !;
}

public sealed partial class ConvertCallDataTypeLink
{
    public string Id { get; set; } = null !;
    public ConvertCall ConvertCall { get; set; } = null !;
    public DataTypeReference DataTypeReference { get; set; } = null !;
}

public sealed partial class ConvertCallParameterLink
{
    public string Id { get; set; } = null !;
    public ConvertCall ConvertCall { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class ConvertCallStyleLink
{
    public string Id { get; set; } = null !;
    public ConvertCall ConvertCall { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class CubeGroupingSpecification
{
    public string Id { get; set; } = null !;
    public GroupingSpecification GroupingSpecification { get; set; } = null !;
}

public sealed partial class CubeGroupingSpecificationArgumentsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public CubeGroupingSpecification CubeGroupingSpecification { get; set; } = null !;
    public GroupingSpecification GroupingSpecification { get; set; } = null !;
}

public sealed partial class DataTypeReference
{
    public string Id { get; set; } = null !;
}

public sealed partial class DataTypeReferenceNameLink
{
    public string Id { get; set; } = null !;
    public DataTypeReference DataTypeReference { get; set; } = null !;
    public SchemaObjectName SchemaObjectName { get; set; } = null !;
}

public sealed partial class DeleteStatement
{
    public string Id { get; set; } = null !;
    public StatementWithCtesAndXmlNamespaces StatementWithCtesAndXmlNamespaces { get; set; } = null !;
}

public sealed partial class DeleteStatementFromClauseLink
{
    public string Id { get; set; } = null !;
    public DeleteStatement DeleteStatement { get; set; } = null !;
    public FromClause FromClause { get; set; } = null !;
}

public sealed partial class DeleteStatementTargetLink
{
    public string Id { get; set; } = null !;
    public DeleteStatement DeleteStatement { get; set; } = null !;
    public SchemaObjectName SchemaObjectName { get; set; } = null !;
}

public sealed partial class DeleteStatementWhereClauseLink
{
    public string Id { get; set; } = null !;
    public DeleteStatement DeleteStatement { get; set; } = null !;
    public WhereClause WhereClause { get; set; } = null !;
}

public sealed partial class DistinctPredicate
{
    public string Id { get; set; } = null !;
    public string? IsNot { get; set; }
    public BooleanExpression BooleanExpression { get; set; } = null !;
}

public sealed partial class DistinctPredicateFirstExpressionLink
{
    public string Id { get; set; } = null !;
    public DistinctPredicate DistinctPredicate { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class DistinctPredicateSecondExpressionLink
{
    public string Id { get; set; } = null !;
    public DistinctPredicate DistinctPredicate { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class ExistsPredicate
{
    public string Id { get; set; } = null !;
    public BooleanExpression BooleanExpression { get; set; } = null !;
}

public sealed partial class ExistsPredicateSubqueryLink
{
    public string Id { get; set; } = null !;
    public ExistsPredicate ExistsPredicate { get; set; } = null !;
    public ScalarSubquery ScalarSubquery { get; set; } = null !;
}

public sealed partial class ExpressionGroupingSpecification
{
    public string Id { get; set; } = null !;
    public string? DistributedAggregation { get; set; }
    public GroupingSpecification GroupingSpecification { get; set; } = null !;
}

public sealed partial class ExpressionGroupingSpecificationExpressionLink
{
    public string Id { get; set; } = null !;
    public ExpressionGroupingSpecification ExpressionGroupingSpecification { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class ExpressionWithSortOrder
{
    public string Id { get; set; } = null !;
    public string? SortOrder { get; set; }
}

public sealed partial class ExpressionWithSortOrderExpressionLink
{
    public string Id { get; set; } = null !;
    public ExpressionWithSortOrder ExpressionWithSortOrder { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class FromClause
{
    public string Id { get; set; } = null !;
}

public sealed partial class FromClauseTableReferencesItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public FromClause FromClause { get; set; } = null !;
    public TableReference TableReference { get; set; } = null !;
}

public sealed partial class FullTextPredicate
{
    public string Id { get; set; } = null !;
    public string? FullTextFunctionType { get; set; }
    public BooleanExpression BooleanExpression { get; set; } = null !;
}

public sealed partial class FullTextPredicateColumnsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public ColumnReferenceExpression ColumnReferenceExpression { get; set; } = null !;
    public FullTextPredicate FullTextPredicate { get; set; } = null !;
}

public sealed partial class FullTextPredicateValueLink
{
    public string Id { get; set; } = null !;
    public FullTextPredicate FullTextPredicate { get; set; } = null !;
    public ValueExpression ValueExpression { get; set; } = null !;
}

public sealed partial class FullTextTableReference
{
    public string Id { get; set; } = null !;
    public string? FullTextFunctionType { get; set; }
    public TableReferenceWithAlias TableReferenceWithAlias { get; set; } = null !;
}

public sealed partial class FullTextTableReferenceColumnsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public ColumnReferenceExpression ColumnReferenceExpression { get; set; } = null !;
    public FullTextTableReference FullTextTableReference { get; set; } = null !;
}

public sealed partial class FullTextTableReferenceSearchConditionLink
{
    public string Id { get; set; } = null !;
    public FullTextTableReference FullTextTableReference { get; set; } = null !;
    public ValueExpression ValueExpression { get; set; } = null !;
}

public sealed partial class FullTextTableReferenceTableNameLink
{
    public string Id { get; set; } = null !;
    public FullTextTableReference FullTextTableReference { get; set; } = null !;
    public SchemaObjectName SchemaObjectName { get; set; } = null !;
}

public sealed partial class FunctionCall
{
    public string Id { get; set; } = null !;
    public string? UniqueRowFilter { get; set; }
    public string? WithArrayWrapper { get; set; }
    public PrimaryExpression PrimaryExpression { get; set; } = null !;
}

public sealed partial class FunctionCallCallTargetLink
{
    public string Id { get; set; } = null !;
    public CallTarget CallTarget { get; set; } = null !;
    public FunctionCall FunctionCall { get; set; } = null !;
}

public sealed partial class FunctionCallFunctionNameLink
{
    public string Id { get; set; } = null !;
    public FunctionCall FunctionCall { get; set; } = null !;
    public Identifier Identifier { get; set; } = null !;
}

public sealed partial class FunctionCallOverClauseLink
{
    public string Id { get; set; } = null !;
    public FunctionCall FunctionCall { get; set; } = null !;
    public OverClause OverClause { get; set; } = null !;
}

public sealed partial class FunctionCallParametersItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public FunctionCall FunctionCall { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class FunctionCallWithinGroupOrderByClauseLink
{
    public string Id { get; set; } = null !;
    public FunctionCall FunctionCall { get; set; } = null !;
    public OrderByClause OrderByClause { get; set; } = null !;
}

public sealed partial class GlobalFunctionTableReference
{
    public string Id { get; set; } = null !;
    public TableReferenceWithAlias TableReferenceWithAlias { get; set; } = null !;
}

public sealed partial class GlobalFunctionTableReferenceNameLink
{
    public string Id { get; set; } = null !;
    public GlobalFunctionTableReference GlobalFunctionTableReference { get; set; } = null !;
    public Identifier Identifier { get; set; } = null !;
}

public sealed partial class GlobalFunctionTableReferenceParametersItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public GlobalFunctionTableReference GlobalFunctionTableReference { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class GlobalVariableExpression
{
    public string Id { get; set; } = null !;
    public string? Name { get; set; }
    public ValueExpression ValueExpression { get; set; } = null !;
}

public sealed partial class GrandTotalGroupingSpecification
{
    public string Id { get; set; } = null !;
    public GroupingSpecification GroupingSpecification { get; set; } = null !;
}

public sealed partial class GroupByClause
{
    public string Id { get; set; } = null !;
    public string? All { get; set; }
    public string? GroupByOption { get; set; }
}

public sealed partial class GroupByClauseGroupingSpecificationsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public GroupByClause GroupByClause { get; set; } = null !;
    public GroupingSpecification GroupingSpecification { get; set; } = null !;
}

public sealed partial class GroupingSetsGroupingSpecification
{
    public string Id { get; set; } = null !;
    public GroupingSpecification GroupingSpecification { get; set; } = null !;
}

public sealed partial class GroupingSetsGroupingSpecificationSetsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public GroupingSetsGroupingSpecification GroupingSetsGroupingSpecification { get; set; } = null !;
    public GroupingSpecification GroupingSpecification { get; set; } = null !;
}

public sealed partial class GroupingSpecification
{
    public string Id { get; set; } = null !;
}

public sealed partial class HavingClause
{
    public string Id { get; set; } = null !;
}

public sealed partial class HavingClauseSearchConditionLink
{
    public string Id { get; set; } = null !;
    public BooleanExpression BooleanExpression { get; set; } = null !;
    public HavingClause HavingClause { get; set; } = null !;
}

public sealed partial class Identifier
{
    public string Id { get; set; } = null !;
    public string? QuoteType { get; set; }
    public string? Value { get; set; }
}

public sealed partial class IdentifierOrValueExpression
{
    public string Id { get; set; } = null !;
    public string? Value { get; set; }
}

public sealed partial class IdentifierOrValueExpressionIdentifierLink
{
    public string Id { get; set; } = null !;
    public Identifier Identifier { get; set; } = null !;
    public IdentifierOrValueExpression IdentifierOrValueExpression { get; set; } = null !;
}

public sealed partial class IIfCall
{
    public string Id { get; set; } = null !;
    public PrimaryExpression PrimaryExpression { get; set; } = null !;
}

public sealed partial class IIfCallElseExpressionLink
{
    public string Id { get; set; } = null !;
    public IIfCall IIfCall { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class IIfCallPredicateLink
{
    public string Id { get; set; } = null !;
    public BooleanExpression BooleanExpression { get; set; } = null !;
    public IIfCall IIfCall { get; set; } = null !;
}

public sealed partial class IIfCallThenExpressionLink
{
    public string Id { get; set; } = null !;
    public IIfCall IIfCall { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class InlineDerivedTable
{
    public string Id { get; set; } = null !;
    public TableReferenceWithAliasAndColumns TableReferenceWithAliasAndColumns { get; set; } = null !;
}

public sealed partial class InlineDerivedTableRowValuesItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public InlineDerivedTable InlineDerivedTable { get; set; } = null !;
    public RowValue RowValue { get; set; } = null !;
}

public sealed partial class InPredicate
{
    public string Id { get; set; } = null !;
    public string? NotDefined { get; set; }
    public BooleanExpression BooleanExpression { get; set; } = null !;
}

public sealed partial class InPredicateExpressionLink
{
    public string Id { get; set; } = null !;
    public InPredicate InPredicate { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class InPredicateSubqueryLink
{
    public string Id { get; set; } = null !;
    public InPredicate InPredicate { get; set; } = null !;
    public ScalarSubquery ScalarSubquery { get; set; } = null !;
}

public sealed partial class InPredicateValuesItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public InPredicate InPredicate { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class InsertQuerySource
{
    public string Id { get; set; } = null !;
    public InsertSource InsertSource { get; set; } = null !;
}

public sealed partial class InsertQuerySourceQueryExpressionLink
{
    public string Id { get; set; } = null !;
    public InsertQuerySource InsertQuerySource { get; set; } = null !;
    public QueryExpression QueryExpression { get; set; } = null !;
}

public sealed partial class InsertSource
{
    public string Id { get; set; } = null !;
}

public sealed partial class InsertStatement
{
    public string Id { get; set; } = null !;
    public StatementWithCtesAndXmlNamespaces StatementWithCtesAndXmlNamespaces { get; set; } = null !;
}

public sealed partial class InsertStatementColumnsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public Identifier Identifier { get; set; } = null !;
    public InsertStatement InsertStatement { get; set; } = null !;
}

public sealed partial class InsertStatementSourceLink
{
    public string Id { get; set; } = null !;
    public InsertSource InsertSource { get; set; } = null !;
    public InsertStatement InsertStatement { get; set; } = null !;
}

public sealed partial class InsertStatementTargetLink
{
    public string Id { get; set; } = null !;
    public InsertStatement InsertStatement { get; set; } = null !;
    public SchemaObjectName SchemaObjectName { get; set; } = null !;
}

public sealed partial class InsertValuesSource
{
    public string Id { get; set; } = null !;
    public InsertSource InsertSource { get; set; } = null !;
}

public sealed partial class InsertValuesSourceRowValuesItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public InsertValuesSource InsertValuesSource { get; set; } = null !;
    public RowValue RowValue { get; set; } = null !;
}

public sealed partial class IntegerLiteral
{
    public string Id { get; set; } = null !;
    public string? LiteralType { get; set; }
    public Literal Literal { get; set; } = null !;
}

public sealed partial class JoinParenthesisTableReference
{
    public string Id { get; set; } = null !;
    public TableReference TableReference { get; set; } = null !;
}

public sealed partial class JoinParenthesisTableReferenceJoinLink
{
    public string Id { get; set; } = null !;
    public JoinParenthesisTableReference JoinParenthesisTableReference { get; set; } = null !;
    public TableReference TableReference { get; set; } = null !;
}

public sealed partial class JoinTableReference
{
    public string Id { get; set; } = null !;
    public TableReference TableReference { get; set; } = null !;
}

public sealed partial class JoinTableReferenceFirstTableReferenceLink
{
    public string Id { get; set; } = null !;
    public JoinTableReference JoinTableReference { get; set; } = null !;
    public TableReference TableReference { get; set; } = null !;
}

public sealed partial class JoinTableReferenceSecondTableReferenceLink
{
    public string Id { get; set; } = null !;
    public JoinTableReference JoinTableReference { get; set; } = null !;
    public TableReference TableReference { get; set; } = null !;
}

public sealed partial class LeftFunctionCall
{
    public string Id { get; set; } = null !;
    public PrimaryExpression PrimaryExpression { get; set; } = null !;
}

public sealed partial class LeftFunctionCallParametersItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public LeftFunctionCall LeftFunctionCall { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class LikePredicate
{
    public string Id { get; set; } = null !;
    public string? NotDefined { get; set; }
    public string? OdbcEscape { get; set; }
    public BooleanExpression BooleanExpression { get; set; } = null !;
}

public sealed partial class LikePredicateEscapeExpressionLink
{
    public string Id { get; set; } = null !;
    public LikePredicate LikePredicate { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class LikePredicateFirstExpressionLink
{
    public string Id { get; set; } = null !;
    public LikePredicate LikePredicate { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class LikePredicateSecondExpressionLink
{
    public string Id { get; set; } = null !;
    public LikePredicate LikePredicate { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class Literal
{
    public string Id { get; set; } = null !;
    public string? LiteralType { get; set; }
    public string? Value { get; set; }
    public ValueExpression ValueExpression { get; set; } = null !;
}

public sealed partial class MaxLiteral
{
    public string Id { get; set; } = null !;
    public string? LiteralType { get; set; }
    public Literal Literal { get; set; } = null !;
}

public sealed partial class MergeAction
{
    public string Id { get; set; } = null !;
}

public sealed partial class MergeDeleteAction
{
    public string Id { get; set; } = null !;
    public MergeAction MergeAction { get; set; } = null !;
}

public sealed partial class MergeInsertAction
{
    public string Id { get; set; } = null !;
    public MergeAction MergeAction { get; set; } = null !;
}

public sealed partial class MergeInsertActionColumnsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public Identifier Identifier { get; set; } = null !;
    public MergeInsertAction MergeInsertAction { get; set; } = null !;
}

public sealed partial class MergeInsertActionValuesItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public MergeInsertAction MergeInsertAction { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class MergeMatchedWhenClause
{
    public string Id { get; set; } = null !;
    public MergeWhenClause MergeWhenClause { get; set; } = null !;
}

public sealed partial class MergeNotMatchedBySourceWhenClause
{
    public string Id { get; set; } = null !;
    public MergeWhenClause MergeWhenClause { get; set; } = null !;
}

public sealed partial class MergeNotMatchedByTargetWhenClause
{
    public string Id { get; set; } = null !;
    public MergeWhenClause MergeWhenClause { get; set; } = null !;
}

public sealed partial class MergeStatement
{
    public string Id { get; set; } = null !;
    public StatementWithCtesAndXmlNamespaces StatementWithCtesAndXmlNamespaces { get; set; } = null !;
}

public sealed partial class MergeStatementOptionClauseLink
{
    public string Id { get; set; } = null !;
    public MergeStatement MergeStatement { get; set; } = null !;
    public OptionClause OptionClause { get; set; } = null !;
}

public sealed partial class MergeStatementOutputClauseLink
{
    public string Id { get; set; } = null !;
    public MergeStatement MergeStatement { get; set; } = null !;
    public OutputClause OutputClause { get; set; } = null !;
}

public sealed partial class MergeStatementSearchConditionLink
{
    public string Id { get; set; } = null !;
    public BooleanExpression BooleanExpression { get; set; } = null !;
    public MergeStatement MergeStatement { get; set; } = null !;
}

public sealed partial class MergeStatementSourceLink
{
    public string Id { get; set; } = null !;
    public MergeStatement MergeStatement { get; set; } = null !;
    public TableReference TableReference { get; set; } = null !;
}

public sealed partial class MergeStatementTargetAliasLink
{
    public string Id { get; set; } = null !;
    public Identifier Identifier { get; set; } = null !;
    public MergeStatement MergeStatement { get; set; } = null !;
}

public sealed partial class MergeStatementTargetHintsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public MergeStatement MergeStatement { get; set; } = null !;
    public SqlHint SqlHint { get; set; } = null !;
}

public sealed partial class MergeStatementTargetLink
{
    public string Id { get; set; } = null !;
    public MergeStatement MergeStatement { get; set; } = null !;
    public SchemaObjectName SchemaObjectName { get; set; } = null !;
}

public sealed partial class MergeStatementTopRowFilterLink
{
    public string Id { get; set; } = null !;
    public MergeStatement MergeStatement { get; set; } = null !;
    public TopRowFilter TopRowFilter { get; set; } = null !;
}

public sealed partial class MergeStatementWhenClausesItem
{
    public string Id { get; set; } = null !;
    public MergeStatement MergeStatement { get; set; } = null !;
    public MergeWhenClause MergeWhenClause { get; set; } = null !;
    public MergeStatementWhenClausesItem? PreviousMergeWhenClause { get; set; }
}

public sealed partial class MergeUpdateAction
{
    public string Id { get; set; } = null !;
    public MergeAction MergeAction { get; set; } = null !;
}

public sealed partial class MergeUpdateActionSetClauseLink
{
    public string Id { get; set; } = null !;
    public MergeUpdateAction MergeUpdateAction { get; set; } = null !;
    public SetClause SetClause { get; set; } = null !;
}

public sealed partial class MergeWhenClause
{
    public string Id { get; set; } = null !;
}

public sealed partial class MergeWhenClauseActionLink
{
    public string Id { get; set; } = null !;
    public MergeAction MergeAction { get; set; } = null !;
    public MergeWhenClause MergeWhenClause { get; set; } = null !;
}

public sealed partial class MergeWhenClauseSearchConditionLink
{
    public string Id { get; set; } = null !;
    public BooleanExpression BooleanExpression { get; set; } = null !;
    public MergeWhenClause MergeWhenClause { get; set; } = null !;
}

public sealed partial class MultiPartIdentifier
{
    public string Id { get; set; } = null !;
    public string? Count { get; set; }
}

public sealed partial class MultiPartIdentifierCallTarget
{
    public string Id { get; set; } = null !;
    public CallTarget CallTarget { get; set; } = null !;
}

public sealed partial class MultiPartIdentifierCallTargetMultiPartIdentifierLink
{
    public string Id { get; set; } = null !;
    public MultiPartIdentifierCallTarget MultiPartIdentifierCallTarget { get; set; } = null !;
    public MultiPartIdentifier MultiPartIdentifier { get; set; } = null !;
}

public sealed partial class MultiPartIdentifierIdentifiersItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public Identifier Identifier { get; set; } = null !;
    public MultiPartIdentifier MultiPartIdentifier { get; set; } = null !;
}

public sealed partial class NamedTableReference
{
    public string Id { get; set; } = null !;
    public TableReferenceWithAlias TableReferenceWithAlias { get; set; } = null !;
}

public sealed partial class NamedTableReferenceSchemaObjectLink
{
    public string Id { get; set; } = null !;
    public NamedTableReference NamedTableReference { get; set; } = null !;
    public SchemaObjectName SchemaObjectName { get; set; } = null !;
}

public sealed partial class NamedTableReferenceTableSampleClauseLink
{
    public string Id { get; set; } = null !;
    public NamedTableReference NamedTableReference { get; set; } = null !;
    public TableSampleClause TableSampleClause { get; set; } = null !;
}

public sealed partial class NextValueForExpression
{
    public string Id { get; set; } = null !;
    public PrimaryExpression PrimaryExpression { get; set; } = null !;
}

public sealed partial class NextValueForExpressionSequenceNameLink
{
    public string Id { get; set; } = null !;
    public NextValueForExpression NextValueForExpression { get; set; } = null !;
    public SchemaObjectName SchemaObjectName { get; set; } = null !;
}

public sealed partial class NullIfExpression
{
    public string Id { get; set; } = null !;
    public PrimaryExpression PrimaryExpression { get; set; } = null !;
}

public sealed partial class NullIfExpressionFirstExpressionLink
{
    public string Id { get; set; } = null !;
    public NullIfExpression NullIfExpression { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class NullIfExpressionSecondExpressionLink
{
    public string Id { get; set; } = null !;
    public NullIfExpression NullIfExpression { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class NullLiteral
{
    public string Id { get; set; } = null !;
    public string? LiteralType { get; set; }
    public Literal Literal { get; set; } = null !;
}

public sealed partial class NumericLiteral
{
    public string Id { get; set; } = null !;
    public string? LiteralType { get; set; }
    public Literal Literal { get; set; } = null !;
}

public sealed partial class OffsetClause
{
    public string Id { get; set; } = null !;
    public string? WithApproximate { get; set; }
}

public sealed partial class OffsetClauseFetchExpressionLink
{
    public string Id { get; set; } = null !;
    public OffsetClause OffsetClause { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class OffsetClauseOffsetExpressionLink
{
    public string Id { get; set; } = null !;
    public OffsetClause OffsetClause { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class OptionClause
{
    public string Id { get; set; } = null !;
}

public sealed partial class OptionClauseQueryHintsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public OptionClause OptionClause { get; set; } = null !;
    public SqlHint SqlHint { get; set; } = null !;
}

public sealed partial class OrderByClause
{
    public string Id { get; set; } = null !;
}

public sealed partial class OrderByClauseOrderByElementsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public ExpressionWithSortOrder ExpressionWithSortOrder { get; set; } = null !;
    public OrderByClause OrderByClause { get; set; } = null !;
}

public sealed partial class OutputClause
{
    public string Id { get; set; } = null !;
}

public sealed partial class OutputClauseIntoColumnsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public Identifier Identifier { get; set; } = null !;
    public OutputClause OutputClause { get; set; } = null !;
}

public sealed partial class OutputClauseIntoTargetLink
{
    public string Id { get; set; } = null !;
    public OutputClause OutputClause { get; set; } = null !;
    public SchemaObjectName SchemaObjectName { get; set; } = null !;
}

public sealed partial class OutputClauseSelectElementsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public OutputClause OutputClause { get; set; } = null !;
    public SelectElement SelectElement { get; set; } = null !;
}

public sealed partial class OverClause
{
    public string Id { get; set; } = null !;
}

public sealed partial class OverClauseOrderByClauseLink
{
    public string Id { get; set; } = null !;
    public OrderByClause OrderByClause { get; set; } = null !;
    public OverClause OverClause { get; set; } = null !;
}

public sealed partial class OverClausePartitionsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public OverClause OverClause { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class OverClauseWindowFrameClauseLink
{
    public string Id { get; set; } = null !;
    public OverClause OverClause { get; set; } = null !;
    public WindowFrameClause WindowFrameClause { get; set; } = null !;
}

public sealed partial class OverClauseWindowNameLink
{
    public string Id { get; set; } = null !;
    public Identifier Identifier { get; set; } = null !;
    public OverClause OverClause { get; set; } = null !;
}

public sealed partial class ParameterizedDataTypeReference
{
    public string Id { get; set; } = null !;
    public DataTypeReference DataTypeReference { get; set; } = null !;
}

public sealed partial class ParameterizedDataTypeReferenceParametersItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public Literal Literal { get; set; } = null !;
    public ParameterizedDataTypeReference ParameterizedDataTypeReference { get; set; } = null !;
}

public sealed partial class ParameterlessCall
{
    public string Id { get; set; } = null !;
    public string? ParameterlessCallType { get; set; }
    public PrimaryExpression PrimaryExpression { get; set; } = null !;
}

public sealed partial class ParenthesisExpression
{
    public string Id { get; set; } = null !;
    public PrimaryExpression PrimaryExpression { get; set; } = null !;
}

public sealed partial class ParenthesisExpressionExpressionLink
{
    public string Id { get; set; } = null !;
    public ParenthesisExpression ParenthesisExpression { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class ParseCall
{
    public string Id { get; set; } = null !;
    public PrimaryExpression PrimaryExpression { get; set; } = null !;
}

public sealed partial class ParseCallCultureLink
{
    public string Id { get; set; } = null !;
    public ParseCall ParseCall { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class ParseCallDataTypeLink
{
    public string Id { get; set; } = null !;
    public DataTypeReference DataTypeReference { get; set; } = null !;
    public ParseCall ParseCall { get; set; } = null !;
}

public sealed partial class ParseCallStringValueLink
{
    public string Id { get; set; } = null !;
    public ParseCall ParseCall { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class PivotedTableReference
{
    public string Id { get; set; } = null !;
    public TableReferenceWithAlias TableReferenceWithAlias { get; set; } = null !;
}

public sealed partial class PivotedTableReferenceAggregateFunctionIdentifierLink
{
    public string Id { get; set; } = null !;
    public MultiPartIdentifier MultiPartIdentifier { get; set; } = null !;
    public PivotedTableReference PivotedTableReference { get; set; } = null !;
}

public sealed partial class PivotedTableReferenceInColumnsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public Identifier Identifier { get; set; } = null !;
    public PivotedTableReference PivotedTableReference { get; set; } = null !;
}

public sealed partial class PivotedTableReferencePivotColumnLink
{
    public string Id { get; set; } = null !;
    public ColumnReferenceExpression ColumnReferenceExpression { get; set; } = null !;
    public PivotedTableReference PivotedTableReference { get; set; } = null !;
}

public sealed partial class PivotedTableReferenceTableReferenceLink
{
    public string Id { get; set; } = null !;
    public PivotedTableReference PivotedTableReference { get; set; } = null !;
    public TableReference TableReference { get; set; } = null !;
}

public sealed partial class PivotedTableReferenceValueColumnsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public ColumnReferenceExpression ColumnReferenceExpression { get; set; } = null !;
    public PivotedTableReference PivotedTableReference { get; set; } = null !;
}

public sealed partial class PrimaryExpression
{
    public string Id { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class PrimaryExpressionCollationLink
{
    public string Id { get; set; } = null !;
    public Identifier Identifier { get; set; } = null !;
    public PrimaryExpression PrimaryExpression { get; set; } = null !;
}

public sealed partial class QualifiedJoin
{
    public string Id { get; set; } = null !;
    public string? JoinHint { get; set; }
    public string? QualifiedJoinType { get; set; }
    public JoinTableReference JoinTableReference { get; set; } = null !;
}

public sealed partial class QualifiedJoinSearchConditionLink
{
    public string Id { get; set; } = null !;
    public BooleanExpression BooleanExpression { get; set; } = null !;
    public QualifiedJoin QualifiedJoin { get; set; } = null !;
}

public sealed partial class QueryDerivedTable
{
    public string Id { get; set; } = null !;
    public TableReferenceWithAliasAndColumns TableReferenceWithAliasAndColumns { get; set; } = null !;
}

public sealed partial class QueryDerivedTableQueryExpressionLink
{
    public string Id { get; set; } = null !;
    public QueryDerivedTable QueryDerivedTable { get; set; } = null !;
    public QueryExpression QueryExpression { get; set; } = null !;
}

public sealed partial class QueryExpression
{
    public string Id { get; set; } = null !;
}

public sealed partial class QueryExpressionOffsetClauseLink
{
    public string Id { get; set; } = null !;
    public OffsetClause OffsetClause { get; set; } = null !;
    public QueryExpression QueryExpression { get; set; } = null !;
}

public sealed partial class QueryExpressionOrderByClauseLink
{
    public string Id { get; set; } = null !;
    public OrderByClause OrderByClause { get; set; } = null !;
    public QueryExpression QueryExpression { get; set; } = null !;
}

public sealed partial class QueryParenthesisExpression
{
    public string Id { get; set; } = null !;
    public QueryExpression QueryExpression { get; set; } = null !;
}

public sealed partial class QueryParenthesisExpressionQueryExpressionLink
{
    public string Id { get; set; } = null !;
    public QueryExpression QueryExpression { get; set; } = null !;
    public QueryParenthesisExpression QueryParenthesisExpression { get; set; } = null !;
}

public sealed partial class QuerySpecification
{
    public string Id { get; set; } = null !;
    public string? UniqueRowFilter { get; set; }
    public QueryExpression QueryExpression { get; set; } = null !;
}

public sealed partial class QuerySpecificationFromClauseLink
{
    public string Id { get; set; } = null !;
    public FromClause FromClause { get; set; } = null !;
    public QuerySpecification QuerySpecification { get; set; } = null !;
}

public sealed partial class QuerySpecificationGroupByClauseLink
{
    public string Id { get; set; } = null !;
    public GroupByClause GroupByClause { get; set; } = null !;
    public QuerySpecification QuerySpecification { get; set; } = null !;
}

public sealed partial class QuerySpecificationHavingClauseLink
{
    public string Id { get; set; } = null !;
    public HavingClause HavingClause { get; set; } = null !;
    public QuerySpecification QuerySpecification { get; set; } = null !;
}

public sealed partial class QuerySpecificationSelectElementsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public QuerySpecification QuerySpecification { get; set; } = null !;
    public SelectElement SelectElement { get; set; } = null !;
}

public sealed partial class QuerySpecificationTopRowFilterLink
{
    public string Id { get; set; } = null !;
    public QuerySpecification QuerySpecification { get; set; } = null !;
    public TopRowFilter TopRowFilter { get; set; } = null !;
}

public sealed partial class QuerySpecificationWhereClauseLink
{
    public string Id { get; set; } = null !;
    public QuerySpecification QuerySpecification { get; set; } = null !;
    public WhereClause WhereClause { get; set; } = null !;
}

public sealed partial class QuerySpecificationWindowClauseLink
{
    public string Id { get; set; } = null !;
    public QuerySpecification QuerySpecification { get; set; } = null !;
    public WindowClause WindowClause { get; set; } = null !;
}

public sealed partial class RealLiteral
{
    public string Id { get; set; } = null !;
    public string? LiteralType { get; set; }
    public Literal Literal { get; set; } = null !;
}

public sealed partial class RightFunctionCall
{
    public string Id { get; set; } = null !;
    public PrimaryExpression PrimaryExpression { get; set; } = null !;
}

public sealed partial class RightFunctionCallParametersItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public RightFunctionCall RightFunctionCall { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class RollupGroupingSpecification
{
    public string Id { get; set; } = null !;
    public GroupingSpecification GroupingSpecification { get; set; } = null !;
}

public sealed partial class RollupGroupingSpecificationArgumentsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public GroupingSpecification GroupingSpecification { get; set; } = null !;
    public RollupGroupingSpecification RollupGroupingSpecification { get; set; } = null !;
}

public sealed partial class RowValue
{
    public string Id { get; set; } = null !;
}

public sealed partial class RowValueColumnValuesItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public RowValue RowValue { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class ScalarExpression
{
    public string Id { get; set; } = null !;
}

public sealed partial class ScalarSubquery
{
    public string Id { get; set; } = null !;
    public PrimaryExpression PrimaryExpression { get; set; } = null !;
}

public sealed partial class ScalarSubqueryQueryExpressionLink
{
    public string Id { get; set; } = null !;
    public QueryExpression QueryExpression { get; set; } = null !;
    public ScalarSubquery ScalarSubquery { get; set; } = null !;
}

public sealed partial class SchemaObjectFunctionTableReference
{
    public string Id { get; set; } = null !;
    public TableReferenceWithAliasAndColumns TableReferenceWithAliasAndColumns { get; set; } = null !;
}

public sealed partial class SchemaObjectFunctionTableReferenceParametersItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public ScalarExpression ScalarExpression { get; set; } = null !;
    public SchemaObjectFunctionTableReference SchemaObjectFunctionTableReference { get; set; } = null !;
}

public sealed partial class SchemaObjectFunctionTableReferenceSchemaObjectLink
{
    public string Id { get; set; } = null !;
    public SchemaObjectFunctionTableReference SchemaObjectFunctionTableReference { get; set; } = null !;
    public SchemaObjectName SchemaObjectName { get; set; } = null !;
}

public sealed partial class SchemaObjectName
{
    public string Id { get; set; } = null !;
    public MultiPartIdentifier MultiPartIdentifier { get; set; } = null !;
}

public sealed partial class SchemaObjectNameBaseIdentifierLink
{
    public string Id { get; set; } = null !;
    public Identifier Identifier { get; set; } = null !;
    public SchemaObjectName SchemaObjectName { get; set; } = null !;
}

public sealed partial class SchemaObjectNameSchemaIdentifierLink
{
    public string Id { get; set; } = null !;
    public Identifier Identifier { get; set; } = null !;
    public SchemaObjectName SchemaObjectName { get; set; } = null !;
}

public sealed partial class ScriptObjectScalarFunction
{
    public string Id { get; set; } = null !;
    public DataTypeReference DataTypeReference { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
    public TransformScript TransformScript { get; set; } = null !;
}

public sealed partial class ScriptObjectStoredProcedure
{
    public string Id { get; set; } = null !;
    public string DefinitionSql { get; set; } = null !;
    public TransformScript TransformScript { get; set; } = null !;
}

public sealed partial class ScriptObjectTVF
{
    public string Id { get; set; } = null !;
    public TransformScript TransformScript { get; set; } = null !;
}

public sealed partial class ScriptObjectView
{
    public string Id { get; set; } = null !;
    public string TargetSqlIdentifier { get; set; } = null !;
    public TransformScript TransformScript { get; set; } = null !;
}

public sealed partial class SearchedCaseExpression
{
    public string Id { get; set; } = null !;
    public CaseExpression CaseExpression { get; set; } = null !;
}

public sealed partial class SearchedCaseExpressionWhenClausesItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public SearchedCaseExpression SearchedCaseExpression { get; set; } = null !;
    public SearchedWhenClause SearchedWhenClause { get; set; } = null !;
}

public sealed partial class SearchedWhenClause
{
    public string Id { get; set; } = null !;
    public WhenClause WhenClause { get; set; } = null !;
}

public sealed partial class SearchedWhenClauseWhenExpressionLink
{
    public string Id { get; set; } = null !;
    public BooleanExpression BooleanExpression { get; set; } = null !;
    public SearchedWhenClause SearchedWhenClause { get; set; } = null !;
}

public sealed partial class SelectElement
{
    public string Id { get; set; } = null !;
}

public sealed partial class SelectScalarExpression
{
    public string Id { get; set; } = null !;
    public SelectElement SelectElement { get; set; } = null !;
}

public sealed partial class SelectScalarExpressionColumnNameLink
{
    public string Id { get; set; } = null !;
    public IdentifierOrValueExpression IdentifierOrValueExpression { get; set; } = null !;
    public SelectScalarExpression SelectScalarExpression { get; set; } = null !;
}

public sealed partial class SelectScalarExpressionExpressionLink
{
    public string Id { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
    public SelectScalarExpression SelectScalarExpression { get; set; } = null !;
}

public sealed partial class SelectStarExpression
{
    public string Id { get; set; } = null !;
    public SelectElement SelectElement { get; set; } = null !;
}

public sealed partial class SelectStarExpressionQualifierLink
{
    public string Id { get; set; } = null !;
    public MultiPartIdentifier MultiPartIdentifier { get; set; } = null !;
    public SelectStarExpression SelectStarExpression { get; set; } = null !;
}

public sealed partial class SelectStatement
{
    public string Id { get; set; } = null !;
    public StatementWithCtesAndXmlNamespaces StatementWithCtesAndXmlNamespaces { get; set; } = null !;
}

public sealed partial class SelectStatementQueryExpressionLink
{
    public string Id { get; set; } = null !;
    public QueryExpression QueryExpression { get; set; } = null !;
    public SelectStatement SelectStatement { get; set; } = null !;
}

public sealed partial class SetAssignment
{
    public string Id { get; set; } = null !;
}

public sealed partial class SetAssignmentTargetLink
{
    public string Id { get; set; } = null !;
    public ColumnReferenceExpression ColumnReferenceExpression { get; set; } = null !;
    public SetAssignment SetAssignment { get; set; } = null !;
}

public sealed partial class SetAssignmentValueLink
{
    public string Id { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
    public SetAssignment SetAssignment { get; set; } = null !;
}

public sealed partial class SetClause
{
    public string Id { get; set; } = null !;
}

public sealed partial class SetClauseAssignmentsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public SetAssignment SetAssignment { get; set; } = null !;
    public SetClause SetClause { get; set; } = null !;
}

public sealed partial class SimpleCaseExpression
{
    public string Id { get; set; } = null !;
    public CaseExpression CaseExpression { get; set; } = null !;
}

public sealed partial class SimpleCaseExpressionInputExpressionLink
{
    public string Id { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
    public SimpleCaseExpression SimpleCaseExpression { get; set; } = null !;
}

public sealed partial class SimpleCaseExpressionWhenClausesItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public SimpleCaseExpression SimpleCaseExpression { get; set; } = null !;
    public SimpleWhenClause SimpleWhenClause { get; set; } = null !;
}

public sealed partial class SimpleWhenClause
{
    public string Id { get; set; } = null !;
    public WhenClause WhenClause { get; set; } = null !;
}

public sealed partial class SimpleWhenClauseWhenExpressionLink
{
    public string Id { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
    public SimpleWhenClause SimpleWhenClause { get; set; } = null !;
}

public sealed partial class SqlDataTypeReference
{
    public string Id { get; set; } = null !;
    public string? SqlDataTypeOption { get; set; }
    public ParameterizedDataTypeReference ParameterizedDataTypeReference { get; set; } = null !;
}

public sealed partial class SqlHint
{
    public string Id { get; set; } = null !;
    public string? ArgumentStyle { get; set; }
}

public sealed partial class SqlHintArgumentsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public ScalarExpression ScalarExpression { get; set; } = null !;
    public SqlHint SqlHint { get; set; } = null !;
}

public sealed partial class SqlHintKeywordsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public Identifier Identifier { get; set; } = null !;
    public SqlHint SqlHint { get; set; } = null !;
}

public sealed partial class StatementWithCtesAndXmlNamespaces
{
    public string Id { get; set; } = null !;
    public TSqlStatement TSqlStatement { get; set; } = null !;
}

public sealed partial class StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLink
{
    public string Id { get; set; } = null !;
    public StatementWithCtesAndXmlNamespaces StatementWithCtesAndXmlNamespaces { get; set; } = null !;
    public WithCtesAndXmlNamespaces WithCtesAndXmlNamespaces { get; set; } = null !;
}

public sealed partial class StoredProcedureContract
{
    public string Id { get; set; } = null !;
    public string? Notes { get; set; }
    public ScriptObjectStoredProcedure ScriptObjectStoredProcedure { get; set; } = null !;
}

public sealed partial class StoredProcedureContractOperation
{
    public string Id { get; set; } = null !;
    public string? AccessRole { get; set; }
    public string? Notes { get; set; }
    public string OperationKind { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public string SqlIdentifier { get; set; } = null !;
    public StoredProcedureContract StoredProcedureContract { get; set; } = null !;
}

public sealed partial class StoredProcedureResultColumnItem
{
    public string Id { get; set; } = null !;
    public string? IsNullable { get; set; }
    public string? MetaDataTypeId { get; set; }
    public string Name { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public StoredProcedureResultRowsetItem StoredProcedureResultRowsetItem { get; set; } = null !;
}

public sealed partial class StoredProcedureResultRowsetItem
{
    public string Id { get; set; } = null !;
    public string? Name { get; set; }
    public string Ordinal { get; set; } = null !;
    public StoredProcedureContract StoredProcedureContract { get; set; } = null !;
}

public sealed partial class StringLiteral
{
    public string Id { get; set; } = null !;
    public string? IsLargeObject { get; set; }
    public string? IsNational { get; set; }
    public string? LiteralType { get; set; }
    public Literal Literal { get; set; } = null !;
}

public sealed partial class SubqueryComparisonPredicate
{
    public string Id { get; set; } = null !;
    public string? ComparisonType { get; set; }
    public string? SubqueryComparisonPredicateType { get; set; }
    public BooleanExpression BooleanExpression { get; set; } = null !;
}

public sealed partial class SubqueryComparisonPredicateExpressionLink
{
    public string Id { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
    public SubqueryComparisonPredicate SubqueryComparisonPredicate { get; set; } = null !;
}

public sealed partial class SubqueryComparisonPredicateSubqueryLink
{
    public string Id { get; set; } = null !;
    public ScalarSubquery ScalarSubquery { get; set; } = null !;
    public SubqueryComparisonPredicate SubqueryComparisonPredicate { get; set; } = null !;
}

public sealed partial class TableReference
{
    public string Id { get; set; } = null !;
}

public sealed partial class TableReferenceWithAlias
{
    public string Id { get; set; } = null !;
    public string? ForPath { get; set; }
    public TableReference TableReference { get; set; } = null !;
}

public sealed partial class TableReferenceWithAliasAliasLink
{
    public string Id { get; set; } = null !;
    public Identifier Identifier { get; set; } = null !;
    public TableReferenceWithAlias TableReferenceWithAlias { get; set; } = null !;
}

public sealed partial class TableReferenceWithAliasAndColumns
{
    public string Id { get; set; } = null !;
    public TableReferenceWithAlias TableReferenceWithAlias { get; set; } = null !;
}

public sealed partial class TableReferenceWithAliasAndColumnsColumnsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public Identifier Identifier { get; set; } = null !;
    public TableReferenceWithAliasAndColumns TableReferenceWithAliasAndColumns { get; set; } = null !;
}

public sealed partial class TableReferenceWithAliasTableHintsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public SqlHint SqlHint { get; set; } = null !;
    public TableReferenceWithAlias TableReferenceWithAlias { get; set; } = null !;
}

public sealed partial class TableSampleClause
{
    public string Id { get; set; } = null !;
    public string? System { get; set; }
    public string? TableSampleClauseOption { get; set; }
}

public sealed partial class TableSampleClauseRepeatSeedLink
{
    public string Id { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
    public TableSampleClause TableSampleClause { get; set; } = null !;
}

public sealed partial class TableSampleClauseSampleNumberLink
{
    public string Id { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
    public TableSampleClause TableSampleClause { get; set; } = null !;
}

public sealed partial class TopRowFilter
{
    public string Id { get; set; } = null !;
    public string? Percent { get; set; }
    public string? WithApproximate { get; set; }
    public string? WithTies { get; set; }
}

public sealed partial class TopRowFilterExpressionLink
{
    public string Id { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
    public TopRowFilter TopRowFilter { get; set; } = null !;
}

public sealed partial class TransformScript
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string? SourcePath { get; set; }
}

public sealed partial class TransformScriptFunctionParametersItem
{
    public string Id { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public DataTypeReference DataTypeReference { get; set; } = null !;
    public Identifier Identifier { get; set; } = null !;
    public TransformScript TransformScript { get; set; } = null !;
}

public sealed partial class TransformScriptObjectIdentifierLink
{
    public string Id { get; set; } = null !;
    public Identifier Identifier { get; set; } = null !;
    public TransformScript TransformScript { get; set; } = null !;
}

public sealed partial class TransformScriptSchemaIdentifierLink
{
    public string Id { get; set; } = null !;
    public Identifier Identifier { get; set; } = null !;
    public TransformScript TransformScript { get; set; } = null !;
}

public sealed partial class TransformScriptStatementLink
{
    public string Id { get; set; } = null !;
    public TransformScript TransformScript { get; set; } = null !;
    public TSqlStatement TSqlStatement { get; set; } = null !;
}

public sealed partial class TransformScriptViewColumnsItem
{
    public string Id { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public Identifier Identifier { get; set; } = null !;
    public TransformScript TransformScript { get; set; } = null !;
}

public sealed partial class TruncateStatement
{
    public string Id { get; set; } = null !;
    public TSqlStatement TSqlStatement { get; set; } = null !;
}

public sealed partial class TruncateStatementTargetLink
{
    public string Id { get; set; } = null !;
    public SchemaObjectName SchemaObjectName { get; set; } = null !;
    public TruncateStatement TruncateStatement { get; set; } = null !;
}

public sealed partial class TryCastCall
{
    public string Id { get; set; } = null !;
    public PrimaryExpression PrimaryExpression { get; set; } = null !;
}

public sealed partial class TryCastCallDataTypeLink
{
    public string Id { get; set; } = null !;
    public DataTypeReference DataTypeReference { get; set; } = null !;
    public TryCastCall TryCastCall { get; set; } = null !;
}

public sealed partial class TryCastCallParameterLink
{
    public string Id { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
    public TryCastCall TryCastCall { get; set; } = null !;
}

public sealed partial class TryConvertCall
{
    public string Id { get; set; } = null !;
    public PrimaryExpression PrimaryExpression { get; set; } = null !;
}

public sealed partial class TryConvertCallDataTypeLink
{
    public string Id { get; set; } = null !;
    public DataTypeReference DataTypeReference { get; set; } = null !;
    public TryConvertCall TryConvertCall { get; set; } = null !;
}

public sealed partial class TryConvertCallParameterLink
{
    public string Id { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
    public TryConvertCall TryConvertCall { get; set; } = null !;
}

public sealed partial class TryConvertCallStyleLink
{
    public string Id { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
    public TryConvertCall TryConvertCall { get; set; } = null !;
}

public sealed partial class TryParseCall
{
    public string Id { get; set; } = null !;
    public PrimaryExpression PrimaryExpression { get; set; } = null !;
}

public sealed partial class TryParseCallCultureLink
{
    public string Id { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
    public TryParseCall TryParseCall { get; set; } = null !;
}

public sealed partial class TryParseCallDataTypeLink
{
    public string Id { get; set; } = null !;
    public DataTypeReference DataTypeReference { get; set; } = null !;
    public TryParseCall TryParseCall { get; set; } = null !;
}

public sealed partial class TryParseCallStringValueLink
{
    public string Id { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
    public TryParseCall TryParseCall { get; set; } = null !;
}

public sealed partial class TSqlStatement
{
    public string Id { get; set; } = null !;
}

public sealed partial class UnaryExpression
{
    public string Id { get; set; } = null !;
    public string? UnaryExpressionType { get; set; }
    public ScalarExpression ScalarExpression { get; set; } = null !;
}

public sealed partial class UnaryExpressionExpressionLink
{
    public string Id { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
    public UnaryExpression UnaryExpression { get; set; } = null !;
}

public sealed partial class UnpivotedTableReference
{
    public string Id { get; set; } = null !;
    public TableReferenceWithAlias TableReferenceWithAlias { get; set; } = null !;
}

public sealed partial class UnpivotedTableReferenceInColumnsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public ColumnReferenceExpression ColumnReferenceExpression { get; set; } = null !;
    public UnpivotedTableReference UnpivotedTableReference { get; set; } = null !;
}

public sealed partial class UnpivotedTableReferencePivotColumnLink
{
    public string Id { get; set; } = null !;
    public Identifier Identifier { get; set; } = null !;
    public UnpivotedTableReference UnpivotedTableReference { get; set; } = null !;
}

public sealed partial class UnpivotedTableReferenceTableReferenceLink
{
    public string Id { get; set; } = null !;
    public TableReference TableReference { get; set; } = null !;
    public UnpivotedTableReference UnpivotedTableReference { get; set; } = null !;
}

public sealed partial class UnpivotedTableReferenceValueColumnLink
{
    public string Id { get; set; } = null !;
    public Identifier Identifier { get; set; } = null !;
    public UnpivotedTableReference UnpivotedTableReference { get; set; } = null !;
}

public sealed partial class UnqualifiedJoin
{
    public string Id { get; set; } = null !;
    public string? UnqualifiedJoinType { get; set; }
    public JoinTableReference JoinTableReference { get; set; } = null !;
}

public sealed partial class UpdateStatement
{
    public string Id { get; set; } = null !;
    public StatementWithCtesAndXmlNamespaces StatementWithCtesAndXmlNamespaces { get; set; } = null !;
}

public sealed partial class UpdateStatementFromClauseLink
{
    public string Id { get; set; } = null !;
    public FromClause FromClause { get; set; } = null !;
    public UpdateStatement UpdateStatement { get; set; } = null !;
}

public sealed partial class UpdateStatementSetClauseLink
{
    public string Id { get; set; } = null !;
    public SetClause SetClause { get; set; } = null !;
    public UpdateStatement UpdateStatement { get; set; } = null !;
}

public sealed partial class UpdateStatementTargetAliasLink
{
    public string Id { get; set; } = null !;
    public Identifier Identifier { get; set; } = null !;
    public UpdateStatement UpdateStatement { get; set; } = null !;
}

public sealed partial class UpdateStatementTargetLink
{
    public string Id { get; set; } = null !;
    public SchemaObjectName SchemaObjectName { get; set; } = null !;
    public UpdateStatement UpdateStatement { get; set; } = null !;
}

public sealed partial class UpdateStatementWhereClauseLink
{
    public string Id { get; set; } = null !;
    public UpdateStatement UpdateStatement { get; set; } = null !;
    public WhereClause WhereClause { get; set; } = null !;
}

public sealed partial class ValueExpression
{
    public string Id { get; set; } = null !;
    public PrimaryExpression PrimaryExpression { get; set; } = null !;
}

public sealed partial class WhenClause
{
    public string Id { get; set; } = null !;
}

public sealed partial class WhenClauseThenExpressionLink
{
    public string Id { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
    public WhenClause WhenClause { get; set; } = null !;
}

public sealed partial class WhereClause
{
    public string Id { get; set; } = null !;
}

public sealed partial class WhereClauseSearchConditionLink
{
    public string Id { get; set; } = null !;
    public BooleanExpression BooleanExpression { get; set; } = null !;
    public WhereClause WhereClause { get; set; } = null !;
}

public sealed partial class WindowClause
{
    public string Id { get; set; } = null !;
}

public sealed partial class WindowClauseWindowDefinitionItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public WindowClause WindowClause { get; set; } = null !;
    public WindowDefinition WindowDefinition { get; set; } = null !;
}

public sealed partial class WindowDefinition
{
    public string Id { get; set; } = null !;
}

public sealed partial class WindowDefinitionOrderByClauseLink
{
    public string Id { get; set; } = null !;
    public OrderByClause OrderByClause { get; set; } = null !;
    public WindowDefinition WindowDefinition { get; set; } = null !;
}

public sealed partial class WindowDefinitionPartitionsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public ScalarExpression ScalarExpression { get; set; } = null !;
    public WindowDefinition WindowDefinition { get; set; } = null !;
}

public sealed partial class WindowDefinitionRefWindowNameLink
{
    public string Id { get; set; } = null !;
    public Identifier Identifier { get; set; } = null !;
    public WindowDefinition WindowDefinition { get; set; } = null !;
}

public sealed partial class WindowDefinitionWindowFrameClauseLink
{
    public string Id { get; set; } = null !;
    public WindowDefinition WindowDefinition { get; set; } = null !;
    public WindowFrameClause WindowFrameClause { get; set; } = null !;
}

public sealed partial class WindowDefinitionWindowNameLink
{
    public string Id { get; set; } = null !;
    public Identifier Identifier { get; set; } = null !;
    public WindowDefinition WindowDefinition { get; set; } = null !;
}

public sealed partial class WindowDelimiter
{
    public string Id { get; set; } = null !;
    public string? WindowDelimiterType { get; set; }
}

public sealed partial class WindowDelimiterOffsetValueLink
{
    public string Id { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
    public WindowDelimiter WindowDelimiter { get; set; } = null !;
}

public sealed partial class WindowFrameClause
{
    public string Id { get; set; } = null !;
    public string? WindowFrameType { get; set; }
}

public sealed partial class WindowFrameClauseBottomLink
{
    public string Id { get; set; } = null !;
    public WindowDelimiter WindowDelimiter { get; set; } = null !;
    public WindowFrameClause WindowFrameClause { get; set; } = null !;
}

public sealed partial class WindowFrameClauseTopLink
{
    public string Id { get; set; } = null !;
    public WindowDelimiter WindowDelimiter { get; set; } = null !;
    public WindowFrameClause WindowFrameClause { get; set; } = null !;
}

public sealed partial class WithCtesAndXmlNamespaces
{
    public string Id { get; set; } = null !;
}

public sealed partial class WithCtesAndXmlNamespacesCommonTableExpressionsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public CommonTableExpression CommonTableExpression { get; set; } = null !;
    public WithCtesAndXmlNamespaces WithCtesAndXmlNamespaces { get; set; } = null !;
}

public sealed partial class WithCtesAndXmlNamespacesXmlNamespacesLink
{
    public string Id { get; set; } = null !;
    public WithCtesAndXmlNamespaces WithCtesAndXmlNamespaces { get; set; } = null !;
    public XmlNamespaces XmlNamespaces { get; set; } = null !;
}

public sealed partial class XmlNamespaces
{
    public string Id { get; set; } = null !;
}

public sealed partial class XmlNamespacesAliasElement
{
    public string Id { get; set; } = null !;
    public XmlNamespacesElement XmlNamespacesElement { get; set; } = null !;
}

public sealed partial class XmlNamespacesAliasElementIdentifierLink
{
    public string Id { get; set; } = null !;
    public Identifier Identifier { get; set; } = null !;
    public XmlNamespacesAliasElement XmlNamespacesAliasElement { get; set; } = null !;
}

public sealed partial class XmlNamespacesDefaultElement
{
    public string Id { get; set; } = null !;
    public XmlNamespacesElement XmlNamespacesElement { get; set; } = null !;
}

public sealed partial class XmlNamespacesElement
{
    public string Id { get; set; } = null !;
}

public sealed partial class XmlNamespacesElementStringLink
{
    public string Id { get; set; } = null !;
    public StringLiteral StringLiteral { get; set; } = null !;
    public XmlNamespacesElement XmlNamespacesElement { get; set; } = null !;
}

public sealed partial class XmlNamespacesXmlNamespacesElementsItem
{
    public string Id { get; set; } = null !;
    public string? Ordinal { get; set; }
    public XmlNamespacesElement XmlNamespacesElement { get; set; } = null !;
    public XmlNamespaces XmlNamespaces { get; set; } = null !;
}

public sealed partial class XmlNodesTableReference
{
    public string Id { get; set; } = null !;
    public TableReferenceWithAliasAndColumns TableReferenceWithAliasAndColumns { get; set; } = null !;
}

public sealed partial class XmlNodesTableReferenceTargetExpressionLink
{
    public string Id { get; set; } = null !;
    public ScalarExpression ScalarExpression { get; set; } = null !;
    public XmlNodesTableReference XmlNodesTableReference { get; set; } = null !;
}

public sealed partial class XmlNodesTableReferenceXQueryStringLink
{
    public string Id { get; set; } = null !;
    public StringLiteral StringLiteral { get; set; } = null !;
    public XmlNodesTableReference XmlNodesTableReference { get; set; } = null !;
}

public sealed partial class MetaTransformScriptModel
{
    public static MetaTransformScriptModel CreateEmpty() => new();
    public List<AtTimeZoneCall> AtTimeZoneCallList { get; set; } = new();
    public List<AtTimeZoneCallDateValueLink> AtTimeZoneCallDateValueLinkList { get; set; } = new();
    public List<AtTimeZoneCallTimeZoneLink> AtTimeZoneCallTimeZoneLinkList { get; set; } = new();
    public List<BinaryExpression> BinaryExpressionList { get; set; } = new();
    public List<BinaryExpressionFirstExpressionLink> BinaryExpressionFirstExpressionLinkList { get; set; } = new();
    public List<BinaryExpressionSecondExpressionLink> BinaryExpressionSecondExpressionLinkList { get; set; } = new();
    public List<BinaryLiteral> BinaryLiteralList { get; set; } = new();
    public List<BinaryQueryExpression> BinaryQueryExpressionList { get; set; } = new();
    public List<BinaryQueryExpressionFirstQueryExpressionLink> BinaryQueryExpressionFirstQueryExpressionLinkList { get; set; } = new();
    public List<BinaryQueryExpressionSecondQueryExpressionLink> BinaryQueryExpressionSecondQueryExpressionLinkList { get; set; } = new();
    public List<BooleanBinaryExpression> BooleanBinaryExpressionList { get; set; } = new();
    public List<BooleanBinaryExpressionFirstExpressionLink> BooleanBinaryExpressionFirstExpressionLinkList { get; set; } = new();
    public List<BooleanBinaryExpressionSecondExpressionLink> BooleanBinaryExpressionSecondExpressionLinkList { get; set; } = new();
    public List<BooleanComparisonExpression> BooleanComparisonExpressionList { get; set; } = new();
    public List<BooleanComparisonExpressionFirstExpressionLink> BooleanComparisonExpressionFirstExpressionLinkList { get; set; } = new();
    public List<BooleanComparisonExpressionSecondExpressionLink> BooleanComparisonExpressionSecondExpressionLinkList { get; set; } = new();
    public List<BooleanExpression> BooleanExpressionList { get; set; } = new();
    public List<BooleanIsNullExpression> BooleanIsNullExpressionList { get; set; } = new();
    public List<BooleanIsNullExpressionExpressionLink> BooleanIsNullExpressionExpressionLinkList { get; set; } = new();
    public List<BooleanNotExpression> BooleanNotExpressionList { get; set; } = new();
    public List<BooleanNotExpressionExpressionLink> BooleanNotExpressionExpressionLinkList { get; set; } = new();
    public List<BooleanParenthesisExpression> BooleanParenthesisExpressionList { get; set; } = new();
    public List<BooleanParenthesisExpressionExpressionLink> BooleanParenthesisExpressionExpressionLinkList { get; set; } = new();
    public List<BooleanTernaryExpression> BooleanTernaryExpressionList { get; set; } = new();
    public List<BooleanTernaryExpressionFirstExpressionLink> BooleanTernaryExpressionFirstExpressionLinkList { get; set; } = new();
    public List<BooleanTernaryExpressionSecondExpressionLink> BooleanTernaryExpressionSecondExpressionLinkList { get; set; } = new();
    public List<BooleanTernaryExpressionThirdExpressionLink> BooleanTernaryExpressionThirdExpressionLinkList { get; set; } = new();
    public List<CallTarget> CallTargetList { get; set; } = new();
    public List<CaseExpression> CaseExpressionList { get; set; } = new();
    public List<CaseExpressionElseExpressionLink> CaseExpressionElseExpressionLinkList { get; set; } = new();
    public List<CastCall> CastCallList { get; set; } = new();
    public List<CastCallDataTypeLink> CastCallDataTypeLinkList { get; set; } = new();
    public List<CastCallParameterLink> CastCallParameterLinkList { get; set; } = new();
    public List<CoalesceExpression> CoalesceExpressionList { get; set; } = new();
    public List<CoalesceExpressionExpressionsItem> CoalesceExpressionExpressionsItemList { get; set; } = new();
    public List<ColumnReferenceExpression> ColumnReferenceExpressionList { get; set; } = new();
    public List<ColumnReferenceExpressionMultiPartIdentifierLink> ColumnReferenceExpressionMultiPartIdentifierLinkList { get; set; } = new();
    public List<CommonTableExpression> CommonTableExpressionList { get; set; } = new();
    public List<CommonTableExpressionColumnsItem> CommonTableExpressionColumnsItemList { get; set; } = new();
    public List<CommonTableExpressionExpressionNameLink> CommonTableExpressionExpressionNameLinkList { get; set; } = new();
    public List<CommonTableExpressionQueryExpressionLink> CommonTableExpressionQueryExpressionLinkList { get; set; } = new();
    public List<CompositeGroupingSpecification> CompositeGroupingSpecificationList { get; set; } = new();
    public List<CompositeGroupingSpecificationItemsItem> CompositeGroupingSpecificationItemsItemList { get; set; } = new();
    public List<ConvertCall> ConvertCallList { get; set; } = new();
    public List<ConvertCallDataTypeLink> ConvertCallDataTypeLinkList { get; set; } = new();
    public List<ConvertCallParameterLink> ConvertCallParameterLinkList { get; set; } = new();
    public List<ConvertCallStyleLink> ConvertCallStyleLinkList { get; set; } = new();
    public List<CubeGroupingSpecification> CubeGroupingSpecificationList { get; set; } = new();
    public List<CubeGroupingSpecificationArgumentsItem> CubeGroupingSpecificationArgumentsItemList { get; set; } = new();
    public List<DataTypeReference> DataTypeReferenceList { get; set; } = new();
    public List<DataTypeReferenceNameLink> DataTypeReferenceNameLinkList { get; set; } = new();
    public List<DeleteStatement> DeleteStatementList { get; set; } = new();
    public List<DeleteStatementFromClauseLink> DeleteStatementFromClauseLinkList { get; set; } = new();
    public List<DeleteStatementTargetLink> DeleteStatementTargetLinkList { get; set; } = new();
    public List<DeleteStatementWhereClauseLink> DeleteStatementWhereClauseLinkList { get; set; } = new();
    public List<DistinctPredicate> DistinctPredicateList { get; set; } = new();
    public List<DistinctPredicateFirstExpressionLink> DistinctPredicateFirstExpressionLinkList { get; set; } = new();
    public List<DistinctPredicateSecondExpressionLink> DistinctPredicateSecondExpressionLinkList { get; set; } = new();
    public List<ExistsPredicate> ExistsPredicateList { get; set; } = new();
    public List<ExistsPredicateSubqueryLink> ExistsPredicateSubqueryLinkList { get; set; } = new();
    public List<ExpressionGroupingSpecification> ExpressionGroupingSpecificationList { get; set; } = new();
    public List<ExpressionGroupingSpecificationExpressionLink> ExpressionGroupingSpecificationExpressionLinkList { get; set; } = new();
    public List<ExpressionWithSortOrder> ExpressionWithSortOrderList { get; set; } = new();
    public List<ExpressionWithSortOrderExpressionLink> ExpressionWithSortOrderExpressionLinkList { get; set; } = new();
    public List<FromClause> FromClauseList { get; set; } = new();
    public List<FromClauseTableReferencesItem> FromClauseTableReferencesItemList { get; set; } = new();
    public List<FullTextPredicate> FullTextPredicateList { get; set; } = new();
    public List<FullTextPredicateColumnsItem> FullTextPredicateColumnsItemList { get; set; } = new();
    public List<FullTextPredicateValueLink> FullTextPredicateValueLinkList { get; set; } = new();
    public List<FullTextTableReference> FullTextTableReferenceList { get; set; } = new();
    public List<FullTextTableReferenceColumnsItem> FullTextTableReferenceColumnsItemList { get; set; } = new();
    public List<FullTextTableReferenceSearchConditionLink> FullTextTableReferenceSearchConditionLinkList { get; set; } = new();
    public List<FullTextTableReferenceTableNameLink> FullTextTableReferenceTableNameLinkList { get; set; } = new();
    public List<FunctionCall> FunctionCallList { get; set; } = new();
    public List<FunctionCallCallTargetLink> FunctionCallCallTargetLinkList { get; set; } = new();
    public List<FunctionCallFunctionNameLink> FunctionCallFunctionNameLinkList { get; set; } = new();
    public List<FunctionCallOverClauseLink> FunctionCallOverClauseLinkList { get; set; } = new();
    public List<FunctionCallParametersItem> FunctionCallParametersItemList { get; set; } = new();
    public List<FunctionCallWithinGroupOrderByClauseLink> FunctionCallWithinGroupOrderByClauseLinkList { get; set; } = new();
    public List<GlobalFunctionTableReference> GlobalFunctionTableReferenceList { get; set; } = new();
    public List<GlobalFunctionTableReferenceNameLink> GlobalFunctionTableReferenceNameLinkList { get; set; } = new();
    public List<GlobalFunctionTableReferenceParametersItem> GlobalFunctionTableReferenceParametersItemList { get; set; } = new();
    public List<GlobalVariableExpression> GlobalVariableExpressionList { get; set; } = new();
    public List<GrandTotalGroupingSpecification> GrandTotalGroupingSpecificationList { get; set; } = new();
    public List<GroupByClause> GroupByClauseList { get; set; } = new();
    public List<GroupByClauseGroupingSpecificationsItem> GroupByClauseGroupingSpecificationsItemList { get; set; } = new();
    public List<GroupingSetsGroupingSpecification> GroupingSetsGroupingSpecificationList { get; set; } = new();
    public List<GroupingSetsGroupingSpecificationSetsItem> GroupingSetsGroupingSpecificationSetsItemList { get; set; } = new();
    public List<GroupingSpecification> GroupingSpecificationList { get; set; } = new();
    public List<HavingClause> HavingClauseList { get; set; } = new();
    public List<HavingClauseSearchConditionLink> HavingClauseSearchConditionLinkList { get; set; } = new();
    public List<Identifier> IdentifierList { get; set; } = new();
    public List<IdentifierOrValueExpression> IdentifierOrValueExpressionList { get; set; } = new();
    public List<IdentifierOrValueExpressionIdentifierLink> IdentifierOrValueExpressionIdentifierLinkList { get; set; } = new();
    public List<IIfCall> IIfCallList { get; set; } = new();
    public List<IIfCallElseExpressionLink> IIfCallElseExpressionLinkList { get; set; } = new();
    public List<IIfCallPredicateLink> IIfCallPredicateLinkList { get; set; } = new();
    public List<IIfCallThenExpressionLink> IIfCallThenExpressionLinkList { get; set; } = new();
    public List<InlineDerivedTable> InlineDerivedTableList { get; set; } = new();
    public List<InlineDerivedTableRowValuesItem> InlineDerivedTableRowValuesItemList { get; set; } = new();
    public List<InPredicate> InPredicateList { get; set; } = new();
    public List<InPredicateExpressionLink> InPredicateExpressionLinkList { get; set; } = new();
    public List<InPredicateSubqueryLink> InPredicateSubqueryLinkList { get; set; } = new();
    public List<InPredicateValuesItem> InPredicateValuesItemList { get; set; } = new();
    public List<InsertQuerySource> InsertQuerySourceList { get; set; } = new();
    public List<InsertQuerySourceQueryExpressionLink> InsertQuerySourceQueryExpressionLinkList { get; set; } = new();
    public List<InsertSource> InsertSourceList { get; set; } = new();
    public List<InsertStatement> InsertStatementList { get; set; } = new();
    public List<InsertStatementColumnsItem> InsertStatementColumnsItemList { get; set; } = new();
    public List<InsertStatementSourceLink> InsertStatementSourceLinkList { get; set; } = new();
    public List<InsertStatementTargetLink> InsertStatementTargetLinkList { get; set; } = new();
    public List<InsertValuesSource> InsertValuesSourceList { get; set; } = new();
    public List<InsertValuesSourceRowValuesItem> InsertValuesSourceRowValuesItemList { get; set; } = new();
    public List<IntegerLiteral> IntegerLiteralList { get; set; } = new();
    public List<JoinParenthesisTableReference> JoinParenthesisTableReferenceList { get; set; } = new();
    public List<JoinParenthesisTableReferenceJoinLink> JoinParenthesisTableReferenceJoinLinkList { get; set; } = new();
    public List<JoinTableReference> JoinTableReferenceList { get; set; } = new();
    public List<JoinTableReferenceFirstTableReferenceLink> JoinTableReferenceFirstTableReferenceLinkList { get; set; } = new();
    public List<JoinTableReferenceSecondTableReferenceLink> JoinTableReferenceSecondTableReferenceLinkList { get; set; } = new();
    public List<LeftFunctionCall> LeftFunctionCallList { get; set; } = new();
    public List<LeftFunctionCallParametersItem> LeftFunctionCallParametersItemList { get; set; } = new();
    public List<LikePredicate> LikePredicateList { get; set; } = new();
    public List<LikePredicateEscapeExpressionLink> LikePredicateEscapeExpressionLinkList { get; set; } = new();
    public List<LikePredicateFirstExpressionLink> LikePredicateFirstExpressionLinkList { get; set; } = new();
    public List<LikePredicateSecondExpressionLink> LikePredicateSecondExpressionLinkList { get; set; } = new();
    public List<Literal> LiteralList { get; set; } = new();
    public List<MaxLiteral> MaxLiteralList { get; set; } = new();
    public List<MergeAction> MergeActionList { get; set; } = new();
    public List<MergeDeleteAction> MergeDeleteActionList { get; set; } = new();
    public List<MergeInsertAction> MergeInsertActionList { get; set; } = new();
    public List<MergeInsertActionColumnsItem> MergeInsertActionColumnsItemList { get; set; } = new();
    public List<MergeInsertActionValuesItem> MergeInsertActionValuesItemList { get; set; } = new();
    public List<MergeMatchedWhenClause> MergeMatchedWhenClauseList { get; set; } = new();
    public List<MergeNotMatchedBySourceWhenClause> MergeNotMatchedBySourceWhenClauseList { get; set; } = new();
    public List<MergeNotMatchedByTargetWhenClause> MergeNotMatchedByTargetWhenClauseList { get; set; } = new();
    public List<MergeStatement> MergeStatementList { get; set; } = new();
    public List<MergeStatementOptionClauseLink> MergeStatementOptionClauseLinkList { get; set; } = new();
    public List<MergeStatementOutputClauseLink> MergeStatementOutputClauseLinkList { get; set; } = new();
    public List<MergeStatementSearchConditionLink> MergeStatementSearchConditionLinkList { get; set; } = new();
    public List<MergeStatementSourceLink> MergeStatementSourceLinkList { get; set; } = new();
    public List<MergeStatementTargetAliasLink> MergeStatementTargetAliasLinkList { get; set; } = new();
    public List<MergeStatementTargetHintsItem> MergeStatementTargetHintsItemList { get; set; } = new();
    public List<MergeStatementTargetLink> MergeStatementTargetLinkList { get; set; } = new();
    public List<MergeStatementTopRowFilterLink> MergeStatementTopRowFilterLinkList { get; set; } = new();
    public List<MergeStatementWhenClausesItem> MergeStatementWhenClausesItemList { get; set; } = new();
    public List<MergeUpdateAction> MergeUpdateActionList { get; set; } = new();
    public List<MergeUpdateActionSetClauseLink> MergeUpdateActionSetClauseLinkList { get; set; } = new();
    public List<MergeWhenClause> MergeWhenClauseList { get; set; } = new();
    public List<MergeWhenClauseActionLink> MergeWhenClauseActionLinkList { get; set; } = new();
    public List<MergeWhenClauseSearchConditionLink> MergeWhenClauseSearchConditionLinkList { get; set; } = new();
    public List<MultiPartIdentifier> MultiPartIdentifierList { get; set; } = new();
    public List<MultiPartIdentifierCallTarget> MultiPartIdentifierCallTargetList { get; set; } = new();
    public List<MultiPartIdentifierCallTargetMultiPartIdentifierLink> MultiPartIdentifierCallTargetMultiPartIdentifierLinkList { get; set; } = new();
    public List<MultiPartIdentifierIdentifiersItem> MultiPartIdentifierIdentifiersItemList { get; set; } = new();
    public List<NamedTableReference> NamedTableReferenceList { get; set; } = new();
    public List<NamedTableReferenceSchemaObjectLink> NamedTableReferenceSchemaObjectLinkList { get; set; } = new();
    public List<NamedTableReferenceTableSampleClauseLink> NamedTableReferenceTableSampleClauseLinkList { get; set; } = new();
    public List<NextValueForExpression> NextValueForExpressionList { get; set; } = new();
    public List<NextValueForExpressionSequenceNameLink> NextValueForExpressionSequenceNameLinkList { get; set; } = new();
    public List<NullIfExpression> NullIfExpressionList { get; set; } = new();
    public List<NullIfExpressionFirstExpressionLink> NullIfExpressionFirstExpressionLinkList { get; set; } = new();
    public List<NullIfExpressionSecondExpressionLink> NullIfExpressionSecondExpressionLinkList { get; set; } = new();
    public List<NullLiteral> NullLiteralList { get; set; } = new();
    public List<NumericLiteral> NumericLiteralList { get; set; } = new();
    public List<OffsetClause> OffsetClauseList { get; set; } = new();
    public List<OffsetClauseFetchExpressionLink> OffsetClauseFetchExpressionLinkList { get; set; } = new();
    public List<OffsetClauseOffsetExpressionLink> OffsetClauseOffsetExpressionLinkList { get; set; } = new();
    public List<OptionClause> OptionClauseList { get; set; } = new();
    public List<OptionClauseQueryHintsItem> OptionClauseQueryHintsItemList { get; set; } = new();
    public List<OrderByClause> OrderByClauseList { get; set; } = new();
    public List<OrderByClauseOrderByElementsItem> OrderByClauseOrderByElementsItemList { get; set; } = new();
    public List<OutputClause> OutputClauseList { get; set; } = new();
    public List<OutputClauseIntoColumnsItem> OutputClauseIntoColumnsItemList { get; set; } = new();
    public List<OutputClauseIntoTargetLink> OutputClauseIntoTargetLinkList { get; set; } = new();
    public List<OutputClauseSelectElementsItem> OutputClauseSelectElementsItemList { get; set; } = new();
    public List<OverClause> OverClauseList { get; set; } = new();
    public List<OverClauseOrderByClauseLink> OverClauseOrderByClauseLinkList { get; set; } = new();
    public List<OverClausePartitionsItem> OverClausePartitionsItemList { get; set; } = new();
    public List<OverClauseWindowFrameClauseLink> OverClauseWindowFrameClauseLinkList { get; set; } = new();
    public List<OverClauseWindowNameLink> OverClauseWindowNameLinkList { get; set; } = new();
    public List<ParameterizedDataTypeReference> ParameterizedDataTypeReferenceList { get; set; } = new();
    public List<ParameterizedDataTypeReferenceParametersItem> ParameterizedDataTypeReferenceParametersItemList { get; set; } = new();
    public List<ParameterlessCall> ParameterlessCallList { get; set; } = new();
    public List<ParenthesisExpression> ParenthesisExpressionList { get; set; } = new();
    public List<ParenthesisExpressionExpressionLink> ParenthesisExpressionExpressionLinkList { get; set; } = new();
    public List<ParseCall> ParseCallList { get; set; } = new();
    public List<ParseCallCultureLink> ParseCallCultureLinkList { get; set; } = new();
    public List<ParseCallDataTypeLink> ParseCallDataTypeLinkList { get; set; } = new();
    public List<ParseCallStringValueLink> ParseCallStringValueLinkList { get; set; } = new();
    public List<PivotedTableReference> PivotedTableReferenceList { get; set; } = new();
    public List<PivotedTableReferenceAggregateFunctionIdentifierLink> PivotedTableReferenceAggregateFunctionIdentifierLinkList { get; set; } = new();
    public List<PivotedTableReferenceInColumnsItem> PivotedTableReferenceInColumnsItemList { get; set; } = new();
    public List<PivotedTableReferencePivotColumnLink> PivotedTableReferencePivotColumnLinkList { get; set; } = new();
    public List<PivotedTableReferenceTableReferenceLink> PivotedTableReferenceTableReferenceLinkList { get; set; } = new();
    public List<PivotedTableReferenceValueColumnsItem> PivotedTableReferenceValueColumnsItemList { get; set; } = new();
    public List<PrimaryExpression> PrimaryExpressionList { get; set; } = new();
    public List<PrimaryExpressionCollationLink> PrimaryExpressionCollationLinkList { get; set; } = new();
    public List<QualifiedJoin> QualifiedJoinList { get; set; } = new();
    public List<QualifiedJoinSearchConditionLink> QualifiedJoinSearchConditionLinkList { get; set; } = new();
    public List<QueryDerivedTable> QueryDerivedTableList { get; set; } = new();
    public List<QueryDerivedTableQueryExpressionLink> QueryDerivedTableQueryExpressionLinkList { get; set; } = new();
    public List<QueryExpression> QueryExpressionList { get; set; } = new();
    public List<QueryExpressionOffsetClauseLink> QueryExpressionOffsetClauseLinkList { get; set; } = new();
    public List<QueryExpressionOrderByClauseLink> QueryExpressionOrderByClauseLinkList { get; set; } = new();
    public List<QueryParenthesisExpression> QueryParenthesisExpressionList { get; set; } = new();
    public List<QueryParenthesisExpressionQueryExpressionLink> QueryParenthesisExpressionQueryExpressionLinkList { get; set; } = new();
    public List<QuerySpecification> QuerySpecificationList { get; set; } = new();
    public List<QuerySpecificationFromClauseLink> QuerySpecificationFromClauseLinkList { get; set; } = new();
    public List<QuerySpecificationGroupByClauseLink> QuerySpecificationGroupByClauseLinkList { get; set; } = new();
    public List<QuerySpecificationHavingClauseLink> QuerySpecificationHavingClauseLinkList { get; set; } = new();
    public List<QuerySpecificationSelectElementsItem> QuerySpecificationSelectElementsItemList { get; set; } = new();
    public List<QuerySpecificationTopRowFilterLink> QuerySpecificationTopRowFilterLinkList { get; set; } = new();
    public List<QuerySpecificationWhereClauseLink> QuerySpecificationWhereClauseLinkList { get; set; } = new();
    public List<QuerySpecificationWindowClauseLink> QuerySpecificationWindowClauseLinkList { get; set; } = new();
    public List<RealLiteral> RealLiteralList { get; set; } = new();
    public List<RightFunctionCall> RightFunctionCallList { get; set; } = new();
    public List<RightFunctionCallParametersItem> RightFunctionCallParametersItemList { get; set; } = new();
    public List<RollupGroupingSpecification> RollupGroupingSpecificationList { get; set; } = new();
    public List<RollupGroupingSpecificationArgumentsItem> RollupGroupingSpecificationArgumentsItemList { get; set; } = new();
    public List<RowValue> RowValueList { get; set; } = new();
    public List<RowValueColumnValuesItem> RowValueColumnValuesItemList { get; set; } = new();
    public List<ScalarExpression> ScalarExpressionList { get; set; } = new();
    public List<ScalarSubquery> ScalarSubqueryList { get; set; } = new();
    public List<ScalarSubqueryQueryExpressionLink> ScalarSubqueryQueryExpressionLinkList { get; set; } = new();
    public List<SchemaObjectFunctionTableReference> SchemaObjectFunctionTableReferenceList { get; set; } = new();
    public List<SchemaObjectFunctionTableReferenceParametersItem> SchemaObjectFunctionTableReferenceParametersItemList { get; set; } = new();
    public List<SchemaObjectFunctionTableReferenceSchemaObjectLink> SchemaObjectFunctionTableReferenceSchemaObjectLinkList { get; set; } = new();
    public List<SchemaObjectName> SchemaObjectNameList { get; set; } = new();
    public List<SchemaObjectNameBaseIdentifierLink> SchemaObjectNameBaseIdentifierLinkList { get; set; } = new();
    public List<SchemaObjectNameSchemaIdentifierLink> SchemaObjectNameSchemaIdentifierLinkList { get; set; } = new();
    public List<ScriptObjectScalarFunction> ScriptObjectScalarFunctionList { get; set; } = new();
    public List<ScriptObjectStoredProcedure> ScriptObjectStoredProcedureList { get; set; } = new();
    public List<ScriptObjectTVF> ScriptObjectTVFList { get; set; } = new();
    public List<ScriptObjectView> ScriptObjectViewList { get; set; } = new();
    public List<SearchedCaseExpression> SearchedCaseExpressionList { get; set; } = new();
    public List<SearchedCaseExpressionWhenClausesItem> SearchedCaseExpressionWhenClausesItemList { get; set; } = new();
    public List<SearchedWhenClause> SearchedWhenClauseList { get; set; } = new();
    public List<SearchedWhenClauseWhenExpressionLink> SearchedWhenClauseWhenExpressionLinkList { get; set; } = new();
    public List<SelectElement> SelectElementList { get; set; } = new();
    public List<SelectScalarExpression> SelectScalarExpressionList { get; set; } = new();
    public List<SelectScalarExpressionColumnNameLink> SelectScalarExpressionColumnNameLinkList { get; set; } = new();
    public List<SelectScalarExpressionExpressionLink> SelectScalarExpressionExpressionLinkList { get; set; } = new();
    public List<SelectStarExpression> SelectStarExpressionList { get; set; } = new();
    public List<SelectStarExpressionQualifierLink> SelectStarExpressionQualifierLinkList { get; set; } = new();
    public List<SelectStatement> SelectStatementList { get; set; } = new();
    public List<SelectStatementQueryExpressionLink> SelectStatementQueryExpressionLinkList { get; set; } = new();
    public List<SetAssignment> SetAssignmentList { get; set; } = new();
    public List<SetAssignmentTargetLink> SetAssignmentTargetLinkList { get; set; } = new();
    public List<SetAssignmentValueLink> SetAssignmentValueLinkList { get; set; } = new();
    public List<SetClause> SetClauseList { get; set; } = new();
    public List<SetClauseAssignmentsItem> SetClauseAssignmentsItemList { get; set; } = new();
    public List<SimpleCaseExpression> SimpleCaseExpressionList { get; set; } = new();
    public List<SimpleCaseExpressionInputExpressionLink> SimpleCaseExpressionInputExpressionLinkList { get; set; } = new();
    public List<SimpleCaseExpressionWhenClausesItem> SimpleCaseExpressionWhenClausesItemList { get; set; } = new();
    public List<SimpleWhenClause> SimpleWhenClauseList { get; set; } = new();
    public List<SimpleWhenClauseWhenExpressionLink> SimpleWhenClauseWhenExpressionLinkList { get; set; } = new();
    public List<SqlDataTypeReference> SqlDataTypeReferenceList { get; set; } = new();
    public List<SqlHint> SqlHintList { get; set; } = new();
    public List<SqlHintArgumentsItem> SqlHintArgumentsItemList { get; set; } = new();
    public List<SqlHintKeywordsItem> SqlHintKeywordsItemList { get; set; } = new();
    public List<StatementWithCtesAndXmlNamespaces> StatementWithCtesAndXmlNamespacesList { get; set; } = new();
    public List<StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLink> StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkList { get; set; } = new();
    public List<StoredProcedureContract> StoredProcedureContractList { get; set; } = new();
    public List<StoredProcedureContractOperation> StoredProcedureContractOperationList { get; set; } = new();
    public List<StoredProcedureResultColumnItem> StoredProcedureResultColumnItemList { get; set; } = new();
    public List<StoredProcedureResultRowsetItem> StoredProcedureResultRowsetItemList { get; set; } = new();
    public List<StringLiteral> StringLiteralList { get; set; } = new();
    public List<SubqueryComparisonPredicate> SubqueryComparisonPredicateList { get; set; } = new();
    public List<SubqueryComparisonPredicateExpressionLink> SubqueryComparisonPredicateExpressionLinkList { get; set; } = new();
    public List<SubqueryComparisonPredicateSubqueryLink> SubqueryComparisonPredicateSubqueryLinkList { get; set; } = new();
    public List<TableReference> TableReferenceList { get; set; } = new();
    public List<TableReferenceWithAlias> TableReferenceWithAliasList { get; set; } = new();
    public List<TableReferenceWithAliasAliasLink> TableReferenceWithAliasAliasLinkList { get; set; } = new();
    public List<TableReferenceWithAliasAndColumns> TableReferenceWithAliasAndColumnsList { get; set; } = new();
    public List<TableReferenceWithAliasAndColumnsColumnsItem> TableReferenceWithAliasAndColumnsColumnsItemList { get; set; } = new();
    public List<TableReferenceWithAliasTableHintsItem> TableReferenceWithAliasTableHintsItemList { get; set; } = new();
    public List<TableSampleClause> TableSampleClauseList { get; set; } = new();
    public List<TableSampleClauseRepeatSeedLink> TableSampleClauseRepeatSeedLinkList { get; set; } = new();
    public List<TableSampleClauseSampleNumberLink> TableSampleClauseSampleNumberLinkList { get; set; } = new();
    public List<TopRowFilter> TopRowFilterList { get; set; } = new();
    public List<TopRowFilterExpressionLink> TopRowFilterExpressionLinkList { get; set; } = new();
    public List<TransformScript> TransformScriptList { get; set; } = new();
    public List<TransformScriptFunctionParametersItem> TransformScriptFunctionParametersItemList { get; set; } = new();
    public List<TransformScriptObjectIdentifierLink> TransformScriptObjectIdentifierLinkList { get; set; } = new();
    public List<TransformScriptSchemaIdentifierLink> TransformScriptSchemaIdentifierLinkList { get; set; } = new();
    public List<TransformScriptStatementLink> TransformScriptStatementLinkList { get; set; } = new();
    public List<TransformScriptViewColumnsItem> TransformScriptViewColumnsItemList { get; set; } = new();
    public List<TruncateStatement> TruncateStatementList { get; set; } = new();
    public List<TruncateStatementTargetLink> TruncateStatementTargetLinkList { get; set; } = new();
    public List<TryCastCall> TryCastCallList { get; set; } = new();
    public List<TryCastCallDataTypeLink> TryCastCallDataTypeLinkList { get; set; } = new();
    public List<TryCastCallParameterLink> TryCastCallParameterLinkList { get; set; } = new();
    public List<TryConvertCall> TryConvertCallList { get; set; } = new();
    public List<TryConvertCallDataTypeLink> TryConvertCallDataTypeLinkList { get; set; } = new();
    public List<TryConvertCallParameterLink> TryConvertCallParameterLinkList { get; set; } = new();
    public List<TryConvertCallStyleLink> TryConvertCallStyleLinkList { get; set; } = new();
    public List<TryParseCall> TryParseCallList { get; set; } = new();
    public List<TryParseCallCultureLink> TryParseCallCultureLinkList { get; set; } = new();
    public List<TryParseCallDataTypeLink> TryParseCallDataTypeLinkList { get; set; } = new();
    public List<TryParseCallStringValueLink> TryParseCallStringValueLinkList { get; set; } = new();
    public List<TSqlStatement> TSqlStatementList { get; set; } = new();
    public List<UnaryExpression> UnaryExpressionList { get; set; } = new();
    public List<UnaryExpressionExpressionLink> UnaryExpressionExpressionLinkList { get; set; } = new();
    public List<UnpivotedTableReference> UnpivotedTableReferenceList { get; set; } = new();
    public List<UnpivotedTableReferenceInColumnsItem> UnpivotedTableReferenceInColumnsItemList { get; set; } = new();
    public List<UnpivotedTableReferencePivotColumnLink> UnpivotedTableReferencePivotColumnLinkList { get; set; } = new();
    public List<UnpivotedTableReferenceTableReferenceLink> UnpivotedTableReferenceTableReferenceLinkList { get; set; } = new();
    public List<UnpivotedTableReferenceValueColumnLink> UnpivotedTableReferenceValueColumnLinkList { get; set; } = new();
    public List<UnqualifiedJoin> UnqualifiedJoinList { get; set; } = new();
    public List<UpdateStatement> UpdateStatementList { get; set; } = new();
    public List<UpdateStatementFromClauseLink> UpdateStatementFromClauseLinkList { get; set; } = new();
    public List<UpdateStatementSetClauseLink> UpdateStatementSetClauseLinkList { get; set; } = new();
    public List<UpdateStatementTargetAliasLink> UpdateStatementTargetAliasLinkList { get; set; } = new();
    public List<UpdateStatementTargetLink> UpdateStatementTargetLinkList { get; set; } = new();
    public List<UpdateStatementWhereClauseLink> UpdateStatementWhereClauseLinkList { get; set; } = new();
    public List<ValueExpression> ValueExpressionList { get; set; } = new();
    public List<WhenClause> WhenClauseList { get; set; } = new();
    public List<WhenClauseThenExpressionLink> WhenClauseThenExpressionLinkList { get; set; } = new();
    public List<WhereClause> WhereClauseList { get; set; } = new();
    public List<WhereClauseSearchConditionLink> WhereClauseSearchConditionLinkList { get; set; } = new();
    public List<WindowClause> WindowClauseList { get; set; } = new();
    public List<WindowClauseWindowDefinitionItem> WindowClauseWindowDefinitionItemList { get; set; } = new();
    public List<WindowDefinition> WindowDefinitionList { get; set; } = new();
    public List<WindowDefinitionOrderByClauseLink> WindowDefinitionOrderByClauseLinkList { get; set; } = new();
    public List<WindowDefinitionPartitionsItem> WindowDefinitionPartitionsItemList { get; set; } = new();
    public List<WindowDefinitionRefWindowNameLink> WindowDefinitionRefWindowNameLinkList { get; set; } = new();
    public List<WindowDefinitionWindowFrameClauseLink> WindowDefinitionWindowFrameClauseLinkList { get; set; } = new();
    public List<WindowDefinitionWindowNameLink> WindowDefinitionWindowNameLinkList { get; set; } = new();
    public List<WindowDelimiter> WindowDelimiterList { get; set; } = new();
    public List<WindowDelimiterOffsetValueLink> WindowDelimiterOffsetValueLinkList { get; set; } = new();
    public List<WindowFrameClause> WindowFrameClauseList { get; set; } = new();
    public List<WindowFrameClauseBottomLink> WindowFrameClauseBottomLinkList { get; set; } = new();
    public List<WindowFrameClauseTopLink> WindowFrameClauseTopLinkList { get; set; } = new();
    public List<WithCtesAndXmlNamespaces> WithCtesAndXmlNamespacesList { get; set; } = new();
    public List<WithCtesAndXmlNamespacesCommonTableExpressionsItem> WithCtesAndXmlNamespacesCommonTableExpressionsItemList { get; set; } = new();
    public List<WithCtesAndXmlNamespacesXmlNamespacesLink> WithCtesAndXmlNamespacesXmlNamespacesLinkList { get; set; } = new();
    public List<XmlNamespaces> XmlNamespacesList { get; set; } = new();
    public List<XmlNamespacesAliasElement> XmlNamespacesAliasElementList { get; set; } = new();
    public List<XmlNamespacesAliasElementIdentifierLink> XmlNamespacesAliasElementIdentifierLinkList { get; set; } = new();
    public List<XmlNamespacesDefaultElement> XmlNamespacesDefaultElementList { get; set; } = new();
    public List<XmlNamespacesElement> XmlNamespacesElementList { get; set; } = new();
    public List<XmlNamespacesElementStringLink> XmlNamespacesElementStringLinkList { get; set; } = new();
    public List<XmlNamespacesXmlNamespacesElementsItem> XmlNamespacesXmlNamespacesElementsItemList { get; set; } = new();
    public List<XmlNodesTableReference> XmlNodesTableReferenceList { get; set; } = new();
    public List<XmlNodesTableReferenceTargetExpressionLink> XmlNodesTableReferenceTargetExpressionLinkList { get; set; } = new();
    public List<XmlNodesTableReferenceXQueryStringLink> XmlNodesTableReferenceXQueryStringLinkList { get; set; } = new();
}

public static partial class MetaTransformScriptInstance
{
    private static readonly MetaTransformScriptModel _builtIn = CreateBuiltIn();
    public static MetaTransformScriptModel BuiltIn => _builtIn;

    public static MetaTransformScriptModel CreateBuiltIn()
    {
        var model = MetaTransformScriptModel.CreateEmpty();
        return model;
    }
}