namespace MetaTransform.Binding;

public sealed record TransformBindingIssue(
    string Code,
    string Message,
    string? SyntaxId = null);

public sealed record RuntimeBoundColumn(
    string Id,
    string Name,
    int Ordinal,
    string? SourceFieldId,
    string? SourceTableId);

public sealed record RuntimeBoundRowsetInput(
    int Ordinal,
    string? InputRole,
    RuntimeBoundRowset Rowset);

public sealed record RuntimeBoundRowset(
    string Id,
    string Name,
    string DerivationKind,
    string? RowsetRole,
    string? SyntaxId,
    string? SourceTableId,
    IReadOnlyList<RuntimeBoundColumn> Columns,
    IReadOnlyList<RuntimeBoundRowsetInput> Inputs);

public sealed record RuntimeBoundTableSource(
    string SyntaxTableReferenceId,
    string ExposedName,
    string SourceTableId,
    string SchemaName,
    string TableName,
    RuntimeBoundRowset Rowset);

public sealed record RuntimeBoundColumnReference(
    string SyntaxColumnReferenceId,
    IReadOnlyList<string> IdentifierParts,
    RuntimeBoundColumn ResolvedColumn,
    RuntimeBoundTableSource ResolvedTableSource);

public sealed record BoundScope(
    IReadOnlyList<RuntimeBoundTableSource> VisibleTableSources);

internal sealed record RuntimeBoundTableReferenceBinding(
    RuntimeBoundRowset Rowset,
    IReadOnlyList<RuntimeBoundTableSource> VisibleTableSources);

internal sealed record RuntimeQueryBindingResult(
    BoundScope Scope,
    RuntimeBoundRowset? InputRowset,
    RuntimeBoundRowset OutputRowset);

public sealed record BoundTransform(
    string TransformScriptId,
    string TransformScriptName,
    string ActiveLanguageProfileId,
    BoundScope? TopLevelScope,
    RuntimeBoundRowset? TopLevelInputRowset,
    RuntimeBoundRowset? TopLevelRowset,
    IReadOnlyList<RuntimeBoundTableSource> BoundTableSources,
    IReadOnlyList<RuntimeBoundColumnReference> BoundColumnReferences,
    IReadOnlyList<RuntimeBoundRowset> BoundRowsets,
    IReadOnlyList<TransformBindingIssue> Issues)
{
    public bool HasErrors => Issues.Count > 0;
}
