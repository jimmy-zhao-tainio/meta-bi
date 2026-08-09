#nullable enable

using System.Collections.Generic;

namespace MetaTransformBinding
{
    public sealed partial class MetaTransformBindingModel
    {
        public static MetaTransformBindingModel CreateEmpty() => new();

        public List<Column> ColumnList { get; set; } = new();
        public List<ColumnReference> ColumnReferenceList { get; set; } = new();
        public List<Delete> DeleteList { get; set; } = new();
        public List<InsertQueryWrite> InsertQueryWriteList { get; set; } = new();
        public List<InsertValuesWrite> InsertValuesWriteList { get; set; } = new();
        public List<MergeDelete> MergeDeleteList { get; set; } = new();
        public List<MergeInsertWrite> MergeInsertWriteList { get; set; } = new();
        public List<MergeUpdateWrite> MergeUpdateWriteList { get; set; } = new();
        public List<OutputRowset> OutputRowsetList { get; set; } = new();
        public List<Rowset> RowsetList { get; set; } = new();
        public List<SourceTarget> SourceTargetList { get; set; } = new();
        public List<TableSource> TableSourceList { get; set; } = new();
        public List<TargetColumnReference> TargetColumnReferenceList { get; set; } = new();
        public List<TransformBinding> TransformBindingList { get; set; } = new();
        public List<TransformBindingTarget> TransformBindingTargetList { get; set; } = new();
        public List<Truncate> TruncateList { get; set; } = new();
        public List<UpdateWrite> UpdateWriteList { get; set; } = new();
        public List<Validation> ValidationList { get; set; } = new();
        public List<ValidationSourceColumnLink> ValidationSourceColumnLinkList { get; set; } = new();
        public List<ValidationSourceRowsetLink> ValidationSourceRowsetLinkList { get; set; } = new();
        public List<ValidationTargetColumnLink> ValidationTargetColumnLinkList { get; set; } = new();
        public List<ValidationTargetColumnTypeExact> ValidationTargetColumnTypeExactList { get; set; } = new();
        public List<ValidationTargetColumnTypeSanctionedConversion> ValidationTargetColumnTypeSanctionedConversionList { get; set; } = new();
        public List<ValidationTargetIgnoredColumn> ValidationTargetIgnoredColumnList { get; set; } = new();
        public List<ValidationTargetRowsetLink> ValidationTargetRowsetLinkList { get; set; } = new();
        public List<Write> WriteList { get; set; } = new();
        public List<WriteValue> WriteValueList { get; set; } = new();
        public List<WriteValueScalarExpression> WriteValueScalarExpressionList { get; set; } = new();
    }
}
