using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Meta.Core.Serialization;

namespace MetaTransformScript
{
    [XmlRoot("MetaTransformScript")]
    public sealed partial class MetaTransformScriptModel
    {
        public static MetaTransformScriptModel CreateEmpty() => new();

        [XmlArray("AtTimeZoneCallList")]
        [XmlArrayItem("AtTimeZoneCall")]
        public List<AtTimeZoneCall> AtTimeZoneCallList { get; set; } = new();
        public bool ShouldSerializeAtTimeZoneCallList() => AtTimeZoneCallList.Count > 0;

        [XmlArray("AtTimeZoneCallDateValueLinkList")]
        [XmlArrayItem("AtTimeZoneCallDateValueLink")]
        public List<AtTimeZoneCallDateValueLink> AtTimeZoneCallDateValueLinkList { get; set; } = new();
        public bool ShouldSerializeAtTimeZoneCallDateValueLinkList() => AtTimeZoneCallDateValueLinkList.Count > 0;

        [XmlArray("AtTimeZoneCallTimeZoneLinkList")]
        [XmlArrayItem("AtTimeZoneCallTimeZoneLink")]
        public List<AtTimeZoneCallTimeZoneLink> AtTimeZoneCallTimeZoneLinkList { get; set; } = new();
        public bool ShouldSerializeAtTimeZoneCallTimeZoneLinkList() => AtTimeZoneCallTimeZoneLinkList.Count > 0;

        [XmlArray("BinaryExpressionList")]
        [XmlArrayItem("BinaryExpression")]
        public List<BinaryExpression> BinaryExpressionList { get; set; } = new();
        public bool ShouldSerializeBinaryExpressionList() => BinaryExpressionList.Count > 0;

        [XmlArray("BinaryExpressionFirstExpressionLinkList")]
        [XmlArrayItem("BinaryExpressionFirstExpressionLink")]
        public List<BinaryExpressionFirstExpressionLink> BinaryExpressionFirstExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeBinaryExpressionFirstExpressionLinkList() => BinaryExpressionFirstExpressionLinkList.Count > 0;

        [XmlArray("BinaryExpressionSecondExpressionLinkList")]
        [XmlArrayItem("BinaryExpressionSecondExpressionLink")]
        public List<BinaryExpressionSecondExpressionLink> BinaryExpressionSecondExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeBinaryExpressionSecondExpressionLinkList() => BinaryExpressionSecondExpressionLinkList.Count > 0;

        [XmlArray("BinaryLiteralList")]
        [XmlArrayItem("BinaryLiteral")]
        public List<BinaryLiteral> BinaryLiteralList { get; set; } = new();
        public bool ShouldSerializeBinaryLiteralList() => BinaryLiteralList.Count > 0;

        [XmlArray("BinaryQueryExpressionList")]
        [XmlArrayItem("BinaryQueryExpression")]
        public List<BinaryQueryExpression> BinaryQueryExpressionList { get; set; } = new();
        public bool ShouldSerializeBinaryQueryExpressionList() => BinaryQueryExpressionList.Count > 0;

        [XmlArray("BinaryQueryExpressionFirstQueryExpressionLinkList")]
        [XmlArrayItem("BinaryQueryExpressionFirstQueryExpressionLink")]
        public List<BinaryQueryExpressionFirstQueryExpressionLink> BinaryQueryExpressionFirstQueryExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeBinaryQueryExpressionFirstQueryExpressionLinkList() => BinaryQueryExpressionFirstQueryExpressionLinkList.Count > 0;

        [XmlArray("BinaryQueryExpressionSecondQueryExpressionLinkList")]
        [XmlArrayItem("BinaryQueryExpressionSecondQueryExpressionLink")]
        public List<BinaryQueryExpressionSecondQueryExpressionLink> BinaryQueryExpressionSecondQueryExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeBinaryQueryExpressionSecondQueryExpressionLinkList() => BinaryQueryExpressionSecondQueryExpressionLinkList.Count > 0;

        [XmlArray("BooleanBinaryExpressionList")]
        [XmlArrayItem("BooleanBinaryExpression")]
        public List<BooleanBinaryExpression> BooleanBinaryExpressionList { get; set; } = new();
        public bool ShouldSerializeBooleanBinaryExpressionList() => BooleanBinaryExpressionList.Count > 0;

        [XmlArray("BooleanBinaryExpressionFirstExpressionLinkList")]
        [XmlArrayItem("BooleanBinaryExpressionFirstExpressionLink")]
        public List<BooleanBinaryExpressionFirstExpressionLink> BooleanBinaryExpressionFirstExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeBooleanBinaryExpressionFirstExpressionLinkList() => BooleanBinaryExpressionFirstExpressionLinkList.Count > 0;

        [XmlArray("BooleanBinaryExpressionSecondExpressionLinkList")]
        [XmlArrayItem("BooleanBinaryExpressionSecondExpressionLink")]
        public List<BooleanBinaryExpressionSecondExpressionLink> BooleanBinaryExpressionSecondExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeBooleanBinaryExpressionSecondExpressionLinkList() => BooleanBinaryExpressionSecondExpressionLinkList.Count > 0;

        [XmlArray("BooleanComparisonExpressionList")]
        [XmlArrayItem("BooleanComparisonExpression")]
        public List<BooleanComparisonExpression> BooleanComparisonExpressionList { get; set; } = new();
        public bool ShouldSerializeBooleanComparisonExpressionList() => BooleanComparisonExpressionList.Count > 0;

        [XmlArray("BooleanComparisonExpressionFirstExpressionLinkList")]
        [XmlArrayItem("BooleanComparisonExpressionFirstExpressionLink")]
        public List<BooleanComparisonExpressionFirstExpressionLink> BooleanComparisonExpressionFirstExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeBooleanComparisonExpressionFirstExpressionLinkList() => BooleanComparisonExpressionFirstExpressionLinkList.Count > 0;

        [XmlArray("BooleanComparisonExpressionSecondExpressionLinkList")]
        [XmlArrayItem("BooleanComparisonExpressionSecondExpressionLink")]
        public List<BooleanComparisonExpressionSecondExpressionLink> BooleanComparisonExpressionSecondExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeBooleanComparisonExpressionSecondExpressionLinkList() => BooleanComparisonExpressionSecondExpressionLinkList.Count > 0;

        [XmlArray("BooleanExpressionList")]
        [XmlArrayItem("BooleanExpression")]
        public List<BooleanExpression> BooleanExpressionList { get; set; } = new();
        public bool ShouldSerializeBooleanExpressionList() => BooleanExpressionList.Count > 0;

        [XmlArray("BooleanIsNullExpressionList")]
        [XmlArrayItem("BooleanIsNullExpression")]
        public List<BooleanIsNullExpression> BooleanIsNullExpressionList { get; set; } = new();
        public bool ShouldSerializeBooleanIsNullExpressionList() => BooleanIsNullExpressionList.Count > 0;

        [XmlArray("BooleanIsNullExpressionExpressionLinkList")]
        [XmlArrayItem("BooleanIsNullExpressionExpressionLink")]
        public List<BooleanIsNullExpressionExpressionLink> BooleanIsNullExpressionExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeBooleanIsNullExpressionExpressionLinkList() => BooleanIsNullExpressionExpressionLinkList.Count > 0;

        [XmlArray("BooleanNotExpressionList")]
        [XmlArrayItem("BooleanNotExpression")]
        public List<BooleanNotExpression> BooleanNotExpressionList { get; set; } = new();
        public bool ShouldSerializeBooleanNotExpressionList() => BooleanNotExpressionList.Count > 0;

        [XmlArray("BooleanNotExpressionExpressionLinkList")]
        [XmlArrayItem("BooleanNotExpressionExpressionLink")]
        public List<BooleanNotExpressionExpressionLink> BooleanNotExpressionExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeBooleanNotExpressionExpressionLinkList() => BooleanNotExpressionExpressionLinkList.Count > 0;

        [XmlArray("BooleanParenthesisExpressionList")]
        [XmlArrayItem("BooleanParenthesisExpression")]
        public List<BooleanParenthesisExpression> BooleanParenthesisExpressionList { get; set; } = new();
        public bool ShouldSerializeBooleanParenthesisExpressionList() => BooleanParenthesisExpressionList.Count > 0;

        [XmlArray("BooleanParenthesisExpressionExpressionLinkList")]
        [XmlArrayItem("BooleanParenthesisExpressionExpressionLink")]
        public List<BooleanParenthesisExpressionExpressionLink> BooleanParenthesisExpressionExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeBooleanParenthesisExpressionExpressionLinkList() => BooleanParenthesisExpressionExpressionLinkList.Count > 0;

        [XmlArray("BooleanTernaryExpressionList")]
        [XmlArrayItem("BooleanTernaryExpression")]
        public List<BooleanTernaryExpression> BooleanTernaryExpressionList { get; set; } = new();
        public bool ShouldSerializeBooleanTernaryExpressionList() => BooleanTernaryExpressionList.Count > 0;

        [XmlArray("BooleanTernaryExpressionFirstExpressionLinkList")]
        [XmlArrayItem("BooleanTernaryExpressionFirstExpressionLink")]
        public List<BooleanTernaryExpressionFirstExpressionLink> BooleanTernaryExpressionFirstExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeBooleanTernaryExpressionFirstExpressionLinkList() => BooleanTernaryExpressionFirstExpressionLinkList.Count > 0;

        [XmlArray("BooleanTernaryExpressionSecondExpressionLinkList")]
        [XmlArrayItem("BooleanTernaryExpressionSecondExpressionLink")]
        public List<BooleanTernaryExpressionSecondExpressionLink> BooleanTernaryExpressionSecondExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeBooleanTernaryExpressionSecondExpressionLinkList() => BooleanTernaryExpressionSecondExpressionLinkList.Count > 0;

        [XmlArray("BooleanTernaryExpressionThirdExpressionLinkList")]
        [XmlArrayItem("BooleanTernaryExpressionThirdExpressionLink")]
        public List<BooleanTernaryExpressionThirdExpressionLink> BooleanTernaryExpressionThirdExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeBooleanTernaryExpressionThirdExpressionLinkList() => BooleanTernaryExpressionThirdExpressionLinkList.Count > 0;

        [XmlArray("CallTargetList")]
        [XmlArrayItem("CallTarget")]
        public List<CallTarget> CallTargetList { get; set; } = new();
        public bool ShouldSerializeCallTargetList() => CallTargetList.Count > 0;

        [XmlArray("CaseExpressionList")]
        [XmlArrayItem("CaseExpression")]
        public List<CaseExpression> CaseExpressionList { get; set; } = new();
        public bool ShouldSerializeCaseExpressionList() => CaseExpressionList.Count > 0;

        [XmlArray("CaseExpressionElseExpressionLinkList")]
        [XmlArrayItem("CaseExpressionElseExpressionLink")]
        public List<CaseExpressionElseExpressionLink> CaseExpressionElseExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeCaseExpressionElseExpressionLinkList() => CaseExpressionElseExpressionLinkList.Count > 0;

        [XmlArray("CastCallList")]
        [XmlArrayItem("CastCall")]
        public List<CastCall> CastCallList { get; set; } = new();
        public bool ShouldSerializeCastCallList() => CastCallList.Count > 0;

        [XmlArray("CastCallDataTypeLinkList")]
        [XmlArrayItem("CastCallDataTypeLink")]
        public List<CastCallDataTypeLink> CastCallDataTypeLinkList { get; set; } = new();
        public bool ShouldSerializeCastCallDataTypeLinkList() => CastCallDataTypeLinkList.Count > 0;

        [XmlArray("CastCallParameterLinkList")]
        [XmlArrayItem("CastCallParameterLink")]
        public List<CastCallParameterLink> CastCallParameterLinkList { get; set; } = new();
        public bool ShouldSerializeCastCallParameterLinkList() => CastCallParameterLinkList.Count > 0;

        [XmlArray("CoalesceExpressionList")]
        [XmlArrayItem("CoalesceExpression")]
        public List<CoalesceExpression> CoalesceExpressionList { get; set; } = new();
        public bool ShouldSerializeCoalesceExpressionList() => CoalesceExpressionList.Count > 0;

        [XmlArray("CoalesceExpressionExpressionsItemList")]
        [XmlArrayItem("CoalesceExpressionExpressionsItem")]
        public List<CoalesceExpressionExpressionsItem> CoalesceExpressionExpressionsItemList { get; set; } = new();
        public bool ShouldSerializeCoalesceExpressionExpressionsItemList() => CoalesceExpressionExpressionsItemList.Count > 0;

        [XmlArray("ColumnReferenceExpressionList")]
        [XmlArrayItem("ColumnReferenceExpression")]
        public List<ColumnReferenceExpression> ColumnReferenceExpressionList { get; set; } = new();
        public bool ShouldSerializeColumnReferenceExpressionList() => ColumnReferenceExpressionList.Count > 0;

        [XmlArray("ColumnReferenceExpressionMultiPartIdentifierLinkList")]
        [XmlArrayItem("ColumnReferenceExpressionMultiPartIdentifierLink")]
        public List<ColumnReferenceExpressionMultiPartIdentifierLink> ColumnReferenceExpressionMultiPartIdentifierLinkList { get; set; } = new();
        public bool ShouldSerializeColumnReferenceExpressionMultiPartIdentifierLinkList() => ColumnReferenceExpressionMultiPartIdentifierLinkList.Count > 0;

        [XmlArray("CommonTableExpressionList")]
        [XmlArrayItem("CommonTableExpression")]
        public List<CommonTableExpression> CommonTableExpressionList { get; set; } = new();
        public bool ShouldSerializeCommonTableExpressionList() => CommonTableExpressionList.Count > 0;

        [XmlArray("CommonTableExpressionColumnsItemList")]
        [XmlArrayItem("CommonTableExpressionColumnsItem")]
        public List<CommonTableExpressionColumnsItem> CommonTableExpressionColumnsItemList { get; set; } = new();
        public bool ShouldSerializeCommonTableExpressionColumnsItemList() => CommonTableExpressionColumnsItemList.Count > 0;

        [XmlArray("CommonTableExpressionExpressionNameLinkList")]
        [XmlArrayItem("CommonTableExpressionExpressionNameLink")]
        public List<CommonTableExpressionExpressionNameLink> CommonTableExpressionExpressionNameLinkList { get; set; } = new();
        public bool ShouldSerializeCommonTableExpressionExpressionNameLinkList() => CommonTableExpressionExpressionNameLinkList.Count > 0;

        [XmlArray("CommonTableExpressionQueryExpressionLinkList")]
        [XmlArrayItem("CommonTableExpressionQueryExpressionLink")]
        public List<CommonTableExpressionQueryExpressionLink> CommonTableExpressionQueryExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeCommonTableExpressionQueryExpressionLinkList() => CommonTableExpressionQueryExpressionLinkList.Count > 0;

        [XmlArray("CompositeGroupingSpecificationList")]
        [XmlArrayItem("CompositeGroupingSpecification")]
        public List<CompositeGroupingSpecification> CompositeGroupingSpecificationList { get; set; } = new();
        public bool ShouldSerializeCompositeGroupingSpecificationList() => CompositeGroupingSpecificationList.Count > 0;

        [XmlArray("CompositeGroupingSpecificationItemsItemList")]
        [XmlArrayItem("CompositeGroupingSpecificationItemsItem")]
        public List<CompositeGroupingSpecificationItemsItem> CompositeGroupingSpecificationItemsItemList { get; set; } = new();
        public bool ShouldSerializeCompositeGroupingSpecificationItemsItemList() => CompositeGroupingSpecificationItemsItemList.Count > 0;

        [XmlArray("ConvertCallList")]
        [XmlArrayItem("ConvertCall")]
        public List<ConvertCall> ConvertCallList { get; set; } = new();
        public bool ShouldSerializeConvertCallList() => ConvertCallList.Count > 0;

        [XmlArray("ConvertCallDataTypeLinkList")]
        [XmlArrayItem("ConvertCallDataTypeLink")]
        public List<ConvertCallDataTypeLink> ConvertCallDataTypeLinkList { get; set; } = new();
        public bool ShouldSerializeConvertCallDataTypeLinkList() => ConvertCallDataTypeLinkList.Count > 0;

        [XmlArray("ConvertCallParameterLinkList")]
        [XmlArrayItem("ConvertCallParameterLink")]
        public List<ConvertCallParameterLink> ConvertCallParameterLinkList { get; set; } = new();
        public bool ShouldSerializeConvertCallParameterLinkList() => ConvertCallParameterLinkList.Count > 0;

        [XmlArray("ConvertCallStyleLinkList")]
        [XmlArrayItem("ConvertCallStyleLink")]
        public List<ConvertCallStyleLink> ConvertCallStyleLinkList { get; set; } = new();
        public bool ShouldSerializeConvertCallStyleLinkList() => ConvertCallStyleLinkList.Count > 0;

        [XmlArray("CubeGroupingSpecificationList")]
        [XmlArrayItem("CubeGroupingSpecification")]
        public List<CubeGroupingSpecification> CubeGroupingSpecificationList { get; set; } = new();
        public bool ShouldSerializeCubeGroupingSpecificationList() => CubeGroupingSpecificationList.Count > 0;

        [XmlArray("CubeGroupingSpecificationArgumentsItemList")]
        [XmlArrayItem("CubeGroupingSpecificationArgumentsItem")]
        public List<CubeGroupingSpecificationArgumentsItem> CubeGroupingSpecificationArgumentsItemList { get; set; } = new();
        public bool ShouldSerializeCubeGroupingSpecificationArgumentsItemList() => CubeGroupingSpecificationArgumentsItemList.Count > 0;

        [XmlArray("DataTypeReferenceList")]
        [XmlArrayItem("DataTypeReference")]
        public List<DataTypeReference> DataTypeReferenceList { get; set; } = new();
        public bool ShouldSerializeDataTypeReferenceList() => DataTypeReferenceList.Count > 0;

        [XmlArray("DataTypeReferenceNameLinkList")]
        [XmlArrayItem("DataTypeReferenceNameLink")]
        public List<DataTypeReferenceNameLink> DataTypeReferenceNameLinkList { get; set; } = new();
        public bool ShouldSerializeDataTypeReferenceNameLinkList() => DataTypeReferenceNameLinkList.Count > 0;

        [XmlArray("DistinctPredicateList")]
        [XmlArrayItem("DistinctPredicate")]
        public List<DistinctPredicate> DistinctPredicateList { get; set; } = new();
        public bool ShouldSerializeDistinctPredicateList() => DistinctPredicateList.Count > 0;

        [XmlArray("DistinctPredicateFirstExpressionLinkList")]
        [XmlArrayItem("DistinctPredicateFirstExpressionLink")]
        public List<DistinctPredicateFirstExpressionLink> DistinctPredicateFirstExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeDistinctPredicateFirstExpressionLinkList() => DistinctPredicateFirstExpressionLinkList.Count > 0;

        [XmlArray("DistinctPredicateSecondExpressionLinkList")]
        [XmlArrayItem("DistinctPredicateSecondExpressionLink")]
        public List<DistinctPredicateSecondExpressionLink> DistinctPredicateSecondExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeDistinctPredicateSecondExpressionLinkList() => DistinctPredicateSecondExpressionLinkList.Count > 0;

        [XmlArray("ExistsPredicateList")]
        [XmlArrayItem("ExistsPredicate")]
        public List<ExistsPredicate> ExistsPredicateList { get; set; } = new();
        public bool ShouldSerializeExistsPredicateList() => ExistsPredicateList.Count > 0;

        [XmlArray("ExistsPredicateSubqueryLinkList")]
        [XmlArrayItem("ExistsPredicateSubqueryLink")]
        public List<ExistsPredicateSubqueryLink> ExistsPredicateSubqueryLinkList { get; set; } = new();
        public bool ShouldSerializeExistsPredicateSubqueryLinkList() => ExistsPredicateSubqueryLinkList.Count > 0;

        [XmlArray("ExpressionGroupingSpecificationList")]
        [XmlArrayItem("ExpressionGroupingSpecification")]
        public List<ExpressionGroupingSpecification> ExpressionGroupingSpecificationList { get; set; } = new();
        public bool ShouldSerializeExpressionGroupingSpecificationList() => ExpressionGroupingSpecificationList.Count > 0;

        [XmlArray("ExpressionGroupingSpecificationExpressionLinkList")]
        [XmlArrayItem("ExpressionGroupingSpecificationExpressionLink")]
        public List<ExpressionGroupingSpecificationExpressionLink> ExpressionGroupingSpecificationExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeExpressionGroupingSpecificationExpressionLinkList() => ExpressionGroupingSpecificationExpressionLinkList.Count > 0;

        [XmlArray("ExpressionWithSortOrderList")]
        [XmlArrayItem("ExpressionWithSortOrder")]
        public List<ExpressionWithSortOrder> ExpressionWithSortOrderList { get; set; } = new();
        public bool ShouldSerializeExpressionWithSortOrderList() => ExpressionWithSortOrderList.Count > 0;

        [XmlArray("ExpressionWithSortOrderExpressionLinkList")]
        [XmlArrayItem("ExpressionWithSortOrderExpressionLink")]
        public List<ExpressionWithSortOrderExpressionLink> ExpressionWithSortOrderExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeExpressionWithSortOrderExpressionLinkList() => ExpressionWithSortOrderExpressionLinkList.Count > 0;

        [XmlArray("FromClauseList")]
        [XmlArrayItem("FromClause")]
        public List<FromClause> FromClauseList { get; set; } = new();
        public bool ShouldSerializeFromClauseList() => FromClauseList.Count > 0;

        [XmlArray("FromClauseTableReferencesItemList")]
        [XmlArrayItem("FromClauseTableReferencesItem")]
        public List<FromClauseTableReferencesItem> FromClauseTableReferencesItemList { get; set; } = new();
        public bool ShouldSerializeFromClauseTableReferencesItemList() => FromClauseTableReferencesItemList.Count > 0;

        [XmlArray("FullTextPredicateList")]
        [XmlArrayItem("FullTextPredicate")]
        public List<FullTextPredicate> FullTextPredicateList { get; set; } = new();
        public bool ShouldSerializeFullTextPredicateList() => FullTextPredicateList.Count > 0;

        [XmlArray("FullTextPredicateColumnsItemList")]
        [XmlArrayItem("FullTextPredicateColumnsItem")]
        public List<FullTextPredicateColumnsItem> FullTextPredicateColumnsItemList { get; set; } = new();
        public bool ShouldSerializeFullTextPredicateColumnsItemList() => FullTextPredicateColumnsItemList.Count > 0;

        [XmlArray("FullTextPredicateValueLinkList")]
        [XmlArrayItem("FullTextPredicateValueLink")]
        public List<FullTextPredicateValueLink> FullTextPredicateValueLinkList { get; set; } = new();
        public bool ShouldSerializeFullTextPredicateValueLinkList() => FullTextPredicateValueLinkList.Count > 0;

        [XmlArray("FullTextTableReferenceList")]
        [XmlArrayItem("FullTextTableReference")]
        public List<FullTextTableReference> FullTextTableReferenceList { get; set; } = new();
        public bool ShouldSerializeFullTextTableReferenceList() => FullTextTableReferenceList.Count > 0;

        [XmlArray("FullTextTableReferenceColumnsItemList")]
        [XmlArrayItem("FullTextTableReferenceColumnsItem")]
        public List<FullTextTableReferenceColumnsItem> FullTextTableReferenceColumnsItemList { get; set; } = new();
        public bool ShouldSerializeFullTextTableReferenceColumnsItemList() => FullTextTableReferenceColumnsItemList.Count > 0;

        [XmlArray("FullTextTableReferenceSearchConditionLinkList")]
        [XmlArrayItem("FullTextTableReferenceSearchConditionLink")]
        public List<FullTextTableReferenceSearchConditionLink> FullTextTableReferenceSearchConditionLinkList { get; set; } = new();
        public bool ShouldSerializeFullTextTableReferenceSearchConditionLinkList() => FullTextTableReferenceSearchConditionLinkList.Count > 0;

        [XmlArray("FullTextTableReferenceTableNameLinkList")]
        [XmlArrayItem("FullTextTableReferenceTableNameLink")]
        public List<FullTextTableReferenceTableNameLink> FullTextTableReferenceTableNameLinkList { get; set; } = new();
        public bool ShouldSerializeFullTextTableReferenceTableNameLinkList() => FullTextTableReferenceTableNameLinkList.Count > 0;

        [XmlArray("FunctionCallList")]
        [XmlArrayItem("FunctionCall")]
        public List<FunctionCall> FunctionCallList { get; set; } = new();
        public bool ShouldSerializeFunctionCallList() => FunctionCallList.Count > 0;

        [XmlArray("FunctionCallCallTargetLinkList")]
        [XmlArrayItem("FunctionCallCallTargetLink")]
        public List<FunctionCallCallTargetLink> FunctionCallCallTargetLinkList { get; set; } = new();
        public bool ShouldSerializeFunctionCallCallTargetLinkList() => FunctionCallCallTargetLinkList.Count > 0;

        [XmlArray("FunctionCallFunctionNameLinkList")]
        [XmlArrayItem("FunctionCallFunctionNameLink")]
        public List<FunctionCallFunctionNameLink> FunctionCallFunctionNameLinkList { get; set; } = new();
        public bool ShouldSerializeFunctionCallFunctionNameLinkList() => FunctionCallFunctionNameLinkList.Count > 0;

        [XmlArray("FunctionCallOverClauseLinkList")]
        [XmlArrayItem("FunctionCallOverClauseLink")]
        public List<FunctionCallOverClauseLink> FunctionCallOverClauseLinkList { get; set; } = new();
        public bool ShouldSerializeFunctionCallOverClauseLinkList() => FunctionCallOverClauseLinkList.Count > 0;

        [XmlArray("FunctionCallParametersItemList")]
        [XmlArrayItem("FunctionCallParametersItem")]
        public List<FunctionCallParametersItem> FunctionCallParametersItemList { get; set; } = new();
        public bool ShouldSerializeFunctionCallParametersItemList() => FunctionCallParametersItemList.Count > 0;

        [XmlArray("GlobalFunctionTableReferenceList")]
        [XmlArrayItem("GlobalFunctionTableReference")]
        public List<GlobalFunctionTableReference> GlobalFunctionTableReferenceList { get; set; } = new();
        public bool ShouldSerializeGlobalFunctionTableReferenceList() => GlobalFunctionTableReferenceList.Count > 0;

        [XmlArray("GlobalFunctionTableReferenceNameLinkList")]
        [XmlArrayItem("GlobalFunctionTableReferenceNameLink")]
        public List<GlobalFunctionTableReferenceNameLink> GlobalFunctionTableReferenceNameLinkList { get; set; } = new();
        public bool ShouldSerializeGlobalFunctionTableReferenceNameLinkList() => GlobalFunctionTableReferenceNameLinkList.Count > 0;

        [XmlArray("GlobalFunctionTableReferenceParametersItemList")]
        [XmlArrayItem("GlobalFunctionTableReferenceParametersItem")]
        public List<GlobalFunctionTableReferenceParametersItem> GlobalFunctionTableReferenceParametersItemList { get; set; } = new();
        public bool ShouldSerializeGlobalFunctionTableReferenceParametersItemList() => GlobalFunctionTableReferenceParametersItemList.Count > 0;

        [XmlArray("GlobalVariableExpressionList")]
        [XmlArrayItem("GlobalVariableExpression")]
        public List<GlobalVariableExpression> GlobalVariableExpressionList { get; set; } = new();
        public bool ShouldSerializeGlobalVariableExpressionList() => GlobalVariableExpressionList.Count > 0;

        [XmlArray("GrandTotalGroupingSpecificationList")]
        [XmlArrayItem("GrandTotalGroupingSpecification")]
        public List<GrandTotalGroupingSpecification> GrandTotalGroupingSpecificationList { get; set; } = new();
        public bool ShouldSerializeGrandTotalGroupingSpecificationList() => GrandTotalGroupingSpecificationList.Count > 0;

        [XmlArray("GroupByClauseList")]
        [XmlArrayItem("GroupByClause")]
        public List<GroupByClause> GroupByClauseList { get; set; } = new();
        public bool ShouldSerializeGroupByClauseList() => GroupByClauseList.Count > 0;

        [XmlArray("GroupByClauseGroupingSpecificationsItemList")]
        [XmlArrayItem("GroupByClauseGroupingSpecificationsItem")]
        public List<GroupByClauseGroupingSpecificationsItem> GroupByClauseGroupingSpecificationsItemList { get; set; } = new();
        public bool ShouldSerializeGroupByClauseGroupingSpecificationsItemList() => GroupByClauseGroupingSpecificationsItemList.Count > 0;

        [XmlArray("GroupingSetsGroupingSpecificationList")]
        [XmlArrayItem("GroupingSetsGroupingSpecification")]
        public List<GroupingSetsGroupingSpecification> GroupingSetsGroupingSpecificationList { get; set; } = new();
        public bool ShouldSerializeGroupingSetsGroupingSpecificationList() => GroupingSetsGroupingSpecificationList.Count > 0;

        [XmlArray("GroupingSetsGroupingSpecificationSetsItemList")]
        [XmlArrayItem("GroupingSetsGroupingSpecificationSetsItem")]
        public List<GroupingSetsGroupingSpecificationSetsItem> GroupingSetsGroupingSpecificationSetsItemList { get; set; } = new();
        public bool ShouldSerializeGroupingSetsGroupingSpecificationSetsItemList() => GroupingSetsGroupingSpecificationSetsItemList.Count > 0;

        [XmlArray("GroupingSpecificationList")]
        [XmlArrayItem("GroupingSpecification")]
        public List<GroupingSpecification> GroupingSpecificationList { get; set; } = new();
        public bool ShouldSerializeGroupingSpecificationList() => GroupingSpecificationList.Count > 0;

        [XmlArray("HavingClauseList")]
        [XmlArrayItem("HavingClause")]
        public List<HavingClause> HavingClauseList { get; set; } = new();
        public bool ShouldSerializeHavingClauseList() => HavingClauseList.Count > 0;

        [XmlArray("HavingClauseSearchConditionLinkList")]
        [XmlArrayItem("HavingClauseSearchConditionLink")]
        public List<HavingClauseSearchConditionLink> HavingClauseSearchConditionLinkList { get; set; } = new();
        public bool ShouldSerializeHavingClauseSearchConditionLinkList() => HavingClauseSearchConditionLinkList.Count > 0;

        [XmlArray("IdentifierList")]
        [XmlArrayItem("Identifier")]
        public List<Identifier> IdentifierList { get; set; } = new();
        public bool ShouldSerializeIdentifierList() => IdentifierList.Count > 0;

        [XmlArray("IdentifierOrValueExpressionList")]
        [XmlArrayItem("IdentifierOrValueExpression")]
        public List<IdentifierOrValueExpression> IdentifierOrValueExpressionList { get; set; } = new();
        public bool ShouldSerializeIdentifierOrValueExpressionList() => IdentifierOrValueExpressionList.Count > 0;

        [XmlArray("IdentifierOrValueExpressionIdentifierLinkList")]
        [XmlArrayItem("IdentifierOrValueExpressionIdentifierLink")]
        public List<IdentifierOrValueExpressionIdentifierLink> IdentifierOrValueExpressionIdentifierLinkList { get; set; } = new();
        public bool ShouldSerializeIdentifierOrValueExpressionIdentifierLinkList() => IdentifierOrValueExpressionIdentifierLinkList.Count > 0;

        [XmlArray("IIfCallList")]
        [XmlArrayItem("IIfCall")]
        public List<IIfCall> IIfCallList { get; set; } = new();
        public bool ShouldSerializeIIfCallList() => IIfCallList.Count > 0;

        [XmlArray("IIfCallElseExpressionLinkList")]
        [XmlArrayItem("IIfCallElseExpressionLink")]
        public List<IIfCallElseExpressionLink> IIfCallElseExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeIIfCallElseExpressionLinkList() => IIfCallElseExpressionLinkList.Count > 0;

        [XmlArray("IIfCallPredicateLinkList")]
        [XmlArrayItem("IIfCallPredicateLink")]
        public List<IIfCallPredicateLink> IIfCallPredicateLinkList { get; set; } = new();
        public bool ShouldSerializeIIfCallPredicateLinkList() => IIfCallPredicateLinkList.Count > 0;

        [XmlArray("IIfCallThenExpressionLinkList")]
        [XmlArrayItem("IIfCallThenExpressionLink")]
        public List<IIfCallThenExpressionLink> IIfCallThenExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeIIfCallThenExpressionLinkList() => IIfCallThenExpressionLinkList.Count > 0;

        [XmlArray("InlineDerivedTableList")]
        [XmlArrayItem("InlineDerivedTable")]
        public List<InlineDerivedTable> InlineDerivedTableList { get; set; } = new();
        public bool ShouldSerializeInlineDerivedTableList() => InlineDerivedTableList.Count > 0;

        [XmlArray("InlineDerivedTableRowValuesItemList")]
        [XmlArrayItem("InlineDerivedTableRowValuesItem")]
        public List<InlineDerivedTableRowValuesItem> InlineDerivedTableRowValuesItemList { get; set; } = new();
        public bool ShouldSerializeInlineDerivedTableRowValuesItemList() => InlineDerivedTableRowValuesItemList.Count > 0;

        [XmlArray("InPredicateList")]
        [XmlArrayItem("InPredicate")]
        public List<InPredicate> InPredicateList { get; set; } = new();
        public bool ShouldSerializeInPredicateList() => InPredicateList.Count > 0;

        [XmlArray("InPredicateExpressionLinkList")]
        [XmlArrayItem("InPredicateExpressionLink")]
        public List<InPredicateExpressionLink> InPredicateExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeInPredicateExpressionLinkList() => InPredicateExpressionLinkList.Count > 0;

        [XmlArray("InPredicateSubqueryLinkList")]
        [XmlArrayItem("InPredicateSubqueryLink")]
        public List<InPredicateSubqueryLink> InPredicateSubqueryLinkList { get; set; } = new();
        public bool ShouldSerializeInPredicateSubqueryLinkList() => InPredicateSubqueryLinkList.Count > 0;

        [XmlArray("InPredicateValuesItemList")]
        [XmlArrayItem("InPredicateValuesItem")]
        public List<InPredicateValuesItem> InPredicateValuesItemList { get; set; } = new();
        public bool ShouldSerializeInPredicateValuesItemList() => InPredicateValuesItemList.Count > 0;

        [XmlArray("IntegerLiteralList")]
        [XmlArrayItem("IntegerLiteral")]
        public List<IntegerLiteral> IntegerLiteralList { get; set; } = new();
        public bool ShouldSerializeIntegerLiteralList() => IntegerLiteralList.Count > 0;

        [XmlArray("JoinParenthesisTableReferenceList")]
        [XmlArrayItem("JoinParenthesisTableReference")]
        public List<JoinParenthesisTableReference> JoinParenthesisTableReferenceList { get; set; } = new();
        public bool ShouldSerializeJoinParenthesisTableReferenceList() => JoinParenthesisTableReferenceList.Count > 0;

        [XmlArray("JoinParenthesisTableReferenceJoinLinkList")]
        [XmlArrayItem("JoinParenthesisTableReferenceJoinLink")]
        public List<JoinParenthesisTableReferenceJoinLink> JoinParenthesisTableReferenceJoinLinkList { get; set; } = new();
        public bool ShouldSerializeJoinParenthesisTableReferenceJoinLinkList() => JoinParenthesisTableReferenceJoinLinkList.Count > 0;

        [XmlArray("JoinTableReferenceList")]
        [XmlArrayItem("JoinTableReference")]
        public List<JoinTableReference> JoinTableReferenceList { get; set; } = new();
        public bool ShouldSerializeJoinTableReferenceList() => JoinTableReferenceList.Count > 0;

        [XmlArray("JoinTableReferenceFirstTableReferenceLinkList")]
        [XmlArrayItem("JoinTableReferenceFirstTableReferenceLink")]
        public List<JoinTableReferenceFirstTableReferenceLink> JoinTableReferenceFirstTableReferenceLinkList { get; set; } = new();
        public bool ShouldSerializeJoinTableReferenceFirstTableReferenceLinkList() => JoinTableReferenceFirstTableReferenceLinkList.Count > 0;

        [XmlArray("JoinTableReferenceSecondTableReferenceLinkList")]
        [XmlArrayItem("JoinTableReferenceSecondTableReferenceLink")]
        public List<JoinTableReferenceSecondTableReferenceLink> JoinTableReferenceSecondTableReferenceLinkList { get; set; } = new();
        public bool ShouldSerializeJoinTableReferenceSecondTableReferenceLinkList() => JoinTableReferenceSecondTableReferenceLinkList.Count > 0;

        [XmlArray("LeftFunctionCallList")]
        [XmlArrayItem("LeftFunctionCall")]
        public List<LeftFunctionCall> LeftFunctionCallList { get; set; } = new();
        public bool ShouldSerializeLeftFunctionCallList() => LeftFunctionCallList.Count > 0;

        [XmlArray("LeftFunctionCallParametersItemList")]
        [XmlArrayItem("LeftFunctionCallParametersItem")]
        public List<LeftFunctionCallParametersItem> LeftFunctionCallParametersItemList { get; set; } = new();
        public bool ShouldSerializeLeftFunctionCallParametersItemList() => LeftFunctionCallParametersItemList.Count > 0;

        [XmlArray("LikePredicateList")]
        [XmlArrayItem("LikePredicate")]
        public List<LikePredicate> LikePredicateList { get; set; } = new();
        public bool ShouldSerializeLikePredicateList() => LikePredicateList.Count > 0;

        [XmlArray("LikePredicateFirstExpressionLinkList")]
        [XmlArrayItem("LikePredicateFirstExpressionLink")]
        public List<LikePredicateFirstExpressionLink> LikePredicateFirstExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeLikePredicateFirstExpressionLinkList() => LikePredicateFirstExpressionLinkList.Count > 0;

        [XmlArray("LikePredicateSecondExpressionLinkList")]
        [XmlArrayItem("LikePredicateSecondExpressionLink")]
        public List<LikePredicateSecondExpressionLink> LikePredicateSecondExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeLikePredicateSecondExpressionLinkList() => LikePredicateSecondExpressionLinkList.Count > 0;

        [XmlArray("LiteralList")]
        [XmlArrayItem("Literal")]
        public List<Literal> LiteralList { get; set; } = new();
        public bool ShouldSerializeLiteralList() => LiteralList.Count > 0;

        [XmlArray("MaxLiteralList")]
        [XmlArrayItem("MaxLiteral")]
        public List<MaxLiteral> MaxLiteralList { get; set; } = new();
        public bool ShouldSerializeMaxLiteralList() => MaxLiteralList.Count > 0;

        [XmlArray("MultiPartIdentifierList")]
        [XmlArrayItem("MultiPartIdentifier")]
        public List<MultiPartIdentifier> MultiPartIdentifierList { get; set; } = new();
        public bool ShouldSerializeMultiPartIdentifierList() => MultiPartIdentifierList.Count > 0;

        [XmlArray("MultiPartIdentifierCallTargetList")]
        [XmlArrayItem("MultiPartIdentifierCallTarget")]
        public List<MultiPartIdentifierCallTarget> MultiPartIdentifierCallTargetList { get; set; } = new();
        public bool ShouldSerializeMultiPartIdentifierCallTargetList() => MultiPartIdentifierCallTargetList.Count > 0;

        [XmlArray("MultiPartIdentifierCallTargetMultiPartIdentifierLinkList")]
        [XmlArrayItem("MultiPartIdentifierCallTargetMultiPartIdentifierLink")]
        public List<MultiPartIdentifierCallTargetMultiPartIdentifierLink> MultiPartIdentifierCallTargetMultiPartIdentifierLinkList { get; set; } = new();
        public bool ShouldSerializeMultiPartIdentifierCallTargetMultiPartIdentifierLinkList() => MultiPartIdentifierCallTargetMultiPartIdentifierLinkList.Count > 0;

        [XmlArray("MultiPartIdentifierIdentifiersItemList")]
        [XmlArrayItem("MultiPartIdentifierIdentifiersItem")]
        public List<MultiPartIdentifierIdentifiersItem> MultiPartIdentifierIdentifiersItemList { get; set; } = new();
        public bool ShouldSerializeMultiPartIdentifierIdentifiersItemList() => MultiPartIdentifierIdentifiersItemList.Count > 0;

        [XmlArray("NamedTableReferenceList")]
        [XmlArrayItem("NamedTableReference")]
        public List<NamedTableReference> NamedTableReferenceList { get; set; } = new();
        public bool ShouldSerializeNamedTableReferenceList() => NamedTableReferenceList.Count > 0;

        [XmlArray("NamedTableReferenceSchemaObjectLinkList")]
        [XmlArrayItem("NamedTableReferenceSchemaObjectLink")]
        public List<NamedTableReferenceSchemaObjectLink> NamedTableReferenceSchemaObjectLinkList { get; set; } = new();
        public bool ShouldSerializeNamedTableReferenceSchemaObjectLinkList() => NamedTableReferenceSchemaObjectLinkList.Count > 0;

        [XmlArray("NamedTableReferenceTableSampleClauseLinkList")]
        [XmlArrayItem("NamedTableReferenceTableSampleClauseLink")]
        public List<NamedTableReferenceTableSampleClauseLink> NamedTableReferenceTableSampleClauseLinkList { get; set; } = new();
        public bool ShouldSerializeNamedTableReferenceTableSampleClauseLinkList() => NamedTableReferenceTableSampleClauseLinkList.Count > 0;

        [XmlArray("NextValueForExpressionList")]
        [XmlArrayItem("NextValueForExpression")]
        public List<NextValueForExpression> NextValueForExpressionList { get; set; } = new();
        public bool ShouldSerializeNextValueForExpressionList() => NextValueForExpressionList.Count > 0;

        [XmlArray("NextValueForExpressionSequenceNameLinkList")]
        [XmlArrayItem("NextValueForExpressionSequenceNameLink")]
        public List<NextValueForExpressionSequenceNameLink> NextValueForExpressionSequenceNameLinkList { get; set; } = new();
        public bool ShouldSerializeNextValueForExpressionSequenceNameLinkList() => NextValueForExpressionSequenceNameLinkList.Count > 0;

        [XmlArray("NullIfExpressionList")]
        [XmlArrayItem("NullIfExpression")]
        public List<NullIfExpression> NullIfExpressionList { get; set; } = new();
        public bool ShouldSerializeNullIfExpressionList() => NullIfExpressionList.Count > 0;

        [XmlArray("NullIfExpressionFirstExpressionLinkList")]
        [XmlArrayItem("NullIfExpressionFirstExpressionLink")]
        public List<NullIfExpressionFirstExpressionLink> NullIfExpressionFirstExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeNullIfExpressionFirstExpressionLinkList() => NullIfExpressionFirstExpressionLinkList.Count > 0;

        [XmlArray("NullIfExpressionSecondExpressionLinkList")]
        [XmlArrayItem("NullIfExpressionSecondExpressionLink")]
        public List<NullIfExpressionSecondExpressionLink> NullIfExpressionSecondExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeNullIfExpressionSecondExpressionLinkList() => NullIfExpressionSecondExpressionLinkList.Count > 0;

        [XmlArray("NullLiteralList")]
        [XmlArrayItem("NullLiteral")]
        public List<NullLiteral> NullLiteralList { get; set; } = new();
        public bool ShouldSerializeNullLiteralList() => NullLiteralList.Count > 0;

        [XmlArray("NumericLiteralList")]
        [XmlArrayItem("NumericLiteral")]
        public List<NumericLiteral> NumericLiteralList { get; set; } = new();
        public bool ShouldSerializeNumericLiteralList() => NumericLiteralList.Count > 0;

        [XmlArray("OffsetClauseList")]
        [XmlArrayItem("OffsetClause")]
        public List<OffsetClause> OffsetClauseList { get; set; } = new();
        public bool ShouldSerializeOffsetClauseList() => OffsetClauseList.Count > 0;

        [XmlArray("OffsetClauseFetchExpressionLinkList")]
        [XmlArrayItem("OffsetClauseFetchExpressionLink")]
        public List<OffsetClauseFetchExpressionLink> OffsetClauseFetchExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeOffsetClauseFetchExpressionLinkList() => OffsetClauseFetchExpressionLinkList.Count > 0;

        [XmlArray("OffsetClauseOffsetExpressionLinkList")]
        [XmlArrayItem("OffsetClauseOffsetExpressionLink")]
        public List<OffsetClauseOffsetExpressionLink> OffsetClauseOffsetExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeOffsetClauseOffsetExpressionLinkList() => OffsetClauseOffsetExpressionLinkList.Count > 0;

        [XmlArray("OrderByClauseList")]
        [XmlArrayItem("OrderByClause")]
        public List<OrderByClause> OrderByClauseList { get; set; } = new();
        public bool ShouldSerializeOrderByClauseList() => OrderByClauseList.Count > 0;

        [XmlArray("OrderByClauseOrderByElementsItemList")]
        [XmlArrayItem("OrderByClauseOrderByElementsItem")]
        public List<OrderByClauseOrderByElementsItem> OrderByClauseOrderByElementsItemList { get; set; } = new();
        public bool ShouldSerializeOrderByClauseOrderByElementsItemList() => OrderByClauseOrderByElementsItemList.Count > 0;

        [XmlArray("OverClauseList")]
        [XmlArrayItem("OverClause")]
        public List<OverClause> OverClauseList { get; set; } = new();
        public bool ShouldSerializeOverClauseList() => OverClauseList.Count > 0;

        [XmlArray("OverClauseOrderByClauseLinkList")]
        [XmlArrayItem("OverClauseOrderByClauseLink")]
        public List<OverClauseOrderByClauseLink> OverClauseOrderByClauseLinkList { get; set; } = new();
        public bool ShouldSerializeOverClauseOrderByClauseLinkList() => OverClauseOrderByClauseLinkList.Count > 0;

        [XmlArray("OverClausePartitionsItemList")]
        [XmlArrayItem("OverClausePartitionsItem")]
        public List<OverClausePartitionsItem> OverClausePartitionsItemList { get; set; } = new();
        public bool ShouldSerializeOverClausePartitionsItemList() => OverClausePartitionsItemList.Count > 0;

        [XmlArray("OverClauseWindowFrameClauseLinkList")]
        [XmlArrayItem("OverClauseWindowFrameClauseLink")]
        public List<OverClauseWindowFrameClauseLink> OverClauseWindowFrameClauseLinkList { get; set; } = new();
        public bool ShouldSerializeOverClauseWindowFrameClauseLinkList() => OverClauseWindowFrameClauseLinkList.Count > 0;

        [XmlArray("OverClauseWindowNameLinkList")]
        [XmlArrayItem("OverClauseWindowNameLink")]
        public List<OverClauseWindowNameLink> OverClauseWindowNameLinkList { get; set; } = new();
        public bool ShouldSerializeOverClauseWindowNameLinkList() => OverClauseWindowNameLinkList.Count > 0;

        [XmlArray("ParameterizedDataTypeReferenceList")]
        [XmlArrayItem("ParameterizedDataTypeReference")]
        public List<ParameterizedDataTypeReference> ParameterizedDataTypeReferenceList { get; set; } = new();
        public bool ShouldSerializeParameterizedDataTypeReferenceList() => ParameterizedDataTypeReferenceList.Count > 0;

        [XmlArray("ParameterizedDataTypeReferenceParametersItemList")]
        [XmlArrayItem("ParameterizedDataTypeReferenceParametersItem")]
        public List<ParameterizedDataTypeReferenceParametersItem> ParameterizedDataTypeReferenceParametersItemList { get; set; } = new();
        public bool ShouldSerializeParameterizedDataTypeReferenceParametersItemList() => ParameterizedDataTypeReferenceParametersItemList.Count > 0;

        [XmlArray("ParameterlessCallList")]
        [XmlArrayItem("ParameterlessCall")]
        public List<ParameterlessCall> ParameterlessCallList { get; set; } = new();
        public bool ShouldSerializeParameterlessCallList() => ParameterlessCallList.Count > 0;

        [XmlArray("ParenthesisExpressionList")]
        [XmlArrayItem("ParenthesisExpression")]
        public List<ParenthesisExpression> ParenthesisExpressionList { get; set; } = new();
        public bool ShouldSerializeParenthesisExpressionList() => ParenthesisExpressionList.Count > 0;

        [XmlArray("ParenthesisExpressionExpressionLinkList")]
        [XmlArrayItem("ParenthesisExpressionExpressionLink")]
        public List<ParenthesisExpressionExpressionLink> ParenthesisExpressionExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeParenthesisExpressionExpressionLinkList() => ParenthesisExpressionExpressionLinkList.Count > 0;

        [XmlArray("ParseCallList")]
        [XmlArrayItem("ParseCall")]
        public List<ParseCall> ParseCallList { get; set; } = new();
        public bool ShouldSerializeParseCallList() => ParseCallList.Count > 0;

        [XmlArray("ParseCallCultureLinkList")]
        [XmlArrayItem("ParseCallCultureLink")]
        public List<ParseCallCultureLink> ParseCallCultureLinkList { get; set; } = new();
        public bool ShouldSerializeParseCallCultureLinkList() => ParseCallCultureLinkList.Count > 0;

        [XmlArray("ParseCallDataTypeLinkList")]
        [XmlArrayItem("ParseCallDataTypeLink")]
        public List<ParseCallDataTypeLink> ParseCallDataTypeLinkList { get; set; } = new();
        public bool ShouldSerializeParseCallDataTypeLinkList() => ParseCallDataTypeLinkList.Count > 0;

        [XmlArray("ParseCallStringValueLinkList")]
        [XmlArrayItem("ParseCallStringValueLink")]
        public List<ParseCallStringValueLink> ParseCallStringValueLinkList { get; set; } = new();
        public bool ShouldSerializeParseCallStringValueLinkList() => ParseCallStringValueLinkList.Count > 0;

        [XmlArray("PivotedTableReferenceList")]
        [XmlArrayItem("PivotedTableReference")]
        public List<PivotedTableReference> PivotedTableReferenceList { get; set; } = new();
        public bool ShouldSerializePivotedTableReferenceList() => PivotedTableReferenceList.Count > 0;

        [XmlArray("PivotedTableReferenceAggregateFunctionIdentifierLinkList")]
        [XmlArrayItem("PivotedTableReferenceAggregateFunctionIdentifierLink")]
        public List<PivotedTableReferenceAggregateFunctionIdentifierLink> PivotedTableReferenceAggregateFunctionIdentifierLinkList { get; set; } = new();
        public bool ShouldSerializePivotedTableReferenceAggregateFunctionIdentifierLinkList() => PivotedTableReferenceAggregateFunctionIdentifierLinkList.Count > 0;

        [XmlArray("PivotedTableReferenceInColumnsItemList")]
        [XmlArrayItem("PivotedTableReferenceInColumnsItem")]
        public List<PivotedTableReferenceInColumnsItem> PivotedTableReferenceInColumnsItemList { get; set; } = new();
        public bool ShouldSerializePivotedTableReferenceInColumnsItemList() => PivotedTableReferenceInColumnsItemList.Count > 0;

        [XmlArray("PivotedTableReferencePivotColumnLinkList")]
        [XmlArrayItem("PivotedTableReferencePivotColumnLink")]
        public List<PivotedTableReferencePivotColumnLink> PivotedTableReferencePivotColumnLinkList { get; set; } = new();
        public bool ShouldSerializePivotedTableReferencePivotColumnLinkList() => PivotedTableReferencePivotColumnLinkList.Count > 0;

        [XmlArray("PivotedTableReferenceTableReferenceLinkList")]
        [XmlArrayItem("PivotedTableReferenceTableReferenceLink")]
        public List<PivotedTableReferenceTableReferenceLink> PivotedTableReferenceTableReferenceLinkList { get; set; } = new();
        public bool ShouldSerializePivotedTableReferenceTableReferenceLinkList() => PivotedTableReferenceTableReferenceLinkList.Count > 0;

        [XmlArray("PivotedTableReferenceValueColumnsItemList")]
        [XmlArrayItem("PivotedTableReferenceValueColumnsItem")]
        public List<PivotedTableReferenceValueColumnsItem> PivotedTableReferenceValueColumnsItemList { get; set; } = new();
        public bool ShouldSerializePivotedTableReferenceValueColumnsItemList() => PivotedTableReferenceValueColumnsItemList.Count > 0;

        [XmlArray("PrimaryExpressionList")]
        [XmlArrayItem("PrimaryExpression")]
        public List<PrimaryExpression> PrimaryExpressionList { get; set; } = new();
        public bool ShouldSerializePrimaryExpressionList() => PrimaryExpressionList.Count > 0;

        [XmlArray("PrimaryExpressionCollationLinkList")]
        [XmlArrayItem("PrimaryExpressionCollationLink")]
        public List<PrimaryExpressionCollationLink> PrimaryExpressionCollationLinkList { get; set; } = new();
        public bool ShouldSerializePrimaryExpressionCollationLinkList() => PrimaryExpressionCollationLinkList.Count > 0;

        [XmlArray("QualifiedJoinList")]
        [XmlArrayItem("QualifiedJoin")]
        public List<QualifiedJoin> QualifiedJoinList { get; set; } = new();
        public bool ShouldSerializeQualifiedJoinList() => QualifiedJoinList.Count > 0;

        [XmlArray("QualifiedJoinSearchConditionLinkList")]
        [XmlArrayItem("QualifiedJoinSearchConditionLink")]
        public List<QualifiedJoinSearchConditionLink> QualifiedJoinSearchConditionLinkList { get; set; } = new();
        public bool ShouldSerializeQualifiedJoinSearchConditionLinkList() => QualifiedJoinSearchConditionLinkList.Count > 0;

        [XmlArray("QueryDerivedTableList")]
        [XmlArrayItem("QueryDerivedTable")]
        public List<QueryDerivedTable> QueryDerivedTableList { get; set; } = new();
        public bool ShouldSerializeQueryDerivedTableList() => QueryDerivedTableList.Count > 0;

        [XmlArray("QueryDerivedTableQueryExpressionLinkList")]
        [XmlArrayItem("QueryDerivedTableQueryExpressionLink")]
        public List<QueryDerivedTableQueryExpressionLink> QueryDerivedTableQueryExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeQueryDerivedTableQueryExpressionLinkList() => QueryDerivedTableQueryExpressionLinkList.Count > 0;

        [XmlArray("QueryExpressionList")]
        [XmlArrayItem("QueryExpression")]
        public List<QueryExpression> QueryExpressionList { get; set; } = new();
        public bool ShouldSerializeQueryExpressionList() => QueryExpressionList.Count > 0;

        [XmlArray("QueryExpressionOffsetClauseLinkList")]
        [XmlArrayItem("QueryExpressionOffsetClauseLink")]
        public List<QueryExpressionOffsetClauseLink> QueryExpressionOffsetClauseLinkList { get; set; } = new();
        public bool ShouldSerializeQueryExpressionOffsetClauseLinkList() => QueryExpressionOffsetClauseLinkList.Count > 0;

        [XmlArray("QueryExpressionOrderByClauseLinkList")]
        [XmlArrayItem("QueryExpressionOrderByClauseLink")]
        public List<QueryExpressionOrderByClauseLink> QueryExpressionOrderByClauseLinkList { get; set; } = new();
        public bool ShouldSerializeQueryExpressionOrderByClauseLinkList() => QueryExpressionOrderByClauseLinkList.Count > 0;

        [XmlArray("QueryParenthesisExpressionList")]
        [XmlArrayItem("QueryParenthesisExpression")]
        public List<QueryParenthesisExpression> QueryParenthesisExpressionList { get; set; } = new();
        public bool ShouldSerializeQueryParenthesisExpressionList() => QueryParenthesisExpressionList.Count > 0;

        [XmlArray("QueryParenthesisExpressionQueryExpressionLinkList")]
        [XmlArrayItem("QueryParenthesisExpressionQueryExpressionLink")]
        public List<QueryParenthesisExpressionQueryExpressionLink> QueryParenthesisExpressionQueryExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeQueryParenthesisExpressionQueryExpressionLinkList() => QueryParenthesisExpressionQueryExpressionLinkList.Count > 0;

        [XmlArray("QuerySpecificationList")]
        [XmlArrayItem("QuerySpecification")]
        public List<QuerySpecification> QuerySpecificationList { get; set; } = new();
        public bool ShouldSerializeQuerySpecificationList() => QuerySpecificationList.Count > 0;

        [XmlArray("QuerySpecificationFromClauseLinkList")]
        [XmlArrayItem("QuerySpecificationFromClauseLink")]
        public List<QuerySpecificationFromClauseLink> QuerySpecificationFromClauseLinkList { get; set; } = new();
        public bool ShouldSerializeQuerySpecificationFromClauseLinkList() => QuerySpecificationFromClauseLinkList.Count > 0;

        [XmlArray("QuerySpecificationGroupByClauseLinkList")]
        [XmlArrayItem("QuerySpecificationGroupByClauseLink")]
        public List<QuerySpecificationGroupByClauseLink> QuerySpecificationGroupByClauseLinkList { get; set; } = new();
        public bool ShouldSerializeQuerySpecificationGroupByClauseLinkList() => QuerySpecificationGroupByClauseLinkList.Count > 0;

        [XmlArray("QuerySpecificationHavingClauseLinkList")]
        [XmlArrayItem("QuerySpecificationHavingClauseLink")]
        public List<QuerySpecificationHavingClauseLink> QuerySpecificationHavingClauseLinkList { get; set; } = new();
        public bool ShouldSerializeQuerySpecificationHavingClauseLinkList() => QuerySpecificationHavingClauseLinkList.Count > 0;

        [XmlArray("QuerySpecificationSelectElementsItemList")]
        [XmlArrayItem("QuerySpecificationSelectElementsItem")]
        public List<QuerySpecificationSelectElementsItem> QuerySpecificationSelectElementsItemList { get; set; } = new();
        public bool ShouldSerializeQuerySpecificationSelectElementsItemList() => QuerySpecificationSelectElementsItemList.Count > 0;

        [XmlArray("QuerySpecificationTopRowFilterLinkList")]
        [XmlArrayItem("QuerySpecificationTopRowFilterLink")]
        public List<QuerySpecificationTopRowFilterLink> QuerySpecificationTopRowFilterLinkList { get; set; } = new();
        public bool ShouldSerializeQuerySpecificationTopRowFilterLinkList() => QuerySpecificationTopRowFilterLinkList.Count > 0;

        [XmlArray("QuerySpecificationWhereClauseLinkList")]
        [XmlArrayItem("QuerySpecificationWhereClauseLink")]
        public List<QuerySpecificationWhereClauseLink> QuerySpecificationWhereClauseLinkList { get; set; } = new();
        public bool ShouldSerializeQuerySpecificationWhereClauseLinkList() => QuerySpecificationWhereClauseLinkList.Count > 0;

        [XmlArray("QuerySpecificationWindowClauseLinkList")]
        [XmlArrayItem("QuerySpecificationWindowClauseLink")]
        public List<QuerySpecificationWindowClauseLink> QuerySpecificationWindowClauseLinkList { get; set; } = new();
        public bool ShouldSerializeQuerySpecificationWindowClauseLinkList() => QuerySpecificationWindowClauseLinkList.Count > 0;

        [XmlArray("RealLiteralList")]
        [XmlArrayItem("RealLiteral")]
        public List<RealLiteral> RealLiteralList { get; set; } = new();
        public bool ShouldSerializeRealLiteralList() => RealLiteralList.Count > 0;

        [XmlArray("RightFunctionCallList")]
        [XmlArrayItem("RightFunctionCall")]
        public List<RightFunctionCall> RightFunctionCallList { get; set; } = new();
        public bool ShouldSerializeRightFunctionCallList() => RightFunctionCallList.Count > 0;

        [XmlArray("RightFunctionCallParametersItemList")]
        [XmlArrayItem("RightFunctionCallParametersItem")]
        public List<RightFunctionCallParametersItem> RightFunctionCallParametersItemList { get; set; } = new();
        public bool ShouldSerializeRightFunctionCallParametersItemList() => RightFunctionCallParametersItemList.Count > 0;

        [XmlArray("RollupGroupingSpecificationList")]
        [XmlArrayItem("RollupGroupingSpecification")]
        public List<RollupGroupingSpecification> RollupGroupingSpecificationList { get; set; } = new();
        public bool ShouldSerializeRollupGroupingSpecificationList() => RollupGroupingSpecificationList.Count > 0;

        [XmlArray("RollupGroupingSpecificationArgumentsItemList")]
        [XmlArrayItem("RollupGroupingSpecificationArgumentsItem")]
        public List<RollupGroupingSpecificationArgumentsItem> RollupGroupingSpecificationArgumentsItemList { get; set; } = new();
        public bool ShouldSerializeRollupGroupingSpecificationArgumentsItemList() => RollupGroupingSpecificationArgumentsItemList.Count > 0;

        [XmlArray("RowValueList")]
        [XmlArrayItem("RowValue")]
        public List<RowValue> RowValueList { get; set; } = new();
        public bool ShouldSerializeRowValueList() => RowValueList.Count > 0;

        [XmlArray("RowValueColumnValuesItemList")]
        [XmlArrayItem("RowValueColumnValuesItem")]
        public List<RowValueColumnValuesItem> RowValueColumnValuesItemList { get; set; } = new();
        public bool ShouldSerializeRowValueColumnValuesItemList() => RowValueColumnValuesItemList.Count > 0;

        [XmlArray("ScalarExpressionList")]
        [XmlArrayItem("ScalarExpression")]
        public List<ScalarExpression> ScalarExpressionList { get; set; } = new();
        public bool ShouldSerializeScalarExpressionList() => ScalarExpressionList.Count > 0;

        [XmlArray("ScalarSubqueryList")]
        [XmlArrayItem("ScalarSubquery")]
        public List<ScalarSubquery> ScalarSubqueryList { get; set; } = new();
        public bool ShouldSerializeScalarSubqueryList() => ScalarSubqueryList.Count > 0;

        [XmlArray("ScalarSubqueryQueryExpressionLinkList")]
        [XmlArrayItem("ScalarSubqueryQueryExpressionLink")]
        public List<ScalarSubqueryQueryExpressionLink> ScalarSubqueryQueryExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeScalarSubqueryQueryExpressionLinkList() => ScalarSubqueryQueryExpressionLinkList.Count > 0;

        [XmlArray("SchemaObjectFunctionTableReferenceList")]
        [XmlArrayItem("SchemaObjectFunctionTableReference")]
        public List<SchemaObjectFunctionTableReference> SchemaObjectFunctionTableReferenceList { get; set; } = new();
        public bool ShouldSerializeSchemaObjectFunctionTableReferenceList() => SchemaObjectFunctionTableReferenceList.Count > 0;

        [XmlArray("SchemaObjectFunctionTableReferenceParametersItemList")]
        [XmlArrayItem("SchemaObjectFunctionTableReferenceParametersItem")]
        public List<SchemaObjectFunctionTableReferenceParametersItem> SchemaObjectFunctionTableReferenceParametersItemList { get; set; } = new();
        public bool ShouldSerializeSchemaObjectFunctionTableReferenceParametersItemList() => SchemaObjectFunctionTableReferenceParametersItemList.Count > 0;

        [XmlArray("SchemaObjectFunctionTableReferenceSchemaObjectLinkList")]
        [XmlArrayItem("SchemaObjectFunctionTableReferenceSchemaObjectLink")]
        public List<SchemaObjectFunctionTableReferenceSchemaObjectLink> SchemaObjectFunctionTableReferenceSchemaObjectLinkList { get; set; } = new();
        public bool ShouldSerializeSchemaObjectFunctionTableReferenceSchemaObjectLinkList() => SchemaObjectFunctionTableReferenceSchemaObjectLinkList.Count > 0;

        [XmlArray("SchemaObjectNameList")]
        [XmlArrayItem("SchemaObjectName")]
        public List<SchemaObjectName> SchemaObjectNameList { get; set; } = new();
        public bool ShouldSerializeSchemaObjectNameList() => SchemaObjectNameList.Count > 0;

        [XmlArray("SchemaObjectNameBaseIdentifierLinkList")]
        [XmlArrayItem("SchemaObjectNameBaseIdentifierLink")]
        public List<SchemaObjectNameBaseIdentifierLink> SchemaObjectNameBaseIdentifierLinkList { get; set; } = new();
        public bool ShouldSerializeSchemaObjectNameBaseIdentifierLinkList() => SchemaObjectNameBaseIdentifierLinkList.Count > 0;

        [XmlArray("SchemaObjectNameSchemaIdentifierLinkList")]
        [XmlArrayItem("SchemaObjectNameSchemaIdentifierLink")]
        public List<SchemaObjectNameSchemaIdentifierLink> SchemaObjectNameSchemaIdentifierLinkList { get; set; } = new();
        public bool ShouldSerializeSchemaObjectNameSchemaIdentifierLinkList() => SchemaObjectNameSchemaIdentifierLinkList.Count > 0;

        [XmlArray("SearchedCaseExpressionList")]
        [XmlArrayItem("SearchedCaseExpression")]
        public List<SearchedCaseExpression> SearchedCaseExpressionList { get; set; } = new();
        public bool ShouldSerializeSearchedCaseExpressionList() => SearchedCaseExpressionList.Count > 0;

        [XmlArray("SearchedCaseExpressionWhenClausesItemList")]
        [XmlArrayItem("SearchedCaseExpressionWhenClausesItem")]
        public List<SearchedCaseExpressionWhenClausesItem> SearchedCaseExpressionWhenClausesItemList { get; set; } = new();
        public bool ShouldSerializeSearchedCaseExpressionWhenClausesItemList() => SearchedCaseExpressionWhenClausesItemList.Count > 0;

        [XmlArray("SearchedWhenClauseList")]
        [XmlArrayItem("SearchedWhenClause")]
        public List<SearchedWhenClause> SearchedWhenClauseList { get; set; } = new();
        public bool ShouldSerializeSearchedWhenClauseList() => SearchedWhenClauseList.Count > 0;

        [XmlArray("SearchedWhenClauseWhenExpressionLinkList")]
        [XmlArrayItem("SearchedWhenClauseWhenExpressionLink")]
        public List<SearchedWhenClauseWhenExpressionLink> SearchedWhenClauseWhenExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeSearchedWhenClauseWhenExpressionLinkList() => SearchedWhenClauseWhenExpressionLinkList.Count > 0;

        [XmlArray("SelectElementList")]
        [XmlArrayItem("SelectElement")]
        public List<SelectElement> SelectElementList { get; set; } = new();
        public bool ShouldSerializeSelectElementList() => SelectElementList.Count > 0;

        [XmlArray("SelectScalarExpressionList")]
        [XmlArrayItem("SelectScalarExpression")]
        public List<SelectScalarExpression> SelectScalarExpressionList { get; set; } = new();
        public bool ShouldSerializeSelectScalarExpressionList() => SelectScalarExpressionList.Count > 0;

        [XmlArray("SelectScalarExpressionColumnNameLinkList")]
        [XmlArrayItem("SelectScalarExpressionColumnNameLink")]
        public List<SelectScalarExpressionColumnNameLink> SelectScalarExpressionColumnNameLinkList { get; set; } = new();
        public bool ShouldSerializeSelectScalarExpressionColumnNameLinkList() => SelectScalarExpressionColumnNameLinkList.Count > 0;

        [XmlArray("SelectScalarExpressionExpressionLinkList")]
        [XmlArrayItem("SelectScalarExpressionExpressionLink")]
        public List<SelectScalarExpressionExpressionLink> SelectScalarExpressionExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeSelectScalarExpressionExpressionLinkList() => SelectScalarExpressionExpressionLinkList.Count > 0;

        [XmlArray("SelectStarExpressionList")]
        [XmlArrayItem("SelectStarExpression")]
        public List<SelectStarExpression> SelectStarExpressionList { get; set; } = new();
        public bool ShouldSerializeSelectStarExpressionList() => SelectStarExpressionList.Count > 0;

        [XmlArray("SelectStarExpressionQualifierLinkList")]
        [XmlArrayItem("SelectStarExpressionQualifierLink")]
        public List<SelectStarExpressionQualifierLink> SelectStarExpressionQualifierLinkList { get; set; } = new();
        public bool ShouldSerializeSelectStarExpressionQualifierLinkList() => SelectStarExpressionQualifierLinkList.Count > 0;

        [XmlArray("SelectStatementList")]
        [XmlArrayItem("SelectStatement")]
        public List<SelectStatement> SelectStatementList { get; set; } = new();
        public bool ShouldSerializeSelectStatementList() => SelectStatementList.Count > 0;

        [XmlArray("SelectStatementQueryExpressionLinkList")]
        [XmlArrayItem("SelectStatementQueryExpressionLink")]
        public List<SelectStatementQueryExpressionLink> SelectStatementQueryExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeSelectStatementQueryExpressionLinkList() => SelectStatementQueryExpressionLinkList.Count > 0;

        [XmlArray("SimpleCaseExpressionList")]
        [XmlArrayItem("SimpleCaseExpression")]
        public List<SimpleCaseExpression> SimpleCaseExpressionList { get; set; } = new();
        public bool ShouldSerializeSimpleCaseExpressionList() => SimpleCaseExpressionList.Count > 0;

        [XmlArray("SimpleCaseExpressionInputExpressionLinkList")]
        [XmlArrayItem("SimpleCaseExpressionInputExpressionLink")]
        public List<SimpleCaseExpressionInputExpressionLink> SimpleCaseExpressionInputExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeSimpleCaseExpressionInputExpressionLinkList() => SimpleCaseExpressionInputExpressionLinkList.Count > 0;

        [XmlArray("SimpleCaseExpressionWhenClausesItemList")]
        [XmlArrayItem("SimpleCaseExpressionWhenClausesItem")]
        public List<SimpleCaseExpressionWhenClausesItem> SimpleCaseExpressionWhenClausesItemList { get; set; } = new();
        public bool ShouldSerializeSimpleCaseExpressionWhenClausesItemList() => SimpleCaseExpressionWhenClausesItemList.Count > 0;

        [XmlArray("SimpleWhenClauseList")]
        [XmlArrayItem("SimpleWhenClause")]
        public List<SimpleWhenClause> SimpleWhenClauseList { get; set; } = new();
        public bool ShouldSerializeSimpleWhenClauseList() => SimpleWhenClauseList.Count > 0;

        [XmlArray("SimpleWhenClauseWhenExpressionLinkList")]
        [XmlArrayItem("SimpleWhenClauseWhenExpressionLink")]
        public List<SimpleWhenClauseWhenExpressionLink> SimpleWhenClauseWhenExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeSimpleWhenClauseWhenExpressionLinkList() => SimpleWhenClauseWhenExpressionLinkList.Count > 0;

        [XmlArray("SqlDataTypeReferenceList")]
        [XmlArrayItem("SqlDataTypeReference")]
        public List<SqlDataTypeReference> SqlDataTypeReferenceList { get; set; } = new();
        public bool ShouldSerializeSqlDataTypeReferenceList() => SqlDataTypeReferenceList.Count > 0;

        [XmlArray("StatementWithCtesAndXmlNamespacesList")]
        [XmlArrayItem("StatementWithCtesAndXmlNamespaces")]
        public List<StatementWithCtesAndXmlNamespaces> StatementWithCtesAndXmlNamespacesList { get; set; } = new();
        public bool ShouldSerializeStatementWithCtesAndXmlNamespacesList() => StatementWithCtesAndXmlNamespacesList.Count > 0;

        [XmlArray("StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkList")]
        [XmlArrayItem("StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLink")]
        public List<StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLink> StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkList { get; set; } = new();
        public bool ShouldSerializeStatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkList() => StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkList.Count > 0;

        [XmlArray("StringLiteralList")]
        [XmlArrayItem("StringLiteral")]
        public List<StringLiteral> StringLiteralList { get; set; } = new();
        public bool ShouldSerializeStringLiteralList() => StringLiteralList.Count > 0;

        [XmlArray("SubqueryComparisonPredicateList")]
        [XmlArrayItem("SubqueryComparisonPredicate")]
        public List<SubqueryComparisonPredicate> SubqueryComparisonPredicateList { get; set; } = new();
        public bool ShouldSerializeSubqueryComparisonPredicateList() => SubqueryComparisonPredicateList.Count > 0;

        [XmlArray("SubqueryComparisonPredicateExpressionLinkList")]
        [XmlArrayItem("SubqueryComparisonPredicateExpressionLink")]
        public List<SubqueryComparisonPredicateExpressionLink> SubqueryComparisonPredicateExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeSubqueryComparisonPredicateExpressionLinkList() => SubqueryComparisonPredicateExpressionLinkList.Count > 0;

        [XmlArray("SubqueryComparisonPredicateSubqueryLinkList")]
        [XmlArrayItem("SubqueryComparisonPredicateSubqueryLink")]
        public List<SubqueryComparisonPredicateSubqueryLink> SubqueryComparisonPredicateSubqueryLinkList { get; set; } = new();
        public bool ShouldSerializeSubqueryComparisonPredicateSubqueryLinkList() => SubqueryComparisonPredicateSubqueryLinkList.Count > 0;

        [XmlArray("TableReferenceList")]
        [XmlArrayItem("TableReference")]
        public List<TableReference> TableReferenceList { get; set; } = new();
        public bool ShouldSerializeTableReferenceList() => TableReferenceList.Count > 0;

        [XmlArray("TableReferenceWithAliasList")]
        [XmlArrayItem("TableReferenceWithAlias")]
        public List<TableReferenceWithAlias> TableReferenceWithAliasList { get; set; } = new();
        public bool ShouldSerializeTableReferenceWithAliasList() => TableReferenceWithAliasList.Count > 0;

        [XmlArray("TableReferenceWithAliasAliasLinkList")]
        [XmlArrayItem("TableReferenceWithAliasAliasLink")]
        public List<TableReferenceWithAliasAliasLink> TableReferenceWithAliasAliasLinkList { get; set; } = new();
        public bool ShouldSerializeTableReferenceWithAliasAliasLinkList() => TableReferenceWithAliasAliasLinkList.Count > 0;

        [XmlArray("TableReferenceWithAliasAndColumnsList")]
        [XmlArrayItem("TableReferenceWithAliasAndColumns")]
        public List<TableReferenceWithAliasAndColumns> TableReferenceWithAliasAndColumnsList { get; set; } = new();
        public bool ShouldSerializeTableReferenceWithAliasAndColumnsList() => TableReferenceWithAliasAndColumnsList.Count > 0;

        [XmlArray("TableReferenceWithAliasAndColumnsColumnsItemList")]
        [XmlArrayItem("TableReferenceWithAliasAndColumnsColumnsItem")]
        public List<TableReferenceWithAliasAndColumnsColumnsItem> TableReferenceWithAliasAndColumnsColumnsItemList { get; set; } = new();
        public bool ShouldSerializeTableReferenceWithAliasAndColumnsColumnsItemList() => TableReferenceWithAliasAndColumnsColumnsItemList.Count > 0;

        [XmlArray("TableSampleClauseList")]
        [XmlArrayItem("TableSampleClause")]
        public List<TableSampleClause> TableSampleClauseList { get; set; } = new();
        public bool ShouldSerializeTableSampleClauseList() => TableSampleClauseList.Count > 0;

        [XmlArray("TableSampleClauseRepeatSeedLinkList")]
        [XmlArrayItem("TableSampleClauseRepeatSeedLink")]
        public List<TableSampleClauseRepeatSeedLink> TableSampleClauseRepeatSeedLinkList { get; set; } = new();
        public bool ShouldSerializeTableSampleClauseRepeatSeedLinkList() => TableSampleClauseRepeatSeedLinkList.Count > 0;

        [XmlArray("TableSampleClauseSampleNumberLinkList")]
        [XmlArrayItem("TableSampleClauseSampleNumberLink")]
        public List<TableSampleClauseSampleNumberLink> TableSampleClauseSampleNumberLinkList { get; set; } = new();
        public bool ShouldSerializeTableSampleClauseSampleNumberLinkList() => TableSampleClauseSampleNumberLinkList.Count > 0;

        [XmlArray("TopRowFilterList")]
        [XmlArrayItem("TopRowFilter")]
        public List<TopRowFilter> TopRowFilterList { get; set; } = new();
        public bool ShouldSerializeTopRowFilterList() => TopRowFilterList.Count > 0;

        [XmlArray("TopRowFilterExpressionLinkList")]
        [XmlArrayItem("TopRowFilterExpressionLink")]
        public List<TopRowFilterExpressionLink> TopRowFilterExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeTopRowFilterExpressionLinkList() => TopRowFilterExpressionLinkList.Count > 0;

        [XmlArray("TransformScriptList")]
        [XmlArrayItem("TransformScript")]
        public List<TransformScript> TransformScriptList { get; set; } = new();
        public bool ShouldSerializeTransformScriptList() => TransformScriptList.Count > 0;

        [XmlArray("TransformScriptObjectIdentifierLinkList")]
        [XmlArrayItem("TransformScriptObjectIdentifierLink")]
        public List<TransformScriptObjectIdentifierLink> TransformScriptObjectIdentifierLinkList { get; set; } = new();
        public bool ShouldSerializeTransformScriptObjectIdentifierLinkList() => TransformScriptObjectIdentifierLinkList.Count > 0;

        [XmlArray("TransformScriptSchemaIdentifierLinkList")]
        [XmlArrayItem("TransformScriptSchemaIdentifierLink")]
        public List<TransformScriptSchemaIdentifierLink> TransformScriptSchemaIdentifierLinkList { get; set; } = new();
        public bool ShouldSerializeTransformScriptSchemaIdentifierLinkList() => TransformScriptSchemaIdentifierLinkList.Count > 0;

        [XmlArray("TransformScriptSelectStatementLinkList")]
        [XmlArrayItem("TransformScriptSelectStatementLink")]
        public List<TransformScriptSelectStatementLink> TransformScriptSelectStatementLinkList { get; set; } = new();
        public bool ShouldSerializeTransformScriptSelectStatementLinkList() => TransformScriptSelectStatementLinkList.Count > 0;

        [XmlArray("TransformScriptViewColumnsItemList")]
        [XmlArrayItem("TransformScriptViewColumnsItem")]
        public List<TransformScriptViewColumnsItem> TransformScriptViewColumnsItemList { get; set; } = new();
        public bool ShouldSerializeTransformScriptViewColumnsItemList() => TransformScriptViewColumnsItemList.Count > 0;

        [XmlArray("TryCastCallList")]
        [XmlArrayItem("TryCastCall")]
        public List<TryCastCall> TryCastCallList { get; set; } = new();
        public bool ShouldSerializeTryCastCallList() => TryCastCallList.Count > 0;

        [XmlArray("TryCastCallDataTypeLinkList")]
        [XmlArrayItem("TryCastCallDataTypeLink")]
        public List<TryCastCallDataTypeLink> TryCastCallDataTypeLinkList { get; set; } = new();
        public bool ShouldSerializeTryCastCallDataTypeLinkList() => TryCastCallDataTypeLinkList.Count > 0;

        [XmlArray("TryCastCallParameterLinkList")]
        [XmlArrayItem("TryCastCallParameterLink")]
        public List<TryCastCallParameterLink> TryCastCallParameterLinkList { get; set; } = new();
        public bool ShouldSerializeTryCastCallParameterLinkList() => TryCastCallParameterLinkList.Count > 0;

        [XmlArray("TryConvertCallList")]
        [XmlArrayItem("TryConvertCall")]
        public List<TryConvertCall> TryConvertCallList { get; set; } = new();
        public bool ShouldSerializeTryConvertCallList() => TryConvertCallList.Count > 0;

        [XmlArray("TryConvertCallDataTypeLinkList")]
        [XmlArrayItem("TryConvertCallDataTypeLink")]
        public List<TryConvertCallDataTypeLink> TryConvertCallDataTypeLinkList { get; set; } = new();
        public bool ShouldSerializeTryConvertCallDataTypeLinkList() => TryConvertCallDataTypeLinkList.Count > 0;

        [XmlArray("TryConvertCallParameterLinkList")]
        [XmlArrayItem("TryConvertCallParameterLink")]
        public List<TryConvertCallParameterLink> TryConvertCallParameterLinkList { get; set; } = new();
        public bool ShouldSerializeTryConvertCallParameterLinkList() => TryConvertCallParameterLinkList.Count > 0;

        [XmlArray("TryConvertCallStyleLinkList")]
        [XmlArrayItem("TryConvertCallStyleLink")]
        public List<TryConvertCallStyleLink> TryConvertCallStyleLinkList { get; set; } = new();
        public bool ShouldSerializeTryConvertCallStyleLinkList() => TryConvertCallStyleLinkList.Count > 0;

        [XmlArray("TryParseCallList")]
        [XmlArrayItem("TryParseCall")]
        public List<TryParseCall> TryParseCallList { get; set; } = new();
        public bool ShouldSerializeTryParseCallList() => TryParseCallList.Count > 0;

        [XmlArray("TryParseCallCultureLinkList")]
        [XmlArrayItem("TryParseCallCultureLink")]
        public List<TryParseCallCultureLink> TryParseCallCultureLinkList { get; set; } = new();
        public bool ShouldSerializeTryParseCallCultureLinkList() => TryParseCallCultureLinkList.Count > 0;

        [XmlArray("TryParseCallDataTypeLinkList")]
        [XmlArrayItem("TryParseCallDataTypeLink")]
        public List<TryParseCallDataTypeLink> TryParseCallDataTypeLinkList { get; set; } = new();
        public bool ShouldSerializeTryParseCallDataTypeLinkList() => TryParseCallDataTypeLinkList.Count > 0;

        [XmlArray("TryParseCallStringValueLinkList")]
        [XmlArrayItem("TryParseCallStringValueLink")]
        public List<TryParseCallStringValueLink> TryParseCallStringValueLinkList { get; set; } = new();
        public bool ShouldSerializeTryParseCallStringValueLinkList() => TryParseCallStringValueLinkList.Count > 0;

        [XmlArray("TSqlStatementList")]
        [XmlArrayItem("TSqlStatement")]
        public List<TSqlStatement> TSqlStatementList { get; set; } = new();
        public bool ShouldSerializeTSqlStatementList() => TSqlStatementList.Count > 0;

        [XmlArray("UnaryExpressionList")]
        [XmlArrayItem("UnaryExpression")]
        public List<UnaryExpression> UnaryExpressionList { get; set; } = new();
        public bool ShouldSerializeUnaryExpressionList() => UnaryExpressionList.Count > 0;

        [XmlArray("UnaryExpressionExpressionLinkList")]
        [XmlArrayItem("UnaryExpressionExpressionLink")]
        public List<UnaryExpressionExpressionLink> UnaryExpressionExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeUnaryExpressionExpressionLinkList() => UnaryExpressionExpressionLinkList.Count > 0;

        [XmlArray("UnpivotedTableReferenceList")]
        [XmlArrayItem("UnpivotedTableReference")]
        public List<UnpivotedTableReference> UnpivotedTableReferenceList { get; set; } = new();
        public bool ShouldSerializeUnpivotedTableReferenceList() => UnpivotedTableReferenceList.Count > 0;

        [XmlArray("UnpivotedTableReferenceInColumnsItemList")]
        [XmlArrayItem("UnpivotedTableReferenceInColumnsItem")]
        public List<UnpivotedTableReferenceInColumnsItem> UnpivotedTableReferenceInColumnsItemList { get; set; } = new();
        public bool ShouldSerializeUnpivotedTableReferenceInColumnsItemList() => UnpivotedTableReferenceInColumnsItemList.Count > 0;

        [XmlArray("UnpivotedTableReferencePivotColumnLinkList")]
        [XmlArrayItem("UnpivotedTableReferencePivotColumnLink")]
        public List<UnpivotedTableReferencePivotColumnLink> UnpivotedTableReferencePivotColumnLinkList { get; set; } = new();
        public bool ShouldSerializeUnpivotedTableReferencePivotColumnLinkList() => UnpivotedTableReferencePivotColumnLinkList.Count > 0;

        [XmlArray("UnpivotedTableReferenceTableReferenceLinkList")]
        [XmlArrayItem("UnpivotedTableReferenceTableReferenceLink")]
        public List<UnpivotedTableReferenceTableReferenceLink> UnpivotedTableReferenceTableReferenceLinkList { get; set; } = new();
        public bool ShouldSerializeUnpivotedTableReferenceTableReferenceLinkList() => UnpivotedTableReferenceTableReferenceLinkList.Count > 0;

        [XmlArray("UnpivotedTableReferenceValueColumnLinkList")]
        [XmlArrayItem("UnpivotedTableReferenceValueColumnLink")]
        public List<UnpivotedTableReferenceValueColumnLink> UnpivotedTableReferenceValueColumnLinkList { get; set; } = new();
        public bool ShouldSerializeUnpivotedTableReferenceValueColumnLinkList() => UnpivotedTableReferenceValueColumnLinkList.Count > 0;

        [XmlArray("UnqualifiedJoinList")]
        [XmlArrayItem("UnqualifiedJoin")]
        public List<UnqualifiedJoin> UnqualifiedJoinList { get; set; } = new();
        public bool ShouldSerializeUnqualifiedJoinList() => UnqualifiedJoinList.Count > 0;

        [XmlArray("ValueExpressionList")]
        [XmlArrayItem("ValueExpression")]
        public List<ValueExpression> ValueExpressionList { get; set; } = new();
        public bool ShouldSerializeValueExpressionList() => ValueExpressionList.Count > 0;

        [XmlArray("WhenClauseList")]
        [XmlArrayItem("WhenClause")]
        public List<WhenClause> WhenClauseList { get; set; } = new();
        public bool ShouldSerializeWhenClauseList() => WhenClauseList.Count > 0;

        [XmlArray("WhenClauseThenExpressionLinkList")]
        [XmlArrayItem("WhenClauseThenExpressionLink")]
        public List<WhenClauseThenExpressionLink> WhenClauseThenExpressionLinkList { get; set; } = new();
        public bool ShouldSerializeWhenClauseThenExpressionLinkList() => WhenClauseThenExpressionLinkList.Count > 0;

        [XmlArray("WhereClauseList")]
        [XmlArrayItem("WhereClause")]
        public List<WhereClause> WhereClauseList { get; set; } = new();
        public bool ShouldSerializeWhereClauseList() => WhereClauseList.Count > 0;

        [XmlArray("WhereClauseSearchConditionLinkList")]
        [XmlArrayItem("WhereClauseSearchConditionLink")]
        public List<WhereClauseSearchConditionLink> WhereClauseSearchConditionLinkList { get; set; } = new();
        public bool ShouldSerializeWhereClauseSearchConditionLinkList() => WhereClauseSearchConditionLinkList.Count > 0;

        [XmlArray("WindowClauseList")]
        [XmlArrayItem("WindowClause")]
        public List<WindowClause> WindowClauseList { get; set; } = new();
        public bool ShouldSerializeWindowClauseList() => WindowClauseList.Count > 0;

        [XmlArray("WindowClauseWindowDefinitionItemList")]
        [XmlArrayItem("WindowClauseWindowDefinitionItem")]
        public List<WindowClauseWindowDefinitionItem> WindowClauseWindowDefinitionItemList { get; set; } = new();
        public bool ShouldSerializeWindowClauseWindowDefinitionItemList() => WindowClauseWindowDefinitionItemList.Count > 0;

        [XmlArray("WindowDefinitionList")]
        [XmlArrayItem("WindowDefinition")]
        public List<WindowDefinition> WindowDefinitionList { get; set; } = new();
        public bool ShouldSerializeWindowDefinitionList() => WindowDefinitionList.Count > 0;

        [XmlArray("WindowDefinitionOrderByClauseLinkList")]
        [XmlArrayItem("WindowDefinitionOrderByClauseLink")]
        public List<WindowDefinitionOrderByClauseLink> WindowDefinitionOrderByClauseLinkList { get; set; } = new();
        public bool ShouldSerializeWindowDefinitionOrderByClauseLinkList() => WindowDefinitionOrderByClauseLinkList.Count > 0;

        [XmlArray("WindowDefinitionPartitionsItemList")]
        [XmlArrayItem("WindowDefinitionPartitionsItem")]
        public List<WindowDefinitionPartitionsItem> WindowDefinitionPartitionsItemList { get; set; } = new();
        public bool ShouldSerializeWindowDefinitionPartitionsItemList() => WindowDefinitionPartitionsItemList.Count > 0;

        [XmlArray("WindowDefinitionRefWindowNameLinkList")]
        [XmlArrayItem("WindowDefinitionRefWindowNameLink")]
        public List<WindowDefinitionRefWindowNameLink> WindowDefinitionRefWindowNameLinkList { get; set; } = new();
        public bool ShouldSerializeWindowDefinitionRefWindowNameLinkList() => WindowDefinitionRefWindowNameLinkList.Count > 0;

        [XmlArray("WindowDefinitionWindowFrameClauseLinkList")]
        [XmlArrayItem("WindowDefinitionWindowFrameClauseLink")]
        public List<WindowDefinitionWindowFrameClauseLink> WindowDefinitionWindowFrameClauseLinkList { get; set; } = new();
        public bool ShouldSerializeWindowDefinitionWindowFrameClauseLinkList() => WindowDefinitionWindowFrameClauseLinkList.Count > 0;

        [XmlArray("WindowDefinitionWindowNameLinkList")]
        [XmlArrayItem("WindowDefinitionWindowNameLink")]
        public List<WindowDefinitionWindowNameLink> WindowDefinitionWindowNameLinkList { get; set; } = new();
        public bool ShouldSerializeWindowDefinitionWindowNameLinkList() => WindowDefinitionWindowNameLinkList.Count > 0;

        [XmlArray("WindowDelimiterList")]
        [XmlArrayItem("WindowDelimiter")]
        public List<WindowDelimiter> WindowDelimiterList { get; set; } = new();
        public bool ShouldSerializeWindowDelimiterList() => WindowDelimiterList.Count > 0;

        [XmlArray("WindowDelimiterOffsetValueLinkList")]
        [XmlArrayItem("WindowDelimiterOffsetValueLink")]
        public List<WindowDelimiterOffsetValueLink> WindowDelimiterOffsetValueLinkList { get; set; } = new();
        public bool ShouldSerializeWindowDelimiterOffsetValueLinkList() => WindowDelimiterOffsetValueLinkList.Count > 0;

        [XmlArray("WindowFrameClauseList")]
        [XmlArrayItem("WindowFrameClause")]
        public List<WindowFrameClause> WindowFrameClauseList { get; set; } = new();
        public bool ShouldSerializeWindowFrameClauseList() => WindowFrameClauseList.Count > 0;

        [XmlArray("WindowFrameClauseBottomLinkList")]
        [XmlArrayItem("WindowFrameClauseBottomLink")]
        public List<WindowFrameClauseBottomLink> WindowFrameClauseBottomLinkList { get; set; } = new();
        public bool ShouldSerializeWindowFrameClauseBottomLinkList() => WindowFrameClauseBottomLinkList.Count > 0;

        [XmlArray("WindowFrameClauseTopLinkList")]
        [XmlArrayItem("WindowFrameClauseTopLink")]
        public List<WindowFrameClauseTopLink> WindowFrameClauseTopLinkList { get; set; } = new();
        public bool ShouldSerializeWindowFrameClauseTopLinkList() => WindowFrameClauseTopLinkList.Count > 0;

        [XmlArray("WithCtesAndXmlNamespacesList")]
        [XmlArrayItem("WithCtesAndXmlNamespaces")]
        public List<WithCtesAndXmlNamespaces> WithCtesAndXmlNamespacesList { get; set; } = new();
        public bool ShouldSerializeWithCtesAndXmlNamespacesList() => WithCtesAndXmlNamespacesList.Count > 0;

        [XmlArray("WithCtesAndXmlNamespacesCommonTableExpressionsItemList")]
        [XmlArrayItem("WithCtesAndXmlNamespacesCommonTableExpressionsItem")]
        public List<WithCtesAndXmlNamespacesCommonTableExpressionsItem> WithCtesAndXmlNamespacesCommonTableExpressionsItemList { get; set; } = new();
        public bool ShouldSerializeWithCtesAndXmlNamespacesCommonTableExpressionsItemList() => WithCtesAndXmlNamespacesCommonTableExpressionsItemList.Count > 0;

        [XmlArray("WithCtesAndXmlNamespacesXmlNamespacesLinkList")]
        [XmlArrayItem("WithCtesAndXmlNamespacesXmlNamespacesLink")]
        public List<WithCtesAndXmlNamespacesXmlNamespacesLink> WithCtesAndXmlNamespacesXmlNamespacesLinkList { get; set; } = new();
        public bool ShouldSerializeWithCtesAndXmlNamespacesXmlNamespacesLinkList() => WithCtesAndXmlNamespacesXmlNamespacesLinkList.Count > 0;

        [XmlArray("XmlNamespacesList")]
        [XmlArrayItem("XmlNamespaces")]
        public List<XmlNamespaces> XmlNamespacesList { get; set; } = new();
        public bool ShouldSerializeXmlNamespacesList() => XmlNamespacesList.Count > 0;

        [XmlArray("XmlNamespacesAliasElementList")]
        [XmlArrayItem("XmlNamespacesAliasElement")]
        public List<XmlNamespacesAliasElement> XmlNamespacesAliasElementList { get; set; } = new();
        public bool ShouldSerializeXmlNamespacesAliasElementList() => XmlNamespacesAliasElementList.Count > 0;

        [XmlArray("XmlNamespacesAliasElementIdentifierLinkList")]
        [XmlArrayItem("XmlNamespacesAliasElementIdentifierLink")]
        public List<XmlNamespacesAliasElementIdentifierLink> XmlNamespacesAliasElementIdentifierLinkList { get; set; } = new();
        public bool ShouldSerializeXmlNamespacesAliasElementIdentifierLinkList() => XmlNamespacesAliasElementIdentifierLinkList.Count > 0;

        [XmlArray("XmlNamespacesDefaultElementList")]
        [XmlArrayItem("XmlNamespacesDefaultElement")]
        public List<XmlNamespacesDefaultElement> XmlNamespacesDefaultElementList { get; set; } = new();
        public bool ShouldSerializeXmlNamespacesDefaultElementList() => XmlNamespacesDefaultElementList.Count > 0;

        [XmlArray("XmlNamespacesElementList")]
        [XmlArrayItem("XmlNamespacesElement")]
        public List<XmlNamespacesElement> XmlNamespacesElementList { get; set; } = new();
        public bool ShouldSerializeXmlNamespacesElementList() => XmlNamespacesElementList.Count > 0;

        [XmlArray("XmlNamespacesElementStringLinkList")]
        [XmlArrayItem("XmlNamespacesElementStringLink")]
        public List<XmlNamespacesElementStringLink> XmlNamespacesElementStringLinkList { get; set; } = new();
        public bool ShouldSerializeXmlNamespacesElementStringLinkList() => XmlNamespacesElementStringLinkList.Count > 0;

        [XmlArray("XmlNamespacesXmlNamespacesElementsItemList")]
        [XmlArrayItem("XmlNamespacesXmlNamespacesElementsItem")]
        public List<XmlNamespacesXmlNamespacesElementsItem> XmlNamespacesXmlNamespacesElementsItemList { get; set; } = new();
        public bool ShouldSerializeXmlNamespacesXmlNamespacesElementsItemList() => XmlNamespacesXmlNamespacesElementsItemList.Count > 0;

        public static MetaTransformScriptModel LoadFromXmlWorkspace(
            string workspacePath,
            bool searchUpward = true)
        {
            var model = TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(workspacePath, searchUpward);
            MetaTransformScriptModelFactory.Bind(model);
            return model;
        }

        public static Task<MetaTransformScriptModel> LoadFromXmlWorkspaceAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(LoadFromXmlWorkspace(workspacePath, searchUpward));
        }

        public void SaveToXmlWorkspace(string workspacePath)
        {
            MetaTransformScriptModelFactory.Bind(this);
            TypedWorkspaceXmlSerializer.Save(this, workspacePath, ResolveBundledModelXmlPath());
        }

        public Task SaveToXmlWorkspaceAsync(
            string workspacePath,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SaveToXmlWorkspace(workspacePath);
            return Task.CompletedTask;
        }

        private static string? ResolveBundledModelXmlPath()
        {
            var assemblyDirectory = Path.GetDirectoryName(typeof(MetaTransformScriptModel).Assembly.Location);
            if (string.IsNullOrWhiteSpace(assemblyDirectory))
            {
                return null;
            }

            var directPath = Path.Combine(assemblyDirectory, "model.xml");
            if (File.Exists(directPath))
            {
                return directPath;
            }

            var namespacedPath = Path.Combine(assemblyDirectory, "MetaTransformScript", "model.xml");
            return File.Exists(namespacedPath) ? namespacedPath : null;
        }
    }

    internal static class MetaTransformScriptModelFactory
    {
        internal static void Bind(MetaTransformScriptModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            model.AtTimeZoneCallList ??= new List<AtTimeZoneCall>();
            model.AtTimeZoneCallDateValueLinkList ??= new List<AtTimeZoneCallDateValueLink>();
            model.AtTimeZoneCallTimeZoneLinkList ??= new List<AtTimeZoneCallTimeZoneLink>();
            model.BinaryExpressionList ??= new List<BinaryExpression>();
            model.BinaryExpressionFirstExpressionLinkList ??= new List<BinaryExpressionFirstExpressionLink>();
            model.BinaryExpressionSecondExpressionLinkList ??= new List<BinaryExpressionSecondExpressionLink>();
            model.BinaryLiteralList ??= new List<BinaryLiteral>();
            model.BinaryQueryExpressionList ??= new List<BinaryQueryExpression>();
            model.BinaryQueryExpressionFirstQueryExpressionLinkList ??= new List<BinaryQueryExpressionFirstQueryExpressionLink>();
            model.BinaryQueryExpressionSecondQueryExpressionLinkList ??= new List<BinaryQueryExpressionSecondQueryExpressionLink>();
            model.BooleanBinaryExpressionList ??= new List<BooleanBinaryExpression>();
            model.BooleanBinaryExpressionFirstExpressionLinkList ??= new List<BooleanBinaryExpressionFirstExpressionLink>();
            model.BooleanBinaryExpressionSecondExpressionLinkList ??= new List<BooleanBinaryExpressionSecondExpressionLink>();
            model.BooleanComparisonExpressionList ??= new List<BooleanComparisonExpression>();
            model.BooleanComparisonExpressionFirstExpressionLinkList ??= new List<BooleanComparisonExpressionFirstExpressionLink>();
            model.BooleanComparisonExpressionSecondExpressionLinkList ??= new List<BooleanComparisonExpressionSecondExpressionLink>();
            model.BooleanExpressionList ??= new List<BooleanExpression>();
            model.BooleanIsNullExpressionList ??= new List<BooleanIsNullExpression>();
            model.BooleanIsNullExpressionExpressionLinkList ??= new List<BooleanIsNullExpressionExpressionLink>();
            model.BooleanNotExpressionList ??= new List<BooleanNotExpression>();
            model.BooleanNotExpressionExpressionLinkList ??= new List<BooleanNotExpressionExpressionLink>();
            model.BooleanParenthesisExpressionList ??= new List<BooleanParenthesisExpression>();
            model.BooleanParenthesisExpressionExpressionLinkList ??= new List<BooleanParenthesisExpressionExpressionLink>();
            model.BooleanTernaryExpressionList ??= new List<BooleanTernaryExpression>();
            model.BooleanTernaryExpressionFirstExpressionLinkList ??= new List<BooleanTernaryExpressionFirstExpressionLink>();
            model.BooleanTernaryExpressionSecondExpressionLinkList ??= new List<BooleanTernaryExpressionSecondExpressionLink>();
            model.BooleanTernaryExpressionThirdExpressionLinkList ??= new List<BooleanTernaryExpressionThirdExpressionLink>();
            model.CallTargetList ??= new List<CallTarget>();
            model.CaseExpressionList ??= new List<CaseExpression>();
            model.CaseExpressionElseExpressionLinkList ??= new List<CaseExpressionElseExpressionLink>();
            model.CastCallList ??= new List<CastCall>();
            model.CastCallDataTypeLinkList ??= new List<CastCallDataTypeLink>();
            model.CastCallParameterLinkList ??= new List<CastCallParameterLink>();
            model.CoalesceExpressionList ??= new List<CoalesceExpression>();
            model.CoalesceExpressionExpressionsItemList ??= new List<CoalesceExpressionExpressionsItem>();
            model.ColumnReferenceExpressionList ??= new List<ColumnReferenceExpression>();
            model.ColumnReferenceExpressionMultiPartIdentifierLinkList ??= new List<ColumnReferenceExpressionMultiPartIdentifierLink>();
            model.CommonTableExpressionList ??= new List<CommonTableExpression>();
            model.CommonTableExpressionColumnsItemList ??= new List<CommonTableExpressionColumnsItem>();
            model.CommonTableExpressionExpressionNameLinkList ??= new List<CommonTableExpressionExpressionNameLink>();
            model.CommonTableExpressionQueryExpressionLinkList ??= new List<CommonTableExpressionQueryExpressionLink>();
            model.CompositeGroupingSpecificationList ??= new List<CompositeGroupingSpecification>();
            model.CompositeGroupingSpecificationItemsItemList ??= new List<CompositeGroupingSpecificationItemsItem>();
            model.ConvertCallList ??= new List<ConvertCall>();
            model.ConvertCallDataTypeLinkList ??= new List<ConvertCallDataTypeLink>();
            model.ConvertCallParameterLinkList ??= new List<ConvertCallParameterLink>();
            model.ConvertCallStyleLinkList ??= new List<ConvertCallStyleLink>();
            model.CubeGroupingSpecificationList ??= new List<CubeGroupingSpecification>();
            model.CubeGroupingSpecificationArgumentsItemList ??= new List<CubeGroupingSpecificationArgumentsItem>();
            model.DataTypeReferenceList ??= new List<DataTypeReference>();
            model.DataTypeReferenceNameLinkList ??= new List<DataTypeReferenceNameLink>();
            model.DistinctPredicateList ??= new List<DistinctPredicate>();
            model.DistinctPredicateFirstExpressionLinkList ??= new List<DistinctPredicateFirstExpressionLink>();
            model.DistinctPredicateSecondExpressionLinkList ??= new List<DistinctPredicateSecondExpressionLink>();
            model.ExistsPredicateList ??= new List<ExistsPredicate>();
            model.ExistsPredicateSubqueryLinkList ??= new List<ExistsPredicateSubqueryLink>();
            model.ExpressionGroupingSpecificationList ??= new List<ExpressionGroupingSpecification>();
            model.ExpressionGroupingSpecificationExpressionLinkList ??= new List<ExpressionGroupingSpecificationExpressionLink>();
            model.ExpressionWithSortOrderList ??= new List<ExpressionWithSortOrder>();
            model.ExpressionWithSortOrderExpressionLinkList ??= new List<ExpressionWithSortOrderExpressionLink>();
            model.FromClauseList ??= new List<FromClause>();
            model.FromClauseTableReferencesItemList ??= new List<FromClauseTableReferencesItem>();
            model.FullTextPredicateList ??= new List<FullTextPredicate>();
            model.FullTextPredicateColumnsItemList ??= new List<FullTextPredicateColumnsItem>();
            model.FullTextPredicateValueLinkList ??= new List<FullTextPredicateValueLink>();
            model.FullTextTableReferenceList ??= new List<FullTextTableReference>();
            model.FullTextTableReferenceColumnsItemList ??= new List<FullTextTableReferenceColumnsItem>();
            model.FullTextTableReferenceSearchConditionLinkList ??= new List<FullTextTableReferenceSearchConditionLink>();
            model.FullTextTableReferenceTableNameLinkList ??= new List<FullTextTableReferenceTableNameLink>();
            model.FunctionCallList ??= new List<FunctionCall>();
            model.FunctionCallCallTargetLinkList ??= new List<FunctionCallCallTargetLink>();
            model.FunctionCallFunctionNameLinkList ??= new List<FunctionCallFunctionNameLink>();
            model.FunctionCallOverClauseLinkList ??= new List<FunctionCallOverClauseLink>();
            model.FunctionCallParametersItemList ??= new List<FunctionCallParametersItem>();
            model.GlobalFunctionTableReferenceList ??= new List<GlobalFunctionTableReference>();
            model.GlobalFunctionTableReferenceNameLinkList ??= new List<GlobalFunctionTableReferenceNameLink>();
            model.GlobalFunctionTableReferenceParametersItemList ??= new List<GlobalFunctionTableReferenceParametersItem>();
            model.GlobalVariableExpressionList ??= new List<GlobalVariableExpression>();
            model.GrandTotalGroupingSpecificationList ??= new List<GrandTotalGroupingSpecification>();
            model.GroupByClauseList ??= new List<GroupByClause>();
            model.GroupByClauseGroupingSpecificationsItemList ??= new List<GroupByClauseGroupingSpecificationsItem>();
            model.GroupingSetsGroupingSpecificationList ??= new List<GroupingSetsGroupingSpecification>();
            model.GroupingSetsGroupingSpecificationSetsItemList ??= new List<GroupingSetsGroupingSpecificationSetsItem>();
            model.GroupingSpecificationList ??= new List<GroupingSpecification>();
            model.HavingClauseList ??= new List<HavingClause>();
            model.HavingClauseSearchConditionLinkList ??= new List<HavingClauseSearchConditionLink>();
            model.IdentifierList ??= new List<Identifier>();
            model.IdentifierOrValueExpressionList ??= new List<IdentifierOrValueExpression>();
            model.IdentifierOrValueExpressionIdentifierLinkList ??= new List<IdentifierOrValueExpressionIdentifierLink>();
            model.IIfCallList ??= new List<IIfCall>();
            model.IIfCallElseExpressionLinkList ??= new List<IIfCallElseExpressionLink>();
            model.IIfCallPredicateLinkList ??= new List<IIfCallPredicateLink>();
            model.IIfCallThenExpressionLinkList ??= new List<IIfCallThenExpressionLink>();
            model.InlineDerivedTableList ??= new List<InlineDerivedTable>();
            model.InlineDerivedTableRowValuesItemList ??= new List<InlineDerivedTableRowValuesItem>();
            model.InPredicateList ??= new List<InPredicate>();
            model.InPredicateExpressionLinkList ??= new List<InPredicateExpressionLink>();
            model.InPredicateSubqueryLinkList ??= new List<InPredicateSubqueryLink>();
            model.InPredicateValuesItemList ??= new List<InPredicateValuesItem>();
            model.IntegerLiteralList ??= new List<IntegerLiteral>();
            model.JoinParenthesisTableReferenceList ??= new List<JoinParenthesisTableReference>();
            model.JoinParenthesisTableReferenceJoinLinkList ??= new List<JoinParenthesisTableReferenceJoinLink>();
            model.JoinTableReferenceList ??= new List<JoinTableReference>();
            model.JoinTableReferenceFirstTableReferenceLinkList ??= new List<JoinTableReferenceFirstTableReferenceLink>();
            model.JoinTableReferenceSecondTableReferenceLinkList ??= new List<JoinTableReferenceSecondTableReferenceLink>();
            model.LeftFunctionCallList ??= new List<LeftFunctionCall>();
            model.LeftFunctionCallParametersItemList ??= new List<LeftFunctionCallParametersItem>();
            model.LikePredicateList ??= new List<LikePredicate>();
            model.LikePredicateFirstExpressionLinkList ??= new List<LikePredicateFirstExpressionLink>();
            model.LikePredicateSecondExpressionLinkList ??= new List<LikePredicateSecondExpressionLink>();
            model.LiteralList ??= new List<Literal>();
            model.MaxLiteralList ??= new List<MaxLiteral>();
            model.MultiPartIdentifierList ??= new List<MultiPartIdentifier>();
            model.MultiPartIdentifierCallTargetList ??= new List<MultiPartIdentifierCallTarget>();
            model.MultiPartIdentifierCallTargetMultiPartIdentifierLinkList ??= new List<MultiPartIdentifierCallTargetMultiPartIdentifierLink>();
            model.MultiPartIdentifierIdentifiersItemList ??= new List<MultiPartIdentifierIdentifiersItem>();
            model.NamedTableReferenceList ??= new List<NamedTableReference>();
            model.NamedTableReferenceSchemaObjectLinkList ??= new List<NamedTableReferenceSchemaObjectLink>();
            model.NamedTableReferenceTableSampleClauseLinkList ??= new List<NamedTableReferenceTableSampleClauseLink>();
            model.NextValueForExpressionList ??= new List<NextValueForExpression>();
            model.NextValueForExpressionSequenceNameLinkList ??= new List<NextValueForExpressionSequenceNameLink>();
            model.NullIfExpressionList ??= new List<NullIfExpression>();
            model.NullIfExpressionFirstExpressionLinkList ??= new List<NullIfExpressionFirstExpressionLink>();
            model.NullIfExpressionSecondExpressionLinkList ??= new List<NullIfExpressionSecondExpressionLink>();
            model.NullLiteralList ??= new List<NullLiteral>();
            model.NumericLiteralList ??= new List<NumericLiteral>();
            model.OffsetClauseList ??= new List<OffsetClause>();
            model.OffsetClauseFetchExpressionLinkList ??= new List<OffsetClauseFetchExpressionLink>();
            model.OffsetClauseOffsetExpressionLinkList ??= new List<OffsetClauseOffsetExpressionLink>();
            model.OrderByClauseList ??= new List<OrderByClause>();
            model.OrderByClauseOrderByElementsItemList ??= new List<OrderByClauseOrderByElementsItem>();
            model.OverClauseList ??= new List<OverClause>();
            model.OverClauseOrderByClauseLinkList ??= new List<OverClauseOrderByClauseLink>();
            model.OverClausePartitionsItemList ??= new List<OverClausePartitionsItem>();
            model.OverClauseWindowFrameClauseLinkList ??= new List<OverClauseWindowFrameClauseLink>();
            model.OverClauseWindowNameLinkList ??= new List<OverClauseWindowNameLink>();
            model.ParameterizedDataTypeReferenceList ??= new List<ParameterizedDataTypeReference>();
            model.ParameterizedDataTypeReferenceParametersItemList ??= new List<ParameterizedDataTypeReferenceParametersItem>();
            model.ParameterlessCallList ??= new List<ParameterlessCall>();
            model.ParenthesisExpressionList ??= new List<ParenthesisExpression>();
            model.ParenthesisExpressionExpressionLinkList ??= new List<ParenthesisExpressionExpressionLink>();
            model.ParseCallList ??= new List<ParseCall>();
            model.ParseCallCultureLinkList ??= new List<ParseCallCultureLink>();
            model.ParseCallDataTypeLinkList ??= new List<ParseCallDataTypeLink>();
            model.ParseCallStringValueLinkList ??= new List<ParseCallStringValueLink>();
            model.PivotedTableReferenceList ??= new List<PivotedTableReference>();
            model.PivotedTableReferenceAggregateFunctionIdentifierLinkList ??= new List<PivotedTableReferenceAggregateFunctionIdentifierLink>();
            model.PivotedTableReferenceInColumnsItemList ??= new List<PivotedTableReferenceInColumnsItem>();
            model.PivotedTableReferencePivotColumnLinkList ??= new List<PivotedTableReferencePivotColumnLink>();
            model.PivotedTableReferenceTableReferenceLinkList ??= new List<PivotedTableReferenceTableReferenceLink>();
            model.PivotedTableReferenceValueColumnsItemList ??= new List<PivotedTableReferenceValueColumnsItem>();
            model.PrimaryExpressionList ??= new List<PrimaryExpression>();
            model.PrimaryExpressionCollationLinkList ??= new List<PrimaryExpressionCollationLink>();
            model.QualifiedJoinList ??= new List<QualifiedJoin>();
            model.QualifiedJoinSearchConditionLinkList ??= new List<QualifiedJoinSearchConditionLink>();
            model.QueryDerivedTableList ??= new List<QueryDerivedTable>();
            model.QueryDerivedTableQueryExpressionLinkList ??= new List<QueryDerivedTableQueryExpressionLink>();
            model.QueryExpressionList ??= new List<QueryExpression>();
            model.QueryExpressionOffsetClauseLinkList ??= new List<QueryExpressionOffsetClauseLink>();
            model.QueryExpressionOrderByClauseLinkList ??= new List<QueryExpressionOrderByClauseLink>();
            model.QueryParenthesisExpressionList ??= new List<QueryParenthesisExpression>();
            model.QueryParenthesisExpressionQueryExpressionLinkList ??= new List<QueryParenthesisExpressionQueryExpressionLink>();
            model.QuerySpecificationList ??= new List<QuerySpecification>();
            model.QuerySpecificationFromClauseLinkList ??= new List<QuerySpecificationFromClauseLink>();
            model.QuerySpecificationGroupByClauseLinkList ??= new List<QuerySpecificationGroupByClauseLink>();
            model.QuerySpecificationHavingClauseLinkList ??= new List<QuerySpecificationHavingClauseLink>();
            model.QuerySpecificationSelectElementsItemList ??= new List<QuerySpecificationSelectElementsItem>();
            model.QuerySpecificationTopRowFilterLinkList ??= new List<QuerySpecificationTopRowFilterLink>();
            model.QuerySpecificationWhereClauseLinkList ??= new List<QuerySpecificationWhereClauseLink>();
            model.QuerySpecificationWindowClauseLinkList ??= new List<QuerySpecificationWindowClauseLink>();
            model.RealLiteralList ??= new List<RealLiteral>();
            model.RightFunctionCallList ??= new List<RightFunctionCall>();
            model.RightFunctionCallParametersItemList ??= new List<RightFunctionCallParametersItem>();
            model.RollupGroupingSpecificationList ??= new List<RollupGroupingSpecification>();
            model.RollupGroupingSpecificationArgumentsItemList ??= new List<RollupGroupingSpecificationArgumentsItem>();
            model.RowValueList ??= new List<RowValue>();
            model.RowValueColumnValuesItemList ??= new List<RowValueColumnValuesItem>();
            model.ScalarExpressionList ??= new List<ScalarExpression>();
            model.ScalarSubqueryList ??= new List<ScalarSubquery>();
            model.ScalarSubqueryQueryExpressionLinkList ??= new List<ScalarSubqueryQueryExpressionLink>();
            model.SchemaObjectFunctionTableReferenceList ??= new List<SchemaObjectFunctionTableReference>();
            model.SchemaObjectFunctionTableReferenceParametersItemList ??= new List<SchemaObjectFunctionTableReferenceParametersItem>();
            model.SchemaObjectFunctionTableReferenceSchemaObjectLinkList ??= new List<SchemaObjectFunctionTableReferenceSchemaObjectLink>();
            model.SchemaObjectNameList ??= new List<SchemaObjectName>();
            model.SchemaObjectNameBaseIdentifierLinkList ??= new List<SchemaObjectNameBaseIdentifierLink>();
            model.SchemaObjectNameSchemaIdentifierLinkList ??= new List<SchemaObjectNameSchemaIdentifierLink>();
            model.SearchedCaseExpressionList ??= new List<SearchedCaseExpression>();
            model.SearchedCaseExpressionWhenClausesItemList ??= new List<SearchedCaseExpressionWhenClausesItem>();
            model.SearchedWhenClauseList ??= new List<SearchedWhenClause>();
            model.SearchedWhenClauseWhenExpressionLinkList ??= new List<SearchedWhenClauseWhenExpressionLink>();
            model.SelectElementList ??= new List<SelectElement>();
            model.SelectScalarExpressionList ??= new List<SelectScalarExpression>();
            model.SelectScalarExpressionColumnNameLinkList ??= new List<SelectScalarExpressionColumnNameLink>();
            model.SelectScalarExpressionExpressionLinkList ??= new List<SelectScalarExpressionExpressionLink>();
            model.SelectStarExpressionList ??= new List<SelectStarExpression>();
            model.SelectStarExpressionQualifierLinkList ??= new List<SelectStarExpressionQualifierLink>();
            model.SelectStatementList ??= new List<SelectStatement>();
            model.SelectStatementQueryExpressionLinkList ??= new List<SelectStatementQueryExpressionLink>();
            model.SimpleCaseExpressionList ??= new List<SimpleCaseExpression>();
            model.SimpleCaseExpressionInputExpressionLinkList ??= new List<SimpleCaseExpressionInputExpressionLink>();
            model.SimpleCaseExpressionWhenClausesItemList ??= new List<SimpleCaseExpressionWhenClausesItem>();
            model.SimpleWhenClauseList ??= new List<SimpleWhenClause>();
            model.SimpleWhenClauseWhenExpressionLinkList ??= new List<SimpleWhenClauseWhenExpressionLink>();
            model.SqlDataTypeReferenceList ??= new List<SqlDataTypeReference>();
            model.StatementWithCtesAndXmlNamespacesList ??= new List<StatementWithCtesAndXmlNamespaces>();
            model.StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkList ??= new List<StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLink>();
            model.StringLiteralList ??= new List<StringLiteral>();
            model.SubqueryComparisonPredicateList ??= new List<SubqueryComparisonPredicate>();
            model.SubqueryComparisonPredicateExpressionLinkList ??= new List<SubqueryComparisonPredicateExpressionLink>();
            model.SubqueryComparisonPredicateSubqueryLinkList ??= new List<SubqueryComparisonPredicateSubqueryLink>();
            model.TableReferenceList ??= new List<TableReference>();
            model.TableReferenceWithAliasList ??= new List<TableReferenceWithAlias>();
            model.TableReferenceWithAliasAliasLinkList ??= new List<TableReferenceWithAliasAliasLink>();
            model.TableReferenceWithAliasAndColumnsList ??= new List<TableReferenceWithAliasAndColumns>();
            model.TableReferenceWithAliasAndColumnsColumnsItemList ??= new List<TableReferenceWithAliasAndColumnsColumnsItem>();
            model.TableSampleClauseList ??= new List<TableSampleClause>();
            model.TableSampleClauseRepeatSeedLinkList ??= new List<TableSampleClauseRepeatSeedLink>();
            model.TableSampleClauseSampleNumberLinkList ??= new List<TableSampleClauseSampleNumberLink>();
            model.TopRowFilterList ??= new List<TopRowFilter>();
            model.TopRowFilterExpressionLinkList ??= new List<TopRowFilterExpressionLink>();
            model.TransformScriptList ??= new List<TransformScript>();
            model.TransformScriptObjectIdentifierLinkList ??= new List<TransformScriptObjectIdentifierLink>();
            model.TransformScriptSchemaIdentifierLinkList ??= new List<TransformScriptSchemaIdentifierLink>();
            model.TransformScriptSelectStatementLinkList ??= new List<TransformScriptSelectStatementLink>();
            model.TransformScriptViewColumnsItemList ??= new List<TransformScriptViewColumnsItem>();
            model.TryCastCallList ??= new List<TryCastCall>();
            model.TryCastCallDataTypeLinkList ??= new List<TryCastCallDataTypeLink>();
            model.TryCastCallParameterLinkList ??= new List<TryCastCallParameterLink>();
            model.TryConvertCallList ??= new List<TryConvertCall>();
            model.TryConvertCallDataTypeLinkList ??= new List<TryConvertCallDataTypeLink>();
            model.TryConvertCallParameterLinkList ??= new List<TryConvertCallParameterLink>();
            model.TryConvertCallStyleLinkList ??= new List<TryConvertCallStyleLink>();
            model.TryParseCallList ??= new List<TryParseCall>();
            model.TryParseCallCultureLinkList ??= new List<TryParseCallCultureLink>();
            model.TryParseCallDataTypeLinkList ??= new List<TryParseCallDataTypeLink>();
            model.TryParseCallStringValueLinkList ??= new List<TryParseCallStringValueLink>();
            model.TSqlStatementList ??= new List<TSqlStatement>();
            model.UnaryExpressionList ??= new List<UnaryExpression>();
            model.UnaryExpressionExpressionLinkList ??= new List<UnaryExpressionExpressionLink>();
            model.UnpivotedTableReferenceList ??= new List<UnpivotedTableReference>();
            model.UnpivotedTableReferenceInColumnsItemList ??= new List<UnpivotedTableReferenceInColumnsItem>();
            model.UnpivotedTableReferencePivotColumnLinkList ??= new List<UnpivotedTableReferencePivotColumnLink>();
            model.UnpivotedTableReferenceTableReferenceLinkList ??= new List<UnpivotedTableReferenceTableReferenceLink>();
            model.UnpivotedTableReferenceValueColumnLinkList ??= new List<UnpivotedTableReferenceValueColumnLink>();
            model.UnqualifiedJoinList ??= new List<UnqualifiedJoin>();
            model.ValueExpressionList ??= new List<ValueExpression>();
            model.WhenClauseList ??= new List<WhenClause>();
            model.WhenClauseThenExpressionLinkList ??= new List<WhenClauseThenExpressionLink>();
            model.WhereClauseList ??= new List<WhereClause>();
            model.WhereClauseSearchConditionLinkList ??= new List<WhereClauseSearchConditionLink>();
            model.WindowClauseList ??= new List<WindowClause>();
            model.WindowClauseWindowDefinitionItemList ??= new List<WindowClauseWindowDefinitionItem>();
            model.WindowDefinitionList ??= new List<WindowDefinition>();
            model.WindowDefinitionOrderByClauseLinkList ??= new List<WindowDefinitionOrderByClauseLink>();
            model.WindowDefinitionPartitionsItemList ??= new List<WindowDefinitionPartitionsItem>();
            model.WindowDefinitionRefWindowNameLinkList ??= new List<WindowDefinitionRefWindowNameLink>();
            model.WindowDefinitionWindowFrameClauseLinkList ??= new List<WindowDefinitionWindowFrameClauseLink>();
            model.WindowDefinitionWindowNameLinkList ??= new List<WindowDefinitionWindowNameLink>();
            model.WindowDelimiterList ??= new List<WindowDelimiter>();
            model.WindowDelimiterOffsetValueLinkList ??= new List<WindowDelimiterOffsetValueLink>();
            model.WindowFrameClauseList ??= new List<WindowFrameClause>();
            model.WindowFrameClauseBottomLinkList ??= new List<WindowFrameClauseBottomLink>();
            model.WindowFrameClauseTopLinkList ??= new List<WindowFrameClauseTopLink>();
            model.WithCtesAndXmlNamespacesList ??= new List<WithCtesAndXmlNamespaces>();
            model.WithCtesAndXmlNamespacesCommonTableExpressionsItemList ??= new List<WithCtesAndXmlNamespacesCommonTableExpressionsItem>();
            model.WithCtesAndXmlNamespacesXmlNamespacesLinkList ??= new List<WithCtesAndXmlNamespacesXmlNamespacesLink>();
            model.XmlNamespacesList ??= new List<XmlNamespaces>();
            model.XmlNamespacesAliasElementList ??= new List<XmlNamespacesAliasElement>();
            model.XmlNamespacesAliasElementIdentifierLinkList ??= new List<XmlNamespacesAliasElementIdentifierLink>();
            model.XmlNamespacesDefaultElementList ??= new List<XmlNamespacesDefaultElement>();
            model.XmlNamespacesElementList ??= new List<XmlNamespacesElement>();
            model.XmlNamespacesElementStringLinkList ??= new List<XmlNamespacesElementStringLink>();
            model.XmlNamespacesXmlNamespacesElementsItemList ??= new List<XmlNamespacesXmlNamespacesElementsItem>();

            NormalizeAtTimeZoneCallList(model);
            NormalizeAtTimeZoneCallDateValueLinkList(model);
            NormalizeAtTimeZoneCallTimeZoneLinkList(model);
            NormalizeBinaryExpressionList(model);
            NormalizeBinaryExpressionFirstExpressionLinkList(model);
            NormalizeBinaryExpressionSecondExpressionLinkList(model);
            NormalizeBinaryLiteralList(model);
            NormalizeBinaryQueryExpressionList(model);
            NormalizeBinaryQueryExpressionFirstQueryExpressionLinkList(model);
            NormalizeBinaryQueryExpressionSecondQueryExpressionLinkList(model);
            NormalizeBooleanBinaryExpressionList(model);
            NormalizeBooleanBinaryExpressionFirstExpressionLinkList(model);
            NormalizeBooleanBinaryExpressionSecondExpressionLinkList(model);
            NormalizeBooleanComparisonExpressionList(model);
            NormalizeBooleanComparisonExpressionFirstExpressionLinkList(model);
            NormalizeBooleanComparisonExpressionSecondExpressionLinkList(model);
            NormalizeBooleanExpressionList(model);
            NormalizeBooleanIsNullExpressionList(model);
            NormalizeBooleanIsNullExpressionExpressionLinkList(model);
            NormalizeBooleanNotExpressionList(model);
            NormalizeBooleanNotExpressionExpressionLinkList(model);
            NormalizeBooleanParenthesisExpressionList(model);
            NormalizeBooleanParenthesisExpressionExpressionLinkList(model);
            NormalizeBooleanTernaryExpressionList(model);
            NormalizeBooleanTernaryExpressionFirstExpressionLinkList(model);
            NormalizeBooleanTernaryExpressionSecondExpressionLinkList(model);
            NormalizeBooleanTernaryExpressionThirdExpressionLinkList(model);
            NormalizeCallTargetList(model);
            NormalizeCaseExpressionList(model);
            NormalizeCaseExpressionElseExpressionLinkList(model);
            NormalizeCastCallList(model);
            NormalizeCastCallDataTypeLinkList(model);
            NormalizeCastCallParameterLinkList(model);
            NormalizeCoalesceExpressionList(model);
            NormalizeCoalesceExpressionExpressionsItemList(model);
            NormalizeColumnReferenceExpressionList(model);
            NormalizeColumnReferenceExpressionMultiPartIdentifierLinkList(model);
            NormalizeCommonTableExpressionList(model);
            NormalizeCommonTableExpressionColumnsItemList(model);
            NormalizeCommonTableExpressionExpressionNameLinkList(model);
            NormalizeCommonTableExpressionQueryExpressionLinkList(model);
            NormalizeCompositeGroupingSpecificationList(model);
            NormalizeCompositeGroupingSpecificationItemsItemList(model);
            NormalizeConvertCallList(model);
            NormalizeConvertCallDataTypeLinkList(model);
            NormalizeConvertCallParameterLinkList(model);
            NormalizeConvertCallStyleLinkList(model);
            NormalizeCubeGroupingSpecificationList(model);
            NormalizeCubeGroupingSpecificationArgumentsItemList(model);
            NormalizeDataTypeReferenceList(model);
            NormalizeDataTypeReferenceNameLinkList(model);
            NormalizeDistinctPredicateList(model);
            NormalizeDistinctPredicateFirstExpressionLinkList(model);
            NormalizeDistinctPredicateSecondExpressionLinkList(model);
            NormalizeExistsPredicateList(model);
            NormalizeExistsPredicateSubqueryLinkList(model);
            NormalizeExpressionGroupingSpecificationList(model);
            NormalizeExpressionGroupingSpecificationExpressionLinkList(model);
            NormalizeExpressionWithSortOrderList(model);
            NormalizeExpressionWithSortOrderExpressionLinkList(model);
            NormalizeFromClauseList(model);
            NormalizeFromClauseTableReferencesItemList(model);
            NormalizeFullTextPredicateList(model);
            NormalizeFullTextPredicateColumnsItemList(model);
            NormalizeFullTextPredicateValueLinkList(model);
            NormalizeFullTextTableReferenceList(model);
            NormalizeFullTextTableReferenceColumnsItemList(model);
            NormalizeFullTextTableReferenceSearchConditionLinkList(model);
            NormalizeFullTextTableReferenceTableNameLinkList(model);
            NormalizeFunctionCallList(model);
            NormalizeFunctionCallCallTargetLinkList(model);
            NormalizeFunctionCallFunctionNameLinkList(model);
            NormalizeFunctionCallOverClauseLinkList(model);
            NormalizeFunctionCallParametersItemList(model);
            NormalizeGlobalFunctionTableReferenceList(model);
            NormalizeGlobalFunctionTableReferenceNameLinkList(model);
            NormalizeGlobalFunctionTableReferenceParametersItemList(model);
            NormalizeGlobalVariableExpressionList(model);
            NormalizeGrandTotalGroupingSpecificationList(model);
            NormalizeGroupByClauseList(model);
            NormalizeGroupByClauseGroupingSpecificationsItemList(model);
            NormalizeGroupingSetsGroupingSpecificationList(model);
            NormalizeGroupingSetsGroupingSpecificationSetsItemList(model);
            NormalizeGroupingSpecificationList(model);
            NormalizeHavingClauseList(model);
            NormalizeHavingClauseSearchConditionLinkList(model);
            NormalizeIdentifierList(model);
            NormalizeIdentifierOrValueExpressionList(model);
            NormalizeIdentifierOrValueExpressionIdentifierLinkList(model);
            NormalizeIIfCallList(model);
            NormalizeIIfCallElseExpressionLinkList(model);
            NormalizeIIfCallPredicateLinkList(model);
            NormalizeIIfCallThenExpressionLinkList(model);
            NormalizeInlineDerivedTableList(model);
            NormalizeInlineDerivedTableRowValuesItemList(model);
            NormalizeInPredicateList(model);
            NormalizeInPredicateExpressionLinkList(model);
            NormalizeInPredicateSubqueryLinkList(model);
            NormalizeInPredicateValuesItemList(model);
            NormalizeIntegerLiteralList(model);
            NormalizeJoinParenthesisTableReferenceList(model);
            NormalizeJoinParenthesisTableReferenceJoinLinkList(model);
            NormalizeJoinTableReferenceList(model);
            NormalizeJoinTableReferenceFirstTableReferenceLinkList(model);
            NormalizeJoinTableReferenceSecondTableReferenceLinkList(model);
            NormalizeLeftFunctionCallList(model);
            NormalizeLeftFunctionCallParametersItemList(model);
            NormalizeLikePredicateList(model);
            NormalizeLikePredicateFirstExpressionLinkList(model);
            NormalizeLikePredicateSecondExpressionLinkList(model);
            NormalizeLiteralList(model);
            NormalizeMaxLiteralList(model);
            NormalizeMultiPartIdentifierList(model);
            NormalizeMultiPartIdentifierCallTargetList(model);
            NormalizeMultiPartIdentifierCallTargetMultiPartIdentifierLinkList(model);
            NormalizeMultiPartIdentifierIdentifiersItemList(model);
            NormalizeNamedTableReferenceList(model);
            NormalizeNamedTableReferenceSchemaObjectLinkList(model);
            NormalizeNamedTableReferenceTableSampleClauseLinkList(model);
            NormalizeNextValueForExpressionList(model);
            NormalizeNextValueForExpressionSequenceNameLinkList(model);
            NormalizeNullIfExpressionList(model);
            NormalizeNullIfExpressionFirstExpressionLinkList(model);
            NormalizeNullIfExpressionSecondExpressionLinkList(model);
            NormalizeNullLiteralList(model);
            NormalizeNumericLiteralList(model);
            NormalizeOffsetClauseList(model);
            NormalizeOffsetClauseFetchExpressionLinkList(model);
            NormalizeOffsetClauseOffsetExpressionLinkList(model);
            NormalizeOrderByClauseList(model);
            NormalizeOrderByClauseOrderByElementsItemList(model);
            NormalizeOverClauseList(model);
            NormalizeOverClauseOrderByClauseLinkList(model);
            NormalizeOverClausePartitionsItemList(model);
            NormalizeOverClauseWindowFrameClauseLinkList(model);
            NormalizeOverClauseWindowNameLinkList(model);
            NormalizeParameterizedDataTypeReferenceList(model);
            NormalizeParameterizedDataTypeReferenceParametersItemList(model);
            NormalizeParameterlessCallList(model);
            NormalizeParenthesisExpressionList(model);
            NormalizeParenthesisExpressionExpressionLinkList(model);
            NormalizeParseCallList(model);
            NormalizeParseCallCultureLinkList(model);
            NormalizeParseCallDataTypeLinkList(model);
            NormalizeParseCallStringValueLinkList(model);
            NormalizePivotedTableReferenceList(model);
            NormalizePivotedTableReferenceAggregateFunctionIdentifierLinkList(model);
            NormalizePivotedTableReferenceInColumnsItemList(model);
            NormalizePivotedTableReferencePivotColumnLinkList(model);
            NormalizePivotedTableReferenceTableReferenceLinkList(model);
            NormalizePivotedTableReferenceValueColumnsItemList(model);
            NormalizePrimaryExpressionList(model);
            NormalizePrimaryExpressionCollationLinkList(model);
            NormalizeQualifiedJoinList(model);
            NormalizeQualifiedJoinSearchConditionLinkList(model);
            NormalizeQueryDerivedTableList(model);
            NormalizeQueryDerivedTableQueryExpressionLinkList(model);
            NormalizeQueryExpressionList(model);
            NormalizeQueryExpressionOffsetClauseLinkList(model);
            NormalizeQueryExpressionOrderByClauseLinkList(model);
            NormalizeQueryParenthesisExpressionList(model);
            NormalizeQueryParenthesisExpressionQueryExpressionLinkList(model);
            NormalizeQuerySpecificationList(model);
            NormalizeQuerySpecificationFromClauseLinkList(model);
            NormalizeQuerySpecificationGroupByClauseLinkList(model);
            NormalizeQuerySpecificationHavingClauseLinkList(model);
            NormalizeQuerySpecificationSelectElementsItemList(model);
            NormalizeQuerySpecificationTopRowFilterLinkList(model);
            NormalizeQuerySpecificationWhereClauseLinkList(model);
            NormalizeQuerySpecificationWindowClauseLinkList(model);
            NormalizeRealLiteralList(model);
            NormalizeRightFunctionCallList(model);
            NormalizeRightFunctionCallParametersItemList(model);
            NormalizeRollupGroupingSpecificationList(model);
            NormalizeRollupGroupingSpecificationArgumentsItemList(model);
            NormalizeRowValueList(model);
            NormalizeRowValueColumnValuesItemList(model);
            NormalizeScalarExpressionList(model);
            NormalizeScalarSubqueryList(model);
            NormalizeScalarSubqueryQueryExpressionLinkList(model);
            NormalizeSchemaObjectFunctionTableReferenceList(model);
            NormalizeSchemaObjectFunctionTableReferenceParametersItemList(model);
            NormalizeSchemaObjectFunctionTableReferenceSchemaObjectLinkList(model);
            NormalizeSchemaObjectNameList(model);
            NormalizeSchemaObjectNameBaseIdentifierLinkList(model);
            NormalizeSchemaObjectNameSchemaIdentifierLinkList(model);
            NormalizeSearchedCaseExpressionList(model);
            NormalizeSearchedCaseExpressionWhenClausesItemList(model);
            NormalizeSearchedWhenClauseList(model);
            NormalizeSearchedWhenClauseWhenExpressionLinkList(model);
            NormalizeSelectElementList(model);
            NormalizeSelectScalarExpressionList(model);
            NormalizeSelectScalarExpressionColumnNameLinkList(model);
            NormalizeSelectScalarExpressionExpressionLinkList(model);
            NormalizeSelectStarExpressionList(model);
            NormalizeSelectStarExpressionQualifierLinkList(model);
            NormalizeSelectStatementList(model);
            NormalizeSelectStatementQueryExpressionLinkList(model);
            NormalizeSimpleCaseExpressionList(model);
            NormalizeSimpleCaseExpressionInputExpressionLinkList(model);
            NormalizeSimpleCaseExpressionWhenClausesItemList(model);
            NormalizeSimpleWhenClauseList(model);
            NormalizeSimpleWhenClauseWhenExpressionLinkList(model);
            NormalizeSqlDataTypeReferenceList(model);
            NormalizeStatementWithCtesAndXmlNamespacesList(model);
            NormalizeStatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkList(model);
            NormalizeStringLiteralList(model);
            NormalizeSubqueryComparisonPredicateList(model);
            NormalizeSubqueryComparisonPredicateExpressionLinkList(model);
            NormalizeSubqueryComparisonPredicateSubqueryLinkList(model);
            NormalizeTableReferenceList(model);
            NormalizeTableReferenceWithAliasList(model);
            NormalizeTableReferenceWithAliasAliasLinkList(model);
            NormalizeTableReferenceWithAliasAndColumnsList(model);
            NormalizeTableReferenceWithAliasAndColumnsColumnsItemList(model);
            NormalizeTableSampleClauseList(model);
            NormalizeTableSampleClauseRepeatSeedLinkList(model);
            NormalizeTableSampleClauseSampleNumberLinkList(model);
            NormalizeTopRowFilterList(model);
            NormalizeTopRowFilterExpressionLinkList(model);
            NormalizeTransformScriptList(model);
            NormalizeTransformScriptObjectIdentifierLinkList(model);
            NormalizeTransformScriptSchemaIdentifierLinkList(model);
            NormalizeTransformScriptSelectStatementLinkList(model);
            NormalizeTransformScriptViewColumnsItemList(model);
            NormalizeTryCastCallList(model);
            NormalizeTryCastCallDataTypeLinkList(model);
            NormalizeTryCastCallParameterLinkList(model);
            NormalizeTryConvertCallList(model);
            NormalizeTryConvertCallDataTypeLinkList(model);
            NormalizeTryConvertCallParameterLinkList(model);
            NormalizeTryConvertCallStyleLinkList(model);
            NormalizeTryParseCallList(model);
            NormalizeTryParseCallCultureLinkList(model);
            NormalizeTryParseCallDataTypeLinkList(model);
            NormalizeTryParseCallStringValueLinkList(model);
            NormalizeTSqlStatementList(model);
            NormalizeUnaryExpressionList(model);
            NormalizeUnaryExpressionExpressionLinkList(model);
            NormalizeUnpivotedTableReferenceList(model);
            NormalizeUnpivotedTableReferenceInColumnsItemList(model);
            NormalizeUnpivotedTableReferencePivotColumnLinkList(model);
            NormalizeUnpivotedTableReferenceTableReferenceLinkList(model);
            NormalizeUnpivotedTableReferenceValueColumnLinkList(model);
            NormalizeUnqualifiedJoinList(model);
            NormalizeValueExpressionList(model);
            NormalizeWhenClauseList(model);
            NormalizeWhenClauseThenExpressionLinkList(model);
            NormalizeWhereClauseList(model);
            NormalizeWhereClauseSearchConditionLinkList(model);
            NormalizeWindowClauseList(model);
            NormalizeWindowClauseWindowDefinitionItemList(model);
            NormalizeWindowDefinitionList(model);
            NormalizeWindowDefinitionOrderByClauseLinkList(model);
            NormalizeWindowDefinitionPartitionsItemList(model);
            NormalizeWindowDefinitionRefWindowNameLinkList(model);
            NormalizeWindowDefinitionWindowFrameClauseLinkList(model);
            NormalizeWindowDefinitionWindowNameLinkList(model);
            NormalizeWindowDelimiterList(model);
            NormalizeWindowDelimiterOffsetValueLinkList(model);
            NormalizeWindowFrameClauseList(model);
            NormalizeWindowFrameClauseBottomLinkList(model);
            NormalizeWindowFrameClauseTopLinkList(model);
            NormalizeWithCtesAndXmlNamespacesList(model);
            NormalizeWithCtesAndXmlNamespacesCommonTableExpressionsItemList(model);
            NormalizeWithCtesAndXmlNamespacesXmlNamespacesLinkList(model);
            NormalizeXmlNamespacesList(model);
            NormalizeXmlNamespacesAliasElementList(model);
            NormalizeXmlNamespacesAliasElementIdentifierLinkList(model);
            NormalizeXmlNamespacesDefaultElementList(model);
            NormalizeXmlNamespacesElementList(model);
            NormalizeXmlNamespacesElementStringLinkList(model);
            NormalizeXmlNamespacesXmlNamespacesElementsItemList(model);

            var atTimeZoneCallListById = BuildById(model.AtTimeZoneCallList, row => row.Id, "AtTimeZoneCall");
            var atTimeZoneCallDateValueLinkListById = BuildById(model.AtTimeZoneCallDateValueLinkList, row => row.Id, "AtTimeZoneCallDateValueLink");
            var atTimeZoneCallTimeZoneLinkListById = BuildById(model.AtTimeZoneCallTimeZoneLinkList, row => row.Id, "AtTimeZoneCallTimeZoneLink");
            var binaryExpressionListById = BuildById(model.BinaryExpressionList, row => row.Id, "BinaryExpression");
            var binaryExpressionFirstExpressionLinkListById = BuildById(model.BinaryExpressionFirstExpressionLinkList, row => row.Id, "BinaryExpressionFirstExpressionLink");
            var binaryExpressionSecondExpressionLinkListById = BuildById(model.BinaryExpressionSecondExpressionLinkList, row => row.Id, "BinaryExpressionSecondExpressionLink");
            var binaryLiteralListById = BuildById(model.BinaryLiteralList, row => row.Id, "BinaryLiteral");
            var binaryQueryExpressionListById = BuildById(model.BinaryQueryExpressionList, row => row.Id, "BinaryQueryExpression");
            var binaryQueryExpressionFirstQueryExpressionLinkListById = BuildById(model.BinaryQueryExpressionFirstQueryExpressionLinkList, row => row.Id, "BinaryQueryExpressionFirstQueryExpressionLink");
            var binaryQueryExpressionSecondQueryExpressionLinkListById = BuildById(model.BinaryQueryExpressionSecondQueryExpressionLinkList, row => row.Id, "BinaryQueryExpressionSecondQueryExpressionLink");
            var booleanBinaryExpressionListById = BuildById(model.BooleanBinaryExpressionList, row => row.Id, "BooleanBinaryExpression");
            var booleanBinaryExpressionFirstExpressionLinkListById = BuildById(model.BooleanBinaryExpressionFirstExpressionLinkList, row => row.Id, "BooleanBinaryExpressionFirstExpressionLink");
            var booleanBinaryExpressionSecondExpressionLinkListById = BuildById(model.BooleanBinaryExpressionSecondExpressionLinkList, row => row.Id, "BooleanBinaryExpressionSecondExpressionLink");
            var booleanComparisonExpressionListById = BuildById(model.BooleanComparisonExpressionList, row => row.Id, "BooleanComparisonExpression");
            var booleanComparisonExpressionFirstExpressionLinkListById = BuildById(model.BooleanComparisonExpressionFirstExpressionLinkList, row => row.Id, "BooleanComparisonExpressionFirstExpressionLink");
            var booleanComparisonExpressionSecondExpressionLinkListById = BuildById(model.BooleanComparisonExpressionSecondExpressionLinkList, row => row.Id, "BooleanComparisonExpressionSecondExpressionLink");
            var booleanExpressionListById = BuildById(model.BooleanExpressionList, row => row.Id, "BooleanExpression");
            var booleanIsNullExpressionListById = BuildById(model.BooleanIsNullExpressionList, row => row.Id, "BooleanIsNullExpression");
            var booleanIsNullExpressionExpressionLinkListById = BuildById(model.BooleanIsNullExpressionExpressionLinkList, row => row.Id, "BooleanIsNullExpressionExpressionLink");
            var booleanNotExpressionListById = BuildById(model.BooleanNotExpressionList, row => row.Id, "BooleanNotExpression");
            var booleanNotExpressionExpressionLinkListById = BuildById(model.BooleanNotExpressionExpressionLinkList, row => row.Id, "BooleanNotExpressionExpressionLink");
            var booleanParenthesisExpressionListById = BuildById(model.BooleanParenthesisExpressionList, row => row.Id, "BooleanParenthesisExpression");
            var booleanParenthesisExpressionExpressionLinkListById = BuildById(model.BooleanParenthesisExpressionExpressionLinkList, row => row.Id, "BooleanParenthesisExpressionExpressionLink");
            var booleanTernaryExpressionListById = BuildById(model.BooleanTernaryExpressionList, row => row.Id, "BooleanTernaryExpression");
            var booleanTernaryExpressionFirstExpressionLinkListById = BuildById(model.BooleanTernaryExpressionFirstExpressionLinkList, row => row.Id, "BooleanTernaryExpressionFirstExpressionLink");
            var booleanTernaryExpressionSecondExpressionLinkListById = BuildById(model.BooleanTernaryExpressionSecondExpressionLinkList, row => row.Id, "BooleanTernaryExpressionSecondExpressionLink");
            var booleanTernaryExpressionThirdExpressionLinkListById = BuildById(model.BooleanTernaryExpressionThirdExpressionLinkList, row => row.Id, "BooleanTernaryExpressionThirdExpressionLink");
            var callTargetListById = BuildById(model.CallTargetList, row => row.Id, "CallTarget");
            var caseExpressionListById = BuildById(model.CaseExpressionList, row => row.Id, "CaseExpression");
            var caseExpressionElseExpressionLinkListById = BuildById(model.CaseExpressionElseExpressionLinkList, row => row.Id, "CaseExpressionElseExpressionLink");
            var castCallListById = BuildById(model.CastCallList, row => row.Id, "CastCall");
            var castCallDataTypeLinkListById = BuildById(model.CastCallDataTypeLinkList, row => row.Id, "CastCallDataTypeLink");
            var castCallParameterLinkListById = BuildById(model.CastCallParameterLinkList, row => row.Id, "CastCallParameterLink");
            var coalesceExpressionListById = BuildById(model.CoalesceExpressionList, row => row.Id, "CoalesceExpression");
            var coalesceExpressionExpressionsItemListById = BuildById(model.CoalesceExpressionExpressionsItemList, row => row.Id, "CoalesceExpressionExpressionsItem");
            var columnReferenceExpressionListById = BuildById(model.ColumnReferenceExpressionList, row => row.Id, "ColumnReferenceExpression");
            var columnReferenceExpressionMultiPartIdentifierLinkListById = BuildById(model.ColumnReferenceExpressionMultiPartIdentifierLinkList, row => row.Id, "ColumnReferenceExpressionMultiPartIdentifierLink");
            var commonTableExpressionListById = BuildById(model.CommonTableExpressionList, row => row.Id, "CommonTableExpression");
            var commonTableExpressionColumnsItemListById = BuildById(model.CommonTableExpressionColumnsItemList, row => row.Id, "CommonTableExpressionColumnsItem");
            var commonTableExpressionExpressionNameLinkListById = BuildById(model.CommonTableExpressionExpressionNameLinkList, row => row.Id, "CommonTableExpressionExpressionNameLink");
            var commonTableExpressionQueryExpressionLinkListById = BuildById(model.CommonTableExpressionQueryExpressionLinkList, row => row.Id, "CommonTableExpressionQueryExpressionLink");
            var compositeGroupingSpecificationListById = BuildById(model.CompositeGroupingSpecificationList, row => row.Id, "CompositeGroupingSpecification");
            var compositeGroupingSpecificationItemsItemListById = BuildById(model.CompositeGroupingSpecificationItemsItemList, row => row.Id, "CompositeGroupingSpecificationItemsItem");
            var convertCallListById = BuildById(model.ConvertCallList, row => row.Id, "ConvertCall");
            var convertCallDataTypeLinkListById = BuildById(model.ConvertCallDataTypeLinkList, row => row.Id, "ConvertCallDataTypeLink");
            var convertCallParameterLinkListById = BuildById(model.ConvertCallParameterLinkList, row => row.Id, "ConvertCallParameterLink");
            var convertCallStyleLinkListById = BuildById(model.ConvertCallStyleLinkList, row => row.Id, "ConvertCallStyleLink");
            var cubeGroupingSpecificationListById = BuildById(model.CubeGroupingSpecificationList, row => row.Id, "CubeGroupingSpecification");
            var cubeGroupingSpecificationArgumentsItemListById = BuildById(model.CubeGroupingSpecificationArgumentsItemList, row => row.Id, "CubeGroupingSpecificationArgumentsItem");
            var dataTypeReferenceListById = BuildById(model.DataTypeReferenceList, row => row.Id, "DataTypeReference");
            var dataTypeReferenceNameLinkListById = BuildById(model.DataTypeReferenceNameLinkList, row => row.Id, "DataTypeReferenceNameLink");
            var distinctPredicateListById = BuildById(model.DistinctPredicateList, row => row.Id, "DistinctPredicate");
            var distinctPredicateFirstExpressionLinkListById = BuildById(model.DistinctPredicateFirstExpressionLinkList, row => row.Id, "DistinctPredicateFirstExpressionLink");
            var distinctPredicateSecondExpressionLinkListById = BuildById(model.DistinctPredicateSecondExpressionLinkList, row => row.Id, "DistinctPredicateSecondExpressionLink");
            var existsPredicateListById = BuildById(model.ExistsPredicateList, row => row.Id, "ExistsPredicate");
            var existsPredicateSubqueryLinkListById = BuildById(model.ExistsPredicateSubqueryLinkList, row => row.Id, "ExistsPredicateSubqueryLink");
            var expressionGroupingSpecificationListById = BuildById(model.ExpressionGroupingSpecificationList, row => row.Id, "ExpressionGroupingSpecification");
            var expressionGroupingSpecificationExpressionLinkListById = BuildById(model.ExpressionGroupingSpecificationExpressionLinkList, row => row.Id, "ExpressionGroupingSpecificationExpressionLink");
            var expressionWithSortOrderListById = BuildById(model.ExpressionWithSortOrderList, row => row.Id, "ExpressionWithSortOrder");
            var expressionWithSortOrderExpressionLinkListById = BuildById(model.ExpressionWithSortOrderExpressionLinkList, row => row.Id, "ExpressionWithSortOrderExpressionLink");
            var fromClauseListById = BuildById(model.FromClauseList, row => row.Id, "FromClause");
            var fromClauseTableReferencesItemListById = BuildById(model.FromClauseTableReferencesItemList, row => row.Id, "FromClauseTableReferencesItem");
            var fullTextPredicateListById = BuildById(model.FullTextPredicateList, row => row.Id, "FullTextPredicate");
            var fullTextPredicateColumnsItemListById = BuildById(model.FullTextPredicateColumnsItemList, row => row.Id, "FullTextPredicateColumnsItem");
            var fullTextPredicateValueLinkListById = BuildById(model.FullTextPredicateValueLinkList, row => row.Id, "FullTextPredicateValueLink");
            var fullTextTableReferenceListById = BuildById(model.FullTextTableReferenceList, row => row.Id, "FullTextTableReference");
            var fullTextTableReferenceColumnsItemListById = BuildById(model.FullTextTableReferenceColumnsItemList, row => row.Id, "FullTextTableReferenceColumnsItem");
            var fullTextTableReferenceSearchConditionLinkListById = BuildById(model.FullTextTableReferenceSearchConditionLinkList, row => row.Id, "FullTextTableReferenceSearchConditionLink");
            var fullTextTableReferenceTableNameLinkListById = BuildById(model.FullTextTableReferenceTableNameLinkList, row => row.Id, "FullTextTableReferenceTableNameLink");
            var functionCallListById = BuildById(model.FunctionCallList, row => row.Id, "FunctionCall");
            var functionCallCallTargetLinkListById = BuildById(model.FunctionCallCallTargetLinkList, row => row.Id, "FunctionCallCallTargetLink");
            var functionCallFunctionNameLinkListById = BuildById(model.FunctionCallFunctionNameLinkList, row => row.Id, "FunctionCallFunctionNameLink");
            var functionCallOverClauseLinkListById = BuildById(model.FunctionCallOverClauseLinkList, row => row.Id, "FunctionCallOverClauseLink");
            var functionCallParametersItemListById = BuildById(model.FunctionCallParametersItemList, row => row.Id, "FunctionCallParametersItem");
            var globalFunctionTableReferenceListById = BuildById(model.GlobalFunctionTableReferenceList, row => row.Id, "GlobalFunctionTableReference");
            var globalFunctionTableReferenceNameLinkListById = BuildById(model.GlobalFunctionTableReferenceNameLinkList, row => row.Id, "GlobalFunctionTableReferenceNameLink");
            var globalFunctionTableReferenceParametersItemListById = BuildById(model.GlobalFunctionTableReferenceParametersItemList, row => row.Id, "GlobalFunctionTableReferenceParametersItem");
            var globalVariableExpressionListById = BuildById(model.GlobalVariableExpressionList, row => row.Id, "GlobalVariableExpression");
            var grandTotalGroupingSpecificationListById = BuildById(model.GrandTotalGroupingSpecificationList, row => row.Id, "GrandTotalGroupingSpecification");
            var groupByClauseListById = BuildById(model.GroupByClauseList, row => row.Id, "GroupByClause");
            var groupByClauseGroupingSpecificationsItemListById = BuildById(model.GroupByClauseGroupingSpecificationsItemList, row => row.Id, "GroupByClauseGroupingSpecificationsItem");
            var groupingSetsGroupingSpecificationListById = BuildById(model.GroupingSetsGroupingSpecificationList, row => row.Id, "GroupingSetsGroupingSpecification");
            var groupingSetsGroupingSpecificationSetsItemListById = BuildById(model.GroupingSetsGroupingSpecificationSetsItemList, row => row.Id, "GroupingSetsGroupingSpecificationSetsItem");
            var groupingSpecificationListById = BuildById(model.GroupingSpecificationList, row => row.Id, "GroupingSpecification");
            var havingClauseListById = BuildById(model.HavingClauseList, row => row.Id, "HavingClause");
            var havingClauseSearchConditionLinkListById = BuildById(model.HavingClauseSearchConditionLinkList, row => row.Id, "HavingClauseSearchConditionLink");
            var identifierListById = BuildById(model.IdentifierList, row => row.Id, "Identifier");
            var identifierOrValueExpressionListById = BuildById(model.IdentifierOrValueExpressionList, row => row.Id, "IdentifierOrValueExpression");
            var identifierOrValueExpressionIdentifierLinkListById = BuildById(model.IdentifierOrValueExpressionIdentifierLinkList, row => row.Id, "IdentifierOrValueExpressionIdentifierLink");
            var iIfCallListById = BuildById(model.IIfCallList, row => row.Id, "IIfCall");
            var iIfCallElseExpressionLinkListById = BuildById(model.IIfCallElseExpressionLinkList, row => row.Id, "IIfCallElseExpressionLink");
            var iIfCallPredicateLinkListById = BuildById(model.IIfCallPredicateLinkList, row => row.Id, "IIfCallPredicateLink");
            var iIfCallThenExpressionLinkListById = BuildById(model.IIfCallThenExpressionLinkList, row => row.Id, "IIfCallThenExpressionLink");
            var inlineDerivedTableListById = BuildById(model.InlineDerivedTableList, row => row.Id, "InlineDerivedTable");
            var inlineDerivedTableRowValuesItemListById = BuildById(model.InlineDerivedTableRowValuesItemList, row => row.Id, "InlineDerivedTableRowValuesItem");
            var inPredicateListById = BuildById(model.InPredicateList, row => row.Id, "InPredicate");
            var inPredicateExpressionLinkListById = BuildById(model.InPredicateExpressionLinkList, row => row.Id, "InPredicateExpressionLink");
            var inPredicateSubqueryLinkListById = BuildById(model.InPredicateSubqueryLinkList, row => row.Id, "InPredicateSubqueryLink");
            var inPredicateValuesItemListById = BuildById(model.InPredicateValuesItemList, row => row.Id, "InPredicateValuesItem");
            var integerLiteralListById = BuildById(model.IntegerLiteralList, row => row.Id, "IntegerLiteral");
            var joinParenthesisTableReferenceListById = BuildById(model.JoinParenthesisTableReferenceList, row => row.Id, "JoinParenthesisTableReference");
            var joinParenthesisTableReferenceJoinLinkListById = BuildById(model.JoinParenthesisTableReferenceJoinLinkList, row => row.Id, "JoinParenthesisTableReferenceJoinLink");
            var joinTableReferenceListById = BuildById(model.JoinTableReferenceList, row => row.Id, "JoinTableReference");
            var joinTableReferenceFirstTableReferenceLinkListById = BuildById(model.JoinTableReferenceFirstTableReferenceLinkList, row => row.Id, "JoinTableReferenceFirstTableReferenceLink");
            var joinTableReferenceSecondTableReferenceLinkListById = BuildById(model.JoinTableReferenceSecondTableReferenceLinkList, row => row.Id, "JoinTableReferenceSecondTableReferenceLink");
            var leftFunctionCallListById = BuildById(model.LeftFunctionCallList, row => row.Id, "LeftFunctionCall");
            var leftFunctionCallParametersItemListById = BuildById(model.LeftFunctionCallParametersItemList, row => row.Id, "LeftFunctionCallParametersItem");
            var likePredicateListById = BuildById(model.LikePredicateList, row => row.Id, "LikePredicate");
            var likePredicateFirstExpressionLinkListById = BuildById(model.LikePredicateFirstExpressionLinkList, row => row.Id, "LikePredicateFirstExpressionLink");
            var likePredicateSecondExpressionLinkListById = BuildById(model.LikePredicateSecondExpressionLinkList, row => row.Id, "LikePredicateSecondExpressionLink");
            var literalListById = BuildById(model.LiteralList, row => row.Id, "Literal");
            var maxLiteralListById = BuildById(model.MaxLiteralList, row => row.Id, "MaxLiteral");
            var multiPartIdentifierListById = BuildById(model.MultiPartIdentifierList, row => row.Id, "MultiPartIdentifier");
            var multiPartIdentifierCallTargetListById = BuildById(model.MultiPartIdentifierCallTargetList, row => row.Id, "MultiPartIdentifierCallTarget");
            var multiPartIdentifierCallTargetMultiPartIdentifierLinkListById = BuildById(model.MultiPartIdentifierCallTargetMultiPartIdentifierLinkList, row => row.Id, "MultiPartIdentifierCallTargetMultiPartIdentifierLink");
            var multiPartIdentifierIdentifiersItemListById = BuildById(model.MultiPartIdentifierIdentifiersItemList, row => row.Id, "MultiPartIdentifierIdentifiersItem");
            var namedTableReferenceListById = BuildById(model.NamedTableReferenceList, row => row.Id, "NamedTableReference");
            var namedTableReferenceSchemaObjectLinkListById = BuildById(model.NamedTableReferenceSchemaObjectLinkList, row => row.Id, "NamedTableReferenceSchemaObjectLink");
            var namedTableReferenceTableSampleClauseLinkListById = BuildById(model.NamedTableReferenceTableSampleClauseLinkList, row => row.Id, "NamedTableReferenceTableSampleClauseLink");
            var nextValueForExpressionListById = BuildById(model.NextValueForExpressionList, row => row.Id, "NextValueForExpression");
            var nextValueForExpressionSequenceNameLinkListById = BuildById(model.NextValueForExpressionSequenceNameLinkList, row => row.Id, "NextValueForExpressionSequenceNameLink");
            var nullIfExpressionListById = BuildById(model.NullIfExpressionList, row => row.Id, "NullIfExpression");
            var nullIfExpressionFirstExpressionLinkListById = BuildById(model.NullIfExpressionFirstExpressionLinkList, row => row.Id, "NullIfExpressionFirstExpressionLink");
            var nullIfExpressionSecondExpressionLinkListById = BuildById(model.NullIfExpressionSecondExpressionLinkList, row => row.Id, "NullIfExpressionSecondExpressionLink");
            var nullLiteralListById = BuildById(model.NullLiteralList, row => row.Id, "NullLiteral");
            var numericLiteralListById = BuildById(model.NumericLiteralList, row => row.Id, "NumericLiteral");
            var offsetClauseListById = BuildById(model.OffsetClauseList, row => row.Id, "OffsetClause");
            var offsetClauseFetchExpressionLinkListById = BuildById(model.OffsetClauseFetchExpressionLinkList, row => row.Id, "OffsetClauseFetchExpressionLink");
            var offsetClauseOffsetExpressionLinkListById = BuildById(model.OffsetClauseOffsetExpressionLinkList, row => row.Id, "OffsetClauseOffsetExpressionLink");
            var orderByClauseListById = BuildById(model.OrderByClauseList, row => row.Id, "OrderByClause");
            var orderByClauseOrderByElementsItemListById = BuildById(model.OrderByClauseOrderByElementsItemList, row => row.Id, "OrderByClauseOrderByElementsItem");
            var overClauseListById = BuildById(model.OverClauseList, row => row.Id, "OverClause");
            var overClauseOrderByClauseLinkListById = BuildById(model.OverClauseOrderByClauseLinkList, row => row.Id, "OverClauseOrderByClauseLink");
            var overClausePartitionsItemListById = BuildById(model.OverClausePartitionsItemList, row => row.Id, "OverClausePartitionsItem");
            var overClauseWindowFrameClauseLinkListById = BuildById(model.OverClauseWindowFrameClauseLinkList, row => row.Id, "OverClauseWindowFrameClauseLink");
            var overClauseWindowNameLinkListById = BuildById(model.OverClauseWindowNameLinkList, row => row.Id, "OverClauseWindowNameLink");
            var parameterizedDataTypeReferenceListById = BuildById(model.ParameterizedDataTypeReferenceList, row => row.Id, "ParameterizedDataTypeReference");
            var parameterizedDataTypeReferenceParametersItemListById = BuildById(model.ParameterizedDataTypeReferenceParametersItemList, row => row.Id, "ParameterizedDataTypeReferenceParametersItem");
            var parameterlessCallListById = BuildById(model.ParameterlessCallList, row => row.Id, "ParameterlessCall");
            var parenthesisExpressionListById = BuildById(model.ParenthesisExpressionList, row => row.Id, "ParenthesisExpression");
            var parenthesisExpressionExpressionLinkListById = BuildById(model.ParenthesisExpressionExpressionLinkList, row => row.Id, "ParenthesisExpressionExpressionLink");
            var parseCallListById = BuildById(model.ParseCallList, row => row.Id, "ParseCall");
            var parseCallCultureLinkListById = BuildById(model.ParseCallCultureLinkList, row => row.Id, "ParseCallCultureLink");
            var parseCallDataTypeLinkListById = BuildById(model.ParseCallDataTypeLinkList, row => row.Id, "ParseCallDataTypeLink");
            var parseCallStringValueLinkListById = BuildById(model.ParseCallStringValueLinkList, row => row.Id, "ParseCallStringValueLink");
            var pivotedTableReferenceListById = BuildById(model.PivotedTableReferenceList, row => row.Id, "PivotedTableReference");
            var pivotedTableReferenceAggregateFunctionIdentifierLinkListById = BuildById(model.PivotedTableReferenceAggregateFunctionIdentifierLinkList, row => row.Id, "PivotedTableReferenceAggregateFunctionIdentifierLink");
            var pivotedTableReferenceInColumnsItemListById = BuildById(model.PivotedTableReferenceInColumnsItemList, row => row.Id, "PivotedTableReferenceInColumnsItem");
            var pivotedTableReferencePivotColumnLinkListById = BuildById(model.PivotedTableReferencePivotColumnLinkList, row => row.Id, "PivotedTableReferencePivotColumnLink");
            var pivotedTableReferenceTableReferenceLinkListById = BuildById(model.PivotedTableReferenceTableReferenceLinkList, row => row.Id, "PivotedTableReferenceTableReferenceLink");
            var pivotedTableReferenceValueColumnsItemListById = BuildById(model.PivotedTableReferenceValueColumnsItemList, row => row.Id, "PivotedTableReferenceValueColumnsItem");
            var primaryExpressionListById = BuildById(model.PrimaryExpressionList, row => row.Id, "PrimaryExpression");
            var primaryExpressionCollationLinkListById = BuildById(model.PrimaryExpressionCollationLinkList, row => row.Id, "PrimaryExpressionCollationLink");
            var qualifiedJoinListById = BuildById(model.QualifiedJoinList, row => row.Id, "QualifiedJoin");
            var qualifiedJoinSearchConditionLinkListById = BuildById(model.QualifiedJoinSearchConditionLinkList, row => row.Id, "QualifiedJoinSearchConditionLink");
            var queryDerivedTableListById = BuildById(model.QueryDerivedTableList, row => row.Id, "QueryDerivedTable");
            var queryDerivedTableQueryExpressionLinkListById = BuildById(model.QueryDerivedTableQueryExpressionLinkList, row => row.Id, "QueryDerivedTableQueryExpressionLink");
            var queryExpressionListById = BuildById(model.QueryExpressionList, row => row.Id, "QueryExpression");
            var queryExpressionOffsetClauseLinkListById = BuildById(model.QueryExpressionOffsetClauseLinkList, row => row.Id, "QueryExpressionOffsetClauseLink");
            var queryExpressionOrderByClauseLinkListById = BuildById(model.QueryExpressionOrderByClauseLinkList, row => row.Id, "QueryExpressionOrderByClauseLink");
            var queryParenthesisExpressionListById = BuildById(model.QueryParenthesisExpressionList, row => row.Id, "QueryParenthesisExpression");
            var queryParenthesisExpressionQueryExpressionLinkListById = BuildById(model.QueryParenthesisExpressionQueryExpressionLinkList, row => row.Id, "QueryParenthesisExpressionQueryExpressionLink");
            var querySpecificationListById = BuildById(model.QuerySpecificationList, row => row.Id, "QuerySpecification");
            var querySpecificationFromClauseLinkListById = BuildById(model.QuerySpecificationFromClauseLinkList, row => row.Id, "QuerySpecificationFromClauseLink");
            var querySpecificationGroupByClauseLinkListById = BuildById(model.QuerySpecificationGroupByClauseLinkList, row => row.Id, "QuerySpecificationGroupByClauseLink");
            var querySpecificationHavingClauseLinkListById = BuildById(model.QuerySpecificationHavingClauseLinkList, row => row.Id, "QuerySpecificationHavingClauseLink");
            var querySpecificationSelectElementsItemListById = BuildById(model.QuerySpecificationSelectElementsItemList, row => row.Id, "QuerySpecificationSelectElementsItem");
            var querySpecificationTopRowFilterLinkListById = BuildById(model.QuerySpecificationTopRowFilterLinkList, row => row.Id, "QuerySpecificationTopRowFilterLink");
            var querySpecificationWhereClauseLinkListById = BuildById(model.QuerySpecificationWhereClauseLinkList, row => row.Id, "QuerySpecificationWhereClauseLink");
            var querySpecificationWindowClauseLinkListById = BuildById(model.QuerySpecificationWindowClauseLinkList, row => row.Id, "QuerySpecificationWindowClauseLink");
            var realLiteralListById = BuildById(model.RealLiteralList, row => row.Id, "RealLiteral");
            var rightFunctionCallListById = BuildById(model.RightFunctionCallList, row => row.Id, "RightFunctionCall");
            var rightFunctionCallParametersItemListById = BuildById(model.RightFunctionCallParametersItemList, row => row.Id, "RightFunctionCallParametersItem");
            var rollupGroupingSpecificationListById = BuildById(model.RollupGroupingSpecificationList, row => row.Id, "RollupGroupingSpecification");
            var rollupGroupingSpecificationArgumentsItemListById = BuildById(model.RollupGroupingSpecificationArgumentsItemList, row => row.Id, "RollupGroupingSpecificationArgumentsItem");
            var rowValueListById = BuildById(model.RowValueList, row => row.Id, "RowValue");
            var rowValueColumnValuesItemListById = BuildById(model.RowValueColumnValuesItemList, row => row.Id, "RowValueColumnValuesItem");
            var scalarExpressionListById = BuildById(model.ScalarExpressionList, row => row.Id, "ScalarExpression");
            var scalarSubqueryListById = BuildById(model.ScalarSubqueryList, row => row.Id, "ScalarSubquery");
            var scalarSubqueryQueryExpressionLinkListById = BuildById(model.ScalarSubqueryQueryExpressionLinkList, row => row.Id, "ScalarSubqueryQueryExpressionLink");
            var schemaObjectFunctionTableReferenceListById = BuildById(model.SchemaObjectFunctionTableReferenceList, row => row.Id, "SchemaObjectFunctionTableReference");
            var schemaObjectFunctionTableReferenceParametersItemListById = BuildById(model.SchemaObjectFunctionTableReferenceParametersItemList, row => row.Id, "SchemaObjectFunctionTableReferenceParametersItem");
            var schemaObjectFunctionTableReferenceSchemaObjectLinkListById = BuildById(model.SchemaObjectFunctionTableReferenceSchemaObjectLinkList, row => row.Id, "SchemaObjectFunctionTableReferenceSchemaObjectLink");
            var schemaObjectNameListById = BuildById(model.SchemaObjectNameList, row => row.Id, "SchemaObjectName");
            var schemaObjectNameBaseIdentifierLinkListById = BuildById(model.SchemaObjectNameBaseIdentifierLinkList, row => row.Id, "SchemaObjectNameBaseIdentifierLink");
            var schemaObjectNameSchemaIdentifierLinkListById = BuildById(model.SchemaObjectNameSchemaIdentifierLinkList, row => row.Id, "SchemaObjectNameSchemaIdentifierLink");
            var searchedCaseExpressionListById = BuildById(model.SearchedCaseExpressionList, row => row.Id, "SearchedCaseExpression");
            var searchedCaseExpressionWhenClausesItemListById = BuildById(model.SearchedCaseExpressionWhenClausesItemList, row => row.Id, "SearchedCaseExpressionWhenClausesItem");
            var searchedWhenClauseListById = BuildById(model.SearchedWhenClauseList, row => row.Id, "SearchedWhenClause");
            var searchedWhenClauseWhenExpressionLinkListById = BuildById(model.SearchedWhenClauseWhenExpressionLinkList, row => row.Id, "SearchedWhenClauseWhenExpressionLink");
            var selectElementListById = BuildById(model.SelectElementList, row => row.Id, "SelectElement");
            var selectScalarExpressionListById = BuildById(model.SelectScalarExpressionList, row => row.Id, "SelectScalarExpression");
            var selectScalarExpressionColumnNameLinkListById = BuildById(model.SelectScalarExpressionColumnNameLinkList, row => row.Id, "SelectScalarExpressionColumnNameLink");
            var selectScalarExpressionExpressionLinkListById = BuildById(model.SelectScalarExpressionExpressionLinkList, row => row.Id, "SelectScalarExpressionExpressionLink");
            var selectStarExpressionListById = BuildById(model.SelectStarExpressionList, row => row.Id, "SelectStarExpression");
            var selectStarExpressionQualifierLinkListById = BuildById(model.SelectStarExpressionQualifierLinkList, row => row.Id, "SelectStarExpressionQualifierLink");
            var selectStatementListById = BuildById(model.SelectStatementList, row => row.Id, "SelectStatement");
            var selectStatementQueryExpressionLinkListById = BuildById(model.SelectStatementQueryExpressionLinkList, row => row.Id, "SelectStatementQueryExpressionLink");
            var simpleCaseExpressionListById = BuildById(model.SimpleCaseExpressionList, row => row.Id, "SimpleCaseExpression");
            var simpleCaseExpressionInputExpressionLinkListById = BuildById(model.SimpleCaseExpressionInputExpressionLinkList, row => row.Id, "SimpleCaseExpressionInputExpressionLink");
            var simpleCaseExpressionWhenClausesItemListById = BuildById(model.SimpleCaseExpressionWhenClausesItemList, row => row.Id, "SimpleCaseExpressionWhenClausesItem");
            var simpleWhenClauseListById = BuildById(model.SimpleWhenClauseList, row => row.Id, "SimpleWhenClause");
            var simpleWhenClauseWhenExpressionLinkListById = BuildById(model.SimpleWhenClauseWhenExpressionLinkList, row => row.Id, "SimpleWhenClauseWhenExpressionLink");
            var sqlDataTypeReferenceListById = BuildById(model.SqlDataTypeReferenceList, row => row.Id, "SqlDataTypeReference");
            var statementWithCtesAndXmlNamespacesListById = BuildById(model.StatementWithCtesAndXmlNamespacesList, row => row.Id, "StatementWithCtesAndXmlNamespaces");
            var statementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkListById = BuildById(model.StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkList, row => row.Id, "StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLink");
            var stringLiteralListById = BuildById(model.StringLiteralList, row => row.Id, "StringLiteral");
            var subqueryComparisonPredicateListById = BuildById(model.SubqueryComparisonPredicateList, row => row.Id, "SubqueryComparisonPredicate");
            var subqueryComparisonPredicateExpressionLinkListById = BuildById(model.SubqueryComparisonPredicateExpressionLinkList, row => row.Id, "SubqueryComparisonPredicateExpressionLink");
            var subqueryComparisonPredicateSubqueryLinkListById = BuildById(model.SubqueryComparisonPredicateSubqueryLinkList, row => row.Id, "SubqueryComparisonPredicateSubqueryLink");
            var tableReferenceListById = BuildById(model.TableReferenceList, row => row.Id, "TableReference");
            var tableReferenceWithAliasListById = BuildById(model.TableReferenceWithAliasList, row => row.Id, "TableReferenceWithAlias");
            var tableReferenceWithAliasAliasLinkListById = BuildById(model.TableReferenceWithAliasAliasLinkList, row => row.Id, "TableReferenceWithAliasAliasLink");
            var tableReferenceWithAliasAndColumnsListById = BuildById(model.TableReferenceWithAliasAndColumnsList, row => row.Id, "TableReferenceWithAliasAndColumns");
            var tableReferenceWithAliasAndColumnsColumnsItemListById = BuildById(model.TableReferenceWithAliasAndColumnsColumnsItemList, row => row.Id, "TableReferenceWithAliasAndColumnsColumnsItem");
            var tableSampleClauseListById = BuildById(model.TableSampleClauseList, row => row.Id, "TableSampleClause");
            var tableSampleClauseRepeatSeedLinkListById = BuildById(model.TableSampleClauseRepeatSeedLinkList, row => row.Id, "TableSampleClauseRepeatSeedLink");
            var tableSampleClauseSampleNumberLinkListById = BuildById(model.TableSampleClauseSampleNumberLinkList, row => row.Id, "TableSampleClauseSampleNumberLink");
            var topRowFilterListById = BuildById(model.TopRowFilterList, row => row.Id, "TopRowFilter");
            var topRowFilterExpressionLinkListById = BuildById(model.TopRowFilterExpressionLinkList, row => row.Id, "TopRowFilterExpressionLink");
            var transformScriptListById = BuildById(model.TransformScriptList, row => row.Id, "TransformScript");
            var transformScriptObjectIdentifierLinkListById = BuildById(model.TransformScriptObjectIdentifierLinkList, row => row.Id, "TransformScriptObjectIdentifierLink");
            var transformScriptSchemaIdentifierLinkListById = BuildById(model.TransformScriptSchemaIdentifierLinkList, row => row.Id, "TransformScriptSchemaIdentifierLink");
            var transformScriptSelectStatementLinkListById = BuildById(model.TransformScriptSelectStatementLinkList, row => row.Id, "TransformScriptSelectStatementLink");
            var transformScriptViewColumnsItemListById = BuildById(model.TransformScriptViewColumnsItemList, row => row.Id, "TransformScriptViewColumnsItem");
            var tryCastCallListById = BuildById(model.TryCastCallList, row => row.Id, "TryCastCall");
            var tryCastCallDataTypeLinkListById = BuildById(model.TryCastCallDataTypeLinkList, row => row.Id, "TryCastCallDataTypeLink");
            var tryCastCallParameterLinkListById = BuildById(model.TryCastCallParameterLinkList, row => row.Id, "TryCastCallParameterLink");
            var tryConvertCallListById = BuildById(model.TryConvertCallList, row => row.Id, "TryConvertCall");
            var tryConvertCallDataTypeLinkListById = BuildById(model.TryConvertCallDataTypeLinkList, row => row.Id, "TryConvertCallDataTypeLink");
            var tryConvertCallParameterLinkListById = BuildById(model.TryConvertCallParameterLinkList, row => row.Id, "TryConvertCallParameterLink");
            var tryConvertCallStyleLinkListById = BuildById(model.TryConvertCallStyleLinkList, row => row.Id, "TryConvertCallStyleLink");
            var tryParseCallListById = BuildById(model.TryParseCallList, row => row.Id, "TryParseCall");
            var tryParseCallCultureLinkListById = BuildById(model.TryParseCallCultureLinkList, row => row.Id, "TryParseCallCultureLink");
            var tryParseCallDataTypeLinkListById = BuildById(model.TryParseCallDataTypeLinkList, row => row.Id, "TryParseCallDataTypeLink");
            var tryParseCallStringValueLinkListById = BuildById(model.TryParseCallStringValueLinkList, row => row.Id, "TryParseCallStringValueLink");
            var tSqlStatementListById = BuildById(model.TSqlStatementList, row => row.Id, "TSqlStatement");
            var unaryExpressionListById = BuildById(model.UnaryExpressionList, row => row.Id, "UnaryExpression");
            var unaryExpressionExpressionLinkListById = BuildById(model.UnaryExpressionExpressionLinkList, row => row.Id, "UnaryExpressionExpressionLink");
            var unpivotedTableReferenceListById = BuildById(model.UnpivotedTableReferenceList, row => row.Id, "UnpivotedTableReference");
            var unpivotedTableReferenceInColumnsItemListById = BuildById(model.UnpivotedTableReferenceInColumnsItemList, row => row.Id, "UnpivotedTableReferenceInColumnsItem");
            var unpivotedTableReferencePivotColumnLinkListById = BuildById(model.UnpivotedTableReferencePivotColumnLinkList, row => row.Id, "UnpivotedTableReferencePivotColumnLink");
            var unpivotedTableReferenceTableReferenceLinkListById = BuildById(model.UnpivotedTableReferenceTableReferenceLinkList, row => row.Id, "UnpivotedTableReferenceTableReferenceLink");
            var unpivotedTableReferenceValueColumnLinkListById = BuildById(model.UnpivotedTableReferenceValueColumnLinkList, row => row.Id, "UnpivotedTableReferenceValueColumnLink");
            var unqualifiedJoinListById = BuildById(model.UnqualifiedJoinList, row => row.Id, "UnqualifiedJoin");
            var valueExpressionListById = BuildById(model.ValueExpressionList, row => row.Id, "ValueExpression");
            var whenClauseListById = BuildById(model.WhenClauseList, row => row.Id, "WhenClause");
            var whenClauseThenExpressionLinkListById = BuildById(model.WhenClauseThenExpressionLinkList, row => row.Id, "WhenClauseThenExpressionLink");
            var whereClauseListById = BuildById(model.WhereClauseList, row => row.Id, "WhereClause");
            var whereClauseSearchConditionLinkListById = BuildById(model.WhereClauseSearchConditionLinkList, row => row.Id, "WhereClauseSearchConditionLink");
            var windowClauseListById = BuildById(model.WindowClauseList, row => row.Id, "WindowClause");
            var windowClauseWindowDefinitionItemListById = BuildById(model.WindowClauseWindowDefinitionItemList, row => row.Id, "WindowClauseWindowDefinitionItem");
            var windowDefinitionListById = BuildById(model.WindowDefinitionList, row => row.Id, "WindowDefinition");
            var windowDefinitionOrderByClauseLinkListById = BuildById(model.WindowDefinitionOrderByClauseLinkList, row => row.Id, "WindowDefinitionOrderByClauseLink");
            var windowDefinitionPartitionsItemListById = BuildById(model.WindowDefinitionPartitionsItemList, row => row.Id, "WindowDefinitionPartitionsItem");
            var windowDefinitionRefWindowNameLinkListById = BuildById(model.WindowDefinitionRefWindowNameLinkList, row => row.Id, "WindowDefinitionRefWindowNameLink");
            var windowDefinitionWindowFrameClauseLinkListById = BuildById(model.WindowDefinitionWindowFrameClauseLinkList, row => row.Id, "WindowDefinitionWindowFrameClauseLink");
            var windowDefinitionWindowNameLinkListById = BuildById(model.WindowDefinitionWindowNameLinkList, row => row.Id, "WindowDefinitionWindowNameLink");
            var windowDelimiterListById = BuildById(model.WindowDelimiterList, row => row.Id, "WindowDelimiter");
            var windowDelimiterOffsetValueLinkListById = BuildById(model.WindowDelimiterOffsetValueLinkList, row => row.Id, "WindowDelimiterOffsetValueLink");
            var windowFrameClauseListById = BuildById(model.WindowFrameClauseList, row => row.Id, "WindowFrameClause");
            var windowFrameClauseBottomLinkListById = BuildById(model.WindowFrameClauseBottomLinkList, row => row.Id, "WindowFrameClauseBottomLink");
            var windowFrameClauseTopLinkListById = BuildById(model.WindowFrameClauseTopLinkList, row => row.Id, "WindowFrameClauseTopLink");
            var withCtesAndXmlNamespacesListById = BuildById(model.WithCtesAndXmlNamespacesList, row => row.Id, "WithCtesAndXmlNamespaces");
            var withCtesAndXmlNamespacesCommonTableExpressionsItemListById = BuildById(model.WithCtesAndXmlNamespacesCommonTableExpressionsItemList, row => row.Id, "WithCtesAndXmlNamespacesCommonTableExpressionsItem");
            var withCtesAndXmlNamespacesXmlNamespacesLinkListById = BuildById(model.WithCtesAndXmlNamespacesXmlNamespacesLinkList, row => row.Id, "WithCtesAndXmlNamespacesXmlNamespacesLink");
            var xmlNamespacesListById = BuildById(model.XmlNamespacesList, row => row.Id, "XmlNamespaces");
            var xmlNamespacesAliasElementListById = BuildById(model.XmlNamespacesAliasElementList, row => row.Id, "XmlNamespacesAliasElement");
            var xmlNamespacesAliasElementIdentifierLinkListById = BuildById(model.XmlNamespacesAliasElementIdentifierLinkList, row => row.Id, "XmlNamespacesAliasElementIdentifierLink");
            var xmlNamespacesDefaultElementListById = BuildById(model.XmlNamespacesDefaultElementList, row => row.Id, "XmlNamespacesDefaultElement");
            var xmlNamespacesElementListById = BuildById(model.XmlNamespacesElementList, row => row.Id, "XmlNamespacesElement");
            var xmlNamespacesElementStringLinkListById = BuildById(model.XmlNamespacesElementStringLinkList, row => row.Id, "XmlNamespacesElementStringLink");
            var xmlNamespacesXmlNamespacesElementsItemListById = BuildById(model.XmlNamespacesXmlNamespacesElementsItemList, row => row.Id, "XmlNamespacesXmlNamespacesElementsItem");

            foreach (var row in model.AtTimeZoneCallList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "AtTimeZoneCall",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    primaryExpressionListById,
                    row.BaseId,
                    "AtTimeZoneCall",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.AtTimeZoneCallDateValueLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "AtTimeZoneCallDateValueLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    atTimeZoneCallListById,
                    row.OwnerId,
                    "AtTimeZoneCallDateValueLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.AtTimeZoneCallDateValueLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "AtTimeZoneCallDateValueLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "AtTimeZoneCallDateValueLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.AtTimeZoneCallTimeZoneLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "AtTimeZoneCallTimeZoneLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    atTimeZoneCallListById,
                    row.OwnerId,
                    "AtTimeZoneCallTimeZoneLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.AtTimeZoneCallTimeZoneLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "AtTimeZoneCallTimeZoneLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "AtTimeZoneCallTimeZoneLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.BinaryExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "BinaryExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    scalarExpressionListById,
                    row.BaseId,
                    "BinaryExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.BinaryExpressionFirstExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "BinaryExpressionFirstExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    binaryExpressionListById,
                    row.OwnerId,
                    "BinaryExpressionFirstExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.BinaryExpressionFirstExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "BinaryExpressionFirstExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "BinaryExpressionFirstExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.BinaryExpressionSecondExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "BinaryExpressionSecondExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    binaryExpressionListById,
                    row.OwnerId,
                    "BinaryExpressionSecondExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.BinaryExpressionSecondExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "BinaryExpressionSecondExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "BinaryExpressionSecondExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.BinaryLiteralList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "BinaryLiteral",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    literalListById,
                    row.BaseId,
                    "BinaryLiteral",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.BinaryQueryExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "BinaryQueryExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    queryExpressionListById,
                    row.BaseId,
                    "BinaryQueryExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.BinaryQueryExpressionFirstQueryExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "BinaryQueryExpressionFirstQueryExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    binaryQueryExpressionListById,
                    row.OwnerId,
                    "BinaryQueryExpressionFirstQueryExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.BinaryQueryExpressionFirstQueryExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "BinaryQueryExpressionFirstQueryExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    queryExpressionListById,
                    row.ValueId,
                    "BinaryQueryExpressionFirstQueryExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.BinaryQueryExpressionSecondQueryExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "BinaryQueryExpressionSecondQueryExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    binaryQueryExpressionListById,
                    row.OwnerId,
                    "BinaryQueryExpressionSecondQueryExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.BinaryQueryExpressionSecondQueryExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "BinaryQueryExpressionSecondQueryExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    queryExpressionListById,
                    row.ValueId,
                    "BinaryQueryExpressionSecondQueryExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.BooleanBinaryExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "BooleanBinaryExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    booleanExpressionListById,
                    row.BaseId,
                    "BooleanBinaryExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.BooleanBinaryExpressionFirstExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "BooleanBinaryExpressionFirstExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    booleanBinaryExpressionListById,
                    row.OwnerId,
                    "BooleanBinaryExpressionFirstExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.BooleanBinaryExpressionFirstExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "BooleanBinaryExpressionFirstExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    booleanExpressionListById,
                    row.ValueId,
                    "BooleanBinaryExpressionFirstExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.BooleanBinaryExpressionSecondExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "BooleanBinaryExpressionSecondExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    booleanBinaryExpressionListById,
                    row.OwnerId,
                    "BooleanBinaryExpressionSecondExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.BooleanBinaryExpressionSecondExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "BooleanBinaryExpressionSecondExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    booleanExpressionListById,
                    row.ValueId,
                    "BooleanBinaryExpressionSecondExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.BooleanComparisonExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "BooleanComparisonExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    booleanExpressionListById,
                    row.BaseId,
                    "BooleanComparisonExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.BooleanComparisonExpressionFirstExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "BooleanComparisonExpressionFirstExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    booleanComparisonExpressionListById,
                    row.OwnerId,
                    "BooleanComparisonExpressionFirstExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.BooleanComparisonExpressionFirstExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "BooleanComparisonExpressionFirstExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "BooleanComparisonExpressionFirstExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.BooleanComparisonExpressionSecondExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "BooleanComparisonExpressionSecondExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    booleanComparisonExpressionListById,
                    row.OwnerId,
                    "BooleanComparisonExpressionSecondExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.BooleanComparisonExpressionSecondExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "BooleanComparisonExpressionSecondExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "BooleanComparisonExpressionSecondExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.BooleanIsNullExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "BooleanIsNullExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    booleanExpressionListById,
                    row.BaseId,
                    "BooleanIsNullExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.BooleanIsNullExpressionExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "BooleanIsNullExpressionExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    booleanIsNullExpressionListById,
                    row.OwnerId,
                    "BooleanIsNullExpressionExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.BooleanIsNullExpressionExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "BooleanIsNullExpressionExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "BooleanIsNullExpressionExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.BooleanNotExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "BooleanNotExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    booleanExpressionListById,
                    row.BaseId,
                    "BooleanNotExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.BooleanNotExpressionExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "BooleanNotExpressionExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    booleanNotExpressionListById,
                    row.OwnerId,
                    "BooleanNotExpressionExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.BooleanNotExpressionExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "BooleanNotExpressionExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    booleanExpressionListById,
                    row.ValueId,
                    "BooleanNotExpressionExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.BooleanParenthesisExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "BooleanParenthesisExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    booleanExpressionListById,
                    row.BaseId,
                    "BooleanParenthesisExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.BooleanParenthesisExpressionExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "BooleanParenthesisExpressionExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    booleanParenthesisExpressionListById,
                    row.OwnerId,
                    "BooleanParenthesisExpressionExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.BooleanParenthesisExpressionExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "BooleanParenthesisExpressionExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    booleanExpressionListById,
                    row.ValueId,
                    "BooleanParenthesisExpressionExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.BooleanTernaryExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "BooleanTernaryExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    booleanExpressionListById,
                    row.BaseId,
                    "BooleanTernaryExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.BooleanTernaryExpressionFirstExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "BooleanTernaryExpressionFirstExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    booleanTernaryExpressionListById,
                    row.OwnerId,
                    "BooleanTernaryExpressionFirstExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.BooleanTernaryExpressionFirstExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "BooleanTernaryExpressionFirstExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "BooleanTernaryExpressionFirstExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.BooleanTernaryExpressionSecondExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "BooleanTernaryExpressionSecondExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    booleanTernaryExpressionListById,
                    row.OwnerId,
                    "BooleanTernaryExpressionSecondExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.BooleanTernaryExpressionSecondExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "BooleanTernaryExpressionSecondExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "BooleanTernaryExpressionSecondExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.BooleanTernaryExpressionThirdExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "BooleanTernaryExpressionThirdExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    booleanTernaryExpressionListById,
                    row.OwnerId,
                    "BooleanTernaryExpressionThirdExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.BooleanTernaryExpressionThirdExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "BooleanTernaryExpressionThirdExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "BooleanTernaryExpressionThirdExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.CaseExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "CaseExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    primaryExpressionListById,
                    row.BaseId,
                    "CaseExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.CaseExpressionElseExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "CaseExpressionElseExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    caseExpressionListById,
                    row.OwnerId,
                    "CaseExpressionElseExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.CaseExpressionElseExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "CaseExpressionElseExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "CaseExpressionElseExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.CastCallList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "CastCall",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    primaryExpressionListById,
                    row.BaseId,
                    "CastCall",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.CastCallDataTypeLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "CastCallDataTypeLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    castCallListById,
                    row.OwnerId,
                    "CastCallDataTypeLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.CastCallDataTypeLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "CastCallDataTypeLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    dataTypeReferenceListById,
                    row.ValueId,
                    "CastCallDataTypeLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.CastCallParameterLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "CastCallParameterLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    castCallListById,
                    row.OwnerId,
                    "CastCallParameterLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.CastCallParameterLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "CastCallParameterLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "CastCallParameterLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.CoalesceExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "CoalesceExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    primaryExpressionListById,
                    row.BaseId,
                    "CoalesceExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.CoalesceExpressionExpressionsItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "CoalesceExpressionExpressionsItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    coalesceExpressionListById,
                    row.OwnerId,
                    "CoalesceExpressionExpressionsItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.CoalesceExpressionExpressionsItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "CoalesceExpressionExpressionsItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "CoalesceExpressionExpressionsItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.ColumnReferenceExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "ColumnReferenceExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    primaryExpressionListById,
                    row.BaseId,
                    "ColumnReferenceExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.ColumnReferenceExpressionMultiPartIdentifierLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "ColumnReferenceExpressionMultiPartIdentifierLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    columnReferenceExpressionListById,
                    row.OwnerId,
                    "ColumnReferenceExpressionMultiPartIdentifierLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.ColumnReferenceExpressionMultiPartIdentifierLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "ColumnReferenceExpressionMultiPartIdentifierLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    multiPartIdentifierListById,
                    row.ValueId,
                    "ColumnReferenceExpressionMultiPartIdentifierLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.CommonTableExpressionColumnsItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "CommonTableExpressionColumnsItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    commonTableExpressionListById,
                    row.OwnerId,
                    "CommonTableExpressionColumnsItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.CommonTableExpressionColumnsItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "CommonTableExpressionColumnsItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    identifierListById,
                    row.ValueId,
                    "CommonTableExpressionColumnsItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.CommonTableExpressionExpressionNameLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "CommonTableExpressionExpressionNameLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    commonTableExpressionListById,
                    row.OwnerId,
                    "CommonTableExpressionExpressionNameLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.CommonTableExpressionExpressionNameLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "CommonTableExpressionExpressionNameLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    identifierListById,
                    row.ValueId,
                    "CommonTableExpressionExpressionNameLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.CommonTableExpressionQueryExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "CommonTableExpressionQueryExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    commonTableExpressionListById,
                    row.OwnerId,
                    "CommonTableExpressionQueryExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.CommonTableExpressionQueryExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "CommonTableExpressionQueryExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    queryExpressionListById,
                    row.ValueId,
                    "CommonTableExpressionQueryExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.CompositeGroupingSpecificationList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "CompositeGroupingSpecification",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    groupingSpecificationListById,
                    row.BaseId,
                    "CompositeGroupingSpecification",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.CompositeGroupingSpecificationItemsItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "CompositeGroupingSpecificationItemsItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    compositeGroupingSpecificationListById,
                    row.OwnerId,
                    "CompositeGroupingSpecificationItemsItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.CompositeGroupingSpecificationItemsItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "CompositeGroupingSpecificationItemsItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    groupingSpecificationListById,
                    row.ValueId,
                    "CompositeGroupingSpecificationItemsItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.ConvertCallList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "ConvertCall",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    primaryExpressionListById,
                    row.BaseId,
                    "ConvertCall",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.ConvertCallDataTypeLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "ConvertCallDataTypeLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    convertCallListById,
                    row.OwnerId,
                    "ConvertCallDataTypeLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.ConvertCallDataTypeLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "ConvertCallDataTypeLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    dataTypeReferenceListById,
                    row.ValueId,
                    "ConvertCallDataTypeLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.ConvertCallParameterLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "ConvertCallParameterLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    convertCallListById,
                    row.OwnerId,
                    "ConvertCallParameterLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.ConvertCallParameterLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "ConvertCallParameterLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "ConvertCallParameterLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.ConvertCallStyleLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "ConvertCallStyleLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    convertCallListById,
                    row.OwnerId,
                    "ConvertCallStyleLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.ConvertCallStyleLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "ConvertCallStyleLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "ConvertCallStyleLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.CubeGroupingSpecificationList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "CubeGroupingSpecification",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    groupingSpecificationListById,
                    row.BaseId,
                    "CubeGroupingSpecification",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.CubeGroupingSpecificationArgumentsItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "CubeGroupingSpecificationArgumentsItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    cubeGroupingSpecificationListById,
                    row.OwnerId,
                    "CubeGroupingSpecificationArgumentsItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.CubeGroupingSpecificationArgumentsItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "CubeGroupingSpecificationArgumentsItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    groupingSpecificationListById,
                    row.ValueId,
                    "CubeGroupingSpecificationArgumentsItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.DataTypeReferenceNameLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "DataTypeReferenceNameLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    dataTypeReferenceListById,
                    row.OwnerId,
                    "DataTypeReferenceNameLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.DataTypeReferenceNameLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "DataTypeReferenceNameLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    schemaObjectNameListById,
                    row.ValueId,
                    "DataTypeReferenceNameLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.DistinctPredicateList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "DistinctPredicate",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    booleanExpressionListById,
                    row.BaseId,
                    "DistinctPredicate",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.DistinctPredicateFirstExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "DistinctPredicateFirstExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    distinctPredicateListById,
                    row.OwnerId,
                    "DistinctPredicateFirstExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.DistinctPredicateFirstExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "DistinctPredicateFirstExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "DistinctPredicateFirstExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.DistinctPredicateSecondExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "DistinctPredicateSecondExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    distinctPredicateListById,
                    row.OwnerId,
                    "DistinctPredicateSecondExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.DistinctPredicateSecondExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "DistinctPredicateSecondExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "DistinctPredicateSecondExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.ExistsPredicateList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "ExistsPredicate",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    booleanExpressionListById,
                    row.BaseId,
                    "ExistsPredicate",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.ExistsPredicateSubqueryLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "ExistsPredicateSubqueryLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    existsPredicateListById,
                    row.OwnerId,
                    "ExistsPredicateSubqueryLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.ExistsPredicateSubqueryLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "ExistsPredicateSubqueryLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarSubqueryListById,
                    row.ValueId,
                    "ExistsPredicateSubqueryLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.ExpressionGroupingSpecificationList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "ExpressionGroupingSpecification",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    groupingSpecificationListById,
                    row.BaseId,
                    "ExpressionGroupingSpecification",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.ExpressionGroupingSpecificationExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "ExpressionGroupingSpecificationExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    expressionGroupingSpecificationListById,
                    row.OwnerId,
                    "ExpressionGroupingSpecificationExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.ExpressionGroupingSpecificationExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "ExpressionGroupingSpecificationExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "ExpressionGroupingSpecificationExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.ExpressionWithSortOrderExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "ExpressionWithSortOrderExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    expressionWithSortOrderListById,
                    row.OwnerId,
                    "ExpressionWithSortOrderExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.ExpressionWithSortOrderExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "ExpressionWithSortOrderExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "ExpressionWithSortOrderExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.FromClauseTableReferencesItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "FromClauseTableReferencesItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    fromClauseListById,
                    row.OwnerId,
                    "FromClauseTableReferencesItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.FromClauseTableReferencesItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "FromClauseTableReferencesItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    tableReferenceListById,
                    row.ValueId,
                    "FromClauseTableReferencesItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.FullTextPredicateList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "FullTextPredicate",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    booleanExpressionListById,
                    row.BaseId,
                    "FullTextPredicate",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.FullTextPredicateColumnsItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "FullTextPredicateColumnsItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    fullTextPredicateListById,
                    row.OwnerId,
                    "FullTextPredicateColumnsItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.FullTextPredicateColumnsItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "FullTextPredicateColumnsItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    columnReferenceExpressionListById,
                    row.ValueId,
                    "FullTextPredicateColumnsItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.FullTextPredicateValueLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "FullTextPredicateValueLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    fullTextPredicateListById,
                    row.OwnerId,
                    "FullTextPredicateValueLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.FullTextPredicateValueLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "FullTextPredicateValueLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    valueExpressionListById,
                    row.ValueId,
                    "FullTextPredicateValueLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.FullTextTableReferenceList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "FullTextTableReference",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    tableReferenceWithAliasListById,
                    row.BaseId,
                    "FullTextTableReference",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.FullTextTableReferenceColumnsItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "FullTextTableReferenceColumnsItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    fullTextTableReferenceListById,
                    row.OwnerId,
                    "FullTextTableReferenceColumnsItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.FullTextTableReferenceColumnsItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "FullTextTableReferenceColumnsItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    columnReferenceExpressionListById,
                    row.ValueId,
                    "FullTextTableReferenceColumnsItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.FullTextTableReferenceSearchConditionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "FullTextTableReferenceSearchConditionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    fullTextTableReferenceListById,
                    row.OwnerId,
                    "FullTextTableReferenceSearchConditionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.FullTextTableReferenceSearchConditionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "FullTextTableReferenceSearchConditionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    valueExpressionListById,
                    row.ValueId,
                    "FullTextTableReferenceSearchConditionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.FullTextTableReferenceTableNameLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "FullTextTableReferenceTableNameLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    fullTextTableReferenceListById,
                    row.OwnerId,
                    "FullTextTableReferenceTableNameLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.FullTextTableReferenceTableNameLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "FullTextTableReferenceTableNameLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    schemaObjectNameListById,
                    row.ValueId,
                    "FullTextTableReferenceTableNameLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.FunctionCallList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "FunctionCall",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    primaryExpressionListById,
                    row.BaseId,
                    "FunctionCall",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.FunctionCallCallTargetLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "FunctionCallCallTargetLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    functionCallListById,
                    row.OwnerId,
                    "FunctionCallCallTargetLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.FunctionCallCallTargetLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "FunctionCallCallTargetLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    callTargetListById,
                    row.ValueId,
                    "FunctionCallCallTargetLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.FunctionCallFunctionNameLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "FunctionCallFunctionNameLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    functionCallListById,
                    row.OwnerId,
                    "FunctionCallFunctionNameLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.FunctionCallFunctionNameLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "FunctionCallFunctionNameLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    identifierListById,
                    row.ValueId,
                    "FunctionCallFunctionNameLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.FunctionCallOverClauseLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "FunctionCallOverClauseLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    functionCallListById,
                    row.OwnerId,
                    "FunctionCallOverClauseLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.FunctionCallOverClauseLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "FunctionCallOverClauseLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    overClauseListById,
                    row.ValueId,
                    "FunctionCallOverClauseLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.FunctionCallParametersItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "FunctionCallParametersItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    functionCallListById,
                    row.OwnerId,
                    "FunctionCallParametersItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.FunctionCallParametersItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "FunctionCallParametersItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "FunctionCallParametersItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.GlobalFunctionTableReferenceList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "GlobalFunctionTableReference",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    tableReferenceWithAliasListById,
                    row.BaseId,
                    "GlobalFunctionTableReference",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.GlobalFunctionTableReferenceNameLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "GlobalFunctionTableReferenceNameLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    globalFunctionTableReferenceListById,
                    row.OwnerId,
                    "GlobalFunctionTableReferenceNameLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.GlobalFunctionTableReferenceNameLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "GlobalFunctionTableReferenceNameLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    identifierListById,
                    row.ValueId,
                    "GlobalFunctionTableReferenceNameLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.GlobalFunctionTableReferenceParametersItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "GlobalFunctionTableReferenceParametersItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    globalFunctionTableReferenceListById,
                    row.OwnerId,
                    "GlobalFunctionTableReferenceParametersItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.GlobalFunctionTableReferenceParametersItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "GlobalFunctionTableReferenceParametersItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "GlobalFunctionTableReferenceParametersItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.GlobalVariableExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "GlobalVariableExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    valueExpressionListById,
                    row.BaseId,
                    "GlobalVariableExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.GrandTotalGroupingSpecificationList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "GrandTotalGroupingSpecification",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    groupingSpecificationListById,
                    row.BaseId,
                    "GrandTotalGroupingSpecification",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.GroupByClauseGroupingSpecificationsItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "GroupByClauseGroupingSpecificationsItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    groupByClauseListById,
                    row.OwnerId,
                    "GroupByClauseGroupingSpecificationsItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.GroupByClauseGroupingSpecificationsItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "GroupByClauseGroupingSpecificationsItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    groupingSpecificationListById,
                    row.ValueId,
                    "GroupByClauseGroupingSpecificationsItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.GroupingSetsGroupingSpecificationList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "GroupingSetsGroupingSpecification",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    groupingSpecificationListById,
                    row.BaseId,
                    "GroupingSetsGroupingSpecification",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.GroupingSetsGroupingSpecificationSetsItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "GroupingSetsGroupingSpecificationSetsItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    groupingSetsGroupingSpecificationListById,
                    row.OwnerId,
                    "GroupingSetsGroupingSpecificationSetsItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.GroupingSetsGroupingSpecificationSetsItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "GroupingSetsGroupingSpecificationSetsItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    groupingSpecificationListById,
                    row.ValueId,
                    "GroupingSetsGroupingSpecificationSetsItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.HavingClauseSearchConditionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "HavingClauseSearchConditionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    havingClauseListById,
                    row.OwnerId,
                    "HavingClauseSearchConditionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.HavingClauseSearchConditionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "HavingClauseSearchConditionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    booleanExpressionListById,
                    row.ValueId,
                    "HavingClauseSearchConditionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.IdentifierOrValueExpressionIdentifierLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "IdentifierOrValueExpressionIdentifierLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    identifierOrValueExpressionListById,
                    row.OwnerId,
                    "IdentifierOrValueExpressionIdentifierLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.IdentifierOrValueExpressionIdentifierLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "IdentifierOrValueExpressionIdentifierLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    identifierListById,
                    row.ValueId,
                    "IdentifierOrValueExpressionIdentifierLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.IIfCallList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "IIfCall",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    primaryExpressionListById,
                    row.BaseId,
                    "IIfCall",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.IIfCallElseExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "IIfCallElseExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    iIfCallListById,
                    row.OwnerId,
                    "IIfCallElseExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.IIfCallElseExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "IIfCallElseExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "IIfCallElseExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.IIfCallPredicateLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "IIfCallPredicateLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    iIfCallListById,
                    row.OwnerId,
                    "IIfCallPredicateLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.IIfCallPredicateLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "IIfCallPredicateLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    booleanExpressionListById,
                    row.ValueId,
                    "IIfCallPredicateLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.IIfCallThenExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "IIfCallThenExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    iIfCallListById,
                    row.OwnerId,
                    "IIfCallThenExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.IIfCallThenExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "IIfCallThenExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "IIfCallThenExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.InlineDerivedTableList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "InlineDerivedTable",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    tableReferenceWithAliasAndColumnsListById,
                    row.BaseId,
                    "InlineDerivedTable",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.InlineDerivedTableRowValuesItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "InlineDerivedTableRowValuesItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    inlineDerivedTableListById,
                    row.OwnerId,
                    "InlineDerivedTableRowValuesItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.InlineDerivedTableRowValuesItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "InlineDerivedTableRowValuesItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    rowValueListById,
                    row.ValueId,
                    "InlineDerivedTableRowValuesItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.InPredicateList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "InPredicate",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    booleanExpressionListById,
                    row.BaseId,
                    "InPredicate",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.InPredicateExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "InPredicateExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    inPredicateListById,
                    row.OwnerId,
                    "InPredicateExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.InPredicateExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "InPredicateExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "InPredicateExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.InPredicateSubqueryLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "InPredicateSubqueryLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    inPredicateListById,
                    row.OwnerId,
                    "InPredicateSubqueryLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.InPredicateSubqueryLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "InPredicateSubqueryLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarSubqueryListById,
                    row.ValueId,
                    "InPredicateSubqueryLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.InPredicateValuesItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "InPredicateValuesItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    inPredicateListById,
                    row.OwnerId,
                    "InPredicateValuesItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.InPredicateValuesItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "InPredicateValuesItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "InPredicateValuesItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.IntegerLiteralList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "IntegerLiteral",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    literalListById,
                    row.BaseId,
                    "IntegerLiteral",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.JoinParenthesisTableReferenceList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "JoinParenthesisTableReference",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    tableReferenceListById,
                    row.BaseId,
                    "JoinParenthesisTableReference",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.JoinParenthesisTableReferenceJoinLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "JoinParenthesisTableReferenceJoinLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    joinParenthesisTableReferenceListById,
                    row.OwnerId,
                    "JoinParenthesisTableReferenceJoinLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.JoinParenthesisTableReferenceJoinLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "JoinParenthesisTableReferenceJoinLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    tableReferenceListById,
                    row.ValueId,
                    "JoinParenthesisTableReferenceJoinLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.JoinTableReferenceList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "JoinTableReference",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    tableReferenceListById,
                    row.BaseId,
                    "JoinTableReference",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.JoinTableReferenceFirstTableReferenceLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "JoinTableReferenceFirstTableReferenceLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    joinTableReferenceListById,
                    row.OwnerId,
                    "JoinTableReferenceFirstTableReferenceLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.JoinTableReferenceFirstTableReferenceLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "JoinTableReferenceFirstTableReferenceLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    tableReferenceListById,
                    row.ValueId,
                    "JoinTableReferenceFirstTableReferenceLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.JoinTableReferenceSecondTableReferenceLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "JoinTableReferenceSecondTableReferenceLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    joinTableReferenceListById,
                    row.OwnerId,
                    "JoinTableReferenceSecondTableReferenceLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.JoinTableReferenceSecondTableReferenceLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "JoinTableReferenceSecondTableReferenceLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    tableReferenceListById,
                    row.ValueId,
                    "JoinTableReferenceSecondTableReferenceLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.LeftFunctionCallList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "LeftFunctionCall",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    primaryExpressionListById,
                    row.BaseId,
                    "LeftFunctionCall",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.LeftFunctionCallParametersItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "LeftFunctionCallParametersItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    leftFunctionCallListById,
                    row.OwnerId,
                    "LeftFunctionCallParametersItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.LeftFunctionCallParametersItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "LeftFunctionCallParametersItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "LeftFunctionCallParametersItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.LikePredicateList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "LikePredicate",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    booleanExpressionListById,
                    row.BaseId,
                    "LikePredicate",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.LikePredicateFirstExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "LikePredicateFirstExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    likePredicateListById,
                    row.OwnerId,
                    "LikePredicateFirstExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.LikePredicateFirstExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "LikePredicateFirstExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "LikePredicateFirstExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.LikePredicateSecondExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "LikePredicateSecondExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    likePredicateListById,
                    row.OwnerId,
                    "LikePredicateSecondExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.LikePredicateSecondExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "LikePredicateSecondExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "LikePredicateSecondExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.LiteralList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "Literal",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    valueExpressionListById,
                    row.BaseId,
                    "Literal",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.MaxLiteralList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "MaxLiteral",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    literalListById,
                    row.BaseId,
                    "MaxLiteral",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.MultiPartIdentifierCallTargetList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "MultiPartIdentifierCallTarget",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    callTargetListById,
                    row.BaseId,
                    "MultiPartIdentifierCallTarget",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.MultiPartIdentifierCallTargetMultiPartIdentifierLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "MultiPartIdentifierCallTargetMultiPartIdentifierLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    multiPartIdentifierCallTargetListById,
                    row.OwnerId,
                    "MultiPartIdentifierCallTargetMultiPartIdentifierLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.MultiPartIdentifierCallTargetMultiPartIdentifierLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "MultiPartIdentifierCallTargetMultiPartIdentifierLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    multiPartIdentifierListById,
                    row.ValueId,
                    "MultiPartIdentifierCallTargetMultiPartIdentifierLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.MultiPartIdentifierIdentifiersItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "MultiPartIdentifierIdentifiersItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    multiPartIdentifierListById,
                    row.OwnerId,
                    "MultiPartIdentifierIdentifiersItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.MultiPartIdentifierIdentifiersItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "MultiPartIdentifierIdentifiersItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    identifierListById,
                    row.ValueId,
                    "MultiPartIdentifierIdentifiersItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.NamedTableReferenceList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "NamedTableReference",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    tableReferenceWithAliasListById,
                    row.BaseId,
                    "NamedTableReference",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.NamedTableReferenceSchemaObjectLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "NamedTableReferenceSchemaObjectLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    namedTableReferenceListById,
                    row.OwnerId,
                    "NamedTableReferenceSchemaObjectLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.NamedTableReferenceSchemaObjectLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "NamedTableReferenceSchemaObjectLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    schemaObjectNameListById,
                    row.ValueId,
                    "NamedTableReferenceSchemaObjectLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.NamedTableReferenceTableSampleClauseLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "NamedTableReferenceTableSampleClauseLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    namedTableReferenceListById,
                    row.OwnerId,
                    "NamedTableReferenceTableSampleClauseLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.NamedTableReferenceTableSampleClauseLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "NamedTableReferenceTableSampleClauseLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    tableSampleClauseListById,
                    row.ValueId,
                    "NamedTableReferenceTableSampleClauseLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.NextValueForExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "NextValueForExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    primaryExpressionListById,
                    row.BaseId,
                    "NextValueForExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.NextValueForExpressionSequenceNameLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "NextValueForExpressionSequenceNameLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    nextValueForExpressionListById,
                    row.OwnerId,
                    "NextValueForExpressionSequenceNameLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.NextValueForExpressionSequenceNameLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "NextValueForExpressionSequenceNameLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    schemaObjectNameListById,
                    row.ValueId,
                    "NextValueForExpressionSequenceNameLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.NullIfExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "NullIfExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    primaryExpressionListById,
                    row.BaseId,
                    "NullIfExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.NullIfExpressionFirstExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "NullIfExpressionFirstExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    nullIfExpressionListById,
                    row.OwnerId,
                    "NullIfExpressionFirstExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.NullIfExpressionFirstExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "NullIfExpressionFirstExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "NullIfExpressionFirstExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.NullIfExpressionSecondExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "NullIfExpressionSecondExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    nullIfExpressionListById,
                    row.OwnerId,
                    "NullIfExpressionSecondExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.NullIfExpressionSecondExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "NullIfExpressionSecondExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "NullIfExpressionSecondExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.NullLiteralList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "NullLiteral",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    literalListById,
                    row.BaseId,
                    "NullLiteral",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.NumericLiteralList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "NumericLiteral",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    literalListById,
                    row.BaseId,
                    "NumericLiteral",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.OffsetClauseFetchExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "OffsetClauseFetchExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    offsetClauseListById,
                    row.OwnerId,
                    "OffsetClauseFetchExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.OffsetClauseFetchExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "OffsetClauseFetchExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "OffsetClauseFetchExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.OffsetClauseOffsetExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "OffsetClauseOffsetExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    offsetClauseListById,
                    row.OwnerId,
                    "OffsetClauseOffsetExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.OffsetClauseOffsetExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "OffsetClauseOffsetExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "OffsetClauseOffsetExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.OrderByClauseOrderByElementsItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "OrderByClauseOrderByElementsItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    orderByClauseListById,
                    row.OwnerId,
                    "OrderByClauseOrderByElementsItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.OrderByClauseOrderByElementsItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "OrderByClauseOrderByElementsItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    expressionWithSortOrderListById,
                    row.ValueId,
                    "OrderByClauseOrderByElementsItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.OverClauseOrderByClauseLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "OverClauseOrderByClauseLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    overClauseListById,
                    row.OwnerId,
                    "OverClauseOrderByClauseLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.OverClauseOrderByClauseLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "OverClauseOrderByClauseLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    orderByClauseListById,
                    row.ValueId,
                    "OverClauseOrderByClauseLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.OverClausePartitionsItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "OverClausePartitionsItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    overClauseListById,
                    row.OwnerId,
                    "OverClausePartitionsItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.OverClausePartitionsItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "OverClausePartitionsItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "OverClausePartitionsItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.OverClauseWindowFrameClauseLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "OverClauseWindowFrameClauseLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    overClauseListById,
                    row.OwnerId,
                    "OverClauseWindowFrameClauseLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.OverClauseWindowFrameClauseLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "OverClauseWindowFrameClauseLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    windowFrameClauseListById,
                    row.ValueId,
                    "OverClauseWindowFrameClauseLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.OverClauseWindowNameLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "OverClauseWindowNameLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    overClauseListById,
                    row.OwnerId,
                    "OverClauseWindowNameLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.OverClauseWindowNameLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "OverClauseWindowNameLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    identifierListById,
                    row.ValueId,
                    "OverClauseWindowNameLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.ParameterizedDataTypeReferenceList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "ParameterizedDataTypeReference",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    dataTypeReferenceListById,
                    row.BaseId,
                    "ParameterizedDataTypeReference",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.ParameterizedDataTypeReferenceParametersItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "ParameterizedDataTypeReferenceParametersItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    parameterizedDataTypeReferenceListById,
                    row.OwnerId,
                    "ParameterizedDataTypeReferenceParametersItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.ParameterizedDataTypeReferenceParametersItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "ParameterizedDataTypeReferenceParametersItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    literalListById,
                    row.ValueId,
                    "ParameterizedDataTypeReferenceParametersItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.ParameterlessCallList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "ParameterlessCall",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    primaryExpressionListById,
                    row.BaseId,
                    "ParameterlessCall",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.ParenthesisExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "ParenthesisExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    primaryExpressionListById,
                    row.BaseId,
                    "ParenthesisExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.ParenthesisExpressionExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "ParenthesisExpressionExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    parenthesisExpressionListById,
                    row.OwnerId,
                    "ParenthesisExpressionExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.ParenthesisExpressionExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "ParenthesisExpressionExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "ParenthesisExpressionExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.ParseCallList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "ParseCall",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    primaryExpressionListById,
                    row.BaseId,
                    "ParseCall",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.ParseCallCultureLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "ParseCallCultureLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    parseCallListById,
                    row.OwnerId,
                    "ParseCallCultureLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.ParseCallCultureLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "ParseCallCultureLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "ParseCallCultureLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.ParseCallDataTypeLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "ParseCallDataTypeLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    parseCallListById,
                    row.OwnerId,
                    "ParseCallDataTypeLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.ParseCallDataTypeLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "ParseCallDataTypeLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    dataTypeReferenceListById,
                    row.ValueId,
                    "ParseCallDataTypeLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.ParseCallStringValueLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "ParseCallStringValueLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    parseCallListById,
                    row.OwnerId,
                    "ParseCallStringValueLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.ParseCallStringValueLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "ParseCallStringValueLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "ParseCallStringValueLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.PivotedTableReferenceList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "PivotedTableReference",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    tableReferenceWithAliasListById,
                    row.BaseId,
                    "PivotedTableReference",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.PivotedTableReferenceAggregateFunctionIdentifierLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "PivotedTableReferenceAggregateFunctionIdentifierLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    pivotedTableReferenceListById,
                    row.OwnerId,
                    "PivotedTableReferenceAggregateFunctionIdentifierLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.PivotedTableReferenceAggregateFunctionIdentifierLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "PivotedTableReferenceAggregateFunctionIdentifierLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    multiPartIdentifierListById,
                    row.ValueId,
                    "PivotedTableReferenceAggregateFunctionIdentifierLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.PivotedTableReferenceInColumnsItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "PivotedTableReferenceInColumnsItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    pivotedTableReferenceListById,
                    row.OwnerId,
                    "PivotedTableReferenceInColumnsItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.PivotedTableReferenceInColumnsItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "PivotedTableReferenceInColumnsItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    identifierListById,
                    row.ValueId,
                    "PivotedTableReferenceInColumnsItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.PivotedTableReferencePivotColumnLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "PivotedTableReferencePivotColumnLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    pivotedTableReferenceListById,
                    row.OwnerId,
                    "PivotedTableReferencePivotColumnLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.PivotedTableReferencePivotColumnLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "PivotedTableReferencePivotColumnLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    columnReferenceExpressionListById,
                    row.ValueId,
                    "PivotedTableReferencePivotColumnLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.PivotedTableReferenceTableReferenceLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "PivotedTableReferenceTableReferenceLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    pivotedTableReferenceListById,
                    row.OwnerId,
                    "PivotedTableReferenceTableReferenceLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.PivotedTableReferenceTableReferenceLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "PivotedTableReferenceTableReferenceLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    tableReferenceListById,
                    row.ValueId,
                    "PivotedTableReferenceTableReferenceLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.PivotedTableReferenceValueColumnsItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "PivotedTableReferenceValueColumnsItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    pivotedTableReferenceListById,
                    row.OwnerId,
                    "PivotedTableReferenceValueColumnsItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.PivotedTableReferenceValueColumnsItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "PivotedTableReferenceValueColumnsItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    columnReferenceExpressionListById,
                    row.ValueId,
                    "PivotedTableReferenceValueColumnsItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.PrimaryExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "PrimaryExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    scalarExpressionListById,
                    row.BaseId,
                    "PrimaryExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.PrimaryExpressionCollationLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "PrimaryExpressionCollationLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    primaryExpressionListById,
                    row.OwnerId,
                    "PrimaryExpressionCollationLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.PrimaryExpressionCollationLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "PrimaryExpressionCollationLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    identifierListById,
                    row.ValueId,
                    "PrimaryExpressionCollationLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.QualifiedJoinList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "QualifiedJoin",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    joinTableReferenceListById,
                    row.BaseId,
                    "QualifiedJoin",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.QualifiedJoinSearchConditionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "QualifiedJoinSearchConditionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    qualifiedJoinListById,
                    row.OwnerId,
                    "QualifiedJoinSearchConditionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.QualifiedJoinSearchConditionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "QualifiedJoinSearchConditionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    booleanExpressionListById,
                    row.ValueId,
                    "QualifiedJoinSearchConditionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.QueryDerivedTableList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "QueryDerivedTable",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    tableReferenceWithAliasAndColumnsListById,
                    row.BaseId,
                    "QueryDerivedTable",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.QueryDerivedTableQueryExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "QueryDerivedTableQueryExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    queryDerivedTableListById,
                    row.OwnerId,
                    "QueryDerivedTableQueryExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.QueryDerivedTableQueryExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "QueryDerivedTableQueryExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    queryExpressionListById,
                    row.ValueId,
                    "QueryDerivedTableQueryExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.QueryExpressionOffsetClauseLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "QueryExpressionOffsetClauseLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    queryExpressionListById,
                    row.OwnerId,
                    "QueryExpressionOffsetClauseLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.QueryExpressionOffsetClauseLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "QueryExpressionOffsetClauseLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    offsetClauseListById,
                    row.ValueId,
                    "QueryExpressionOffsetClauseLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.QueryExpressionOrderByClauseLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "QueryExpressionOrderByClauseLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    queryExpressionListById,
                    row.OwnerId,
                    "QueryExpressionOrderByClauseLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.QueryExpressionOrderByClauseLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "QueryExpressionOrderByClauseLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    orderByClauseListById,
                    row.ValueId,
                    "QueryExpressionOrderByClauseLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.QueryParenthesisExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "QueryParenthesisExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    queryExpressionListById,
                    row.BaseId,
                    "QueryParenthesisExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.QueryParenthesisExpressionQueryExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "QueryParenthesisExpressionQueryExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    queryParenthesisExpressionListById,
                    row.OwnerId,
                    "QueryParenthesisExpressionQueryExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.QueryParenthesisExpressionQueryExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "QueryParenthesisExpressionQueryExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    queryExpressionListById,
                    row.ValueId,
                    "QueryParenthesisExpressionQueryExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.QuerySpecificationList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "QuerySpecification",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    queryExpressionListById,
                    row.BaseId,
                    "QuerySpecification",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.QuerySpecificationFromClauseLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "QuerySpecificationFromClauseLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    querySpecificationListById,
                    row.OwnerId,
                    "QuerySpecificationFromClauseLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.QuerySpecificationFromClauseLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "QuerySpecificationFromClauseLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    fromClauseListById,
                    row.ValueId,
                    "QuerySpecificationFromClauseLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.QuerySpecificationGroupByClauseLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "QuerySpecificationGroupByClauseLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    querySpecificationListById,
                    row.OwnerId,
                    "QuerySpecificationGroupByClauseLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.QuerySpecificationGroupByClauseLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "QuerySpecificationGroupByClauseLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    groupByClauseListById,
                    row.ValueId,
                    "QuerySpecificationGroupByClauseLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.QuerySpecificationHavingClauseLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "QuerySpecificationHavingClauseLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    querySpecificationListById,
                    row.OwnerId,
                    "QuerySpecificationHavingClauseLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.QuerySpecificationHavingClauseLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "QuerySpecificationHavingClauseLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    havingClauseListById,
                    row.ValueId,
                    "QuerySpecificationHavingClauseLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.QuerySpecificationSelectElementsItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "QuerySpecificationSelectElementsItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    querySpecificationListById,
                    row.OwnerId,
                    "QuerySpecificationSelectElementsItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.QuerySpecificationSelectElementsItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "QuerySpecificationSelectElementsItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    selectElementListById,
                    row.ValueId,
                    "QuerySpecificationSelectElementsItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.QuerySpecificationTopRowFilterLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "QuerySpecificationTopRowFilterLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    querySpecificationListById,
                    row.OwnerId,
                    "QuerySpecificationTopRowFilterLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.QuerySpecificationTopRowFilterLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "QuerySpecificationTopRowFilterLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    topRowFilterListById,
                    row.ValueId,
                    "QuerySpecificationTopRowFilterLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.QuerySpecificationWhereClauseLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "QuerySpecificationWhereClauseLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    querySpecificationListById,
                    row.OwnerId,
                    "QuerySpecificationWhereClauseLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.QuerySpecificationWhereClauseLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "QuerySpecificationWhereClauseLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    whereClauseListById,
                    row.ValueId,
                    "QuerySpecificationWhereClauseLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.QuerySpecificationWindowClauseLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "QuerySpecificationWindowClauseLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    querySpecificationListById,
                    row.OwnerId,
                    "QuerySpecificationWindowClauseLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.QuerySpecificationWindowClauseLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "QuerySpecificationWindowClauseLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    windowClauseListById,
                    row.ValueId,
                    "QuerySpecificationWindowClauseLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.RealLiteralList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "RealLiteral",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    literalListById,
                    row.BaseId,
                    "RealLiteral",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.RightFunctionCallList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "RightFunctionCall",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    primaryExpressionListById,
                    row.BaseId,
                    "RightFunctionCall",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.RightFunctionCallParametersItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "RightFunctionCallParametersItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    rightFunctionCallListById,
                    row.OwnerId,
                    "RightFunctionCallParametersItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.RightFunctionCallParametersItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "RightFunctionCallParametersItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "RightFunctionCallParametersItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.RollupGroupingSpecificationList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "RollupGroupingSpecification",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    groupingSpecificationListById,
                    row.BaseId,
                    "RollupGroupingSpecification",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.RollupGroupingSpecificationArgumentsItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "RollupGroupingSpecificationArgumentsItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    rollupGroupingSpecificationListById,
                    row.OwnerId,
                    "RollupGroupingSpecificationArgumentsItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.RollupGroupingSpecificationArgumentsItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "RollupGroupingSpecificationArgumentsItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    groupingSpecificationListById,
                    row.ValueId,
                    "RollupGroupingSpecificationArgumentsItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.RowValueColumnValuesItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "RowValueColumnValuesItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    rowValueListById,
                    row.OwnerId,
                    "RowValueColumnValuesItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.RowValueColumnValuesItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "RowValueColumnValuesItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "RowValueColumnValuesItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.ScalarSubqueryList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "ScalarSubquery",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    primaryExpressionListById,
                    row.BaseId,
                    "ScalarSubquery",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.ScalarSubqueryQueryExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "ScalarSubqueryQueryExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    scalarSubqueryListById,
                    row.OwnerId,
                    "ScalarSubqueryQueryExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.ScalarSubqueryQueryExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "ScalarSubqueryQueryExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    queryExpressionListById,
                    row.ValueId,
                    "ScalarSubqueryQueryExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.SchemaObjectFunctionTableReferenceList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "SchemaObjectFunctionTableReference",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    tableReferenceWithAliasAndColumnsListById,
                    row.BaseId,
                    "SchemaObjectFunctionTableReference",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.SchemaObjectFunctionTableReferenceParametersItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "SchemaObjectFunctionTableReferenceParametersItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    schemaObjectFunctionTableReferenceListById,
                    row.OwnerId,
                    "SchemaObjectFunctionTableReferenceParametersItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.SchemaObjectFunctionTableReferenceParametersItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "SchemaObjectFunctionTableReferenceParametersItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "SchemaObjectFunctionTableReferenceParametersItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.SchemaObjectFunctionTableReferenceSchemaObjectLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "SchemaObjectFunctionTableReferenceSchemaObjectLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    schemaObjectFunctionTableReferenceListById,
                    row.OwnerId,
                    "SchemaObjectFunctionTableReferenceSchemaObjectLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.SchemaObjectFunctionTableReferenceSchemaObjectLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "SchemaObjectFunctionTableReferenceSchemaObjectLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    schemaObjectNameListById,
                    row.ValueId,
                    "SchemaObjectFunctionTableReferenceSchemaObjectLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.SchemaObjectNameList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "SchemaObjectName",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    multiPartIdentifierListById,
                    row.BaseId,
                    "SchemaObjectName",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.SchemaObjectNameBaseIdentifierLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "SchemaObjectNameBaseIdentifierLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    schemaObjectNameListById,
                    row.OwnerId,
                    "SchemaObjectNameBaseIdentifierLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.SchemaObjectNameBaseIdentifierLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "SchemaObjectNameBaseIdentifierLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    identifierListById,
                    row.ValueId,
                    "SchemaObjectNameBaseIdentifierLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.SchemaObjectNameSchemaIdentifierLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "SchemaObjectNameSchemaIdentifierLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    schemaObjectNameListById,
                    row.OwnerId,
                    "SchemaObjectNameSchemaIdentifierLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.SchemaObjectNameSchemaIdentifierLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "SchemaObjectNameSchemaIdentifierLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    identifierListById,
                    row.ValueId,
                    "SchemaObjectNameSchemaIdentifierLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.SearchedCaseExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "SearchedCaseExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    caseExpressionListById,
                    row.BaseId,
                    "SearchedCaseExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.SearchedCaseExpressionWhenClausesItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "SearchedCaseExpressionWhenClausesItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    searchedCaseExpressionListById,
                    row.OwnerId,
                    "SearchedCaseExpressionWhenClausesItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.SearchedCaseExpressionWhenClausesItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "SearchedCaseExpressionWhenClausesItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    searchedWhenClauseListById,
                    row.ValueId,
                    "SearchedCaseExpressionWhenClausesItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.SearchedWhenClauseList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "SearchedWhenClause",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    whenClauseListById,
                    row.BaseId,
                    "SearchedWhenClause",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.SearchedWhenClauseWhenExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "SearchedWhenClauseWhenExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    searchedWhenClauseListById,
                    row.OwnerId,
                    "SearchedWhenClauseWhenExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.SearchedWhenClauseWhenExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "SearchedWhenClauseWhenExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    booleanExpressionListById,
                    row.ValueId,
                    "SearchedWhenClauseWhenExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.SelectScalarExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "SelectScalarExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    selectElementListById,
                    row.BaseId,
                    "SelectScalarExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.SelectScalarExpressionColumnNameLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "SelectScalarExpressionColumnNameLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    selectScalarExpressionListById,
                    row.OwnerId,
                    "SelectScalarExpressionColumnNameLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.SelectScalarExpressionColumnNameLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "SelectScalarExpressionColumnNameLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    identifierOrValueExpressionListById,
                    row.ValueId,
                    "SelectScalarExpressionColumnNameLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.SelectScalarExpressionExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "SelectScalarExpressionExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    selectScalarExpressionListById,
                    row.OwnerId,
                    "SelectScalarExpressionExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.SelectScalarExpressionExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "SelectScalarExpressionExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "SelectScalarExpressionExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.SelectStarExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "SelectStarExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    selectElementListById,
                    row.BaseId,
                    "SelectStarExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.SelectStarExpressionQualifierLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "SelectStarExpressionQualifierLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    selectStarExpressionListById,
                    row.OwnerId,
                    "SelectStarExpressionQualifierLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.SelectStarExpressionQualifierLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "SelectStarExpressionQualifierLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    multiPartIdentifierListById,
                    row.ValueId,
                    "SelectStarExpressionQualifierLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.SelectStatementList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "SelectStatement",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    statementWithCtesAndXmlNamespacesListById,
                    row.BaseId,
                    "SelectStatement",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.SelectStatementQueryExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "SelectStatementQueryExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    selectStatementListById,
                    row.OwnerId,
                    "SelectStatementQueryExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.SelectStatementQueryExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "SelectStatementQueryExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    queryExpressionListById,
                    row.ValueId,
                    "SelectStatementQueryExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.SimpleCaseExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "SimpleCaseExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    caseExpressionListById,
                    row.BaseId,
                    "SimpleCaseExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.SimpleCaseExpressionInputExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "SimpleCaseExpressionInputExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    simpleCaseExpressionListById,
                    row.OwnerId,
                    "SimpleCaseExpressionInputExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.SimpleCaseExpressionInputExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "SimpleCaseExpressionInputExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "SimpleCaseExpressionInputExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.SimpleCaseExpressionWhenClausesItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "SimpleCaseExpressionWhenClausesItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    simpleCaseExpressionListById,
                    row.OwnerId,
                    "SimpleCaseExpressionWhenClausesItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.SimpleCaseExpressionWhenClausesItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "SimpleCaseExpressionWhenClausesItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    simpleWhenClauseListById,
                    row.ValueId,
                    "SimpleCaseExpressionWhenClausesItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.SimpleWhenClauseList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "SimpleWhenClause",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    whenClauseListById,
                    row.BaseId,
                    "SimpleWhenClause",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.SimpleWhenClauseWhenExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "SimpleWhenClauseWhenExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    simpleWhenClauseListById,
                    row.OwnerId,
                    "SimpleWhenClauseWhenExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.SimpleWhenClauseWhenExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "SimpleWhenClauseWhenExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "SimpleWhenClauseWhenExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.SqlDataTypeReferenceList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "SqlDataTypeReference",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    parameterizedDataTypeReferenceListById,
                    row.BaseId,
                    "SqlDataTypeReference",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.StatementWithCtesAndXmlNamespacesList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "StatementWithCtesAndXmlNamespaces",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    tSqlStatementListById,
                    row.BaseId,
                    "StatementWithCtesAndXmlNamespaces",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    statementWithCtesAndXmlNamespacesListById,
                    row.OwnerId,
                    "StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    withCtesAndXmlNamespacesListById,
                    row.ValueId,
                    "StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.StringLiteralList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "StringLiteral",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    literalListById,
                    row.BaseId,
                    "StringLiteral",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.SubqueryComparisonPredicateList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "SubqueryComparisonPredicate",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    booleanExpressionListById,
                    row.BaseId,
                    "SubqueryComparisonPredicate",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.SubqueryComparisonPredicateExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "SubqueryComparisonPredicateExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    subqueryComparisonPredicateListById,
                    row.OwnerId,
                    "SubqueryComparisonPredicateExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.SubqueryComparisonPredicateExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "SubqueryComparisonPredicateExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "SubqueryComparisonPredicateExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.SubqueryComparisonPredicateSubqueryLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "SubqueryComparisonPredicateSubqueryLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    subqueryComparisonPredicateListById,
                    row.OwnerId,
                    "SubqueryComparisonPredicateSubqueryLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.SubqueryComparisonPredicateSubqueryLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "SubqueryComparisonPredicateSubqueryLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarSubqueryListById,
                    row.ValueId,
                    "SubqueryComparisonPredicateSubqueryLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.TableReferenceWithAliasList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "TableReferenceWithAlias",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    tableReferenceListById,
                    row.BaseId,
                    "TableReferenceWithAlias",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.TableReferenceWithAliasAliasLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "TableReferenceWithAliasAliasLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    tableReferenceWithAliasListById,
                    row.OwnerId,
                    "TableReferenceWithAliasAliasLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.TableReferenceWithAliasAliasLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "TableReferenceWithAliasAliasLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    identifierListById,
                    row.ValueId,
                    "TableReferenceWithAliasAliasLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.TableReferenceWithAliasAndColumnsList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "TableReferenceWithAliasAndColumns",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    tableReferenceWithAliasListById,
                    row.BaseId,
                    "TableReferenceWithAliasAndColumns",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.TableReferenceWithAliasAndColumnsColumnsItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "TableReferenceWithAliasAndColumnsColumnsItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    tableReferenceWithAliasAndColumnsListById,
                    row.OwnerId,
                    "TableReferenceWithAliasAndColumnsColumnsItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.TableReferenceWithAliasAndColumnsColumnsItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "TableReferenceWithAliasAndColumnsColumnsItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    identifierListById,
                    row.ValueId,
                    "TableReferenceWithAliasAndColumnsColumnsItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.TableSampleClauseRepeatSeedLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "TableSampleClauseRepeatSeedLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    tableSampleClauseListById,
                    row.OwnerId,
                    "TableSampleClauseRepeatSeedLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.TableSampleClauseRepeatSeedLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "TableSampleClauseRepeatSeedLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "TableSampleClauseRepeatSeedLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.TableSampleClauseSampleNumberLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "TableSampleClauseSampleNumberLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    tableSampleClauseListById,
                    row.OwnerId,
                    "TableSampleClauseSampleNumberLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.TableSampleClauseSampleNumberLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "TableSampleClauseSampleNumberLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "TableSampleClauseSampleNumberLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.TopRowFilterExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "TopRowFilterExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    topRowFilterListById,
                    row.OwnerId,
                    "TopRowFilterExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.TopRowFilterExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "TopRowFilterExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "TopRowFilterExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.TransformScriptObjectIdentifierLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "TransformScriptObjectIdentifierLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    transformScriptListById,
                    row.OwnerId,
                    "TransformScriptObjectIdentifierLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.TransformScriptObjectIdentifierLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "TransformScriptObjectIdentifierLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    identifierListById,
                    row.ValueId,
                    "TransformScriptObjectIdentifierLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.TransformScriptSchemaIdentifierLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "TransformScriptSchemaIdentifierLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    transformScriptListById,
                    row.OwnerId,
                    "TransformScriptSchemaIdentifierLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.TransformScriptSchemaIdentifierLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "TransformScriptSchemaIdentifierLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    identifierListById,
                    row.ValueId,
                    "TransformScriptSchemaIdentifierLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.TransformScriptSelectStatementLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "TransformScriptSelectStatementLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    transformScriptListById,
                    row.OwnerId,
                    "TransformScriptSelectStatementLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.TransformScriptSelectStatementLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "TransformScriptSelectStatementLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    selectStatementListById,
                    row.ValueId,
                    "TransformScriptSelectStatementLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.TransformScriptViewColumnsItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "TransformScriptViewColumnsItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    transformScriptListById,
                    row.OwnerId,
                    "TransformScriptViewColumnsItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.TransformScriptViewColumnsItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "TransformScriptViewColumnsItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    identifierListById,
                    row.ValueId,
                    "TransformScriptViewColumnsItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.TryCastCallList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "TryCastCall",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    primaryExpressionListById,
                    row.BaseId,
                    "TryCastCall",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.TryCastCallDataTypeLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "TryCastCallDataTypeLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    tryCastCallListById,
                    row.OwnerId,
                    "TryCastCallDataTypeLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.TryCastCallDataTypeLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "TryCastCallDataTypeLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    dataTypeReferenceListById,
                    row.ValueId,
                    "TryCastCallDataTypeLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.TryCastCallParameterLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "TryCastCallParameterLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    tryCastCallListById,
                    row.OwnerId,
                    "TryCastCallParameterLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.TryCastCallParameterLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "TryCastCallParameterLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "TryCastCallParameterLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.TryConvertCallList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "TryConvertCall",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    primaryExpressionListById,
                    row.BaseId,
                    "TryConvertCall",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.TryConvertCallDataTypeLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "TryConvertCallDataTypeLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    tryConvertCallListById,
                    row.OwnerId,
                    "TryConvertCallDataTypeLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.TryConvertCallDataTypeLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "TryConvertCallDataTypeLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    dataTypeReferenceListById,
                    row.ValueId,
                    "TryConvertCallDataTypeLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.TryConvertCallParameterLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "TryConvertCallParameterLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    tryConvertCallListById,
                    row.OwnerId,
                    "TryConvertCallParameterLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.TryConvertCallParameterLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "TryConvertCallParameterLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "TryConvertCallParameterLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.TryConvertCallStyleLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "TryConvertCallStyleLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    tryConvertCallListById,
                    row.OwnerId,
                    "TryConvertCallStyleLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.TryConvertCallStyleLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "TryConvertCallStyleLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "TryConvertCallStyleLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.TryParseCallList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "TryParseCall",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    primaryExpressionListById,
                    row.BaseId,
                    "TryParseCall",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.TryParseCallCultureLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "TryParseCallCultureLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    tryParseCallListById,
                    row.OwnerId,
                    "TryParseCallCultureLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.TryParseCallCultureLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "TryParseCallCultureLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "TryParseCallCultureLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.TryParseCallDataTypeLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "TryParseCallDataTypeLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    tryParseCallListById,
                    row.OwnerId,
                    "TryParseCallDataTypeLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.TryParseCallDataTypeLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "TryParseCallDataTypeLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    dataTypeReferenceListById,
                    row.ValueId,
                    "TryParseCallDataTypeLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.TryParseCallStringValueLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "TryParseCallStringValueLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    tryParseCallListById,
                    row.OwnerId,
                    "TryParseCallStringValueLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.TryParseCallStringValueLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "TryParseCallStringValueLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "TryParseCallStringValueLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.UnaryExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "UnaryExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    scalarExpressionListById,
                    row.BaseId,
                    "UnaryExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.UnaryExpressionExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "UnaryExpressionExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    unaryExpressionListById,
                    row.OwnerId,
                    "UnaryExpressionExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.UnaryExpressionExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "UnaryExpressionExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "UnaryExpressionExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.UnpivotedTableReferenceList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "UnpivotedTableReference",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    tableReferenceWithAliasListById,
                    row.BaseId,
                    "UnpivotedTableReference",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.UnpivotedTableReferenceInColumnsItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "UnpivotedTableReferenceInColumnsItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    unpivotedTableReferenceListById,
                    row.OwnerId,
                    "UnpivotedTableReferenceInColumnsItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.UnpivotedTableReferenceInColumnsItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "UnpivotedTableReferenceInColumnsItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    columnReferenceExpressionListById,
                    row.ValueId,
                    "UnpivotedTableReferenceInColumnsItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.UnpivotedTableReferencePivotColumnLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "UnpivotedTableReferencePivotColumnLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    unpivotedTableReferenceListById,
                    row.OwnerId,
                    "UnpivotedTableReferencePivotColumnLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.UnpivotedTableReferencePivotColumnLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "UnpivotedTableReferencePivotColumnLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    identifierListById,
                    row.ValueId,
                    "UnpivotedTableReferencePivotColumnLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.UnpivotedTableReferenceTableReferenceLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "UnpivotedTableReferenceTableReferenceLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    unpivotedTableReferenceListById,
                    row.OwnerId,
                    "UnpivotedTableReferenceTableReferenceLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.UnpivotedTableReferenceTableReferenceLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "UnpivotedTableReferenceTableReferenceLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    tableReferenceListById,
                    row.ValueId,
                    "UnpivotedTableReferenceTableReferenceLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.UnpivotedTableReferenceValueColumnLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "UnpivotedTableReferenceValueColumnLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    unpivotedTableReferenceListById,
                    row.OwnerId,
                    "UnpivotedTableReferenceValueColumnLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.UnpivotedTableReferenceValueColumnLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "UnpivotedTableReferenceValueColumnLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    identifierListById,
                    row.ValueId,
                    "UnpivotedTableReferenceValueColumnLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.UnqualifiedJoinList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "UnqualifiedJoin",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    joinTableReferenceListById,
                    row.BaseId,
                    "UnqualifiedJoin",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.ValueExpressionList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "ValueExpression",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    primaryExpressionListById,
                    row.BaseId,
                    "ValueExpression",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.WhenClauseThenExpressionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "WhenClauseThenExpressionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    whenClauseListById,
                    row.OwnerId,
                    "WhenClauseThenExpressionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.WhenClauseThenExpressionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "WhenClauseThenExpressionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "WhenClauseThenExpressionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.WhereClauseSearchConditionLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "WhereClauseSearchConditionLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    whereClauseListById,
                    row.OwnerId,
                    "WhereClauseSearchConditionLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.WhereClauseSearchConditionLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "WhereClauseSearchConditionLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    booleanExpressionListById,
                    row.ValueId,
                    "WhereClauseSearchConditionLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.WindowClauseWindowDefinitionItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "WindowClauseWindowDefinitionItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    windowClauseListById,
                    row.OwnerId,
                    "WindowClauseWindowDefinitionItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.WindowClauseWindowDefinitionItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "WindowClauseWindowDefinitionItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    windowDefinitionListById,
                    row.ValueId,
                    "WindowClauseWindowDefinitionItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.WindowDefinitionOrderByClauseLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "WindowDefinitionOrderByClauseLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    windowDefinitionListById,
                    row.OwnerId,
                    "WindowDefinitionOrderByClauseLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.WindowDefinitionOrderByClauseLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "WindowDefinitionOrderByClauseLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    orderByClauseListById,
                    row.ValueId,
                    "WindowDefinitionOrderByClauseLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.WindowDefinitionPartitionsItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "WindowDefinitionPartitionsItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    windowDefinitionListById,
                    row.OwnerId,
                    "WindowDefinitionPartitionsItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.WindowDefinitionPartitionsItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "WindowDefinitionPartitionsItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "WindowDefinitionPartitionsItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.WindowDefinitionRefWindowNameLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "WindowDefinitionRefWindowNameLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    windowDefinitionListById,
                    row.OwnerId,
                    "WindowDefinitionRefWindowNameLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.WindowDefinitionRefWindowNameLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "WindowDefinitionRefWindowNameLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    identifierListById,
                    row.ValueId,
                    "WindowDefinitionRefWindowNameLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.WindowDefinitionWindowFrameClauseLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "WindowDefinitionWindowFrameClauseLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    windowDefinitionListById,
                    row.OwnerId,
                    "WindowDefinitionWindowFrameClauseLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.WindowDefinitionWindowFrameClauseLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "WindowDefinitionWindowFrameClauseLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    windowFrameClauseListById,
                    row.ValueId,
                    "WindowDefinitionWindowFrameClauseLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.WindowDefinitionWindowNameLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "WindowDefinitionWindowNameLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    windowDefinitionListById,
                    row.OwnerId,
                    "WindowDefinitionWindowNameLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.WindowDefinitionWindowNameLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "WindowDefinitionWindowNameLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    identifierListById,
                    row.ValueId,
                    "WindowDefinitionWindowNameLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.WindowDelimiterOffsetValueLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "WindowDelimiterOffsetValueLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    windowDelimiterListById,
                    row.OwnerId,
                    "WindowDelimiterOffsetValueLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.WindowDelimiterOffsetValueLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "WindowDelimiterOffsetValueLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    scalarExpressionListById,
                    row.ValueId,
                    "WindowDelimiterOffsetValueLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.WindowFrameClauseBottomLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "WindowFrameClauseBottomLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    windowFrameClauseListById,
                    row.OwnerId,
                    "WindowFrameClauseBottomLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.WindowFrameClauseBottomLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "WindowFrameClauseBottomLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    windowDelimiterListById,
                    row.ValueId,
                    "WindowFrameClauseBottomLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.WindowFrameClauseTopLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "WindowFrameClauseTopLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    windowFrameClauseListById,
                    row.OwnerId,
                    "WindowFrameClauseTopLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.WindowFrameClauseTopLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "WindowFrameClauseTopLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    windowDelimiterListById,
                    row.ValueId,
                    "WindowFrameClauseTopLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.WithCtesAndXmlNamespacesCommonTableExpressionsItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "WithCtesAndXmlNamespacesCommonTableExpressionsItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    withCtesAndXmlNamespacesListById,
                    row.OwnerId,
                    "WithCtesAndXmlNamespacesCommonTableExpressionsItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.WithCtesAndXmlNamespacesCommonTableExpressionsItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "WithCtesAndXmlNamespacesCommonTableExpressionsItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    commonTableExpressionListById,
                    row.ValueId,
                    "WithCtesAndXmlNamespacesCommonTableExpressionsItem",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.WithCtesAndXmlNamespacesXmlNamespacesLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "WithCtesAndXmlNamespacesXmlNamespacesLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    withCtesAndXmlNamespacesListById,
                    row.OwnerId,
                    "WithCtesAndXmlNamespacesXmlNamespacesLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.WithCtesAndXmlNamespacesXmlNamespacesLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "WithCtesAndXmlNamespacesXmlNamespacesLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    xmlNamespacesListById,
                    row.ValueId,
                    "WithCtesAndXmlNamespacesXmlNamespacesLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.XmlNamespacesAliasElementList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "XmlNamespacesAliasElement",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    xmlNamespacesElementListById,
                    row.BaseId,
                    "XmlNamespacesAliasElement",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.XmlNamespacesAliasElementIdentifierLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "XmlNamespacesAliasElementIdentifierLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    xmlNamespacesAliasElementListById,
                    row.OwnerId,
                    "XmlNamespacesAliasElementIdentifierLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.XmlNamespacesAliasElementIdentifierLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "XmlNamespacesAliasElementIdentifierLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    identifierListById,
                    row.ValueId,
                    "XmlNamespacesAliasElementIdentifierLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.XmlNamespacesDefaultElementList)
            {
                row.BaseId = ResolveRelationshipId(
                    row.BaseId,
                    row.Base?.Id,
                    "XmlNamespacesDefaultElement",
                    row.Id,
                    "BaseId");
                row.Base = RequireTarget(
                    xmlNamespacesElementListById,
                    row.BaseId,
                    "XmlNamespacesDefaultElement",
                    row.Id,
                    "BaseId");
            }

            foreach (var row in model.XmlNamespacesElementStringLinkList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "XmlNamespacesElementStringLink",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    xmlNamespacesElementListById,
                    row.OwnerId,
                    "XmlNamespacesElementStringLink",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.XmlNamespacesElementStringLinkList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "XmlNamespacesElementStringLink",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    stringLiteralListById,
                    row.ValueId,
                    "XmlNamespacesElementStringLink",
                    row.Id,
                    "ValueId");
            }

            foreach (var row in model.XmlNamespacesXmlNamespacesElementsItemList)
            {
                row.OwnerId = ResolveRelationshipId(
                    row.OwnerId,
                    row.Owner?.Id,
                    "XmlNamespacesXmlNamespacesElementsItem",
                    row.Id,
                    "OwnerId");
                row.Owner = RequireTarget(
                    xmlNamespacesListById,
                    row.OwnerId,
                    "XmlNamespacesXmlNamespacesElementsItem",
                    row.Id,
                    "OwnerId");
            }

            foreach (var row in model.XmlNamespacesXmlNamespacesElementsItemList)
            {
                row.ValueId = ResolveRelationshipId(
                    row.ValueId,
                    row.Value?.Id,
                    "XmlNamespacesXmlNamespacesElementsItem",
                    row.Id,
                    "ValueId");
                row.Value = RequireTarget(
                    xmlNamespacesElementListById,
                    row.ValueId,
                    "XmlNamespacesXmlNamespacesElementsItem",
                    row.Id,
                    "ValueId");
            }

        }

        private static void NormalizeAtTimeZoneCallList(MetaTransformScriptModel model)
        {
            foreach (var row in model.AtTimeZoneCallList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'AtTimeZoneCall' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeAtTimeZoneCallDateValueLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.AtTimeZoneCallDateValueLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'AtTimeZoneCallDateValueLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeAtTimeZoneCallTimeZoneLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.AtTimeZoneCallTimeZoneLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'AtTimeZoneCallTimeZoneLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeBinaryExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BinaryExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BinaryExpression' contains a row with empty Id.");
                row.BinaryExpressionType ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeBinaryExpressionFirstExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BinaryExpressionFirstExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BinaryExpressionFirstExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeBinaryExpressionSecondExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BinaryExpressionSecondExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BinaryExpressionSecondExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeBinaryLiteralList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BinaryLiteralList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BinaryLiteral' contains a row with empty Id.");
                row.IsLargeObject ??= string.Empty;
                row.LiteralType ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeBinaryQueryExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BinaryQueryExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BinaryQueryExpression' contains a row with empty Id.");
                row.All ??= string.Empty;
                row.BinaryQueryExpressionType ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeBinaryQueryExpressionFirstQueryExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BinaryQueryExpressionFirstQueryExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BinaryQueryExpressionFirstQueryExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeBinaryQueryExpressionSecondQueryExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BinaryQueryExpressionSecondQueryExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BinaryQueryExpressionSecondQueryExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeBooleanBinaryExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BooleanBinaryExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BooleanBinaryExpression' contains a row with empty Id.");
                row.BinaryExpressionType ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeBooleanBinaryExpressionFirstExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BooleanBinaryExpressionFirstExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BooleanBinaryExpressionFirstExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeBooleanBinaryExpressionSecondExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BooleanBinaryExpressionSecondExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BooleanBinaryExpressionSecondExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeBooleanComparisonExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BooleanComparisonExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BooleanComparisonExpression' contains a row with empty Id.");
                row.ComparisonType ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeBooleanComparisonExpressionFirstExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BooleanComparisonExpressionFirstExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BooleanComparisonExpressionFirstExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeBooleanComparisonExpressionSecondExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BooleanComparisonExpressionSecondExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BooleanComparisonExpressionSecondExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeBooleanExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BooleanExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BooleanExpression' contains a row with empty Id.");
            }
        }

        private static void NormalizeBooleanIsNullExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BooleanIsNullExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BooleanIsNullExpression' contains a row with empty Id.");
                row.IsNot ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeBooleanIsNullExpressionExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BooleanIsNullExpressionExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BooleanIsNullExpressionExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeBooleanNotExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BooleanNotExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BooleanNotExpression' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeBooleanNotExpressionExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BooleanNotExpressionExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BooleanNotExpressionExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeBooleanParenthesisExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BooleanParenthesisExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BooleanParenthesisExpression' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeBooleanParenthesisExpressionExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BooleanParenthesisExpressionExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BooleanParenthesisExpressionExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeBooleanTernaryExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BooleanTernaryExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BooleanTernaryExpression' contains a row with empty Id.");
                row.TernaryExpressionType ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeBooleanTernaryExpressionFirstExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BooleanTernaryExpressionFirstExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BooleanTernaryExpressionFirstExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeBooleanTernaryExpressionSecondExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BooleanTernaryExpressionSecondExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BooleanTernaryExpressionSecondExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeBooleanTernaryExpressionThirdExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.BooleanTernaryExpressionThirdExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BooleanTernaryExpressionThirdExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeCallTargetList(MetaTransformScriptModel model)
        {
            foreach (var row in model.CallTargetList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'CallTarget' contains a row with empty Id.");
            }
        }

        private static void NormalizeCaseExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.CaseExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'CaseExpression' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeCaseExpressionElseExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.CaseExpressionElseExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'CaseExpressionElseExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeCastCallList(MetaTransformScriptModel model)
        {
            foreach (var row in model.CastCallList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'CastCall' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeCastCallDataTypeLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.CastCallDataTypeLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'CastCallDataTypeLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeCastCallParameterLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.CastCallParameterLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'CastCallParameterLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeCoalesceExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.CoalesceExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'CoalesceExpression' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeCoalesceExpressionExpressionsItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.CoalesceExpressionExpressionsItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'CoalesceExpressionExpressionsItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeColumnReferenceExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ColumnReferenceExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ColumnReferenceExpression' contains a row with empty Id.");
                row.ColumnType ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeColumnReferenceExpressionMultiPartIdentifierLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ColumnReferenceExpressionMultiPartIdentifierLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ColumnReferenceExpressionMultiPartIdentifierLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeCommonTableExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.CommonTableExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'CommonTableExpression' contains a row with empty Id.");
            }
        }

        private static void NormalizeCommonTableExpressionColumnsItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.CommonTableExpressionColumnsItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'CommonTableExpressionColumnsItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeCommonTableExpressionExpressionNameLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.CommonTableExpressionExpressionNameLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'CommonTableExpressionExpressionNameLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeCommonTableExpressionQueryExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.CommonTableExpressionQueryExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'CommonTableExpressionQueryExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeCompositeGroupingSpecificationList(MetaTransformScriptModel model)
        {
            foreach (var row in model.CompositeGroupingSpecificationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'CompositeGroupingSpecification' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeCompositeGroupingSpecificationItemsItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.CompositeGroupingSpecificationItemsItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'CompositeGroupingSpecificationItemsItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeConvertCallList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ConvertCallList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ConvertCall' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeConvertCallDataTypeLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ConvertCallDataTypeLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ConvertCallDataTypeLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeConvertCallParameterLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ConvertCallParameterLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ConvertCallParameterLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeConvertCallStyleLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ConvertCallStyleLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ConvertCallStyleLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeCubeGroupingSpecificationList(MetaTransformScriptModel model)
        {
            foreach (var row in model.CubeGroupingSpecificationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'CubeGroupingSpecification' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeCubeGroupingSpecificationArgumentsItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.CubeGroupingSpecificationArgumentsItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'CubeGroupingSpecificationArgumentsItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeDataTypeReferenceList(MetaTransformScriptModel model)
        {
            foreach (var row in model.DataTypeReferenceList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'DataTypeReference' contains a row with empty Id.");
            }
        }

        private static void NormalizeDataTypeReferenceNameLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.DataTypeReferenceNameLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'DataTypeReferenceNameLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeDistinctPredicateList(MetaTransformScriptModel model)
        {
            foreach (var row in model.DistinctPredicateList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'DistinctPredicate' contains a row with empty Id.");
                row.IsNot ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeDistinctPredicateFirstExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.DistinctPredicateFirstExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'DistinctPredicateFirstExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeDistinctPredicateSecondExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.DistinctPredicateSecondExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'DistinctPredicateSecondExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeExistsPredicateList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ExistsPredicateList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ExistsPredicate' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeExistsPredicateSubqueryLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ExistsPredicateSubqueryLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ExistsPredicateSubqueryLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeExpressionGroupingSpecificationList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ExpressionGroupingSpecificationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ExpressionGroupingSpecification' contains a row with empty Id.");
                row.DistributedAggregation ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeExpressionGroupingSpecificationExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ExpressionGroupingSpecificationExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ExpressionGroupingSpecificationExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeExpressionWithSortOrderList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ExpressionWithSortOrderList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ExpressionWithSortOrder' contains a row with empty Id.");
                row.SortOrder ??= string.Empty;
            }
        }

        private static void NormalizeExpressionWithSortOrderExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ExpressionWithSortOrderExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ExpressionWithSortOrderExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeFromClauseList(MetaTransformScriptModel model)
        {
            foreach (var row in model.FromClauseList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'FromClause' contains a row with empty Id.");
            }
        }

        private static void NormalizeFromClauseTableReferencesItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.FromClauseTableReferencesItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'FromClauseTableReferencesItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeFullTextPredicateList(MetaTransformScriptModel model)
        {
            foreach (var row in model.FullTextPredicateList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'FullTextPredicate' contains a row with empty Id.");
                row.FullTextFunctionType ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeFullTextPredicateColumnsItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.FullTextPredicateColumnsItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'FullTextPredicateColumnsItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeFullTextPredicateValueLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.FullTextPredicateValueLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'FullTextPredicateValueLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeFullTextTableReferenceList(MetaTransformScriptModel model)
        {
            foreach (var row in model.FullTextTableReferenceList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'FullTextTableReference' contains a row with empty Id.");
                row.FullTextFunctionType ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeFullTextTableReferenceColumnsItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.FullTextTableReferenceColumnsItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'FullTextTableReferenceColumnsItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeFullTextTableReferenceSearchConditionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.FullTextTableReferenceSearchConditionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'FullTextTableReferenceSearchConditionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeFullTextTableReferenceTableNameLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.FullTextTableReferenceTableNameLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'FullTextTableReferenceTableNameLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeFunctionCallList(MetaTransformScriptModel model)
        {
            foreach (var row in model.FunctionCallList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'FunctionCall' contains a row with empty Id.");
                row.UniqueRowFilter ??= string.Empty;
                row.WithArrayWrapper ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeFunctionCallCallTargetLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.FunctionCallCallTargetLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'FunctionCallCallTargetLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeFunctionCallFunctionNameLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.FunctionCallFunctionNameLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'FunctionCallFunctionNameLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeFunctionCallOverClauseLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.FunctionCallOverClauseLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'FunctionCallOverClauseLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeFunctionCallParametersItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.FunctionCallParametersItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'FunctionCallParametersItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeGlobalFunctionTableReferenceList(MetaTransformScriptModel model)
        {
            foreach (var row in model.GlobalFunctionTableReferenceList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'GlobalFunctionTableReference' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeGlobalFunctionTableReferenceNameLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.GlobalFunctionTableReferenceNameLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'GlobalFunctionTableReferenceNameLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeGlobalFunctionTableReferenceParametersItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.GlobalFunctionTableReferenceParametersItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'GlobalFunctionTableReferenceParametersItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeGlobalVariableExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.GlobalVariableExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'GlobalVariableExpression' contains a row with empty Id.");
                row.Name ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeGrandTotalGroupingSpecificationList(MetaTransformScriptModel model)
        {
            foreach (var row in model.GrandTotalGroupingSpecificationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'GrandTotalGroupingSpecification' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeGroupByClauseList(MetaTransformScriptModel model)
        {
            foreach (var row in model.GroupByClauseList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'GroupByClause' contains a row with empty Id.");
                row.All ??= string.Empty;
                row.GroupByOption ??= string.Empty;
            }
        }

        private static void NormalizeGroupByClauseGroupingSpecificationsItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.GroupByClauseGroupingSpecificationsItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'GroupByClauseGroupingSpecificationsItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeGroupingSetsGroupingSpecificationList(MetaTransformScriptModel model)
        {
            foreach (var row in model.GroupingSetsGroupingSpecificationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'GroupingSetsGroupingSpecification' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeGroupingSetsGroupingSpecificationSetsItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.GroupingSetsGroupingSpecificationSetsItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'GroupingSetsGroupingSpecificationSetsItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeGroupingSpecificationList(MetaTransformScriptModel model)
        {
            foreach (var row in model.GroupingSpecificationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'GroupingSpecification' contains a row with empty Id.");
            }
        }

        private static void NormalizeHavingClauseList(MetaTransformScriptModel model)
        {
            foreach (var row in model.HavingClauseList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'HavingClause' contains a row with empty Id.");
            }
        }

        private static void NormalizeHavingClauseSearchConditionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.HavingClauseSearchConditionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'HavingClauseSearchConditionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeIdentifierList(MetaTransformScriptModel model)
        {
            foreach (var row in model.IdentifierList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'Identifier' contains a row with empty Id.");
                row.QuoteType ??= string.Empty;
                row.Value ??= string.Empty;
            }
        }

        private static void NormalizeIdentifierOrValueExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.IdentifierOrValueExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'IdentifierOrValueExpression' contains a row with empty Id.");
                row.Value ??= string.Empty;
            }
        }

        private static void NormalizeIdentifierOrValueExpressionIdentifierLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.IdentifierOrValueExpressionIdentifierLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'IdentifierOrValueExpressionIdentifierLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeIIfCallList(MetaTransformScriptModel model)
        {
            foreach (var row in model.IIfCallList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'IIfCall' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeIIfCallElseExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.IIfCallElseExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'IIfCallElseExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeIIfCallPredicateLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.IIfCallPredicateLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'IIfCallPredicateLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeIIfCallThenExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.IIfCallThenExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'IIfCallThenExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeInlineDerivedTableList(MetaTransformScriptModel model)
        {
            foreach (var row in model.InlineDerivedTableList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'InlineDerivedTable' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeInlineDerivedTableRowValuesItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.InlineDerivedTableRowValuesItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'InlineDerivedTableRowValuesItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeInPredicateList(MetaTransformScriptModel model)
        {
            foreach (var row in model.InPredicateList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'InPredicate' contains a row with empty Id.");
                row.NotDefined ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeInPredicateExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.InPredicateExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'InPredicateExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeInPredicateSubqueryLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.InPredicateSubqueryLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'InPredicateSubqueryLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeInPredicateValuesItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.InPredicateValuesItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'InPredicateValuesItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeIntegerLiteralList(MetaTransformScriptModel model)
        {
            foreach (var row in model.IntegerLiteralList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'IntegerLiteral' contains a row with empty Id.");
                row.LiteralType ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeJoinParenthesisTableReferenceList(MetaTransformScriptModel model)
        {
            foreach (var row in model.JoinParenthesisTableReferenceList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'JoinParenthesisTableReference' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeJoinParenthesisTableReferenceJoinLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.JoinParenthesisTableReferenceJoinLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'JoinParenthesisTableReferenceJoinLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeJoinTableReferenceList(MetaTransformScriptModel model)
        {
            foreach (var row in model.JoinTableReferenceList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'JoinTableReference' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeJoinTableReferenceFirstTableReferenceLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.JoinTableReferenceFirstTableReferenceLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'JoinTableReferenceFirstTableReferenceLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeJoinTableReferenceSecondTableReferenceLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.JoinTableReferenceSecondTableReferenceLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'JoinTableReferenceSecondTableReferenceLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeLeftFunctionCallList(MetaTransformScriptModel model)
        {
            foreach (var row in model.LeftFunctionCallList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'LeftFunctionCall' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeLeftFunctionCallParametersItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.LeftFunctionCallParametersItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'LeftFunctionCallParametersItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeLikePredicateList(MetaTransformScriptModel model)
        {
            foreach (var row in model.LikePredicateList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'LikePredicate' contains a row with empty Id.");
                row.NotDefined ??= string.Empty;
                row.OdbcEscape ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeLikePredicateFirstExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.LikePredicateFirstExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'LikePredicateFirstExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeLikePredicateSecondExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.LikePredicateSecondExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'LikePredicateSecondExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeLiteralList(MetaTransformScriptModel model)
        {
            foreach (var row in model.LiteralList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'Literal' contains a row with empty Id.");
                row.LiteralType ??= string.Empty;
                row.Value ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeMaxLiteralList(MetaTransformScriptModel model)
        {
            foreach (var row in model.MaxLiteralList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'MaxLiteral' contains a row with empty Id.");
                row.LiteralType ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeMultiPartIdentifierList(MetaTransformScriptModel model)
        {
            foreach (var row in model.MultiPartIdentifierList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'MultiPartIdentifier' contains a row with empty Id.");
                row.Count ??= string.Empty;
            }
        }

        private static void NormalizeMultiPartIdentifierCallTargetList(MetaTransformScriptModel model)
        {
            foreach (var row in model.MultiPartIdentifierCallTargetList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'MultiPartIdentifierCallTarget' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeMultiPartIdentifierCallTargetMultiPartIdentifierLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.MultiPartIdentifierCallTargetMultiPartIdentifierLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'MultiPartIdentifierCallTargetMultiPartIdentifierLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeMultiPartIdentifierIdentifiersItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.MultiPartIdentifierIdentifiersItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'MultiPartIdentifierIdentifiersItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeNamedTableReferenceList(MetaTransformScriptModel model)
        {
            foreach (var row in model.NamedTableReferenceList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'NamedTableReference' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeNamedTableReferenceSchemaObjectLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.NamedTableReferenceSchemaObjectLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'NamedTableReferenceSchemaObjectLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeNamedTableReferenceTableSampleClauseLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.NamedTableReferenceTableSampleClauseLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'NamedTableReferenceTableSampleClauseLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeNextValueForExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.NextValueForExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'NextValueForExpression' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeNextValueForExpressionSequenceNameLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.NextValueForExpressionSequenceNameLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'NextValueForExpressionSequenceNameLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeNullIfExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.NullIfExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'NullIfExpression' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeNullIfExpressionFirstExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.NullIfExpressionFirstExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'NullIfExpressionFirstExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeNullIfExpressionSecondExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.NullIfExpressionSecondExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'NullIfExpressionSecondExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeNullLiteralList(MetaTransformScriptModel model)
        {
            foreach (var row in model.NullLiteralList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'NullLiteral' contains a row with empty Id.");
                row.LiteralType ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeNumericLiteralList(MetaTransformScriptModel model)
        {
            foreach (var row in model.NumericLiteralList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'NumericLiteral' contains a row with empty Id.");
                row.LiteralType ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeOffsetClauseList(MetaTransformScriptModel model)
        {
            foreach (var row in model.OffsetClauseList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'OffsetClause' contains a row with empty Id.");
                row.WithApproximate ??= string.Empty;
            }
        }

        private static void NormalizeOffsetClauseFetchExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.OffsetClauseFetchExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'OffsetClauseFetchExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeOffsetClauseOffsetExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.OffsetClauseOffsetExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'OffsetClauseOffsetExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeOrderByClauseList(MetaTransformScriptModel model)
        {
            foreach (var row in model.OrderByClauseList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'OrderByClause' contains a row with empty Id.");
            }
        }

        private static void NormalizeOrderByClauseOrderByElementsItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.OrderByClauseOrderByElementsItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'OrderByClauseOrderByElementsItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeOverClauseList(MetaTransformScriptModel model)
        {
            foreach (var row in model.OverClauseList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'OverClause' contains a row with empty Id.");
            }
        }

        private static void NormalizeOverClauseOrderByClauseLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.OverClauseOrderByClauseLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'OverClauseOrderByClauseLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeOverClausePartitionsItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.OverClausePartitionsItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'OverClausePartitionsItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeOverClauseWindowFrameClauseLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.OverClauseWindowFrameClauseLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'OverClauseWindowFrameClauseLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeOverClauseWindowNameLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.OverClauseWindowNameLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'OverClauseWindowNameLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeParameterizedDataTypeReferenceList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ParameterizedDataTypeReferenceList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ParameterizedDataTypeReference' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeParameterizedDataTypeReferenceParametersItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ParameterizedDataTypeReferenceParametersItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ParameterizedDataTypeReferenceParametersItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeParameterlessCallList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ParameterlessCallList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ParameterlessCall' contains a row with empty Id.");
                row.ParameterlessCallType ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeParenthesisExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ParenthesisExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ParenthesisExpression' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeParenthesisExpressionExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ParenthesisExpressionExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ParenthesisExpressionExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeParseCallList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ParseCallList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ParseCall' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeParseCallCultureLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ParseCallCultureLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ParseCallCultureLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeParseCallDataTypeLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ParseCallDataTypeLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ParseCallDataTypeLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeParseCallStringValueLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ParseCallStringValueLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ParseCallStringValueLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizePivotedTableReferenceList(MetaTransformScriptModel model)
        {
            foreach (var row in model.PivotedTableReferenceList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'PivotedTableReference' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizePivotedTableReferenceAggregateFunctionIdentifierLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.PivotedTableReferenceAggregateFunctionIdentifierLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'PivotedTableReferenceAggregateFunctionIdentifierLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizePivotedTableReferenceInColumnsItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.PivotedTableReferenceInColumnsItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'PivotedTableReferenceInColumnsItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizePivotedTableReferencePivotColumnLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.PivotedTableReferencePivotColumnLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'PivotedTableReferencePivotColumnLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizePivotedTableReferenceTableReferenceLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.PivotedTableReferenceTableReferenceLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'PivotedTableReferenceTableReferenceLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizePivotedTableReferenceValueColumnsItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.PivotedTableReferenceValueColumnsItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'PivotedTableReferenceValueColumnsItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizePrimaryExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.PrimaryExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'PrimaryExpression' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizePrimaryExpressionCollationLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.PrimaryExpressionCollationLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'PrimaryExpressionCollationLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeQualifiedJoinList(MetaTransformScriptModel model)
        {
            foreach (var row in model.QualifiedJoinList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'QualifiedJoin' contains a row with empty Id.");
                row.JoinHint ??= string.Empty;
                row.QualifiedJoinType ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeQualifiedJoinSearchConditionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.QualifiedJoinSearchConditionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'QualifiedJoinSearchConditionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeQueryDerivedTableList(MetaTransformScriptModel model)
        {
            foreach (var row in model.QueryDerivedTableList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'QueryDerivedTable' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeQueryDerivedTableQueryExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.QueryDerivedTableQueryExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'QueryDerivedTableQueryExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeQueryExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.QueryExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'QueryExpression' contains a row with empty Id.");
            }
        }

        private static void NormalizeQueryExpressionOffsetClauseLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.QueryExpressionOffsetClauseLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'QueryExpressionOffsetClauseLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeQueryExpressionOrderByClauseLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.QueryExpressionOrderByClauseLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'QueryExpressionOrderByClauseLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeQueryParenthesisExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.QueryParenthesisExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'QueryParenthesisExpression' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeQueryParenthesisExpressionQueryExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.QueryParenthesisExpressionQueryExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'QueryParenthesisExpressionQueryExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeQuerySpecificationList(MetaTransformScriptModel model)
        {
            foreach (var row in model.QuerySpecificationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'QuerySpecification' contains a row with empty Id.");
                row.UniqueRowFilter ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeQuerySpecificationFromClauseLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.QuerySpecificationFromClauseLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'QuerySpecificationFromClauseLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeQuerySpecificationGroupByClauseLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.QuerySpecificationGroupByClauseLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'QuerySpecificationGroupByClauseLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeQuerySpecificationHavingClauseLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.QuerySpecificationHavingClauseLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'QuerySpecificationHavingClauseLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeQuerySpecificationSelectElementsItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.QuerySpecificationSelectElementsItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'QuerySpecificationSelectElementsItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeQuerySpecificationTopRowFilterLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.QuerySpecificationTopRowFilterLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'QuerySpecificationTopRowFilterLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeQuerySpecificationWhereClauseLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.QuerySpecificationWhereClauseLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'QuerySpecificationWhereClauseLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeQuerySpecificationWindowClauseLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.QuerySpecificationWindowClauseLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'QuerySpecificationWindowClauseLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeRealLiteralList(MetaTransformScriptModel model)
        {
            foreach (var row in model.RealLiteralList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'RealLiteral' contains a row with empty Id.");
                row.LiteralType ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeRightFunctionCallList(MetaTransformScriptModel model)
        {
            foreach (var row in model.RightFunctionCallList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'RightFunctionCall' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeRightFunctionCallParametersItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.RightFunctionCallParametersItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'RightFunctionCallParametersItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeRollupGroupingSpecificationList(MetaTransformScriptModel model)
        {
            foreach (var row in model.RollupGroupingSpecificationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'RollupGroupingSpecification' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeRollupGroupingSpecificationArgumentsItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.RollupGroupingSpecificationArgumentsItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'RollupGroupingSpecificationArgumentsItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeRowValueList(MetaTransformScriptModel model)
        {
            foreach (var row in model.RowValueList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'RowValue' contains a row with empty Id.");
            }
        }

        private static void NormalizeRowValueColumnValuesItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.RowValueColumnValuesItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'RowValueColumnValuesItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeScalarExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ScalarExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ScalarExpression' contains a row with empty Id.");
            }
        }

        private static void NormalizeScalarSubqueryList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ScalarSubqueryList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ScalarSubquery' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeScalarSubqueryQueryExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ScalarSubqueryQueryExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ScalarSubqueryQueryExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeSchemaObjectFunctionTableReferenceList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SchemaObjectFunctionTableReferenceList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SchemaObjectFunctionTableReference' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeSchemaObjectFunctionTableReferenceParametersItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SchemaObjectFunctionTableReferenceParametersItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SchemaObjectFunctionTableReferenceParametersItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeSchemaObjectFunctionTableReferenceSchemaObjectLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SchemaObjectFunctionTableReferenceSchemaObjectLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SchemaObjectFunctionTableReferenceSchemaObjectLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeSchemaObjectNameList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SchemaObjectNameList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SchemaObjectName' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeSchemaObjectNameBaseIdentifierLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SchemaObjectNameBaseIdentifierLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SchemaObjectNameBaseIdentifierLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeSchemaObjectNameSchemaIdentifierLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SchemaObjectNameSchemaIdentifierLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SchemaObjectNameSchemaIdentifierLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeSearchedCaseExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SearchedCaseExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SearchedCaseExpression' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeSearchedCaseExpressionWhenClausesItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SearchedCaseExpressionWhenClausesItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SearchedCaseExpressionWhenClausesItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeSearchedWhenClauseList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SearchedWhenClauseList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SearchedWhenClause' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeSearchedWhenClauseWhenExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SearchedWhenClauseWhenExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SearchedWhenClauseWhenExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeSelectElementList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SelectElementList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SelectElement' contains a row with empty Id.");
            }
        }

        private static void NormalizeSelectScalarExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SelectScalarExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SelectScalarExpression' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeSelectScalarExpressionColumnNameLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SelectScalarExpressionColumnNameLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SelectScalarExpressionColumnNameLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeSelectScalarExpressionExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SelectScalarExpressionExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SelectScalarExpressionExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeSelectStarExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SelectStarExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SelectStarExpression' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeSelectStarExpressionQualifierLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SelectStarExpressionQualifierLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SelectStarExpressionQualifierLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeSelectStatementList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SelectStatementList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SelectStatement' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeSelectStatementQueryExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SelectStatementQueryExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SelectStatementQueryExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeSimpleCaseExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SimpleCaseExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SimpleCaseExpression' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeSimpleCaseExpressionInputExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SimpleCaseExpressionInputExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SimpleCaseExpressionInputExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeSimpleCaseExpressionWhenClausesItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SimpleCaseExpressionWhenClausesItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SimpleCaseExpressionWhenClausesItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeSimpleWhenClauseList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SimpleWhenClauseList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SimpleWhenClause' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeSimpleWhenClauseWhenExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SimpleWhenClauseWhenExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SimpleWhenClauseWhenExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeSqlDataTypeReferenceList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SqlDataTypeReferenceList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SqlDataTypeReference' contains a row with empty Id.");
                row.SqlDataTypeOption ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeStatementWithCtesAndXmlNamespacesList(MetaTransformScriptModel model)
        {
            foreach (var row in model.StatementWithCtesAndXmlNamespacesList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'StatementWithCtesAndXmlNamespaces' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeStatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeStringLiteralList(MetaTransformScriptModel model)
        {
            foreach (var row in model.StringLiteralList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'StringLiteral' contains a row with empty Id.");
                row.IsLargeObject ??= string.Empty;
                row.IsNational ??= string.Empty;
                row.LiteralType ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeSubqueryComparisonPredicateList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SubqueryComparisonPredicateList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SubqueryComparisonPredicate' contains a row with empty Id.");
                row.ComparisonType ??= string.Empty;
                row.SubqueryComparisonPredicateType ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeSubqueryComparisonPredicateExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SubqueryComparisonPredicateExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SubqueryComparisonPredicateExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeSubqueryComparisonPredicateSubqueryLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.SubqueryComparisonPredicateSubqueryLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SubqueryComparisonPredicateSubqueryLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeTableReferenceList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TableReferenceList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TableReference' contains a row with empty Id.");
            }
        }

        private static void NormalizeTableReferenceWithAliasList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TableReferenceWithAliasList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TableReferenceWithAlias' contains a row with empty Id.");
                row.ForPath ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeTableReferenceWithAliasAliasLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TableReferenceWithAliasAliasLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TableReferenceWithAliasAliasLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeTableReferenceWithAliasAndColumnsList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TableReferenceWithAliasAndColumnsList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TableReferenceWithAliasAndColumns' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeTableReferenceWithAliasAndColumnsColumnsItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TableReferenceWithAliasAndColumnsColumnsItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TableReferenceWithAliasAndColumnsColumnsItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeTableSampleClauseList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TableSampleClauseList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TableSampleClause' contains a row with empty Id.");
                row.System ??= string.Empty;
                row.TableSampleClauseOption ??= string.Empty;
            }
        }

        private static void NormalizeTableSampleClauseRepeatSeedLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TableSampleClauseRepeatSeedLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TableSampleClauseRepeatSeedLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeTableSampleClauseSampleNumberLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TableSampleClauseSampleNumberLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TableSampleClauseSampleNumberLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeTopRowFilterList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TopRowFilterList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TopRowFilter' contains a row with empty Id.");
                row.Percent ??= string.Empty;
                row.WithApproximate ??= string.Empty;
                row.WithTies ??= string.Empty;
            }
        }

        private static void NormalizeTopRowFilterExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TopRowFilterExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TopRowFilterExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeTransformScriptList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TransformScriptList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TransformScript' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'TransformScript' row '{row.Id}' is missing required property 'Name'.");
                row.SourcePath ??= string.Empty;
            }
        }

        private static void NormalizeTransformScriptObjectIdentifierLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TransformScriptObjectIdentifierLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TransformScriptObjectIdentifierLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeTransformScriptSchemaIdentifierLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TransformScriptSchemaIdentifierLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TransformScriptSchemaIdentifierLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeTransformScriptSelectStatementLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TransformScriptSelectStatementLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TransformScriptSelectStatementLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeTransformScriptViewColumnsItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TransformScriptViewColumnsItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TransformScriptViewColumnsItem' contains a row with empty Id.");
                row.Ordinal = RequireText(row.Ordinal, $"Entity 'TransformScriptViewColumnsItem' row '{row.Id}' is missing required property 'Ordinal'.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeTryCastCallList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TryCastCallList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TryCastCall' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeTryCastCallDataTypeLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TryCastCallDataTypeLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TryCastCallDataTypeLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeTryCastCallParameterLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TryCastCallParameterLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TryCastCallParameterLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeTryConvertCallList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TryConvertCallList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TryConvertCall' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeTryConvertCallDataTypeLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TryConvertCallDataTypeLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TryConvertCallDataTypeLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeTryConvertCallParameterLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TryConvertCallParameterLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TryConvertCallParameterLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeTryConvertCallStyleLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TryConvertCallStyleLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TryConvertCallStyleLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeTryParseCallList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TryParseCallList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TryParseCall' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeTryParseCallCultureLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TryParseCallCultureLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TryParseCallCultureLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeTryParseCallDataTypeLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TryParseCallDataTypeLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TryParseCallDataTypeLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeTryParseCallStringValueLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TryParseCallStringValueLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TryParseCallStringValueLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeTSqlStatementList(MetaTransformScriptModel model)
        {
            foreach (var row in model.TSqlStatementList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TSqlStatement' contains a row with empty Id.");
            }
        }

        private static void NormalizeUnaryExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.UnaryExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'UnaryExpression' contains a row with empty Id.");
                row.UnaryExpressionType ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeUnaryExpressionExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.UnaryExpressionExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'UnaryExpressionExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeUnpivotedTableReferenceList(MetaTransformScriptModel model)
        {
            foreach (var row in model.UnpivotedTableReferenceList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'UnpivotedTableReference' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeUnpivotedTableReferenceInColumnsItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.UnpivotedTableReferenceInColumnsItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'UnpivotedTableReferenceInColumnsItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeUnpivotedTableReferencePivotColumnLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.UnpivotedTableReferencePivotColumnLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'UnpivotedTableReferencePivotColumnLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeUnpivotedTableReferenceTableReferenceLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.UnpivotedTableReferenceTableReferenceLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'UnpivotedTableReferenceTableReferenceLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeUnpivotedTableReferenceValueColumnLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.UnpivotedTableReferenceValueColumnLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'UnpivotedTableReferenceValueColumnLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeUnqualifiedJoinList(MetaTransformScriptModel model)
        {
            foreach (var row in model.UnqualifiedJoinList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'UnqualifiedJoin' contains a row with empty Id.");
                row.UnqualifiedJoinType ??= string.Empty;
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeValueExpressionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.ValueExpressionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ValueExpression' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeWhenClauseList(MetaTransformScriptModel model)
        {
            foreach (var row in model.WhenClauseList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'WhenClause' contains a row with empty Id.");
            }
        }

        private static void NormalizeWhenClauseThenExpressionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.WhenClauseThenExpressionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'WhenClauseThenExpressionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeWhereClauseList(MetaTransformScriptModel model)
        {
            foreach (var row in model.WhereClauseList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'WhereClause' contains a row with empty Id.");
            }
        }

        private static void NormalizeWhereClauseSearchConditionLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.WhereClauseSearchConditionLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'WhereClauseSearchConditionLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeWindowClauseList(MetaTransformScriptModel model)
        {
            foreach (var row in model.WindowClauseList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'WindowClause' contains a row with empty Id.");
            }
        }

        private static void NormalizeWindowClauseWindowDefinitionItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.WindowClauseWindowDefinitionItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'WindowClauseWindowDefinitionItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeWindowDefinitionList(MetaTransformScriptModel model)
        {
            foreach (var row in model.WindowDefinitionList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'WindowDefinition' contains a row with empty Id.");
            }
        }

        private static void NormalizeWindowDefinitionOrderByClauseLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.WindowDefinitionOrderByClauseLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'WindowDefinitionOrderByClauseLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeWindowDefinitionPartitionsItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.WindowDefinitionPartitionsItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'WindowDefinitionPartitionsItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeWindowDefinitionRefWindowNameLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.WindowDefinitionRefWindowNameLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'WindowDefinitionRefWindowNameLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeWindowDefinitionWindowFrameClauseLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.WindowDefinitionWindowFrameClauseLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'WindowDefinitionWindowFrameClauseLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeWindowDefinitionWindowNameLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.WindowDefinitionWindowNameLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'WindowDefinitionWindowNameLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeWindowDelimiterList(MetaTransformScriptModel model)
        {
            foreach (var row in model.WindowDelimiterList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'WindowDelimiter' contains a row with empty Id.");
                row.WindowDelimiterType ??= string.Empty;
            }
        }

        private static void NormalizeWindowDelimiterOffsetValueLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.WindowDelimiterOffsetValueLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'WindowDelimiterOffsetValueLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeWindowFrameClauseList(MetaTransformScriptModel model)
        {
            foreach (var row in model.WindowFrameClauseList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'WindowFrameClause' contains a row with empty Id.");
                row.WindowFrameType ??= string.Empty;
            }
        }

        private static void NormalizeWindowFrameClauseBottomLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.WindowFrameClauseBottomLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'WindowFrameClauseBottomLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeWindowFrameClauseTopLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.WindowFrameClauseTopLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'WindowFrameClauseTopLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeWithCtesAndXmlNamespacesList(MetaTransformScriptModel model)
        {
            foreach (var row in model.WithCtesAndXmlNamespacesList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'WithCtesAndXmlNamespaces' contains a row with empty Id.");
            }
        }

        private static void NormalizeWithCtesAndXmlNamespacesCommonTableExpressionsItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.WithCtesAndXmlNamespacesCommonTableExpressionsItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'WithCtesAndXmlNamespacesCommonTableExpressionsItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeWithCtesAndXmlNamespacesXmlNamespacesLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.WithCtesAndXmlNamespacesXmlNamespacesLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'WithCtesAndXmlNamespacesXmlNamespacesLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeXmlNamespacesList(MetaTransformScriptModel model)
        {
            foreach (var row in model.XmlNamespacesList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'XmlNamespaces' contains a row with empty Id.");
            }
        }

        private static void NormalizeXmlNamespacesAliasElementList(MetaTransformScriptModel model)
        {
            foreach (var row in model.XmlNamespacesAliasElementList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'XmlNamespacesAliasElement' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeXmlNamespacesAliasElementIdentifierLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.XmlNamespacesAliasElementIdentifierLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'XmlNamespacesAliasElementIdentifierLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeXmlNamespacesDefaultElementList(MetaTransformScriptModel model)
        {
            foreach (var row in model.XmlNamespacesDefaultElementList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'XmlNamespacesDefaultElement' contains a row with empty Id.");
                row.BaseId ??= string.Empty;
            }
        }

        private static void NormalizeXmlNamespacesElementList(MetaTransformScriptModel model)
        {
            foreach (var row in model.XmlNamespacesElementList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'XmlNamespacesElement' contains a row with empty Id.");
            }
        }

        private static void NormalizeXmlNamespacesElementStringLinkList(MetaTransformScriptModel model)
        {
            foreach (var row in model.XmlNamespacesElementStringLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'XmlNamespacesElementStringLink' contains a row with empty Id.");
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static void NormalizeXmlNamespacesXmlNamespacesElementsItemList(MetaTransformScriptModel model)
        {
            foreach (var row in model.XmlNamespacesXmlNamespacesElementsItemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'XmlNamespacesXmlNamespacesElementsItem' contains a row with empty Id.");
                row.Ordinal ??= string.Empty;
                row.OwnerId ??= string.Empty;
                row.ValueId ??= string.Empty;
            }
        }

        private static Dictionary<string, T> BuildById<T>(
            IEnumerable<T> rows,
            Func<T, string> getId,
            string entityName)
            where T : class
        {
            var rowsById = new Dictionary<string, T>(StringComparer.Ordinal);
            foreach (var row in rows)
            {
                ArgumentNullException.ThrowIfNull(row);
                var id = RequireIdentity(getId(row), $"Entity '{entityName}' contains a row with empty Id.");
                if (!rowsById.TryAdd(id, row))
                {
                    throw new InvalidOperationException($"Entity '{entityName}' contains duplicate Id '{id}'.");
                }
            }

            return rowsById;
        }

        private static T RequireTarget<T>(
            Dictionary<string, T> rowsById,
            string targetId,
            string sourceEntityName,
            string sourceId,
            string relationshipName)
            where T : class
        {
            var normalizedTargetId = RequireIdentity(targetId, $"Relationship '{sourceEntityName}.{relationshipName}' on row '{sourceEntityName}:{sourceId}' is empty.");
            if (!rowsById.TryGetValue(normalizedTargetId, out var target))
            {
                throw new InvalidOperationException($"Relationship '{sourceEntityName}.{relationshipName}' on row '{sourceEntityName}:{sourceId}' points to missing Id '{normalizedTargetId}'.");
            }

            return target;
        }

        private static string ResolveRelationshipId(
            string relationshipId,
            string? navigationId,
            string sourceEntityName,
            string sourceId,
            string relationshipName)
        {
            var normalizedRelationshipId = NormalizeIdentity(relationshipId);
            var normalizedNavigationId = NormalizeIdentity(navigationId);
            if (!string.IsNullOrEmpty(normalizedRelationshipId) &&
                !string.IsNullOrEmpty(normalizedNavigationId) &&
                !string.Equals(normalizedRelationshipId, normalizedNavigationId, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Relationship '{sourceEntityName}.{relationshipName}' on row '{sourceEntityName}:{sourceId}' conflicts between '{normalizedRelationshipId}' and '{normalizedNavigationId}'.");
            }

            var resolvedTargetId = string.IsNullOrEmpty(normalizedRelationshipId)
                ? normalizedNavigationId
                : normalizedRelationshipId;
            return RequireIdentity(resolvedTargetId, $"Relationship '{sourceEntityName}.{relationshipName}' on row '{sourceEntityName}:{sourceId}' is empty.");
        }

        private static string RequireIdentity(string? value, string errorMessage)
        {
            var normalizedValue = NormalizeIdentity(value);
            if (string.IsNullOrEmpty(normalizedValue))
            {
                throw new InvalidOperationException(errorMessage);
            }

            return normalizedValue;
        }

        private static string RequireText(string? value, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException(errorMessage);
            }

            return value;
        }

        private static string NormalizeIdentity(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim();
        }
    }
}
