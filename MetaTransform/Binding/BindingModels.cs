namespace MetaTransform.Binding;

public enum BoundStatementKind
{
    Unsupported,
    ScalarFunction,
    StoredProcedure,
    Select,
    Insert,
    Update,
    Delete,
    Truncate,
    Merge
}

public sealed record TransformBindingIssue(
    string Code,
    string Message,
    string? MetaTransformScriptEntityId = null);

public sealed record RuntimeColumn(
    string Id,
    string Name,
    int Ordinal,
    RuntimeColumnDataType? DataType = null);

public sealed record RuntimeColumnDataType(
    string MetaDataTypeId,
    bool? IsNullable,
    int? Length,
    int? Precision,
    int? Scale,
    string DisplayName);

public sealed record RuntimeRowsetInput(
    int Ordinal,
    string? InputRole,
    RuntimeRowset Rowset);

public sealed record RuntimeRowset(
    string Id,
    string Name,
    string DerivationKind,
    string? RowsetRole,
    string? MetaTransformScriptEntityId,
    string? SqlIdentifier,
    IReadOnlyList<RuntimeColumn> Columns,
    IReadOnlyList<RuntimeRowsetInput> Inputs);

public sealed record RuntimeTableSource(
    string SyntaxTableReferenceId,
    string ExposedName,
    string SqlIdentifier,
    RuntimeRowset Rowset);

public sealed record RuntimeColumnReference(
    string SyntaxColumnReferenceId,
    IReadOnlyList<string> IdentifierParts,
    RuntimeColumn ResolvedColumn,
    RuntimeTableSource ResolvedTableSource);

public sealed record BindingScope(
    IReadOnlyList<RuntimeTableSource> VisibleTableSources,
    int LocalVisibleTableSourceCount = 0);

internal sealed record RuntimeTableReferenceBinding(
    RuntimeRowset Rowset,
    IReadOnlyList<RuntimeTableSource> VisibleTableSources);

internal sealed record RuntimeQueryBindingResult(
    BindingScope Scope,
    RuntimeRowset? InputRowset,
    RuntimeRowset OutputRowset);

internal sealed record RuntimeWriteValue(
    RuntimeColumn ValueColumn,
    string TargetFieldName,
    string? MetaTransformScriptScalarExpressionId);

internal abstract record RuntimeMutationEffect(
    string TargetSqlIdentifier,
    RuntimeRowset TargetRowset);

internal abstract record RuntimeWriteEffect(
    string TargetSqlIdentifier,
    RuntimeRowset TargetRowset,
    RuntimeRowset ValueRowset,
    IReadOnlyList<RuntimeWriteValue> Values,
    bool RequiresRequiredFieldCoverage)
    : RuntimeMutationEffect(TargetSqlIdentifier, TargetRowset);

internal sealed record RuntimeInsertQueryWriteEffect(
    string TargetSqlIdentifier,
    RuntimeRowset TargetRowset,
    RuntimeRowset ValueRowset,
    IReadOnlyList<RuntimeWriteValue> Values,
    bool RequiresRequiredFieldCoverage,
    string MetaTransformScriptQueryExpressionId)
    : RuntimeWriteEffect(
        TargetSqlIdentifier,
        TargetRowset,
        ValueRowset,
        Values,
        RequiresRequiredFieldCoverage);

internal sealed record RuntimeInsertValuesWriteEffect(
    string TargetSqlIdentifier,
    RuntimeRowset TargetRowset,
    RuntimeRowset ValueRowset,
    IReadOnlyList<RuntimeWriteValue> Values,
    bool RequiresRequiredFieldCoverage,
    string MetaTransformScriptRowValueId)
    : RuntimeWriteEffect(
        TargetSqlIdentifier,
        TargetRowset,
        ValueRowset,
        Values,
        RequiresRequiredFieldCoverage);

internal sealed record RuntimeUpdateWriteEffect(
    string TargetSqlIdentifier,
    RuntimeRowset TargetRowset,
    RuntimeRowset ValueRowset,
    IReadOnlyList<RuntimeWriteValue> Values,
    bool RequiresRequiredFieldCoverage,
    string MetaTransformScriptSetClauseId)
    : RuntimeWriteEffect(
        TargetSqlIdentifier,
        TargetRowset,
        ValueRowset,
        Values,
        RequiresRequiredFieldCoverage);

internal sealed record RuntimeMergeInsertWriteEffect(
    string TargetSqlIdentifier,
    RuntimeRowset TargetRowset,
    RuntimeRowset ValueRowset,
    IReadOnlyList<RuntimeWriteValue> Values,
    bool RequiresRequiredFieldCoverage,
    string MetaTransformScriptMergeInsertActionId)
    : RuntimeWriteEffect(
        TargetSqlIdentifier,
        TargetRowset,
        ValueRowset,
        Values,
        RequiresRequiredFieldCoverage);

internal sealed record RuntimeMergeUpdateWriteEffect(
    string TargetSqlIdentifier,
    RuntimeRowset TargetRowset,
    RuntimeRowset ValueRowset,
    IReadOnlyList<RuntimeWriteValue> Values,
    bool RequiresRequiredFieldCoverage,
    string MetaTransformScriptMergeUpdateActionId)
    : RuntimeWriteEffect(
        TargetSqlIdentifier,
        TargetRowset,
        ValueRowset,
        Values,
        RequiresRequiredFieldCoverage);

internal sealed record RuntimeDeleteEffect(
    string TargetSqlIdentifier,
    RuntimeRowset TargetRowset,
    string MetaTransformScriptDeleteStatementId)
    : RuntimeMutationEffect(TargetSqlIdentifier, TargetRowset);

internal sealed record RuntimeMergeDeleteEffect(
    string TargetSqlIdentifier,
    RuntimeRowset TargetRowset,
    string MetaTransformScriptMergeDeleteActionId)
    : RuntimeMutationEffect(TargetSqlIdentifier, TargetRowset);

internal sealed record RuntimeTruncateEffect(
    string TargetSqlIdentifier,
    RuntimeRowset TargetRowset,
    string MetaTransformScriptTruncateStatementId)
    : RuntimeMutationEffect(TargetSqlIdentifier, TargetRowset);

public sealed record TransformBindingResult(
    string TransformScriptId,
    string TransformScriptName,
    BindingScope? TopLevelScope,
    RuntimeRowset? TopLevelInputRowset,
    RuntimeRowset? TopLevelRowset,
    IReadOnlyList<RuntimeTableSource> TableSources,
    IReadOnlyList<RuntimeColumnReference> ColumnReferences,
    IReadOnlyList<RuntimeRowset> Rowsets,
    IReadOnlyList<TransformBindingIssue> Issues)
{
    internal IReadOnlyList<RuntimeMutationEffect> MutationEffects { get; init; } = [];

    public bool HasErrors => Issues.Count > 0;
}
