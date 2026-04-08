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

public sealed record RuntimeBoundRowset(
    string Id,
    string Name,
    IReadOnlyList<RuntimeBoundColumn> Columns);

public sealed record RuntimeBoundTableSource(
    string SyntaxTableReferenceId,
    string ExposedName,
    string SourceTableId,
    string SchemaName,
    string TableName,
    RuntimeBoundRowset Rowset);

public sealed record BoundColumnReference(
    string SyntaxColumnReferenceId,
    IReadOnlyList<string> IdentifierParts,
    RuntimeBoundColumn ResolvedColumn,
    RuntimeBoundTableSource ResolvedTableSource);

public sealed record BoundScope(
    IReadOnlyList<RuntimeBoundTableSource> VisibleTableSources);

public sealed record BoundTransform(
    string TransformScriptId,
    string TransformScriptName,
    string ActiveLanguageProfileId,
    BoundScope? TopLevelScope,
    RuntimeBoundRowset? TopLevelRowset,
    IReadOnlyList<RuntimeBoundTableSource> BoundTableSources,
    IReadOnlyList<BoundColumnReference> BoundColumnReferences,
    IReadOnlyList<TransformBindingIssue> Issues)
{
    public bool HasErrors => Issues.Count > 0;
}
