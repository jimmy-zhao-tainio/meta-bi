#nullable enable
using System;
using System.Collections.Generic;

namespace MetaTransformBinding;
public sealed partial class Column
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public Rowset Rowset { get; set; } = null !;
}

public sealed partial class ColumnReference
{
    public string Id { get; set; } = null !;
    public string MetaTransformScriptColumnReferenceId { get; set; } = null !;
    public Column Column { get; set; } = null !;
    public TableSource TableSource { get; set; } = null !;
    public TransformBinding TransformBinding { get; set; } = null !;
}

public sealed partial class Delete
{
    public string Id { get; set; } = null !;
    public string MetaTransformScriptDeleteStatementId { get; set; } = null !;
    public ValidationTargetRowsetLink ValidationTargetRowsetLink { get; set; } = null !;
}

public sealed partial class InsertQueryWrite
{
    public string Id { get; set; } = null !;
    public string MetaTransformScriptQueryExpressionId { get; set; } = null !;
    public Write Write { get; set; } = null !;
}

public sealed partial class InsertValuesWrite
{
    public string Id { get; set; } = null !;
    public string MetaTransformScriptRowValueId { get; set; } = null !;
    public Write Write { get; set; } = null !;
}

public sealed partial class MergeDelete
{
    public string Id { get; set; } = null !;
    public string MetaTransformScriptMergeDeleteActionId { get; set; } = null !;
    public ValidationTargetRowsetLink ValidationTargetRowsetLink { get; set; } = null !;
}

public sealed partial class MergeInsertWrite
{
    public string Id { get; set; } = null !;
    public string MetaTransformScriptMergeInsertActionId { get; set; } = null !;
    public Write Write { get; set; } = null !;
}

public sealed partial class MergeUpdateWrite
{
    public string Id { get; set; } = null !;
    public string MetaTransformScriptMergeUpdateActionId { get; set; } = null !;
    public Write Write { get; set; } = null !;
}

public sealed partial class OutputRowset
{
    public string Id { get; set; } = null !;
    public Rowset Rowset { get; set; } = null !;
    public TransformBinding TransformBinding { get; set; } = null !;
}

public sealed partial class Rowset
{
    public string Id { get; set; } = null !;
    public string DerivationKind { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string? SqlIdentifier { get; set; }
    public TransformBinding TransformBinding { get; set; } = null !;
}

public sealed partial class SourceTarget
{
    public string Id { get; set; } = null !;
    public string? InputRole { get; set; }
    public string Ordinal { get; set; } = null !;
    public Rowset Source { get; set; } = null !;
    public Rowset Target { get; set; } = null !;
}

public sealed partial class TableSource
{
    public string Id { get; set; } = null !;
    public string ExposedName { get; set; } = null !;
    public string MetaTransformScriptTableReferenceId { get; set; } = null !;
    public Rowset Rowset { get; set; } = null !;
    public TransformBinding TransformBinding { get; set; } = null !;
}

public sealed partial class TargetColumnReference
{
    public string Id { get; set; } = null !;
    public string MetaSchemaFieldId { get; set; } = null !;
    public string MetaTransformScriptColumnReferenceId { get; set; } = null !;
    public Column Column { get; set; } = null !;
    public TransformBindingTarget TransformBindingTarget { get; set; } = null !;
}

public sealed partial class TransformBinding
{
    public string Id { get; set; } = null !;
    public string MetaTransformScriptTransformScriptId { get; set; } = null !;
    public string? TransformScriptName { get; set; }
}

public sealed partial class TransformBindingTarget
{
    public string Id { get; set; } = null !;
    public string SqlIdentifier { get; set; } = null !;
    public TransformBinding TransformBinding { get; set; } = null !;
}

public sealed partial class Truncate
{
    public string Id { get; set; } = null !;
    public string MetaTransformScriptTruncateStatementId { get; set; } = null !;
    public ValidationTargetRowsetLink ValidationTargetRowsetLink { get; set; } = null !;
}

public sealed partial class UpdateWrite
{
    public string Id { get; set; } = null !;
    public string MetaTransformScriptSetClauseId { get; set; } = null !;
    public Write Write { get; set; } = null !;
}

public sealed partial class Validation
{
    public string Id { get; set; } = null !;
    public TransformBinding TransformBinding { get; set; } = null !;
}

public sealed partial class ValidationSourceColumnLink
{
    public string Id { get; set; } = null !;
    public string MetaSchemaFieldId { get; set; } = null !;
    public Column Column { get; set; } = null !;
    public ValidationSourceRowsetLink ValidationSourceRowsetLink { get; set; } = null !;
}

public sealed partial class ValidationSourceRowsetLink
{
    public string Id { get; set; } = null !;
    public string MetaSchemaTableId { get; set; } = null !;
    public Rowset Rowset { get; set; } = null !;
    public Validation Validation { get; set; } = null !;
}

public sealed partial class ValidationTargetColumnLink
{
    public string Id { get; set; } = null !;
    public string MetaSchemaFieldId { get; set; } = null !;
    public Column Column { get; set; } = null !;
    public ValidationTargetRowsetLink ValidationTargetRowsetLink { get; set; } = null !;
}

public sealed partial class ValidationTargetColumnTypeExact
{
    public string Id { get; set; } = null !;
    public string SourceMetaDataTypeId { get; set; } = null !;
    public string TargetMetaDataTypeId { get; set; } = null !;
    public ValidationTargetColumnLink ValidationTargetColumnLink { get; set; } = null !;
}

public sealed partial class ValidationTargetColumnTypeSanctionedConversion
{
    public string Id { get; set; } = null !;
    public string SourceMetaDataTypeId { get; set; } = null !;
    public string TargetMetaDataTypeId { get; set; } = null !;
    public ValidationTargetColumnLink ValidationTargetColumnLink { get; set; } = null !;
}

public sealed partial class ValidationTargetIgnoredColumn
{
    public string Id { get; set; } = null !;
    public string MetaSchemaFieldId { get; set; } = null !;
    public ValidationTargetRowsetLink ValidationTargetRowsetLink { get; set; } = null !;
}

public sealed partial class ValidationTargetRowsetLink
{
    public string Id { get; set; } = null !;
    public string MetaSchemaTableId { get; set; } = null !;
    public Rowset Rowset { get; set; } = null !;
    public TransformBindingTarget TransformBindingTarget { get; set; } = null !;
    public Validation Validation { get; set; } = null !;
}

public sealed partial class Write
{
    public string Id { get; set; } = null !;
    public ValidationTargetRowsetLink ValidationTargetRowsetLink { get; set; } = null !;
}

public sealed partial class WriteValue
{
    public string Id { get; set; } = null !;
    public ValidationTargetColumnLink ValidationTargetColumnLink { get; set; } = null !;
    public Write Write { get; set; } = null !;
}

public sealed partial class WriteValueScalarExpression
{
    public string Id { get; set; } = null !;
    public string MetaTransformScriptScalarExpressionId { get; set; } = null !;
    public WriteValue WriteValue { get; set; } = null !;
}

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

public static partial class MetaTransformBindingInstance
{
    private static readonly MetaTransformBindingModel _builtIn = CreateBuiltIn();
    public static MetaTransformBindingModel BuiltIn => _builtIn;

    public static MetaTransformBindingModel CreateBuiltIn()
    {
        var model = MetaTransformBindingModel.CreateEmpty();
        return model;
    }
}