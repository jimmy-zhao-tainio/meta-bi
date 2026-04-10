namespace MetaTransform.Binding;

public sealed record TransformBindingIssue(
    string Code,
    string Message,
    string? SyntaxId = null);

public sealed record RuntimeColumn(
    string Id,
    string Name,
    int Ordinal);

public sealed record RuntimeRowsetInput(
    int Ordinal,
    string? InputRole,
    RuntimeRowset Rowset);

public sealed record RuntimeRowset(
    string Id,
    string Name,
    string DerivationKind,
    string? RowsetRole,
    string? SyntaxId,
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
    IReadOnlyList<RuntimeTableSource> VisibleTableSources);

internal sealed record RuntimeTableReferenceBinding(
    RuntimeRowset Rowset,
    IReadOnlyList<RuntimeTableSource> VisibleTableSources);

internal sealed record RuntimeQueryBindingResult(
    BindingScope Scope,
    RuntimeRowset? InputRowset,
    RuntimeRowset OutputRowset);

public sealed record TransformBindingResult(
    string TransformScriptId,
    string TransformScriptName,
    string ActiveLanguageProfileId,
    BindingScope? TopLevelScope,
    RuntimeRowset? TopLevelInputRowset,
    RuntimeRowset? TopLevelRowset,
    IReadOnlyList<RuntimeTableSource> TableSources,
    IReadOnlyList<RuntimeColumnReference> ColumnReferences,
    IReadOnlyList<RuntimeRowset> Rowsets,
    IReadOnlyList<TransformBindingIssue> Issues)
{
    public bool HasErrors => Issues.Count > 0;
}
